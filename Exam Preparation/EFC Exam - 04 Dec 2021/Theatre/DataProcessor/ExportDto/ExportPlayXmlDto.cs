using Theatre.Data.Models.Enums;
using System.Xml.Serialization;

namespace Theatre.DataProcessor.ExportDto
{
    [XmlType("Play")]
    public class ExportPlayXmlDto
    {
        [XmlAttribute("Title")]
        public string Title { get; set; } = null!;

        [XmlAttribute("Duration")]
        public string Duration { get; set; }
      
        [XmlAttribute("Rating")]
        public string Rating { get; set; } = null!;

        [XmlAttribute("Genre")]
        public Genre Genre { get; set; }

        [XmlArray("Actors")]
        public ExportCastXmlDto[] Actors { get; set; } = null!;
    }
}
