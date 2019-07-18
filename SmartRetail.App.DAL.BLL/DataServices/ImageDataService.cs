using System.Collections.Generic;
using SmartRetail.App.DAL.DropBox;
using SmartRetail.App.DAL.Entities;
using SmartRetail.App.DAL.Repository;

namespace SmartRetail.App.DAL.BLL.DataServices
{
    public class ImageDataService
    {
        private readonly IPictureWareHouse _dropBoxBase;
        private readonly IImageRepository _imgRepo;
        
        public ImageDataService(IPictureWareHouse ipvh, IImageRepository imgRepo)
        {
            _dropBoxBase = ipvh;
            _imgRepo = imgRepo;
            _dropBoxBase.GeneratedAuthenticationURL();
            _dropBoxBase.GenerateAccessToken();
        }

        public void AddPathDropBox(IEnumerable<Images> images)
        {
            foreach (var image in images)
            {
                _imgRepo.UpdateImage(image.prod_id, "img_url_temp", "N'" + MakeTemporary(image.img_url) + "'");
            }
        }
        
        public static string MakeTemporary(string link)
        {
            return link.Replace("https://www", "https://dl").Replace("?dl=0", "?dl=1");
        }

        //public void C
    }
}