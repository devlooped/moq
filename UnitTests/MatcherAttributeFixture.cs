using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace Moq.Tests
{
	public class MatcherAttributeFixture
	{
		public interface IFoo
		{
			void Bar(string p);
		}

		[Fact]
		public void ShouldTranslateToUseMatcherImplementation()
		{			
			var mock = new Mock<IFoo>(MockBehavior.Strict);
			mock.Expect(x => x.Bar(IsMagicString()));
			IsMagicStringCalled = false;
			mock.Object.Bar("magic");
			Assert.True(IsMagicStringCalled);
		}

		[Fact]
		//[ExpectedException] not used so IsMagicStringCalled can be verified
		public void ShouldTranslateToUseMatcherImplementation2()
		{
			var mock = new Mock<IFoo>(MockBehavior.Strict);
			mock.Expect(x => x.Bar(IsMagicString()));
			IsMagicStringCalled = false;
			Exception expectedException = null;
			try
			{
				mock.Object.Bar("no-magic");
			}
			catch (Exception e)
			{
				expectedException = e;
			}
			Assert.True(IsMagicStringCalled);
			Assert.NotNull(expectedException);
		}

		static bool IsMagicStringCalled;

		[Matcher]
		public static string IsMagicString() { return null; }
		public static bool IsMagicString(string arg)
		{
			IsMagicStringCalled = true;
			return arg == "magic";
		}

		[Fact]
		public void ShouldUseAditionalArguments()
		{
			var mock = new Mock<IFoo>(MockBehavior.Strict);
			mock.Expect(x => x.Bar(StartsWith("ma")));
			mock.Object.Bar("magic");
		}

		[Fact]
		public void ShouldUseAditionalArguments2()
		{
			var mock = new Mock<IFoo>(MockBehavior.Strict);
			mock.Expect(x => x.Bar(StartsWith("ma")));
			Assert.Throws<MockException>(() => mock.Object.Bar("no-magic"));
		}

		[Matcher]
		public static string StartsWith(string prefix) { return null; }
		public static bool StartsWith(string arg, string prefix)
		{
			return arg.StartsWith(prefix);
		}

		[Fact]
		public void ExpectMissingMatcherMethod()
		{
			var mock = new Mock<IFoo>(MockBehavior.Strict);
			
			Assert.Throws<MissingMethodException>(
				"public static bool MatcherHookWithoutMatcherMethod(System.String) in class Moq.Tests.MatcherAttributeFixture.", 
				() => mock.Expect(x => x.Bar(MatcherHookWithoutMatcherMethod())));
		}

		[Matcher]
		public static string MatcherHookWithoutMatcherMethod() { return null; }

		[Fact]
		public void ExpectMissingMatcherWithArgsMethod()
		{
			var mock = new Mock<IFoo>(MockBehavior.Strict);
			
			Assert.Throws<MissingMethodException>(
				"public static bool MatcherHook2WithoutMatcherMethod(System.String, System.Int32) in class Moq.Tests.MatcherAttributeFixture.",
				() => mock.Expect(x => x.Bar(MatcherHook2WithoutMatcherMethod(6))));
		}

		[Matcher]
		public static string MatcherHook2WithoutMatcherMethod(int a) { return null; }

		[Fact]
		public void UseCurrentInstanceAsContext()
		{
			var mock = new Mock<IFoo>(MockBehavior.Strict);
			mock.Expect(x => x.Bar(NonStaticMatcherHook()));
			NonStaticMatcherHookExpectedArg = "Do It";

			mock.Object.Bar("Do It");
		}

		[Matcher]
		public string NonStaticMatcherHook() { return null; }
		public bool NonStaticMatcherHook(string arg) { return arg == NonStaticMatcherHookExpectedArg; }
		string NonStaticMatcherHookExpectedArg;

		[Fact]
		public void ExpectMissingNonStaticMatcherMethod()
		{
			var mock = new Mock<IFoo>(MockBehavior.Strict);

			Assert.Throws<MissingMethodException>(
				"public bool NonStaticMatcherHookWithoutMatcherMethod(System.String) in class Moq.Tests.MatcherAttributeFixture.",
				() => mock.Expect(x => x.Bar(NonStaticMatcherHookWithoutMatcherMethod())));
		}

		[Matcher]
		public string NonStaticMatcherHookWithoutMatcherMethod() { return null; }

		//[Fact]
		//public void AllowStaticPropertyAsMatcherHook()
		//{
		//    var mock = new Mock<IFoo>(MockBehavior.Strict);
		//    mock.Expect(x => x.Bar(NotNull));
		//    mock.Object.Bar("a");
		//}

		//[Matcher]
		//public static string NotNull { get { return null; } }
		//public static bool NotNull(string arg) { return arg != null; }

		[Fact]
		public void AllowStaticMethodsInHelperClassAsMatcherHook()
		{
		    var mock = new Mock<IFoo>(MockBehavior.Strict);
		    mock.Expect(x => x.Bar(A.NotNull()));
		    mock.Object.Bar("a");
		}

		public static class A
		{
			[Matcher]
			public static string NotNull() { return null; }
			public static bool NotNull(string arg) { return arg != null; }
		}

		[Fact]
		public void AllowHelperClassInstance()
		{
			var mock = new Mock<IFoo>(MockBehavior.Strict);
			var b = new B();
			mock.Expect(x => x.Bar(b.NotNull()));
			mock.Object.Bar("a");
		}

		public class B
		{
			[Matcher]
			public string NotNull() { return null; }
			public bool NotNull(string arg) { return arg != null; }
		}
	}
}
