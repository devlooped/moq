using System;
using System.Web.Mvc;
using System.Web.Routing;

namespace ProductsMvcSample
{
	public class Global : System.Web.HttpApplication
	{

		protected void Application_Start(object sender, EventArgs e)
		{
			// Note: Change Url= to Url="[controller].mvc/[action]/[id]" to enable 
			//       automatic support on IIS6 
			RegisterRoutes(RouteTable.Routes);
		}

		public static void RegisterRoutes(RouteCollection routes)
		{
			routes.MapRoute(
				"Default",                                              // Route name
				"{controller}/{action}/{id}",                           // URL with parameters
				new { controller = "Home", action = "Index", id = "" }, // Parameter defaults
				new { controller = @"[^\.]*" }							// Parameter constraints
			);

			routes.MapRoute(
				"Root",													// Route name
				"Default.aspx",											// URL with parameters
				new { controller = "Home", action = "Index", id = "" }	// Parameter defaults
			);
		}
	}
}