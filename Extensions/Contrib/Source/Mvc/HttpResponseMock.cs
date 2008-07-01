using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.IO;

namespace Moq.Mvc
{
	/// <summary>
	/// Mock the complete HttpResponseBase object hierarchy
	/// </summary>
	public class HttpResponseMock : Mock<HttpResponseBase>
	{
		/// <summary>
		/// 
		/// </summary>
		public HttpResponseMock()
		{
			this.Output = new Mock<TextWriter>();
			this.OutputStream = new Mock<Stream>();
			this.Cache = new HttpCachePolicyBaseMock();

			ExpectGet(m => m.Output).Returns(this.Output.Object);
			ExpectGet(m => m.OutputStream).Returns(this.OutputStream.Object);
			ExpectGet(m => m.Cache).Returns(this.Cache.Object);
		}

		// TODO: mock other properties.

		/// <summary>
		/// 
		/// </summary>
		public HttpCachePolicyBaseMock Cache { get; private set; }

		/// <summary>
		/// 
		/// </summary>
		public Mock<TextWriter> Output { get; private set; }

		/// <summary>
		/// 
		/// </summary>
		public Mock<Stream> OutputStream { get; private set; }

		/// <summary>
		/// 
		/// </summary>
		public override void Verify()
		{
			this.Cache.Verify();
			this.Output.Verify();
			this.OutputStream.Verify();

			base.Verify();
		}

		/// <summary>
		/// 
		/// </summary>
		public override void VerifyAll()
		{
			this.Cache.VerifyAll();
			this.Output.VerifyAll();
			this.OutputStream.VerifyAll();

			base.VerifyAll();
		}
	}
}
