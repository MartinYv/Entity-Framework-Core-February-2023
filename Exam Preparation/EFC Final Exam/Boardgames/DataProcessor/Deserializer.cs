namespace Boardgames.DataProcessor
{
    using System.ComponentModel.DataAnnotations;
    using System.Text;
    using System.Xml.Serialization;
    using Boardgames.Data;
    using Boardgames.Data.Models;
    using Boardgames.Data.Models.Enums;
    using Boardgames.DataProcessor.ImportDto;
    using Newtonsoft.Json;

    public class Deserializer
    {
        private const string ErrorMessage = "Invalid data!";

        private const string SuccessfullyImportedCreator
            = "Successfully imported creator – {0} {1} with {2} boardgames.";

        private const string SuccessfullyImportedSeller
            = "Successfully imported seller - {0} with {1} boardgames.";

        public static string ImportCreators(BoardgamesContext context, string xmlString)
        {
            StringBuilder output = new StringBuilder();
            StringReader reader = new StringReader(xmlString);

            XmlRootAttribute root = new XmlRootAttribute("Creators");

            XmlSerializer serializer = new XmlSerializer(typeof(ImportCreatorXmlDto[]), root);

            ImportCreatorXmlDto[] creatorDtos = (ImportCreatorXmlDto[])serializer.Deserialize(reader);

            HashSet<Creator> validEntities = new HashSet<Creator>();

            foreach (var creatorDto in creatorDtos)
            {
                if (!IsValid(creatorDto))
                {
                    output.AppendLine(ErrorMessage);
                    continue;
                }

                Creator creator = new Creator()
                {
                    FirstName = creatorDto.FirstName,
                    LastName = creatorDto.LastName
                };

                validEntities.Add(creator);


                foreach (var boardgameDto in creatorDto.Boardgames)
                {
                    if (!IsValid(boardgameDto))
                    {
                        output.AppendLine(ErrorMessage);
                        continue;
                    }

                    Boardgame boardgame = new Boardgame()
                    {
                        Name = boardgameDto.Name,
                        YearPublished = boardgameDto.YearPublished,
                        CategoryType = Enum.Parse<CategoryType>(boardgameDto.CategoryType),
                        Rating = boardgameDto.Rating,
                        Mechanics = boardgameDto.Mechanics
                    };

                    creator.Boardgames.Add(boardgame);
                }

                output.AppendLine(string.Format(SuccessfullyImportedCreator, creator.FirstName, creator.LastName, creator.Boardgames.Count()));
            }

            context.AddRange(validEntities);
            context.SaveChanges();

            return output.ToString().Trim();
        }

        public static string ImportSellers(BoardgamesContext context, string jsonString)
        {
            StringBuilder output = new StringBuilder();

            List<int> validBoardGamesIds = context.Boardgames.Select(b => b.Id).ToList();

            HashSet<Seller> validEntities = new HashSet<Seller>();


            ImportSellerJsonDto[] sellerDtos = JsonConvert.DeserializeObject<ImportSellerJsonDto[]>(jsonString);

            foreach (var sellerDto in sellerDtos)
            {
                if (!IsValid(sellerDto))
                {
                    output.AppendLine(ErrorMessage);
                    continue;
                }

                Seller seller = new Seller()
                {
                    Name = sellerDto.Name,
                    Address = sellerDto.Address,
                    Website = sellerDto.Website,
                    Country = sellerDto.Country
                };

                validEntities.Add(seller);

                foreach (var boardGameDtoId in sellerDto.Boardgames.Distinct())
                {
                    if (!validBoardGamesIds.Contains(boardGameDtoId))
                    {
                        output.AppendLine(ErrorMessage);
                        continue;
                    }

                    BoardgameSeller boardgameSeller = new BoardgameSeller()
                    {
                        Seller = seller,
                        BoardgameId = boardGameDtoId
                    };

                    seller.BoardgamesSellers.Add(boardgameSeller);
                }


                output.AppendLine(string.Format(SuccessfullyImportedSeller, seller.Name, seller.BoardgamesSellers.Count()));
            }

            context.AddRange(validEntities);
            context.SaveChanges();

            return output.ToString().Trim();
        }

        private static bool IsValid(object dto)
        {
            var validationContext = new ValidationContext(dto);
            var validationResult = new List<ValidationResult>();

            return Validator.TryValidateObject(dto, validationContext, validationResult, true);
        }
    }
}
