using System.ComponentModel.DataAnnotations;

namespace EntityEquity.Models
{
    public class DepositModel : eCheckPaymentFormModel
    {
        [Required]
        [Range(1, 10_000_000_000_000, ErrorMessage = "The amount must be at least 1 and no greater than 10,000,000,000,000.00")]
        public decimal Amount { get; set; }
    }
}
