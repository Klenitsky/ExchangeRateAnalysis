using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace Client.Models
{
    public class ExchangeRate
    {
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
