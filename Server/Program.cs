using System;
using System.Collections.Generic;



namespace Server
{
    static class Program
    {
        private static RateConatiner exchangeRates = new RateConatiner();
        private static string dataFile = "rates.json";
        static void Main(string[] args)
        {

            List<DataStructures.ExchangeRate> testRates = new List<DataStructures.ExchangeRate>();
            testRates.Add(new DataStructures.ExchangeRate("USD", new DateTime(2022, 1, 1), 4.33, 1));
            testRates.Add(new DataStructures.ExchangeRate("EUR", new DateTime(2021, 1, 1), 3.33, 1));
            testRates.Add(new DataStructures.ExchangeRate("EUR", new DateTime(2020, 1, 1), 2.33, 1));
            testRates.Add(new DataStructures.ExchangeRate("USD", new DateTime(2021, 1, 1), 1.33, 1));

            exchangeRates = new RateConatiner(testRates);
            exchangeRates.SaveToJson(dataFile);
            exchangeRates.LoadFromJson(dataFile);
            exchangeRates.GetRatesFromRange(new DateTime(2022, 1, 2), DateTime.Today, "USD");
            Console.WriteLine("Dat");
        }
    }
}
