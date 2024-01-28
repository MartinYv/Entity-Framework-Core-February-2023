﻿using System.ComponentModel.DataAnnotations;
using Theatre.Data.Models.Enums;

namespace Theatre.Data.Models
{
    public class Play
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string Title { get; set; } = null!;

        [Required]
        public TimeSpan Duration { get; set; }

        [Required]
        public float Rating { get; set; }

        [Required]
        public Genre Genre { get; set; }

        [Required]
        [MaxLength(700)]
        public string Description { get; set; } = null!;

        [Required]
        [MaxLength(30)]
        public string Screenwriter { get; set; } = null!;

        public virtual ICollection<Cast> Casts { get; set; } = new HashSet<Cast>();
        public virtual ICollection<Ticket> Tickets { get; set; } = new HashSet<Ticket>();
    }
}
// Id – integer, Primary Key
// Title – text with length [4, 50] (required)
// Duration – TimeSpan in format {hours:minutes: seconds}, with a minimum length of 1 hour. (required)
// Rating – float in the range [0.00….10.00] (required)
// Genre – enumeration of type Genre, with possible values (Drama, Comedy, Romance, Musical) (required)
// Description – text with length up to 700 characters (required)
// Screenwriter – text with length [4, 30] (required)
// Casts – a collection of type Cast
// Tickets – a collection of type Ticket
