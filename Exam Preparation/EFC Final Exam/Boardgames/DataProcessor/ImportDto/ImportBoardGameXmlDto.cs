using Boardgames.Data.Models.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace Boardgames.DataProcessor.ImportDto
{
    [XmlType("Boardgame")]
    public class ImportBoardGameXmlDto
    {
        [XmlElement("Name")]
        [Required]
        [MinLength(10)]
        [MaxLength(20)]
        public string Name { get; set; } = null!;


        [XmlElement("Rating")]
        [Required]
        [Range(1, 10.00)]
        public double Rating { get; set; }

        [XmlElement("YearPublished")]
        [Required]
        [Range(2018, 2023)]
        public int YearPublished { get; set; }

        [XmlElement("CategoryType")]
        [Required]
        public string CategoryType { get; set; } = null!;

        [XmlElement("Mechanics")]
        [Required]
        public string Mechanics { get; set; } = null!;
    }
}

// Id – integer, Primary Key
// 
//  Name – text with length [10…20] (required)
// 
//  Rating – double in range [1…10.00] (required)
// 
//  YearPublished – integer in range [2018…2023] (required)
// 
//  CategoryType – enumeration of type CategoryType, with possible values (Abstract, Children, Family, Party, Strategy) (required)
// 
//  Mechanics – text (string, not an array) (required)
// 
//  CreatorId – integer, foreign key (required)
// 
//  Creator – Creator
// 
//  BoardgamesSellers – collection of type BoardgameSeller




// Name > 4 Gods </ Name >
// 
// Rating > 7.28 </ Rating >
// 
// YearPublished > 2017 </ YearPublished >
// 
// CategoryType > 4 </ CategoryType >
// 
// Mechanics >

