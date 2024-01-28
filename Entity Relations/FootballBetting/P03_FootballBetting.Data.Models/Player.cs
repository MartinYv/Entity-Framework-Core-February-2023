﻿using P03_FootballBetting.Data.Constants;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace P03_FootballBetting.Data.Models
{
   public class Player
    {
        public Player()
        {
            PlayerStatistics = new HashSet<PlayerStatistic>();
        }


        [Key]
        public int PlayerId { get; set; }

        [Required]
        [MaxLength(GlobalConstants.PersonNameMaxLength)]
        public string Name { get; set; }

        public byte SquadNumber { get; set; }


        [ForeignKey(nameof(Team))]
        public int TeamId { get; set; } 
        public virtual Team Team { get; set; }


        [ForeignKey(nameof(Position))]
        public int PositionId { get; set; }
        public virtual Position Position { get; set; }

        public bool IsInjured { get; set; }


        public virtual ICollection<PlayerStatistic> PlayerStatistics { get; set; }

    }
}
