using Footballers.Data.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Footballers.DataProcessor.ImportDto
{
    [XmlType("Coach")]
    public class ImportCoachesAndFootballersDtoXml
    {
        [XmlElement("Name")]
        [MaxLength(40)]
        public string Name { get; set; } = null!;


        [XmlElement("Nationality")]
        public string Nationality { get; set; } = null!;


        [XmlArray("Footballers")]
        public FootballersDtoXml[] Footballers { get; set; } = null!;
    }



    [XmlType("Footballer")]
    public class FootballersDtoXml
    {
        [XmlElement("Name")]
        public string Name { get; set; } = null!;


        [XmlElement("ContractStartDate")]
        public string ContractStartDate { get; set; }


        [XmlElement("ContractEndDate")]
        public string ContractEndDate { get; set; }


        [XmlElement("PositionType")]
        public string PositionType { get; set; }


        [XmlElement("BestSkillType")]
        public string BestSkillType { get; set; }
    }
}
