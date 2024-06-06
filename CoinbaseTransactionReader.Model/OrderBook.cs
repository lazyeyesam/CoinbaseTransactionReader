using System;
using System.Collections.Generic;
using System.Linq;

namespace CoinbaseTransactionReader.Model
{
    public class OrderBook
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OrderBook"/> class.
        /// </summary>
        public OrderBook()
        {
            Orders = new List<Order>();
        }

        /// <summary>
        /// Calculates the PnL values for the OrderBook.
        /// </summary>
        /// <param name="dateFrom">The date from.</param>
        /// <param name="dateTo">The date to.</param>
        public void Calculate(DateTime? dateFrom = null, DateTime? dateTo = null)
        {
            decimal runningBuyCost = 0;
            decimal runningBuyQty = 0;
            decimal totalBuyQty = 0;
            decimal totalSellQty = 0;
            decimal realisedPnL = 0;
            decimal totalFees = 0;
            decimal taxablePnL = 0;

            dateFrom ??= DateTime.MinValue;
            dateTo ??= DateTime.MaxValue;

            Orders.OrderBy(order => order.Timestamp).ToList().ForEach(order =>
            {
                totalFees += order.Fees;
                if (order.OrderType == OrderType.Buy)
                {
                    totalBuyQty += order.Quantity;

                    runningBuyQty += order.Quantity;
                    var tradeCost = order.Price * order.Quantity;
                    runningBuyCost += tradeCost;
                }
                else
                {
                    totalSellQty += order.Quantity;

                    var averagePrice = runningBuyQty == 0 ? 0 : runningBuyCost / runningBuyQty;
                    var pnl = (order.Price - averagePrice) * order.Quantity;
                    realisedPnL += pnl;

                    if (order.Timestamp.Date >= dateFrom.Value.Date && order.Timestamp.Date <= dateTo.Value.Date)
                        taxablePnL += (pnl - order.Fees);

                    runningBuyQty -= order.Quantity;
                    runningBuyCost = averagePrice * runningBuyQty;
                }
            });

            var position = totalBuyQty - totalSellQty;
            var averageBuyPrice = runningBuyQty > 0 ? runningBuyCost / runningBuyQty : 0;

            // assign calculated values
            TotalBought = totalBuyQty;
            TotalSold = totalSellQty;
            RealisedPnL = realisedPnL;
            TaxablePnL = taxablePnL;
            Position = position;
            TotalFees = totalFees;
            AveragePrice = averageBuyPrice;
            MarketValue = CurrentPrice.Rate * position;

            if (CurrentPrice?.Rate > 0 && position > 0)
            {
                UnrealisedPnL = (CurrentPrice.Rate - averageBuyPrice) * position;
                UnrealisedGrowth = UnrealisedPnL / (averageBuyPrice * position);
            }

            Orders = Orders.OrderByDescending(order => order.Timestamp).ToList();
        }

        public string Asset { get; set; }
        public Price CurrentPrice { get; set; }
        public List<Order> Orders { get; set; }
        public decimal RealisedPnL { get; set; }
        public decimal TaxablePnL { get; set; }
        public decimal Position { get; set; }
        public decimal AveragePrice { get; set; }
        public decimal TotalBought { get; set; }
        public decimal TotalSold { get; set; }
        public decimal TotalFees { get; set; }
        public decimal UnrealisedPnL { get; set; }
        public decimal UnrealisedGrowth { get; set; }
        public decimal MarketValue { get; set; }
    }
}
