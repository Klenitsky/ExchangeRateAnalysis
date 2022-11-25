using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Client.Models;
using System.Linq;

using Newtonsoft.Json;
using RestSharp;

namespace Client
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private static string connectionString = "http://127.0.0.1:8888/ExchangeRates/";
        private List<string> dates = new List<string>();
        public MainWindow()
        {
            InitializeComponent();
        }

        public string[] Labels { get; set; }
        public Func<double, string> YFormatter { get; set; }

        private void sumbitRequest_Click(object sender, RoutedEventArgs e)
        {
            var client = new RestClient(connectionString+ СurrencyType.Text);
            var request = new RestRequest()
                .AddQueryParameter("startDate", DateTime.ParseExact(startDateBox.Text, "dd-MM-yyyy", CultureInfo.InvariantCulture).ToString("dd-MM-yyyy"))
                .AddQueryParameter("endDate", DateTime.ParseExact(endDateBox.Text, "dd-MM-yyyy", CultureInfo.InvariantCulture).ToString("dd-MM-yyyy"));
            RestResponse response = client.Execute(request);

            var jsonRate = response.Content;
            var rates = JsonConvert.DeserializeObject<ExchangeRate[]>(jsonRate);

            List<double> ExchangeRatesValues = new List<double>();
            List<double> RatesTimes = new List<double>();

            foreach (var rate in rates)
            {
                ExchangeRatesValues.Add(rate.Value);
                RatesTimes.Add(rate.Date.ToOADate());
            }
           ExchangeRatePlot.Plot.Clear();
           ExchangeRatePlot.Plot.AddScatterLines(RatesTimes.ToArray(), ExchangeRatesValues.ToArray());
            ExchangeRatePlot.Plot.XAxis.DateTimeFormat(true);
            ExchangeRatePlot.Refresh();
        }
    }
    
}
