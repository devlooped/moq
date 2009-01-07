using System;
using System.Web;
using ProductsMvcSample.Services;
using ProductsMvcSample.Models;
using System.Web.Mvc;

namespace ProductsMvcSample.Controllers
{
	public class ProductsController : Controller
	{
		IProductsCatalogService catalogService;

		public ProductsController()
			: this(new ProductsCatalogService())
		{

		}

		public ProductsController(IProductsCatalogService catalogService)
		{
			this.catalogService = catalogService;
		}

		public void Index()
		{
		}

		public void Category(int id)
		{
			var model = new ProductsListViewData();
			model.CategoryId = id;
			model.CategoryName = catalogService.GetCategoryName(id);
			model.Products.AddRange(catalogService.GetProducts(id) ?? new Product[0]);
			View("ProductsList", model);
		}
	}
}
