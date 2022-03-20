using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace Diploma_project.ViewModels
{
    public class EditPasswordViewModel
    {
        [HiddenInput(DisplayValue = false)]
        public string Id { get; set; }

        [Required(ErrorMessage = "Не указана пароль")]
        [DataType(DataType.Password)]
        [StringLength(50, MinimumLength = 8, ErrorMessage = "Длина пароль должна быть от 8 до 50 символов")]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [StringLength(50, MinimumLength = 8, ErrorMessage = "Длина пароль должна быть от 8 до 50 символов")]
        public string OldPassword { get; set; }

    }
}
