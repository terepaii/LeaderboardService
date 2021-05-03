using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;


namespace LeaderboardClient
{
    public class Row
    {
        public Guid ClientId {get; set;}

        public long Rating {get; set;}

        public short LeaderboardId {get; set;}

    }

    public class UserModel
    {
        [JsonPropertyName("Id")]
        public Guid Id { get; set; }

        [JsonPropertyName("password")]
        public string Password { get; set; }

        [JsonPropertyName("user_name")]
        public string UserName { get; set; }

        [JsonPropertyName("token")]
        public string Token { get; set; }

        [JsonPropertyName("refresh_token")]
        public string RefreshToken { get; set; }

        [JsonPropertyName("created_at")]
        public string CreatedAt { get; set; }

        [JsonPropertyName("updated_at")]
        public string UpdatedAt { get; set;}

        [JsonPropertyName("user_id")]
        public Guid UserId { get; set; }
    }

    class Program
    {
        static HttpClient client = new HttpClient();

        static async Task<Uri> CreateRow(Row row, string token)
        {
            var json = JsonSerializer.Serialize(row);
            var stringContent = new StringContent(json, Encoding.UTF8, "application/json");
            var request = new HttpRequestMessage(HttpMethod.Post, client.BaseAddress);
            request.Content = stringContent;
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            HttpResponseMessage response = await client.SendAsync(request);
            Console.WriteLine($"Client received a HTTP response {(int)response.StatusCode} - {response.ReasonPhrase}");

            return response.Headers.Location;
        }

        static async Task<Row> GetRow(Guid clientId, short leaderboardId, string token)
        {
            Row row = null;

            var request = new HttpRequestMessage(HttpMethod.Get, client.BaseAddress + $"/{clientId}/{leaderboardId}");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            HttpResponseMessage response = await client.SendAsync(request); 

            if (response.IsSuccessStatusCode) 
            {
                var jsonString = await response.Content.ReadAsStringAsync();
                row = JsonSerializer.Deserialize<Row>(jsonString);
            }
            return row;
        }
        static async Task<List<Row>> GetRowsPaginated(short leaderboardId, string token, int offset=0, int limit=10)
        {
            List<Row> stats = null;

            var request = new HttpRequestMessage(HttpMethod.Get, client.BaseAddress + $"/{leaderboardId}?offset={offset}&limit={limit}");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            HttpResponseMessage response = await client.SendAsync(request);

            if (response.IsSuccessStatusCode)
            {
                var jsonString = await response.Content.ReadAsStringAsync();
                stats = JsonSerializer.Deserialize<List<Row>>(jsonString);
            } 
            return stats;
        }

        static async Task<bool> Update(Guid clientId, Row RowIn, string token)
        {
            var json = JsonSerializer.Serialize(RowIn);
            var stringContent = new StringContent(json, Encoding.UTF8, "application/json");
            var request = new HttpRequestMessage(HttpMethod.Put, client.BaseAddress);
            request.Content = stringContent;
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            HttpResponseMessage response = await client.SendAsync(request);

            if(response.IsSuccessStatusCode)
            {
                return true;
            }
            return false;
        }

        static async Task<bool> Delete(Guid clientId, short leaderboardId, string token)
        {
            var request = new HttpRequestMessage(HttpMethod.Delete, client.BaseAddress + $"/{clientId}/{leaderboardId}");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            HttpResponseMessage response = await client.SendAsync(request);

            if(response.IsSuccessStatusCode)
            {
                return true;
            }
            return false;
        }

        static async Task<string> Register(UserModel user)
        {
            var json = JsonSerializer.Serialize(user);
            var stringContent = new StringContent(json, Encoding.UTF8, "application/json");
            HttpResponseMessage response = await client.PostAsync("http://localhost/users/register", stringContent);

            return response.ToString();
        }

        static async Task<UserModel> Login(UserModel user)
        {
            var json = JsonSerializer.Serialize(user);
            var stringContent = new StringContent(json, Encoding.UTF8, "application/json");
            HttpResponseMessage response = await client.PostAsync("http://localhost/users/login", stringContent);

            var jsonString = await response.Content.ReadAsStringAsync();

            return JsonSerializer.Deserialize<UserModel>(jsonString);
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

        static string GenerateRandomNCharacterString(short n)
        {
            string s = "";
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            Random random = new Random();
            for (int i = 0; i < n; i++)
            {
                s = String.Concat(s, chars.ToCharArray()[random.Next(chars.Length)]);
            }
            return s;
        }

        static void ShowRows(List<Row> rows)
        {
            foreach(Row row in rows)
            {
                Console.WriteLine($"ClientId: {row.ClientId}\tRating: {row.Rating}");
            }
        }

        static void Main(string[] args)
        {
            RunAsync().GetAwaiter().GetResult();
        }

        static async Task RunAsync()
        {
            client.BaseAddress = new Uri("http://localhost/api/Client");
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json")
            );


            UserModel user = new UserModel {
                UserName = GenerateRandomNCharacterString(5).ToString(),
                Password = GenerateRandomNCharacterString(6).ToString()
            };

            Console.WriteLine(await Register(user));

            await Task.Delay(100);

            var loginResponse = await Login(user);

            // Row to write
            Row row = new Row
            {
                ClientId = loginResponse.UserId,
                Rating = GenerateRandomNDigitNumber(5),
                LeaderboardId = (short)GenerateRandomNDigitNumber(2)
            };

            // Create
            try
            {
               Console.WriteLine($"Creating a new row from client {row.ClientId} with Rating {row.Rating} on leaderboard {row.LeaderboardId}");
               var uri = await CreateRow(row, loginResponse.Token);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.StackTrace);
            }

            // Read one
            try
            {
               Console.WriteLine($"Retrieving the new row from client {row.ClientId} on leaderboard [{row.LeaderboardId}]");
               var retrievedRow = await GetRow(row.ClientId, row.LeaderboardId, loginResponse.Token);
               ShowRows(new List<Row> {retrievedRow});
            }
            catch (Exception e)
            {
                Console.WriteLine(e.StackTrace);
            }

            // Read all
            try 
            {
               Console.WriteLine("Getting all rows available");
               var retrievedRows = await GetRowsPaginated(row.LeaderboardId, loginResponse.Token);
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
                    Rating = GenerateRandomNDigitNumber(5),
                    LeaderboardId = row.LeaderboardId
                };

                Console.WriteLine($"Trying to update rating  of client [{row.ClientId}] from [{row.Rating}] to [{updatedRow.Rating}] on leaderboard [{row.LeaderboardId}]");
                var updated = await Update(row.ClientId, updatedRow, loginResponse.Token);

                if (updated)
                {
                    Console.WriteLine($"Successfully updated row for client [{row.ClientId}] on leaderboard [{row.LeaderboardId}]");
                }
                else
                {
                    Console.WriteLine($"Failed to updated row for client [{row.ClientId}] on leaderboard [{row.LeaderboardId}]");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.StackTrace);
            }

            //Delete
            try
            {
                Console.WriteLine($"Trying to delete row for client [{row.ClientId}] on leaderboard [{row.LeaderboardId}]");
                var deleted = await Delete(row.ClientId, row.LeaderboardId, loginResponse.Token);
                if (deleted)
                {
                    Console.WriteLine($"Successfully deleted row for client [{row.ClientId}] on leaderboard [{row.LeaderboardId}]");
                }
                else
                {
                    Console.WriteLine($"Failed to delete row for client [{row.ClientId}] on leaderboard [{row.LeaderboardId}]");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.StackTrace);
            }
        }
    }
}
