﻿using Hackjaggo.Proxy;
using Newtonsoft.Json.Linq;

namespace Hackjaggo.NetproxyUI
{
    public class ProxyCheck
    {
        private static Dictionary<string, IpInfoDetails> _ipInfoCache = new();

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

        public async Task<IpInfoDetails?> GetProxyInfoAsync(string ipAddress)
        {
            IpInfoDetails? ipInfoDetails = null;

            if (_ipInfoCache.TryGetValue(ipAddress, out ipInfoDetails))
                return ipInfoDetails;

            try
            {
                string url = $"https://proxycheck.io/v2/{ipAddress}?vpn=1&asn=1";

                using (HttpClient client = new HttpClient())
                {
                    var response = await client.GetStringAsync(url).ConfigureAwait(false);

                    var jsonResponse = JObject.Parse(response);

                    if (jsonResponse["status"]?.ToString() == "ok" && jsonResponse[ipAddress] != null)
                    {
                        ipInfoDetails = jsonResponse![ipAddress]!.ToObject<IpInfoDetails>()!;
                        _ipInfoCache.TryAdd(ipAddress, ipInfoDetails);
                        return ipInfoDetails!;
                    }

                    Logger.LogInfo($"ProxyCheck failed for IP: {ipAddress}");

                    return ipInfoDetails;
                }
            }
            catch (Exception ex)
            {
                Logger.LogError($"Exception: {ex.Message}");
                return ipInfoDetails;
            }
        }
    }
}
