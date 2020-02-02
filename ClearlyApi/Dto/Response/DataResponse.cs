using System;
using System.Collections;
using System.Collections.Generic;

namespace clearlyApi.Dto.Response
{
    public class DataResponse<T> : BaseResponse
    {
        public ICollection<T> Data { get; set; }
    }
}
