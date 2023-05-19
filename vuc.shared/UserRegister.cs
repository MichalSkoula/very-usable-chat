namespace vuc.shared;

public class UserRegister
{
    public string UserName { get; set; }
    public string Password { get; set; }

    public UserRegister(string userName, string password)
    {
        this.UserName = userName;
        this.Password = password;
    }
}