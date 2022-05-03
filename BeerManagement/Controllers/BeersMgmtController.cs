using BeerManagement.Models;
using Microsoft.AspNetCore.Mvc;
using System.Data.Entity;
using System.Net;
using System.Text;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace BeerManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BeersMgmtController : ControllerBase
    {

        // GET: api/<ValuesController>
        [HttpGet]
        public List<BeersList> Get()
        {
            using (var db = new BeerManagementContext())
            {
                CheckValues(db);
                
                List<BeersList> result = new List<BeersList>(); 
                List<Brewery> breweries = db.Breweries.Distinct().ToList(); 
                foreach(var brewery in breweries)
                {
                    BeersList current = new BeersList();
                    current.BreweryName = brewery.Name;
                    current.Beers = new List<BeerDistribution>();
                    List<Beer> beers = db.Beers.Where(be => be.BreweryId == brewery.BreweryId).ToList();
                    foreach (var beer in beers)
                    {
                        BeerDistribution currentDist = new BeerDistribution();
                        currentDist.BeerName = beer.Name;
                        currentDist.WholesalersName = db.Stocks.Where(s => s.BeerId == beer.BeerId)
                            .Select(s => s.Wholesaler).Select(w => w.Name).ToList();
                        current.Beers.Add(currentDist);
                    }
                    result.Add(current);
                }
                return result;

            }
        }


        // Post api/<ValuesController>/5
        [HttpPost]
        public void Post([FromBody] Beer value)
        {
            using (var db = new BeerManagementContext())
            {
                Beer beer = new Beer()
                {
                    BeerId = Guid.NewGuid(),
                    Name = value.Name,
                    AlcoholDegree = value.AlcoholDegree,
                    Price = value.Price,
                    Brewery = db.Breweries.FirstOrDefault(br => br.BreweryId == value.BreweryId)

                };
                db.Beers.Add(beer);
                db.SaveChanges();
            }
        }


        // PUT api/<ValuesController>/5
        [HttpPut]
        public void Put(Guid Beer, [FromBody] string value)
        {
            Guid beerId = Beer;
            Guid wholeId = new Guid(value);
            using (var db = new BeerManagementContext())
            {
                Stock sto = new Stock()
                {
                    StockId = Guid.NewGuid(),
                    Beer = db.Beers.FirstOrDefault(s => s.BeerId == beerId),
                    Wholesaler = db.Wholesalers.FirstOrDefault(w => w.WholesalerId == wholeId),
                    Quantity = 0
                };
                db.Stocks.Add(sto);
                db.SaveChanges();
            }
        }

        // PUT api/<ValuesController>/5
        [HttpPatch]
        public void Patch(int quantity, [FromBody] BeerWholesaler value)
        {
            Guid beerId = new Guid(value.BeerId);
            Guid wholeId = new Guid(value.WholesalerId);
            using (var db = new BeerManagementContext())
            {
                db.Stocks
                    .Where(s => s.BeerId == beerId)
                    .Where(s => s.WholesalerId == wholeId)
                    .FirstOrDefault().Quantity = quantity;
                db.SaveChanges();
            }
        }

        // DELETE api/<ValuesController>/5
        [HttpDelete]
        public void Delete(Guid Beer)
        {
            Guid beerId = Beer;
            using (var db = new BeerManagementContext())
            {
                Beer beer = db.Beers.FirstOrDefault(b => b.BeerId == beerId);
                db.Beers.Remove(beer);
                db.SaveChanges();
            }
        }

        // GET: api/<ValuesController>
        [HttpPost]
        public IActionResult Post([FromBody]List<Commande> orders)
        {
            if (orders.Count() <= 0)
                return BadRequest(new CommandeException("La commande ne peut pas être vide"));
            for (int i = 0; i < orders.Count() - 1; i++)
            {
                for (int j = i; j < orders.Count(); j++)
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
                    for (int j = i; j < orders.Count(); j++)
                    {
                        if (orders[i].BeerId == orders[j].BeerId
                            && orders[i].WholeSalerId == orders[j].WholeSalerId)
                        {
                            askedQuantity += orders[j].Quantity;
                        }
                    }
                    if (db.Stocks.Any(s => s.WholesalerId.Equals(new Guid(orders[i].WholeSalerId))
                    && s.BeerId.Equals(new Guid(orders[i].BeerId)) && askedQuantity > s.Quantity))
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
                if(totalquantity > 20)
                    prix = prix - (prix/5);
                else if (totalquantity > 10)
                    prix = prix - (prix / 10);
                return Ok(prix);
            }
        }


        private void CheckValues(BeerManagementContext db)
        {
            if (db.Breweries.Count() == 0
                && db.Stocks.Count() == 0
                && db.Beers.Count() == 0
                && db.Wholesalers.Count() == 0)
            {
                #region Breweries
                var breweryLeffe = new Brewery()
                {
                    BreweryId = Guid.NewGuid(),
                    Name = "Abbaye de Leffe"
                };
                db.Breweries.Add(breweryLeffe);
                var breweryScourmont = new Brewery()
                {
                    BreweryId = Guid.NewGuid(),
                    Name = "Abbaye de Scourmont"
                };
                db.Breweries.Add(breweryScourmont);
                #endregion

                #region Beers
                var beerLeffeBlonde = new Beer()
                {
                    BeerId = Guid.NewGuid(),
                    Name = "Leffe Blonde",
                    AlcoholDegree = "6,6%",
                    Price = 2.2,
                    Brewery = breweryLeffe
                };
                db.Beers.Add(beerLeffeBlonde);
                var beerLeffeBrune = new Beer()
                {
                    BeerId = Guid.NewGuid(),
                    Name = "Leffe Brune",
                    AlcoholDegree = "6,5%",
                    Price = 2.2,
                    Brewery = breweryLeffe
                };
                db.Beers.Add(beerLeffeBrune);

                var beerChimayRed = new Beer()
                {
                    BeerId = Guid.NewGuid(),
                    Name = "Chimay Rouge",
                    AlcoholDegree = "6%",
                    Price = 2.3,
                    Brewery = breweryScourmont
                };
                db.Beers.Add(beerChimayRed);
                var beerChimayBlue = new Beer()
                {
                    BeerId = Guid.NewGuid(),
                    Name = "Chimay Bleue",
                    AlcoholDegree = "8%",
                    Price = 2.3,
                    Brewery = breweryScourmont
                };
                db.Beers.Add(beerChimayBlue);
                #endregion

                #region Wholesalers
                var wholesalerGene = new Wholesaler()
                {
                    WholesalerId = Guid.NewGuid(),
                    Name = "GeneDrinks"
                };
                db.Wholesalers.Add(wholesalerGene);
                var wholesalerDisco = new Wholesaler()
                {
                    WholesalerId = Guid.NewGuid(),
                    Name = "DiscoBeer"
                };
                db.Wholesalers.Add(wholesalerDisco);
                #endregion

                #region Stocks
                var stockGeneLefBl = new Stock()
                {
                    StockId = Guid.NewGuid(),
                    Wholesaler = wholesalerGene,
                    Beer = beerLeffeBlonde,
                    Quantity = 10
                };
                db.Stocks.Add(stockGeneLefBl);
                var stockGeneLefBr = new Stock()
                {
                    StockId = Guid.NewGuid(),
                    Wholesaler = wholesalerGene,
                    Beer = beerLeffeBrune,
                    Quantity = 0
                };
                db.Stocks.Add(stockGeneLefBr);
                var stockGeneChR = new Stock()
                {
                    StockId = Guid.NewGuid(),
                    Wholesaler = wholesalerGene,
                    Beer = beerChimayRed,
                    Quantity = 10
                };
                db.Stocks.Add(stockGeneChR);
                var stockDiscoChB = new Stock()
                {
                    StockId = Guid.NewGuid(),
                    Wholesaler = wholesalerDisco,
                    Beer = beerChimayBlue,
                    Quantity = 20
                };
                db.Stocks.Add(stockDiscoChB);
                var stockDiscoLefBl = new Stock()
                {
                    StockId = Guid.NewGuid(),
                    Wholesaler = wholesalerDisco,
                    Beer = beerLeffeBlonde,
                    Quantity = 0
                };
                db.Stocks.Add(stockDiscoLefBl);
                var stockDiscoLefBr = new Stock()
                {
                    StockId = Guid.NewGuid(),
                    Wholesaler = wholesalerDisco,
                    Beer = beerLeffeBrune,
                    Quantity = 5
                };
                #endregion

                db.SaveChanges();
            }
        }
    }
    public class BeerManagementContext : DbContext
    {
        public DbSet<Beer> Beers { get; set; }
        public DbSet<Brewery> Breweries { get; set; }
        public DbSet<Wholesaler> Wholesalers { get; set; }
        public DbSet<Stock> Stocks { get; set; }
    }
}
