using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using TaskForge.Service;
using static TaskForge.Service.DropboxService;

namespace TaskForge.Controllers
{
    public class DropboxController : Controller
    {
        private readonly IConfiguration _configuration;

        public DropboxController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        // Bước 1: Yêu cầu quyền truy cập offline
        public IActionResult AuthorizeDropbox()
        {
            var clientId = _configuration["Dropbox:ClientId"];
            var redirectUri = _configuration["Dropbox:RedirectUri"];
            var authorizationUrl = $"https://www.dropbox.com/oauth2/authorize?client_id={clientId}&redirect_uri={redirectUri}&response_type=code&token_access_type=offline";

            return Redirect(authorizationUrl);
        }

        // Bước 2: Trao đổi mã ủy quyền lấy access token và refresh token
        [HttpGet]
        public async Task<IActionResult> DropboxCallback(string code)
        {
            var clientId = _configuration["Dropbox:ClientId"];
            var clientSecret = _configuration["Dropbox:ClientSecret"];
            var redirectUri = _configuration["Dropbox:RedirectUri"];

            var tokenRequestUrl = "https://api.dropboxapi.com/oauth2/token";
            var content = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("code", code),
                new KeyValuePair<string, string>("grant_type", "authorization_code"),
                new KeyValuePair<string, string>("client_id", clientId),
                new KeyValuePair<string, string>("client_secret", clientSecret),
                new KeyValuePair<string, string>("redirect_uri", redirectUri)
            });

            using (var client = new HttpClient())
            {
                var response = await client.PostAsync(tokenRequestUrl, content);
                var responseContent = await response.Content.ReadAsStringAsync();
                var tokenResponse = JsonConvert.DeserializeObject<TokenResponse>(responseContent);

                // Lưu access token và refresh token vào session hoặc cơ sở dữ liệu
                HttpContext.Session.SetString("DropboxAccessToken", tokenResponse.access_token);
                HttpContext.Session.SetString("DropboxRefreshToken", tokenResponse.refresh_token);

                return RedirectToAction("Index", "Home");
            }
        }
    }
}
