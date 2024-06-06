using CoinbaseTransactionReader.Model;

namespace CoinbaseTransactionReader.Infrastructure.Interfaces
{
    public interface ITransactionParser
    {
        Order Parse(string[] entry);
    }
}