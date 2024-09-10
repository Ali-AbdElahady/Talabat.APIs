using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Talabat.Core.Entities;
using Talabat.Core.Entities.Order_Agggregate;

namespace Talabat.Core.Services
{
	public interface IPaymentService
	{
		// Func to Create or Update Payment Intent
		Task<CustomerBasket?> CreateOrUpdatePaymentIntent(string BasketId);
		Task<Order> UpdatePaymentIntentToSuccedOrFailed(string PaymentIntentId,bool flag);
	}
}
