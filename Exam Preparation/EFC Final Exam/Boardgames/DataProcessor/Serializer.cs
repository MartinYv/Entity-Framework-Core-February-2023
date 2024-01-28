namespace Boardgames.DataProcessor
{
    using Boardgames.Data;
    using Boardgames.DataProcessor.ExportDto;
    using Newtonsoft.Json;
    using System.Text;
    using System.Xml.Serialization;

    public class Serializer
    {
        public static string ExportCreatorsWithTheirBoardgames(BoardgamesContext context)
        {
            StringBuilder output = new StringBuilder();
            StringWriter writer = new StringWriter(output);      

            var creatorsToExport = context.Creators.Where(c => c.Boardgames.Any()).ToArray()
                .Select(c => new ExportCreatorXmlDto()
                {
                    Count = c.Boardgames.Count(),
                    CreatorName = $"{c.FirstName} {c.LastName}",
                    Boardgames = c.Boardgames.ToArray()
                    .Select(bg => new ExportBoardgameXmlExport()
                    {
                        BoardgameName = bg.Name,
                        YearPublished = bg.YearPublished
                    }).OrderBy(bg => bg.BoardgameName).ToArray()
                }).OrderByDescending(c => c.Boardgames.Count()).ThenBy(c => c.CreatorName).ToArray();


            XmlRootAttribute root = new XmlRootAttribute("Creators");

            XmlSerializerNamespaces namespaces = new XmlSerializerNamespaces();
            namespaces.Add(string.Empty, string.Empty);

            XmlSerializer serializer = new XmlSerializer(typeof(ExportCreatorXmlDto[]), root);
            serializer.Serialize(writer, creatorsToExport, namespaces);

            return output.ToString().Trim();
        }

        public static string ExportSellersWithMostBoardgames(BoardgamesContext context, int year, double rating)
        {
            var sellersToExport = context.Sellers
                .Where(s => s.BoardgamesSellers.Any(bg => bg.Boardgame.YearPublished >= year && bg.Boardgame.Rating <= rating))
                .Select(s => new
                {
                    s.Name,
                    s.Website,
                    Boardgames = s.BoardgamesSellers.Where(s => s.Boardgame.YearPublished >= year && s.Boardgame.Rating <= rating)
                    .Select(bg => new
                    {
                        bg.Boardgame.Name,
                        bg.Boardgame.Rating,
                        bg.Boardgame.Mechanics,
                        Category = bg.Boardgame.CategoryType.ToString()
                    }).OrderByDescending(bg => bg.Rating).ThenBy(bg => bg.Name).ToArray()
                }).OrderByDescending(s => s.Boardgames.Count()).ThenBy(s => s.Name).Take(5).ToArray();

            return JsonConvert.SerializeObject(sellersToExport, Formatting.Indented);
        }
    }
}