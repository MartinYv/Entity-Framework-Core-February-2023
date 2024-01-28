using System.ComponentModel.DataAnnotations.Schema;

namespace Artillery.Data.Models
{
    public class CountryGun
    {
        [ForeignKey("Country")]
        public int CountryId { get; set; }
        public virtual Country Country { get; set; } = null!;



        [ForeignKey("Gun")]
        public int GunId { get; set; }

        public virtual Gun Gun { get; set; } = null!;

    }
}
//	CountryId – Primary Key integer, foreign key (required)
//	GunId – Primary Key integer, foreign key (required)
//