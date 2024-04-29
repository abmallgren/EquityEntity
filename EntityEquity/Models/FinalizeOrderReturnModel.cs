using EntityEquity.Common;
using EntityEquity.Common.Payment;
using EntityEquity.Data;
namespace EntityEquity.Models
{
    public class FinalizeOrderReturnModel
    {
        public Order Order { get; set; }
        public List<string> EmailAddresses { get; set; }
        public PaymentResult Result { get; set; }
        public bool MustShip { get; set; }
        public bool PaidMustShip
        {
            get
            {
                return Result.Successful && MustShip;
            }
        }
    }
}
