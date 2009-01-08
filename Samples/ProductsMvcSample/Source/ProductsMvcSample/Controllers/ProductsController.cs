using System.Web.Mvc;
using ProductsMvcSample.Models;
using ProductsMvcSample.Services;

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

		public ActionResult Index()
		{
			return View();
		}

		public ActionResult Category(int id)
		{
			var model = new ProductsListViewData();
			model.CategoryId = id;
			model.CategoryName = catalogService.GetCategoryName(id);
			model.Products.AddRange(catalogService.GetProducts(id) ?? new Product[0]);
			return View("ProductsList", model);
		}
	}
}
