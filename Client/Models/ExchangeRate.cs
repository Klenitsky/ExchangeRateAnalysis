using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json.Serialization;

namespace Client.Models
{
    public class ExchangeRate : INotifyPropertyChanged
    {
        public int Id { get; set; }

        private string currency;
        private DateTime date;
        private double value;
        private int amount;
        public string Currency 
        {
            get { return currency; }
            set 
            {
                currency = value;
                OnPropertyChanged("Currency");
            } 
        }
        public DateTime Date
        {
            get { return date; }
            set
            {
                date = value;
                OnPropertyChanged("Date");
            }
        }
        public double Value
        {
            get { return value; }
            set
            {
                this.value = value;
                OnPropertyChanged("Value");
            }
        }
        public int Amount
        {
            get { return amount; }
            set
            {
                amount = value;
                OnPropertyChanged("Amount");
            }
        }

        [JsonConstructor]
        public ExchangeRate(string currency, DateTime date, double value, int amount)
        {
            this.Currency = currency;
            this.Date = date;
            this.Value = value;
            this.Amount = amount;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }
    }
}
