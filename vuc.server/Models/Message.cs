namespace vuc.server.Models;

using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

public class Message
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int MessageId { get; set; }

    public string? Content { get; set; }

    [ForeignKey("Standard")]
    [JsonIgnore]
    public int UserId { get; set; }
    public User User { get; set; }

    [ForeignKey("Standard")]
    public int RoomId { get; set; }
    [JsonIgnore]
    public Room Room { get; set; }

    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public DateTime InsertedAt { get; set; }
}