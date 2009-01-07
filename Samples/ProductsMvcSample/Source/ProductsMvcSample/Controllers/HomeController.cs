using System;
using System.Web;
using System.Web.Mvc;

namespace ProductsMvcSample.Controllers
{
	public class HomeController : Controller
	{
		public void Index()
		{
			View("Welcome");
		}
	}
}
