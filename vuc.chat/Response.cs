namespace vuc.chat
{
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
}
