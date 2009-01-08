using System.Collections.Generic;

namespace ProductsMvcSample.Models
{
	public class ProductsListViewData
	{
		public ProductsListViewData ()
		{
			Products = new List<Product>();
		}

		public int CategoryId { get; set; }
		public string CategoryName { get; set; }
		public List<Product> Products { get; private set; }
	}
}
