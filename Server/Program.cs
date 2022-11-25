using Newtonsoft.Json;
using Server.Services;
using Server.Services.Decorators;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    static class Program
    {
        private static IExchangeRateService exchangeRates;
        private static string dataFile = "rates.json";
        private static string logFile = "serviceLog.log";
        private static string connectionString = "http://127.0.0.1:8888/ExchangeRates/";
        private static HttpListener server = new HttpListener();


        static async Task Main(string[] args)
        {
            exchangeRates = new RateContainerLogger(new RateContainer(), logFile);
            exchangeRates.LoadFromJson(dataFile);

            server.Prefixes.Add(connectionString);
            server.Start();
            Console.WriteLine(DateTime.Now + " Server started.");
            while (true)
            {
                var context = await server.GetContextAsync();
                var response = context.Response;
                var requestString = context.Request.Url.ToString();
                requestString = requestString.Remove(0,connectionString.Length);
                if (requestString.ToUpper() == "STOPSERVER")
                {
                    break;
                }

                string responseText = RequestHandler.GetRequestString(requestString, exchangeRates);
                byte[] buffer = Encoding.UTF8.GetBytes(responseText);
                // получаем поток ответа и пишем в него ответ
                response.ContentLength64 = buffer.Length;
                using Stream output = response.OutputStream;
                // отправляем данные
                await output.WriteAsync(buffer);
                await output.FlushAsync();
            }

            server.Stop();
            exchangeRates.SaveToJson(dataFile);
            Console.WriteLine(DateTime.Now + " Server stopped.");
            



        }
    }
}
