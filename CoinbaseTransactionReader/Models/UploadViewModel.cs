using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNetCore.Http;

namespace CoinbaseTransactionReader.Models
{
    public class UploadViewModel
    {
        public UploadTransactionRange TransactionRange { get; set; }
        public DateTime DateFrom { get; set; }
        public DateTime DateTo { get; set; }
        public IFormFile File { get; set; }
    }

    public enum UploadTransactionRange
    {
        [Description("All time")]
        AllTime = 0,
        [Description("Year to date")]
        YearToDate,
        [Description("Last 30 days")]
        Thirtydays,
        [Description("Custom")]
        Custom
    }
}
