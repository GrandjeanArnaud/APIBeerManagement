using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace BeerManagement.Models
{
    [Table("Beers")]
    public class Beer
    {
        [Key]
        public Guid BeerId { get; set; }
        public string Name { get; set; }
        public string AlcoholDegree { get; set; }
        public double Price { get; set; }

        public Guid BreweryId { get; set; }
        [ForeignKey("BreweryId")]
        public virtual Brewery Brewery { get; set; }

        public Beer()
        {
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
