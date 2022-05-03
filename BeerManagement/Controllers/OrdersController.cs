using BeerManagement.Utils;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace BeerManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        // GET: api/<ValuesController>
        [HttpPost]
        public IActionResult Post([FromBody] List<Commande> orders)
        {

            if (orders.Count() <= 0)
                return BadRequest(new CommandeException("La commande ne peut pas être vide"));
            for (int i = 0; i < orders.Count() - 1; i++)
            {
                for (int j = i+1; j < orders.Count(); j++)
                {
                    if (orders[i].Quantity == orders[j].Quantity
                        && orders[i].BeerId == orders[j].BeerId
                        && orders[i].WholeSalerId == orders[j].WholeSalerId)
                    {
                        return BadRequest(new CommandeException("Il ne peut pas y avoir de doublon dans la commande"));
                    }
                }
            }
            using (var db = new BeerManagementContext())
            {
                foreach (var order in orders)
                {
                    Guid wholeId = new Guid(order.WholeSalerId);
                    Guid beerId = new Guid(order.BeerId);
                    if (!db.Wholesalers.Any(w => w.WholesalerId.Equals(wholeId)))
                        return BadRequest(new CommandeException("La grossiste doit exister"));
                    if (!db.Stocks.Any(s => s.WholesalerId.Equals(wholeId) && s.BeerId.Equals(beerId)))
                        return BadRequest(new CommandeException("La bière doit être vendue par le grossiste"));
                    if (db.Stocks.Any(s => s.WholesalerId.Equals(wholeId) && s.BeerId.Equals(beerId) && order.Quantity > s.Quantity))
                        return BadRequest(new CommandeException("Le nombre de bières commandées ne doit pas être supérieur au stock du grossiste"));
                }

                for (int i = 0; i < orders.Count() - 1; i++)
                {
                    int askedQuantity = orders[i].Quantity;
                    for (int j = i+1; j < orders.Count(); j++)
                    {
                        if (orders[i].BeerId == orders[j].BeerId
                            && orders[i].WholeSalerId == orders[j].WholeSalerId)
                        {
                            askedQuantity += orders[j].Quantity;
                        }
                    }
                    Guid wholeId = new Guid(orders[i].WholeSalerId);
                    Guid beerId = new Guid(orders[i].BeerId);
                    if (db.Stocks.Any(s => s.WholesalerId.Equals(wholeId)
                    && s.BeerId.Equals(beerId) && askedQuantity > s.Quantity))
                        return BadRequest(new CommandeException("Le nombre de bières commandées ne doit pas être supérieur au stock du grossiste"));
                }

                double prix = 0;
                int totalquantity = 0;
                foreach (var order in orders)
                {
                    Guid beerId = new Guid(order.BeerId);
                    totalquantity += order.Quantity;
                    prix += db.Beers.FirstOrDefault(b => b.BeerId == beerId).Price * order.Quantity;
                }
                if (totalquantity > 20)
                    prix = prix - (prix / 5);
                else if (totalquantity > 10)
                    prix = prix - (prix / 10);
                return Ok(prix);
            }
        }

    }
}
