using clearlyApi.Dto;
using clearlyApi.Dto.Response;
using ClearlyApi.Entities;
using ClearlyApi.Enums;

namespace ClearlyApi.Services.Auth
{
    public interface IAuthService
    {
        BaseResponse Register(string login, LoginType loginType);

        BaseResponse Auth(User user);

        SecurityTokenViewModel CreateToken(User user);
    }
}
