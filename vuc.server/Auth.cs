namespace vuc.server;

using System.Text;

public static class Auth
{
    public static User? Login(ChatContext db, string? authorization)
    {
        if (authorization == null)
        {
            return null;
        }
        

        UserBody credentials = ExtractCredentials(authorization);
        if (credentials == null)
        {
            return null;
        }

        return db.Users.FirstOrDefault(u => u.Password == Hash(credentials.Password) && u.UserName == credentials.UserName);
    }

    public static bool CanRegister(ChatContext db, string username)
    {
        return db.Users.FirstOrDefault(u => u.UserName == username) is null;
    }

    public static string Hash(string text)
    {
        var sha = new System.Security.Cryptography.SHA256Managed();

        // Convert the string to a byte array first, to be processed
        byte[] textBytes = System.Text.Encoding.UTF8.GetBytes(text);
        byte[] hashBytes = sha.ComputeHash(textBytes);

        // Convert back to a string, removing the '-' that BitConverter adds
        string hash = BitConverter.ToString(hashBytes).Replace("-", String.Empty);

        return hash;
    }

    public static UserBody? ExtractCredentials(string authorization)
    {
        if (String.IsNullOrEmpty(authorization) || !authorization.StartsWith("Basic"))
        {
            return null;
        }

        // extract username and password
        authorization = authorization.Substring("Basic ".Length).Trim();
        authorization = Encoding.GetEncoding("iso-8859-1").GetString(Convert.FromBase64String(authorization));

        string username = authorization.Split(':')[0];
        string password = authorization.Split(':')[1];

        return new UserBody(username, password);
    }
}