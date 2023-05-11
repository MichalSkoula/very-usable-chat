using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;
using System.Net.Mail;

public class ChatContext : DbContext
{
    public string DbPath { get; }
    public DbSet<Room> Rooms { get; set; }
    public DbSet<Message> Messages { get; set; }

    public ChatContext()
    {
        if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development")
        {
            var folder = Environment.SpecialFolder.LocalApplicationData;
            var path = Environment.GetFolderPath(folder);
            DbPath = System.IO.Path.Join(path, "vuc.db");
        } 
        else
        {
            DbPath = "data/vuc.db";
        }
    }

    // The following configures EF to create a Sqlite database file in the
    // special "local" folder for your platform.
    protected override void OnConfiguring(DbContextOptionsBuilder options)
        => options.UseSqlite($"Data Source={DbPath}");
}

[Index(nameof(Name), IsUnique = true)]
public class Room
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int RoomId { get; set; }
    
    public string? Name { get; set; }

    public List<Message> Messages { get; } = new();
}

public class Message
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int MessageId { get; set; }
    public string? Content { get; set; }
    public string? UserIp { get; set; }
    public string? UserName { get; set; }

    public int RoomId { get; set; }
    //public Room Room { get; set; }
}


