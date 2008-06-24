using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Collections.Specialized;

namespace Moq.Mvc
{
	/// <summary>
	/// Mock the complete HttpRequestBase object hierarchy
	/// </summary>
	public class HttpRequestMock : Mock<HttpRequestBase>
	{
		NameValueCollection form = new NameValueCollection();
		NameValueCollection headers = new NameValueCollection();

		/// <summary>
		/// Default Constructor
		/// </summary>
		public HttpRequestMock()
		{
			this.ExpectGet(f => f.Form).Returns(form);
			this.ExpectGet(f => f.Headers).Returns(headers);
		}

	}
}
