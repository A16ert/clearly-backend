using ClearlyApi.Entities;
using ClearlyApi.Enums;
using ClearlyApi.Enums.Converter;
using Newtonsoft.Json;
using Org.BouncyCastle.Utilities.Collections;

namespace clearlyApi.Dto.Response
{
    public class NotificationResponseDto
    {
        public NotificationResponseDto(Notification model)
        {
            Id = model.Id;
            
            Type = model.Type;
            TypeText = NotificationTypeConverter.ToString(model.Type);

            Text = model.Text;
            
            Login = model.User.Login;
            UserId = model.User.Id;
        }
        
        [JsonProperty("id")]
        public int Id { get; set; }
        
        [JsonProperty("userId")]
        public int UserId { get; set; }
        
        [JsonProperty("login")]
        public string Login { get; set; }
        
        [JsonProperty("type")]
        public NotificationType Type { get; set; }
        
        [JsonProperty("typeText")]
        public string TypeText { get; set; }
        
        [JsonProperty("text")]
        public string Text { get; set; }
        
        
    }
}