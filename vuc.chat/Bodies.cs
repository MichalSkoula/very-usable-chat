namespace vuc.chat;

public class Room
{
    public int RoomId { get; set; }
    public string Name { get; set; }

    public override string ToString()
    {
        return this.Name;
    }
}

public class Message
{
    public int MessageId { get; set; }
    public string Content { get; set; }
    public User User { get; set; }
    public int RoomId { get; set; }
}

public class User
{
    public int UserId { get; set; }
    public string UserName { get; set; }
}