namespace Footballers.DataProcessor
{
    using Data;
    using Newtonsoft.Json;
    using System.Globalization;
    using System.Xml.Serialization;
    using Footballers.DataProcessor.ExportDto;
    using System.Text;
    using AutoMapper.QueryableExtensions;
    using AutoMapper;

    public class Serializer
    {
       public static string ExportCoachesWithTheirFootballers(FootballersContext context)
       {
          
            XmlSerializerNamespaces namespases = new XmlSerializerNamespaces();
            namespases.Add(string.Empty, string.Empty);
            
            StringBuilder output = new StringBuilder();
            StringWriter writer = new StringWriter(output);


            XmlRootAttribute root = new XmlRootAttribute("Coaches");
            XmlSerializer serializer = new XmlSerializer(typeof(ExportTeamDto[]), root);

            var exportDto = context.Coaches.Where(c => c.Footballers.Any())
                .Select(x => new ExportTeamDto
                {
                    FootballersCount = x.Footballers.Count(),
                    Name=x.Name,
                    Footballers = x.Footballers.OrderBy(f => f.Name).ToArray().Select(a => new ExportFootballerDto
                    {
                        Name = a.Name,
                        PositionType = a.PositionType.ToString()
                    }).ToArray()
                }).OrderByDescending(x => x.FootballersCount)
                .ToArray();


            serializer.Serialize(writer, exportDto, namespases);
            return output.ToString().TrimEnd();
       }

        public static string ExportTeamsWithMostFootballers(FootballersContext context, DateTime date)
        {
            var exportTeams = context.Teams
                .Where(tf => tf.TeamsFootballers.Any(f => f.Footballer.ContractStartDate >= date))
                .ToArray()
                 .Select(t => new
                 {
                     t.Name,
                     Footballers = t.TeamsFootballers.Where(t=>t.Footballer.ContractStartDate >= date)
                     .OrderByDescending(f => f.Footballer.ContractEndDate)
                     .ThenBy(f => f.Footballer.Name)
                     .ToArray()
                     .Select(f => new
                     {
                         FootballerName = f.Footballer.Name,
                         ContractStartDate = f.Footballer.ContractStartDate.ToString("d", CultureInfo.InvariantCulture),
                         ContractEndDate = f.Footballer.ContractEndDate.ToString("d", CultureInfo.InvariantCulture),
                         BestSkillType = f.Footballer.BestSkillType.ToString(),
                         PositionType = f.Footballer.PositionType.ToString()
                     })
                 }).ToArray().OrderByDescending(t => t.Footballers.Count()).ThenBy(t => t.Name)
                 .Take(5)
                 .ToArray();



            return JsonConvert.SerializeObject(exportTeams, Formatting.Indented);
        }
    }
}
