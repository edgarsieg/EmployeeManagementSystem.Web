using System.ComponentModel.DataAnnotations;

namespace EmployeeManagementSystem.Web.Models
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "El. paštas privalomas")]
        [EmailAddress]
        public required string Email { get; set; }

        [Required(ErrorMessage = "Slaptažodis privalomas")]
        [DataType(DataType.Password)]
        public required string Password { get; set; }
    }
}