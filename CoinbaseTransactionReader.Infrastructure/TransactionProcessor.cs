using System;
using System.Collections.Generic;
using System.Linq;
using CoinbaseTransactionReader.Infrastructure.Interfaces;
using CoinbaseTransactionReader.Infrastructure.Parsers;
using CoinbaseTransactionReader.Model;

namespace CoinbaseTransactionReader.Infrastructure
{
    public class TransactionProcessor
    {
        private readonly ITransactionParser _buyParser;
        private readonly ITransactionParser _sellParser;
        private readonly ITransactionParser _convertToBuyParser;
        private readonly ITransactionParser _convertToSellParser;
        private readonly IPricesConnection _pricesConnection;


        public TransactionProcessor(IPricesConnection pricesConnection)
        {
            _buyParser = new BuyParser();
            _sellParser = new SellParser();
            _convertToBuyParser = new ConvertToBuyParser();
            _convertToSellParser = new ConvertToSellParser();
            _pricesConnection = pricesConnection;
        }

        public Portfolio Process(List<string[]> fileContents, DateTime? dateFrom = null, DateTime? dateTo = null)
        {
            dateFrom ??= DateTime.MinValue;
            dateTo ??= DateTime.MaxValue;

            // create and name the portfolio
            var portfolio = new Portfolio { Name = fileContents[5][1] };

            // get base currency
            var baseCurrency = fileContents[8][4].Split(' ')[0];
            portfolio.Currency = baseCurrency;

            // remove header information
            fileContents.RemoveRange(0, 8);

            // process the transactions
            fileContents.ForEach(entry =>
            {
                var asset = entry[2];
                var orderBook = GetOrderBook(asset, portfolio);

                // parse the transactions and add to the order book
                Order order;
                var type = entry[1];
                switch (type)
                {
                    case CoinbaseType.Buy:
                    case CoinbaseType.Earn:
                    case CoinbaseType.Receive:
                    case CoinbaseType.Reward:
                        order = _buyParser.Parse(entry);
                        orderBook.Orders.Add(order);
                        break;
                    case CoinbaseType.Sell:
                    case CoinbaseType.Send:
                        order = _sellParser.Parse(entry);
                        orderBook.Orders.Add(order);
                        break;
                    case CoinbaseType.Convert:
                        var buyOrder = _convertToBuyParser.Parse(entry);
                        var buyOrderBook = GetOrderBook(buyOrder.Asset, portfolio);
                        buyOrderBook.Orders.Add(buyOrder);

                        var sellOrder = _convertToSellParser.Parse(entry);
                        orderBook.Orders.Add(sellOrder);
                        break;
                    default:
                        throw new NotSupportedException(type);
                }
            });

            portfolio.Calculate(dateFrom, dateTo);
            portfolio.OrderBooks = portfolio.OrderBooks.OrderBy(ob => ob.Asset).ToList();

            return portfolio;
        }

        private OrderBook GetOrderBook(string asset, Portfolio portfolio)
        {
            var orderBook = portfolio.OrderBooks.SingleOrDefault(books => books.Asset == asset);
            if (orderBook != null) return orderBook;

            var price = _pricesConnection.GetPrice(asset, portfolio.Currency);

            orderBook = new OrderBook { Asset = asset, CurrentPrice = price};
            portfolio.OrderBooks.Add(orderBook);
            return orderBook;
        }
    }
}
