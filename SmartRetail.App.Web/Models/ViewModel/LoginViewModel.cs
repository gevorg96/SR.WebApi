using System.ComponentModel.DataAnnotations;

namespace SmartRetail.App.Web.Models.ViewModel
{
    public class LoginViewModel
    {
        [Required]
        public string username { get; set; }

        [Required]
        public string password { get; set; }
    }
}
