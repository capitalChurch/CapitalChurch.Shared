using System;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace CapitalChurch.Shared.Extensions
{
    public static class HttpClientExtensions
    {

        public static Task<dynamic> Get(this HttpClient http, string url) => http.Get<dynamic>(url);

        public static Task<T> Get<T>(this HttpClient http, string url) =>
            HandleHttpCall<T>(http.GetAsync(url));

        public static Task<TResult> Post<TResult>(this HttpClient http, string url, object obj) => http.Post<object, TResult>(url, obj);

        public static Task<dynamic> Post(this HttpClient http, string url, object obj) =>
            http.Post<object, dynamic>(url, obj);
        
        public static Task<TResult> Post<TSource, TResult>(this HttpClient http, string url, TSource obj) =>
            HandleHttpCall<TResult>(http.GetResponsePost(url, obj));

        public static async Task<HttpResponseMessage> GetResponsePost<TSource>(this HttpClient http, string url, TSource obj)
        {
            var stringPayload = JsonConvert.SerializeObject(obj);
            var httpContent = new StringContent(stringPayload, Encoding.UTF8, "application/json");

            return await http.PostAsync(url, httpContent);
        }

        private static async Task<TResult> HandleHttpCall<TResult>(Task<HttpResponseMessage> taskResponse)
        {
            var response = await taskResponse;

            var stringBody = await response.Content.ReadAsStringAsync();
            
            if(!response.IsSuccessStatusCode)
                throw new Exception($"Error on call: {stringBody}");

            try
            {
                return JsonConvert.DeserializeObject<TResult>(stringBody);
            }
            catch
            {
                throw new Exception($"Error parsing to type {typeof(TResult).Name} the value: {stringBody} ");   
            }
        }
    }
}