using System;
using System.Web;
using System.Web.Mvc;

namespace ProductsMvcSample.Controllers
{
	public class HomeController : Controller
	{

		[ControllerAction]
		public void Index()
		{
			RenderView("Welcome");
		}
	}
}
