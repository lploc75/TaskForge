using Dropbox.Api.Files;
using Dropbox.Api;
using TaskForge.Repository;
using Azure.Core;
using Newtonsoft.Json;

namespace TaskForge.Service
{
    public class DropboxService
    {
        private const string BaseFolder = "/TaskForge"; // Thư mục gốc "TaskForge" trên Dropbox
        
        private readonly IConfiguration _configuration;
        private readonly FileRepository _fileRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public DropboxService(FileRepository fileRepository, IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
        {
            _configuration = configuration;
            _fileRepository = fileRepository;
            _httpContextAccessor = httpContextAccessor;
        }


        // Kiểm tra và tạo thư mục nếu chưa tồn tại
        private async Task EnsureFolderExistsAsync(DropboxClient dbx, string folderPath)
        {
            try
            {
                // Kiểm tra nếu thư mục đã tồn tại
                await dbx.Files.GetMetadataAsync(folderPath);
            }
            catch (ApiException<GetMetadataError>)
            {
                // Thư mục chưa tồn tại, tạo thư mục
                await dbx.Files.CreateFolderV2Async(folderPath);
            }
        }
        public async Task<string> UploadFileAsync(string filePath, string fileName, string accountId, string subtaskId, Models.File fileModel)
        {
            try
            {
                // Lấy access token từ session
                var accessToken = _httpContextAccessor.HttpContext.Session.GetString("DropboxAccessToken");

                if (string.IsNullOrEmpty(accessToken))
                {
                    // Làm mới token nếu access token chưa tồn tại hoặc hết hạn
                    accessToken = await RefreshAccessTokenAsync();
                    if (string.IsNullOrEmpty(accessToken))
                    {
                        throw new Exception("Access token không tồn tại, cần xác thực lại");
                    }
                }

                using (var dbx = new DropboxClient(accessToken))
                {
                    // Kiểm tra và tạo thư mục TaskForge nếu chưa tồn tại
                    await EnsureFolderExistsAsync(dbx, BaseFolder);

                    // Đường dẫn tới thư mục accountId (VD: /TaskForge/ACC001)
                    var accountFolder = $"{BaseFolder}/{accountId}";
                    await EnsureFolderExistsAsync(dbx, accountFolder); // Kiểm tra và tạo thư mục accountId nếu chưa có

                    // Tạo tên file với định dạng: subtaskId_TenFile
                    var dropboxFileName = $"{subtaskId}_{fileName}";
                    var dropboxPath = $"{accountFolder}/{dropboxFileName}";

                    // Tải file lên Dropbox
                    using (var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
                    {
                        var uploadResult = await dbx.Files.UploadAsync(dropboxPath, WriteMode.Overwrite.Instance, body: fileStream);
                        fileModel.FilePath = uploadResult.PathLower; // Cập nhật đường dẫn trên Dropbox vào fileModel
                    }

                    // Sau khi tải lên thành công, lưu thông tin file vào database
                    await _fileRepository.SaveFileAsync(fileModel);

                    return fileModel.FilePath; // Trả về đường dẫn đã upload
                }
            }
            catch (Exception ex)
            {
                // Log lỗi và xử lý tùy theo yêu cầu của bạn
                throw new Exception($"Có lỗi xảy ra khi tải file lên Dropbox: {ex.Message}");
            }
        }

        public async Task ExchangeCodeForTokenAsync(string code)
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

                // Lưu access token và refresh token vào session
                _httpContextAccessor.HttpContext.Session.SetString("DropboxAccessToken", tokenResponse.access_token);
                _httpContextAccessor.HttpContext.Session.SetString("DropboxRefreshToken", tokenResponse.refresh_token);
            }
        }

        public async Task<string> RefreshAccessTokenAsync()
        {
            var clientId = _configuration["Dropbox:ClientId"];
            var clientSecret = _configuration["Dropbox:ClientSecret"];
            var refreshToken = _httpContextAccessor.HttpContext.Session.GetString("DropboxRefreshToken");

            if (string.IsNullOrEmpty(refreshToken))
            {
                // Nếu không có refresh token, yêu cầu người dùng xác thực lại
                return null;
            }

            var tokenRequestUrl = "https://api.dropboxapi.com/oauth2/token";
            var content = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("grant_type", "refresh_token"),
                new KeyValuePair<string, string>("refresh_token", refreshToken),
                new KeyValuePair<string, string>("client_id", clientId),
                new KeyValuePair<string, string>("client_secret", clientSecret)
            });

            using (var client = new HttpClient())
            {
                var response = await client.PostAsync(tokenRequestUrl, content);
                var responseContent = await response.Content.ReadAsStringAsync();
                var tokenResponse = JsonConvert.DeserializeObject<TokenResponse>(responseContent);

                // Lưu access token mới vào session
                _httpContextAccessor.HttpContext.Session.SetString("DropboxAccessToken", tokenResponse.access_token);
                return tokenResponse.access_token;
            }
        }

        public class TokenResponse
        {
            public string access_token { get; set; }
            public string refresh_token { get; set; }
        }
    //// Phương thức download file
    //public async Task DownloadFileAsync(string dropboxPath, string localPath)
    //{
    //    using (var dbx = new DropboxClient(accessToken))
    //    {
    //        var response = await dbx.Files.DownloadAsync(dropboxPath);
    //        var content = await response.GetContentAsByteArrayAsync();
    //        await File.WriteAllBytesAsync(localPath, content);
    //    }
    //}
}
}
