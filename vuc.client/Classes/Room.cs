namespace vuc.client.Classes;

public class Room
{
    public int RoomId { get; set; }
    public string Name { get; set; }

    public int? UserId { get; set; }

    public override string ToString()
    {
        return this.Name;
    }
}
