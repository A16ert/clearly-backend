using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ClearlyApi.Entities
{
    public class Notification : PersistantObject
    {
        public int UserId { get; set; }
        public User User { get; set; }
        
        public string Text { get; set; }
        
        public Enums.NotificationType Type { get; set; }
        
        [DataType(DataType.DateTime)]
        public DateTime Created { get; set; }
    }
}