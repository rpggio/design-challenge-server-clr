using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace DCS.Core.Net
{
    /// <summary>
    /// Meh, not sure about these
    /// </summary>
    public static class HttpClientUtil
    {
        public static Task<T> GetObjectAsync<T>(this HttpClient client, string url) where T : new()
        {
            return GetObjectAsync<T>(client, url, CancellationToken.None);
        }

        public static async Task<T> GetObjectAsync<T>(this HttpClient client, string url, CancellationToken cancellationToken)
            where T : new()
        {
            var response = await client.GetAsync(url, cancellationToken);
            string responseString = await response.Content.ReadAsStringAsync();
            var result = await Task<T>.Factory.StartNew(() =>
                JsonConvert.DeserializeObject<T>(responseString));
            return result;
        }

        public static Task<T> GetObjectAsync<T>(this HttpClient client, string baseUrl, Dictionary<string, object> parameters)
            where T : new()
        {
            return GetObjectAsync<T>(client, AddUrlParams(baseUrl, parameters));
        }

        public static Task<T> GetObjectAsync<T>(this HttpClient client, string baseUrl, Dictionary<string, object> parameters,
            CancellationToken cancellationToken) where T : new()
        {
            return GetObjectAsync<T>(client, AddUrlParams(baseUrl, parameters), cancellationToken);
        }

        public static Task<T> PostObjectAsync<T>(this HttpClient client, string url, object postContent) where T : new()
        {
            return PostObjectAsync<T>(client, url, postContent, CancellationToken.None);
        }

        public static async Task<T> PostObjectAsync<T>(this HttpClient client, string url, object postContent,
            CancellationToken cancellationToken) where T : new()
        {
            string serializedContent = JsonConvert.SerializeObject(postContent);
            HttpContent httpContent = new StringContent(serializedContent, Encoding.UTF8, "application/json");
            var response = await client.PostAsync(url, httpContent, cancellationToken);
            string responseString = await response.Content.ReadAsStringAsync();
            var result = await Task<T>.Factory.StartNew(() => JsonConvert.DeserializeObject<T>(responseString));
            return result;
        }

        public static Task<T> PostObjectAsync<T>(this HttpClient client, string baseUrl, Dictionary<string, object> parameters,
            object postContent) where T : new()
        {
            return PostObjectAsync<T>(client, AddUrlParams(baseUrl, parameters), postContent);
        }

        public static Task<T> PostObjectAsync<T>(this HttpClient client, string baseUrl, Dictionary<string, object> parameters,
            object postContent, CancellationToken cancellationToken) where T : new()
        {
            return PostObjectAsync<T>(client, AddUrlParams(baseUrl, parameters), postContent, cancellationToken);
        }

        private static string AddUrlParams(string baseUrl, Dictionary<string, object> parameters)
        {
            var stringBuilder = new StringBuilder(baseUrl);
            bool hasFirstParam = baseUrl.Contains("?");

            foreach (var parameter in parameters)
            {
                string format = hasFirstParam ? "&{0}={1}" : "?{0}={1}";
                stringBuilder.AppendFormat(format, Uri.EscapeDataString(parameter.Key),
                    Uri.EscapeDataString(parameter.Value.ToString()));
                hasFirstParam = true;
            }

            return stringBuilder.ToString();
        }
    }
}