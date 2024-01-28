using Footballers.Data.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Footballers.DataProcessor.ExportDto
{

    [XmlType("Coach")]
    public class ExportTeamDto
    {
        [XmlAttribute("FootballersCount")]
        public int FootballersCount { get; set; }


        [XmlElement("CoachName")]
        public string Name { get; set; }


        [XmlArray("Footballers")]
        public ExportFootballerDto[] Footballers { get; set; }
    }
      
    
    [XmlType("Footballer")]
    public class ExportFootballerDto
    {
        [XmlElement("Name")]
        public string Name { get; set; }

            
        [XmlElement("Position")]
        public string PositionType { get; set; }
    }
 
}
