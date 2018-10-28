// Copyright (c) 2007, Clarius Consulting, Manas Technology Solutions, InSTEDD.
// All rights reserved. Licensed under the BSD 3-Clause License; see License.txt.

using System;
using System.Collections;
using System.Collections.Generic;
#if DESKTOP
using System.Data.Entity;
using System.Data.Entity.Core.EntityClient;
#endif
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using Castle.DynamicProxy;
using Moq;
using Moq.Properties;
using Moq.Protected;
using Xunit;

#if !NETCORE
using System.Web.UI.HtmlControls;
#endif
#if FEATURE_SERIALIZATION
using System.Runtime.Serialization;
#endif
using System.Threading;
using System.Threading.Tasks;

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
		// @ GitHub

		#region #54

		public class Issue54
		{
			public interface IFoo
			{
				void Bar();
			}

			[Fact]
			public void when_as_disposable_then_succeeds()
			{
				var mock = new Mock<IFoo>();
				mock.Setup(x => x.Bar());
				mock.As<IDisposable>().Setup(x => x.Dispose());

				Action<IFoo> testMock = (IFoo foo) =>
				{
					foo.Bar();

					var disposable = foo as IDisposable;

					if (disposable != null)
					{
						disposable.Dispose();
					}
				};

				testMock(mock.Object);

				mock.VerifyAll();
			}
		}


		#endregion

		#region 47 & 62

		public class Issue47ClassToMock
		{
			AutoResetEvent reset = new AutoResetEvent(false);
			public virtual void M1()
			{
				//we're inside the interceptor's stack now

				//kick off a new thread to a method on ourselves, which will also go through the interceptor
				Thread th = new Thread(new ThreadStart(M2));
				th.Start();
				//ensure the thread has started
				Thread.Sleep(500);
				//release the thing the thread 'should' be waiting on
				reset.Set();
				//wait for the thread to finish
				th.Join();
			}

			public virtual void M2()
			{
				//this method will get called on the thread, via the interceptor
				//if the interceptor is locking then we won't get here until after the thread.Join above has
				//finished, which is blocked waiting on us.
				reset.WaitOne();
			}
		}

		[Fact]
		public void CallsToExternalCodeNotLockedInInterceptor()
		{
			var testMock = new Mock<Issue47ClassToMock> { CallBase = true };
			testMock.Object.M1(); // <-- This will never return if the interceptor is locking!
			testMock.Verify(x => x.M1());
			testMock.Verify(x => x.M2());
		}

#endregion

		#region #78

		public interface IIssue78Interface
		{
			Issue78TypeOne GetTypeOne();
			Issue78TypeTwo GetTypeTwo();
		}

		public class Issue78TypeOne
		{
		}
		public class Issue78TypeTwo
		{
		}

		public class Issue78Sut
		{
			public void TestMethod(IIssue78Interface intOne)
			{
				Task<Issue78TypeOne> getTypeOneTask = Task<Issue78TypeOne>.Factory.StartNew(() => intOne.GetTypeOne());
				Task<Issue78TypeTwo> getTypeTwoTask = Task<Issue78TypeTwo>.Factory.StartNew(() => intOne.GetTypeTwo());

				Issue78TypeOne objOne = getTypeOneTask.Result;
				Issue78TypeTwo objTwo = getTypeTwoTask.Result;
			}
		}

		public class Issue78Tests
		{
			[Fact()]
			public void DoTest()
			{
				Mock<IIssue78Interface> mock = new Mock<IIssue78Interface>();

				Issue78TypeOne expectedResOne = new Issue78TypeOne();
				Issue78TypeTwo expectedResTwo = new Issue78TypeTwo();

				mock.Setup(it => it.GetTypeOne()).Returns(expectedResOne);
				mock.Setup(it => it.GetTypeTwo()).Returns(expectedResTwo);

				Issue78Sut sut = new Issue78Sut();
				sut.TestMethod(mock.Object);

				mock.VerifyAll();
			}
		}

		#endregion

		#region 82

		public class Issue82
		{
			public interface ILogger
			{
				event Action info;
				void add_info(string info);
				void Add_info(string info);
				void remove_info(string info);
				void Remove_info(string info);
			}

			[Fact]
			public void MethodWithAddUnderscoreNamePrefixDoesNotGetMisrecognizedAsEventAccessor()
			{
				var mock = new Mock<ILogger>();
				mock.Setup(x => x.add_info(It.IsAny<string>()));
				mock.Setup(x => x.Add_info(It.IsAny<string>()));

				mock.Object.add_info(string.Empty);
				mock.Object.Add_info(string.Empty);
			}

			[Fact]
			public void MethodWithRemoveUnderscoreNamePrefixDoesNotGetMisrecognizedAsEventAccessor()
			{
				var mock = new Mock<ILogger>();
				mock.Setup(x => x.remove_info(It.IsAny<string>()));
				mock.Setup(x => x.Remove_info(It.IsAny<string>()));

				mock.Object.remove_info(string.Empty);
				mock.Object.Remove_info(string.Empty);
			}
		}

		#endregion

		#region 131

		public class Issue131
		{
			[Fact]
			public void Order_of_setups_should_not_matter_when_setting_up_methods_in_a_hierarchy_of_interfaces()
			{
				var mock = new Mock<Derived>();

				mock.Setup(x => x.Method())
					.Returns("Derived");
				mock.As<BaseA>().Setup(x => x.Method())
					.Returns("BaseA");
				mock.As<BaseB>().Setup(x => x.Method())
					.Returns("BaseB");

				var derived = mock.Object;
				Assert.Equal("Derived", derived.Method());
				Assert.Equal("BaseA", (derived as BaseA).Method());
				Assert.Equal("BaseB", (derived as BaseB).Method());
			}

			public interface BaseA
			{
				string Method();
			}

			public interface BaseB
			{
				string Method();
			}

			public interface Derived : BaseA, BaseB
			{
				new string Method();
			}
		}

		#endregion

		#region 141

		public class Issue141
		{
			[Fact]
			public void MockingDoesNotChangeVirtualnessAndFinalnessOfInheritedInterfaceMethod()
			{
				var actualTypeMethod = typeof(ConcreteClass).GetMethod("Method");
				Assert.True(actualTypeMethod.IsVirtual && actualTypeMethod.IsFinal);

				var mockedTypeMethod = new Mock<ConcreteClass>().Object.GetType().GetMethod("Method");
				Assert.True(mockedTypeMethod.IsVirtual && mockedTypeMethod.IsFinal);
			}

			public interface ISomeInterface
			{
				void Method();
			}

			public class ConcreteClass : ISomeInterface
			{
				public void Method() { }
			}
		}

		#endregion

		#region 156

		public class Issue156
		{
			[Fact]
			public void Test()
			{
				var mock = new Mock<A>(MockBehavior.Strict);

				var actualViaObject = mock.Object.Foo();
				var actualViaInterface = ((IA)mock.Object).Foo();

				Assert.Equal(42, actualViaObject);
				Assert.Equal(42, actualViaInterface);
			}

			public class A : IA
			{
				public int Foo()
				{
					return 42;
				}
			}

			public interface IA
			{
				int Foo();
			}
		}

		#endregion

		#region 157

		public class Issue157
		{
			[Fact]
			public void Test()
			{
				var streamMock = new Mock<Stream>();

				using (var stream = streamMock.Object) { }

				// non-mocked Dispose methods calls Close which is virtual and can thus be verified
				streamMock.Verify(x => x.Close());
			}

			public class Stream : IDisposable
			{
				public void Dispose()
				{
					Close();
				}

				public virtual void Close()
				{
				}
			}
		}

		#endregion

		#region 162

		public class Issue162
		{
			[Fact]
			public void GetSetPropertyThatOverridesGetPropertyRetainsValueSetUpWithMockOf()
			{
				const decimal expectedValue = .14M;
				var i = Mock.Of<B>(b => b.Value == expectedValue);
				Assert.Equal(expectedValue, actual: i.Value);
			}

			public interface A
			{
				decimal? Value { get; }
			}

			public interface B : A
			{
				new decimal? Value { get; set; }
			}
		}

		#endregion

		#region 163

#if FEATURE_SERIALIZATION
		public class Issue163  // see also issue 340 below
		{
			[Fact]
			public void WhenMoqEncountersTypeThatDynamicProxyCannotHandleFallbackToEmptyDefaultValue()
			{
				// First, establish that we're looking at situation involving a type that DynamicProxy
				// cannot handle:
				var proxyGenerator = new ProxyGenerator();
				Assert.Throws<ArgumentException>(() => proxyGenerator.CreateClassProxy<NoDeserializationCtor>());

				// With such a type, Moq should fall back to the empty default value provider:
				var foo = Mock.Of<Foo>();
				Assert.Null(foo.SomeSerializableObject);
			}

			public abstract class Foo
			{
				public abstract NoDeserializationCtor SomeSerializableObject { get; }
			}

			[Serializable]
			public abstract class NoDeserializationCtor : ISerializable
			{
				public virtual void GetObjectData(SerializationInfo info, StreamingContext context) { }
			}

			/// <summary>
			/// The tests in this class document some of Moq's assumptions about
			/// how Castle DynamicProxy handles various correct and incorrect
			/// implementations of `ISerializable`. If any one these tests start
			/// failing, this is a signal that DynamicProxy has changed, and that
			/// Moq might have to be adjusted accordingly.
			/// </summary>
			public class AssumptionsAboutDynamicProxy
			{
				[Theory]
				[InlineData(typeof(CorrectImplementation))]
				public void CanCreateProxyForCorrectImplementaiton(Type classToProxy)
				{
					var proxyGenerator = new ProxyGenerator();
					var proxy = proxyGenerator.CreateClassProxy(classToProxy);
					Assert.NotNull(proxy);
				}

				[Theory]
				[InlineData(typeof(NoSerializableAttribute))]
				[InlineData(typeof(NoSerializableAttributeAndNoDeserializationCtor))]
				[InlineData(typeof(NoSerializableAttributeAndGetObjectDataNotVirtual))]
				public void DoesNotMindPossibleErrorsIfNoSerializableAttribute(Type classToProxy)
				{
					var proxyGenerator = new ProxyGenerator();
					var proxy = proxyGenerator.CreateClassProxy(classToProxy);
					Assert.NotNull(proxy);
				}

				[Theory]
				[InlineData(typeof(NotISerializable))]
				[InlineData(typeof(NotISerializableAndNoDeserializationCtor))]
				[InlineData(typeof(NotISerializableAndGetObjectDataNotVirtual))]
				public void DoesNotMindPossibleErrorsIfNotISerializable(Type classToProxy)
				{
					var proxyGenerator = new ProxyGenerator();
					var proxy = proxyGenerator.CreateClassProxy(classToProxy);
					Assert.NotNull(proxy);
				}


				[Theory]
				[InlineData(typeof(NoDeserializationCtor))]
				public void DoesMindMissingDeserializationCtor(Type classToProxy)
				{
					var proxyGenerator = new ProxyGenerator();
					Assert.Throws<ArgumentException>(() => proxyGenerator.CreateClassProxy(classToProxy));
				}

				[Theory]
				[InlineData(typeof(GetObjectDataNotVirtual))]
				public void DoesMindNonVirtualGetObjectData(Type classToProxy)
				{
					var proxyGenerator = new ProxyGenerator();
					Assert.Throws<ArgumentException>(() => proxyGenerator.CreateClassProxy(classToProxy));
				}

				public abstract class NoSerializableAttribute : ISerializable
				{
					protected NoSerializableAttribute() { }
					protected NoSerializableAttribute(SerializationInfo info, StreamingContext context) { }
					public void GetObjectData(SerializationInfo info, StreamingContext context) { }
				}

				public abstract class NoSerializableAttributeAndNoDeserializationCtor : ISerializable
				{
					public void GetObjectData(SerializationInfo info, StreamingContext context) { }
				}

				public abstract class NoSerializableAttributeAndGetObjectDataNotVirtual : ISerializable
				{
					protected NoSerializableAttributeAndGetObjectDataNotVirtual() { }
					protected NoSerializableAttributeAndGetObjectDataNotVirtual(SerializationInfo info, StreamingContext context) { }
					public virtual void GetObjectData(SerializationInfo info, StreamingContext context) { }
				}

				[Serializable]
				public abstract class CorrectImplementation : ISerializable
				{
					protected CorrectImplementation() { }
					protected CorrectImplementation(SerializationInfo info, StreamingContext context) { }
					public virtual void GetObjectData(SerializationInfo info, StreamingContext context) { }
				}

				[Serializable]
				public abstract class NotISerializable
				{
					protected NotISerializable() { }
					protected NotISerializable(SerializationInfo info, StreamingContext context) { }
					public void GetObjectData(SerializationInfo info, StreamingContext context) { }
				}

				[Serializable]
				public abstract class NotISerializableAndNoDeserializationCtor
				{
					public virtual void GetObjectData(SerializationInfo info, StreamingContext context) { }
				}

				[Serializable]
				public abstract class NotISerializableAndGetObjectDataNotVirtual
				{
					protected NotISerializableAndGetObjectDataNotVirtual() { }
					protected NotISerializableAndGetObjectDataNotVirtual(SerializationInfo info, StreamingContext context) { }
					public virtual void GetObjectData(SerializationInfo info, StreamingContext context) { }
				}

				[Serializable]
				public abstract class NoDeserializationCtor : ISerializable
				{
					public virtual void GetObjectData(SerializationInfo info, StreamingContext context) { }
				}

				[Serializable]
				public abstract class GetObjectDataNotVirtual : ISerializable
				{
					protected GetObjectDataNotVirtual() { }
					protected GetObjectDataNotVirtual(SerializationInfo info, StreamingContext context) { }
					public void GetObjectData(SerializationInfo info, StreamingContext context) { }
				}
			}
		}
#endif

		#endregion

		#region 164

		public class Issue164
		{
			[Fact]
			public void PropertyFails()
			{
				var mock = new Mock<LogWrapper>();
				mock.Setup(o => o.IsDebugEnabled).Returns(false).Verifiable();
				Checker(mock.Object);
				mock.Verify(log => log.IsDebugEnabled, Times.Exactly(1));
			}

			private static void Checker(ILogger log)
			{
				log.Debug("some message");
			}

			public interface ILogger
			{
				bool IsDebugEnabled { get; }

				void Debug(object message);
			}

			public class LogWrapper : ILogger
			{
				public virtual bool IsDebugEnabled
				{
					get { return true; }
				}

				public void Debug(object message)
				{
					if (IsDebugEnabled)
					{
						Console.WriteLine(message);
					}
				}
			}
		}

		#endregion

		#region 166

		public class Issue166
		{
			[Fact]
			public void Events_with_same_name_in_object_graph_can_both_be_raised()
			{
				var mock = new Mock<IFoo> { DefaultValue = DefaultValue.Mock };

				var fooEventRaiseCount = 0;
				mock.Object.Event += (s, e) => ++fooEventRaiseCount;

				var barEventRaiseCount = 0;
				mock.Object.Bar.Event += (s, e) => ++barEventRaiseCount;

				mock.Raise(x => x.Event += null, EventArgs.Empty);
				mock.Raise(x => x.Bar.Event += null, EventArgs.Empty);

				Assert.Equal(1, fooEventRaiseCount);
				Assert.Equal(1, barEventRaiseCount);
			}

			[Fact]
			public void Events_with_different_names_in_object_graph_can_both_be_raised()
			{
				var mock = new Mock<IFoo> { DefaultValue = DefaultValue.Mock };

				var fooEventRaiseCount = 0;
				mock.Object.Event += (s, e) => ++fooEventRaiseCount;

				var barAnotherEventRaiseCount = 0;
				mock.Object.Bar.AnotherEvent += (s, e) => ++barAnotherEventRaiseCount;

				mock.Raise(x => x.Event += null, EventArgs.Empty);
				mock.Raise(x => x.Bar.AnotherEvent += null, EventArgs.Empty);

				Assert.Equal(1, fooEventRaiseCount);
				Assert.Equal(1, barAnotherEventRaiseCount);
			}

			[Fact]
			public void Event_in_child_mock_can_be_raised_if_parent_object_has_no_events()
			{
				var mock = new Mock<IQux>() { DefaultValue = DefaultValue.Mock };

				var quuxEventRaiseCount = 0;
				mock.Object.Quux.Event += (s, e) => ++quuxEventRaiseCount;

				mock.Raise(x => x.Quux.Event += null, EventArgs.Empty);

				Assert.Equal(1, quuxEventRaiseCount);
			}

			public interface IFoo
			{
				IBar Bar { get; }
				event EventHandler Event;
			}

			public interface IBar
			{
				event EventHandler Event;
				event EventHandler AnotherEvent;
			}

			public interface IQux
			{
				IQuux Quux { get; }
			}

			public interface IQuux : IQux
			{
				event EventHandler Event;
			}
		}

		#endregion

		#region 169

		public class Issue169
		{
			[Fact]
			public void Mocks_that_implement_IEnumerable_which_is_not_set_up_can_be_used_without_NullReferenceException()
			{
				// This test makes sure that a mock is usable when it implements IEnumerable,
				// but that interface isn't set up to correctly work. (Moq will attempt to use
				// IEnumerable to generate an ExpressionKey, for example.)

				var mocks = new MockRepository(MockBehavior.Loose);
				var service = mocks.Create<IDependency>();

				var input = mocks.OneOf<IInput>();
				service.Setup(x => x.Run(input)).Returns(1);

				var foo = new Foo(service.Object);
				foo.Calculate(input);
			}

			public interface IDependency
			{
				decimal Run(IInput input);
			}

			public interface IInput : IEnumerable<int>
			{
			}

			public class Foo
			{
				private readonly IDependency dependency;

				public Foo(IDependency dependency)
				{
					this.dependency = dependency;
				}

				public void Calculate(IInput input)
				{
					this.dependency.Run(input);
				}
			}
		}

		#endregion

		#region 175

		public class Issue175
		{
			[Fact]
			public void MoqErrors()
			{
				Mock<ExtendingTypeBase> fake = new Mock<ExtendingTypeBase>(42);
				ExtendingTypeBase realFake = fake.Object;

				// Make sure we're telling the truth that the value is mocked prior to our frobbing
				Assert.Equal(42, realFake.ExtendedValue);

				Frobber frobber = new Frobber(realFake);

				Assert.True(frobber.WasExtendedType);
				// BUGBUG: Seems to have been set back to the default for this type (Moq type is by default LOOSE)
				Assert.Equal(42, frobber.ExtendedTypeValue);  // "BUGBUG: Moq Lost the value and set back to default."
			}

			[Fact]
			public void CSharpIsCoolWithIt()
			{
				ExtendingTypeBase real = new ExtendedConcreteType(42);

				// Make sure we're telling the truth that the value is mocked prior to our frobbing
				Assert.Equal(42, real.ExtendedValue);

				Frobber frobber = new Frobber(real);

				Assert.True(frobber.WasExtendedType);
				Assert.Equal(42, frobber.ExtendedTypeValue);
			}

			[Fact]
			public void MoqErrorsTwo()
			{
				var rootMock = new Mock<ExtendingTypeBase>(42);
				ExtendingTypeBase realObject = rootMock.Object;
				IExtendedType realObjectInterfaceType = rootMock.Object;
				Assert.Equal(42, realObject.ExtendedValue);
				// BUGBUG: Seems to have been set back to the default for this type (Moq type is by default LOOSE)
				Assert.Equal(42, realObjectInterfaceType.ExtendedValue);
			}

			[Fact]
			public void MoqErrorsThree()
			{
				var rootMock = new Mock<ExtendingTypeBase>(42);
				ExtendingTypeBase realObject = rootMock.Object;
				IExtendedType realObjectInterfaceType = rootMock.As<IExtendedType>().Object;
				Assert.Equal(42, realObject.ExtendedValue);
				// BUGBUG: Seems to have been set back to the default for this type (Moq type is by default LOOSE)
				Assert.Equal(42, realObjectInterfaceType.ExtendedValue);
			}

			public class Frobber
			{
				List<ISharedType> internalStore;

				public Frobber(params ISharedType[] inputs)
				{
					// Save the Internal Store
					this.internalStore = new List<ISharedType>(inputs);
				}

				public bool WasExtendedType
				{
					get
					{
						return this.internalStore.First() is IExtendedType;
					}
				}

				public int ExtendedTypeValue
				{
					get
					{
						return ((IExtendedType)this.internalStore.First()).ExtendedValue;
					}
				}
			}

			public class ExtendedConcreteType : ExtendingTypeBase
			{
				public ExtendedConcreteType(int extendedValue) :
					base(extendedValue)
				{
				}

				public override int CommonValue
				{
					get;
					set;
				}
			}

			public interface ISharedType
			{
				int CommonValue { get; set; }
			}

			public interface IExtendedType
			{
				int ExtendedValue { get; set; }
			}

			public abstract class ExtendingTypeBase : ISharedType, IExtendedType
			{
				public ExtendingTypeBase(int extendedValue)
				{
					this.ExtendedValue = extendedValue;
				}

				public abstract int CommonValue { get; set; }
				public int ExtendedValue { get; set; }
			}
		}

		#endregion

		#region #176

		public class Issue176
		{
			public interface ISomeInterface
			{
				TResult DoSomething<TResult>(int anInt);
				int DoSomethingElse(int anInt);
			}

			[Fact]
			public void when_a_mock_doesnt_match_generic_parameters_exception_indicates_generic_parameters()
			{
				var mock = new Mock<ISomeInterface>(MockBehavior.Strict);
				mock.Setup(m => m.DoSomething<int>(0)).Returns(1);

				try
				{
					mock.Object.DoSomething<string>(0);
				}
				catch (MockException exception)
				{
					var genericTypesRE = new Regex(@"\<.*?\>");
					var match = genericTypesRE.Match(exception.Message);

					Assert.True(match.Success);
					Assert.Equal("<string>", match.Captures[0].Value, StringComparer.OrdinalIgnoreCase);
					return;
				}

				Assert.True(false, "No exception was thrown when one should have been");
			}

			[Fact]
			public void when_a_method_doesnt_have_generic_parameters_exception_doesnt_include_brackets()
			{
				var mock = new Mock<ISomeInterface>(MockBehavior.Strict);
				mock.Setup(m => m.DoSomething<int>(0)).Returns(1);

				try
				{
					mock.Object.DoSomethingElse(0);
				}
				catch (MockException exception)
				{
					var genericTypesRE = new Regex(@"\<.*?\>");
					var match = genericTypesRE.Match(exception.Message);

					Assert.False(match.Success);
					return;
				}

				Assert.True(false, "No exception was thrown when one should have been");
			}
		}

		#endregion // #176

		#region #184

		public class Issue184
		{
			public interface ISimpleInterface
			{
				void Method(Guid? g);
			}

			[Fact]
			public void strict_mock_accepts_null_as_nullable_guid_value()
			{
				var mock = new Mock<ISimpleInterface>(MockBehavior.Strict);
				mock.Setup(x => x.Method(It.IsAny<Guid?>()));
				mock.Object.Method(null);
				mock.Verify();
			}
		}

		#endregion // #184

		#region 224

		public class Issue224
		{
			[Fact]
			public void Delegate_invocation_in_Mock_Of_does_not_cause_infinite_loop()
			{
				var infiniteLoopTimeout = TimeSpan.FromSeconds(5);

				var timedOut = !Task.Run(() =>
				{
					var fn = Mock.Of<Func<int>>(f => f() == 42);
					Assert.Equal(42, fn());
				}).Wait(infiniteLoopTimeout);

				Assert.False(timedOut);
			}
		}

		#endregion

		#region 239

		public class Issue239
		{
			[Fact]
			public void PropertyInBaseInterfaceRetainsValueSetUpWitMockOf()
			{
				var i1 = Mock.Of<Interface1>(i => i.ABoolean == true);
				Assert.True(i1.ABoolean);
			}
			[Fact]
			public void RedeclaredPropertyInDerivedInterfaceRetainsValueSetUpWithNewMockAndSetupReturns()
			{
				var i2 = new Mock<Interface2>();
				i2.Setup(i => i.ABoolean).Returns(true);
				Assert.True(i2.Object.ABoolean);
			}
			[Fact]
			public void RedeclaredPropertyInDerivedInterfaceRetainsValueSetUpWithMockOf()
			{
				var i2 = Mock.Of<Interface2>(i => i.ABoolean == true);
				Assert.True(i2.ABoolean);
			}
			[Fact]
			public void RedeclaredPropertyInDerivedInterfaceRetainsValueSetUpWitSetupAllPropertiesAndSetter()
			{
				var i2 = new Mock<Interface2>();
				i2.SetupAllProperties();
				i2.Object.ABoolean = true;
				Assert.True(i2.Object.ABoolean);
			}

			public interface Interface1
			{
				bool ABoolean { get; }
			}
			public interface Interface2 : Interface1
			{
				new bool ABoolean { get; set; }
			}
		}

		#endregion

		#region #252

		public class Issue252
		{
			[Fact]
			public void SetupsWithSameArgumentsInDifferentOrderShouldNotOverwriteEachOther()
			{
				var mock = new Mock<ISimpleInterface>();

				var a = new MyClass();
				var b = new MyClass();

				mock.Setup(m => m.Method(a, b)).Returns(1);
				mock.Setup(m => m.Method(b, a)).Returns(2);

				Assert.Equal(1, mock.Object.Method(a, b));
				Assert.Equal(2, mock.Object.Method(b, a));
			}

			public interface ISimpleInterface
			{
				int Method(MyClass a, MyClass b);
			}

			public class MyClass { }
		}

		#endregion // #252

		#region #273

		public class Issue273
		{
			[Fact]
			public void SystemObjectMethodsShouldWorkInStrictMocks()
			{
				var mock = new Mock<IMyInterface>(MockBehavior.Strict);

				mock.Setup(x => x.Test()).Returns(true);

				Assert.IsType<int>(mock.Object.GetHashCode());
				Assert.IsType<string>(mock.Object.ToString());
				Assert.False(mock.Object.Equals("ImNotTheObject"));
				Assert.True(mock.Object.Equals(mock.Object));
			}

			public interface IMyInterface
			{
				bool Test();
			}
		}

		#endregion // #252

		#region 275

		public class Issue275
		{
			private const int EXPECTED = int.MaxValue;

			[Fact]
			public void Root1Test()
			{
				var mock = Mock.Of<IRoot1>(c => c.Value == EXPECTED);
				Assert.Equal(EXPECTED, mock.Value);
			}

			[Fact]
			public void Derived1Test()
			{
				var mock = Mock.Of<IDerived1>(c => c.Value == EXPECTED);
				Assert.Equal(EXPECTED, mock.Value);
			}

			[Fact]
			public void Implementation1Test()
			{
				var mock = Mock.Of<Implementation1>(c => c.Value == EXPECTED);
				Assert.Equal(EXPECTED, mock.Value);
			}

			[Fact]
			public void Root2Test()
			{
				var mock = Mock.Of<IRoot2>(c => c.Value == EXPECTED);
				Assert.Equal(EXPECTED, mock.Value);
			}

			[Fact]
			public void Derived2Test()
			{
				var mock = Mock.Of<IDerived2>(c => c.Value == EXPECTED);
				Assert.Equal(EXPECTED, mock.Value);
			}

			[Fact]
			public void Implementation2Test()
			{
				var mock = Mock.Of<Implementation2>(c => c.Value == EXPECTED);
				Assert.Equal(EXPECTED, mock.Value);
			}

			public interface IRoot1
			{
				int Value { get; }
			}

			public interface IRoot2
			{
				int Value { get; set; }
			}

			public interface IDerived1 : IRoot1
			{
				new int Value { get; set; }
			}

			public interface IDerived2 : IRoot2
			{
				new int Value { get; set; }
			}

			public class Implementation1 : IDerived1
			{
				public int Value { get; set; }
			}

			public class Implementation2 : IDerived1
			{
				public int Value { get; set; }
			}
		}

		#endregion

		#region 292

		public class Issue292
		{
			// This test is somewhat dangerous, since xUnit cannot catch a StackOverflowException.
			// So if this test fails, and does produce a stack overflow, the whole test run will
			// abort abruptly.
			[Fact]
			public void Conditional_setup_where_condition_involves_own_mock_does_not_cause_stack_overflow()
			{
				var dbResultFound = new Mock<IDataReader>();
				dbResultFound.Setup(_ => _.Read()).Returns(true);
				var dbResultNotFound = new Mock<IDataReader>();
				dbResultNotFound.Setup(_ => _.Read()).Returns(false);

				var command = new Mock<IDbCommand>();
				command.SetupProperty(_ => _.CommandText);
				command.When(() => command.Object.CommandText == "SELECT * FROM TABLE WHERE ID=1")
					   .Setup(_ => _.ExecuteReader())
					   .Returns(dbResultFound.Object);
				command.When(() => command.Object.CommandText == "SELECT * FROM TABLE WHERE ID=2")
					   .Setup(_ => _.ExecuteReader())
					   .Returns(dbResultNotFound.Object);

				command.Object.CommandText = "SELECT * FROM TABLE WHERE ID=1";
				Assert.True(command.Object.ExecuteReader().Read());

				command.Object.CommandText = "SELECT * FROM TABLE WHERE ID=2";
				Assert.False(command.Object.ExecuteReader().Read());
			}

			// We redeclare the following two types from System.Data so we
			// don't need the additional assembly reference (read, "dependency"):

			public interface IDataReader
			{
				bool Read();
			}

			public interface IDbCommand
			{
				string CommandText { get; set; }
				IDataReader ExecuteReader();
			}
		}

		#endregion

		#region 296

		public class Issue296
		{
			[Fact]
			public void Can_subscribe_to_and_raise_abstract_class_event_when_CallBase_true()
			{
				var mock = new Mock<Foo>() { CallBase = true };

				var eventHandlerWasCalled = false;
				mock.Object.SomethingChanged += (object sender, EventArgs e) =>
				{
					eventHandlerWasCalled = true;
				};

				mock.Raise(foo => foo.SomethingChanged += null, EventArgs.Empty);
				Assert.True(eventHandlerWasCalled);
			}

			public abstract class Foo
			{
				public abstract event EventHandler SomethingChanged;
			}
		}

		#endregion

		#region 311

		public sealed class Issue311
		{
			[Fact]
			public void Can_still_provide_default_value_for_vector()
			{
				var mocked = Mock.Of<ITypeWithVector>();

				var vector = mocked.Vector;
				Assert.NotNull(vector);
				Assert.True(vector.GetType().IsArray);
				Assert.Equal(1, vector.GetType().GetArrayRank());
			}

			[Fact]
			public void Can_provide_default_value_for_twodimensional_array_property()
			{
				var mocked = Mock.Of<ITypeWithTwoDimensionalArray>();

				var twoDimensionalArray = mocked.TwoDimensionalArray;
				Assert.NotNull(twoDimensionalArray);
				Assert.True(twoDimensionalArray.GetType().IsArray);
				Assert.Equal(2, twoDimensionalArray.GetType().GetArrayRank());
			}

			[Fact]
			public void Can_provide_default_value_for_threedimensional_arrays()
			{
				var mocked = Mock.Of<ITypeWithThreeDimensionalArray>();

				var threeDimensionalArray = mocked.ThreeDimensionalArray;
				Assert.NotNull(threeDimensionalArray);
				Assert.True(threeDimensionalArray.GetType().IsArray);
				Assert.Equal(3, threeDimensionalArray.GetType().GetArrayRank());
			}

			public interface ITypeWithVector
			{
				int[] Vector { get; }
			}

			public interface ITypeWithTwoDimensionalArray
			{
				int[,] TwoDimensionalArray { get; }
			}

			public interface ITypeWithThreeDimensionalArray
			{
				int[,,] ThreeDimensionalArray { get; }
			}
		}

		#endregion 311

		#region #135

		public class Issue135
		{
			public interface IDoer
			{
				void Foo(Item x, Item y);
			}

			public class Item
			{
				public int Id { get; set; }
			}

			[Fact]
			public void strict_mock_differenciates_multiple_setups_with_same_arguments_in_different_order()
			{
				var aMock = new Mock<IDoer>(MockBehavior.Strict);

				var i1 = new Item { Id = 1 };
				var i2 = new Item { Id = 2 };

				aMock.Setup(m => m.Foo(i1, i2));
				aMock.Setup(m => m.Foo(i2, i1));

				aMock.Object.Foo(i1, i2);
				aMock.Object.Foo(i2, i1);

				aMock.Verify(m => m.Foo(i1, i2), Times.Once());
				aMock.Verify(m => m.Foo(i2, i1), Times.Once());
			}
		}

		#endregion // #135

		#region 328

		public class Issue328
		{
			public struct BadlyHashed<T> : IEquatable<BadlyHashed<T>>
			{
				public static implicit operator BadlyHashed<T>(T value)
				{
					return new BadlyHashed<T>(value);
				}

				private T value;

				public BadlyHashed(T value)
				{
					this.value = value;
				}

				public bool Equals(BadlyHashed<T> other)
				{
					return this.value.Equals(other.value);
				}

				public override bool Equals(object obj)
				{
					return obj is BadlyHashed<T> other && this.Equals(other);
				}

				public override int GetHashCode()
				{
					// This is legal: Equal objects must have equal hash codes,
					// but objects with equal hash codes are not necessarily equal.
					// We are essentially rendering GetHashCode useless for equality
					// comparison, so whoever compares instances of this type will
					// (or should!) end up using the more precise Equals.
					return 0;
				}
			}

			[Fact]
			public void Two_BadlyHashed_instances_with_equal_values_are_equal()
			{
				BadlyHashed<string> a1 = "a", a2 = "a";
				Assert.Equal(a1, a2);
				Assert.Equal(a2, a1);
			}

			[Fact]
			public void Two_BadlyHashed_instances_with_nonequal_values_are_not_equal()
			{
				BadlyHashed<string> a = "a", b = "b";
				Assert.NotEqual(a, b);
				Assert.NotEqual(b, a);
			}

			public interface IMockableService
			{
				void Use(BadlyHashed<string> wrappedString);
			}

			[Fact]
			public void Strict_mock_expecting_calls_with_nonequal_BadlyHashed_values_should_verify_when_called_properly()
			{
				// The above test has already established that `a` and `b` are not equal.
				BadlyHashed<string> a = "a", b = "b";

				// We are setting up two calls in Strict mode, i.e. both calls must happen.
				var mock = new Mock<IMockableService>(MockBehavior.Strict);
				{
					mock.Setup(service => service.Use(a));
					mock.Setup(service => service.Use(b));
				}

				// And they do happen!
				{
					var service = mock.Object;
					service.Use(a);
					service.Use(b);
				}

				// So the following verification should succeed.
				mock.VerifyAll();
			}
		}

		#endregion // #328

		#region 331

		public class Issue331
		{
			[Fact]
			public void Test()
			{
				var mock = new Mock<Foo>();
				IFoo i = mock.Object;
				i.Property = true;
				Assert.True(i.Property);
			}

			public interface IFoo
			{
				bool Property { get; set; }
			}

			public class Foo : IFoo
			{
				public bool Property { get; set; }
			}
		}

		#endregion

		#region 337

		public class Issue337
		{
			[Fact]
			public void Mock_Of_can_setup_null_return_value()
			{
				// The following mock setup might appear redundant; after all, `null` is
				// the default value returned for most reference types. However, this test
				// becomes relevant once Moq starts supporting custom implementations of
				// `IDefaultValueProvider`. Then it might no longer be a given that `null`
				// is the default return value that no one would want to explicitly set up.
				var userProvider = Mock.Of<IUserProvider>(p => p.GetUserByEmail("alice@example.com") == null);
				var user = userProvider.GetUserByEmail("alice@example.com");
				Assert.Null(user);
			}

			public class User { }

			public interface IUserProvider
			{
				User GetUserByEmail(string email);
			}
		}

		#endregion

		#region 340

#if FEATURE_SERIALIZATION
		/// <summary>
		/// These tests check whether the presence of a deserialization ctor and/or a GetObjectData
		/// method alone can fool Moq into assuming that a type is ISerializable, or implements
		/// it incompletely when it isn't ISerializable at all.
		/// </summary>
		public class Issue340  // see also issue 163 above
		{
			[Fact]
			public void ClaimsPrincipal_has_ISerializable_contract_but_is_not_ISerializable()
			{
				var ex = Record.Exception(() => Mock.Of<Repro1>());
				Assert.Null(ex);
			}

			public abstract class Repro1
			{
				public abstract System.Security.Claims.ClaimsPrincipal Principal { get; }
			}

			[Fact]
			public void Foo_has_incomplete_ISerializable_contract_but_is_not_ISerializable()
			{
				var ex = Record.Exception(() => Mock.Of<Repro2>());
				Assert.Null(ex);
			}

			public abstract class Repro2
			{
				public abstract Foo FooProperty { get; }
			}

			[Serializable]
			public class Foo
			{
				public Foo() { }
				protected Foo(SerializationInfo info, StreamingContext context) { }
			}
		}
#endif

#endregion

		#region 383

		public class Issue383
		{
			[Fact]
			public void AsInterface_CanSetupMethodOfInterfaceThatClassDoesNotImplement()
			{
				var mock = new Mock<DoesNotImplementI>();
				mock.As<I>().Setup(x => x.Foo()).Returns(42);
				Assert.Equal(42, (mock.Object as I).Foo());
			}

			[Fact]
			public void AsInterface_CanSetupMethodOfInterfaceThatClassImplementsAsNonVirtual()
			{
				var mock = new Mock<ImplementsI>();
				mock.As<I>().Setup(x => x.Foo()).Returns(42);
				Assert.Equal(42, (mock.Object as I).Foo());
			}

			public class DoesNotImplementI
			{
				public int Foo() => 13;
			}

			public interface I
			{
				int Foo();
			}

			public class ImplementsI : I
			{
				public int Foo() => 13;
			}
		}

		#endregion

		#region 421

		public class Issue421
		{
			[Fact]
			public void SetupShouldAcceptMockedIEnumerableInMethodCall()
			{
				var mock = new Mock<IFoo>();
				var values = new Mock<IEnumerable>().Object;
				mock.Setup(m => m.Bar(values));
			}
			public interface IFoo
			{
				void Bar(IEnumerable values);
			}
		}

		#endregion

		#region 438

		public class Issue438
		{
			[Fact]
			public void SetupAllPropertiesCanSetupSeveralSiblingPropertiesOfTheSameType()
			{
				var resultMock = new Mock<IResult> { DefaultValue = DefaultValue.Mock };
				resultMock.SetupAllProperties();
				var result = resultMock.Object;
				result.Part1.Name = "Foo";
				result.Part2.Name = "Bar";

				Assert.Equal("Foo", result.Part1.Name);
				Assert.Equal("Bar", result.Part2.Name);
			}

			public interface IResult
			{
				ISubResult Part1 { get; }
				ISubResult Part2 { get; }
			}

			public interface ISubResult
			{
				string Name { get; set; }
			}

			[Fact]
			public void SetupAllPropertiesCanDealWithSelfReferencingTypes()
			{
				var mock = new Mock<INode>() { DefaultValue = DefaultValue.Mock };
				mock.SetupAllProperties();
				Assert.NotNull(mock.Object.Parent);
			}

			public interface INode
			{
				INode Parent { get; }
			}

			[Fact]
			public void SetupAllPropertiesCanDealWithMutuallyReferencingTypes()
			{
				var mock = new Mock<IPing>() { DefaultValue = DefaultValue.Mock };
				mock.SetupAllProperties();

				Assert.NotNull(mock.Object.Pong);

				Assert.NotNull(mock.Object.Pong.Ping);
				Assert.NotSame(mock.Object, mock.Object.Pong.Ping);

				Assert.NotNull(mock.Object.Pong.Ping.Pong);
				Assert.NotSame(mock.Object.Pong, mock.Object.Pong.Ping.Pong);
			}

			public interface IPing
			{
				IPong Pong { get; }
			}

			public interface IPong
			{
				IPing Ping { get; }
			}
		}

		#endregion

		#region 448

		public class Issue448
		{
			// A strict mock requires `SetupGet` to provide a return value.
			// If none is provided, a strict mock is expected to throw during the Act stage.
			[Fact]
			public void SetupGet_StrictMockThrowsIfSetupDoesNotProvideAReturnValue()
			{
				var mock = new Mock<Foo>(MockBehavior.Strict);
				mock.SetupGet(f => f.BooleanProperty);
				Assert.Throws<MockException>(() => mock.Object.BooleanProperty);
			}

			// Case 1: Providing a return value directly, via `.Returns(...)`:
			[Fact]
			public void SetupGet_StrictMockIdentifiesReturnsAsProvidingAReturnValue()
			{
				var mock = new Mock<Foo>(MockBehavior.Strict);
				mock.SetupGet(f => f.BooleanProperty).Returns(true);
				Assert.True(mock.Object.BooleanProperty);
			}

			// Case 2: Providing a return value indirectly, via `.CallBase()`:
			[Fact]
			public void SetupGet_StrictMockIdentifiesCallBaseAsProvidingAReturnValue()
			{
				var mock = new Mock<Foo>(MockBehavior.Strict);
				mock.SetupGet(f => f.BooleanProperty).CallBase();
				Assert.True(mock.Object.BooleanProperty);
			}

			public class Foo
			{
				public virtual bool BooleanProperty => true;
			}
		}

		#endregion

		#region 458

		public class Issue458
		{
			[Fact]
			public void Mock_Object_always_returns_same_object_even_when_first_instantiated_through_AsInterface_cast()
			{
				Mock<IFoo> mock = new Mock<Foo>().As<IFoo>();

				object o1 = ((Mock)mock).Object;
				object o2 = mock.Object;

				Assert.Same(o1, o2);
			}

			public interface IFoo { }

			public class Foo : IFoo { }
		}

		#endregion

		#region 464

		#if DESKTOP
		public class Issue464
		{
			[Fact]
			public void Test()
			{
				Mock<MyDbContext> mockDbContext = new Mock<MyDbContext>();
				Mock<DbSet<MyEntity>> mockDbSet = new Mock<DbSet<MyEntity>>();
				mockDbContext.Setup(m => m.Set<MyEntity>()).Returns(mockDbSet.Object);

				var triggerObjectCreation = mockDbContext.Object;

				var exception = Record.Exception(() =>
				{
					mockDbContext.Verify(m => m.SaveChanges(), Times.Once);
				});

				//Should have thrown verify exception because no calls were made
				Assert.IsType<MockException>(exception);
			}

			public partial class MyDbContext : DbContext
			{
				public virtual DbSet<MyEntity> MyEntity { get; set; }
			}

			public class MyEntity { }
		}
		#endif

		#endregion

		#region 469

		public class Issue469
		{
			[Fact]
			public void Mock_Object_should_be_able_to_mock_interface_with_overloaded_generic_methods_with_generic_parameters()
			{
				object dummy = new Mock<IHaveOverloadedGenericMethod>().Object;
			}

			public interface IHaveOverloadedGenericMethod
			{
				void GenericMethod<T>(GenericClass1<T> a);
				void GenericMethod<T>(GenericClass2<T> a);
			}

			public abstract class BaseType<T>
			{
			}

			public class GenericClass1<T> : BaseType<T>
			{
			}

			public class GenericClass2<T> : BaseType<T>
			{
			}
		}

		#endregion

		#region 467

		public class Issue467
		{
			public interface ITest
			{
				int Method();
			}

			[Fact]
			public async Task SetupSequence_is_thread_safe_ie_concurrent_invocations_dont_observe_same_response()
			{
				const int length = 20;

				var m = new Mock<ITest>();

				// Setups the mock to return a sequence of results.
				var seqSetup = m.SetupSequence(x => x.Method());
				for (int i = 1; i <= length; i++)
				{
					seqSetup = seqSetup.Returns(i);
				}

				// Queue to store the invocations of the mock.
				var invocationsQueue = new System.Collections.Concurrent.ConcurrentQueue<int>();

				// We will invoke the mock from two different tasks.
				var t1 = Task.Run(() => RegisterInvocations(m, invocationsQueue));
				var t2 = Task.Run(() => RegisterInvocations(m, invocationsQueue));
				await Task.WhenAll(t1, t2);

				var orderedInvocations = invocationsQueue.OrderBy(x => x).ToArray();

				// The assertion prints the real invocations, you will see duplicates there!
				Assert.Equal(Enumerable.Range(1, length).ToArray(), orderedInvocations);
			}

			// Calls the mock until it returns default(int)
			private static void RegisterInvocations(IMock<ITest> m, System.Collections.Concurrent.ConcurrentQueue<int> invocationsQueue)
			{
				int result;
				do
				{
					result = m.Object.Method();
					if (result != default(int))
					{
						invocationsQueue.Enqueue(result);
					}
				} while (result != default(int));
			}
		}

		#endregion


		#region 526

		#if DESKTOP
		public sealed class Issue526
		{
			[Fact]
			public void Given_EntityConnection_mock_created_with_new_Mock_SetupGet_can_setup_ConnectionString_property()
			{
				var mockConnection = new Mock<EntityConnection>();
				var connection = mockConnection.Object;

				mockConnection.SetupGet(c => c.ConnectionString).Returns("_");


				Assert.Equal("_", connection.ConnectionString);
			}

			[Fact]
			public void Given_EntityConnection_mock_created_with_Mock_Of_SetupGet_can_setup_ConnectionString_property()
			{
				var connection = Mock.Of<EntityConnection>();
				var mockConnection = Mock.Get(connection);

				mockConnection.SetupGet(c => c.ConnectionString).Returns("_");

				Assert.Equal("_", connection.ConnectionString);
			}
		}
		#endif

		#endregion

		#region 557

		public sealed class Issue557
		{
			[Fact]
			public void CallBase_works_when_called_on_AsInterface_mock()
			{
				var mock = new Mock<MyClass>().As<IMyClass>();
				mock.CallBase = true;
				Assert.NotNull(mock.Object.DoSomething());
			}

			public interface IMyClass
			{
				object DoSomething();
			}

			public class MyClass : IMyClass
			{
				public object DoSomething()
				{
					return new object();
				}
			}
		}

		#endregion

		#region 582

		public sealed class Issue582
		{
			public interface IFoo
			{
				void Method();
			}

			public class Bar
			{
			}

			[Fact]
			public void CallBase_has_no_effect_for_methods_of_additional_interfaces()
			{
				var bar = new Mock<Bar>() { CallBase = true };
				var foo = bar.As<IFoo>().Object;

				foo.Method();
			}
		}

		#endregion

		#region 592

		public class Issue592 : IDisposable
		{
			private UnobservedTaskExceptionEventArgs _unobservedEventArgs;

			public Issue592()
			{
				TaskScheduler.UnobservedTaskException += OnUnobservedTaskException;
			}

			private void OnUnobservedTaskException(object sender, UnobservedTaskExceptionEventArgs e)
			{
				_unobservedEventArgs = e;
			}

			[Fact]
			public void ThrowsAsync_does_not_cause_UnobservedTaskException()
			{
				var mock = new Mock<IFoo>();
				mock.Setup(a => a.Foo()).ThrowsAsync(new ArgumentException());
				mock.SetupSequence(a => a.Boo()).ThrowsAsync(new ArgumentException());
			}

			public void Dispose()
			{
				GC.Collect();
				GC.WaitForPendingFinalizers();
				TaskScheduler.UnobservedTaskException -= OnUnobservedTaskException;
				if (_unobservedEventArgs != null && !_unobservedEventArgs.Observed)
				{
					throw _unobservedEventArgs.Exception;
				}
			}

			public interface IFoo
			{
				Task Foo();
				Task<int> Boo();
			}
		}

		#endregion

		#region 593

		public class Issue593
		{
			[Fact]
			public async Task ReturnsAsync_ThrowsAsync_start_delay_timer_at_mock_invocation()
			{
				var mock = new Mock<IFoo>();
				mock.Setup(a => a.Foo()).ThrowsAsync(new ArgumentException(), TimeSpan.FromMilliseconds(100));
				mock.Setup(a => a.Boo()).ReturnsAsync(true, TimeSpan.FromMilliseconds(100));

				//Wait for the delay greater then specified for Foo and Boo setup
				await Task.Delay(200);

				Assert.False(mock.Object.Foo().IsCompleted);
				Assert.False(mock.Object.Boo().IsCompleted);
			}

			public interface IFoo
			{
				Task<bool> Foo();
				Task<bool> Boo();
			}
		}

		#endregion

		#region 605

		public class Issue605
		{
			// Taken from https://github.com/castleproject/Core/issues/339.

			public readonly struct Struct
			{
			}

			public interface IStructByRefConsumer
			{
				void Consume(in Struct message);
			}

			public interface IGenericStructByRefConsumer<T>
			{
				T Consume(in Struct message);
			}

			public interface IStructByValueConsumer
			{
				void Consume(Struct message);
			}

			[Fact]
			public void Struct_ByRef_Moq_Test()
			{
				_ = Mock.Of<IStructByRefConsumer>();
			}

			[Fact(Skip = "Currently not supported due to a bug in the runtime, see https://github.com/dotnet/corefx/issues/29254.")]
			public void Struct_ByRef_Generic_Moq_Test()
			{
				_ = Mock.Of<IGenericStructByRefConsumer<int>>();
			}

			[Fact]
			public void Struct_ByValue_Moq_Test()
			{
				_ = Mock.Of<IStructByValueConsumer>();
			}

			// Moq performs its own System.Reflection.Emit-ting for mocking delegate types,
			// so add some tests that target that:

			public delegate void StructByValueDelegate(Struct message);

			public delegate void StructByRefDelegate(in Struct message);

			[Fact]
			public void Struct_ByValue_Delegate()
			{
				_ = Mock.Of<StructByValueDelegate>();
			}

			[Fact]
			public void Struct_ByRef_Delegate()
			{
				_ = Mock.Of<StructByRefDelegate>();

				// Note: If this test suddenly starts failing, then the CLR / CoreCLR might have tightened
				// is signature matching for delegates; see also the comment in `CastleProxyFactory`.
			}
		}

		#endregion

		#region 652

		public class Issue652
		{
			[Fact]
			public void Callback_delegates_compiled_from_Expression_are_usable_despite_additional_hidden_Closure_parameter()
			{
				Mock<IMyInterface> mock = new Moq.Mock<IMyInterface>();
				var setupAction = mock.Setup(my => my.MyAction(It.IsAny<int>()));
				var setupFunc = mock.Setup(my => my.MyFunc(It.IsAny<int>()));

				int a = 5;
				Expression<Action<int>> callback = x => Console.WriteLine(a);
				Action<int> callbackCompiled = callback.Compile();

				int b = 4;
				Expression<Func<int, int>> funcResult = x => b;
				Func<int, int> funcResultCompiled = funcResult.Compile();

				setupAction.Callback(callbackCompiled); // OK
				setupFunc.Callback(callbackCompiled); // OK
				setupFunc.Returns(funcResultCompiled); // Fail
			}

			public interface IMyInterface
			{
				void MyAction(int x);
				int MyFunc(int x);
			}
		}

		#endregion

		#region 706
		
		public class Issue706
		{
			[Fact]
			public void CallBase_should_not_be_allowed_for_void_delegate_mocks()
			{
				Mock<Action> mock = new Mock<Action>();
				Language.Flow.ISetup<Action> setup = mock.Setup(m => m());
				
				Exception ex = Assert.Throws<NotSupportedException>(() => setup.CallBase());
				Assert.Equal(string.Format(CultureInfo.CurrentCulture, Resources.CallBaseCannotBeUsedWithDelegateMocks), ex.Message);
			}
			
			[Fact]
			public void CallBase_should_not_be_allowed_for_non_void_delegate_mocks()
			{
				Mock<Func<bool>> mock = new Mock<Func<bool>>();
				Language.Flow.ISetup<Func<bool>, bool> setup = mock.Setup(m => m());

				Exception ex = Assert.Throws<NotSupportedException>(() => setup.CallBase());
				Assert.Equal(string.Format(CultureInfo.CurrentCulture, Resources.CallBaseCannotBeUsedWithDelegateMocks), ex.Message);
			}

			[Fact]
			public void CallBase_property_should_not_be_allowed_true_for_delegate_mocks()
			{
				Mock<Action> mock = new Mock<Action>();

				Exception ex = Assert.Throws<NotSupportedException>(() => mock.CallBase = true);
				Assert.Equal(string.Format(CultureInfo.CurrentCulture, Resources.CallBaseCannotBeUsedWithDelegateMocks), ex.Message);
			}

			[Fact]
			public void CallBase_property_should_be_allowed_false_for_delegate_mocks()
			{
				Mock<Action> mock = new Mock<Action>();
				mock.CallBase = false;
				
				Assert.False(mock.CallBase);
			}
		}

		#endregion

		// Old @ Google Code

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
#pragma warning disable 618
		[Obsolete("This test is related to `" + nameof(MatcherAttribute) + "`, which is obsolete.")]
#pragma warning restore 618
		public void UsesCustomMatchersWithGenerics()
		{
			var mock = new Mock<IEvaluateLatest>();

			mock.Setup(e => e.Method(IsEqual.To(5))).Returns(1);
			mock.Setup(e => e.Method(IsEqual.To<int, string>(6, "foo"))).Returns(2);

			Assert.Equal(1, mock.Object.Method(5));
			Assert.Equal(2, mock.Object.Method(6));
		}

		[Obsolete("This class contains matchers using `" + nameof(MatcherAttribute) + "`, which is obsolete.")]
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

		#region #128

		public class Issue128
		{
			[Fact]
			public void That_CallBase_on_interface_should_not_throw_exception()
			{
				var mock = new Mock<IDataServiceFactory>()
				{
					DefaultValue = DefaultValue.Mock,
					CallBase = true
				};

				var service = mock.Object.GetDataService();

				var data = service.GetData();
				var result = data.Sum();

				Assert.Equal( 0, result );
			}

			public interface IDataServiceFactory
			{
				IDataService GetDataService();
			}

			public interface IDataService
			{
				IList<int> GetData();
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

				var e = Assert.Throws<MockException>(() => target.VerifyAll());
				Assert.True(e.IsVerificationError);

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

				public virtual void Bar()
				{
				}
			}

			public void ShouldRenderCustomMessage()
			{
				var foo = new Mock<Foo> { CallBase = true };
				foo.Setup(_ => _.Bar()).Verifiable("Hello");
				foo.Object.Boo();
				var ex = Record.Exception(() => foo.Object.Boo());
				Assert.NotNull(ex);
				Assert.Contains("Hello", ex.Message); // has custom verification error message
				Assert.Contains("once, but was", ex.Message); // has Moq's default message
			}
		}

		#endregion

		#region #160

#if !NETCORE
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
				Assert.Throws<NotSupportedException>(() => mock.Setup(l => l.FirstOrDefault()).Returns("Hello world"));
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
				Assert.True(e.Message.ContainsConsecutiveLines(
					"Configured setups: ",
					"IFoo m => m.Execute(1)",
					"IFoo m => m.Execute(It.IsInRange<Int32>(2, 20, Range.Exclusive))"));
			}

			[Fact]
			public void TestGeneric()
			{
				var mock = new Mock<IFoo>();
				mock.Setup(m => m.Execute<int>(1, 10));
				mock.Setup(m => m.Execute<string>(1, "Foo"));

				mock.Object.Execute(1, 10);

				var e = Assert.Throws<MockException>(() => mock.Verify(m => m.Execute<int>(1, 1)));
				Assert.True(e.Message.ContainsConsecutiveLines(
					"Configured setups: ",
					"IFoo m => m.Execute<Int32>(1, 10)"));
			}

			[Fact]
			public void TestNoSetups()
			{
				var mock = new Mock<IFoo>();

				var e = Assert.Throws<MockException>(() => mock.Verify(m => m.Execute(1)));
				Assert.Contains("No setups configured.", e.Message);

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
				Assert.StartsWith("Invalid verify", e.Message);
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
				new Mock<IFoo>().SetupAllProperties();
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
				var expected = 2;

				var target = new Mock<Foo>();
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

		#region #228

		public class _228
		{
			[Fact]
			public void Test()
			{
				var mock = new Mock<FooBar>() { CallBase = true };
				IFoo foo = mock.Object;
				IBar bar = mock.Object;

				bool fooRaised = false;
				bool barRaised = false;
				foo.Foo += (s, e) => fooRaised = true;
				bar.Bar += (s, e) => barRaised = true;

				mock.Object.RaiseMyEvents();

				Assert.True(fooRaised);
				Assert.True(barRaised);
			}

			public interface IFoo
			{
				event EventHandler Foo;
			}

			public interface IBar
			{
				event EventHandler Bar;
			}

			public class FooBar : IFoo, IBar
			{
				public event EventHandler Foo;
				public event EventHandler Bar;

				public void RaiseMyEvents()
				{
					if (this.Foo != null)
					{
						this.Foo(this, EventArgs.Empty);
					}
					if (this.Bar != null)
					{
						this.Bar(this, EventArgs.Empty);
					}
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
				stream.Object.Read(contents, 0, (int)stream.Object.Length);
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
				instance = mock.Object;
			}

			public interface ITest
			{
				void Do<T1, T2>() where T2 : T1;
			}
		}

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

		#region #325

		public class _325
		{
			[Fact]
			public void SubscribingWorks()
			{
				var target = new Mock<Foo> { CallBase = true };
				target.As<IBar>();

				var bar = (IBar)target.Object;
				var raised = false;
				bar.SomeEvent += (sender, e) => raised = true;

				target.As<IBar>().Raise(b => b.SomeEvent += null, EventArgs.Empty);

				Assert.True(raised);
			}

			[Fact]
			public void UnsubscribingWorks()
			{
				var target = new Mock<Foo> { CallBase = true };
				target.As<IBar>();

				var bar = (IBar)target.Object;
				var raised = false;
				EventHandler handler = (sender, e) => raised = true;
				bar.SomeEvent += handler;
				bar.SomeEvent -= handler;

				target.As<IBar>().Raise(b => b.SomeEvent += null, EventArgs.Empty);

				Assert.False(raised);
			}

			public class Foo
			{
			}

			public interface IBar
			{
				event EventHandler SomeEvent;
			}

		}

		#endregion

		#region #326

#if !NETCORE

		public class _326
		{
			[Fact]
			public void ShouldSupportMockingWinFormsControl()
			{
				var foo = new Mock<System.Windows.Forms.Control>();
				var bar = foo.Object;
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

		#region #250

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

		#region Matcher should work with Convert

		public class MatcherConvertFixture
		{
			public interface IFoo
			{
				string M(long l);
			}

			[Fact]
			public void MatcherDoesNotIgnoreConvert()
			{
				var mock = new Mock<IFoo>(MockBehavior.Strict);
				mock.Setup(x => x.M(int.Parse("2"))).Returns("OK");
				Assert.Equal("OK", mock.Object.M(2L));
			}
		}

		#endregion
	}
}
