using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Talabat.Core.Services;

namespace Talabat.APIs.Helpers
{
	public class CachedAttribute : Attribute, IAsyncActionFilter
	{
		private readonly int expireTimeInSeconds;

		public CachedAttribute(int ExpireTimeInSeconds)
        {
			expireTimeInSeconds = ExpireTimeInSeconds;
		}
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
		{
			// inject IResponseCacheService Explicitly and need to allow dependancy injection for it
			var CachedService = context.HttpContext.RequestServices.GetRequiredService<IResponseCacheService>();

			var CachedKey = GenerateChacheKeyFromRequest(context.HttpContext.Request);

			var CachedResponse = await CachedService.GetCachedResponse(CachedKey);
			if (!string.IsNullOrEmpty(CachedResponse))
			{
				var contextResult = new ContentResult()
				{
					Content = CachedResponse,
					ContentType = "application/json",
					StatusCode = 200,
				};
				context.Result = contextResult;
				return;
			}
			var ExcutedEndPointConetxt = await next.Invoke();
			if(ExcutedEndPointConetxt.Result is OkObjectResult result)
			{
				await CachedService.CacheResponseAsync(CachedKey,result.Value,TimeSpan.FromSeconds(expireTimeInSeconds));
			}
		}

		private string GenerateChacheKeyFromRequest(HttpRequest request)
		{
			var KeyBuilder = new StringBuilder();
			KeyBuilder.Append(request.Path);
            foreach (var (key,value) in request.Query.OrderBy(X=>X.Key))
            {
				// Sort = Name
				// Page Index = 1
				// Page Size =3
                KeyBuilder.Append($"|{key}-{value}");
				// api/Products|Sort-Name|PageIndex-1|pageSize-5
            }

            return KeyBuilder.ToString();
		}
	}
}
