using System.ComponentModel.DataAnnotations.Schema;

namespace TeisterMask.Data.Models

{
    public class EmployeeTask
    {
        [ForeignKey("Employee")]
        public int EmployeeId { get; set; }
        public virtual Employee Employee { get; set; } = null!;


        [ForeignKey("Task")]
        public int TaskId { get; set; }
        public virtual Task Task { get; set; } = null!;


    }
}

// EmployeeId – integer, Primary Key, foreign key (required)
// Employee – Employee
// TaskId – integer, Primary Key, foreign key (required)
// Task – Task
