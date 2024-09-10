using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Talabat.APIs.Errors;
using Talabat.Repository.Data;

namespace Talabat.APIs.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class BuggyController : APIBaseController
	{
		private readonly StoreContext dbContext;

		public BuggyController(StoreContext dbContext)
        {
			this.dbContext = dbContext;
		}
        [HttpGet("NotFound")]
		public async Task<ActionResult> GetNotFoundRequest()
		{
			var Product = await dbContext.Products.FindAsync(100);
			if(Product is null) return NotFound(new ApiResponse(404));
			return  Ok(Product);
		}
        [HttpGet("ServerError")]
        public async Task<ActionResult> GetServerError()
		{
			var Product = await dbContext.Products.FindAsync(100);
			var productToReturn = Product.ToString();
			return Ok(productToReturn);
		}
		[HttpGet("BadRequest")]
		public  ActionResult GetBadRequest()
		{
			return  BadRequest();
		}
		[HttpGet("BadRequest/{id}")]
		public ActionResult GetBadRequest(int id)
		{
			return Ok();
		}
	}
}
