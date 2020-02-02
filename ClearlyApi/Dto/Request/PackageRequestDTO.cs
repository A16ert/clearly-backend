using System;
using System.ComponentModel.DataAnnotations;

namespace clearlyApi.Dto.Request
{
    public class PackageRequestDTO
    {
        [Required]
        public int PackageId { get; set; }

        [Required]
        public int OrderId { get; set; }
    }
}
