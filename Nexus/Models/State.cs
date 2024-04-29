namespace Nexus.Models
{
    public class State
    {
        public int StateId { get; set; }
        public string Name { get; set; }
        public string Abbreviation { get; set; }
        public decimal TaxRate { get; set; }
        public bool AreIntangiblesTaxable { get; set; }
        public bool AreServicesTaxable { get; set; }
        public int MarketplaceFacilitatorAmount { get; set; }
        public TaxTimespan TaxCalculationTimespan { get; set; }
    }
    public enum TaxTimespan
    {
        PreviousCalendarYear
    }
}
