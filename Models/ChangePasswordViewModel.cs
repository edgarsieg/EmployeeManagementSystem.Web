using System.ComponentModel.DataAnnotations;

namespace EmployeeManagementSystem.Web.Models
{
    public class ChangePasswordViewModel
    {
        [Required(ErrorMessage = "Įveskite dabartinį slaptažodį")]
        [DataType(DataType.Password)]
        [Display(Name = "Dabartinis slaptažodis")]
        public string CurrentPassword { get; set; } = string.Empty;

        [Required(ErrorMessage = "Įveskite naują slaptažodį")]
        [DataType(DataType.Password)]
        [Display(Name = "Naujas slaptažodis")]
        public string NewPassword { get; set; } = string.Empty;

        [Required(ErrorMessage = "Pakartokite naują slaptažodį")]
        [DataType(DataType.Password)]
        [Display(Name = "Pakartokite naują slaptažodį")]
        [Compare("NewPassword", ErrorMessage = "Slaptažodžiai nesutampa.")]
        public string ConfirmPassword { get; set; } = string.Empty;
    }
}
