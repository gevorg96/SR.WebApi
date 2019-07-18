using SmartRetail.App.DAL.BLL.DataStructures;
using SmartRetail.App.DAL.BLL.StructureFillers;
using SmartRetail.App.DAL.Repository;
using System.Linq;
using Xunit;
using System.Threading.Tasks;

namespace SmartRetail.App.Test
{
    public class TestDataStructures
    {
        private const string conn =
            "Data Source=SQL6001.site4now.net;Initial Catalog=DB_A497DE_retailsys;User Id=DB_A497DE_retailsys_admin;Password=1234QWer;";
        
        [Fact]
        public void TestCategoryTree()
        {
            var root = new CathegoryTree<string>
            {
                Value = "12321"
            };
            
            var ch1 = root.AddNode("4few");
            var ch2 = root.AddNode("rtret");
            var ch3 = root.AddNode("frefrefre");

            var ch11 = ch1.AddNode("tghyhrt");
            var ch12 = ch1.AddNode("getfgrt");
            var ch21 = ch2.AddNode("4t54t54");
            var ch31 = ch3.AddNode("6546546546");

            var ch111 = ch11.AddNode("erere");

            var node = CathegoryTree<string>.Search(root, "tghyhrt");
            var t = CathegoryTree<string>.Search(root, "yhrt");
            var treeCollection = CathegoryTree<string>.ToList(root);
        }

        [Fact]
        public async void TestCatalogs()
        {
            var filler = new CathegoryTreeFiller(@"D:\Projects\smartretail\SmartRetail.Mvc\SmartRetail.App.Web\App_Data\Pictures\Номенклатура_по_папкам", conn);
                
            await filler.DownloadFiles();
        }

        [Fact]
        public void TestTreeFiller()
        {
            var filler = new CathegoryTreeFiller("1. Кайфы от Петерфельдо", conn);
            var imgRepo = new ImagesRepository(conn);
            var images = imgRepo.GetAllImages().Select(p => new { path = p.img_path + "/" + p.prod_id + "." + p.img_name + "." + p.img_type });
            foreach (var img in images)
            {
                filler.AddPath(img.path);
            }
            var tree = filler.Tree;

            var level = filler.GetLevel("/1. Кайфы от Петерфельдо/Бонусные карты");
        }

        [Fact]
        public void CutDbPath()
        {
            var imgRepo = new ImagesRepository(conn);
            var images = imgRepo.GetAllImages();
            foreach(var i in images)
            {
                imgRepo.UpdateImage(i.prod_id, "img_path", "N'/products" + i.img_path +"'");
            }
        }

        [Fact]
        public async Task TestTreeFillerAdapterAsync()
        {
            var filler = new CathegoryTreeFiller(conn);
            await filler.FillTreeByBusinessAsync(1);
            var level = filler.GetLevel("/products/1. Кайфы от Петерфельдо/Обувь/Женское/Сапоги");
            var tree = filler.SearchSubTree("/products/1. Кайфы от Петерфельдо/Обувь/Женское/Сапоги");
            var res = filler.Search("Са", tree);
        }

    }
}