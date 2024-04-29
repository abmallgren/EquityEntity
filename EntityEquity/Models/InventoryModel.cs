using EntityEquity.Data;
using System.ComponentModel.DataAnnotations;

namespace EntityEquity.Models
{
    public class InventoryModel
    {
        public InventoryModel()
        {
            Name = "";
            InventoryManagers = new();
        }
        public int InventoryId { get; set; }
        [Required]
        public string Name { get; set; }
        public List<InventoryManager> InventoryManagers { get; set; }
    }
}
