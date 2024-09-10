using Microsoft.Extensions.Configuration;
using Stripe;
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
using Product = Talabat.Core.Entities.Product;

namespace Talabat.Service
{
	public class PaymentService : IPaymentService
	{
		private readonly IConfiguration configuartion;
		private readonly IBasketRepository basketRepo;
		private readonly IUnitOfWork unitOfWork;

		public PaymentService(IConfiguration configuartion, IBasketRepository basketRepo,IUnitOfWork unitOfWork)
        {
			this.configuartion = configuartion;
			this.basketRepo = basketRepo;
			this.unitOfWork = unitOfWork;
		}
        public async Task<CustomerBasket?> CreateOrUpdatePaymentIntent(string BasketId)
		{
			StripeConfiguration.ApiKey = configuartion["StripeKeys:Secretkey"];
			// Get Basket
			var Basket = await basketRepo.GetBasketAsync(BasketId);
			if (Basket == null) return null;
			var ShippingPrice = 0M;
			if (Basket.DeliveryMethodId.HasValue)
            {
                var DeliveryMethod = await unitOfWork.Reppository<DeliveryMethod>().GetByIdAsync(Basket.DeliveryMethodId.Value);
				 ShippingPrice = DeliveryMethod.Cost;
            }
			if(Basket.Items.Count > 0)
			{
				foreach (var item in Basket.Items)
				{
					var Product = await unitOfWork.Reppository<Product>().GetByIdAsync(item.Id);
                    if (item.Price != Product.Price)
                    {
						item.Price = Product.Price;
                    }
                }
			}
			var SubTotal = Basket.Items.Sum(item=>item.Price*item.Quantity);
			// Create Payment Intent
			var Service = new PaymentIntentService();
			PaymentIntent paymentIntent;
			if(string.IsNullOrEmpty(Basket.PaymentIntentId)) // Create
			{
				var Options = new PaymentIntentCreateOptions()
				{
					Amount = (long)(SubTotal * 100 + ShippingPrice * 100),
					Currency = "usd",
					PaymentMethodTypes = new List<string>() { "card" }
				};
				paymentIntent = await Service.CreateAsync(Options);
				Basket.PaymentIntentId = paymentIntent.Id;
				Basket.ClientSecret = paymentIntent.ClientSecret;
			}
			else // Update
			{
				var Options = new PaymentIntentUpdateOptions()
				{
					Amount = (long)(SubTotal * 100 + ShippingPrice * 100)
				};
				paymentIntent = await Service.UpdateAsync(Basket.PaymentIntentId, Options);
				Basket.PaymentIntentId = paymentIntent.Id;
				Basket.ClientSecret = paymentIntent.ClientSecret;
			}
			await basketRepo.UpdateBasketAsync(Basket);
			return Basket;
        }

		public async Task<Order> UpdatePaymentIntentToSuccedOrFailed(string PaymentIntentId, bool flag)
		{
			var Spec = new OrderWithPaymentIntentSpec(PaymentIntentId);
			var Order = await unitOfWork.Reppository<Order>().GetEntityWithSpecAsync(Spec);
			if(flag)
			{
				Order.Status = OrderStatus.PaymentReceived;
			}
			else
			{
				Order.Status = OrderStatus.PaymentFailed;
			}
			unitOfWork.Reppository<Order>().update(Order);
			await unitOfWork.CompleteAsync();
			return Order;
		}
	}
}
