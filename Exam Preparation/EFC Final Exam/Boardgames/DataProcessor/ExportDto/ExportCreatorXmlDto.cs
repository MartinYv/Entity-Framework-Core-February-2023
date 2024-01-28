using System.Xml.Serialization;

namespace Boardgames.DataProcessor.ExportDto
{
    [XmlType("Creator")]
    public class ExportCreatorXmlDto
    {
        [XmlAttribute("BoardgamesCount")]
        public int Count { get; set; }

        [XmlElement("CreatorName")]
        public string CreatorName { get; set; } = null!;

        [XmlArray("Boardgames")]
        public ExportBoardgameXmlExport[] Boardgames { get; set; } = null!;
    }
}
