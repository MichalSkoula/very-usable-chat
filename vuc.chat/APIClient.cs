using Newtonsoft.Json;
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
            Authenticate(userName, password);

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
            Authenticate();

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
            Authenticate();

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

    public static List<Message> GetMessages(int roomId)
    {
        try
        {
            HttpResponseMessage response = client.GetAsync(App.SaveData.Server + "messages/" + roomId).Result;
            string json = response.Content.ReadAsStringAsync().Result;

            List<Message> messages = JsonConvert.DeserializeObject<List<Message>>(json);
            return messages;
        }
        catch (HttpRequestException e)
        {
            System.Diagnostics.Debug.WriteLine(e.Message);
            return new List<Message>();
        }
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

    private static void Authenticate(string? userName = null, string? password = null)
    {
        // basic auth
        string credentials = Convert.ToBase64String(Encoding.ASCII.GetBytes($"{userName ?? App.SaveData.UserName}:{password ?? App.SaveData.Password}"));
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", credentials);
    }
}
