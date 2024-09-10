using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Talabat.Core.Entities.Order_Agggregate
{
	public enum OrderStatus
	{
		// the default behavior is to store enum in database by value for (0,1,2)
		// to solve this issue add atttribute for each prop [EnumMember(Value = "Pending")]
		[EnumMember(Value = "Pending")]
		Pending,
		[EnumMember(Value = "Payment Received")]
		PaymentReceived,
		[EnumMember(Value = "Payment Failed")]
		PaymentFailed,
	}
}
