namespace vuc.server;

public class UserBody
{
    public string UserName { get; set; }
    public string Password { get; set; }

    public UserBody(string userName, string password)
    {
        this.UserName = userName;
        this.Password = password;
    }
}
