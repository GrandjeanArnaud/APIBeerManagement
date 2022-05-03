using BeerManagement.Models;
using BeerManagement.Utils;
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
                ContextHelper.CheckValues(db);
                
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

    }

}
