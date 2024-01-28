using System.ComponentModel.DataAnnotations;

namespace TeisterMask.Data.Models
{
    public class Employee
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(40)]
        public string Username { get; set; } = null!;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = null!;

        [Required]
        public string Phone { get; set; } = null!;
        public virtual ICollection<EmployeeTask> EmployeesTasks { get; set; } = new HashSet<EmployeeTask>();

    }
}
//	Id – integer, Primary Key
//	Username – text with length [3, 40]. Should contain only lower or upper case letters and/or digits. (required)
//	Email – text (required). Validate it! There is attribute for this job.
//	Phone – text. Consists only of three groups (separated by '-'), the first two consist of three digits and the last one – of 4 digits. (required)
//	EmployeesTasks – collection of type EmployeeTask