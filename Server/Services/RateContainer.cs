using Server.DataStructures;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Newtonsoft.Json;

using RestSharp;
using Server.Services;

namespace Server
{
    public class RateContainer: IExchangeRateService
    {
        private List<ExchangeRate> exchangeRates;
        private readonly string jsonFilename;

        public RateContainer(string filename)
        {
            jsonFilename = filename;
            exchangeRates = new List<ExchangeRate>();
            this.LoadFromJson();
        }


        public RateContainer(List<ExchangeRate> rates)
        {
            this.exchangeRates = new List<ExchangeRate>();
            foreach(var rate in rates)
            {
                this.exchangeRates.Add(rate);
            }
        }

        public void Clear()
        {
            exchangeRates.Clear();
        }

        internal object ToArray()
        {
            return exchangeRates.ToArray();
        }

        public List<ExchangeRate> GetRatesFromRange(DateTime startDate, DateTime endDate, string currency)
        {
            List<ExchangeRate> appropriateRates = new List<ExchangeRate>();
            TimeSpan oneDay = new TimeSpan(24, 0, 0);
            for(var currentDate = startDate; currentDate != endDate+oneDay; currentDate += oneDay)
            {
                var exchangeRate = exchangeRates.Find(rate => (rate.Currency == currency && rate.Date == currentDate));
                if (exchangeRate != null)
                {
                    appropriateRates.Add(exchangeRate);
                }
                else
                {
                    if(currency == "BTC")
                    {
                        var cryptoRate = loadCryptoCurrency(currentDate);
                        appropriateRates.Add(cryptoRate);
                        exchangeRates.Add(cryptoRate);

                    }
                    else
                    {
                        var basicRate =  loadBasicCurrency(currency, currentDate);
                        appropriateRates.Add(basicRate);
                        exchangeRates.Add(basicRate);
                    }
                }
            }
            this.SaveToJson();
            return appropriateRates;
        }
        public void SaveToJson()
        {
            StreamWriter writer = new StreamWriter(jsonFilename);
            writer.Write(JsonConvert.SerializeObject(exchangeRates.ToArray()));
            writer.Close();
        }

        public void LoadFromJson()
        {
            StreamReader reader = new StreamReader(jsonFilename);
            exchangeRates =  new List<ExchangeRate>(JsonConvert.DeserializeObject<ExchangeRate[]>(reader.ReadToEnd()));
            reader.Close();
        }

        private ExchangeRate loadBasicCurrency(string currencyCode, DateTime date)
        {
            var client = new RestClient("https://www.nbrb.by/api/exrates/rates/" +currencyCode);
            var request = new RestRequest().AddQueryParameter("parammode", "2").AddQueryParameter("ondate", date.ToString("yyyy-M-d"));
            var response = client.Execute(request);

            string responseAsString = response.Content;

            var rate = JsonConvert.DeserializeObject<Rate>(responseAsString);
            return new ExchangeRate(rate.Cur_Abbreviation,rate.Date,rate.Cur_OfficialRate,rate.Cur_Scale);

        }

        private ExchangeRate loadCryptoCurrency( DateTime date)
        {
            var client = new RestClient("https://rest.coinapi.io/v1/exchangerate/BTC/USD");
            var request = new RestRequest().AddQueryParameter("time",date.ToString("yyyy-MM-dd"));
            request.AddHeader("X-CoinAPI-Key", "E084154C-E132-4571-8493-B118285B5164");
            RestResponse response = client.Execute(request);

           var jsonRate =  response.Content;
           var rate = JsonConvert.DeserializeObject<Bitcoin>(jsonRate);


            client = new RestClient("https://www.nbrb.by/api/exrates/rates/USD");
            request = new RestRequest().AddQueryParameter("parammode", "2").AddQueryParameter("ondate", date.ToString("yyyy-M-d")); 
            response = client.Execute(request);
            jsonRate = response.Content;
            var usdRate = JsonConvert.DeserializeObject<Rate>(jsonRate); 
            return new ExchangeRate("BTC", rate.time, rate.rate*usdRate.Cur_OfficialRate, 1);
        }
    }
}
