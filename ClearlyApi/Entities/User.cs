using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using ClearlyApi.Enums;

namespace ClearlyApi.Entities
{
    public class User : PersistantObject
    {
        [Required]
        public string Login { get; set; }

        public LoginType LoginType { get; set; }

        [Required]
        [DefaultValue(UserType.User)]
        public UserType UserType { get; set; }

        [DefaultValue(false)]
        public bool IsActive { get; set; }

        [DataType(DataType.DateTime)]
        public DateTime Created { get; set; }

        public Person Person { get; set; }

        public ICollection<Message> UserMessages { get; set; }

        public ICollection<Message> AdminMessages { get; set; }

        public ICollection<AccountSession> Sessions { get; set; }
    }
}
