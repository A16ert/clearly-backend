using System;
namespace clearlyApi.Dto
{
    public class SecurityTokenViewModel
    {
        public string Token { get; set; }
        public DateTime ExpireDate { get; set; }
    }
}
