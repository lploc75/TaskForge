using Dropbox.Api.Files;
using Dropbox.Api;
using TaskForge.Repository;
using Azure.Core;

namespace TaskForge.Service
{
    public class DropboxService
    {
        private readonly string accessToken = "sl.B_HI_s5xNSmjILM-th0Aegv-q0pi3Uj8KGJCSfpBmgJH_KfQRHh8Psdr5kVvnW0sJ2QOMQFrkEnVl1TngX0SwqHe0mgITk5dwuj7cP9uVtFjZUzUou-7PRFmd0r09Zy6WGCsugK22EyW"; // Token của bạn
        private const string BaseFolder = "/TaskForge"; // Thư mục gốc "TaskForge" trên Dropbox

        private readonly FileRepository _fileRepository;

        public DropboxService(FileRepository fileRepository)
        {
            _fileRepository = fileRepository;
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
