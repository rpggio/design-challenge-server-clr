using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using DCS.Core;
using DCS.ServerRuntime.Framework;
using Newtonsoft.Json;

namespace DCS.ServerRuntime.Services.GitblitApi
{
    /// <summary>
    /// http://gitblit.com/rpc.html
    /// </summary>
    [RegisterComponent]
    public class GitblitClient
    {
        private readonly HttpClient _httpClient;

        public GitblitClient(AppSettings appSettings)
        {
            _httpClient = new HttpClient
            {
                BaseAddress = new Uri(appSettings.Git.RpcUrl)
            };
            var authBytes = Encoding.ASCII.GetBytes("{0}:{1}".FormatFrom(
                appSettings.Git.AdminUser.Username, appSettings.Git.AdminUser.Password));
            _httpClient.DefaultRequestHeaders.Authorization = 
                new AuthenticationHeaderValue("Basic", Convert.ToBase64String(authBytes));
            _httpClient.DefaultRequestHeaders.Accept.Clear();
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        public void CreateRepo(GitblitRepository repo)
        {
            Post(CreatePath("CREATE_REPOSITORY"), repo);
        }

        public void CreateUser(GitblitUser user)
        {
            Post(CreatePath("CREATE_USER"), user);
        }

        public void EditUser(GitblitUser user)
        {
            Post(CreatePath("EDIT_USER", user.username), user);
        }

        public GitblitUser GetUser(string username)
        {
            return Post<GitblitUser>(CreatePath("GET_USER", username), null);
        }

        private void Post(string path, object body)
        {
            ThreadUtil.RunSync(() => PostAsync(path, body));
        }

        private T Post<T>(string path, object body)
        {
            return ThreadUtil.RunSync(() => PostAsync<T>(path, body));
        }

        private T Get<T>(string path)
        {
            return ThreadUtil.RunSync(() => GetAsync<T>(path));
        }

        private async Task PostAsync(string path, object body)
        {
            var json = JsonConvert.SerializeObject(body);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync(path, content);
            response.EnsureSuccessStatusCode();
        }

        private async Task<TResponse> PostAsync<TResponse>(string path, object body)
        {
            var json = JsonConvert.SerializeObject(body);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync(path, content);
            response.EnsureSuccessStatusCode();
            string responseContent = await response.Content.ReadAsStringAsync();
            return await Task.Run(() => JsonConvert.DeserializeObject<TResponse>(responseContent));
        }

        private async Task<T> GetAsync<T>(string path)
        {
            var content = await _httpClient.GetStringAsync(path);
            return await Task.Run(() => JsonConvert.DeserializeObject<T>(content));
        }

        private static string CreatePath(string req, string name = null)
        {
            string path = "/rpc?req={0}".FormatFrom(req);
            if (!string.IsNullOrEmpty(name))
            {
                path += "&name={0}".FormatFrom(WebUtility.UrlEncode(name));
            }
            return path;
        }
    }
}
