using System.ComponentModel.DataAnnotations;
using System.Xml.Serialization;
using TeisterMask.Data.Models.Enums;

namespace TeisterMask.DataProcessor.ImportDto
{
    [XmlType("Task")]
    public class ImportTaskXmlDto
    {
        [XmlElement("Name")]
        [Required]
        [MinLength(2)]
        [MaxLength(40)]
        public string TaskName { get; set; } = null!;

        [XmlElement("OpenDate")]
        [Required]
        public string OpenDate { get; set; } = null!;

        [XmlElement("DueDate")]
        [Required]
        public string DueDate { get; set; } = null!;

        [XmlElement("ExecutionType")]
        [Required] 
        public string ExecutionType { get; set; } = null!;
       
        [XmlElement("LabelType")]
        [Required]
        public string LabelType { get; set; } = null!;
    }
}
//	Id – integer, Primary Key
//	Name – text with length [2, 40] (required)
//	OpenDate – date and time (required)
//	DueDate – date and time (required)
//	ExecutionType – enumeration of type ExecutionType, with possible values (ProductBacklog, SprintBacklog, InProgress, Finished) (required)
//	LabelType – enumeration of type LabelType, with possible values (Priority, CSharpAdvanced, JavaAdvanced, EntityFramework, Hibernate) (required)
//	ProjectId – integer, foreign key (required)
//	Project – Project 
//	EmployeesTasks – collection of type EmployeeTask