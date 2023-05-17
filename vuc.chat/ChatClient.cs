using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace vuc.chat
{
    public static class ChatClient
    {
        static readonly HttpClient client = new HttpClient();

        public static Response Register(string userName, string password)
        {
            try
            {
                Dictionary<string, object> jsonObject = new Dictionary<string, object>()
                {
                    { "UserName", userName },
                    { "Password", password }
                };

                HttpContent content = new StringContent(JsonSerializer.Serialize(jsonObject), Encoding.UTF8, "application/json");
                HttpResponseMessage response = client.PostAsync(App.SaveData.Server + "users/register", content).Result;

                string message = "";
                try
                {
                    string responseContent = response.Content.ReadAsStringAsync().Result;
                    JsonDocument jsonDoc = JsonDocument.Parse(responseContent);
                    message = jsonDoc.RootElement.GetProperty("detail").GetString();
                }
                catch (Exception ex)
                {

                }

                return new Response(response.IsSuccessStatusCode, message);
            }
            catch (HttpRequestException e)
            {
                return new Response(false, e.Message);
            }
        }

        public static bool Login(string userName, string password)
        {
            try
            {
                // basic auth
                string credentials = Convert.ToBase64String(Encoding.ASCII.GetBytes($"{userName}:{password}"));
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", credentials);

                HttpContent content = new StringContent(JsonSerializer.Serialize(new {}), Encoding.UTF8, "application/json");
                HttpResponseMessage response = client.PostAsync(App.SaveData.Server + "users/login", content).Result;

                return response.IsSuccessStatusCode;
            }
            catch (HttpRequestException e)
            {
                return false;
            }
        }
    }
}
