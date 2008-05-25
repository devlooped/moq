using System.Diagnostics;
using Xunit;
using System.Collections.Generic;
using System;
using System.Linq;

namespace Moq.Tests.Regressions
{
	public class IssueReportsFixture
	{
		#region #47

		[Fact]
		public void ShouldReturnListFromDateTimeArg()
		{
			var items = new List<string>() { "Foo", "Bar" };

			var mock = new Mock<IMyClass>(MockBehavior.Strict);
			mock
				.Expect(m => m.GetValuesSince(It.IsAny<DateTime>()))
				.Returns(items);

			var actual = mock.Object.GetValuesSince(DateTime.Now).ToList();

			Assert.Equal(items.Count, actual.Count);
		}

		public interface IMyClass
		{
			IEnumerable<string> GetValuesSince(DateTime since);
		}

		#endregion

		#region #52

		[Fact]
		public void ShouldNotOverridePreviousExpectation()
		{
			var ids = Enumerable.Range(1, 10);
			var mock = new Mock<IOverwritingMethod>(MockBehavior.Strict);

			foreach (var id in ids)
			{
				mock.Expect(x => x.DoSomething(id));
			}

			var component = mock.Object;

			foreach (var id in ids)
			{
				component.DoSomething(id);
			}
		}

		public interface IOverwritingMethod
		{
			void DoSomething(int id);
		}

		#endregion
	}
}
