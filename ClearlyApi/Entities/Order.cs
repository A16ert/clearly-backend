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

        [DataType(DataType.DateTime)]
        public DateTime Created { get; set; }

        public string BankUrl { get; set; }

        public string BankOrderId { get; set; }

        public OrderStatus Status { get; set; }
    }
}
