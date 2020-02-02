using System;
namespace clearlyApi.Dto.Response
{
    public class BaseResponse
    {
        public bool Status { get; set; } = true;

        public string Message { get; set; } = "";
    }
}
