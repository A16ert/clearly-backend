using System;
using System.ComponentModel.DataAnnotations;
using ClearlyApi.Enums;

namespace clearlyApi.Dto.Request
{
    public class AuthRequest
    {
        [Required]
        public string Login { get; set; }

        public LoginType Type { get; set; }

        public string Code { get; set; }
    }
}
