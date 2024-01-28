using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Artillery.Data.Models
{
    public class Manufacturer
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(40)]
        public string ManufacturerName { get; set; } = null!;

        [Required]
        public string Founded { get; set; } = null!;
        public virtual ICollection<Gun> Guns { get; set; } = new HashSet<Gun>();

    }
}
//	Id – integer, Primary Key
//	ManufacturerName – unique text with length [4…40] (required)
//	Founded – text with length [10…100] (required)
//	Guns – a collection of Gun
