using System.Threading;
using SmartRetail.App.DAL.DropBox;
using SmartRetail.App.DAL.Entities;
using SmartRetail.App.DAL.Repository;
using SmartRetail.App.DAL.Repository.Interfaces;
using Xunit;

namespace SmartRetail.App.Test
{
    public class TestDropboxApi
    {
        private const string conn =
            "Data Source=SQL6007.site4now.net;Initial Catalog=DB_A4E50E_smartretail;User Id=DB_A4E50E_smartretail_admin;Password=1234QWer;";

        private const string basePath = "/dropbox/dotnetapi/products";
        private DropBoxBase dbBase;
        private IImageRepository imgRepo;

        private void InitDropbox()
        {
            dbBase = new DropBoxBase("o9340xsv2mzn7ws", "xzky2fzfnmssik1");
            var url = dbBase.GeneratedAuthenticationURL();
            var token = dbBase.GenerateAccessToken();
            imgRepo = new ImagesRepository(conn);
        }

        [Fact]
        public async void TestDropBox()
        {
            InitDropbox();
            var imgRepo = new ImagesRepository(conn);
            var url = dbBase.GeneratedAuthenticationURL();
            var token = dbBase.GenerateAccessToken();
            var image = await dbBase.Upload("/Dropbox/DotNetApi/products", "туфли.png",
                @"D:\Projects\prom\SmartRetail.Mvc\SmartRetail.App.Web\pic\туфли.jpg");
            var img = new Image
            {
                prod_id = 1,
                img_name = "shoes",
                img_type = "jpg",
                img_url = image
            };
            Thread.Sleep(500);
            imgRepo.Add(img);
        }

        [Fact]
        public async void TestSmartDownload()
        {
            InitDropbox();
            var name = await dbBase.Upload("/Dropbox/DotNetApi/products/shop1/продукты магазин 1/sds",
                "17. Туфли детские мальчики 1-5.jpg",
                "D:\\Projects\\prom\\SmartRetail.Mvc\\SmartRetail.App.Web\\App_Data\\Pictures\\Номенклатура_по_папкам\\Обувь\\Детское\\17. Туфли детские мальчики 1-5.jpg");
        }
    }
}