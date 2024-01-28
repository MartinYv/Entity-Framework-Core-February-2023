using System.ComponentModel.DataAnnotations;
using System.Xml.Serialization;

namespace Artillery.DataProcessor.ImportDto
{
    [XmlType("Shell")]
    public class ImportShellXmlDto
    {
        [Required]
        [XmlElement("ShellWeight")]
        [Range(2, 1_680)]
        public double ShellWeight { get; set; }

        [Required]
        [XmlElement("Caliber")]
        [MinLength(4)]
        [MaxLength(30)]
        public string Caliber { get; set; } = null!;
    }
    //ShellWeight – double in range  [2…1_680] (required)
    //Caliber – text with length [4…30] (required)
}
