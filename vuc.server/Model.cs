namespace vuc.server;

using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

public class ChatContext : DbContext
{
    public string DbPath { get; }
    public DbSet<Room> Rooms { get; set; }
    public DbSet<Message> Messages { get; set; }
    public DbSet<User> Users { get; set; }

    public ChatContext()
    {
        if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development")
        {
            var folder = Environment.SpecialFolder.LocalApplicationData;
            var path = Environment.GetFolderPath(folder);
            DbPath = Path.Join(path, "vuc.db");
        }
        else
        {
            DbPath = "data/vuc.db";
        }
    }

    protected override void OnConfiguring(DbContextOptionsBuilder options) => options.UseSqlite($"Data Source={DbPath}");
}

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
}

[Index(nameof(UserName), IsUnique = true)]
public class User
{
    public User(string userName, string password)
    {
        this.UserName = userName;
        this.Password = password;
    }

    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int UserId { get; set; }

    public string UserName { get; set; }

    [JsonIgnore]
    public string Password { get; set; }
}
