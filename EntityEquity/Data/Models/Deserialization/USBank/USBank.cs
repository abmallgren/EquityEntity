using Newtonsoft.Json;

namespace EntityEquity.Data.Models.Deserialization.USBank
{
    public class transaction
    {
        [JsonProperty("clientDetails")]
        public clientDetails ClientDetails { get; set; }
        [JsonProperty("requestorDetails")]
        public requestorDetails RequestorDetails { get; set; }
        [JsonProperty("recipientDetails")]
        public recipientDetails RecipientDetails { get; set; }
        [JsonProperty("transactionDetails")]
        public transactionDetails TransactionDetails { get; set; }
        [JsonProperty("communications")]
        public communications Communications { get; set; }
    }
    public class clientDetails
    {
        [JsonProperty("clientID")]
        public string ClientID { get; set; }
        [JsonProperty("clientRequestID")]
        public string ClientRequestID { get; set; }
    }
    public class requestorDetails
    {
        [JsonProperty("companyName")]
        public string CompanyName { get; set; }
        [JsonProperty("companyID")]
        public string CompanyID { get; set; }
        [JsonProperty("companyNotes")]
        public string CompanyNotes { get; set; }
        [JsonProperty("companyDescriptionDate")]
        public string CompanyDescriptiveDate { get; set; }
        [JsonProperty("discretionaryData")]
        public string DiscretionaryData { get; set; }
    }
    public class recipientDetails
    {
        [JsonProperty("recipientType")]
        public string RecipientType { get; set; }
        [JsonProperty("recipientName")]
        public string RecipientName { get; set; }
        [JsonProperty("recipientAccountType")]
        public string RecipientAccountType { get; set; }
        [JsonProperty("recipientAccountNumber")]
        public string RecipientAccountNumber { get; set; }
        [JsonProperty("recipientRoutingNumber")]
        public string RecipientRoutingNumber { get; set; }
        [JsonProperty("recipientIdentificationNumber")]
        public string RecipientIdentificationNumber { get; set; }
    }
    public class transactionDetails
    {
        [JsonProperty("transactionType")]
        public string TransactionType { get; set; }
        [JsonProperty("standardEntryClassCode")]
        public string StandardEntryClassCode { get; set; }
        [JsonProperty("isWebAuthorized")]
        public bool IsWebAuthorized { get; set; }
        [JsonProperty("isPhoneAuthorized")]
        public bool IsPhoneAuthorized { get; set; }
        [JsonProperty("effectiveDate")]
        public string EffectiveDate { get; set; }
        [JsonProperty("amount")]
        public string Amount { get; set; }
        [JsonProperty("isTestTransaction")]
        public bool IsTestTransaction { get; set; }
    }
    public class communications
    {
        [JsonProperty("commentsForRecipients")]
        public string CommentsForRecipients { get; set; }
        [JsonProperty("remittanceRecords")]
        public string[] RemittanceRecords { get; set; }
    }
    public class achReturn
    {
        [JsonProperty("code")]
        public string Code { get; set; }
        [JsonProperty("returnReason")]
        public string ReturnReason { get; set; }
    }

    public class Token
    {
        public string TokenType { get; set; }
        public string ExpiresIn { get; set; }
        public string AccessToken { get; set; }
    }
    
    public class Transaction
    {
        [JsonProperty("clientDetails")]
        public clientDetails clientDetails { get; set; }
        [JsonProperty("transactionID")]
        public string TransactionID { get; set; }
        [JsonProperty("transactionStatus")]
        public string TransactionStatus { get; set; }
        [JsonProperty("companyNotes")]
        public string companyNotes { get; set; }
        [JsonProperty("commentsForRecipients")]
        public string CommentsForRecipients { get; set; }
        [JsonProperty("companyID")]
        public string CompanyId { get; set; }
        [JsonProperty("recipientName")]
        public string RecipientName { get; set; }
        [JsonProperty("recipientIdentificationNumber")]
        public string RecipientIdentificationNumber { get; set; }
        [JsonProperty("transactionDetails")]
        public transactionDetails transactionalDetails { get; set; }
        [JsonProperty("achReturn")]
        public achReturn achReturn { get; set; }
    }
}
