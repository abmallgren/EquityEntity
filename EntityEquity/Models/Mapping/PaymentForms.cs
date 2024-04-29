using AuthorizeNet.Api.Contracts.V1;
using EntityEquity.Common;
using EntityEquity.Common.Payment;
using EntityEquity.Data.Models.Deserialization.USBank;

namespace EntityEquity.Models.Mapping
{
    public static class PaymentForms
    {
        public static CreditCardPaymentParameters MapCreditCard(CreditCardPaymentFormModel model, List<lineItemType> lineItems, decimal total)
        {
            var creditCard = new creditCardType
            {
                cardNumber = model.CreditCardNumber,
                expirationDate = model.CreditCardExpirationDate,
                cardCode = model.CreditCardSecurityCode
            };
            var billingAddress = new customerAddressType
            {
                firstName = model.BillingFirstName,
                lastName = model.BillingLastName,
                address = model.BillingAddress,
                city = model.BillingCity,
                zip = model.BillingZipCode
            };
            CreditCardPaymentParameters parameters = new()
            {
                CreditCard = creditCard,
                BillingAddress = billingAddress,
                LineItems = lineItems,
                Amount = total
            };
            return parameters;
        }
        public static eCheckPaymentParameters MapECheck(eCheckPaymentFormModel model, decimal total)
        {
            var bankAccount = new bankAccountType
            {
                routingNumber = model.RoutingNumber.ToString(),
                accountNumber = model.AccountNumber.ToString(),
                nameOnAccount = model.NameOnAccount,
                bankName = model.BankName,
                accountType = model.AccountType,
                echeckType = echeckTypeEnum.WEB
            };

            return new eCheckPaymentParameters
            {
                BankAccount = bankAccount,
                Amount = total
            };
        }
        public static eCheckPaymentParameters MapECheck(WithdrawalModel model)
        {
            return MapECheck(model, model.Amount);
        }
        public static eCheckPaymentParameters MapECheck(DepositModel model)
        {
            return MapECheck(model, model.Amount);
        }
        //public static transaction MapAchPament(AchPaymentFormModel model, int orderNumber)
        //{
        //    recipientDetails recipientDetails = new()
        //    {
        //        RecipientType = model.EntityType,
        //        RecipientName = model.NameOnAccount,
        //        RecipientAccountType = model.AccountType,
        //        RecipientAccountNumber = model.AccountNumber,
        //        RecipientRoutingNumber = model.RoutingNumber,
        //        RecipientIdentificationNumber = orderNumber.ToString()
        //    };
        //    rransactionDetails transactionDetails = new()
        //    {
        //        TransactionType = "Payment",
        //        IsWebAuthorized = true,
        //        IsPhoneAuthorized = false,
        //        EffectiveDate = DateTime.Now.ToUniversalTime().ToString("yyyy-MM-dd HH:59:45Z"),
        //        Amount = model.Amount.ToString(),
        //        IsTestTransaction = false
        //    };
        //    transaction transaction = new()
        //    {
        //        RecipientDetails = recipientDetails,
        //        TransactionDetails = transactionDetails
        //    };
        //    return transaction;
        //}
    }
}
