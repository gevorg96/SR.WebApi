using SmartRetail.App.DAL.Repository;
using Xunit;
using SmartRetail.App.DAL.BLL.DataServices;

namespace SmartRetail.App.Test
{
    public class TestDataStructures
    {
        private const string conn =
            "Data Source=SQL6007.site4now.net;Initial Catalog=DB_A4C0E8_srbackend;User Id=DB_A4C0E8_srbackend_admin;Password=1234QWer";
        
       [Fact]
        public async void TestFoldersFiller()
        {
            var foldersDataService = new FoldersDataService(new FoldersRepository(conn), new ProductRepository(conn));
            await foldersDataService.GetFoldersTreeAsync(1);
        }

    }
}