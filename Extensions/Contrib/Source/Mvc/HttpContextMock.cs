using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace Moq.Mvc
{
	/// <summary>
	/// Complete object model for mocking the MVC Http context
	/// </summary>
	public class HttpContextMock : Mock<HttpContextBase>
	{
		/// <summary>
		/// Default Constructor
		/// </summary>
		public HttpContextMock()
		{
			this.HttpApplicationState = new HttpApplicationStateMock();
			this.HttpRequest = new HttpRequestMock();
			this.HttpResponse = new HttpResponseMock();
			this.HttpServerUtility = new HttpServerUtilityMock();
			this.HttpSessionState = new HttpSessionStateMock();

			this.ExpectGet(c => c.Application).Returns(this.HttpApplicationState.Object);
			this.ExpectGet(c => c.Request).Returns(this.HttpRequest.Object);
			this.ExpectGet(c => c.Response).Returns(this.HttpResponse.Object);
			this.ExpectGet(c => c.Server).Returns(this.HttpServerUtility.Object);
			this.ExpectGet(c => c.Session).Returns(this.HttpSessionState.Object);
		}

		/// <summary>
		/// 
		/// </summary>
		public HttpApplicationStateMock HttpApplicationState { get; private set; }

		/// <summary>
		/// 
		/// </summary>
		public HttpRequestMock HttpRequest { get; private set; }

		/// <summary>
		/// 
		/// </summary>
		public HttpResponseMock HttpResponse { get; private set; }

		/// <summary>
		/// 
		/// </summary>
		public HttpServerUtilityMock HttpServerUtility { get; private set; }

		/// <summary>
		/// 
		/// </summary>
		public HttpSessionStateMock HttpSessionState { get; private set; }
		
		/// <summary>
		/// Verify only the mock expectations marked as Verifiable
		/// </summary>
		public override void Verify()
		{
			this.HttpApplicationState.Verify();
			this.HttpRequest.Verify();
			this.HttpResponse.Verify();
			this.HttpServerUtility.Verify();
			this.HttpSessionState.Verify();

			base.Verify();
		}

		/// <summary>
		/// Very all the mock expectations
		/// </summary>
		public override void VerifyAll()
		{
			this.HttpApplicationState.VerifyAll();
			this.HttpRequest.VerifyAll();
			this.HttpResponse.VerifyAll();
			this.HttpServerUtility.VerifyAll();
			this.HttpSessionState.VerifyAll();

			base.VerifyAll();
		}
	}
}
