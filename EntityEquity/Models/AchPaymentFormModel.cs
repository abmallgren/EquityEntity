using System.ComponentModel.DataAnnotations;

namespace EntityEquity.Models
{
    public class AchPaymentFormModel
    {
        [Required]
        public string EntityType { get; set; }
        [Required]
        public string NameOnAccount { get; set; }
        [Required]
        public string AccountType { get; set; }
        [Required]
        public string AccountNumber { get; set; }
        [Required]
        public string RoutingNumber { get; set; }
        [Required]
        public decimal Amount { get; set; }
    }
}
