using System.ComponentModel.DataAnnotations.Schema;

namespace Footballers.Data.Models
{
    public class TeamFootballer
    {
        [ForeignKey("Team")]
        public int TeamId { get; set; }
        public virtual Team Team { get; set; } = null!;


        [ForeignKey("Footballer")]
        public int FootballerId { get; set; }
        public virtual Footballer Footballer { get; set; } = null!;
    }

}
