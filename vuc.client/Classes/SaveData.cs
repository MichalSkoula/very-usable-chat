namespace vuc.client.Classes;

public class SaveData
{
    public string? UserName { get; set; }
    public string? Password { get; set; }
    public string? Server { get; set; }

    public SaveData(string? sever, string? username = null, string? password = null)
    {
        UserName = username;
        Password = password;
        Server = sever;
    }

    public SaveData()
    {
    }
}
