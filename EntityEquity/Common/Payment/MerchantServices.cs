using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using AuthorizeNet.Api.Controllers;
using AuthorizeNet.Api.Contracts.V1;
using AuthorizeNet.Api.Controllers.Bases;
using Microsoft.EntityFrameworkCore;
using EntityEquity.Data;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using EntityEquity.Data.Models.Deserialization;
using EntityEquity.Data.Models.Deserialization.USBank;
using Newtonsoft.Json;

namespace EntityEquity.Common.Payment
{
    public class MerchantServices
    {
        private IConfiguration _configuration;
        private IDbContextFactory<ApplicationDbContext> _dbContextFactory;
        private UserManager<IdentityUser> _userManager;
        private ClaimsPrincipal _user;
        public MerchantServices(IConfiguration configuration, 
            IDbContextFactory<ApplicationDbContext> dbContextFactory, 
            UserManager<IdentityUser> userManager,
            ClaimsPrincipal user)
        {
            _configuration = configuration;
            _dbContextFactory = dbContextFactory;
            _userManager = userManager;
            _user = user;
        }
        private void SetEnvironmentAndMerchant()
        {
            IConfigurationSection authorizeNetConfig = _configuration.GetSection("AuthorizeNet");
            var apiLoginId = authorizeNetConfig.GetValue<string>("ApiLoginId");
            var apiTransactionKey = authorizeNetConfig.GetValue<string>("ApiTransactionKey");
            ApiOperationBase<ANetApiRequest, ANetApiResponse>.RunEnvironment = AuthorizeNet.Environment.SANDBOX;

            // define the merchant information (authentication / transaction id)
            ApiOperationBase<ANetApiRequest, ANetApiResponse>.MerchantAuthentication = new merchantAuthenticationType()
            {
                name = apiLoginId,
                ItemElementName = ItemChoiceType.transactionKey,
                Item = apiTransactionKey
            };
        }
        public async Task<PaymentResult> DepositViaECheck(eCheckPaymentParameters parameters)
        {
            try
            {
                SetEnvironmentAndMerchant();
                var paymentType = new paymentType { Item = parameters.BankAccount };
                var transactionRequest = new transactionRequestType
                {
                    transactionType = transactionTypeEnum.authCaptureTransaction.ToString(),
                    payment = paymentType,
                    amount = parameters.Amount
                };
                return await ProcessPayment(transactionRequest);
            }
            catch(Exception ex)
            {
                
            }
            return null;
        }
        public async Task<PaymentResult> WithdrawalViaECheck(eCheckPaymentParameters parameters)
        {
            try
            {
                SetEnvironmentAndMerchant();
                var paymentType = new paymentType { Item = parameters.BankAccount };
                var transactionRequest = new transactionRequestType
                {
                    transactionType = transactionTypeEnum.refundTransaction.ToString(),
                    payment = paymentType,
                    amount = parameters.Amount
                };
                return await ProcessPayment(transactionRequest);
            }
            catch(Exception ex)
            {
                
            }
            return null;
        }
        private async Task<PaymentResult> ProcessPayment(transactionRequestType transactionRequest)
        {
            Data.CommonDataSets.Orders orderDataSet = new(_dbContextFactory, _userManager, _user);
            var order = orderDataSet.GetIncompleteOrder();

            var request = new createTransactionRequest
            {
                transactionRequest = transactionRequest,
                refId = $"Order #{order.OrderId}"
            };

            var controller = new createTransactionController(request);
            controller.Execute();
            var response = controller.GetApiResponse();

            var result = await ProcessResponse(response, order);

            if (response is null)
            {
                result.Error = ProcessServerError(order, controller);
            }
            return result;
        }
        public async Task<PaymentResult> RunCard(CreditCardPaymentParameters parameters)
        {
            SetEnvironmentAndMerchant();

            var creditCard = parameters.CreditCard;
            var billingAddress = parameters.BillingAddress;
            var paymentType = new paymentType { Item = creditCard };
            var lineItems = parameters.LineItems.ToArray();
            var transactionRequest = new transactionRequestType
            {
                transactionType = transactionTypeEnum.authCaptureTransaction.ToString(),
                amount = parameters.Amount,
                payment = paymentType,
                billTo = billingAddress,
                lineItems = lineItems
            };
            return await ProcessPayment(transactionRequest);
        }
        private PaymentTransactionError? ProcessServerError(Order order, createTransactionController controller)
        {
            var errorResponse = controller.GetErrorResponse();
            PaymentTransactionError error = new PaymentTransactionError()
            {
                Order = order,
                ErrorCode = errorResponse.messages.resultCode.ToString(),
                ErrorMessage = errorResponse.messages.message.ToString()
            };
            LogPaymentTransactionError(error);
            return error;
        }
        private async Task LogPaymentTransactionError(PaymentTransactionError error)
        {
            using (var dbContext = _dbContextFactory.CreateDbContext())
            {
                dbContext.Orders.Attach(error.Order);
                dbContext.PaymentTransactionErrors.Add(error);
                await dbContext.SaveChangesAsync();
            }
        }
        private async Task<PaymentResult> ProcessResponse(createTransactionResponse response, Order order)
        {
            PaymentResult result = new();
            result.Successful = false;
            PaymentTransactionError error = new PaymentTransactionError()
            {
                Order = order,
                ErrorCode = "Unknown",
                ErrorMessage = "Transaction failed."
            };
            using (var dbContext = _dbContextFactory.CreateDbContext())
            {
                dbContext.Orders.Attach(error.Order);
                if (response is not null)
                {
                    if (response.messages.resultCode == messageTypeEnum.Ok)
                    {
                        if (response.transactionResponse.messages is not null)
                        {
                            PaymentTransaction paymentTransaction = new PaymentTransaction()
                            {
                                Order = order,
                                TransactionId = response.transactionResponse.transId,
                                ResponseCode = response.transactionResponse.responseCode,
                                MessageCode = response.transactionResponse.messages[0].code,
                                Description = response.transactionResponse.messages[0].description,
                                AuthorizationCode = response.transactionResponse.authCode
                            };
                            dbContext.Orders.Attach(paymentTransaction.Order);
                            dbContext.PaymentTransactions!.Add(paymentTransaction);
                            result.Successful = true;
                        }
                        else
                        {
                            if (response.transactionResponse.errors != null)
                            {
                                error.ErrorCode = response.transactionResponse.errors[0].errorCode;
                                error.ErrorMessage = response.transactionResponse.errors[0].errorText;
                            }
                            dbContext.PaymentTransactionErrors.Add(error);
                        }
                    }
                    else
                    {
                        if (response.transactionResponse != null && response.transactionResponse.errors != null)
                        {
                            error.ErrorCode = response.transactionResponse.errors[0].errorCode;
                            error.ErrorMessage = response.transactionResponse.errors[0].errorText;
                        }
                        else
                        {
                            error.ErrorCode = response.messages.message[0].code;
                            error.ErrorMessage = response.messages.message[0].text;
                        }
                        dbContext.PaymentTransactionErrors.Add(error);
                    }
                }
                else
                {
                    error.ErrorCode = "Null";
                    error.ErrorMessage = "Response was null.";

                    dbContext.PaymentTransactionErrors.Add(error);
                }
                await dbContext.SaveChangesAsync();
            }
            if (!result.Successful)
                result.Error = error;
            return result;
        }

    }
    public class CreditCardPaymentParameters : PaymentParameters
    {
        public creditCardType CreditCard { get; set; }
        public customerAddressType BillingAddress { get; set; }
        public List<lineItemType> LineItems { get; set; }
    }
    public class eCheckPaymentParameters : PaymentParameters
    {
        public bankAccountType BankAccount { get; set; }
    }

    public class PaymentParameters
    {
        public decimal Amount { get; set; }
    }
    public class PaymentResult
    {
        public bool Successful { get; set; }
        public PaymentTransactionError? Error { get; set; }
    }
    public class PaymentRegister
    {
        public List<PropertyBook> Books = new();
        public decimal GetDeductions(int propertyId)
        {
            return (from r in Books
                          where r.PropertyId == propertyId
                          select r.Deductions).FirstOrDefault();
        }
        public decimal GetRemaining(int propertyId)
        {
            return (from r in Books
                    where r.PropertyId == propertyId
                    select r.Amount - r.Deductions).FirstOrDefault();
        }
        public void Deduct(int propertyId, decimal deduction)
        {
            var record = (from r in Books
                          where r.PropertyId == propertyId
                          select r).FirstOrDefault();
            record.Deductions += deduction;
        }
    }
    public class PropertyBook
    {
        public int PropertyId;
        public decimal Amount;
        public decimal Deductions;
    }

}
