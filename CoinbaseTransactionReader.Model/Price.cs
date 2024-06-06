using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace CoinbaseTransactionReader.Model
{
    public class Price
    {
        public int Id { get; set; }
        public decimal Rate { get; set; }
        public DateTime TimeStamp { get; set; }
        public string BaseCurrency { get; set; }
        public string TargetCurrency { get; set; }
    }
}