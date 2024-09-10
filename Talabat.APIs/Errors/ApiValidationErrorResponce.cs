namespace Talabat.APIs.Errors
{
	public class ApiValidationErrorResponce : ApiResponse
	{
		public IEnumerable<string> Errors { get; set; }
        public ApiValidationErrorResponce():base(400) 
        {
            Errors = new List<string>();
        }
    }
}
