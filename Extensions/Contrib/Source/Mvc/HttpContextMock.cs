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
		Mock<HttpApplicationStateBase> httpApplicationState = new HttpApplicationStateMock();
		Mock<HttpRequestBase> httpRequest = new HttpRequestMock();
		Mock<HttpResponseBase> httpResponse = new HttpResponseMock();
		Mock<HttpServerUtilityBase> httpServerUtility = new HttpServerUtilityMock();
		Mock<HttpSessionStateBase> httpSessionState = new HttpSessionStateMock();

		/// <summary>
		/// Default Constructor
		/// </summary>
		public HttpContextMock()
		{
			this.ExpectGet(c => c.Application).Returns(httpApplicationState.Object);
			this.ExpectGet(c => c.Request).Returns(httpRequest.Object);
			this.ExpectGet(c => c.Response).Returns(httpResponse.Object);
			this.ExpectGet(c => c.Server).Returns(httpServerUtility.Object);
			this.ExpectGet(c => c.Session).Returns(httpSessionState.Object);
		}

		/// <summary>
		/// Verify only the mock expectations marked as Verifiable
		/// </summary>
		public override void Verify()
		{
			httpApplicationState.Verify();
			httpRequest.Verify();
			httpResponse.Verify();
			httpServerUtility.Verify();
			httpSessionState.Verify();

			base.Verify();
		}

		/// <summary>
		/// Very all the mock expectations
		/// </summary>
		public override void VerifyAll()
		{
			httpApplicationState.VerifyAll();
			httpRequest.VerifyAll();
			httpResponse.VerifyAll();
			httpServerUtility.VerifyAll();
			httpSessionState.VerifyAll();

			base.VerifyAll();
		}
	}
}
