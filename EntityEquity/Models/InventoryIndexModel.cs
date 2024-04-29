using EntityEquity.Data;

namespace EntityEquity.Models
{
    public class InventoryIndexModel
    {
        public string BaseAddress { get; set; }
        public string UserId { get; set; }
        public InventoryIndexModel(string baseAddress, string userId)
        {
            BaseAddress = baseAddress;
            UserId = userId;
            Inventories = new();
        }
        public List<Inventory> Inventories { get; set; }
    }
}
