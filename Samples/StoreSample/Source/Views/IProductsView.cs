using System;
using System.Collections.Generic;

namespace Store
{
	public interface IProductsView
	{
		event EventHandler<CategoryEventArgs> CategorySelected;
		void SetCategories(IEnumerable<Category> categories);
		void SetProducts(IEnumerable<Product> products);
	}
}
