using System;
using System.ComponentModel.DataAnnotations;
using ClearlyApi.Enums;

namespace ClearlyApi.Entities
{
    public class Order : PersistantObject
    {
        public int UserId { get; set; }
        public User User { get; set; }

        public int? PackageId { get; set; }
        public Package Package { get; set; }
        
        public Delivery Delivery { get; set; }
        
        [DataType(DataType.DateTime)]
        public DateTime Created { get; set; }

        [DataType(DataType.DateTime)]
        public DateTime Updated { get; set; }
        public OrderStatus Status { get; set; }
    }
}
