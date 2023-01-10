using Newtonsoft.Json;
using RestSharp;
using Server.Contexts;
using Server.DataStructures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Server.Services
{
    internal class DataBaseRateService : IExchangeRateService
    {
        public void Clear()
        {
            using(ExchangeRateDatabaseContext db  = new ExchangeRateDatabaseContext())
            {
                for(int i=0; i < db.ExchangeRates.Count(); i++)
                {
                    ExchangeRate? rate = db.ExchangeRates.FirstOrDefault();
                    if(rate != null)
                    {
                        db.ExchangeRates.Remove(rate);
                        db.SaveChanges();
                    }
                }
            }
        }

        public List<ExchangeRate> GetRatesFromRange(DateTime startDate, DateTime endDate, string currency)
        {
            List<ExchangeRate> appropriateRates = new List<ExchangeRate>();
            TimeSpan oneDay = new TimeSpan(24, 0, 0);

            using (ExchangeRateDatabaseContext db = new ExchangeRateDatabaseContext()) {

                for (var currentDate = startDate; currentDate != endDate + oneDay; currentDate += oneDay)
                {
                    var exchangeRate = db.ExchangeRates.Where(rate => (rate.Currency == currency && rate.Date == currentDate));
                    if (exchangeRate != null && exchangeRate.Count()>0)
                    {
                        foreach (var rate in exchangeRate)
                        {
                            appropriateRates.Add(rate);
                        }
                    }
                    else
                    {
                        if (currency == "BTC")
                        {
                            var cryptoRate = loadCryptoCurrency(currentDate);
                            appropriateRates.Add(cryptoRate);
                            db.ExchangeRates.Add(cryptoRate);
                        }
                        else
                        {
                            var basicRate = loadBasicCurrency(currency, currentDate);
                            appropriateRates.Add(basicRate);
                            db.ExchangeRates.Add(basicRate);
                        }
                    }
                }
                db.SaveChanges();
            }
            return appropriateRates;
        }


        //Not needed
        public void LoadFromJson()
        {
        }

        //Not needed
        public void SaveToJson()
        {
        }

        private ExchangeRate loadBasicCurrency(string currencyCode, DateTime date)
        {
            var client = new RestClient("https://www.nbrb.by/api/exrates/rates/" + currencyCode);
            var request = new RestRequest().AddQueryParameter("parammode", "2").AddQueryParameter("ondate", date.ToString("yyyy-M-d"));
            var response = client.Execute(request);
            if (!response.IsSuccessful)
            {
                throw new ArgumentException("API doesn't response");
            }

            string responseAsString = response.Content;

            var rate = JsonConvert.DeserializeObject<Rate>(responseAsString);
            return new ExchangeRate(rate.Cur_Abbreviation, rate.Date, rate.Cur_OfficialRate, rate.Cur_Scale);

        }

        private ExchangeRate loadCryptoCurrency(DateTime date)
        {
            var client = new RestClient("https://rest.coinapi.io/v1/exchangerate/BTC/USD");
            var request = new RestRequest().AddQueryParameter("time", date.ToString("yyyy-MM-dd"));
            request.AddHeader("X-CoinAPI-Key", "E084154C-E132-4571-8493-B118285B5164");
            RestResponse response = client.Execute(request);

            if (!response.IsSuccessful)
            {
                throw new ArgumentException("API doesn't response");
            }
            var jsonRate = response.Content;
            var btcRate = JsonConvert.DeserializeObject<Bitcoin>(jsonRate);

            return new ExchangeRate("BTC", btcRate.time, btcRate.rate, 1);
        }
    }
}
