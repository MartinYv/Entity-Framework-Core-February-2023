﻿using System.ComponentModel.DataAnnotations;

namespace Trucks.Data.Models
{
    public class Client
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(40)]
        public string Name { get; set; } = null!;

        [Required]
        [MaxLength(40)]
        public string Nationality { get; set; } = null!;


        [Required]
        public string Type { get; set; } = null!;
        public virtual ICollection<ClientTruck> ClientsTrucks { get; set; } = new HashSet<ClientTruck>();




    }
}
//	Id – integer, Primary Key
//	Name – text with length [3, 40] (required)
//	Nationality – text with length [2, 40] (required)
//	Type – text (required)
//	ClientsTrucks – collection of type ClientTruck
