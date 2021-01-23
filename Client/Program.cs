using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;


namespace StatsClient
{
    public class Stat
    {
        public long ClientId {get; set;}

        public long Rating {get; set;}

    }
    class Program
    {
        static HttpClient client = new HttpClient();

        static void ShowStats(List<Stat> stats)
        {
            foreach(Stat stat in stats)
            {
                Console.WriteLine($"ClientId: {stat.ClientId}\tRating: {stat.Rating}");
            }
        }

        static async Task<Uri> CreateStat(Stat stat)
        {
            var json = JsonSerializer.Serialize(stat);
            var stringContent = new StringContent(json, Encoding.UTF8, "application/json");
            HttpResponseMessage response = await client.PostAsync(client.BaseAddress, stringContent);
            Console.WriteLine($"Client received a HTTP response {(int)response.StatusCode} - {response.ReasonPhrase}");

            return response.Headers.Location;
        }

        static async Task<Stat> GetStat(long ClientId)
        {
            Stat stat = null;

            HttpResponseMessage response = await client.GetAsync(client.BaseAddress + $"/{ClientId}"); 

            if (response.IsSuccessStatusCode)
            {
                var jsonString = await response.Content.ReadAsStringAsync();
                stat = JsonSerializer.Deserialize<Stat>(jsonString);
            }
            return stat;
        }
        static async Task<List<Stat>> GetStats()
        {
            List<Stat> stats = null;

            HttpResponseMessage response = await client.GetAsync("");

            if (response.IsSuccessStatusCode)
            {
                var jsonString = await response.Content.ReadAsStringAsync();
                stats = JsonSerializer.Deserialize<List<Stat>>(jsonString);
            } 
            return stats;
        }

        static async Task<bool> Update(long clientId, Stat StatIn)
        {
            var json = JsonSerializer.Serialize(StatIn);
            var stringContent = new StringContent(json, Encoding.UTF8, "application/json");
            HttpResponseMessage response = await client.PutAsync(client.BaseAddress, stringContent);

            if(response.IsSuccessStatusCode)
            {
                return true;
            }
            return false;
        }

        static async Task<bool> Delete(long clientId)
        {
            HttpResponseMessage response = await client.DeleteAsync(client.BaseAddress + $"/{clientId}");

            if(response.IsSuccessStatusCode)
            {
                return true;
            }
            return false;
        }
        static void Main(string[] args)
        {
            RunAsync().GetAwaiter().GetResult();
        }

        static int GenerateRandomNDigitNumber(short n)
        {
            string s = "";
            Random random = new Random();
            for (int i = 0; i < n; i++)
            {
                s = String.Concat(s, random.Next(9).ToString());
            }
            return Int32.Parse(s);
        }
        static async Task RunAsync()
        {
            client.BaseAddress = new Uri("http://localhost:5000/api/Stats");
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json")
            );

            // Stat to write
            Stat stat = new Stat
            {
                ClientId = GenerateRandomNDigitNumber(5),
                Rating = GenerateRandomNDigitNumber(5)
            };

            // Create
            try
            {
               Console.WriteLine($"Creating a new stat from client {stat.ClientId} with Rating {stat.Rating}");
               var uri = await CreateStat(stat);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.StackTrace);
            }

            // Read one
            try
            {
               Console.WriteLine($"Retrieving the new stat from client {stat.ClientId}");
               var retrievedStat = await GetStat(stat.ClientId);
               ShowStats(new List<Stat>{retrievedStat});
            }
            catch (Exception e)
            {
                Console.WriteLine(e.StackTrace);
            }

            // Read all
            try 
            {
               Console.WriteLine("Getting all stats available");
               var retrievedStats = await GetStats();
               ShowStats(retrievedStats); 
            }
            catch (Exception e)
            {
                Console.WriteLine(e.StackTrace);
            }

            // Update
            try
            {
                Stat updatedStat = new Stat
                {
                    ClientId = stat.ClientId, 
                    Rating = GenerateRandomNDigitNumber(5)
                };

                Console.WriteLine($"Trying to update rating  of client [{stat.ClientId}] from [{stat.Rating}] to [{updatedStat.Rating}]");
                var updated = await Update(stat.ClientId, updatedStat);

            }
            catch (Exception e)
            {
                Console.WriteLine(e.StackTrace);
            }

            //Delete
            try
            {
                Console.WriteLine($"Trying to delete stat for client [{stat.ClientId}]");
                var deleted = await Delete(stat.ClientId);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.StackTrace);
            }

        }
    }
}
