using System.ComponentModel.DataAnnotations;

namespace clearlyApi.Dto.Request
{
    public class DeliveryRequestDTO
    {
        [Required]
        public int OrderId { get; set; }

        [Required]
        public string City { get; set; }

        [Required]
        public string Street { get; set; }
        
        [Required]
        public string HouseNumber { get; set; }
        
        [Required]
        public string Apartment { get; set; }
        
        [Required]
        public string PhoneNumber { get; set; }
    }
}