using System.ComponentModel.DataAnnotations;

namespace EmployeeManagementSystem.Web.Models
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "El. paštas privalomas")]
        [EmailAddress(ErrorMessage = "Neteisingas el. pašto formatas")]
        public required string Email { get; set; }

        [Required(ErrorMessage = "Slaptažodis privalomas")]
        [DataType(DataType.Password)]
        public required string Password { get; set; }

        [Required(ErrorMessage = "Vardas pavardė privalomi")]
        [Display(Name = "Vardas ir pavardė")]
        public required string FullName { get; set; }

        [Required(ErrorMessage = "Įmonės pavadinimas privalomas")]
        [Display(Name = "Įmonės pavadinimas")]
        public required string CompanyName { get; set; }
    }
}