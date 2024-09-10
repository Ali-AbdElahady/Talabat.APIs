using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Talabat.APIs.DTO;
using Talabat.APIs.Errors;
using Talabat.Core;
using Talabat.Core.Entities.Order_Agggregate;
using Talabat.Core.Services;

namespace Talabat.APIs.Controllers
{
	public class OrdersController : APIBaseController
	{
		private readonly IOrderService orderService;
		private readonly IMapper mapper;
		private readonly IUnitOfWork unitOfWork;

		public OrdersController(IOrderService orderService, IMapper mapper, IUnitOfWork unitOfWork)
		{
			this.orderService = orderService;
			this.mapper = mapper;
			this.unitOfWork = unitOfWork;
		}
		// Create Order
		[ProducesResponseType(typeof(Order), StatusCodes.Status200OK)]
		[ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]

		[HttpPost]
		[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
		public async Task<ActionResult<Order>> CreateOrder(OrderDto orderDto)
		{
			var BuyerEmail = User.FindFirstValue(ClaimTypes.Email);
			var MappedAddress = mapper.Map<AddressDto, Address>(orderDto.ShippingAddress);

			var Order = await orderService.CreateOrderAsync(BuyerEmail, orderDto.BasketId, orderDto.DeliveryMethodId, MappedAddress);
			if (Order == null) return BadRequest(new ApiResponse(400, "there is a problem with your order"));
			return Ok(Order);
		}

		// GetOrdersForUser 
		[ProducesResponseType(typeof(IReadOnlyList<Order>), StatusCodes.Status200OK)]
		[ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
		[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
		[HttpGet]
		public async Task<ActionResult<IReadOnlySet<OrderToReturnDto>>> GetOrdersForUser()
		{
			var BuyerEmail = User.FindFirstValue(ClaimTypes.Email);
			var Orders = await orderService.GetOrderForSpecificUserAsync(BuyerEmail);
			if (Orders == null) return BadRequest(new ApiResponse(404, "There is no orders for this user"));
			var MappedOrders = mapper.Map<IReadOnlyList<Order>,IReadOnlyList<OrderToReturnDto>>(Orders);
			return Ok(MappedOrders);
		}

		// Get Order By Id For User 
		[ProducesResponseType(typeof(Order), StatusCodes.Status200OK)]
		[ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
		[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
		[HttpGet("{id}")]
		public async Task<ActionResult<OrderToReturnDto>> GetOrderByIdForUser(int id)
		{
			var BuyerEmail = User.FindFirstValue(ClaimTypes.Email);
			var Order = await orderService.GetOrderByIdForSpecificUserAsync(BuyerEmail,id);
			if (Order == null) return BadRequest(new ApiResponse(404, $"There is no order with id = {id} for this user"));
			var MappedOrder = mapper.Map<Order, OrderToReturnDto>(Order);
			return Ok(MappedOrder);
		}

		[HttpGet("DeliveryyMethods")]
		public async Task<ActionResult<IReadOnlyList<DeliveryMethod>>> GetDeliveryMethod()
		{
			var DeliveryMethods = await unitOfWork.Reppository<DeliveryMethod>().GetAllAsync();
			return Ok(DeliveryMethods);
		}

	}
}
