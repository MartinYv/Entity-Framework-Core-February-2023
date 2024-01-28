namespace Trucks.DataProcessor
{
    using System.ComponentModel.DataAnnotations;
    using System.Text;
    using System.Xml.Serialization;
    using AutoMapper;
    using Data;
    using Newtonsoft.Json;
    using Trucks.Data.Models;
    using Trucks.Data.Models.Enums;
    using Trucks.DataProcessor.ImportDto;

    public class Deserializer
    {
        private const string ErrorMessage = "Invalid data!";

        private const string SuccessfullyImportedDespatcher
            = "Successfully imported despatcher - {0} with {1} trucks.";

        private const string SuccessfullyImportedClient
            = "Successfully imported client - {0} with {1} trucks.";

        public static string ImportDespatcher(TrucksContext context, string xmlString)
        {
            StringBuilder output = new StringBuilder();
            StringReader reader = new StringReader(xmlString);

            XmlRootAttribute root = new XmlRootAttribute("Despatchers");
            XmlSerializer serializer = new XmlSerializer(typeof(ImportDespatcherXmlDto[]), root);

            ImportDespatcherXmlDto[] despatcherDtos = (ImportDespatcherXmlDto[])serializer.Deserialize(reader);

            var validEntities = new HashSet<Despatcher>();

            foreach (var despatcherDto in despatcherDtos)
            {

                if (!IsValid(despatcherDto))
                {
                    output.AppendLine(ErrorMessage);
                    continue;
                }

                Despatcher despatcher = new Despatcher()
                {
                    Name = despatcherDto.Name,
                    Position = despatcherDto.Position
                };


                validEntities.Add(despatcher);

                foreach (var truckDto in despatcherDto.Trucks)
                {

                    if (!IsValid(truckDto))
                    {
                        output.AppendLine(ErrorMessage);
                        continue;
                    }

                    Truck truck = new Truck()
                    {
                        RegistrationNumber = truckDto.RegistrationNumber,
                        VinNumber = truckDto.VinNumber,
                        TankCapacity = truckDto.TankCapacity,
                        CargoCapacity = truckDto.CargoCapacity,
                        CategoryType = Enum.Parse<CategoryType>(truckDto.CategoryType),
                        MakeType = Enum.Parse<MakeType>(truckDto.MakeType)
                    };


                    despatcher.Trucks.Add(truck);
                }
                    output.AppendLine(string.Format(SuccessfullyImportedDespatcher, despatcher.Name, despatcher.Trucks.Count()));

            }

            context.AddRange(validEntities);
            context.SaveChanges();


                return output.ToString().TrimEnd();

        }
        public static string ImportClient(TrucksContext context, string jsonString)
        {
            StringBuilder output = new StringBuilder();

            ImportClientsJsonDto[] clientsDto = JsonConvert.DeserializeObject<ImportClientsJsonDto[]>(jsonString);

            List<int> trucksIds = context.Trucks.Select(t => t.Id).ToList();


            HashSet<Client> validClients = new HashSet<Client>();

            foreach (var clientDto in clientsDto)
            {

                if (!IsValid(clientDto))
                {
                    output.AppendLine(ErrorMessage);
                    continue;
                }

                if (clientDto.Type == "usual")
                {
                    output.AppendLine(ErrorMessage);
                    continue;
                }


                Client client = new Client()
                {
                    Name = clientDto.Name,
                    Nationality = clientDto.Nationality,
                    Type = clientDto.Type
                };



                foreach (var idDto in clientDto.Id.Distinct())
                {
                    if (!trucksIds.Contains(idDto))
                    {
                        output.AppendLine(ErrorMessage);
                        continue;
                    }

                    ClientTruck clientTruck = new ClientTruck()
                    {
                        Client = client,
                        TruckId = idDto
                    };

                    client.ClientsTrucks.Add(clientTruck);
                }

                validClients.Add(client);

                output.AppendLine(string.Format(SuccessfullyImportedClient, client.Name, client.ClientsTrucks.Count()));
            }


            context.AddRange(validClients);
            context.SaveChanges();

            return output.ToString().TrimEnd();
        }

        private static bool IsValid(object dto)
        {
            var validationContext = new ValidationContext(dto);
            var validationResult = new List<ValidationResult>();

            return Validator.TryValidateObject(dto, validationContext, validationResult, true);
        }
    }
}