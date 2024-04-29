using EntityEquity.Data;

namespace EntityEquity.Models
{
    public class HomeIndexModel
    {
        public List<PropertyPerformance> Performances { get; set; }
    }
    public class PropertyPerformance
    {
        public Property Property { get; set; }
        public int SharesOwned { get; set; }
        public int SharesForSale { get; set; }
        public decimal SalesInLastWeek { get; set; }
        public decimal ProfitInLastWeek { get; set; }
    }
}
