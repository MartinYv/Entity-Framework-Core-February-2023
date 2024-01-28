using Artillery.Data;
using Artillery.DataProcessor.ExportDto;
using Newtonsoft.Json;
using System.Text;
using System.Xml.Serialization;

namespace Artillery.DataProcessor
{
    public class Serializer
    {
        public static string ExportShells(ArtilleryContext context, double shellWeight)
        {
            var shellsToExport = context.Shells
                .Where(s => s.ShellWeight > shellWeight).ToArray()
                  .Select(s => new
                  {
                      ShellWeight = s.ShellWeight,
                      Caliber = s.Caliber,
                      Guns = s.Guns.Where(g=>g.GunType.ToString() == "AntiAircraftGun").ToArray().Select(g => new
                      {
                          GunType= g.GunType.ToString(),
                          GunWeight = g.GunWeight,
                          BarrelLength = g.BarrelLength,
                          Range = g.Range > 3000 ? "Long-range" : "Regular range"
                      }).OrderByDescending(g => g.GunWeight).ToArray()
                  }).OrderBy(s=>s.ShellWeight).ToArray();

            return JsonConvert.SerializeObject(shellsToExport, Formatting.Indented);

        }

        public static string ExportGuns(ArtilleryContext context, string manufacturer)
        {       

            var exportGunsDto = context.Guns.Where(g => g.Manufacturer.ManufacturerName == manufacturer).ToArray()
                .Select(g => new ExportGunXmlDto()
                {
                    Manufacturer = g.Manufacturer.ManufacturerName,
                    GunType = g.GunType,
                    GunWeight = g.GunWeight,
                    BarrelLength = g.BarrelLength,
                    Range = g.Range,
                    Countries = g.CountriesGuns.Where(c => c.Country.ArmySize > 4_500_000).ToArray()
                    .Select(c => new ExportCountryXmlDto()
                    {
                        CountryName = c.Country.CountryName,
                        ArmySize = c.Country.ArmySize
                    }).OrderBy(c => c.ArmySize).ToArray()
                }).OrderBy(g => g.BarrelLength).ToArray();


            StringBuilder output = new StringBuilder();
            StringWriter writer = new StringWriter(output);

            XmlRootAttribute root = new XmlRootAttribute("Guns");

            XmlSerializer serializer = new XmlSerializer(typeof(ExportGunXmlDto[]), root);

            XmlSerializerNamespaces namespaces = new XmlSerializerNamespaces();
            namespaces.Add(string.Empty, string.Empty);

            serializer.Serialize(writer, exportGunsDto, namespaces);

            return output.ToString().TrimEnd();
        }
    }
}
