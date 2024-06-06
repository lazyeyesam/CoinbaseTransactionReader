using System;
using System.Linq;
using CoinAPI.REST.V1;
using CoinbaseTransactionReader.Connection;
using CoinbaseTransactionReader.Infrastructure.Interfaces;
using CoinbaseTransactionReader.Model;
using Microsoft.Extensions.Logging;

namespace CoinbaseTransactionReader.Infrastructure
{
    public class CoinApiPricesConnection : IPricesConnection
    {
        private readonly CoinApiRestClient _api;
        private readonly TransactionReaderContext _context;
        private readonly ILogger<CoinApiPricesConnection> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="CoinApiPricesConnection"/> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="context">The context.</param>
        public CoinApiPricesConnection(ILogger<CoinApiPricesConnection> logger, TransactionReaderContext context)
        {
            _context = context;
            _logger = logger;
            _api = new CoinApiRestClient("xxx");
        }

        /// <summary>
        /// Gets the price.
        /// </summary>
        /// <param name="asset">The asset.</param>
        /// <param name="currency">The currency.</param>
        /// <returns></returns>
        public Price GetPrice(string asset, string currency)
        {
            // try get the price from the database and return if it's less than an hour old
            var price = _context.Prices.SingleOrDefault(px => asset == px.BaseCurrency && currency == px.TargetCurrency);
            if (price != null && (DateTime.Now - price.TimeStamp).TotalHours < 1)
                return price;

            // if it's totally new then create a new price
            price ??= new Price();

            try
            {
                // get the price from the API
                var coinApiPrice = _api.Exchange_rates_get_specific_rateAsync(asset, currency).GetAwaiter().GetResult();
                price.BaseCurrency = asset;
                price.TimeStamp = coinApiPrice.time;
                price.TargetCurrency = currency;
                price.Rate = coinApiPrice.rate;

                // save the new price or add the price to the database
                _context.Update(price);
                _context.SaveChanges();
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving price for: {asset}");
            }

            return price;
        }
    }
}
