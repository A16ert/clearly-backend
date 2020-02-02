using ClearlyApi.Entities;
using ClearlyApi.Enums;
using Newtonsoft.Json;

namespace clearlyApi.Dto.Response
{
    public class PackageDTOResponse
    {
        public PackageDTOResponse(Package package)
        {
            Id = package.Id;

            Title = package.Title.Ru;
            Description = package.Description.Ru;

            Price = package.Price;
            Type = package.Type;
        }

        [JsonProperty("id")]
        public int Id { get; set; }
        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("price")]
        public int Price { get; set; }

        [JsonProperty("type")]
        public PackageType Type { get; set; }
    }
}
