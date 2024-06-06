using System;

namespace CoinbaseTransactionReader.Model
{
    public class CoinbaseTransaction
    {
        public DateTime Timestamp { get; set; }
        public string TransactionType { get; set; }
        public string Asset { get; set; }
        public decimal Quantity { get; set; }
        public decimal SpotPrice { get; set; }
        public decimal? SubTotal { get; set; }
        public decimal? Total { get; set; }
        public decimal? Fees { get; set; }
        public string Notes { get; set; }
    }
}