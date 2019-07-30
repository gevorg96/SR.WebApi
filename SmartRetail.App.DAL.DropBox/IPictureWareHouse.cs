using System.IO;
using System.Threading.Tasks;
using Dropbox.Api.Files;

namespace SmartRetail.App.DAL.DropBox
{
    public interface IPictureWareHouse
    {
        string GeneratedAuthenticationURL();
        string GenerateAccessToken();
        Task<string> GetFileWithSharedLink(string sharedLink);
        Task<bool> Delete(string path);
        Task<string> GetTempLink(string filePath);
        Task<ListFolderResult> GetAllFolders(string path, bool recursive = true);
        Task<SearchResult> SearchFolder(string path, string query, ulong start, ulong limit);
        Task<string> Upload(string UploadfolderPath, string UploadfileName, string SourceFilePath);
        Task<string> Upload(MemoryStream content, string path);
        Task<string> MoveFile(string from, string to);
    }
}