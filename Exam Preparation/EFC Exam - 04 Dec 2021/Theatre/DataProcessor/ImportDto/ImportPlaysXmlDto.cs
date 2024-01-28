using System.ComponentModel.DataAnnotations;
using System.Xml.Serialization;

namespace Theatre.DataProcessor.ImportDto
{
    [XmlType("Play")]
    public class ImportPlaysXmlDto
    {
        [Required]
        [MinLength(4)]
        [MaxLength(50)]

        public string Title { get; set; } = null!;

        [Required]
        public string Duration { get; set; } = null!;

        [Required]
        [Range(0.00, 10.00)]
        [XmlElement("Raiting")]
        public float Rating { get; set; }

        [Required]
        public string Genre { get; set; } = null!;

        [Required]
        [MaxLength(700)]
        public string Description { get; set; } = null!;

        [Required]
        [MinLength(4)]
        [MaxLength(30)]
        public string Screenwriter { get; set; } = null!;


    }
}
