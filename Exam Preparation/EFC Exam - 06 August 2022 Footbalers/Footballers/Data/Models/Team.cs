using System.ComponentModel.DataAnnotations;

namespace Footballers.Data.Models
{
    public class Team
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MinLength(3)]
        [MaxLength(40)]
        [RegularExpression("[a-zA-Z\\.\\-\\ \\d]")]
        public string  Name { get; set; } = null!; 

        [Required]
        [MinLength(3)]
        [MaxLength(40)]
        public string Nationality { get; set; } = null!;
       
        
        [Required]
        public int Trophies { get; set; }

        public virtual ICollection<TeamFootballer> TeamsFootballers { get; set; } = new HashSet<TeamFootballer>();


    }

//   Id – integer, Primary Key
//
// Name – text with length[3, 40]. Should contain letters(lower and upper case), digits, spaces, a dot sign('.') and a dash('-'). (required)
//
// Nationality – text with length[2, 40] (required)
//
// Trophies – integer(required)
//
// TeamsFootballers – collection of type TeamFootballer
}
