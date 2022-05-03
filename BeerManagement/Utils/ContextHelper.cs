using BeerManagement.Models;
using System.Data.Entity;

namespace BeerManagement.Utils
{
    public static class ContextHelper
    {
        public static void CheckValues(BeerManagementContext db)
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

