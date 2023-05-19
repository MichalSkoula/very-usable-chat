namespace vuc.client.Classes;

public class Message
{
    public int MessageId { get; set; }
    public string Content { get; set; }
    public User User { get; set; }
    public int RoomId { get; set; }
    public DateTime InsertedAt { get; set; }
}
