namespace vuc.chat;

public class Response
{
    public bool Success { get; set; }
    public string Message { get; set; }

    public Response(bool success = false, string message = "")
    {
        this.Success = success;
        this.Message = message;
    }
}

public class ResponseO
{
    public bool Success { get; set; }
    public object Content { get; set; }

    public ResponseO(bool success = false, object content = null)
    {
        this.Success = success;
        this.Content = content;
    }
}
