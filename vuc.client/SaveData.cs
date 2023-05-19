namespace vuc.client;

public class SaveData
{
    public string? UserName { get; set; }
    public string? Password { get; set; }
    public string? RoomId { get; set; }
    public string Server { get; set; }

    public SaveData(string sever, string? username = null, string? password = null, string? roomid = null)
    {
        UserName = username;
        Password = password;
        RoomId = roomid;
        Server = sever;
    }
}
