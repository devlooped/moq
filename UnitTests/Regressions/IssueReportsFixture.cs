using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
#if !SILVERLIGHT
using System.ServiceModel.Web;
#endif
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
				.Setup(m => m.GetValuesSince(It.IsAny<DateTime>()))
				.Returns(items);

			var actual = mock.Object.GetValuesSince(DateTime.Now).ToList();

			Assert.Equal(items.Count, actual.Count);
		}

		public interface IMyClass
		{
			IEnumerable<string> GetValuesSince(DateTime since);
		}

		#endregion

		#region 48

		public class Issue48
		{
			[Fact]
			public void ExpectsOnIndexer()
			{
				var mock = new Mock<ISomeInterface>();
				mock.Setup(m => m[0]).Returns("a");
				mock.Setup(m => m[1]).Returns("b");

				Assert.Equal("a", mock.Object[0]);
				Assert.Equal("b", mock.Object[1]);
				Assert.Equal(default(string), mock.Object[2]);
			}

			public interface ISomeInterface
			{
				string this[int index] { get; set; }
			}
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
				mock.Setup(x => x.DoSomething(id));
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

		[Fact]
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

			mocked.Setup(m => m.DoThings(arg1));
			mocked.Setup(m => m.DoThings(arg2));

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

			mock.Setup(m => m.Method(It.IsAny<int>())).Returns(0);
			mock.Setup(m => m.Method(It.IsInRange<int>(0, 20, Range.Inclusive))).Returns(1);

			mock.Setup(m => m.Method(5)).Returns(2);
			mock.Setup(m => m.Method(10)).Returns(3);

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

			mock.Setup(e => e.Method(IsEqual.To(5))).Returns(1);
			mock.Setup(e => e.Method(IsEqual.To<int, string>(6, "foo"))).Returns(2);

			Assert.Equal(1, mock.Object.Method(5));
			Assert.Equal(2, mock.Object.Method(6));
		}

		public static class IsEqual
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

		[Fact]
		public void ReturnsIntPtr()
		{
			Mock<IFooPtr> mock = new Mock<IFooPtr>(MockBehavior.Strict);
			IntPtr ret = new IntPtr(3);

			mock.Setup(m => m.Get("a")).Returns(ret);

			IntPtr ret3 = mock.Object.Get("a");

			Assert.Equal(ret, mock.Object.Get("a"));
		}


		#endregion

		#region #85

		public class Issue85
		{
			[Fact]
			public void FooTest()
			{
				// Setup
				var fooMock = new Mock<Foo>();
				fooMock.CallBase = true;
				fooMock.Setup(o => o.GetBar()).Returns(new Bar());
				var bar = ((IFoolery)fooMock.Object).DoStuffToBar();
				Assert.NotNull(bar);
			}

			public interface IFoolery
			{
				Bar DoStuffToBar();
			}

			public class Foo : IFoolery
			{
				public virtual Bar GetBar()
				{
					return new Bar();
				}

				Bar IFoolery.DoStuffToBar()
				{
					return DoWeirdStuffToBar();
				}

				protected internal virtual Bar DoWeirdStuffToBar()
				{
					var bar = GetBar();
					//Would do stuff here.
					return bar;
				}
			}

			public class Bar
			{
			}
		}

		#endregion

		#region #89

		public class Issue89
		{
			[Fact]
			public void That_last_expectation_should_win()
			{
				var mock = new Mock<ISample>();
				mock.Setup(s => s.Get(1)).Returns("blah");
				mock.Setup(s => s.Get(It.IsAny<int>())).Returns("foo");
				mock.Setup(s => s.Get(1)).Returns("bar");
				Assert.Equal("bar", mock.Object.Get(1));
			}

			public interface ISample
			{
				string Get(int i);
			}
		}

		#endregion

		#region #134

		public class Issue134
		{
			[Fact]
			public void Test()
			{
				Mock<IFoo> target = new Mock<IFoo>();
				target.Setup(t => t.Submit(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>()));

				MockException e = Assert.Throws<MockVerificationException>(() => target.VerifyAll());

				Assert.Contains(
					"IFoo t => t.Submit(IsAny<String>(), IsAny<String>(), new [] {IsAny<Int32>()})",
					e.Message);
			}

			public interface IFoo
			{
				void Submit(string mailServer, string from, params int[] toRecipient);
			}
		}

		#endregion

		#region #136

		public class _136
		{
			public interface IIndexedFoo
			{
				string this[int key] { get; set; }
			}

			[Fact(Skip="Fix for 3.5")]
			public void ShouldThrowIfSetupSetIndexer()
			{
				var foo = new Mock<IIndexedFoo>();

				Assert.Throws<ArgumentException>(() => foo.SetupSet(f => f[0]));
			}

			[Fact(Skip = "Fix for 3.5")]
			public void ShouldSetIndexer()
			{
				var foo = new Mock<IIndexedFoo>(MockBehavior.Strict);

				foo.SetupSet(f => f[0] = "foo");

				foo.Object[0] = "foo";
			}
		}

		#endregion

		#region #138

		public class _138
		{
			public interface SuperFoo
			{
				string Bar { get; set; }
			}
			public interface Foo : SuperFoo
			{
				string Baz { get; set; }
			}

			[Fact]
			public void superFooMockSetupAllProperties()
			{
				var fac = new MockFactory(MockBehavior.Default);
				var superFooMock = fac.Create<SuperFoo>();
				superFooMock.SetupAllProperties();

				var superFoo = superFooMock.Object;
				superFoo.Bar = "Bar";
				Assert.Equal("Bar", superFoo.Bar);
			}
		}

		#endregion

		#region #145

		public class _145
		{
			public interface IResolver
			{
				string Resolve<T>();
			}

			public class DataWriter<T>
			{
			}

			public class DataA { }
			public class DataB { }

			[Fact]
			public void ShouldDifferentiateBetweenGenericsParams()
			{
				var mock = new Mock<IResolver>();
				mock.Setup(m => m.Resolve<DataWriter<DataA>>()).Returns("Success A");

				Assert.Equal("Success A", mock.Object.Resolve<DataWriter<DataA>>());

				mock.Setup(m => m.Resolve<DataWriter<DataB>>()).Returns("Success B");

				Assert.Equal("Success B", mock.Object.Resolve<DataWriter<DataB>>());
				Assert.Equal("Success A", mock.Object.Resolve<DataWriter<DataA>>());
			}

		}

		#endregion

#if !SILVERLIGHT
		// run "netsh http add urlacl url=http://+:7777/ user=[domain]\[user]"
		// to avoid running the test as an admin
		[Fact(Skip = "Doesn't work in Mono")]
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
		[Fact(Skip = "Doesn't work in Mono")]
		public void ProxiesAndHostsWCFMock()
		{
			//var generator = new Castle.DynamicProxy.ProxyGenerator();
			var proxy = new Mock<ServiceImplementation>();
			using (var host = new WebServiceHost(proxy.Object, new Uri("http://localhost:7777")))
			{
				host.Open();
			}
		}

		[ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
		public class ServiceImplementation : IServiceContract
		{
			public void Do()
			{
				throw new NotImplementedException();
			}
		}
#endif

		[ServiceContract]
		public interface IServiceContract
		{
			[OperationContract]
			void Do();
		}

	}
}
