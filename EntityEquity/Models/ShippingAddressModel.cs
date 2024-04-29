using System.ComponentModel.DataAnnotations;

namespace EntityEquity.Models
{
    public class ShippingAddressModel
    {
        public bool SameAsBillingAddress { get; set; }
        public string Name { get; set; }
        public string StreetAddress { get; set; }
        public string City { get; set; }
        public string ZipCode { get; set; }
    }
}
