namespace vuc.server.Models;

using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

[Index(nameof(UserName), IsUnique = true)]
public class User
{
    public User(string userName, string password)
    {
        UserName = userName;
        Password = password;
    }

    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int UserId { get; set; }

    public string UserName { get; set; }

    [JsonIgnore]
    public string Password { get; set; }
}
