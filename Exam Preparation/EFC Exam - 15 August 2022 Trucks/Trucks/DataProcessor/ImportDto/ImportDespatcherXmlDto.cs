using System.ComponentModel.DataAnnotations;
using System.Xml.Serialization;

namespace Trucks.DataProcessor.ImportDto
{
    [XmlType("Despatcher")]
    public class ImportDespatcherXmlDto
    {
        [XmlElement("Name")]
        [Required]
        [MinLength(2)]
        [MaxLength(40)]
        public string Name { get; set; } = null!;

        [XmlElement("Position")]
        [Required]
        public string? Position { get; set; }

        [XmlArray("Trucks")]
        public ImportTruckDto[] Trucks { get; set; } = null!;
    }


    [XmlType("Truck")]
    public class ImportTruckDto
    {

        [XmlElement("RegistrationNumber")]
        [MinLength(8)]
        [MaxLength(8)]
        [RegularExpression(@"[A-Z]{2}[\d]{4}[A-Z]{2}")]
        public string RegistrationNumber { get; set; } = null!;


        [XmlElement("VinNumber")]
        [MinLength(17)]
        [MaxLength(17)]
        [Required]
        public string VinNumber { get; set; } = null!;


        [XmlElement("TankCapacity")]
        [Required]
        [Range(950,1420)]
        public int TankCapacity { get; set; }


        [XmlElement("CargoCapacity")]
        [Required]
        [Range(5000, 29000)]
        public int CargoCapacity { get; set; }


        [XmlElement("CategoryType")]
        [Required]
        public string CategoryType { get; set; } = null!;


        [XmlElement("MakeType")]
        [Required]
        public string MakeType { get; set; } = null!;

    }
}
