namespace Common.ExceptionHandler
{
    public class ErrorResponse
    {
        public String? Message { get; set; }
        public Int32? StatusCode { get; set; }

        public ErrorResponse(String message, Int32 statusCode)
        {
            this.Message = message;
            this.StatusCode = statusCode;
        }
    }
}
