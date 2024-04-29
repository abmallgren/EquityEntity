using EntityEquity.Data;
using System.ComponentModel.DataAnnotations;

namespace EntityEquity.Models
{
    public class InventoryItemModel
    {
        public InventoryItemModel()
        {
            Inventory = new();
            Name = "";
            SKU = "";
        }
        public Inventory Inventory { get; set; }
        [Required]
        public string Name { get; set; }
        public string SKU { get; set; }
        public decimal Cost { get; set; }
    }
}
