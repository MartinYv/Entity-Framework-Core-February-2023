namespace Trucks.DataProcessor
{
    using AutoMapper;
    using AutoMapper.QueryableExtensions;
    using Data;
    using Newtonsoft.Json;
    using System.Security.Cryptography.X509Certificates;
    using System.Text;
    using System.Xml.Serialization;
    using Trucks.Data.Models.Enums;
    using Trucks.DataProcessor.ExportDto;

    public class Serializer
    {
        public static string ExportDespatchersWithTheirTrucks(TrucksContext context)
        {
            StringBuilder output = new StringBuilder();
            StringWriter writer = new StringWriter(output);

            XmlSerializerNamespaces namespaces= new XmlSerializerNamespaces();
            namespaces.Add(string.Empty, string.Empty);

            
            ExportDespatchersXmlDto[] despatchers = context.Despatchers.Where(d => d.Trucks.Any())
                .Select(d => new ExportDespatchersXmlDto()
                {
                    DespatcherName = d.Name,
                    TrucksCount = d.Trucks.Count(),
                    Trucks = d.Trucks.Select(t => new ExportTruckXmlDto()
                    {
                        RegistrationNumber = t.RegistrationNumber,
                        Make = t.MakeType.ToString()
                    }).OrderBy(t => t.RegistrationNumber).ToArray()
                }).OrderByDescending(d => d.TrucksCount).ThenBy(d => d.DespatcherName).ToArray();


            XmlRootAttribute root = new XmlRootAttribute("Despatchers");

            XmlSerializer serializer = new XmlSerializer(typeof(ExportDespatchersXmlDto[]), root);
            serializer.Serialize(writer, despatchers, namespaces);

            return output.ToString().TrimEnd();

        }

        public static string ExportClientsWithMostTrucks(TrucksContext context, int capacity)
        {

            var clients = context.Clients.Where(c => c.ClientsTrucks.Any(c => c.Truck.TankCapacity >= capacity)).ToArray()
                 .Select(c => new
                 {
                     Name = c.Name,
                     Trucks = c.ClientsTrucks.Where(t => t.Truck.TankCapacity >= capacity)
                     .Select(t => new
                     {
                         TruckRegistrationNumber = t.Truck.RegistrationNumber,
                         VinNumber = t.Truck.VinNumber,
                         TankCapacity = t.Truck.TankCapacity,
                         CargoCapacity = t.Truck.CargoCapacity,
                         CategoryType = t.Truck.CategoryType.ToString(),
                         MakeType = t.Truck.MakeType.ToString()
                     }).ToArray().OrderBy(t => t.MakeType).ThenByDescending(t => t.CargoCapacity)
                 }).OrderByDescending(c => c.Trucks.Count()).ThenBy(c => c.Name).Take(10).ToArray();

            return JsonConvert.SerializeObject(clients, Formatting.Indented);
        }
    }
}
