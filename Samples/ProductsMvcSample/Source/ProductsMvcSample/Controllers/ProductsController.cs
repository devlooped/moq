using System;
using System.Web;
using System.Web.Mvc;
using ProductsMvcSample.Services;
using ProductsMvcSample.Models;

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

		[ControllerAction]
		public void Index()
		{
		}

		[ControllerAction]
		public void Category(int id)
		{
			var model = new ProductsListViewData();
			model.CategoryId = id;
			model.CategoryName = catalogService.GetCategoryName(id);
			model.Products.AddRange(catalogService.GetProducts(id) ?? new Product[0]);
			RenderView("ProductsList", model);
		}
	}
}
