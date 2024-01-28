using System.ComponentModel.DataAnnotations.Schema;

namespace Boardgames.Data.Models
{
    public class BoardgameSeller
    {
        [ForeignKey("Boardgame")]
        public int BoardgameId { get; set; }

        public virtual Boardgame Boardgame { get; set; } = null!;

        [ForeignKey("Seller")]
        public int SellerId { get; set; }
        public virtual Seller Seller { get; set; } = null!;
    }
}
// BoardgameId – integer, Primary Key, foreign key (required)
//
// Boardgame – Boardgame
//
// SellerId – integer, Primary Key, foreign key (required)
//
// Seller – Seller