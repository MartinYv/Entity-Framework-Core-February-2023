using Artillery.Data.Models.Enums;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Artillery.Data.Models
{
    public class Gun
    {
        [Key]
        public int Id { get; set; }


        [ForeignKey("Manufacturer")]
        public int ManufacturerId { get; set; }
        public virtual Manufacturer Manufacturer { get; set; } = null!;


        [Required]
        public int GunWeight { get; set; }

        [Required]
        public double BarrelLength { get; set; }

        public int? NumberBuild { get; set; }

        [Required]
        public int Range { get; set; }

        [Required]
        public GunType GunType { get; set; }


        [ForeignKey("Shell")]
        public int ShellId { get; set; }
        public virtual Shell Shell { get; set; } = null!;

        public virtual ICollection<CountryGun> CountriesGuns { get; set; } = new HashSet<CountryGun>();

    }
}
// Id – integer, Primary Key
// ManufacturerId – integer, foreign key (required)
// GunWeight– integer in range [100…1_350_000] (required)
// BarrelLength – double in range [2.00….35.00] (required)
// NumberBuild – integer
// Range – integer in range [1….100_000] (required)
// GunType – enumeration of GunType, with possible values (Howitzer, Mortar, FieldGun, AntiAircraftGun, MountainGun, AntiTankGun) (required)
// ShellId – integer, foreign key (required)
// CountriesGuns – a collection of CountryGun
