using System.Collections.Generic;
using System.Linq;

namespace DiscountMonitor.AppConsole.Models
{
    public class Product
    {
        private const double DiscountThreshold = 0.05;

        public int Id { get; private set; }
        public string Name { get; private set; }
        public double Price { get; private set; }
        public string Description { get; private set; }
        public string Url { get; private set; }
        public ICollection<HistoryPrice> HistoryPrices { get; private set; } = new List<HistoryPrice>();

        public bool PriceWasChanged { get; private set; }

        private double PreviousPrice 
        { 
            get => this.HistoryPrices
                .Reverse()
                .Skip(1)
                .First()
                .Price;
        }

        public Product() { }

        public Product(string name, double price, string description, string url)
        {
            this.Name = name;
            this.Price = price;
            this.Description = description;
            this.Url = url;
        }

        public void VerifyAndTrackPrice(double price)
        {
            if (this.Price == price)
            {
                this.PriceWasChanged = false;

                return;
            }

            this.Price = price;
            this.PriceWasChanged = true;

            var priceHistory = new HistoryPrice(this, price);

            this.HistoryPrices.Add(priceHistory);
        }

        public bool DiscountWasReached()
        {
            if (!this.PriceWasChanged) 
            {
                return false;
            }

            var discount = this.ComputeDiscount(this.Price, this.PreviousPrice);
            var wasReached = discount >= DiscountThreshold;

            return wasReached;
        }

        private double ComputeDiscount(double newPrice, double previousPrice)
        {
            var variation = newPrice / previousPrice - 1;
            if (variation >= 0) 
            {
                return 0;
            }

            return -variation;
        }
    }
}
