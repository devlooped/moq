using System;
using System.Linq.Expressions;
using Xunit;

namespace Moq.Tests
{
	public class MockFixture
	{
		[Fact]
		public void CreatesMockAndExposesInterface()
		{
			var mock = new Mock<IComparable>();

			IComparable comparable = mock.Object;

			Assert.NotNull(comparable);
		}

		[Fact]
		public void ThrowsIfNullExpectAction()
		{
			var mock = new Mock<IComparable>();

			Assert.Throws<ArgumentNullException>(() => mock.Setup((Expression<Action<IComparable>>)null));
		}

		[Fact]
		public void ThrowsIfNullExpectFunction()
		{
			var mock = new Mock<IComparable>();

			Assert.Throws<ArgumentNullException>(() => mock.Setup((Expression<Func<IComparable, string>>)null));
		}

		[Fact]
		public void ThrowsIfExpectationSetForField()
		{
			var mock = new Mock<FooBase>();

			Assert.Throws<ArgumentException>(() => mock.Setup(x => x.ValueField));
		}

		[Fact]
		public void CallParameterCanBeVariable()
		{
			int value = 5;
			var mock = new Mock<IFoo>();

			mock.Setup(x => x.Echo(value)).Returns(() => value * 2);

			Assert.Equal(value * 2, mock.Object.Echo(value));
		}

		[Fact]
		public void CallParameterCanBeMethodCall()
		{
			int value = 5;
			var mock = new Mock<IFoo>();

			mock.Setup(x => x.Echo(GetValue(value))).Returns(() => value * 2);

			Assert.Equal(value * 2, mock.Object.Echo(value * 2));
		}

		private int GetValue(int value)
		{
			return value * 2;
		}

		[Fact]
		public void ExpectsVoidCall()
		{
			var mock = new Mock<IFoo>();

			mock.Setup(x => x.Submit());

			mock.Object.Submit();
		}

		[Fact]
		public void ThrowsIfExpectationThrows()
		{
			var mock = new Mock<IFoo>();

			mock.Setup(x => x.Submit()).Throws(new FormatException());

			Assert.Throws<FormatException>(() => mock.Object.Submit());
		}

		[Fact]
		public void ThrowsIfExpectationThrowsWithGenericsExceptionType()
		{
			var mock = new Mock<IFoo>();

			mock.Setup(x => x.Submit()).Throws<FormatException>();

			Assert.Throws<FormatException>(() => mock.Object.Submit());
		}

		[Fact]
		public void ReturnsServiceFromServiceProvider()
		{
			var provider = new Mock<IServiceProvider>();

			provider.Setup(x => x.GetService(typeof(IFooService))).Returns(new FooService());

			Assert.True(provider.Object.GetService(typeof(IFooService)) is FooService);
		}

		[Fact]
		public void InvokesLastExpectationThatMatches()
		{
			var mock = new Mock<IFoo>();
			mock.Setup(x => x.Execute(It.IsAny<string>())).Throws(new ArgumentException());
			mock.Setup(x => x.Execute("ping")).Returns("I'm alive!");

			Assert.Equal("I'm alive!", mock.Object.Execute("ping"));

			Assert.Throws<ArgumentException>(() => mock.Object.Execute("asdf"));
		}

		[Fact]
		public void MockObjectIsAssignableToMockedInterface()
		{
			var mock = new Mock<IFoo>();
			Assert.True(typeof(IFoo).IsAssignableFrom(mock.Object.GetType()));
		}

		[Fact]
		public void MockObjectsEqualityIsReferenceEquals()
		{
			var mock1 = new Mock<IFoo>();
			var mock2 = new Mock<IFoo>();

			Assert.True(mock1.Object.Equals(mock1.Object));
			Assert.False(mock1.Object.Equals(mock2.Object));
		}

		[Fact]
		public void HashCodeIsDifferentForEachMock()
		{
			var mock1 = new Mock<IFoo>();
			var mock2 = new Mock<IFoo>();

			Assert.Equal(mock1.Object.GetHashCode(), mock1.Object.GetHashCode());
			Assert.Equal(mock2.Object.GetHashCode(), mock2.Object.GetHashCode());
			Assert.NotEqual(mock1.Object.GetHashCode(), mock2.Object.GetHashCode());
		}

		[Fact]
		public void ToStringIsNullOrEmpty()
		{
			var mock = new Mock<IFoo>();
			Assert.False(String.IsNullOrEmpty(mock.Object.ToString()));
		}

		[Fact(Skip = "Castle.DynamicProxy2 doesn't seem to call interceptors for ToString, GetHashCode & Equals")]
		public void OverridesObjectMethods()
		{
			var mock = new Mock<IFoo>();
			mock.Setup(x => x.GetHashCode()).Returns(1);
			mock.Setup(x => x.ToString()).Returns("foo");
			mock.Setup(x => x.Equals(It.IsAny<object>())).Returns(true);

			Assert.Equal("foo", mock.Object.ToString());
			Assert.Equal(1, mock.Object.GetHashCode());
			Assert.True(mock.Object.Equals(new object()));
		}

		[Fact]
		public void OverridesBehaviorFromAbstractClass()
		{
			var mock = new Mock<FooBase>();
			mock.CallBase = true;

			mock.Setup(x => x.Check("foo")).Returns(false);

			Assert.False(mock.Object.Check("foo"));
			Assert.True(mock.Object.Check("bar"));
		}

		[Fact]
		public void CallsUnderlyingClassEquals()
		{
			var mock = new Mock<FooOverrideEquals>();
			var mock2 = new Mock<FooOverrideEquals>();

			mock.CallBase = true;

			mock.Object.Name = "Foo";
			mock2.Object.Name = "Foo";

			Assert.True(mock.Object.Equals(mock2.Object));
		}

		[Fact]
		public void ThrowsIfSealedClass()
		{
			Assert.Throws<NotSupportedException>(() => new Mock<FooSealed>());
		}

		[Fact]
		public void ThrowsIfExpectOnNonVirtual()
		{
			var mock = new Mock<FooBase>();

			Assert.Throws<NotSupportedException>(() => mock.Setup(x => x.True()).Returns(false));
		}

		[Fact]
		public void OverridesPreviousExpectation()
		{
			var mock = new Mock<IFoo>();

			mock.Setup(x => x.Echo(1)).Returns(5);

			Assert.Equal(5, mock.Object.Echo(1));

			mock.Setup(x => x.Echo(1)).Returns(10);

			Assert.Equal(10, mock.Object.Echo(1));
		}

		[Fact]
		public void ConstructsObjectsWithCtorArguments()
		{
			var mock = new Mock<FooWithConstructors>(MockBehavior.Default, "Hello", 26);

			Assert.Equal("Hello", mock.Object.StringValue);
			Assert.Equal(26, mock.Object.IntValue);

			// Should also construct without args.
			mock = new Mock<FooWithConstructors>(MockBehavior.Default);

			Assert.Equal(null, mock.Object.StringValue);
			Assert.Equal(0, mock.Object.IntValue);
		}

		[Fact]
		public void ConstructsClassWithNoDefaultConstructor()
		{
			var mock = new Mock<ClassWithNoDefaultConstructor>(MockBehavior.Default, "Hello", 26);

			Assert.Equal("Hello", mock.Object.StringValue);
			Assert.Equal(26, mock.Object.IntValue);
		}

		[Fact]
		public void ConstructsClassWithNoDefaultConstructorAndNullValue()
		{
			var mock = new Mock<ClassWithNoDefaultConstructor>(MockBehavior.Default, null, 26);

			Assert.Equal(null, mock.Object.StringValue);
			Assert.Equal(26, mock.Object.IntValue);
		}

		[Fact]
		public void ThrowsIfNoMatchingConstructorFound()
		{
			Assert.Throws<ArgumentException>(() =>
			{
				Console.WriteLine(new Mock<ClassWithNoDefaultConstructor>(25, true).Object);
			});
		}

		[Fact]
		public void ThrowsIfArgumentsPassedForInterface()
		{
			Assert.Throws<ArgumentException>(() => new Mock<IFoo>(25, true));
		}

		[Fact]
		public void ThrowsOnStrictWithExpectButNoReturns()
		{
			var mock = new Mock<IFoo>(MockBehavior.Strict);

			mock.Setup(x => x.Execute("ping"));

			try
			{
				mock.Object.Execute("ping");
				Assert.True(false, "SHould throw");
			}
			catch (MockException mex)
			{
				Assert.Equal(MockException.ExceptionReason.ReturnValueRequired, mex.Reason);
			}
		}

		[Fact]
		public void AllowsSetupNewHiddenProperties()
		{
			var value = new Mock<INewBar>().Object;

			var target = new Mock<INewFoo>();
			target.As<IFoo>().SetupGet(x => x.Bar).Returns(value);
			target.Setup(x => x.Bar).Returns(value);

			Assert.Equal(target.Object.Bar, target.As<IFoo>().Object.Bar);
		}

		[Fact]
		public void AllowsSetupNewHiddenBaseProperty()
		{
			var value = new Mock<INewBar>().Object;

			var target = new Mock<INewFoo>();
			target.As<IFoo>().SetupGet(x => x.Bar).Returns(value);

			Assert.Equal(value, target.As<IFoo>().Object.Bar);
			Assert.Null(target.Object.Bar);
		}

		[Fact]
		public void AllowsSetupNewHiddenInheritedProperty()
		{
			var value = new Mock<INewBar>().Object;

			var target = new Mock<INewFoo>();
			target.As<IFoo>();
			target.SetupGet(x => x.Bar).Returns(value);

			Assert.Equal(value, target.Object.Bar);
			Assert.Null(target.As<IFoo>().Object.Bar);
		}

		[Fact]
		public void ExpectsPropertySetter()
		{
			var mock = new Mock<IFoo>();

			int? value = 0;

			mock.SetupSet(foo => foo.Value = It.IsAny<int?>())
				.Callback<int?>(i => value = i);

			mock.Object.Value = 5;

			Assert.Equal(5, value);
		}

		[Fact]
		public void ExpectsPropertySetterLambda()
		{
			var mock = new Mock<IFoo>();

			mock.SetupSet(foo => foo.Count = 5);

			Assert.Throws<MockVerificationException>(() => mock.VerifyAll());

			mock.Object.Count = 6;

			Assert.Throws<MockVerificationException>(() => mock.VerifyAll());

			mock.Object.Count = 5;

			mock.VerifyAll();
		}

		[Fact]
		public void CallbackReceivesValueWithPropertySetterLambda()
		{
			var mock = new Mock<IFoo>();
			int value = 0;
			int value2 = 0;

			mock.SetupSet(foo => foo.Count = 6).Callback<int>(v => value = v);
			mock.SetupSet<int>(foo => foo.Count = 3).Callback(v => value2 = v);

			mock.Object.Count = 6;

			Assert.Equal(6, value);
			Assert.Equal(0, value2);

			mock.Object.Count = 3;

			Assert.Equal(3, value2);
		}

		[Fact]
		public void SetterLambdaUsesItIsAnyMatcher()
		{
			var mock = new Mock<IFoo>();

			mock.SetupSet(foo => foo.Count = It.IsAny<int>());

			Assert.Throws<MockVerificationException>(() => mock.VerifyAll());

			mock.Object.Count = 6;

			mock.VerifyAll();
		}

		[Fact]
		public void SetterLambdaUsesItIsInRangeMatcher()
		{
			var mock = new Mock<IFoo>();

			mock.SetupSet(foo => foo.Count = It.IsInRange(1, 5, Range.Inclusive));

			Assert.Throws<MockVerificationException>(() => mock.VerifyAll());

			mock.Object.Count = 6;

			Assert.Throws<MockVerificationException>(() => mock.VerifyAll());

			mock.Object.Count = 5;

			mock.VerifyAll();
		}

		[Fact]
		public void SetterLambdaUsesItIsPredicateMatcher()
		{
			var mock = new Mock<IFoo>();

			mock.SetupSet(foo => foo.Count = It.Is<int>(v => v % 2 == 0));

			Assert.Throws<MockVerificationException>(() => mock.VerifyAll());

			mock.Object.Count = 7;

			Assert.Throws<MockVerificationException>(() => mock.VerifyAll());

			mock.Object.Count = 4;

			mock.VerifyAll();
		}

		[Fact]
		public void SetterLambdaCannotHaveMultipleSetups()
		{
			var mock = new Mock<IFoo>();

			mock.SetupSet(foo => foo.Count = It.IsAny<int>())
				.Throws<ArgumentOutOfRangeException>();
			Assert.Throws<ArgumentOutOfRangeException>(() => mock.Object.Count = 5);

			mock.SetupSet(foo => foo.Count = It.IsInRange(1, 5, Range.Inclusive))
				.Throws<ArgumentException>();
			Assert.Throws<ArgumentException>(() => mock.Object.Count = 5);
		}

		[Fact]
		public void SetterLambdaDoesNotCountAsInvocation()
		{
			var mock = new Mock<IFoo>();

			mock.SetupSet(foo => foo.Count = 5);

			Assert.Throws<MockVerificationException>(() => mock.VerifyAll());

			mock.Object.Count = 5;
			mock.VerifyAll();
		}

		[Fact]
		public void SetterLambdaWithStrictMockWorks()
		{
			var mock = new Mock<IFoo>(MockBehavior.Strict);

			mock.SetupSet(foo => foo.Count = 5);
		}

		[Fact]
		public void ShouldAllowMultipleCustomMatcherWithArguments()
		{
			var mock = new Mock<IFoo>();

			mock.Setup(m => m.Echo(IsMultipleOf(2))).Returns(2);
			mock.Setup(m => m.Echo(IsMultipleOf(3))).Returns(3);

			Assert.Equal(2, mock.Object.Echo(4));
			Assert.Equal(2, mock.Object.Echo(8));
			Assert.Equal(3, mock.Object.Echo(9));
			Assert.Equal(3, mock.Object.Echo(3));
		}

		private int IsMultipleOf(int value)
		{
			return Match.Create<int>(i => i % value == 0);
		}

#pragma warning disable 618
		[Matcher]
		private static int OddInt()
		{
			return 0;
		}
#pragma warning restore 618

		private static bool OddInt(int value)
		{
			return value % 2 == 0;
		}

#pragma warning disable 618
		[Matcher]
		private int BigInt()
		{
			return 0;
		}
#pragma warning restore 618

		private bool BigInt(int value)
		{
			return value > 2;
		}

		[Fact]
		public void ExpectsPropertySetterLambdaCoercesNullable()
		{
			var mock = new Mock<IFoo>();

			mock.SetupSet(foo => foo.Value = 5);

			Assert.Throws<MockVerificationException>(() => mock.VerifyAll());

			mock.Object.Value = 6;

			Assert.Throws<MockVerificationException>(() => mock.VerifyAll());

			mock.Object.Value = 5;

			mock.VerifyAll();
		}

		[Fact]
		public void ExpectsPropertySetterLambdaValueReference()
		{
			var mock = new Mock<IFoo>();
			var obj = new object();

			mock.SetupSet(foo => foo.Object = obj);

			Assert.Throws<MockVerificationException>(() => mock.VerifyAll());

			mock.Object.Object = new object();

			Assert.Throws<MockVerificationException>(() => mock.VerifyAll());

			mock.Object.Object = obj;

			mock.VerifyAll();
		}

		[Fact]
		public void ExpectsPropertySetterLambdaRecursive()
		{
			var mock = new Mock<IFoo>();

			mock.SetupSet<string>(foo => foo.Bar.Value = "bar");

			Assert.Throws<MockVerificationException>(() => mock.VerifyAll());

			mock.Object.Bar.Value = "bar";

			mock.VerifyAll();
		}

		[Fact]
		public void ExpectsPropertySetterWithNullValue()
		{
			var mock = new Mock<IFoo>(MockBehavior.Strict);
			mock.SetupSet(m => m.Value = null);

			Assert.Throws<MockException>(() => { mock.Object.Value = 5; });
			Assert.Throws<MockVerificationException>(() => mock.VerifyAll());

			mock.Object.Value = null;

			mock.VerifyAll();
			mock.VerifySet(m => m.Value = It.IsAny<int?>());
		}

		[Fact]
		public void ThrowsIfPropertySetterWithWrongValue()
		{
			var mock = new Mock<IFoo>(MockBehavior.Strict);
			mock.SetupSet(m => m.Value = 5);

			Assert.Throws<MockException>(() => { mock.Object.Value = 6; });
		}

		[Fact]
		public void ExpectsPropertyGetter()
		{
			var mock = new Mock<IFoo>();

			bool called = false;

			mock.SetupGet(x => x.Value)
				.Callback(() => called = true)
				.Returns(25);

			Assert.Equal(25, mock.Object.Value);
			Assert.True(called);
		}

		[Fact]
		public void ThrowsIfExpectPropertySetterOnMethod()
		{
			var mock = new Mock<IFoo>();

			Assert.Throws<ArgumentException>(() => mock.SetupSet(foo => foo.Echo(5)));
		}

		[Fact]
		public void ThrowsIfExpectPropertyGetterOnMethod()
		{
			var mock = new Mock<IFoo>();

			Assert.Throws<ArgumentException>(() => mock.SetupGet(foo => foo.Echo(5)));
		}

		[Fact]
		public void DoesNotCallBaseClassVirtualImplementationByDefault()
		{
			var mock = new Mock<FooBase>();

			Assert.False(mock.Object.BaseCalled);
			mock.Object.BaseCall();

			Assert.False(mock.Object.BaseCalled);
		}

		[Fact]
		public void DoesNotCallBaseClassVirtualImplementationIfSpecified()
		{
			var mock = new Mock<FooBase>();

			mock.CallBase = false;

			Assert.False(mock.Object.BaseCalled);
			mock.Object.BaseCall();

			Assert.Equal(default(bool), mock.Object.BaseCall("foo"));
			Assert.False(mock.Object.BaseCalled);
		}

		[Fact]
		public void ExpectsGetIndexedProperty()
		{
			var mock = new Mock<IFoo>();

			mock.SetupGet(foo => foo[0])
				.Returns(1);
			mock.SetupGet(foo => foo[1])
				.Returns(2);

			Assert.Equal(1, mock.Object[0]);
			Assert.Equal(2, mock.Object[1]);
		}

		[Fact]
		public void ExpectAndExpectGetAreSynonyms()
		{
			var mock = new Mock<IFoo>();

			mock.SetupGet(foo => foo.Value)
				.Returns(1);
			mock.Setup(foo => foo.Value)
				.Returns(2);

			Assert.Equal(2, mock.Object.Value);
		}

		[Fact]
		public void ThrowsIfExpecationSetForNotOverridableMember()
		{
			var target = new Mock<Doer>();

			Assert.Throws<NotSupportedException>(() => target.Setup(t => t.Do()));
		}

		[Fact]
		public void ExpectWithParamArrayEmptyMatchArguments()
		{
			string expected = "bar";
			string argument = "foo";

			var target = new Mock<IParams>();
			target.Setup(x => x.ExecuteByName(argument)).Returns(expected);

			string actual = target.Object.ExecuteByName(argument);
			Assert.Equal(expected, actual);
		}

		[Fact]
		public void ExpectWithParamArrayNotMatchDifferentLengthInArguments()
		{
			string notExpected = "bar";
			string argument = "foo";

			var target = new Mock<IParams>();
			target.Setup(x => x.ExecuteParams(argument, It.IsAny<string>())).Returns(notExpected);

			string actual = target.Object.ExecuteParams(argument);
			Assert.NotEqual(notExpected, actual);
		}

		[Fact]
		public void ExpectWithParamArrayMatchArguments()
		{
			string expected = "bar";
			string argument = "foo";

			var target = new Mock<IParams>();
			target.Setup(x => x.ExecuteParams(argument, It.IsAny<string>())).Returns(expected);

			string ret = target.Object.ExecuteParams(argument, "baz");
			Assert.Equal(expected, ret);
		}

		[Fact]
		public void ExpectWithArrayNotMatchTwoDifferentArrayInstances()
		{
			string expected = "bar";
			string argument = "foo";

			var target = new Mock<IParams>();
			target.Setup(x => x.ExecuteArray(new string[] { argument, It.IsAny<string>() })).Returns(expected);

			string ret = target.Object.ExecuteArray(new string[] { argument, "baz" });
			Assert.Equal(null, ret);
		}

		[Fact]
		public void ExpectGetAndExpectSetMatchArguments()
		{
			var target = new Mock<IFoo>();
			target.SetupGet(d => d.Value).Returns(1);
			target.SetupSet(d => d.Value = 2);

			target.Object.Value = target.Object.Value + 1;

			target.VerifyAll();
		}

		[Fact]
		public void ArgumentNullMatchProperCtor()
		{
			var target = new Mock<Foo>(null);
			Assert.Null(target.Object.Bar);
		}

		[Fact]
		public void DistinguishesSameMethodsWithDifferentGenericArguments()
		{
			var mock = new Mock<FooBase>();

			mock.Setup(foo => foo.Generic<int>()).Returns(2);
			mock.Setup(foo => foo.Generic<string>()).Returns(3);

			Assert.Equal(2, mock.Object.Generic<int>());
			Assert.Equal(3, mock.Object.Generic<string>());
		}

		[Fact]
		public void CanCreateMockOfInternalInterface()
		{
			Assert.NotNull(new Mock<ClassLibrary1.IFooInternal>().Object);
		}

		public class Foo
		{
			public Foo() : this(new Bar()) { }

			public Foo(IBar bar)
			{
				this.Bar = bar;
			}

			public IBar Bar { get; private set; }
		}

		public class Bar : IBar
		{
			public string Value { get; set; }
		}

		public interface IBar
		{
			string Value { get; set; }
		}

		interface IDo { void Do(); }

		public class Doer : IDo
		{
			public void Do()
			{
			}
		}

		public sealed class FooSealed { }
		class FooService : IFooService { }
		interface IFooService { }

		public class FooWithPrivateSetter
		{
			public virtual string Foo { get; private set; }
		}

		public class ClassWithNoDefaultConstructor
		{
			public ClassWithNoDefaultConstructor(string stringValue, int intValue)
			{
				this.StringValue = stringValue;
				this.IntValue = intValue;
			}

			public string StringValue { get; set; }
			public int IntValue { get; set; }
		}

		public abstract class FooWithConstructors
		{
			public FooWithConstructors(string stringValue, int intValue)
			{
				this.StringValue = stringValue;
				this.IntValue = intValue;
			}

			public FooWithConstructors()
			{
			}

			public override string ToString()
			{
				return base.ToString();
			}

			public string StringValue { get; set; }
			public int IntValue { get; set; }
		}

		public class FooOverrideEquals
		{
			public string Name { get; set; }

			public override bool Equals(object obj)
			{
				return (obj is FooOverrideEquals) &&
					((FooOverrideEquals)obj).Name == this.Name;
			}

			public override int GetHashCode()
			{
				return Name.GetHashCode();
			}
		}

		public interface IFoo
		{
			object Object { get; set; }
			IBar Bar { get; set; }
			int Count { set; }
			int? Value { get; set; }
			int Echo(int value);
			void Submit();
			string Execute(string command);
			int this[int index] { get; set; }
		}

		public interface IParams
		{
			string ExecuteByName(string name, params object[] args);
			string ExecuteParams(params string[] args);
			string ExecuteArray(string[] args);
		}

		public abstract class FooBase
		{
			public int ValueField;
			public abstract void Do(int value);

			public virtual bool Check(string value)
			{
				return true;
			}

			public bool GetIsProtected()
			{
				return IsProtected();
			}

			protected virtual bool IsProtected()
			{
				return true;
			}

			public bool True()
			{
				return true;
			}

			public bool BaseCalled = false;

			public virtual void BaseCall()
			{
				BaseCalled = true;
			}

			public virtual int Generic<T>()
			{
				return 0;
			}

			public bool BaseReturnCalled = false;

			public virtual bool BaseCall(string value)
			{
				BaseReturnCalled = true;
				return default(bool);
			}
		}

		public interface INewFoo : IFoo
		{
			new INewBar Bar { get; set; }
		}

		public interface INewBar : IBar
		{
		}
	}
}