using System.ComponentModel.DataAnnotations;

namespace EntityEquity.Models
{
    public class CreditCardPaymentFormModel
    {
        [Required]
        public string BillingFirstName { get; set; }
        [Required]
        public string BillingLastName { get; set; }
        [Required]
        [CreditCard]
        public string CreditCardNumber { get; set; }
        [Required]
        [RegularExpression("\\d{4}", ErrorMessage = "Expiration date should be 4 digits long.")]
        public string CreditCardExpirationDate { get; set; }
        [Required]
        public string CreditCardSecurityCode { get; set; }
        [Required]
        public string BillingAddress { get; set; }
        [Required]
        public string BillingCity { get; set; }
        [Required]
        public string BillingZipCode { get; set; }
    }
}
