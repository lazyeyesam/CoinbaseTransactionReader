using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoinbaseTransactionReader.Model
{
    public class Portfolio
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Portfolio"/> class.
        /// </summary>
        public Portfolio()
        {
            OrderBooks = new List<OrderBook>();
        }

        /// <summary>
        /// Calculates PnL values for the Portfolio.
        /// </summary>
        /// <param name="dateFrom">The date from.</param>
        /// <param name="dateTo">The date to.</param>
        public void Calculate(DateTime? dateFrom = null, DateTime? dateTo = null)
        {
            dateFrom ??= DateTime.MinValue;
            dateTo ??= DateTime.MaxValue;

            Parallel.ForEach(OrderBooks, orderBook => orderBook.Calculate(dateFrom, dateTo));

            var unrealisedPnL = OrderBooks.Sum(orderBook => orderBook.UnrealisedPnL);
            var realisedPnL = OrderBooks.Sum(orderBook => orderBook.RealisedPnL);
            var totalFees = OrderBooks.Sum(orderBook => orderBook.TotalFees);
            var marketValue = OrderBooks.Sum(orderBook => orderBook.MarketValue);
            var taxablePnL = OrderBooks.Sum(orderBook => orderBook.TaxablePnL);
            var unrealisedGrowth = unrealisedPnL / (marketValue - unrealisedPnL);

            MarketValue = marketValue;
            UnrealisedPnL = unrealisedPnL;
            RealisedPnL = realisedPnL;
            TotalFees = totalFees;
            TaxablePnL = taxablePnL;
            UnrealisedGrowth = unrealisedGrowth;
            DateFrom = dateFrom.Value;
            DateTo = dateTo.Value;
        }

        public string Name { get; set; }
        public List<OrderBook> OrderBooks { get; set; }
        public decimal RealisedPnL { get; set; }
        public decimal TaxablePnL { get; set; }
        public decimal TotalFees { get; set; }
        public decimal UnrealisedPnL { get; set; }
        public decimal MarketValue { get; set; }
        public decimal UnrealisedGrowth { get; set; }
        public string Currency { get; set; }
        public DateTime DateFrom { get; set; }
        public DateTime DateTo { get; set; }
    }
}
