using System;
using System.ComponentModel.DataAnnotations;

namespace ClearlyApi.Entities
{
    public class Delivery : PersistantObject
    {
        public int OrderId { get; set; }
        public Order Order { get; set; }

        public string City { get; set; }

        public string Street { get; set; }
        
        public string HouseNumber { get; set; }
        
        public string Apartment { get; set; }
        
        public string PhoneNumber { get; set; }
        
        [DataType(DataType.DateTime)]
        public DateTime Created { get; set; }
    }
}