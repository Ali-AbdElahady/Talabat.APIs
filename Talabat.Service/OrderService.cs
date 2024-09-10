using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Talabat.Core;
using Talabat.Core.Entities;
using Talabat.Core.Entities.Order_Agggregate;
using Talabat.Core.Repositories;
using Talabat.Core.Services;
using Talabat.Core.Specifications.Order_Spec;

namespace Talabat.Service
{
	public class OrderService : IOrderService
	{
		private readonly IBasketRepository basketRepository;
		private readonly IUnitOfWork unitOfWork;
		private readonly IPaymentService paymentService;

		public OrderService(IBasketRepository basketRepository, 
			IUnitOfWork unitOfWork,IPaymentService paymentService)
        {
			this.basketRepository = basketRepository;
			this.unitOfWork = unitOfWork;
			this.paymentService = paymentService;
		}
        public async Task<Order?> CreateOrderAsync(string BuyerEmail, string BasketId, int DeliveryMethodId, Address ShippingAddress)
		{
			// Bussniss Logic
			// 1- Get Basket From Basket Repo
			var Basket = await basketRepository.GetBasketAsync(BasketId);
			// 2- Get Selected Items at Basket from Product Repo
			var OrderItems = new List<OrderItem>();
			if(Basket?.Items.Count > 0)
			{
                foreach (var item in Basket.Items)
                {
					var Product = await unitOfWork.Reppository<Product>().GetByIdAsync(item.Id);
					var ProductItemOrdered = new ProductItemOrder(Product.Id,Product.Name,Product.PictureUrl);
					var OrderItem = new OrderItem(ProductItemOrdered,Product.Price,item.Quantity);
					OrderItems.Add(OrderItem);
                }
            }
			// 3- Calculate SubTotal
			var SubTotal = OrderItems.Sum(item=>item.Price * item.Quantity);
			// 4- Get Delivery Method from DeliveryMethod repo
			var DeliveryMethod = await unitOfWork.Reppository<DeliveryMethod>().GetByIdAsync(DeliveryMethodId);
			// 5- Create Order
			var Spec = new OrderSpecification(Basket.PaymentIntentId);
			var ExOrder = await unitOfWork.Reppository<Order>().GetEntityWithSpecAsync(Spec);
			if(ExOrder != null)
			{
				unitOfWork.Reppository<Order>().Delete(ExOrder);
				await paymentService.CreateOrUpdatePaymentIntent(BasketId);
			}

			var Order = new Order(BuyerEmail, ShippingAddress,DeliveryMethod,OrderItems,SubTotal,Basket.PaymentIntentId);
			// 6- Add Order Locally
			await unitOfWork.Reppository<Order>().AddAsync(Order);
			// 7- Save Order to DataBase
			return Order;
		}

		public async Task<Order> GetOrderByIdForSpecificUserAsync(string BuyerEmail, int orderId)
		{
			var Spec = new OrderSpecification(BuyerEmail);
			var Order = await unitOfWork.Reppository<Order>().GetEntityWithSpecAsync(Spec);
			return Order;
		}

		public Task<IReadOnlyList<Order>> GetOrderForSpecificUserAsync(string BuyerEmail)
		{
			var Spec = new OrderSpecification(BuyerEmail);
			var Orders = unitOfWork.Reppository<Order>().GetAllWithSpecAsync(Spec);
			return Orders;
		}
	}
}
