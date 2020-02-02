using System;
using System.ComponentModel.DataAnnotations;

namespace ClearlyApi.Entities
{
    public abstract class PersistantObject
    {
        [Key]
        public int Id { get; set; }
    }
}
