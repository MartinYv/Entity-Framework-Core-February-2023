using System.ComponentModel.DataAnnotations;

namespace Footballers.Data.Models
{
    public class Coach
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MinLength(2)]
        [MaxLength(40)]
        public string  Name { get; set; } = null!;

        [Required]
        public string Nationality { get; set; } = null!;

        public virtual ICollection<Footballer> Footballers { get; set; } = new HashSet<Footballer>();
    }

//   Id – integer, Primary Key
//
// Name – text with length[2, 40] (required)
//
// Nationality – text(required)
//
// Footballers – collection of type Footballer
}
