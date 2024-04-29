namespace EntityEquity.Models
{
    public class PrepEquityModel
    {
        public string PropertySlug { get; set; }
        public int Shares { get; set; }
        public decimal Price { get; set; }
        public bool MustPurchaseAll { get; set; }
    }
}
