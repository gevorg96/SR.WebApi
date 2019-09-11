using System.IO;
using System.Threading.Tasks;

namespace SmartRetail.App.DAL.DropBox
{
    public interface IPictureWareHouse
    {
        string GeneratedAuthenticationURL();
        string GenerateAccessToken();
        Task<bool> Delete(string path);
        Task<string> Upload(MemoryStream content, string path);
    }
}