namespace vuc.client.Classes;

public class Response
{
    public bool Success { get; set; }
    public string Message { get; set; }

    public Response(bool success = false, string message = "")
    {
        Success = success;
        Message = message;
    }
}
