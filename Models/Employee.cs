using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EmployeeManagementSystem.Web.Models
{
    public class Employee
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Vardas yra privalomas")]
        public required string FirstName { get; set; }

        [Required(ErrorMessage = "Pavardė yra privaloma")]
        public required string LastName { get; set; }

        public required string Position { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Alga turi būti teigiama")]
        public decimal Salary { get; set; }

        public DateTime HireDate { get; set; } = DateTime.Now;

        [ForeignKey("Company")]
        public int CompanyId { get; set; }

        public Company? Company { get; set; }
    }
}