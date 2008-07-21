using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Web;
using Xunit;

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

		#region #62

		public interface ISomething<T>
		{
			void DoSomething<U>() where U : T;
		}

		[Fact(Skip = "Still failing on Castle.DynamicProxy2")]
		public void CreatesMockWithGenericsConstraints()
		{
			var mock = new Mock<ISomething<object>>();
		}

		#endregion

		#region #60

		public interface IFoo
		{
			void DoThings(object arg);
		}

		[Fact]
		public void TwoExpectations()
		{
			Mock<IFoo> mocked = new Mock<IFoo>(MockBehavior.Strict);
			object arg1 = new object();
			object arg2 = new object();

			mocked.Expect(m => m.DoThings(arg1));
			mocked.Expect(m => m.DoThings(arg2));

			mocked.Object.DoThings(arg1);
			mocked.Object.DoThings(arg2);

			mocked.VerifyAll();
		}

		#endregion

		#region #21

		[Fact]
		public void MatchesLatestExpectations()
		{
			var mock = new Mock<IEvaluateLatest>();

			mock.Expect(m => m.Method(It.IsAny<int>())).Returns(0);
			mock.Expect(m => m.Method(It.IsInRange<int>(0, 20, Range.Inclusive))).Returns(1);

			mock.Expect(m => m.Method(5)).Returns(2);
			mock.Expect(m => m.Method(10)).Returns(3);

			Assert.Equal(3, mock.Object.Method(10));
			Assert.Equal(2, mock.Object.Method(5));
			Assert.Equal(1, mock.Object.Method(6));
			Assert.Equal(0, mock.Object.Method(25));
		}

		public interface IEvaluateLatest
		{
			int Method(int value);
		}

		#endregion

		#region #49

		[Fact]
		public void UsesCustomMatchersWithGenerics()
		{
			var mock = new Mock<IEvaluateLatest>();

			mock.Expect(e => e.Method(IsEqual.To(5))).Returns(1);
			mock.Expect(e => e.Method(IsEqual.To<int, string>(6, "foo"))).Returns(2);

			Assert.Equal(1, mock.Object.Method(5));
			Assert.Equal(2, mock.Object.Method(6));
		}

		static class IsEqual
		{
			[Matcher]
			public static T To<T>(T value)
			{
				return value;
			}

			public static bool To<T>(T left, T right)
			{
				return left.Equals(right);
			}

			[Matcher]
			public static T To<T, U>(T value, U value2)
			{
				return value;
			}

			public static bool To<T, U>(T left, T right, U value)
			{
				return left.Equals(right);
			}
		}

		#endregion

		#region #68

		[Fact]
		public void GetMockCastedToObjectThrows()
		{
			var mock = new Mock<IAsyncResult>();
			object m = mock.Object;

			Assert.Throws<ArgumentException>(() => Mock.Get(m));
		}

		#endregion

		#region #69

		public interface IFooPtr
		{
			IntPtr Get(string input);
		}

		[Fact(Skip = "Looks like a bug in DP2. See #69.")]
		public void ReturnsIntPtr()
		{
			Mock<IFooPtr> mock = new Mock<IFooPtr>(MockBehavior.Strict);
			IntPtr ret = new IntPtr(3);

			mock.Expect(m => m.Get("a")).Returns(ret);

			IntPtr ret3 = mock.Object.Get("a");

			Assert.Equal(ret, mock.Object.Get("a"));
		}


		#endregion

		// run "netsh http add urlacl url=http://+:7777/ user=[domain]\[user]"
		// to avoid running the test as an admin
		[Fact]
		public void ProxiesAndHostsWCF()
		{
#if DEBUG
			// On release mode, castle is ILMerged into Moq.dll and this won't compile
			var generator = new Castle.DynamicProxy.ProxyGenerator();
			var proxy = generator.CreateClassProxy<ServiceImplementation>();
			using (var host = new WebServiceHost(proxy, new Uri("http://localhost:7777")))
			{
				host.Open();
			}
#endif
		}

		// run "netsh http add urlacl url=http://+:7777/ user=[domain]\[user]"
		// to avoid running the test as an admin
		[Fact]
		public void ProxiesAndHostsWCFMock()
		{
			//var generator = new Castle.DynamicProxy.ProxyGenerator();
			var proxy = new Mock<ServiceImplementation>();
			using (var host = new WebServiceHost(proxy.Object, new Uri("http://localhost:7777")))
			{
				host.Open();
			}
		}

		[ServiceContract]
		public interface IServiceContract
		{
			[OperationContract]
			void Do();
		}

		[ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
		public class ServiceImplementation : IServiceContract
		{
			public void Do()
			{
				throw new NotImplementedException();
			}
		}
	}
}
