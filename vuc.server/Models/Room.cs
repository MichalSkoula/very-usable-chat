namespace vuc.server.Models;

using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

[Index(nameof(Name), IsUnique = true)]
public class Room
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int RoomId { get; set; }

    public string? Name { get; set; }

    [ForeignKey("Standard")]
    public int UserId { get; set; }
    public User User { get; set; }

    public List<Message> Messages { get; } = new();
}