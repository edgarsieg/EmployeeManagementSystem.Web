using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;

namespace EmployeeManagementSystem.Web.Models
{
    public class ApplicationUser : IdentityUser
    {
        public required string FullName { get; set; }

        public required string CompanyName { get; set; }

        public string? Role { get; set; } // Pvz. "Admin" arba "Employee"

        public bool MustChangePassword { get; set; }

        [ForeignKey("Company")]
        public int? CompanyId { get; set; }

        public Company? Company { get; set; }
    }
}