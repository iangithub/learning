using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace JsonSample
{
    class Program
    {
        static async Task Main(string[] args)
        {
            using (var client = new HttpClient())
            using (var request = new HttpRequestMessage())
            {
                request.Method = HttpMethod.Get;
                request.RequestUri = new Uri("http://data.ntpc.gov.tw/api/v1/rest/datastore/382000000A-000352-001");
                var response = await client.SendAsync(request);
                var responseBody = await response.Content.ReadAsStringAsync();
                UbikeOpenDatas tranresult = JsonConvert.DeserializeObject<UbikeOpenDatas>(responseBody);

                foreach (var item in tranresult.result.records)
                {
                    Console.WriteLine($"站點：{item.sna} / 可借車位數：{item.sbi} / 可還空位數：{item.bemp}");
                }
            }
        }
    }
}
