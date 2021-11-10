using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace FilmsParser.Extensions
{
    public static class HttpClientExtensions
    {
        public static async Task<T> PostAsJsonAsyncEx<T>(this HttpClient httpClient, string url, T data)
        {
            var response = await httpClient.PostAsJsonAsync(url, data);

            return await response.Content.ReadFromJsonAsync<T>();
        }

        public static Task<HttpResponseMessage> PutAsJsonAsync<T>(this HttpClient httpClient, string url, T data)
        {
            var dataAsString = JsonConvert.SerializeObject(data);
            var content = new StringContent(dataAsString);
            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

            return httpClient.PutAsync(url, content);
        }

        public static async Task<T> GetAsync<T>(this HttpClient httpClient, string url)
        {
            var response = await httpClient.GetAsync(url);
            var responseContent = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<T>(responseContent);
        }
    }
}