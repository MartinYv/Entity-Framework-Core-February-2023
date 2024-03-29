﻿using System.ComponentModel.DataAnnotations;

namespace Theatre.Data.Models
{
    public class Theatre
    {

        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(30)] 
        public string Name { get; set; } = null!;
       
        [Required]
        public sbyte NumberOfHalls { get; set; }

        [Required]
        [MaxLength(30)]
        public string Director { get; set; } = null!;

        public virtual ICollection<Ticket> Tickets { get; set; } = new HashSet<Ticket>();

    }
}
//	Id – integer, Primary Key
//	Name – text with length [4, 30] (required)
//	NumberOfHalls – sbyte between [1…10] (required)
//	Director – text with length [4, 30] (required)
//	Tickets – a collection of type Ticket
