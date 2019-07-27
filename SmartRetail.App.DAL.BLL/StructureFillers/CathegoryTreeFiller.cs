using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmartRetail.App.DAL.BLL.DataServices;
using SmartRetail.App.DAL.BLL.DataStructures;
using SmartRetail.App.DAL.BLL.HelperClasses;
using SmartRetail.App.DAL.BLL.Utils;
using SmartRetail.App.DAL.DropBox;
using SmartRetail.App.DAL.Entities;
using SmartRetail.App.DAL.Repository;

namespace SmartRetail.App.DAL.BLL.StructureFillers
{
    public class CathegoryTreeFiller : ITreeFiller
    {
        private static string _basePath;

        public CathegoryTree<ImgTwinModel> Tree { get; private set; }
        private ImagesRepository imgRepo;
        private ProductRepository productRepo;
        private ShopRepository shopRepo;
        private BusinessRepository businessRepo;
        private DropBoxBase dbBase;


        public CathegoryTreeFiller(string connectionString)
        {
            imgRepo = new ImagesRepository(connectionString);
            businessRepo = new BusinessRepository(connectionString);
        }

        public async Task<CathegoryTree<ImgTwinModel>> FillTreeByBusinessAsync(int businessId)
        {
            var bTask = Task.Run(() => businessRepo.GetByIdAsync(businessId));
            var imgTask = Task.Run(() => imgRepo.GetAllImagesInBusinessAsync(businessId));
            var (busObj, imgs) = await Tasker.WhenAll(bTask, imgTask);

            var imgPaths = new List<string>();

            foreach (var img in imgs)   
            {
                imgPaths.Add(img.img_path + "/" + img.prod_id + "." + img.img_name + "." + img.img_type);
            }

            var name = businessId + ". " + busObj.name;

            Tree = new CathegoryTree<ImgTwinModel>
            {
                Value = new ImgTwinModel { folder = name, fullpath = "/" + name },
                Parent = null
            };
            
            if (imgPaths != null && imgPaths.Any())
            {
                foreach (var path in imgPaths)
                {
                    AddPath(path);
                }
            }
            return Tree;
        }

        public async Task<CathegoryTree<ImgTwinModel>> FillFolderTreeByBusinessAsync(int businessId)
        {
            var bTask = Task.Run(() => businessRepo.GetByIdAsync(businessId));
            var imgTask = Task.Run(() => imgRepo.GetAllImagesInBusinessAsync(businessId));
            var (busObj, imgs) = await Tasker.WhenAll(bTask, imgTask);

            var imgPaths = new List<string>();

            foreach (var img in imgs)
            {
                imgPaths.Add(img.img_path);
            }

            var name = businessId + ". " + busObj.name;

            Tree = new CathegoryTree<ImgTwinModel>
            {
                Value = new ImgTwinModel { folder = name, fullpath = "/" + name },
                Parent = null
            };

            if (imgPaths != null && imgPaths.Any())
            {
                foreach (var path in imgPaths)
                {
                    AddPath(path);
                }
            }
            return Tree;
        }

        public void AddPath(string path)
        {
            var parts = path.Split('/');
            var pnext = new ImgTwinModel
            {
                folder = parts[1],
                fullpath = "/" + parts[1]
            };
            var currNode = Tree;

            for (var p = 1; p < parts.Count() - 1; p++)
            {
                var pn = pnext.Clone() as ImgTwinModel;
                pn.folder = parts[p + 1];
                pn.fullpath += ("/" + parts[p + 1]);

                pnext.fullpath = pn.fullpath;

                var t = CathegoryTree<ImgTwinModel>.Search(currNode, pn);
                if (t == null)
                {
                    currNode = currNode.AddNode(pn);
                }
                else
                {
                    currNode = t;
                }
            }
        }
        public IEnumerable<ImgTwinModel> GetLevel(string fullpath)
        {
            return CathegoryTree<ImgTwinModel>.Search(Tree, new ImgTwinModel { fullpath = fullpath })
                .Nodes.Select(p => new ImgTwinModel { folder = p.Value.folder, fullpath = p.Value.fullpath, isFile = p.Value.folder.EndsWith(".jpg") }).ToList();
        }

        public CathegoryTree<ImgTwinModel> SearchSubTree(string fullpath)
        {
            return CathegoryTree<ImgTwinModel>.Search(Tree, new ImgTwinModel { fullpath = fullpath });
        }

        public IEnumerable<ImgTwinModel> Search(string search, CathegoryTree<ImgTwinModel> treePart)
        {
            var result = new List<ImgTwinModel>();
            var treeList = CathegoryTree<ImgTwinModel>.ToList(treePart);

            foreach (var node in treeList)
            {
                if (node.folder.Contains(search))
                {
                    result.Add(node);
                }
            }

            return result.Select(p => new ImgTwinModel { folder = p.folder, fullpath = p.fullpath, isFile = p.folder.EndsWith(".jpg") }).ToList(); ;
        }

        #region Depricated
        public CathegoryTreeFiller(string basePath, string connectionString)
        {
            _basePath = basePath;
            Tree = new CathegoryTree<ImgTwinModel>
            {
                Value = new ImgTwinModel { folder = _basePath, fullpath = "/" + _basePath },
                Parent = null
            };

            imgRepo = new ImagesRepository(connectionString);
            productRepo = new ProductRepository(connectionString);
            shopRepo = new ShopRepository(connectionString);
            businessRepo = new BusinessRepository(connectionString);

            dbBase = new DropBoxBase("o9340xsv2mzn7ws", "xzky2fzfnmssik1");
            dbBase.GeneratedAuthenticationURL();
            dbBase.GenerateAccessToken();
        }

        public async Task DownloadFiles()
        {
            var picNames = GetAllFolders().Select(GetImgInfo).ToList();
            
            var chain = new List<PicInfo>();

            var product = new Product
            {
                business_id = 1,
                unit_id = 1
            };

            foreach (var picInfo in picNames)
            {
                product.name = picInfo.ShortName.Split('.')[0];
                int id = productRepo.AddProduct(product);
                product.id = id;
                chain.Add(picInfo);
                picInfo.Id = id;
                picInfo.Name = id + "." + picInfo.ShortName.Split('.')[0];
                var business = businessRepo.GetById(1);

                await SmartUpload(business.id +". " +  business.name, picInfo);
            }
        }

        private async Task SmartUpload(string business, PicInfo picInfo)
        {
            var uploadPath = "/Dropbox/" + business +
                             picInfo.DirectoryPath.Replace("\\", "/");
            uploadPath = uploadPath.Substring(0, uploadPath.Length - 1);
            var imageUrl = await dbBase.Upload(uploadPath, picInfo.Name, picInfo.FullPath).ConfigureAwait(false);
                
            var file = await dbBase.GetFileWithSharedLink(imageUrl);

            var tempLink = ImageDataService.MakeTemporary(imageUrl);
                       
            var img = new Images
            {
                prod_id = picInfo.Id,
                img_name = picInfo.Name.Split('.')[1],
                img_type = picInfo.ShortName.Split('.')[1],
                img_url = imageUrl,
                img_url_temp = tempLink,
                img_path = uploadPath
            };
            imgRepo.Add(img);
        }
        
        
        public static IEnumerable<PicInfo> GetFiles(string path)
        {
            var dic = new List<PicInfo>();
            var pics = Directory.GetFiles(path);
            foreach (var pic in pics)
            {
                var str = pic.Split('\\');
                var name = str[str.Length - 1];
                var id = Convert.ToInt32(name.Split('.')[0]);
                dic.Add(new PicInfo(id, name, pic));
            }
            return dic;
        }

        private static PicInfo GetImgInfo(string path)
        {
            var str = path.Split('\\');
            var name = str[str.Length - 1];
            var id = Convert.ToInt32(name.Split('.')[0]);
            var fullPath = path.Replace(_basePath, "");
            return new PicInfo(id, name, path, name.Replace(id.ToString() + ". ", ""), fullPath.Replace(name, ""));
        }
        
        
        private List<string> GetAllFolders()
        {
            DirectoryInfo directoryInfo = new DirectoryInfo(_basePath);
            List<string> allFolders = new List<string>();

            foreach (FileInfo subFileInfo in directoryInfo.GetFiles("*.*", SearchOption.AllDirectories))
            {
                allFolders.Add(subFileInfo.FullName);
            }
            return allFolders;
        }

        public async Task<CathegoryTree<KeyValuePair<string, string>>> GetAllFoldersDropbox(Business business, Shop shop = null)
        {
            if (shop == null && business == null)
                return null;
            var sb = new StringBuilder();
            sb.Append(_basePath);

            if (business != null)
                sb.Append("/" + business.id + ". " + business.name);
            if (shop != null)
                sb.Append("/" + shop.id + ". " + shop.name);

            var items = await dbBase.GetAllFolders(sb.ToString());

            var folders = items.Entries.Where(p => p.IsFolder)
                .Select(p => new KeyValuePair<string, string>(p.PathLower, p.Name)).ToList();

            foreach (var folderInfo in folders)
            {
                var path = folderInfo.Key;

            }

            return null;
        }

        #endregion
    }
}