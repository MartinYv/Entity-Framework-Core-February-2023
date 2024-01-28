using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace Trucks.DataProcessor.ImportDto
{
    public class ImportClientsJsonDto
    {
        [Required]
        [MinLength(3)]
        [MaxLength(40)]
        [JsonProperty("Name")]
        public string Name { get; set; } = null!;


        [Required]
        [MinLength(2)]
        [MaxLength(40)]
        [JsonProperty("Nationality")]

        public string Nationality { get; set; } = null!;


        [Required]
        [JsonProperty("Type")]
        public string Type { get; set; } = null!;


        [JsonProperty("Trucks")]
        public int[] Id { get; set; } = null!;
    }

}
