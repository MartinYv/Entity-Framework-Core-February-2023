namespace Theatre.DataProcessor
{
    using Newtonsoft.Json;
    using System.Globalization;
    using System.Text;
    using System.Xml.Serialization;
    using Theatre.Data;
    using Theatre.DataProcessor.ExportDto;

    public class Serializer
    {
        public static string ExportTheatres(TheatreContext context, int numbersOfHalls)
        {
            var theatresToExport = context.Theatres.Where(t => t.NumberOfHalls >= numbersOfHalls && t.Tickets.Count() >= 20)
                .Select(t => new
                {
                    t.Name,
                    Halls = t.NumberOfHalls,
                    TotalIncome = t.Tickets.Where(ti => ti.RowNumber >= 1 && ti.RowNumber <= 5).Sum(ti => ti.Price),
                    Tickets = t.Tickets.Where(ti => ti.RowNumber >= 1 && ti.RowNumber <= 5).ToArray()
                    .Select(ti => new
                    {
                        ti.Price,
                        ti.RowNumber
                    }).OrderByDescending(ti => ti.Price).ToArray()
                }).OrderByDescending(t => t.Halls).ThenBy(t => t.Name).ToArray();


            return JsonConvert.SerializeObject(theatresToExport, Formatting.Indented);
        }

        public static string ExportPlays(TheatreContext context, double raiting)
        {

            StringBuilder output = new StringBuilder();
            StringWriter writer = new StringWriter(output);

            XmlRootAttribute root = new XmlRootAttribute("Plays");
            XmlSerializer serializer = new XmlSerializer(typeof(ExportPlayXmlDto[]),root);

            XmlSerializerNamespaces namespaces = new XmlSerializerNamespaces();
            namespaces.Add(string.Empty, string.Empty);


            var playsToExport = context.Plays.Where(p => p.Rating <= raiting)
                 .Select(p => new ExportPlayXmlDto()
                 {
                     Title = p.Title,
                     Duration = p.Duration.ToString("c", CultureInfo.InvariantCulture),
                     Rating = p.Rating == 0 ? "Premier" : p.Rating.ToString(),
                     Genre = p.Genre,
                     Actors = p.Casts.Where(c => c.IsMainCharacter == true).ToArray()
                     .Select(c => new ExportCastXmlDto()
                     {
                         FullName = c.FullName,
                         MainCharacter = $"Plays main character in '{p.Title}'."
                     }).OrderByDescending(c => c.FullName).ToArray()
                 }).OrderBy(p => p.Title).ThenByDescending(p => p.Genre).ToArray();


            serializer.Serialize(writer, playsToExport, namespaces);

            return output.ToString().Trim();

        }
    }
}