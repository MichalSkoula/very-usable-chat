namespace vuc.server;

using Microsoft.EntityFrameworkCore;
using Models;

public class ChatContext : DbContext
{
    public string DbPath { get; }
    public DbSet<Room> Rooms { get; set; }
    public DbSet<Message> Messages { get; set; }
    public DbSet<User> Users { get; set; }

    public ChatContext()
    {
        DbPath = "data/vuc.db";
        /*
        if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development")
        {
            var folder = Environment.SpecialFolder.LocalApplicationData;
            var path = Environment.GetFolderPath(folder);
            DbPath = Path.Join(path, "vuc.db");
        }
        else
        {
            DbPath = "data/vuc.db";
        }*/
    }

    protected override void OnConfiguring(DbContextOptionsBuilder options) => options.UseSqlite($"Data Source={DbPath}");
}