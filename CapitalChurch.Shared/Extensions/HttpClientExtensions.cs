using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace CapitalChurch.Shared.Extensions
{
    public static class HttpClientExtensions
    {

        public static Task<dynamic> Get(this HttpClient http, string url) => http.Get<dynamic>(url);

        public static async Task<T> Get<T>(this HttpClient http, string url)
        {
            var response = await http.GetAsync(url);

            response.EnsureSuccessStatusCode();

            var stringBody = await response.Content.ReadAsStringAsync();

            try
            {
                return JsonConvert.DeserializeObject<T>(stringBody);
            }
            catch
            {
                throw new Exception($"Error parsing to type {typeof(T).Name} the value: {stringBody} ");   
            }
        }

        public static Task<TResult> Post<TResult>(this HttpClient http, string url, object obj) => http.Post<object, TResult>(url, obj);

        public static Task<dynamic> Post(this HttpClient http, string url, object obj) =>
            http.Post<object, dynamic>(url, obj);
        public static async Task<TResult> Post<TSource, TResult>(this HttpClient http, string url, TSource obj)
        {
            var response = await http.GetResponsePost(url, obj);

            response.EnsureSuccessStatusCode();

            var stringBody = await response.Content.ReadAsStringAsync();

            return JsonConvert.DeserializeObject<TResult>(stringBody);
        }

        public static async Task<HttpResponseMessage> GetResponsePost<TSource>(this HttpClient http, string url, TSource obj)
        {
            var stringPayload = JsonConvert.SerializeObject(obj);
            var httpContent = new StringContent(stringPayload, Encoding.UTF8, "application/json");

            return await http.PostAsync(url, httpContent);
        }
    }
}