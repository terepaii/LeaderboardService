using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;


namespace LeaderboardClient
{
    public class Row
    {
        public long ClientId {get; set;}

        public long Rating {get; set;}

    }
    class Program
    {
        static HttpClient client = new HttpClient();

        static async Task<Uri> CreateRow(Row row)
        {
            var json = JsonSerializer.Serialize(row);
            var stringContent = new StringContent(json, Encoding.UTF8, "application/json");
            HttpResponseMessage response = await client.PostAsync(client.BaseAddress, stringContent);
            Console.WriteLine($"Client received a HTTP response {(int)response.StatusCode} - {response.ReasonPhrase}");

            return response.Headers.Location;
        }

        static async Task<List<Row>> GetRow(long ClientId)
        {
            List<Row> row = null;

            HttpResponseMessage response = await client.GetAsync(client.BaseAddress + $"/{ClientId}"); 

            if (response.IsSuccessStatusCode)
            {
                var jsonString = await response.Content.ReadAsStringAsync();
                row = JsonSerializer.Deserialize<List<Row>>(jsonString);
            }
            return row;
        }
        static async Task<List<Row>> GetRows()
        {
            List<Row> stats = null;

            HttpResponseMessage response = await client.GetAsync("");

            if (response.IsSuccessStatusCode)
            {
                var jsonString = await response.Content.ReadAsStringAsync();
                stats = JsonSerializer.Deserialize<List<Row>>(jsonString);
            } 
            return stats;
        }

        static async Task<bool> Update(long clientId, Row RowIn)
        {
            var json = JsonSerializer.Serialize(RowIn);
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

        static void ShowRows(List<Row> rows)
        {
            foreach(Row row in rows)
            {
                Console.WriteLine($"ClientId: {row.ClientId}\tRating: {row.Rating}");
            }
        }

        static async Task RunAsync()
        {
            client.BaseAddress = new Uri("http://localhost:5000/api/Client");
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json")
            );

            // Row to write
            Row row = new Row
            {
                ClientId = GenerateRandomNDigitNumber(5),
                Rating = GenerateRandomNDigitNumber(5)
            };

            // Create
            try
            {
               Console.WriteLine($"Creating a new row from client {row.ClientId} with Rating {row.Rating}");
               var uri = await CreateRow(row);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.StackTrace);
            }

            // Read one
            try
            {
               Console.WriteLine($"Retrieving the new row from client {row.ClientId}");
               var retrievedRow = await GetRow(row.ClientId);
               ShowRows(retrievedRow);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.StackTrace);
            }

            // Read all
            try 
            {
               Console.WriteLine("Getting all rows available");
               var retrievedRows = await GetRows();
               ShowRows(retrievedRows); 
            }
            catch (Exception e)
            {
                Console.WriteLine(e.StackTrace);
            }

            // Update
            try
            {
                Row updatedRow = new Row
                {
                    ClientId = row.ClientId, 
                    Rating = GenerateRandomNDigitNumber(5)
                };

                Console.WriteLine($"Trying to update rating  of client [{row.ClientId}] from [{row.Rating}] to [{updatedRow.Rating}]");
                var updated = await Update(row.ClientId, updatedRow);

                if (updated)
                {
                    Console.WriteLine($"Successfully updated row for client [{row.ClientId}]");
                }
                else
                {
                    Console.WriteLine($"Failed to updated row for client [{row.ClientId}]");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.StackTrace);
            }

            //Delete
            try
            {
                Console.WriteLine($"Trying to delete row for client [{row.ClientId}]");
                var deleted = await Delete(row.ClientId);
                if (deleted)
                {
                    Console.WriteLine($"Successfully deleted row for client [{row.ClientId}]");
                }
                else
                {
                    Console.WriteLine($"Failed to delete row for client [{row.ClientId}]");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.StackTrace);
            }
        }
    }
}
