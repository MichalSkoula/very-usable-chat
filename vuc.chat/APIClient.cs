using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace vuc.chat;

public static class APIClient
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

            HttpContent content = new StringContent(System.Text.Json.JsonSerializer.Serialize(jsonObject), Encoding.UTF8, "application/json");
            HttpResponseMessage response = client.PostAsync(App.SaveData.Server + "users/register", content).Result;

            string message = TryToParseDetail(response);

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

            HttpContent content = new StringContent(System.Text.Json.JsonSerializer.Serialize(new { }), Encoding.UTF8, "application/json");
            HttpResponseMessage response = client.PostAsync(App.SaveData.Server + "users/login", content).Result;

            return response.IsSuccessStatusCode;
        }
        catch (HttpRequestException e)
        {
            return false;
        }
    }

    public static Response CreateRoom(string roomTitle)
    {
        try
        {
            // basic auth
            string credentials = Convert.ToBase64String(Encoding.ASCII.GetBytes($"{App.SaveData.UserName}:{App.SaveData.Password}"));
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", credentials);

            HttpContent content = new StringContent(System.Text.Json.JsonSerializer.Serialize(new { Name = roomTitle }), Encoding.UTF8, "application/json");
            HttpResponseMessage response = client.PostAsync(App.SaveData.Server + "rooms", content).Result;

            string message = TryToParseDetail(response);

            return new Response(response.IsSuccessStatusCode, message);
        }
        catch (HttpRequestException e)
        {
            return new Response(false, e.Message);
        }
    }

    public static List<Room> GetRooms()
    {
        try
        {
            // basic auth
            string credentials = Convert.ToBase64String(Encoding.ASCII.GetBytes($"{App.SaveData.UserName}:{App.SaveData.Password}"));
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", credentials);

            HttpResponseMessage response = client.GetAsync(App.SaveData.Server + "rooms").Result;
            string json = response.Content.ReadAsStringAsync().Result;

            List<Room> rooms = JsonConvert.DeserializeObject<List<Room>>(json);
            return rooms;
        }
        catch (HttpRequestException e)
        {
            System.Diagnostics.Debug.WriteLine(e.Message);
            return new List<Room>();
        }
    }

    public static JObject? TryToParseContent(HttpResponseMessage response)
    {
        try
        {
            string responseContent = response.Content.ReadAsStringAsync().Result;
            JObject o = JObject.Parse(responseContent);
            return o;
        }
        catch (Exception ex)
        {

        }

        return null;
    }

    public static string TryToParseDetail(HttpResponseMessage response)
    {
        try
        {
            string responseContent = response.Content.ReadAsStringAsync().Result;
            JsonDocument jsonDoc = JsonDocument.Parse(responseContent);
            return jsonDoc.RootElement.GetProperty("detail").GetString();
        }
        catch (Exception ex)
        {

        }

        return "";
    }
}
