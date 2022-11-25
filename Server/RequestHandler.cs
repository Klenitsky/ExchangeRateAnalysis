using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace Server
{
    public static class RequestHandler
    {

        public static string GetRequestString(string requestParameters, RateContainer container)
        {
            char[] separators = { '?', '&' };
            var requestParametersArray = requestParameters.Split(separators);
            List<string> parametersList = new List<string>(requestParametersArray);
            string currency = parametersList[0];
            string startDateAsString = parametersList.Find(property => property.ToUpper().Contains("STARTDATE"));
            string endDateAsString = parametersList.Find(property => property.ToUpper().Contains("ENDDATE"));
            startDateAsString = startDateAsString.ToUpper().Replace("STARTDATE=", "");
            endDateAsString = endDateAsString.ToUpper().Replace("ENDDATE=", "");
            DateTime startDate = DateTime.ParseExact(startDateAsString, "dd-MM-yyyy", CultureInfo.InvariantCulture);
            DateTime endDate = DateTime.ParseExact(endDateAsString, "dd-MM-yyyy", CultureInfo.InvariantCulture);

            var appropriateExchangeRates = container.GetRatesFromRange(startDate, endDate, currency.ToUpper());
            return JsonConvert.SerializeObject(appropriateExchangeRates.ToArray());
        }
    }
}
