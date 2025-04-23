using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace EmployeeManagementSystem.Web.Models
{
    public class Company
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Įmonės pavadinimas privalomas")]
        public required string Name { get; set; }

        public ICollection<ApplicationUser>? Users { get; set; }

        public ICollection<Employee>? Employees { get; set; }
    }
}
