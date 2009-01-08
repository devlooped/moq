using System.Collections.Generic;
using ProductsMvcSample.Models;

namespace ProductsMvcSample.Services
{
	public interface IProductsCatalogService
	{
		string GetCategoryName(int categoryId);
		IEnumerable<Product> GetProducts(int categoryId);
	}
}
