using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;
using System.Net;

namespace EntityEquity.Common
{
    public class CookieBridgeConnection
    {
        private IHttpClientFactory _httpClientFactory;
        private CookieBridge _cookieBridge { get; set; }
        private IConfiguration _configuration { get; set; }
        public CookieBridgeConnection(CookieBridge cookieBridge, IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _cookieBridge = cookieBridge;
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
        }
        public HubConnection GetHubConnection(string hubUrl, string cookie)
        {
            _cookieBridge.Value = cookie;
            string baseAddress = _configuration.GetValue<string>("BaseAddress").TrimEnd('/');
            string fullHubUrl = baseAddress + hubUrl;
            return new HubConnectionBuilder()
            .WithUrl(fullHubUrl, options =>
            {
                options.Cookies = _cookieBridge.CookieContainer;
            })
            .AddNewtonsoftJsonProtocol()
            .Build();
        }
        public HttpClient GetHttpClient(string cookie)
        {
            _cookieBridge.Value = cookie;
            var httpClient = _httpClientFactory.CreateClient("DefaultHttpClient");
            httpClient.DefaultRequestHeaders.Add("Cookie", $"{_cookieBridge.Name}={_cookieBridge.Value}");
            return httpClient;
        }
    }
}
