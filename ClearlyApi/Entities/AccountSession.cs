using System;
using System.ComponentModel.DataAnnotations;

namespace ClearlyApi.Entities
{
    public class AccountSession : PersistantObject
    {
        public int UserId { get; set; }
        public User User { get; set; }

        public string Token { get; set; }

        [DataType(DataType.DateTime)]
        public DateTime Created { get; set; }

        [DataType(DataType.DateTime)]
        public DateTime ExpiredAt { get; set; }
    }
}
