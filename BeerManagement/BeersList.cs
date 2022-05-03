using BeerManagement.Models;

namespace BeerManagement
{
    public class BeersList
    {
        public string BreweryName { get; set; }
        public List<BeerDistribution> Beers { get; set; }
    }

    public class BeerDistribution
    {
        public string BeerName { get; set; }
        public List<String> WholesalersName { get; set; }
    }

    public class BeerWholesaler
    {
        public string BeerId { get; set; }
        public string WholesalerId { get; set; }
    }

}
