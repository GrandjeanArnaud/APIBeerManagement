using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BeerManagement.Models
{
    [Table("Breweries")]
    public class Brewery
    {
        [Key]
        public Guid BreweryId { get; set; }
        public string Name { get; set; }

        public Brewery(string name)
        {
            Name = name;
        }

        public Brewery()
        {
        }
    }
}
