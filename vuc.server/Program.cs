using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<ChatContext>();
var app = builder.Build();

using var db = new ChatContext();

// ROOMS
app.MapGet("/rooms", async (ChatContext db) =>
    await db.Rooms.ToListAsync()
);

app.MapGet("/rooms/{id}", async (int id, ChatContext db) =>
{
    var room = db.Rooms.FirstOrDefault(r => r.RoomId == id);
    if (room is Room)
    {
        return Results.Ok(room);
    }
    else
    {
        return Results.NotFound();
    }
});

app.MapPost("/rooms", async (Room room, ChatContext db) =>
{
    db.Rooms.Add(room);
    await db.SaveChangesAsync();

    return Results.Created($"/rooms/{room.RoomId}", room);
});

// MESSAGES
app.MapGet("/messages/{id}", async (int id, ChatContext db) =>
{
    var room = db.Rooms.FirstOrDefault(r => r.RoomId == id);
    if (room is Room)
    {
        var messages = db.Rooms
            .Where(r => r.RoomId == id)
            .Include(blog => blog.Messages)
            .ToList();
        return Results.Ok(messages);
    }
    else
    {
        return Results.NotFound();
    }
});

app.MapPost("/messages", async (Message message, ChatContext db) =>
{
    db.Messages.Add(message);
    await db.SaveChangesAsync();

    return Results.Created($"/rooms/{message.MessageId}", message);
});

app.Run();