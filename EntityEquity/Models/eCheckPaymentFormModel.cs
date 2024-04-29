using AuthorizeNet.Api.Contracts.V1;
using System.ComponentModel.DataAnnotations;

namespace EntityEquity.Models
{
    public class eCheckPaymentFormModel
    {
        [Required]
        public bankAccountTypeEnum AccountType { get; set; }
        [Required]
        public string NameOnAccount { get; set; }
        [Required]
        public string RoutingNumber { get; set; }
        [Required]
        public string AccountNumber { get; set; }
        [Required]
        public string BankName { get; set; }
    }
}
