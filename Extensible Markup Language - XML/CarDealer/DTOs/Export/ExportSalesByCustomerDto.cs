using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace CarDealer.DTOs.Export
{
    [XmlType("customer")]
    public class ExportSalesByCustomerDto
    {
        [XmlAttribute("full-name")]
        public string CustomerName { get; set; } = null!;

        [XmlAttribute("bought-cars")]
        public int CountOfCars { get; set; }

        [XmlAttribute("spent-money")]
        public decimal MoneySpent { get; set; }
    }
}
