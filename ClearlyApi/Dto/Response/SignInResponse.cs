using System;
namespace clearlyApi.Dto.Response
{
    public class SignInResponse : BaseResponse
    {
        public SecurityTokenViewModel SecurityToken { get; set; }
        public long Id { get; set; }
    }
}
