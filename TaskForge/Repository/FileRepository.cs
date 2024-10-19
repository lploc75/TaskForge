using TaskForge.Models;
using System.Threading.Tasks;
using TaskForge.DBContext;

public class FileRepository
{
    private readonly TaskForgeContext _context;

    public FileRepository(TaskForgeContext context)
    {
        _context = context;
    }

    // Phương thức lưu file vào cơ sở dữ liệu
    public async System.Threading.Tasks.Task SaveFileAsync(TaskForge.Models.File file)
    {
        _context.Files.Add(file);  // Thêm đối tượng file vào bảng Files trong database
        await _context.SaveChangesAsync();  // Lưu thay đổi vào cơ sở dữ liệu
    }
}
