using System;
using System.Web;
using ProductsMvcSample.Models;
using System.Web.Mvc;

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
