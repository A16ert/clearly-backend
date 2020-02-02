using ClearlyApi.Entities;
using ClearlyApi.Enums;
using ClearlyApi.Enums.Converter;
using Newtonsoft.Json;

namespace clearlyApi.Dto.Response
{
    public class UserResponseDTO
    {
        public UserResponseDTO(User user)
        {
            Id = user.Id;
            Login = user.Login;
            LoginType = LoginTypeConverter.ToString(user.LoginType);
        }

        [JsonProperty("id")]
        public int Id { get; set; }
        [JsonProperty("login")]
        public string Login { get; set; }

        [JsonProperty("loginType")]
        public string LoginType { get; set; }
    }
}