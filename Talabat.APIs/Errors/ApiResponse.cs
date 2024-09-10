
namespace Talabat.APIs.Errors
{
	public class ApiResponse
	{
        public int StatusCode { get; set; }
		public string Message { get; set; }

        public ApiResponse(int statusCode, string? message = null)
        {
            StatusCode = statusCode;
            Message = message ?? GetDefaultMessageForStatusCode(statusCode);
        }

		private string? GetDefaultMessageForStatusCode(int? statusCode)
		{
            //500 => Internal Server Error
            //400 => Bad Request
            //401 => Unauthorized
            //404 => Not Found
			return statusCode switch
			{
				500 => "Internal Server Error",
				400 => "Bad Request",
                401 => "Unauthorized",
                404 => "Not Found",
				_ => null
			};
		}
	}
}
