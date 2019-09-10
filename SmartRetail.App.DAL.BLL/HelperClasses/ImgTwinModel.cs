using Newtonsoft.Json;
using System;

namespace SmartRetail.App.DAL.BLL.HelperClasses
{
    [JsonObject(MemberSerialization.OptIn)]
    public class ImgTwinModel: IComparable, ICloneable
    {
        [JsonProperty]
        public int id { get; set; }

        [JsonProperty]
        public string folder { get; set; }

        [JsonProperty]
        public string fullpath { get; set; }
        public bool isFolder
        {
            get { return !isFile; }
        }
        public bool isFile { get; set; }


        public object Clone()
        {
            return new ImgTwinModel { folder = this.folder, fullpath = this.fullpath };
        }

        public int CompareTo(object obj)
        {
            var path = obj as ImgTwinModel;
            if (path != null)
            {
                return fullpath.CompareTo(path.fullpath);
            }
            else
                throw new Exception("Невозможно сравнить два объекта");
        }

        public override bool Equals(object obj)
        {
            var path = obj as ImgTwinModel;
            if (path != null)
            {
                return fullpath.Equals(path.fullpath);
            }
            else
                throw new Exception("Используется объект не того типа.");
        }
    }
}
