using System;
using System.Diagnostics;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Runtime.Serialization.Json;

using DataModels;

namespace StayOrGo
{
    public class OBADataProvider : IRouteProvider
    {
        public class Options
        {
            public Uri URL;
            public UInt16 PollingInterval = 30;
            public string APIKey;
        }

        public event EventHandler<List<DataModels.Route>> OnLocationRoutesChanged; 

        HttpClient client = OBADataProvider.createClient();

        private Options options;

        private string routesForLocation = "{ROOTURL}api/where/routes-for-location.json?key={APIKEY}&lat={LATITUDE}&lon={LONGITUDE}";
        private string stopsForLocation = "{ROOTURL}api/where/stops-for-location.json?key={APIKEY}&lat={LATITUDE}&lon={LONGITUDE}";
        private string stopInformation = "{ROOTURL}api/where/stop/{STOPID}.json?key={APIKEY}";
        private string routeInformation = "{ROOTURL}api/where/route/{ROUTEID}.json?key={APIKEY}";

        private Dictionary<string, string> baseDictionary = new Dictionary<string, string>(); 

        public OBADataProvider(Options options)
        {
            if(string.IsNullOrWhiteSpace(options.URL.ToString()))
            {
                throw new ArgumentException("No URL specified", "options.URL");
            }
            this.options = options;
            baseDictionary.Add("ROOTURL", options.URL.ToString());
            baseDictionary.Add("APIKEY", options.APIKey);

            this.OnLocationRoutesChanged += delegate {};
        }

        /// <summary>
        /// Creates configured REST client.
        /// </summary>
        /// <returns>The client.</returns>
        private static HttpClient createClient()
        {
            var client = new HttpClient();
            client.Timeout = TimeSpan.FromMilliseconds(200);

            return client;
        }

        /// <summary>
        /// Pumps data while watching for cancellation, calling the callback when new data is collected.
        /// </summary>
        /// <returns>The pump task</returns>
        /// <param name="token">Cancellation Token</param>
        /// <param name="callback">Action to get called when data arrives</param>
        public async Task<bool> Pump(CancellationToken token, Func<ClientLocation> clientLocationProvider, Action<DataModels.Trip> callback)
        {
            DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(DataModels.RoutesResponse));

            var clientDictionary = new Dictionary<string, string>();
            DateTime nextRunTime = DateTime.UtcNow;

            while (!token.IsCancellationRequested)
            {

                if (nextRunTime < DateTime.UtcNow)
                {
                    try 
                    {
						var location = clientLocationProvider();
						Debug.WriteLine("Location: {0}", location);
						clientDictionary["LATITUDE"] = location.Latitude.ToString();
						clientDictionary["LONGITUDE"] = location.Longitude.ToString();

						var url = routesForLocation.ExpandStrings(baseDictionary).ExpandStrings(clientDictionary);
						Debug.WriteLine(url);

						var response = await client.GetAsync(url);
                        if(response.IsSuccessStatusCode)
                        {
                            using(var stream = await response.Content.ReadAsStreamAsync())
                            {
								DataModels.RoutesResponse data = serializer.ReadObject(stream) as DataModels.RoutesResponse;
								Debug.WriteLine(data.Version);

                                if(data.Code == 200)
                                {
                                    // publish updated list of routes nearby.
                                    this.OnLocationRoutesChanged(this, data.Data.List);
                                }
							}
                        }
                        else
                        {
                            Debug.WriteLine(response.StatusCode);
                        }
					}
                    catch(Exception ex)
                    {
                        Debug.WriteLine(ex.ToString());
                    }
					nextRunTime = nextRunTime.AddSeconds(options.PollingInterval);
                    Debug.WriteLine("next poll at" + nextRunTime.ToString());
                }
                else
                {
                    await Task.Delay(250);
                }
            }

            return false;
        }
	}

    public class Model
    {
        
    }

    public static class Extensions
    {
		public static string ExpandStrings(this string input, Dictionary<string, string> values)
		{
			foreach (string key in values.Keys)
			{
				input = input.Replace("{" + key.ToUpper() + "}", values[key]);
			}

			return input;
		}        
    }
 }
