using System;
using System.ComponentModel.DataAnnotations;

namespace ClearlyApi.Entities
{
    public class ActivationCode : PersistantObject
    {
        public int UserId { get; set; }
        public User User { get; set; }

        public string Code { get; set; }

        [DataType(DataType.DateTime)]
        public DateTime Created { get; set; }
    }
}
