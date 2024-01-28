using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace Artillery.DataProcessor.ImportDto
{
    public class ImportGunJsonDto
    {

        [JsonProperty("ManufacturerId")]
        [Required]
        public int ManufacturerId { get; set; }


        [JsonProperty("GunWeight")]
        [Required]
            [Range(100, 1_350_000)]
        public int GunWeight { get; set; }


        [JsonProperty("BarrelLength")]
        [Required]
        [Range(2.00, 35.00)]
        public double BarrelLength { get; set; }


        [JsonProperty("NumberBuild")]
        public int? NumberBuild { get; set; }


        [JsonProperty("Range")]
        [Required]
        [Range(1, 100_000)]
        public int Range { get; set; }


        [JsonProperty("GunType")]
        [Required]
        public string GunType { get; set; } = null!;


        [JsonProperty("ShellId")]
        [Required]
        public int ShellId { get; set; }


        [JsonProperty("Countries")]
        public ImportCountryGunDto[] Countries { get; set; } = null!;

    }

    
    public class ImportCountryGunDto
    {
        [JsonProperty("Id")]
        public int Id { get; set; }
    }
}
