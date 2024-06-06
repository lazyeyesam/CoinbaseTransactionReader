using System;

namespace CoinbaseTransactionReader.Model
{
    public class Order
    {
        public DateTime Timestamp { get; set; }
        public string Asset { get; set; }
        public string Notes { get; set; }
        public decimal Quantity { get; set; }
        public decimal Price { get; set; }
        public decimal SubTotal { get; set; }
        public decimal Fees { get; set; }
        public OrderType OrderType { get; set; }
    }
}
