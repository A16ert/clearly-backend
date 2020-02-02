using System;
using System.Collections.Generic;
using System.Linq;
using clearlyApi.Dto.Response;
using ClearlyApi.Entities;
using ClearlyApi.Enums;
using Newtonsoft.Json;

namespace ClearlyApi.Services.Chat
{
    public class MessageDTO
    {
        public MessageDTO(Message message)
        {
            Type = message.Type;
            IsAdmin = message.IsFromAdmin;
            Created = message.Created;
            Data = message.Content;
        }

        public MessageDTO(Message message, List<Package> packages)
        {
            Type = message.Type;
            IsAdmin = message.IsFromAdmin;
            Created = message.Created;

            Int32.TryParse(message.Content, out int orderId);

            Data = JsonConvert.SerializeObject(
                    new PackagesList()
                    {
                        OrderId = orderId,
                        Packages = packages.Select(x => new PackageDTOResponse(x)).ToList()
                    });

        }

        [JsonProperty("isAdmin")]
        public bool IsAdmin { get; set; }
        [JsonProperty("type")]
        public MessageType Type { get; set; }
        [JsonProperty("data")]
        public string Data { get; set; }
        [JsonProperty("created")]
        public DateTime Created { get; set; }
    }

    public class PackagesList
    {
        [JsonProperty("packages")]
        public List<PackageDTOResponse> Packages { get; set; }
        [JsonProperty("orderId")]
        public int OrderId { get; set; }
    }
}
