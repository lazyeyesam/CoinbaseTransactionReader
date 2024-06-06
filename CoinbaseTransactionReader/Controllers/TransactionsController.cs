using System;
using System.Data.SqlTypes;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Threading;
using Microsoft.AspNetCore.Mvc;
using CoinbaseTransactionReader.Infrastructure;
using CoinbaseTransactionReader.Models;
using Microsoft.Extensions.Logging;

namespace CoinbaseTransactionReader.Controllers
{
    public class TransactionsController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly TransactionProcessor _processor;

        /// <summary>
        /// Initializes a new instance of the <see cref="TransactionsController"/> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="processor">The processor.</param>
        public TransactionsController(ILogger<HomeController> logger, TransactionProcessor processor)
        {
            _logger = logger;
            _processor = processor;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Index(UploadViewModel model)
        {
            try
            {
                // get date ranges
                DateTime dateFrom, dateTo;
                switch (model.TransactionRange)
                {
                    case UploadTransactionRange.YearToDate:
                        dateFrom = new DateTime(DateTime.Today.Year, 1, 1);
                        dateTo = DateTime.MaxValue;
                        break;
                    case UploadTransactionRange.Thirtydays:
                        dateFrom = DateTime.Today.AddDays(-30);
                        dateTo = DateTime.MaxValue;
                        break;
                    case UploadTransactionRange.Custom:
                        dateFrom = model.DateFrom;
                        dateTo = model.DateTo;
                        break;
                    case UploadTransactionRange.AllTime:
                        dateFrom = DateTime.MinValue;
                        dateTo = DateTime.MaxValue;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(model.TransactionRange.ToString());
                }

                // check the file is a csv
                var extension = model.File.FileName.Split('.').LastOrDefault();
                if (extension != "csv") return RedirectToAction("Error");

                // process the file
                var file = model.File.ReadAsList();
                var portfolio = _processor.Process(file, dateFrom, dateTo);

                // set the correct currency symbols based on currency used in transaction file
                var culture = CultureInfo.GetCultures(CultureTypes.SpecificCultures)
                    .FirstOrDefault(x => new RegionInfo(x.LCID).ISOCurrencySymbol == portfolio.Currency);

                if (culture != null)
                    Thread.CurrentThread.CurrentCulture = culture;

                return View(portfolio);
            }
            catch (Exception e)
            {
                _logger.Log(LogLevel.Error, e, $"Could not service file {model?.File?.FileName}");
                return RedirectToAction("Error");
            }

        }

        [HttpGet]
        public IActionResult Index()
        {
            return RedirectToAction("Index", "Home");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View("Error");
        }
    }
}
