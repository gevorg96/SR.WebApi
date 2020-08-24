using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SR.DAL.Entities
{
    [Table("Image")]
    public class Image
    {
        [Required]
        public long ProdId { get; set; }

        public string ImgType { get; set; }

        public string ImgName { get; set; }

        public string ImgUrl { get; set; }

        public string ImgUrlTemp { get; set; }

        public string ImgPath { get; set; }
    }
}
