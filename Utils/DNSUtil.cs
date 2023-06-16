using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Caster.Utils
{
    internal class DNSUtil
    {
        public string type { get; set; }
        public string name { get; set; }
        public string content { get; set; }
        public int ttl { get; set; }
        public int priority { get; set; }
        public bool proxied { get; set; }

        public async Task<string> CreateCloudflareDNSCnameRecord(string zoneId, string email, string CF_token, string cnameName, string content)
        {
            var url = $"https://api.cloudflare.com/client/v4/zones/{zoneId}/dns_records";
            var client = new HttpClient();

            client.DefaultRequestHeaders.Add("X-Auth-Email", email);
            client.DefaultRequestHeaders.Add("X-Auth-Key", CF_token);
            //client.DefaultRequestHeaders.Add("Content-Type", "application/json");

            var dnsRecord = new DNSUtil
            {
                type = "CNAME",
                name = cnameName,
                content = content,
                ttl = 120,
                priority = 10,
                proxied = false
            };

            var data = new StringContent(JsonConvert.SerializeObject(dnsRecord), Encoding.UTF8, "application/json");
            var response = await client.PostAsync(url, data);

            return await response.Content.ReadAsStringAsync();
        }

        public async Task<string> CreateCloudflareDNSTXTRecord(string zoneId, string email, string CF_token, string cnameName, string content)
        {
            var url = $"https://api.cloudflare.com/client/v4/zones/{zoneId}/dns_records";
            var client = new HttpClient();

            client.DefaultRequestHeaders.Add("X-Auth-Email", email);
            client.DefaultRequestHeaders.Add("X-Auth-Key", CF_token);
            //client.DefaultRequestHeaders.Add("Content-Type", "application/json");

            var dnsRecord = new DNSUtil
            {
                type = "TXT",
                name = cnameName,
                content = content,
                ttl = 120,
                proxied = false
            };

            var data = new StringContent(JsonConvert.SerializeObject(dnsRecord), Encoding.UTF8, "application/json");
            var response = await client.PostAsync(url, data);

            return await response.Content.ReadAsStringAsync();
        }
    }

}
