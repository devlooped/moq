using System;
using System.Data;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;
using System.Collections.Generic;

namespace ProductsMvcSample.Models
{
	public class ProductsListViewData
	{
		public ProductsListViewData ()
		{
			Products = new List<Product>();
		}

		public int CategoryId { get; set; }
		public string CategoryName { get; set; }
		public List<Product> Products { get; private set; }
	}
}
