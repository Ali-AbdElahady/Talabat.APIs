using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Talabat.Core.Entities.Order_Agggregate
{
	public class Order : BaseEntity
	{
		private readonly string paymentIntentId;

		public Order()
		{
		}
		public Order(string buyerEmail, Address shippingAddress, DeliveryMethod deliveryMethod, ICollection<OrderItem> items, decimal subTotal, string paymentIntentId)
		{
			BuyerEmail = buyerEmail;
			ShippingAddress = shippingAddress;
			DeliveryMethod = deliveryMethod;
			Items = items;
			SubTotal = subTotal;
			this.paymentIntentId = paymentIntentId;
		}


		public string BuyerEmail { get; set; }
        public DateTimeOffset OrderDate { get; set; } = DateTimeOffset.Now;
        public OrderStatus Status { get; set; } = OrderStatus.Pending;
        // take object from address
        public Address ShippingAddress { get; set; }
		// relationship between tables
		// if i put navigation prop here and not in the DeliveryMethod
		// entityframework will consider this relation is 1-1 and create the DeliveryMethodId
        // but does matter we will work with it as 1-M
		public DeliveryMethod DeliveryMethod { get; set; }
        // because this relation is many you must intialize it 
        public ICollection<OrderItem> Items { get; set; } = new List<OrderItem>();
        public decimal SubTotal { get; set; }
        // this form will not represent in database
        public decimal GetTotal() => SubTotal * DeliveryMethod.Cost;
		// put instial value for PaymentIntentId until get into it
		public string PaymentIntentId { get; set; }
    }
}
