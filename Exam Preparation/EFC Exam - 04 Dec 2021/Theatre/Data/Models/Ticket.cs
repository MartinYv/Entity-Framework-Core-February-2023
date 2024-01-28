using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Theatre.Data.Models
{
    public class Ticket
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public decimal Price { get; set; }

        [Required]
        public sbyte RowNumber { get; set; }


        [ForeignKey("Play")]
        public int PlayId { get; set; }
        public virtual Play Play { get; set; } = null!;


        [ForeignKey("Theatre")]
        public int TheatreId { get; set; }
        public virtual Theatre Theatre { get; set; } = null!;

    }
}
//	Id – integer, Primary Key
//	Price – decimal in the range [1.00….100.00] (required)
//	RowNumber – sbyte in range [1….10] (required)
//	PlayId – integer, foreign key (required)
//	TheatreId – integer, foreign key (required)
