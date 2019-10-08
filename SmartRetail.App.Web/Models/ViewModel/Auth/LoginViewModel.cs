using System.ComponentModel.DataAnnotations;

namespace SmartRetail.App.Web.Models.ViewModel.Auth
{
    public class LoginViewModel
    {

        [Required(ErrorMessage = "Не указан логин")]
        public string Username { get; set; }

        [Required(ErrorMessage = "Не указан пароль")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

    }
}