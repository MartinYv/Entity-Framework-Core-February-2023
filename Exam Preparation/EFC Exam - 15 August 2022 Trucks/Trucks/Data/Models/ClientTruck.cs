using System.ComponentModel.DataAnnotations.Schema;

namespace Trucks.Data.Models
{
    public class ClientTruck
    {
        [ForeignKey("Client")]
        public int ClientId { get; set; }
        public virtual Client Client { get; set; } = null!;


        [ForeignKey("Truck")]
        public int TruckId { get; set; }
        public virtual Truck Truck { get; set; } = null!;


    }
}

//	ClientId – integer, Primary Key, foreign key (required)
//	Client – Client
//	TruckId – integer, Primary Key, foreign key (required)
//	Truck – Truck

