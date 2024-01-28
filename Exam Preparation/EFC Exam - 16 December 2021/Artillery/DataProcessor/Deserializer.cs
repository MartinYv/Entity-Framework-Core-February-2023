namespace Artillery.DataProcessor
{
    using Artillery.Data;
    using Artillery.Data.Models;
    using Artillery.Data.Models.Enums;
    using Artillery.DataProcessor.ImportDto;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.Text;
    using System.Xml.Serialization;

    public class Deserializer
    {
        private const string ErrorMessage =
            "Invalid data.";
        private const string SuccessfulImportCountry =
            "Successfully import {0} with {1} army personnel.";
        private const string SuccessfulImportManufacturer =
            "Successfully import manufacturer {0} founded in {1}.";
        private const string SuccessfulImportShell =
            "Successfully import shell caliber #{0} weight {1} kg.";
        private const string SuccessfulImportGun =
            "Successfully import gun {0} with a total weight of {1} kg. and barrel length of {2} m.";

        public static string ImportCountries(ArtilleryContext context, string xmlString)
        {
            StringBuilder output = new StringBuilder();
            StringReader reader = new StringReader(xmlString);

                XmlRootAttribute root = new XmlRootAttribute("Countries");


            XmlSerializer serializer = new XmlSerializer(typeof(ImportCountriesXmlDto[]), root);

            ImportCountriesXmlDto[] countriesDtos = (ImportCountriesXmlDto[])serializer.Deserialize(reader);


            HashSet<Country> validEntities = new HashSet<Country>();

            foreach (var countryDto in countriesDtos)
            {
                if (!IsValid(countryDto))
                {
                    output.AppendLine(ErrorMessage);
                    continue;
                }

                Country country = new Country()
                {
                    CountryName = countryDto.CountryName,
                    ArmySize = countryDto.ArmySize
                };

                validEntities.Add(country);

                output.AppendLine(string.Format(SuccessfulImportCountry, country.CountryName, country.ArmySize));
            }

            context.AddRange(validEntities);
            context.SaveChanges();

            return output.ToString().TrimEnd();
        }

        public static string ImportManufacturers(ArtilleryContext context, string xmlString)
        {
            StringBuilder output = new StringBuilder();
            StringReader reader = new StringReader(xmlString);

            XmlRootAttribute root = new XmlRootAttribute("Manufacturers");

            XmlSerializer serializer = new XmlSerializer(typeof(ImportManufacturerXmlDto[]), root);

            ImportManufacturerXmlDto[] manufacturersDtos = (ImportManufacturerXmlDto[])serializer.Deserialize(reader);


            HashSet<Manufacturer> validEntities = new HashSet<Manufacturer>();

            foreach (var manufacturerDto in manufacturersDtos)
            {
                if (!IsValid(manufacturerDto))
                {
                    output.AppendLine(ErrorMessage);
                    continue;
                }

                if (validEntities.Any(m=>m.ManufacturerName == manufacturerDto.ManufacturerName))
                {
                    output.AppendLine(ErrorMessage);
                    continue;
                }

                Manufacturer manufacturer = new Manufacturer()
                {
                    ManufacturerName= manufacturerDto.ManufacturerName,
                    Founded= manufacturerDto.Founded
                };

                string[] founded = manufacturer.Founded.Split(", ", StringSplitOptions.RemoveEmptyEntries).ToArray();

                string townName = founded[founded.Length - 2];
                string countryName = founded[founded.Length - 1];
                string townNameCountryName = string.Concat($"{townName}, {countryName}");

                validEntities.Add(manufacturer);

                output.AppendLine(string.Format(SuccessfulImportManufacturer, manufacturer.ManufacturerName, townNameCountryName));
            }

            context.AddRange(validEntities);
            context.SaveChanges();

            return output.ToString().Trim();
        }

        public static string ImportShells(ArtilleryContext context, string xmlString)
        {
            StringBuilder output = new StringBuilder();
            StringReader reader = new StringReader(xmlString);
           
          
            XmlRootAttribute root = new XmlRootAttribute("Shells");


            XmlSerializer serializer = new XmlSerializer(typeof(ImportShellXmlDto[]), root);

            ImportShellXmlDto[] shellsDtos = (ImportShellXmlDto[])serializer.Deserialize(reader);


            HashSet<Shell> validEntities = new HashSet<Shell>();

            foreach (var shellDto in shellsDtos)
            {
                if (!IsValid(shellDto))
                {
                    output.AppendLine(ErrorMessage);
                    continue;
                }

                Shell shell = new Shell()
                {
                    ShellWeight= shellDto.ShellWeight,
                    Caliber= shellDto.Caliber
                };

               
                validEntities.Add(shell);

                output.AppendLine(string.Format(SuccessfulImportShell, shell.Caliber, shell.ShellWeight));
            }

            context.AddRange(validEntities);
            context.SaveChanges();

            return output.ToString().TrimEnd();
        }

        public static string ImportGuns(ArtilleryContext context, string jsonString)
        {
            StringBuilder output = new StringBuilder();
            ImportGunJsonDto[] gunsDtos= JsonConvert.DeserializeObject<ImportGunJsonDto[]>(jsonString);

            HashSet<Gun>validEntities = new HashSet<Gun>();

            foreach (var gunDto in gunsDtos)
            {

                if (!IsValid(gunDto))
                {
                    output.AppendLine(ErrorMessage);
                    continue;
                }
                if (!Enum.IsDefined(typeof(GunType), gunDto.GunType))
                {
                    output.AppendLine(ErrorMessage);
                    continue;
                }

                    Gun gun = new Gun()
                {
                    ManufacturerId = gunDto.ManufacturerId,
                    GunWeight = gunDto.GunWeight,
                    BarrelLength = gunDto.BarrelLength,
                    NumberBuild = gunDto.NumberBuild,
                    Range = gunDto.Range,
                    GunType = Enum.Parse<GunType>(gunDto.GunType),
                    ShellId = gunDto.ShellId
                };

                validEntities.Add(gun);

                foreach (var countryDto in gunDto.Countries)
                {

                    CountryGun countryGun = new CountryGun()
                    {
                        Gun = gun,
                        CountryId = countryDto.Id
                    };

                    gun.CountriesGuns.Add(countryGun);

                }

                output.AppendLine(string.Format(SuccessfulImportGun, gun.GunType, gun.GunWeight, gun.BarrelLength));
            }

            context.AddRange(validEntities);
            context.SaveChanges();


            return output.ToString().TrimEnd();
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