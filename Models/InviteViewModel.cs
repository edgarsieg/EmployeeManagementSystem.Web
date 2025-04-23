using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using EmployeeManagementSystem.Web.Data;

namespace EmployeeManagementSystem.Web.Models
{
    public class InviteViewModel
    {
        [Required(ErrorMessage = "Vardas ir pavardė yra privalomi")]
        public string FullName { get; set; } = string.Empty;

        [Required(ErrorMessage = "El. paštas yra privalomas")]
        [EmailAddress(ErrorMessage = "Neteisingas el. pašto formatas")]
        public string Email { get; set; } = string.Empty;

        public string? GeneratedPassword { get; set; }

        public bool InviteSuccessful { get; set; } = false;

        public List<ApplicationUser> InvitedEmployees { get; set; } = new();
    }
}

