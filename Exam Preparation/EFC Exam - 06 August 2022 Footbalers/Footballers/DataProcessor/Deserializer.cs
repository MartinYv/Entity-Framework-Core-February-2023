namespace Footballers.DataProcessor
{
    using Footballers.Data;
    using Footballers.Data.Models;
    using Footballers.DataProcessor.ImportDto;
    using Newtonsoft.Json;
    using System.ComponentModel.DataAnnotations;
    using System.Runtime.CompilerServices;
    using System.Text;
    using System.Xml.Serialization;

    public class Deserializer
    {
        private const string ErrorMessage = "Invalid data!";

        private const string SuccessfullyImportedCoach
            = "Successfully imported coach - {0} with {1} footballers.";

        private const string SuccessfullyImportedTeam
            = "Successfully imported team - {0} with {1} footballers.";

        public static string ImportCoaches(FootballersContext context, string xmlString)
        {
            //XmlRootAttribute root = new XmlRootAttribute("Sales");
            //XmlSerializer xmlSerializer = new XmlSerializer(typeof(ImportSalesDto[]), root);
            //StringReader reader = new StringReader(inputXml);
            //ImportSalesDto[] salesDtos = (ImportSalesDto[])xmlSerializer.Deserialize(reader);


            StringBuilder output = new StringBuilder();


            //StringReader reader = new StringReader(File.ReadAllText("../../../Datasets/coaches.xml"));
            StringReader reader = new StringReader(xmlString);


            XmlRootAttribute root = new XmlRootAttribute("Coaches");
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(ImportCoachesAndFootballersDtoXml[]), root);


            ImportCoachesAndFootballersDtoXml[] coachesAndFootballersDto = (ImportCoachesAndFootballersDtoXml[])xmlSerializer.Deserialize(reader);

            HashSet<Coach> coaches = new HashSet<Coach>();



            foreach (var coachDto in coachesAndFootballersDto)
            {

                if (string.IsNullOrEmpty(coachDto.Name) || string.IsNullOrEmpty(coachDto.Nationality) || coachDto.Name.Length > 40)
                {
                    output.AppendLine(ErrorMessage);
                    continue;
                }


                Coach coach = new Coach()
                {
                    Name = coachDto.Name,
                    Nationality = coachDto.Nationality,
                };
                coaches.Add(coach);


                foreach (var footballerDto in coachDto.Footballers)
                {
                   //|| Enum.IsDefined(typeof(ENUMCLASS), What we are checking) check is the enum representation is there
                        
                    if (string.IsNullOrWhiteSpace(footballerDto.Name) || string.IsNullOrWhiteSpace(footballerDto.ContractStartDate) ||
                        string.IsNullOrWhiteSpace(footballerDto.ContractEndDate) || string.IsNullOrWhiteSpace(footballerDto.PositionType) ||
                        string.IsNullOrWhiteSpace(footballerDto.BestSkillType) ||
                        footballerDto.Name.Length > 40 )
                    {
                        output.AppendLine(ErrorMessage);
                        continue;
                    }


                    DateTime startDate = DateTime.ParseExact(footballerDto.ContractStartDate, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);
                    DateTime endDate = DateTime.ParseExact(footballerDto.ContractEndDate, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);

                    if (startDate > endDate)
                    {
                        output.AppendLine(ErrorMessage);
                        continue;
                    }


                    Footballer footballer = new Footballer()
                    {
                        Name = footballerDto.Name,
                        ContractStartDate = DateTime.ParseExact(footballerDto.ContractStartDate, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture),
                        ContractEndDate = DateTime.ParseExact(footballerDto.ContractEndDate, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture),
                        PositionType = Enum.Parse<PositionType>(footballerDto.PositionType),
                        BestSkillType = Enum.Parse<BestSkillType>(footballerDto.BestSkillType)
                    };

                    coach.Footballers.Add(footballer);

                }

                output.AppendFormat(SuccessfullyImportedCoach, coach.Name, coach.Footballers.Count);
                output.AppendLine();
            }

            context.AddRange(coaches);
            context.SaveChanges();


            return output.ToString().TrimEnd();
        }

    public static string ImportTeams(FootballersContext context, string jsonString)
    {
            StringBuilder output = new StringBuilder();

            ImportTeamJsonDto[] teamsDto= JsonConvert.DeserializeObject<ImportTeamJsonDto[]>(jsonString);

            HashSet<Team> validTeams = new HashSet<Team>();

            foreach (var teamDto in teamsDto)
            {
                if (!IsValid(teamDto) || teamDto.Trophies == 0)
                {
                    output.AppendLine(ErrorMessage);
                    continue;
                }

                Team team = new Team()
                {
                    Name = teamDto.Name,
                    Nationality = teamDto.Nationality,
                    Trophies = teamDto.Trophies
                };

                validTeams.Add(team);


                foreach (var footballerDto in teamDto.Footballers.Distinct())
                {

                    if (!context.Footballers.Any(f=>f.Id == footballerDto))
                    {
                        output.AppendLine(ErrorMessage);
                        continue;
                    }

                    Footballer footballer = context.Footballers.First(f => f.Id == footballerDto);

                    TeamFootballer teamFootbaler = new TeamFootballer()
                    {
                        Team = team,
                        Footballer= footballer
                    };

                    
                    team.TeamsFootballers.Add(teamFootbaler);

                }
                    output.AppendFormat(SuccessfullyImportedTeam, team.Name, team.TeamsFootballers.Count());
                    output.AppendLine();

            }

            context.AddRange(validTeams);
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
