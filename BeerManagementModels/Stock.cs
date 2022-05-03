using BeerManagement.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeerManagement.Models
{
    [Table("Stocks")]
    public class Stock
    {
        [Key]
        public Guid StockId { get; set; }

        public Guid WholesalerId { get; set; }
        [ForeignKey("WholesalerId")]
        public virtual Wholesaler Wholesaler { get; set; }
        public Guid BeerId { get; set; }
        [ForeignKey("BeerId")]
        public virtual Beer Beer { get; set; }

        public int Quantity { get; set; }

        public Stock()
        {
        }
    }
}
