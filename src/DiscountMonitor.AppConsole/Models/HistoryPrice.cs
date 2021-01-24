using System;

namespace DiscountMonitor.AppConsole.Models
{
    public class HistoryPrice
    {
        public int Id { get; private set; }
        public double Price { get; private set; }
        public DateTime Date { get; private set; } = DateTime.Now;
        public Product Product { get; private set; }

        public HistoryPrice() { }

        public HistoryPrice(Product product, double price)
        {
            Product = product;
            Price = price;
        }
    }
}
