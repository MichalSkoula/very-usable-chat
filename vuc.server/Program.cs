using Microsoft.AspNetCore.Mvc;
using vuc.server;
using Microsoft.AspNetCore.OpenApi;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<ChatContext>();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI(c => {
    // hide schemas
    c.DefaultModelsExpandDepth(-1);
});

using var db = new ChatContext();

// USERS
app.MapPost(
    "/users/register",
    async (UserBody user, ChatContext db) =>
    {
        if (!Auth.CanRegister(db, user.UserName))
        {
            return Results.Problem("Problem with registration. User may already exists.");
        }

        db.Users.Add(new User(user.UserName, Auth.Hash(user.Password)));
        await db.SaveChangesAsync();
        return Results.Ok();
    }
)
.WithDescription("Register with UserName and Password. UserName must be unique. All other methods require Authorization: Basic header.")
.WithOpenApi();

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
)
.WithDescription("Get all available chat rooms.")
.WithOpenApi();

app.MapGet(
    "/rooms/{roomId}",
    ([FromRoute] int roomId, [FromHeader(Name = "Authorization")] string? authorization, ChatContext db) =>
    {
        if (Auth.Login(db, authorization) == null)
        {
            return Results.Unauthorized();
        }

        var room = db.Rooms.FirstOrDefault(r => r.RoomId == roomId);
        if (room is Room)
        {
            return Results.Ok(room);
        }
        else
        {
            return Results.NotFound();
        }
    }
)
.WithDescription("Get details of one room")
.WithOpenApi();

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
)
.WithDescription("Create new chat room. Name must be unique.")
.WithOpenApi();

// MESSAGES
app.MapGet(
    "/messages/{roomId}", 
    async ([FromRoute] int roomId, [FromHeader(Name = "Authorization")] string? authorization, ChatContext db) =>
    {
        if (Auth.Login(db, authorization) == null)
        {
            return Results.Unauthorized();
        }

        var room = db.Rooms.FirstOrDefault(r => r.RoomId == roomId);
        if (room is null)
        {
            return Results.NotFound();
        }

        var messages = db.Messages.Where(m => m.RoomId == roomId).Take(100).ToList();
        return Results.Ok(messages);
    }
)
.WithDescription("Get chat messages from selected room.")
.WithOpenApi();

app.MapPost(
    "/messages/{roomId}", 
    async ([FromRoute] int roomId, [FromBody]Message message, [FromHeader(Name = "Authorization")] string? authorization, ChatContext db) =>
    {
        User? user = Auth.Login(db, authorization);
        if (user == null)
        {
            return Results.Unauthorized();
        }

        var room = db.Rooms.FirstOrDefault(r => r.RoomId == roomId);
        if (room is null)
        {
            return Results.NotFound();
        }

        message.UserId = user.UserId;
        message.RoomId = roomId;

        db.Messages.Add(message);
        await db.SaveChangesAsync();

        return Results.Created($"/rooms/{message.MessageId}", message);
    }
)
.WithDescription("Post new message to selected chat room.")
.WithOpenApi();

app.Run();