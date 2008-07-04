using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using System.Web.SessionState;
using System.Xml.Linq;

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
			routes.Add(new Route
			{
				Url = "[controller]/[action]/[id]",
				Defaults = new { action = "Index", id = (string)null },
				RouteHandler = typeof(MvcRouteHandler)
			});

			routes.Add(new Route
			{
				Url = "Default.aspx",
				Defaults = new { controller = "Home", action = "Index", id = (string)null },
				RouteHandler = typeof(MvcRouteHandler)
			});
		}
	}
}