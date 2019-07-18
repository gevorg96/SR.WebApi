using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using SmartRetail.App.DAL.BLL.DataServices;
using SmartRetail.App.DAL.BLL.HelperClasses;
using SmartRetail.App.DAL.DropBox;
using SmartRetail.App.DAL.Entities;
using SmartRetail.App.DAL.Repository;

namespace SmartRetail.App.DAL.BLL.Utils
{
    public class PicturesBatchUpdater
    {
        private string conn;
        public PicturesBatchUpdater(string connection)
        {
            conn = connection;
        }

        public async Task Run(string path)
        {
            var picNames = GetFiles(path);
            var dits = Directory.GetDirectories(path);
            
            var imgRepo = new ImagesRepository(conn);
            
            var dbBase = new DropBoxBase("o9340xsv2mzn7ws", "xzky2fzfnmssik1");
            var imgDataService = new ImageDataService(dbBase, imgRepo);
            foreach (var picInfo in picNames)
            {
                var imageUrl = await dbBase.Upload("/Dropbox/DotNetApi/products", picInfo.Name, picInfo.FullPath).ConfigureAwait(false);
                
                var file = await dbBase.GetFileWithSharedLink(imageUrl);

                var tempLink =  await dbBase.GetTempLink(file);
                       
                var img = new Images
                {
                    prod_id = picInfo.Id,
                    img_name = picInfo.Name.Split('.')[1],
                    img_type = picInfo.Name.Split('.')[2],
                    img_url = imageUrl,
                    img_url_temp = tempLink
                };
                imgRepo.Add(img);
            }
        }

        private IEnumerable<PicInfo> GetFiles(string path)
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
    }
}