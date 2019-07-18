using System;
using System.Collections.Generic;
using System.Data.Common;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Dropbox.Api.Files;
using SmartRetail.App.DAL.BLL.DataServices;
using SmartRetail.App.DAL.BLL.StructureFillers;
using SmartRetail.App.DAL.BLL.Utils;
using SmartRetail.App.DAL.DropBox;
using SmartRetail.App.DAL.Entities;
using SmartRetail.App.DAL.Repository;
using SmartRetail.App.Web.Models;
using SmartRetail.App.Web.Models.ViewModel;
using Xunit;

namespace SmartRetail.App.Test
{
    public class TestDropboxApi
    {
        private const string conn =
            "Data Source=SQL6001.site4now.net;Initial Catalog=DB_A497DE_retailsys;User Id=DB_A497DE_retailsys_admin;Password=1234QWer;";

        private const string basePath = "/dropbox/dotnetapi/products";

        private DropBoxBase dbBase;

        private void InitDropbox()
        {
            dbBase = new DropBoxBase("o9340xsv2mzn7ws", "xzky2fzfnmssik1");
            var url = dbBase.GeneratedAuthenticationURL();
            var token = dbBase.GenerateAccessToken();
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
            var img = new Images
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
        public async void TestTempLink()
        {
            InitDropbox();
            var imgRepo = new ImagesRepository(conn);
            var photo = imgRepo.GetById(1);
            var path = await dbBase.GetFileWithSharedLink(photo.img_url);
            var result = await dbBase.GetTempLink(path);
        }


        [Fact]
        public void PathFill()
        {
            InitDropbox();
            var imgRepo = new ImagesRepository(conn);
            var imgDataService = new ImageDataService(dbBase, imgRepo);
            var images = imgRepo.GetAllImages();
            imgDataService.AddPathDropBox(images);
        }

        [Fact]
        public async void TestImages()
        {
            InitDropbox();
            var imgRepo = new ImagesRepository(conn);
            var photo = imgRepo.GetById(1);
            var path = await dbBase.GetFileWithSharedLink(photo.img_url);
            await dbBase.MoveFile(path, ChangePath(path));
        }

        private string ChangePath(string path)
        {
            var t = path.Split('/');
            var filename = t.TakeLast(1).First();
            t[t.Length - 1] = "products/" + filename;
            return string.Join("/", t);
        }

        [Fact]
        public async void TestDropboxHierarhyStructure()
        {
            InitDropbox();
            var path = "/dropbox/dotnetapi/products/1. Магазин обуви/1. Магазин на тургеневской";
            var items = await dbBase.GetAllFolders(path);
            var folders = items.Entries.Where(p => p.IsFolder)
                .Select(p => new FolderViewModel {folder = p.Name, fullpath = p.PathLower}).ToList();


            

            //var t = res.Entries.Where(p => p.IsFolder).Select(p => p.PathDisplay).ToList();
        }

        [Fact]
        public async void PicBatch()
        {
            var picUpdater = new PicturesBatchUpdater(conn);
            await picUpdater.Run(
                @"D:\Projects\prom\SmartRetail.Mvc\SmartRetail.App.Web\App_Data\Pictures\Номенклатура_по_папкам\Обувь\Детское");
        }

        [Fact]
        public async void TestCreateFolder()
        {
            InitDropbox();
            var t = await dbBase.CreateFolder("/dropbox/DotNetApi/weqwe/ferfew/efewfew/fewfew");
        }

        [Fact]
        public async void TestSmartDownload()
        {
            InitDropbox();
            var name = await dbBase.Upload("/Dropbox/DotNetApi/products/shop1/продукты магазин 1/sds",
                "17. Туфли детские мальчики 1-5.jpg",
                "D:\\Projects\\prom\\SmartRetail.Mvc\\SmartRetail.App.Web\\App_Data\\Pictures\\Номенклатура_по_папкам\\Обувь\\Детское\\17. Туфли детские мальчики 1-5.jpg");
        }

        [Fact]
        public async void TestSearch()
        {
            InitDropbox();
            var res = await dbBase.SearchFolder("/dropbox/DotNetApi/products/1. Магазин обуви", "Сан", 0, 1000);
            if (res.Matches.Count > 0)
            {
                var folders = res.Matches.Where(p => p.Metadata.IsFolder).Select(p => p.Metadata.Name).ToList();
                var files = res.Matches.Where(p => p.Metadata.IsFile).Select(p => p.Metadata.Name).ToList();
            }
        }

        [Fact]
        public async void TestMemoryStreamUpload()
        {
            InitDropbox();
            var path = "/dropbox/dotnetapi/file.jpg";
            var SourceFilePath = @"C:\Users\gevor\OneDrive\Рабочий стол\1.txt";
            var pic = "";
            using (StreamReader sr = new StreamReader(SourceFilePath))
            {
                pic = sr.ReadToEnd();
                var bytes = Convert.FromBase64String(pic);
                var contents = new MemoryStream(bytes);
                var uri = await dbBase.Upload(contents, path);
            }        
        }

        [Fact]
        public async void TestCathegoryFiller()
        {
            var filler = new CathegoryTreeFiller(basePath, conn);
            var s = (new ShopRepository(conn)).GetById(1);
            var b = (new BusinessRepository(conn)).GetById(s.business_id.Value);

            await filler.GetAllFoldersDropbox(b, s);
        }

        [Fact]
        public async void AddPicsInShoes()
        {
            InitDropbox();
            var path = @"C:\Users\gevor\OneDrive\Рабочий стол\файлы";
            var dropboxpath =
                "/dropbox/dotnetapi/products/1. Кайфы от Петерфельдо/1. Магазин на тургеневской/Обувь/Мужское/Туфли";
            var prodRepo = new ProductRepository(conn);
            var imgRepo = new ImagesRepository(conn);

            var prods = prodRepo.GetProducts(37);
            var files = Directory.EnumerateFiles(path).ToList();

            var c = 0;

            foreach (var product in prods)
            {
                var imageUrl = await dbBase.Upload(dropboxpath, product.id + ". " + product.name + ".jpg", files[c]).ConfigureAwait(false);

                var tempLink = ImageDataService.MakeTemporary(imageUrl);

                var img = new Images
                {
                    prod_id = product.id,
                    img_name = product.name,
                    img_type = "jpg",
                    img_url = imageUrl,
                    img_url_temp = tempLink
                };
                imgRepo.Add(img);
                if (c == 2)
                {
                    c = -1;
                }
                c++;
            }
        }

        [Fact]
        public async void AddKartash()
        {
            InitDropbox();
            var path = @"C:\Users\gevor\OneDrive\Рабочий стол\папки";
            var lst = new List<string>();
            var l = DirSearch(lst, path).FirstOrDefault();
            var dbpath = l.Replace(path, "").Replace("\\", "/");
            var dbbaseurl = "/dropbox/dotnetapi/products";
            var businessStr = dbpath.Split('/')[1];
            var shopStr = dbpath.Split("/")[2];
            var b = new BusinessRepository(conn).GetWithFilter("name", "N'" + businessStr + "'").FirstOrDefault();
            var shop = new ShopRepository(conn).GetWithFilter("name", "N'" + shopStr + "'").FirstOrDefault();
            dbpath = dbbaseurl +"/" + b.id + ". " + b.name +"/"+ shop.id + ". " + shop.name + "/Пиво";

            var prodRepo = new ProductRepository(conn);
            var imgRepo = new ImagesRepository(conn);
            var prods = prodRepo.GetProducts(140);

            foreach (var product in prods)
            {
                var imageUrl = await dbBase.Upload(dbpath, product.id + ". " + product.name, l)
                    .ConfigureAwait(false);

                var tempLink = ImageDataService.MakeTemporary(imageUrl);

                var img = new Images
                {
                    prod_id = product.id,
                    img_name = product.name,
                    img_type = "jpg",
                    img_url = imageUrl,
                    img_url_temp = tempLink
                };
                imgRepo.Add(img);
            }
        }

        [Fact]
        public async void TestMove()
        {
            InitDropbox();
            var bRepo = new BusinessRepository(conn);
            var shoprepo = new ShopRepository(conn);
            var prodrepo = new ProductRepository(conn);
            var imgrepo = new ImagesRepository(conn);
            var basepath = "/dropbox/dotnetapi/products/";

            
            var p = prodrepo.GetById(138);
            var img = imgrepo.GetById(138);
            var imgpath = await dbBase.GetFilePath(img.img_url);
            var imgtailpath = string.Join('/',imgpath.Split('/').Skip(6));

            imgtailpath = imgtailpath.Replace(".jpg", "ewqweqwe.jpg");

            var shop = shoprepo.GetById(3);
            var b = bRepo.GetById(shop.business_id.Value);
            var new_path = basepath + b.id + ". " + b.name + "/" + shop.id + ". " + shop.name + "/" + imgtailpath;
            var t = await dbBase.MoveFile(imgpath, new_path);

        }

        [Fact]
        public async void TestMoveV2()
        {
            InitDropbox();
            var bRepo = new BusinessRepository(conn);
            var shoprepo = new ShopRepository(conn);
            var prodrepo = new ProductRepository(conn);
            var imgrepo = new ImagesRepository(conn);
            var basepath = "/dropbox/dotnetapi/products/";

            var p = prodrepo.GetById(138);
            var img = imgrepo.GetById(138);
            var oldshop = shoprepo.GetById(p.shop_id.Value);
            var new_shop = shoprepo.GetById(3);
            
            var imgpath = await dbBase.GetFileWithSharedLink(img.img_url);
            var old = (oldshop.id + ". " + oldshop.name).ToLower();
            var newv = (new_shop.id + ". " + new_shop.name).ToLower();
            var new_path = imgpath.ToLower().Replace(old, newv);
            var moveRes = await dbBase.MoveFile(imgpath, new_path);
        }
        
        
        private IEnumerable<string> DirSearch(List<string> list, string sDir)
        {
            foreach (string d in Directory.GetDirectories(sDir))
            {
                foreach (string f in Directory.GetFiles(d))
                {
                    list.Add(f);
                }
                DirSearch(list, d);
            }

            return list;
        }

        [Fact]
        public void ChangeName()
        {
            InitDropbox();
        }


        [Fact]
        public async void MoveAllFilesToRoot()
        {
            InitDropbox();
            var imgRepo = new ImagesRepository(conn);
            var prodRepo = new ProductRepository(conn);

            var images = imgRepo.GetAllImages(); 
            foreach (var img in images)
            {
                //var source1 = img.img_path + "/" + img.prod_id + "." + img.img_name;
                var source2 = await dbBase.GetFilePath(img.img_url);
                var target = string.Join('/', source2.Split('/').Take(3)) +"/" + img.prod_id + "." + img.img_name + ".jpg";
                await dbBase.MoveFile(source2, target);
            }

            var prod = prodRepo.GetById(1);
            var image = await dbBase.GetAllFolders("/Dropbox/", true);
            
        }
    }

}