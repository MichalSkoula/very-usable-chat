using Microsoft.AspNetCore.Mvc;
using vuc.server;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<ChatContext>();

var app = builder.Build();

using var db = new ChatContext();

// ROOMS
app.MapGet(
    "/rooms",
    ([FromHeader(Name = "Authorization")] string? authorization, ChatContext db) =>
    {
        if (Auth.Login(db, authorization) == null)
        {
            return Results.Unauthorized();
        }

        return Results.Ok(db.Rooms.ToList());
    }
);

app.MapGet(
    "/rooms/{id}",
    ([FromRoute] int id, [FromHeader(Name = "Authorization")] string? authorization, ChatContext db) =>
    {
        if (Auth.Login(db, authorization) == null)
        {
            return Results.Unauthorized();
        }

        var room = db.Rooms.FirstOrDefault(r => r.RoomId == id);
        if (room is Room)
        {
            return Results.Ok(room);
        }
        else
        {
            return Results.NotFound();
        }
    }
);

app.MapPost(
    "/rooms",
    async (Room room, [FromHeader(Name = "Authorization")] string? authorization, ChatContext db) =>
    {
        User? user = Auth.Login(db, authorization);
        if (user == null)
        {
            return Results.Unauthorized();
        }

        if (db.Rooms.Where(r => r.Name == room.Name).Any())
        {
            return Results.Problem("Room already exists");
        }

        room.UserId = user.UserId;

        db.Rooms.Add(room);
        await db.SaveChangesAsync();

        return Results.Created($"/rooms/{room.RoomId}", room);
    }
);

// MESSAGES
app.MapGet("/messages/{id}", async (int id, [FromHeader(Name = "Authorization")] string? authorization, ChatContext db) =>
{
    if (Auth.Login(db, authorization) == null)
    {
        return Results.Unauthorized();
    }

    var room = db.Rooms.FirstOrDefault(r => r.RoomId == id);
    if (room is null)
    {
        return Results.NotFound();
    }

    var messages = db.Messages.Where(m => m.RoomId == id).Take(100).ToList();
    return Results.Ok(messages);
});

app.MapPost("/messages", async (Message message, [FromHeader(Name = "Authorization")] string? authorization, ChatContext db) =>
{
    User? user = Auth.Login(db, authorization);
    if (user == null)
    {
        return Results.Unauthorized();
    }

    var room = db.Rooms.FirstOrDefault(r => r.RoomId == message.RoomId);
    if (room is null)
    {
        return Results.NotFound();
    }

    message.UserId = user.UserId;

    db.Messages.Add(message);
    await db.SaveChangesAsync();

    return Results.Created($"/rooms/{message.MessageId}", message);
});

// USERS
app.MapPost("/users/register", async (UserBody user, ChatContext db) =>
{
    if (!Auth.CanRegister(db, user.UserName))
    {
        return Results.Problem("Problem with registration. User may already exists.");
    }

    db.Users.Add(new User(user.UserName, Auth.Hash(user.Password)));
    await db.SaveChangesAsync();
    return Results.Ok();
});

app.Run();