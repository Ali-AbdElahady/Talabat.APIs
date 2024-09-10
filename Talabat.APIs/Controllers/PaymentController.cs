using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Stripe;
using Talabat.APIs.DTO;
using Talabat.APIs.Errors;
using Talabat.Core.Services;

namespace Talabat.APIs.Controllers
{
	//[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
	public class PaymentController : APIBaseController
	{
		private readonly IPaymentService paymentService;
		private readonly IMapper mapper;

		public PaymentController(IPaymentService PaymentService,IMapper mapper)
        {
			paymentService = PaymentService;
			this.mapper = mapper;
		}
        // Create Or Update EndPoint
		[ProducesResponseType(typeof(CustomerBasketDto), StatusCodes.Status200OK)]
		[ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
        [HttpPost("{BasketId}")]
		public async Task<ActionResult<CustomerBasketDto>> CreateOrUpdatePaymentIntent(string BasketId)
		{
			var CustomerBasket = await paymentService.CreateOrUpdatePaymentIntent(BasketId);
			if (CustomerBasket == null) return BadRequest(new ApiResponse(400, "There is Aproblem with your Basket"));
			var MappedCustomerBasket = mapper.Map<CustomerBasketDto>(CustomerBasket);
			return Ok(MappedCustomerBasket);
		}

		const string endpointSecret = "whsec_495312d4422fd3ad60a0a8579513b3b6beee062979832df53ce16dd7e06c81b8";
		[HttpPost("webhook")]
		public async Task<IActionResult> StripeWebhook()
		{
			var json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();
			try
			{
				var stripeEvent = EventUtility.ConstructEvent(json,
					Request.Headers["Stripe-Signature"], endpointSecret);

				var paymentIntent = stripeEvent.Data.Object as PaymentIntent;
				// Handle the event
				if (stripeEvent.Type == Events.PaymentIntentPaymentFailed)
				{
					await paymentService.UpdatePaymentIntentToSuccedOrFailed(paymentIntent.Id,false);
				}
				else if (stripeEvent.Type == Events.PaymentIntentSucceeded)
				{
					await paymentService.UpdatePaymentIntentToSuccedOrFailed(paymentIntent.Id, true);
				}
				// ... handle other event types
				else
				{
					Console.WriteLine("Unhandled event type: {0}", stripeEvent.Type);
				}

				return Ok();
			}
			catch (StripeException e)
			{
				return BadRequest();
			}
		} 
	}
}
