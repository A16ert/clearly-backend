using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ClearlyApi.Enums;

namespace ClearlyApi.Entities
{
    public class Person : PersistantObject
    {
        public string Name { get; set; }

        public string Phone { get; set; }

        public string Email { get; set; }

        public SexType Sex { get; set; }

        public string Age { get; set; }

        public int UserId { get; set; }

        public User User { get; set; }
    }
}
