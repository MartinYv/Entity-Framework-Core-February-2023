using Artillery.Data.Models.Enums;
using System.Xml.Serialization;

namespace Artillery.DataProcessor.ExportDto
{
    [XmlType("Gun")]
    public class ExportGunXmlDto
    {
        [XmlAttribute("Manufacturer")]
        public string Manufacturer { get; set; } = null!;

        [XmlAttribute("GunType")]
        public GunType GunType { get; set; }


        [XmlAttribute("GunWeight")]
        public int GunWeight { get; set; }

      
        [XmlAttribute("BarrelLength")]
        public double BarrelLength { get; set; }

        
        [XmlAttribute("Range")]
        public int Range { get; set; }


        [XmlArray("Countries")]
        public ExportCountryXmlDto[] Countries { get; set; }
    }
}

[XmlType("Country")]
public class ExportCountryXmlDto
{
    [XmlAttribute("Country")]
    public string CountryName { get; set; } = null!;

    [XmlAttribute("ArmySize")]
    public int ArmySize { get; set; }

}

//< Gun Manufacturer = "Krupp" GunType = "Mortar" GunWeight = "1291272" BarrelLength = "8.31" Range = "14258" >
//    < Countries >
//      < Country Country = "Sweden" ArmySize = "5437337" />
//      < Country Country = "Portugal" ArmySize = "9523599" />