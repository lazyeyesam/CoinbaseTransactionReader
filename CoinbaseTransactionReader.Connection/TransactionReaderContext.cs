using System;
using CoinbaseTransactionReader.Model;
using Microsoft.EntityFrameworkCore;

namespace CoinbaseTransactionReader.Connection
{
    public class TransactionReaderContext : DbContext
    { 
        public DbSet<Price> Prices { get; set; }

        public TransactionReaderContext(DbContextOptions<TransactionReaderContext> options)
        : base(options)
        {
            
        }
    }
}
