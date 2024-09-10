using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Talabat.Core.Entities.Order_Agggregate;

namespace Talabat.Core.Specifications.Order_Spec
{
	public class OrderSpecification : BaseSpecification<Order>
	{
        public OrderSpecification(string Email):base(O=>O.BuyerEmail == Email)
        {
            Includes.Add(O => O.DeliveryMethod);
            Includes.Add(O => O.Items);
            AddOrderByDesc(O => O.OrderDate);
        }
        public OrderSpecification(string Email, int OrderId) : base(O => O.BuyerEmail == Email && O.Id == OrderId)
        {
			Includes.Add(O => O.DeliveryMethod);
			Includes.Add(O => O.Items);
		}

    }
}
