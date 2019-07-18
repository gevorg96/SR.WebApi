using System;

namespace SmartRetail.App.DAL.BLL.HelperClasses
{
    public class ImgTwinModel: IComparable, ICloneable
    {
        public string folder { get; set; }
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
