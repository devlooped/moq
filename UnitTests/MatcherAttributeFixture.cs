using System;
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
		public void ShouldFindGenericMethodMatcher()
		{
			var foo = new Mock<IFoo>();

			foo.Object.Bar("asdf");

			foo.Verify(f => f.Bar(Any<string>()));
		}

#pragma warning disable 618
		[Matcher]
		public T Any<T>()
		{
			return default(T);
		}

#pragma warning restore 618

		public bool Any<T>(T value)
		{
			return true;
		}

		[Fact]
		public void ShouldNotFindPrivateMethodMatcher()
		{
			var foo = new Mock<IFoo>();

			foo.Object.Bar("asd");

			Assert.Throws<MissingMethodException>(() => foo.Verify(f => f.Bar(OddLength())));
		}

#pragma warning disable 618
		[Matcher]
		private static string OddLength()
		{
			return default(string);
		}
#pragma warning restore 618

		private static bool OddLength(string value)
		{
			return value.Length % 2 == 0;
		}

		[Fact]
		public void ShouldTranslateToUseMatcherImplementation()
		{
			var mock = new Mock<IFoo>(MockBehavior.Strict);
			mock.Setup(x => x.Bar(IsMagicString()));
			IsMagicStringCalled = false;
			mock.Object.Bar("magic");
			Assert.True(IsMagicStringCalled);
		}

		[Fact]
		//[ExpectedException] not used so IsMagicStringCalled can be verified
		public void ShouldTranslateToUseMatcherImplementation2()
		{
			var mock = new Mock<IFoo>(MockBehavior.Strict);
			mock.Setup(x => x.Bar(IsMagicString()));
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

		private static bool IsMagicStringCalled;

#pragma warning disable 618
		[Matcher]
		public static string IsMagicString()
		{
			return null;
		}
#pragma warning restore 618

		public static bool IsMagicString(string arg)
		{
			IsMagicStringCalled = true;
			return arg == "magic";
		}

		[Fact]
		public void ShouldUseAditionalArguments()
		{
			var mock = new Mock<IFoo>(MockBehavior.Strict);
			mock.Setup(x => x.Bar(StartsWith("ma")));
			mock.Object.Bar("magic");
		}

		[Fact]
		public void ShouldUseAditionalArguments2()
		{
			var mock = new Mock<IFoo>(MockBehavior.Strict);
			mock.Setup(x => x.Bar(StartsWith("ma")));
			Assert.Throws<MockException>(() => mock.Object.Bar("no-magic"));
		}

#pragma warning disable 618
		[Matcher]
		public static string StartsWith(string prefix)
		{
			return null;
		}
#pragma warning restore 618

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
				() => mock.Setup(x => x.Bar(MatcherHookWithoutMatcherMethod())));
		}

#pragma warning disable 618
		[Matcher]
		public static string MatcherHookWithoutMatcherMethod()
		{
			return null;
		}
#pragma warning restore 618

		[Fact]
		public void ExpectMissingMatcherWithArgsMethod()
		{
			var mock = new Mock<IFoo>(MockBehavior.Strict);

			Assert.Throws<MissingMethodException>(
				"public static bool MatcherHook2WithoutMatcherMethod(System.String, System.Int32) in class Moq.Tests.MatcherAttributeFixture.",
				() => mock.Setup(x => x.Bar(MatcherHook2WithoutMatcherMethod(6))));
		}

#pragma warning disable 618
		[Matcher]
		public static string MatcherHook2WithoutMatcherMethod(int a)
		{
			return null;
		}
#pragma warning restore 618

		[Fact]
		public void UseCurrentInstanceAsContext()
		{
			var mock = new Mock<IFoo>(MockBehavior.Strict);
			mock.Setup(x => x.Bar(NonStaticMatcherHook()));
			NonStaticMatcherHookExpectedArg = "Do It";

			mock.Object.Bar("Do It");
		}

#pragma warning disable 618
		[Matcher]
		public string NonStaticMatcherHook()
		{
			return null;
		}
#pragma warning restore 618

		public bool NonStaticMatcherHook(string arg)
		{
			return arg == NonStaticMatcherHookExpectedArg;
		}

		private string NonStaticMatcherHookExpectedArg;

		[Fact]
		public void ExpectMissingNonStaticMatcherMethod()
		{
			var mock = new Mock<IFoo>(MockBehavior.Strict);

			Assert.Throws<MissingMethodException>(
				"public bool NonStaticMatcherHookWithoutMatcherMethod(System.String) in class Moq.Tests.MatcherAttributeFixture.",
				() => mock.Setup(x => x.Bar(NonStaticMatcherHookWithoutMatcherMethod())));
		}

#pragma warning disable 618
		[Matcher]
		public string NonStaticMatcherHookWithoutMatcherMethod()
		{
			return null;
		}
#pragma warning restore 618

		[Fact]
		public void AllowStaticMethodsInHelperClassAsMatcherHook()
		{
			var mock = new Mock<IFoo>(MockBehavior.Strict);
			mock.Setup(x => x.Bar(A.NotNull()));
			mock.Object.Bar("a");
		}

		public static class A
		{
#pragma warning disable 618
			[Matcher]
			public static string NotNull()
			{
				return null;
			}
#pragma warning restore 618

			public static bool NotNull(string arg)
			{
				return arg != null;
			}
		}

		[Fact]
		public void AllowHelperClassInstance()
		{
			var mock = new Mock<IFoo>(MockBehavior.Strict);
			var b = new B();
			mock.Setup(x => x.Bar(b.NotNull()));
			mock.Object.Bar("a");
		}

		public class B
		{
#pragma warning disable 618
			[Matcher]
			public string NotNull()
			{
				return null;
			}
#pragma warning restore 618

			public bool NotNull(string arg)
			{
				return arg != null;
			}
		}
	}
}
