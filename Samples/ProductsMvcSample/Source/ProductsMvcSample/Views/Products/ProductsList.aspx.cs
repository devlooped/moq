using System;
using System.Web.Mvc;
using ProductsMvcSample.Models;

namespace ProductsMvcSample.Views.Products
{
	public partial class ProductsList : ViewPage<ProductsListViewData>
	{
		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);
			Title = "Products of " + ViewData.Model.CategoryName;
		}
	}
}
