using Client.Models;
using LiveCharts;
using LiveCharts.Defaults;
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
using System.Windows;

namespace Client.ViewModels
{
    class ApplicationViewModel: INotifyPropertyChanged
    {
        private static string connectionString = "http://127.0.0.1:8888/ExchangeRates/";
        private List<string> currencyTypes = new List<string>{ "USD", "RUB", "EUR", "BTC" };
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

        private RelayCommand printSeriesCommand;
        public RelayCommand PrintSeriesCommand
        {
            get
            {
                return printSeriesCommand ??
                  (printSeriesCommand = new RelayCommand(obj =>
                  {
                      var client = new RestClient(connectionString + currencyTypes[currency]);
                      DateTime startDate;
                      try
                      {
                          startDate = DateTime.Parse(startDateString);
                      }
                      catch (FormatException)
                      {
                          MessageBox.Show("Invalid startDate");
                          return;
                      }

                      DateTime endDate;
                      try
                      {
                          endDate = DateTime.Parse(endDateString);
                      }
                      catch (FormatException)
                      {
                          MessageBox.Show("Invalid endDate");
                          return;
                      }

                      TimeSpan fiveYears = new TimeSpan(5 * 24 * 365, 0, 0);

                      if(startDate> DateTime.Now || endDate > DateTime.Now)
                      {
                          MessageBox.Show("Date is in future");
                          return;
                      }

                      if (startDate < (DateTime.Now-fiveYears) || endDate < (DateTime.Now-fiveYears))
                      {
                          MessageBox.Show("Too early date");
                          return;
                      }

                      if (startDate > endDate)
                      {
                          MessageBox.Show("Invalid interval");
                          return;
                      }

                      var request = new RestRequest()
                          .AddQueryParameter("startDate", startDate.ToString("dd-MM-yyyy"))
                          .AddQueryParameter("endDate", endDate.ToString("dd-MM-yyyy"));
                      RestResponse response = client.Execute(request);

                      if (!response.IsSuccessful)
                      {
                          MessageBox.Show("Error on server side!");
                          return;
                      }

                      var jsonRate = response.Content;
                      ExchangeRate[] rates;
                      try
                      {
                          rates = JsonConvert.DeserializeObject<ExchangeRate[]>(jsonRate);
                      }
                      catch(JsonReaderException)
                      {
                          MessageBox.Show(jsonRate);
                          return;
                      }
                      List<double> ExchangeRatesValues = new List<double>();
                      List<string> dates = new List<string>();
                      double minRate=double.MaxValue;
                      int indexMinRate=-1;

                      double maxRate=0;
                      int indexMaxRate=-1;

                      for(int i=0;i<rates.Length;i++)
                      {
                          ExchangeRatesValues.Add(rates[i].Value);
                          dates.Add(rates[i].Date.ToString("dd-MM"));
                          if(rates[i].Value> maxRate)
                          {
                              maxRate = rates[i].Value;
                              indexMaxRate = i;
                          }

                          if (rates[i].Value < minRate)
                          {
                              minRate = rates[i].Value;
                              indexMinRate = i;
                          }
                      }
                      MaxSeriesX = ExchangeRatesValues.Count+1;
                      SeriesCollection[0] = new LineSeries
                      {
                          Title = currencyTypes[Currency],
                          Values = new ChartValues<double>(ExchangeRatesValues)
                      };
                      SeriesCollection[1] = new ScatterSeries
                      {
                          Title = "min",
                          Values = new ChartValues<ObservablePoint> { new ObservablePoint(indexMinRate, minRate) },
                          PointGeometry = DefaultGeometries.Square
                      };

                      SeriesCollection[2] = new ScatterSeries
                      {
                          Title = "max",
                          Values = new ChartValues<ObservablePoint> { new ObservablePoint(indexMaxRate, maxRate) },
                          PointGeometry = DefaultGeometries.Square
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
                    Values = new ChartValues<double> { },
                },

                new ScatterSeries
                {
                    Title = "min",
                    Values = new ChartValues<ObservablePoint> { new ObservablePoint(0, 0) },
                    PointGeometry = DefaultGeometries.Square
                },

                 new ScatterSeries
                {
                    Title = "max",
                    Values = new ChartValues<ObservablePoint> { new ObservablePoint(0, 0) },
                    PointGeometry = DefaultGeometries.Square
                }

            };
            MaxSeriesX = 3;
            Labels = new List<string> {};
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
