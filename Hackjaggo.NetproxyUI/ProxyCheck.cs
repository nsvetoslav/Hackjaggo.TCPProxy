using Newtonsoft.Json.Linq;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Hackjaggo.NetproxyUI
{
    public class ProxyCheck
    {
        public class IpInfoDetails
        {
            public string Asn { get; set; } = string.Empty;
            public string Range { get; set; } = string.Empty;
            public string Provider { get; set; } = string.Empty;
            public string Organisation { get; set; } = string.Empty;
            public string Continent { get; set; } = string.Empty;
            public string ContinentCode { get; set; } = string.Empty;
            public string Country { get; set; } = string.Empty;
            public string IsoCode { get; set; } = string.Empty;
            public string Region { get; set; } = string.Empty;
            public string RegionCode { get; set; } = string.Empty;
            public string Timezone { get; set; } = string.Empty;
            public string City { get; set; } = string.Empty;
            public string Postcode { get; set; } = string.Empty;
            public double Latitude { get; set; }
            public double Longitude { get; set; }
            public Currency Currency { get; set; } = new();
            public Devices Devices { get; set; } = new();
            public string Proxy { get; set; } = string.Empty;
            public string Type { get; set; } = string.Empty;
        }

        public class Currency
        {
            public string Code { get; set; } = string.Empty;
            public string Name { get; set; } = string.Empty;
            public string Symbol { get; set; } = string.Empty;
        }

        public class Devices
        {
            public int Address { get; set; }
            public int Subnet { get; set; }
        }

        public async Task<IpInfoDetails> GetProxyInfoAsync(string ipAddress)
        {
            try
            {
                string url = $"https://proxycheck.io/v2/{ipAddress}?vpn=1&asn=1";

                using (HttpClient client = new HttpClient())
                {
                    var response = await client.GetStringAsync(url).ConfigureAwait(false);

                    var jsonResponse = JObject.Parse(response);

                    if (jsonResponse["status"]?.ToString() == "ok" && jsonResponse[ipAddress] != null)
                    {
                        var ipDetails = jsonResponse![ipAddress]!.ToObject<IpInfoDetails>();
                        return ipDetails!;
                    }

                    return null!;
                }
            }
            catch (Exception)
            {
                return null!;
            }
        }
    }
}
