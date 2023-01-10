using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Server.DataStructures
{
    public class ExchangeRate
    {
        public int Id { get; set; }
        public string Currency { get; set; }
        public DateTime Date { get; set; }
        public double Value { get; set; }
        public int Amount { get; set; }

        [JsonConstructor]
        public ExchangeRate(string currency, DateTime date, double value, int amount)
        {
            this.Currency = currency;
            this.Date = date;
            this.Value = value;
            this.Amount = amount;
        }
    }
}
