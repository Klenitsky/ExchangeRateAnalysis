using Server.DataStructures;
using System;
using System.Collections.Generic;
using System.Text;

namespace Server.Services
{
    public interface IExchangeRateService
    {
        public void Clear();
        public List<ExchangeRate> GetRatesFromRange(DateTime startDate, DateTime endDate, string currency);

        public void SaveToJson();
        public void LoadFromJson();

    }
}
