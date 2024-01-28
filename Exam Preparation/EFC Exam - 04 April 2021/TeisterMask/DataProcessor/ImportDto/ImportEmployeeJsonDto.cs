using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeisterMask.DataProcessor.ImportDto
{
   public class ImportEmployeeJsonDto
    {
        [JsonProperty("Username")]
        [RegularExpression(@"^[A-Za-z0-9]{3,}$")]
        [Required]
        [MinLength(3)]
        [MaxLength(40)]
        public string Username { get; set; } = null!;

        [JsonProperty("Email")]
        [Required]
        [EmailAddress]
        public string Email { get; set; } = null!;

        [JsonProperty("Phone")]
        [Required]
        [RegularExpression(@"\d{3}-\d{3}-\d{4}")]
        public string Phone { get; set; } = null!;

        [JsonProperty("Tasks")]
        public int[] Tasks { get; set; }
    }
}
