using System.Web.Mvc;

namespace ProductsMvcSample.Controllers
{
	public class HomeController : Controller
	{
		public ViewResult Index()
		{
			return View("Welcome");
		}
	}
}
