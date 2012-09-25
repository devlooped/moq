using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using Moq;
using Moq.Properties;
using Moq.Protected;
using Xunit;

#if !SILVERLIGHT
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Web.UI.HtmlControls;
using System.Threading;
#endif

#region #181

// NOTE class without namespace
public class _181
{
	[Fact]
	public void ReproTest()
	{
		var mock = new Mock<IDisposable>();
		mock.Object.Dispose();

		mock.Verify(d => d.Dispose());
	}
}

#endregion

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

		#region #48

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
#pragma warning disable 618
			[Matcher]
			public static T To<T>(T value)
			{
				return value;
			}
#pragma warning restore 618

			public static bool To<T>(T left, T right)
			{
				return left.Equals(right);
			}

#pragma warning disable 618
			[Matcher]
			public static T To<T, U>(T value, U value2)
			{
				return value;
			}
#pragma warning restore 618

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
				var target = new Mock<IFoo>();
				target.Setup(t => t.Submit(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>()));

				var e = Assert.Throws<MockVerificationException>(() => target.VerifyAll());

				Assert.Contains(
					"IFoo t => t.Submit(It.IsAny<String>(), It.IsAny<String>(), new[] { It.IsAny<Int32>() })",
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
			// Fixed on PropertiesFixture.cs
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
				var repo = new MockRepository(MockBehavior.Default);
				var superFooMock = repo.Create<SuperFoo>();
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

		#region #111 & #155

		public class _111
		{
			[Fact]
			public void TestTypedParamsWithNoParams()
			{
				var mock = new Mock<IParams>(MockBehavior.Strict);
				mock.Setup(p => p.Submit(It.IsAny<string>(), It.IsAny<int[]>()));

				mock.Object.Submit("foo");

				mock.VerifyAll();
			}

			[Fact]
			public void TestTypedParams()
			{
				var mock = new Mock<IParams>(MockBehavior.Strict);
				mock.Setup(p => p.Submit(It.IsAny<string>(), It.IsAny<int[]>()));

				mock.Object.Submit("foo", 0, 1, 2);

				mock.VerifyAll();
			}

			[Fact]
			public void TestObjectParamsWithoutParams()
			{
				var mock = new Mock<IParams>(MockBehavior.Strict);
				mock.Setup(p => p.Execute(It.IsAny<int>(), It.IsAny<object[]>()));

				mock.Object.Execute(1);

				mock.VerifyAll();
			}

			[Fact]
			public void TestObjectParams()
			{
				var mock = new Mock<IParams>(MockBehavior.Strict);
				mock.Setup(p => p.Execute(It.IsAny<int>(), It.IsAny<object[]>()));

				mock.Object.Execute(1, "0", "1", "2");

				mock.VerifyAll();
			}

			[Fact]
			public void TestObjectParamsWithExpectedValues()
			{
				var mock = new Mock<IParams>(MockBehavior.Strict);
				mock.Setup(p => p.Execute(5, "foo", "bar"));

				Assert.Throws<MockException>(() => mock.Object.Execute(5, "bar", "foo"));

				mock.Object.Execute(5, "foo", "bar");

				mock.Verify(p => p.Execute(5, "foo", "bar"));
			}

			[Fact]
			public void TestObjectParamsWithArray()
			{
				var mock = new Mock<IParams>();
				mock.Setup(p => p.Execute(It.IsAny<int>(), It.IsAny<string[]>(), It.IsAny<int>()));

				mock.Object.Execute(1, new string[] { "0", "1" }, 3);

				mock.Verify(p => p.Execute(It.IsAny<int>(), It.IsAny<object[]>()));
				mock.Verify(p => p.Execute(It.IsAny<int>(), It.IsAny<string[]>(), It.IsAny<int>()));
				mock.Verify(p => p.Execute(It.IsAny<int>(), It.IsAny<object[]>(), It.IsAny<int>()));
			}

			[Fact]
			public void TestTypedParamsInEachArgument()
			{
				var mock = new Mock<IParams>(MockBehavior.Strict);
				mock.Setup(p => p.Submit(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>()));

				mock.Object.Submit("foo", 0, 1);

				mock.Verify(p => p.Submit(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>()));
				mock.Verify(p => p.Submit(It.IsAny<string>(), It.Is<int[]>(a => a.Length == 2)));
				mock.VerifyAll();
			}

			[Fact]
			public void TestParamsWithReturnValue()
			{
				var mock = new Mock<IParams>();
				mock.Setup(x => x.GetValue("Matt")).Returns("SomeString");

				var ret = mock.Object.GetValue("Matt");
				Assert.Equal("SomeString", ret);
			}

			public interface IParams
			{
				void Submit(string name, params int[] values);
				void Execute(int value, params object[] values);
				string GetValue(string name, params object[] args);
			}
		}

		#endregion

		#region #159

		public class _159
		{
			[Fact]
			public void ImplicitIntToLong()
			{
				int id = 1;
				var mock = new Mock<IFoo>();
				mock.Object.SetIt(id);
				mock.Verify(x => x.SetIt(id));
			}

			[Fact]
			public void ImplicitInterface()
			{
				var barMock = new Mock<IBar>();
				var baz = new Baz(barMock.Object);
				baz.DoBarFoo(new Foo());
				barMock.Verify(x => x.DoFoo(It.IsAny<Foo>()));
			}

			public interface IFoo
			{
				long Id { get; set; }
				void SetIt(long it);
			}

			public class Foo : IFoo
			{
				public long Id { get; set; }
				public void SetIt(long it) { }
			}

			public interface IBar
			{
				void DoFoo(IFoo foo);
			}

			public class Baz
			{
				private readonly IBar _bar;
				public Baz(IBar bar)
				{
					_bar = bar;
				}

				public void DoBarFoo(IFoo foo)
				{
					_bar.DoFoo(foo);
				}
			}
		}

		#endregion

		#region #152

		public class _152
		{
			public enum MembershipCreateStatus { Created, Duplicated, Invalid }
			public interface IMembershipService
			{
				int MinPasswordLength { get; }
				bool ValidateUser(string userName, string password);
				MembershipCreateStatus CreateUser(string userName, string password, string email);
				bool ChangePassword(string userName, string oldPassword, string newPassword);
			}

			[Fact]
			public void ShouldReturnEnum()
			{
				var provider = new Mock<IMembershipService>();

				// For some reason, this particular lambda doesn't let me specify
				// a method return value for the method even though it returns a 
				// MembershipCreateStatus enum
				provider.Setup(p => p.CreateUser(string.Empty, string.Empty, string.Empty)).Returns(MembershipCreateStatus.Invalid);

				Assert.Equal(MembershipCreateStatus.Invalid, provider.Object.CreateUser("", "", ""));
			}
		}

		#endregion

		#region #153

		public class _153
		{
			public struct SomeClass<T> // Struct just to avoid having to implement Equals/GetHashCode
			{
				public static implicit operator SomeClass<T>(T t)
				{
					return new SomeClass<T>();
				}

				public static SomeClass<T> From(T t)
				{
					return t;
				}
			}

			public interface IIfc
			{
				int Get(SomeClass<string> id);
			}

			public class ImplicitConversionProblem
			{
				[Fact]
				public void ImplicitSetupVerifyAll_Fails()
				{
					const string s = "XYZ";
					var mock = new Mock<IIfc>();
					mock.Setup(ifc => ifc.Get(s)).Returns(17);

					var result = mock.Object.Get(s);

					mock.VerifyAll(); // MockVerificationException here
					Assert.Equal(17, result);
				}

				[Fact]
				public void ExplicitSetupVerifyAll_Works()
				{
					const string s = "XYZ";
					var mock = new Mock<IIfc>();
					mock.Setup(ifc => ifc.Get(SomeClass<string>.From(s))).Returns(17);

					var result = mock.Object.Get(s);

					mock.VerifyAll();
					Assert.Equal(17, result);
				}

				[Fact]
				public void ExplicitSetupImplicitVerification_Fails()
				{
					const string s = "XYZ";
					var mock = new Mock<IIfc>();
					mock.Setup(ifc => ifc.Get(SomeClass<string>.From(s))).Returns(17);

					var result = mock.Object.Get(s);

					// Here the problem can be seen even in the exception message:
					// Invocation was not performed on the mock: ifc => ifc.Get("XYZ")
					// -----------------------------------------------------------^
					mock.Verify(ifc => ifc.Get(s));
					Assert.Equal(17, result);
				}

				[Fact]
				public void ImplicitSetupExplicitVerification_Fails()
				{
					const string s = "XYZ";
					var mock = new Mock<IIfc>();
					mock.Setup(ifc => ifc.Get(s)).Returns(17);

					var result = mock.Object.Get(s);

					// This verification passes oddly enough
					mock.Verify(ifc => ifc.Get(SomeClass<string>.From(s)));

					// This assert fails, indicating that the setup was not used
					Assert.Equal(17, result);
				}
			}
		}

		#endregion

		#region #146

		public class _146
		{
			public interface IFoo
			{
				bool Property { get; set; }
				string StringProperty { get; set; }
			}

			[Fact]
			public void StrictMockPropertySet()
			{
				var mock = new Mock<IFoo>(MockBehavior.Strict);

				mock.SetupSet(v => v.Property = false);

				Assert.Throws<MockException>(() => mock.VerifySet(v => v.Property = false));

				mock.Object.Property = false;

				mock.VerifySet(v => v.Property = false);
			}
		}

		#endregion

		#region #158

		public class _158
		{
			public class Foo
			{
				public virtual void Boo()
				{
					Bar();
					Bar();
				}

				protected virtual void Bar()
				{
				}
			}

#pragma warning disable 618
			[Fact(Skip = "This setup doesn't make sense, and xUnit does not provide this message checking capability.")]
			public void ShouldRenderCustomMessage()
			{
				var foo = new Mock<Foo> { CallBase = true };
				foo.Protected().Setup("Bar").AtMostOnce().Verifiable("Hello");
				foo.Object.Boo();
				//Assert.Throws<MockException>("Hello", () => foo.Object.Boo());
			}
#pragma warning restore 618
		}

		#endregion

		#region #160

#if !SILVERLIGHT
		public class _160
		{
			[Fact]
			public void ShouldMockHtmlControl()
			{
				// CallBase was missing
				var htmlInputTextMock = new Mock<HtmlInputText>() { CallBase = true };
				Assert.True(htmlInputTextMock.Object.Visible);
			}
		}
#endif

		#endregion

		#region #161

		public class _161
		{
			[Fact]
			public void InvertEqualObjects()
			{
				var foo1 = new Foo { Id = "1" };
				var foo = new Foo { Id = "2" };

				var dependency = new Mock<IDependency>();

				dependency.Setup(x => x.DoThis(foo, foo1))
				  .Returns(new Foo());

				var f = dependency.Object.DoThis(foo, foo1);

				dependency.Verify(x => x.DoThis(foo, foo1));
				dependency.Verify(x => x.DoThis(foo1, foo), Times.Never());
			}

			[Fact(Skip = "Wrong Equals implemention in the report. Won't Fix")]
			public void ExampleFailingTest()
			{
				var foo1 = new Foo();
				var foo = new Foo();

				var sut = new Perfectly_fine_yet_failing_test();
				var dependency = new Mock<IDependency>();

				dependency.Setup(x => x.DoThis(foo, foo1))
				  .Returns(new Foo());

				sut.Do(dependency.Object, foo, foo1);

				dependency.Verify(x => x.DoThis(foo, foo1));
				dependency.Verify(x => x.DoThis(foo1, foo), Times.Never());
			}

			public class Perfectly_fine_yet_failing_test
			{
				public void Do(IDependency dependency, Foo foo, Foo foo1)
				{
					var foo2 = dependency.DoThis(foo, foo1);
					if (foo2 == null)
						foo2 = dependency.DoThis(foo1, foo);
				}
			}

			public interface IDependency
			{
				Foo DoThis(Foo foo, Foo foo1);
			}

			public class Foo
			{
				public string Id { get; set; }

				public override bool Equals(object obj)
				{
					return obj is Foo && ((Foo)obj).Id == Id;
				}

				public override int GetHashCode()
				{
					return base.GetHashCode();
				}
			}
		}

		#endregion

		#region #174

		public class _174
		{
			[Fact]
			public void Test()
			{
				var serviceNo1Mock = new Mock<IServiceNo1>();
				var collaboratorMock = new Mock<ISomeCollaborator>();

				collaboratorMock.Object.Collaborate(serviceNo1Mock.Object);

				collaboratorMock.Verify(o => o.Collaborate(serviceNo1Mock.Object));
			}

			public interface ISomeCollaborator
			{
				void Collaborate(IServiceNo1 serviceNo1);
			}

			public interface IServiceNo1 : IEnumerable
			{
			}
		}

		#endregion

		#region #177

		public class _177
		{
			[Fact]
			public void Test()
			{
				var mock = new Mock<IMyInterface>();
				Assert.NotNull(mock.Object);
			}

			public interface IMyInterface
			{
				void DoStuff<TFrom, TTo>() where TTo : TFrom;
			}
		}

		#endregion

		#region #184

		public class _184
		{
			[Fact]
			public void Test()
			{
				var fooRaised = false;
				var barRaised = false;

				var fooMock = new Mock<IFoo>();
				var barMock = fooMock.As<IBar>();

				fooMock.Object.FooEvent += (s, e) => fooRaised = true;
				barMock.Object.BarEvent += (s, e) => barRaised = true;

				fooMock.Raise(m => m.FooEvent += null, EventArgs.Empty);
				barMock.Raise(m => m.BarEvent += null, EventArgs.Empty);

				Assert.True(fooRaised);
				Assert.True(barRaised);
			}

			public interface IFoo
			{
				event EventHandler FooEvent;
			}

			public interface IBar
			{
				event EventHandler BarEvent;
			}
		}

		#endregion

		#region #185

		public class _185
		{
			[Fact]
			public void Test()
			{
				var mock = new Mock<IList<string>>();
				Assert.Throws<NotSupportedException>(
					Resources.SetupOnNonMemberMethod,
					() => mock.Setup(l => l.FirstOrDefault()).Returns("Hello world"));
			}
		}

		#endregion

		#region #187

		public class _187
		{
			[Fact]
			public void Test()
			{
				var mock = new Mock<IGeneric>();

				mock.Setup(r => r.Get<Foo.Inner>()).Returns(new Object());
				mock.Setup(r => r.Get<Bar.Inner>()).Returns(new Object());

				Assert.NotNull(mock.Object.Get<Foo.Inner>());
				Assert.NotNull(mock.Object.Get<Bar.Inner>());
			}

			public class Foo
			{
				public class Inner
				{
				}
			}

			public class Bar
			{
				public class Inner
				{
				}
			}

			public interface IGeneric
			{
				object Get<T>() where T : new();
			}
		}

		#endregion

		#region #183

		public class _183
		{
			[Fact]
			public void Test()
			{
				var mock = new Mock<IFoo>();
				mock.Setup(m => m.Execute(1));
				mock.Setup(m => m.Execute(It.IsInRange(2, 20, Range.Exclusive)));
				mock.Setup(m => m.Execute(3, "Caption"));

				mock.Object.Execute(3);
				mock.Object.Execute(4);
				mock.Object.Execute(5);

				var e = Assert.Throws<MockException>(() => mock.Verify(m => m.Execute(0)));
				Assert.Contains(
					"\r\nConfigured setups:" +
					"\r\nm => m.Execute(1), Times.Never" +
					"\r\nm => m.Execute(It.IsInRange<Int32>(2, 20, Range.Exclusive)), Times.Exactly(3)",
					e.Message);
			}

			[Fact]
			public void TestGeneric()
			{
				var mock = new Mock<IFoo>();
				mock.Setup(m => m.Execute<int>(1, 10));
				mock.Setup(m => m.Execute<string>(1, "Foo"));

				mock.Object.Execute(1, 10);

				var e = Assert.Throws<MockException>(() => mock.Verify(m => m.Execute<int>(1, 1)));
				Assert.Contains(
					"\r\nConfigured setups:\r\nm => m.Execute<Int32>(1, 10), Times.Once",
					e.Message);
			}

			[Fact]
			public void TestNoSetups()
			{
				var mock = new Mock<IFoo>();

				var e = Assert.Throws<MockException>(() => mock.Verify(m => m.Execute(1)));
				Assert.Contains("\r\nNo setups configured.", e.Message);

			}

			public interface IFoo
			{
				void Execute(int param);
				void Execute(int param, string caption);
				void Execute<T>(int p, T param);
			}
		}

		#endregion

		#region #186

		public class _186
		{
			[Fact]
			public void TestVerifyMessage()
			{
				var mock = new Mock<Foo>();
				mock.Setup(m => m.OnExecute());

				var e = Assert.Throws<NotSupportedException>(() => mock.Verify(m => m.Execute()));
				Assert.True(e.Message.StartsWith("Invalid verify"));
			}

			public class Foo
			{
				public void Execute()
				{
					this.OnExecute();
				}

				public virtual void OnExecute()
				{
					throw new NotImplementedException();
				}
			}
		}

		#endregion

		#region #190

		public class _190
		{
			[Fact]
			public void Test()
			{
				var mock = new Mock<IDisposable>().As<IComponent>();
				mock.SetupAllProperties();

				ISite site = new FooSite();
				mock.Object.Site = site;
				Assert.Same(site, mock.Object.Site);
			}

			public class FooSite : ISite
			{
				public IComponent Component
				{
					get { throw new NotImplementedException(); }
				}

				public IContainer Container
				{
					get { throw new NotImplementedException(); }
				}

				public bool DesignMode
				{
					get { throw new NotImplementedException(); }
				}

				public string Name
				{
					get { throw new NotImplementedException(); }
					set { throw new NotImplementedException(); }
				}

				public object GetService(Type serviceType)
				{
					throw new NotImplementedException();
				}
			}

		}

		#endregion

		#region #204

		public class _204
		{
			[Fact]
			public void Test()
			{
				var mock = new Mock<IRepository>();
				mock.Setup(x => x.Select<User>(u => u.Id == 100))
					.Returns(new User() { Id = 100 });

				var user = mock.Object.Select<User>(usr => usr.Id == 100);
				Assert.Equal(100, user.Id);
				mock.Verify(x => x.Select<User>(usr => usr.Id == 100), Times.Once());

				user = mock.Object.Select<User>(usr => usr.Id == 101);
				Assert.Null(user);
				mock.Verify(x => x.Select<User>(usr => usr.Id == 101), Times.Once());

				mock.Verify(x => x.Select<User>(usr => usr.Id == 102), Times.Never());
				mock.Verify(x => x.Select<User>(It.IsAny<Expression<Func<User, bool>>>()), Times.Exactly(2));
			}

			public interface IRepository
			{
				T Select<T>(Expression<Func<T, bool>> filter) where T : class;
			}

			public class User
			{
				public int Id { get; set; }
			}
		}

		#endregion

		#region #205

		public class _205
		{
			[Fact]
			public void Test()
			{
				Assert.DoesNotThrow(() => new Mock<IFoo>().SetupAllProperties());
			}

			public interface IFoo
			{
				string Error { get; set; }
				string this[int index] { get; set; }
			}
		}

		#endregion

		#region #223

		public class _223
		{
			[Fact]
			public void TestSetup()
			{
				this.TestSetupHelper<Foo>();
			}

			public void TestSetupHelper<T>() where T : class, IFoo<int>
			{
				var expected = 2;

				var target = new Mock<T>();
				target.Setup(p => p.DoInt32(0)).Returns(expected);
				target.Setup(p => p.DoGeneric(0)).Returns(expected);

				Assert.Equal(expected, target.Object.DoInt32(0));
				Assert.Equal(expected, target.Object.DoGeneric(0));
			}

			public interface IFoo<T>
			{
				int DoInt32(int value);
				T DoGeneric(int value);
			}

			public class Foo : IFoo<int>
			{
				public virtual int DoInt32(int value)
				{
					return 4;
				}

				public virtual int DoGeneric(int value)
				{
					return 5;
				}
			}
		}

		#endregion

		#region #229

		public class _229
		{
			[Fact]
			public void Test()
			{
				var target = new Mock<Foo> { CallBase = true };

				var raised = false;
				target.Object.MyEvent += (s, e) => raised = true;
				target.Object.RaiseMyEvent();

				Assert.True(raised);
			}

			public class Foo
			{
				public virtual event EventHandler MyEvent;

				public void RaiseMyEvent()
				{
					if (this.MyEvent != null)
					{
						this.MyEvent(this, EventArgs.Empty);
					}
				}
			}
		}

		#endregion

		#region #230

		public class _230
		{
			[Fact]
			public void ByteArrayCallbackArgumentShouldNotBeNull()
			{
				var data = new byte[] { 2, 1, 2 };
				var stream = new Mock<Stream>();

				stream.SetupGet(m => m.Length)
					.Returns(data.Length);
				stream.Setup(m => m.Read(It.IsAny<byte[]>(), It.IsAny<int>(), It.IsAny<int>()))
					.Callback<byte[], int, int>((b, o, c) => data.CopyTo(b, 0))
					.Returns(data.Length);

				var contents = new byte[stream.Object.Length];
				Assert.DoesNotThrow(() => stream.Object.Read(contents, 0, (int)stream.Object.Length));
			}
		}

		#endregion

		#region #232

		public class _232
		{
			[Fact]
			public void Test()
			{
				var repository = new Mock<IRepository>();
				var svc = new Service(repository.Object);

				svc.Create();

				repository.Verify(r => r.Insert(It.IsAny<Foo>()), Times.Once());
				repository.Verify(r => r.Insert(It.IsAny<Bar>()), Times.Once());
				repository.Verify(r => r.Insert(It.IsAny<IEntity>()), Times.Exactly(2));
			}

			public interface IRepository
			{
				void Insert(IEntity entity);
			}

			public interface IEntity
			{
			}

			public class Foo : IEntity
			{
			}

			public class Bar : IEntity
			{
			}

			public class Service
			{
				private IRepository repository;

				public Service(IRepository repository)
				{
					this.repository = repository;
				}

				public void Create()
				{
					repository.Insert(new Foo());
					repository.Insert(new Bar());
				}
			}
		}

		#endregion

		#region #242

		public class _242
		{
			[Fact]
			public void PropertyChangedTest()
			{
				var mock = new Mock<PropertyChangedInherited>();
				int callbacks = 0;
				mock.Object.PropertyChanged += (sender, args) => callbacks++;

				mock.Raise(m => m.PropertyChanged += null, new PropertyChangedEventArgs("Foo"));
				Assert.Equal(1, callbacks);
			}

			public class PropertyChangedBase : INotifyPropertyChanged
			{
				public virtual event PropertyChangedEventHandler PropertyChanged = (s, e) => { };
			}

			public class PropertyChangedInherited : PropertyChangedBase
			{
			}
		}

		#endregion

		#region #245

		public class _245
		{
			[Fact]
			public void Test()
			{
				var mock = new Mock<ITest>();

				ITest instance;
				Assert.DoesNotThrow(() => instance = mock.Object);
			}

			public interface ITest
			{
				void Do<T1, T2>() where T2 : T1;
			}
		}

		#endregion

		#region #249

#if !SILVERLIGHT
		//helper class/interfaces for thread safety expectations
		public interface IExpectToBeThreadSafe
		{
			void DoTheJob();
			void RaceCondition();
		}
		public class ExpectToBeThreadSafe : IExpectToBeThreadSafe
		{
			#region IExpectToBeThreadSafe Members
			int someVariable = 0;
			public virtual void DoTheJob()
			{
				if (someVariable > 0)
					RaceCondition();
				someVariable++;
				Thread.Sleep(2);
				someVariable--;
			}

			#endregion

			#region IExpectToBeThreadSafe Members


			public virtual void RaceCondition()
			{

			}

			#endregion
		}
		public class _249
		{
			[Fact]
			public void ExpectAvoidRaceConditions()
			{
				int concurrent = 8;
				var v = new Mock<ExpectToBeThreadSafe>();

				v.CallBase = true;
				for (int i = 0; i < concurrent; ++i)
				{
					ThreadPool.QueueUserWorkItem(
						(k) =>
						{
							v.Object.DoTheJob();
						}
						);
				}

				Thread.Sleep(200);
				v.Verify(k => k.DoTheJob(), Times.Exactly(concurrent));
				v.Verify(k => k.RaceCondition(), Times.Never());
			}
		}

#endif
		#endregion


		#region #251

		public class _251
		{
			[Fact]
			public void Test()
			{
				var repositoryMock = new Mock<IRepository<string>>();

				var repository = repositoryMock.Object;
				repository.Save("test");

				repositoryMock.Verify(m => m.Save("test"));
			}

			public interface IRepository
			{
				void Save(string value);
			}

			public interface IRepository<T> : IRepository
			{
				void Save(T value);
			}
		}

		#endregion

		#region #256

		public class _256
		{
			[Fact]
			public void TestFinalizeNotMocked()
			{
				var mock = new Mock<ClassWithFinalizer>(MockBehavior.Strict);
				mock.Setup(m => m.Foo).Returns(10);
				mock.Setup(m => m.Bar).Returns("Hello mocked world!");
				var instance = mock.Object;

				Assert.Equal(10, instance.Foo);
			}

			public class ClassWithFinalizer
			{
				public virtual int Foo { get; set; }
				public virtual string Bar { get; set; }

				~ClassWithFinalizer()
				{

				}
			}
		}

		#endregion

		#region #261

		public class _261
		{
			[Fact]
			public void Test()
			{
				var mock = new Mock<Foo>();
				mock.Protected().SetupSet<int>("Status", 42);

				mock.Object.SetStatus(42);

				mock.Protected().VerifySet<int>("Status", Times.Once(), 42);
			}

			public class Foo
			{
				public virtual int Status { get; protected set; }

				internal void SetStatus(int value)
				{
					this.Status = value;
				}
			}
		}

		#endregion

		#region #267

		public class _267
		{
			public interface IPerformOperation
			{
				string Operation(object input);
			}

			public class OperationUser
			{
				private readonly IPerformOperation m_OperationPerformer;

				public OperationUser(IPerformOperation operationPerformer)
				{
					m_OperationPerformer = operationPerformer;
				}

				public string DoOperation(object input)
				{
					return m_OperationPerformer.Operation(input);
				}
			}

			public class HelperSetup
			{
				private Mock<IPerformOperation> m_OperationStub;

				public HelperSetup()
				{
					m_OperationStub = new Mock<IPerformOperation>();
				}

				[Fact]
				public void InlineSetupTest()
				{
					m_OperationStub.Setup(m => m.Operation(It.IsAny<string>())).Returns<string>(value => "test");
					m_OperationStub.Setup(m => m.Operation(It.IsAny<int>())).Returns<int>(value => "25");

					var operationUser = new OperationUser(m_OperationStub.Object);

					var intOperationResult = operationUser.DoOperation(9);
					var stringOperationResult = operationUser.DoOperation("Hello");

					Assert.Equal("25", intOperationResult);
					Assert.Equal("test", stringOperationResult);
				}

				[Fact]
				public void HelperSetupTest()
				{
					SetupOperationStub<string>(value => "test");
					SetupOperationStub<int>(value => "25");

					var operationUser = new OperationUser(m_OperationStub.Object);

					var intOperationResult = operationUser.DoOperation(9);
					var stringOperationResult = operationUser.DoOperation("Hello");

					Assert.Equal("25", intOperationResult);
					Assert.Equal("test", stringOperationResult);
				}

				private void SetupOperationStub<T>(Func<T, string> valueFunction)
				{
					m_OperationStub.Setup(m => m.Operation(It.IsAny<T>())).Returns<T>(valueFunction);
				}
			}
		}

		#endregion

		#region #273

#if !SILVERLIGHT

		public class _273
		{
			[Fact]
			public void WhenMockingAnExternalInterface_ThenItWorks()
			{
				Assert.NotNull(new Mock<ClassLibrary1.IFoo>().Object);
				Assert.NotNull(Mock.Of<ClassLibrary1.IFoo>());
				Assert.NotNull(new Mock<ClassLibrary1.Foo>().Object);
				Assert.NotNull(new Mock<ClassLibrary2.IBar>().Object);
				Assert.NotNull(Mock.Of<ClassLibrary2.IBar>());
				Assert.NotNull(new Mock<ClassLibrary2.Bar>().Object);
				Assert.NotNull(new Mock<Baz>().Object);
			}

			public class Baz : ClassLibrary2.Bar
			{
			}
		}

#endif

		#endregion

		#region Recursive issue

		public class RecursiveFixture
		{
			[Fact]
			public void TestRecursive()
			{
				var mock = new Mock<ControllerContext>() { DefaultValue = DefaultValue.Mock };
				mock.Setup(c => c.HttpContext.Response.Write("stuff"));

				mock.Object.HttpContext.Response.Write("stuff");
				mock.Object.HttpContext.Response.ShouldEncode = true;

				Assert.Throws<MockException>(() => mock.VerifySet(
					c => c.HttpContext.Response.ShouldEncode = It.IsAny<bool>(),
					Times.Never()));
			}

			public class ControllerContext
			{
				public virtual HttpContext HttpContext { get; set; }
			}

			public abstract class HttpContext
			{
				protected HttpContext()
				{
				}

				public virtual HttpResponse Response
				{
					get { throw new NotImplementedException(); }
				}
			}

			public abstract class HttpResponse
			{
				protected HttpResponse()
				{
				}

				public virtual bool ShouldEncode
				{
					get { throw new NotImplementedException(); }
					set { throw new NotImplementedException(); }
				}

				public virtual void Write(string s)
				{
					throw new NotImplementedException();
				}
			}
		}

		#endregion

		#region Silverlight excluded

#if !SILVERLIGHT

		#region #250

		/// <summary>
		/// Silverlight MethodInfo protected constructor is internal, unlike desktop .NET
		/// </summary>
		public class _250
		{
			[Fact]
			public void Test()
			{
				var target = new Mock<MethodInfo>();

				Assert.NotNull(target.Object);
			}
		}

		#endregion

		#region #250

		public class _254
		{
			[Fact]
			public void ShouldMockInteropDTE()
			{
				var dte = new Mock<EnvDTE.DTE>();

				Assert.NotNull(dte.Object);
			}

#if !NET3x && !SERVER && HAVEOFFICE
			[Fact]
			public void ShouldRaiseEventOnInteropInterface()
			{
				var app = new Mock<Microsoft.Office.Interop.Word.Application>();
				var count = 0;
				app.Object.DocumentOpen += doc => count++;

				app.Raise(x => x.DocumentOpen += null, new Mock<Microsoft.Office.Interop.Word.Document>().Object);

				Assert.Equal(1, count);
			}
#endif
		}

		#endregion

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

		[ServiceContract]
		public interface IServiceContract
		{
			[OperationContract]
			void Do();
		}

#endif

		#endregion
	}
}