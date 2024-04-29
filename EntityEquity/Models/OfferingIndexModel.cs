using EntityEquity.Data;
namespace EntityEquity.Models
{
    public class OfferingIndexModel
    {
        public string UserId { get; set; }
        public OfferingIndexModel(string userId)
        {
            UserId = userId;
        }
    }
}
