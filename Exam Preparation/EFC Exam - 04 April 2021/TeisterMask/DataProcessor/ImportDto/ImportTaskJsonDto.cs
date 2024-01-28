using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeisterMask.DataProcessor.ImportDto
{
   
    public class ImportTaskJsonDto
    {
        [JsonProperty("Id")]
        public int TaskId { get; set; }
    }
}
