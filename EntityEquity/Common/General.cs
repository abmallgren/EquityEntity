namespace EntityEquity.Common
{
    public class Result
    {
        public bool Successful { get; set; }
        public string? Message { get; set; }
    }
    public enum AccountTabType { Ledger, Pending, Orders }
}
