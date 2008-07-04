using System;
using System.Data;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;
using System.Collections.Generic;
using ProductsMvcSample.Models;

namespace ProductsMvcSample.Services
{
	public class ProductsCatalogService : IProductsCatalogService
	{		
		#region IProductsCatalogService Members

		public string GetCategoryName(int categoryId)
		{
			if (categoryId == 1)
				return "Beverage";
			if (categoryId == 2)
				return "Condiments";
			return null;
		}

		public IEnumerable<Product> GetProducts(int categoryId)
		{
			if (categoryId == 1)
			{
				yield return new Product { Id = 1, Name = "Merlot" };
				yield return new Product { Id = 2, Name = "Spirits" };
				yield return new Product { Id = 3, Name = "Beer" };
				yield return new Product { Id = 4, Name = "Cabernet" };
				yield return new Product { Id = 5, Name = "Chardonnay" };
			}
			if (categoryId == 2)
			{
				yield return new Product { Id = 10, Name = "Aniseed Syrup" };
				yield return new Product { Id = 11, Name = "Chef Anton's Cajun Seasoning" };
				yield return new Product { Id = 12, Name = "Chef Anton's Gumbo Mix" };
				yield return new Product { Id = 13, Name = "Gula Malacca" };
				yield return new Product { Id = 14, Name = "Vegie-spread" };
			}
			yield break;
		}

		#endregion
	}
}
