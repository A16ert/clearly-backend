using System;
using System.ComponentModel.DataAnnotations;

namespace clearlyApi.Dto.Request
{
    public class MessageRequest
    {
        [Required]
        public string Text { get; set; }

        /// <summary>
        /// if admin
        /// </summary>
        public string ToUserLogin { get; set; }
    }
}
