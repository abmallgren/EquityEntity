using EntityEquity.Data;
using EntityEquity.Data.CommonDataSets;
using EntityEquity.Data.Models.Deserialization.USBank;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Security.Claims;

namespace EntityEquity.Common.Payment
{
    public class AchTransaction
    {
        private Token _token { get; set; }
        private IConfiguration _configuration { get; set; }
        private IDbContextFactory<ApplicationDbContext> _dbContextFactory;
        public AchTransaction(IConfiguration configuration, 
            IDbContextFactory<ApplicationDbContext> dbContextFactory)
        {
            _configuration = configuration;
            _dbContextFactory = dbContextFactory;
        }
        private async Task GetToken()
        {
            var UsBankAchApiConfig = _configuration.GetSection("UsBank").Get<UsBankAchApiConfig>();
            var authBaseUrl = UsBankAchApiConfig.AuthApiUrl;
            var client = new HttpClient();
            var bAuthCred = $"{UsBankAchApiConfig.ApiKey}:{UsBankAchApiConfig.Secret}";
            client.DefaultRequestHeaders.Add("Authorization", "Basic " + Convert.ToBase64String(System.Text.ASCIIEncoding.ASCII.GetBytes(bAuthCred)));
            client.BaseAddress = new Uri(authBaseUrl);
            var formData = new List<KeyValuePair<string, string>>();
            formData.Add(new KeyValuePair<string, string>("grant_type", "client_credentials"));
            var request = new HttpRequestMessage(HttpMethod.Post, authBaseUrl + "token") { Content = new FormUrlEncodedContent(formData) };
            HttpResponseMessage response = await client.SendAsync(request);
            response.EnsureSuccessStatusCode();
            string json = await response.Content.ReadAsStringAsync();
            _token = JsonConvert.DeserializeObject<Token>(json);
        }
        public async Task<UsBankAchApiResponse> SubmitTransaction(recipientDetails recipientDetails, string transactionType, decimal amount, int innerId)
        {
            if (_token is null)
            {
                await GetToken();
            }
            var response = await GetAchApiResponse(recipientDetails, transactionType, amount, innerId);
            if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                await GetToken();
                response = await GetAchApiResponse(recipientDetails, transactionType, amount, innerId);
            }
            response.EnsureSuccessStatusCode();
            var deserializedResponse = await response.Content.ReadFromJsonAsync<UsBankAchApiResponse>();
            return deserializedResponse;
        }
        public async Task<Transaction> GetTransaction(string id)
        {
            if (_token is null)
            {
                await GetToken();
            }
            var response = await RequestTransaction(id);
            if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                await GetToken();
                response = await RequestTransaction(id);
            }
            response.EnsureSuccessStatusCode();
            var deserializedResponse = await response.Content.ReadFromJsonAsync<Transaction>();
            return deserializedResponse;
        }
        private async Task<HttpResponseMessage> RequestTransaction(string id)
        {
            var USBankConfigSection = _configuration.GetSection("UsBank");
            var apiBaseUrl = USBankConfigSection.GetValue<string>("AchApiUrl");
            var client = new HttpClient();
            client.BaseAddress = new Uri(apiBaseUrl);
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {_token.AccessToken}");
            var stringContent = new StringContent("");
            stringContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");
            return await client.SendAsync(GetRequestMessage(apiBaseUrl + $"transactions/{id}", stringContent, HttpMethod.Get));
        }
        private async Task<HttpResponseMessage> GetAchApiResponse(recipientDetails recipientDetails, string transactionType, decimal amount, int innerId)
        {
            var USBankConfigSection = _configuration.GetSection("UsBank");
            var apiBaseUrl = USBankConfigSection.GetValue<string>("AchApiUrl");
            var transaction = MakeTransaction(recipientDetails, transactionType, amount, innerId);
            var serializedTransaction = JsonConvert.SerializeObject(transaction);
            var client = new HttpClient();
            client.BaseAddress = new Uri(apiBaseUrl);
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {_token.AccessToken}");
            var stringContent = new StringContent(serializedTransaction);
            stringContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");
            return await client.SendAsync(GetRequestMessage(apiBaseUrl + "transactions/domestic", stringContent, HttpMethod.Post));
        }
        private HttpRequestMessage GetRequestMessage(string url, StringContent content, HttpMethod method)
        {
            HttpRequestMessage request = new HttpRequestMessage();
            request.Method = method;
            request.RequestUri = new Uri(url);
            request.Content = content;
            request.Headers.Add("Accept", "application/json");
            request.Headers.Add("Accept-Encoding", "*");
            request.Headers.Add("Correlation-ID", Guid.NewGuid().ToString());
            request.Headers.Add("Idempotency-Key", Guid.NewGuid().ToString());
            return request;
        }
        public object MakeTransaction(recipientDetails recipientDetails, string transactionType, decimal amount, int innerId)
        {
            var config = _configuration.GetSection("UsBank:RequestorDetails").Get<UsBankRequestorConfig>();
            communications communications = new()
            {
                CommentsForRecipients = "EE"
            };
            transactionDetails transactionDetails = new()
            {
                TransactionType = transactionType,
                StandardEntryClassCode = "CCD",
                IsWebAuthorized = true,
                IsPhoneAuthorized = false,
                EffectiveDate = DateTime.Now.ToUniversalTime().ToString("yyyy-MM-dd HH:59:45Z"),
                Amount = amount.ToString(),
                IsTestTransaction = false
            };
            requestorDetails requestorDetails = new()
            {
                CompanyName = config.CompanyName,
                CompanyID = config.CompanyID,
                CompanyNotes = config.CompanyNotes,
                CompanyDescriptiveDate = DateTime.Now.ToUniversalTime().ToString("yyMMdd"),
                DiscretionaryData = config.DiscretionaryData
            };
            clientDetails clientDetails = new()
            {
                ClientID = "6779ef20e75817b79602",
                ClientRequestID = $"EE-{innerId}"
            };
            transaction transaction = new()
            {
                ClientDetails = clientDetails,
                RequestorDetails = requestorDetails,
                RecipientDetails = recipientDetails,
                TransactionDetails = transactionDetails,
                Communications = communications
            };
            var wrapper = new
            {
                transaction = transaction
            };
            return wrapper;
        }
    }
    public class UsBankAchApiConfig
    {
        public string AchApiUrl { get; set; }
        public string AuthApiUrl { get; set; }
        public string ApiKey { get; set; }
        public string Secret { get; set; }
    }
    public class UsBankRequestorConfig
    {
        public string CompanyName { get; set; }
        public string CompanyID { get; set; }
        public string CompanyNotes { get; set; }
        public string DiscretionaryData { get; set; }
    }
    public class UsBankAchApiResponse
    {
        public clientDetails clientDetails { get; set; }
        public string TransactionID { get; set; }
        public string TransactionStatus { get; set; }
        public UsBankAchApiResponseWarnings Warning { get; set; }
    }
    public class UsBankAchApiResponseWarnings
    {
        public string Code { get; set; }
        public string Message { get; set; }
        public UsBankAchApiResponseWarningsDetails Details { get; set; }
    }
    public class UsBankAchApiResponseWarningsDetails
    {
        public string AttributeName { get; set; }
        public string Reason { get; set; }
    }
}
