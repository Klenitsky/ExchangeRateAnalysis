﻿using Client.Models;
using LiveCharts;
using LiveCharts.Wpf;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Text;

namespace Client.ViewModels
{
    class ApplicationViewModel: INotifyPropertyChanged
    {
        private static string connectionString = "http://127.0.0.1:8888/ExchangeRates/";
        private List<string> currencyTypes = new List<string>{ "USD", "RUB", "EUR", "BTC" };
        private ExchangeRate maxRate;
        private ExchangeRate minRate;
        private int maxSeriesX;


        private string startDateString;
        private string endDateString;
        private int currency;
        public string StartDateString
        {
            get { return startDateString; }
            set
            {
                startDateString = value;
                OnPropertyChanged("StartDateString");
            }
        }

        public string EndDateString
        {
            get { return endDateString; }
            set
            {
                endDateString = value;
                OnPropertyChanged("EndDateString");
            }
        }

        public int Currency
        {
            get { return currency; }
            set
            {
                currency = value;
                OnPropertyChanged("Currency");
            }
        }

        public SeriesCollection SeriesCollection { get; set; }
        private List<string> labels;
        public List<string> Labels 
        {     
            get{ return labels; }
            set 
            {
                labels = value;
                OnPropertyChanged("Labels");
            }
        }
        public Func<double, string> YFormatter { get; set; }

        public int MaxSeriesX
        {
            get { return maxSeriesX; }
            set
            {
                maxSeriesX = value;
                OnPropertyChanged("MaxSeriesX");
            }
        }

        public ExchangeRate MaxRate
        {
            get { return maxRate; }
            set
            {
                maxRate = value;
                OnPropertyChanged("MaxRate");
            }
        }

        public ExchangeRate MinRate
        {
            get { return minRate; }
            set
            {
                minRate = value;
                OnPropertyChanged("MinRate");
            }
        }

        private RelayCommand printSeriesCommand;
        public RelayCommand PrintSeriesCommand
        {
            get
            {
                return printSeriesCommand ??
                  (printSeriesCommand = new RelayCommand(obj =>
                  {
                      var client = new RestClient(connectionString + currencyTypes[currency]);
                      var request = new RestRequest()
                          .AddQueryParameter("startDate", DateTime.ParseExact(startDateString, "dd-MM-yyyy", CultureInfo.InvariantCulture).ToString("dd-MM-yyyy"))
                          .AddQueryParameter("endDate", DateTime.ParseExact(endDateString, "dd-MM-yyyy", CultureInfo.InvariantCulture).ToString("dd-MM-yyyy"));
                      RestResponse response = client.Execute(request);

                      var jsonRate = response.Content;
                      var rates = JsonConvert.DeserializeObject<ExchangeRate[]>(jsonRate);

                      List<double> ExchangeRatesValues = new List<double>();
                      List<string> dates = new List<string>();

                      foreach (var rate in rates)
                      {
                          ExchangeRatesValues.Add(rate.Value);
                          dates.Add(rate.Date.ToString("dd-MM"));
                      }
                      MaxSeriesX = ExchangeRatesValues.Count;
                      SeriesCollection[0] = new LineSeries
                      {
                          Title = currencyTypes[Currency],
                          Values = new ChartValues<double>(ExchangeRatesValues)
                      };
                      Labels = dates;
                  }));
            }
        }
        public ApplicationViewModel()
        {
            SeriesCollection = new SeriesCollection
            {
                new LineSeries
                {
                    Title = "Exchange Rate",
                    Values = new ChartValues<double> {1,2,3 }
                }
               
            };
            MaxSeriesX = 3;
            Labels = new List<string> {"01","02","03"};
            YFormatter = value => Math.Round(value,4).ToString("");
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }
    }
}
