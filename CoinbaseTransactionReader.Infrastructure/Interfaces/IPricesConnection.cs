using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CoinbaseTransactionReader.Model;

namespace CoinbaseTransactionReader.Infrastructure.Interfaces
{
    public interface IPricesConnection
    {
        Price GetPrice(string asset, string currency);
    }
}
