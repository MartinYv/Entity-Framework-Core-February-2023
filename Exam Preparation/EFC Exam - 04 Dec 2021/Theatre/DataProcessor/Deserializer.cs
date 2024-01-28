namespace Theatre.DataProcessor
{
    using Newtonsoft.Json;
    using System.ComponentModel.DataAnnotations;
    using System.Globalization;
    using System.Text;
    using System.Xml.Serialization;
    using Theatre.Data;
    using Theatre.Data.Models;
    using Theatre.Data.Models.Enums;
    using Theatre.DataProcessor.ImportDto;

    public class Deserializer
    {
        private const string ErrorMessage = "Invalid data!";

        private const string SuccessfulImportPlay
            = "Successfully imported {0} with genre {1} and a rating of {2}!";

        private const string SuccessfulImportActor
            = "Successfully imported actor {0} as a {1} character!";

        private const string SuccessfulImportTheatre
            = "Successfully imported theatre {0} with #{1} tickets!";



        public static string ImportPlays(TheatreContext context, string xmlString)
        {
            StringBuilder output = new StringBuilder();
            StringReader reader = new StringReader(xmlString);

            XmlRootAttribute root = new XmlRootAttribute("Plays");
            XmlSerializer serializer = new XmlSerializer(typeof(ImportPlaysXmlDto[]), root);

            ImportPlaysXmlDto[] playsDtos = (ImportPlaysXmlDto[])serializer.Deserialize(reader);

            HashSet<Play> validEntities = new HashSet<Play>();


            foreach (var playDto in playsDtos)
            {

                if (!IsValid(playDto))
                {
                    output.AppendLine(ErrorMessage);
                    continue;
                }

                if (!Enum.IsDefined(typeof(Genre), playDto.Genre))
                {
                    output.AppendLine(ErrorMessage);
                    continue;
                }


                if (TimeSpan.ParseExact(playDto.Duration, "c", CultureInfo.InvariantCulture) < new TimeSpan(1, 0, 0))
                {
                    output.AppendLine(ErrorMessage);
                    continue;
                }

                Play play = new Play()
                {
                    Title = playDto.Title,
                    Duration = TimeSpan.ParseExact(playDto.Duration, @"c", CultureInfo.InvariantCulture),
                    Rating = playDto.Rating,
                    Genre = Enum.Parse<Genre>(playDto.Genre),
                    Description = playDto.Description,
                    Screenwriter = playDto.Screenwriter
                };


                validEntities.Add(play);
                output.AppendLine(string.Format(SuccessfulImportPlay, play.Title, play.Genre.ToString(), play.Rating));
            }


            context.AddRange(validEntities);
            context.SaveChanges();


            return output.ToString().Trim();
        }

        public static string ImportCasts(TheatreContext context, string xmlString)
        {
            StringBuilder output = new StringBuilder();
            StringReader reader = new StringReader(xmlString);

            XmlRootAttribute root = new XmlRootAttribute("Casts");
            XmlSerializer serializer = new XmlSerializer(typeof(ImportCastXmlDto[]), root);

            ImportCastXmlDto[] castsDtos = (ImportCastXmlDto[])serializer.Deserialize(reader);

            HashSet<Cast> validEntities = new HashSet<Cast>();

            foreach (var castDto in castsDtos)
            {

                if (!IsValid(castDto))
                {
                    output.AppendLine(ErrorMessage);
                    continue;
                }

                Cast cast = new Cast()
                {
                    FullName = castDto.FullName,
                    PhoneNumber = castDto.PhoneNumber,
                    IsMainCharacter = castDto.IsMainCharacter,
                    PlayId = castDto.PlayId
                };


                validEntities.Add(cast);
                output.AppendLine(string.Format(SuccessfulImportActor, cast.FullName, cast.IsMainCharacter == true ? "main" : "lesser"));
            }

            context.AddRange(validEntities);
            context.SaveChanges();


            return output.ToString().Trim();
        }

        public static string ImportTtheatersTickets(TheatreContext context, string jsonString)
        {
            StringBuilder output = new StringBuilder();

            HashSet<Theatre> validEntities = new HashSet<Theatre>();

            ImportTeathreJsonDto[] theatresDtos = JsonConvert.DeserializeObject<ImportTeathreJsonDto[]>(jsonString);

            foreach (var theatreDto in theatresDtos)
            {

                if (!IsValid(theatreDto))
                {
                    output.AppendLine(ErrorMessage);
                    continue;
                }

                Theatre theatre = new Theatre()
                {
                    Name = theatreDto.Name,
                    NumberOfHalls = theatreDto.NumberOfHalls,
                    Director = theatreDto.Director
                };

                foreach (var ticketDto in theatreDto.Tickets)
                {
                    if (!IsValid(ticketDto))
                    {
                        output.AppendLine(ErrorMessage);
                        continue;
                    }

                    Ticket ticket = new Ticket()
                    {
                        Price = ticketDto.Price,
                        RowNumber = ticketDto.RowNumber,
                        PlayId = ticketDto.PlayId
                    };


                    theatre.Tickets.Add(ticket);
                }


                if (theatre.Tickets.Count > 0)
                {
                    validEntities.Add(theatre);
                    output.AppendLine(string.Format(SuccessfulImportTheatre, theatre.Name, theatre.Tickets.Count));
                }
                else
                {
                    continue;
                }

            }

            context.AddRange(validEntities);
            context.SaveChanges();

            return output.ToString().Trim();
        }


        private static bool IsValid(object obj)
        {
            var validator = new ValidationContext(obj);
            var validationRes = new List<ValidationResult>();

            var result = Validator.TryValidateObject(obj, validator, validationRes, true);
            return result;
        }
    }
}
