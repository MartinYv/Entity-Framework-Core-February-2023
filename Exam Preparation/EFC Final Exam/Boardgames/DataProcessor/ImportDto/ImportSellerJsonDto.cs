using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Boardgames.DataProcessor.ImportDto
{
    public class ImportSellerJsonDto
    {
        [JsonProperty("Name")]
        [Required]
        [MinLength(5)]
        [MaxLength(20)]
        public string Name { get; set; } = null!;


        [Required]
        [MinLength(2)]
        [MaxLength(30)]
        [JsonProperty("Address")]
        public string Address { get; set; } = null!;


        [JsonProperty("Country")]
        [Required]
        public string Country { get; set; } = null!;


        [JsonProperty("Website")]
        [Required]
        [RegularExpression(@"www.[A-Za-z0-9\-]*.com")]
        public string Website { get; set; } = null!;


        [JsonProperty("Boardgames")]
        public int[] Boardgames { get; set; }
    }
}

//Name – text with length [5…20] (required)
// 
//  Address – text with length [2…30] (required)
// 
//  Country – text (required)
// www.[A - Za - z0 - 9\-] *.com
// 
//     "Name": "6am",
// 
// "Address": "The Netherlands",
// 
// "Country": "Belgium",
// 
// "Website": "www.6pm.com",
// 
// "Boardgames": [