using System;
using System.Collections.Generic;

namespace Store
{
	public interface ICatalogService
	{
		IEnumerable<Category> GetCategories();
		IEnumerable<Product> GetProducts(int categoryId);
		bool HasInventory(int productId, int quantity);
		void Remove(int productId, int quantity);
	}
}
