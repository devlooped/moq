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
using ProductsMvcSample.Models;
using System.Collections.Generic;

namespace ProductsMvcSample.Services
{
	public interface IProductsCatalogService
	{
		string GetCategoryName(int categoryId);
		IEnumerable<Product> GetProducts(int categoryId);
	}
}
