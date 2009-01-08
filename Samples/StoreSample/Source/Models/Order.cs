using System;

namespace Store
{
	public class Order
	{
		public Product Product { get; set; }
		public int Quantity { get; set; }
		public bool Filled { get; set; }
	}
}
