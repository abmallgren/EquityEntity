using EntityEquity.Data;
using Microsoft.AspNetCore.Identity;

namespace EntityEquity.Models
{
    public class PropertyIndexModel
    {
        public List<Property>? Properties { get; set; }
        public string UserId { get; set; }
        public PropertyIndexModel(string userId)
        {
            UserId = userId;
        }
    }
}