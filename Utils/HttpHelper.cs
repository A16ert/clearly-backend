using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace Utils
{
    public static class HttpHelper
    {
        public static async Task<HttpResponse> ServicePostRequestJson(string url, string jsonRequest, string token = null)
        {
            using (var client = new HttpClient())
            {
                HttpContent content = new StringContent(jsonRequest);
                content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                if (!string.IsNullOrEmpty(token))
                    content.Headers.Add("Authorization", token);

                client.Timeout = new TimeSpan(0, 0, 60);

                using (var response = await client.PostAsync(url, content))
                {
                    using (var responseContent = response.Content)
                        return new HttpResponse() { Result = await responseContent.ReadAsStringAsync(), Status = (int)response.StatusCode };
                    
                }
            }
        }
    }

    public class HttpResponse
    {
        public string Result { set; get; }

        public int Status { set; get; }
    }
}
