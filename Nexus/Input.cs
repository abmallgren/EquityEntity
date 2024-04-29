namespace Nexus
{
    public class Input
    {
        public string StateAbbreviation { get; set; }
        public int TotalPropertyShares { get; set; }
        public List<ShareHolder> ShareHolders { get; set; }
        public string SystemCallback { get; set; }
    }
    public class ShareHolder
    {
        public string StateAbbreviation { get; set; }
        public int NumberOfShares { get; set; }
    }
}