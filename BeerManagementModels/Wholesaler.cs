using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BeerManagement.Models
{
    [Table("Wholesalers")]
    public class Wholesaler
    {
        [Key]
        public Guid WholesalerId { get; set; }
        public string Name { get; set; }

        public Wholesaler(string name)
        {
            Name = name;
        }

        public Wholesaler()
        {
        }
    }
}
