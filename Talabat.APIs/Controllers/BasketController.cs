using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Talabat.APIs.Errors;
using Talabat.Core.Entities;
using Talabat.Core.Repositories;

namespace Talabat.APIs.Controllers
{
	public class BasketController : APIBaseController
	{
		private readonly IBasketRepository basketRepo;

		public BasketController(IBasketRepository _basketRepo)
        {
			basketRepo = _basketRepo;
		}
        // GET Or ReCreate Basket
        [HttpGet("{BasketId}")]
		public async Task<ActionResult<CustomerBasket>> GetCustomerBasket (string BasketId)
		{
			var Basket = await basketRepo.GetBasketAsync(BasketId);
			return Basket is null ? new CustomerBasket(BasketId) : Basket;
		}

		// Update Or Create New Basket
		[HttpPost]
		public async Task<ActionResult<CustomerBasket>> UpdateBasket(CustomerBasket Basket)
		{
			var CreatedOrUpdatedBasket = await basketRepo.UpdateBasketAsync(Basket);
			return CreatedOrUpdatedBasket is null ? BadRequest(new ApiResponse(400)) : Ok(CreatedOrUpdatedBasket);  
		}

		// Delete Basket
		[HttpDelete]
		public async Task<ActionResult<bool>> DeleteBasket(string BasketId)
		{
			return await basketRepo.DeleteBasketAsync(BasketId);
		}
	}
}
