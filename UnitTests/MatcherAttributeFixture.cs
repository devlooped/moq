using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace Moq.Tests
{
	[TestFixture]
	public class MatcherAttributeFixture
	{
		public interface IFoo
		{
			void Bar(string p);
		}

		[Test]
		public void ShouldTranslateToUseMatcherImplementation()
		{			
			var mock = new Mock<IFoo>(MockBehavior.Strict);
			mock.Expect(x => x.Bar(IsMagicString()));
			IsMagicStringCalled = false;
			mock.Object.Bar("magic");
			Assert.IsTrue(IsMagicStringCalled);
		}

		[Test]
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
			Assert.IsTrue(IsMagicStringCalled);
			Assert.IsNotNull(expectedException);
		}

		static bool IsMagicStringCalled;

		[Matcher]
		public static string IsMagicString() { return null; }
		public static bool IsMagicString(string arg)
		{
			IsMagicStringCalled = true;
			return arg == "magic";
		}

		[Test]
		public void ShouldUseAditionalArguments()
		{
			var mock = new Mock<IFoo>(MockBehavior.Strict);
			mock.Expect(x => x.Bar(StartsWith("ma")));
			mock.Object.Bar("magic");
		}

		[Test]
		[ExpectedException]
		public void ShouldUseAditionalArguments2()
		{
			var mock = new Mock<IFoo>(MockBehavior.Strict);
			mock.Expect(x => x.Bar(StartsWith("ma")));
			mock.Object.Bar("no-magic");
		}

		[Matcher]
		public static string StartsWith(string prefix) { return null; }
		public static bool StartsWith(string arg, string prefix)
		{
			return arg.StartsWith(prefix);
		}

		[Test]
		[ExpectedException(
			ExceptionType = typeof(MissingMethodException),
			ExpectedMessage = "public static bool MatcherHookWithoutMatcherMethod(System.String) in class Moq.Tests.MatcherAttributeFixture.")]
		public void ExpectMissingMatcherMethod()
		{
			var mock = new Mock<IFoo>(MockBehavior.Strict);
			mock.Expect(x => x.Bar(MatcherHookWithoutMatcherMethod()));
			mock.Object.Bar(string.Empty);
		}

		[Matcher]
		public static string MatcherHookWithoutMatcherMethod() { return null; }

		[Test]
		[ExpectedException(
			ExceptionType = typeof(MissingMethodException),
			ExpectedMessage = "public static bool MatcherHook2WithoutMatcherMethod(System.String, System.Int32) in class Moq.Tests.MatcherAttributeFixture.")]
		public void ExpectMissingMatcherWithArgsMethod()
		{
			var mock = new Mock<IFoo>(MockBehavior.Strict);
			mock.Expect(x => x.Bar(MatcherHook2WithoutMatcherMethod(6)));
			mock.Object.Bar(string.Empty);
		}

		[Matcher]
		public static string MatcherHook2WithoutMatcherMethod(int a) { return null; }

		[Test]
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

		[Test]
		[ExpectedException(
			ExceptionType = typeof(MissingMethodException),
			ExpectedMessage = "public bool NonStaticMatcherHookWithoutMatcherMethod(System.String) in class Moq.Tests.MatcherAttributeFixture.")]
		public void ExpectMissingNonStaticMatcherMethod()
		{
			var mock = new Mock<IFoo>(MockBehavior.Strict);
			mock.Expect(x => x.Bar(NonStaticMatcherHookWithoutMatcherMethod()));
			mock.Object.Bar(string.Empty);
		}

		[Matcher]
		public string NonStaticMatcherHookWithoutMatcherMethod() { return null; }

		//[Test]
		//public void AllowStaticPropertyAsMatcherHook()
		//{
		//    var mock = new Mock<IFoo>(MockBehavior.Strict);
		//    mock.Expect(x => x.Bar(NotNull));
		//    mock.Object.Bar("a");
		//}

		//[Matcher]
		//public static string NotNull { get { return null; } }
		//public static bool NotNull(string arg) { return arg != null; }

		[Test]
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
	}
}
