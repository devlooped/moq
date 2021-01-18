// Copyright (c) 2007, Clarius Consulting, Manas Technology Solutions, InSTEDD, and Contributors.
// All rights reserved. Licensed under the BSD 3-Clause License; see License.txt.

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;

using Moq.Properties;

namespace Moq
{
	/// <summary>
	/// Utility factory class to use to construct multiple 
	/// mocks when consistent verification is 
	/// desired for all of them.
	/// </summary>
	/// <remarks>
	/// If multiple mocks will be created during a test, passing 
	/// the desired <see cref="MockBehavior"/> (if different than the 
	/// <see cref="MockBehavior.Default"/> or the one 
	/// passed to the factory constructor) and later verifying each
	/// mock can become repetitive and tedious.
	/// <para>
	/// This factory class helps in that scenario by providing a 
	/// simplified creation of multiple mocks with a default 
	/// <see cref="MockBehavior"/> (unless overridden by calling
	/// <see cref="Create{T}(MockBehavior)"/>) and posterior verification.
	/// </para>
	/// </remarks>
	/// <example group="factory">
	/// The following is a straightforward example on how to 
	/// create and automatically verify strict mocks using a <see cref="MockFactory"/>:
	/// <code>
	/// var factory = new MockFactory(MockBehavior.Strict);
	/// 
	/// var foo = factory.Create&lt;IFoo&gt;();
	/// var bar = factory.Create&lt;IBar&gt;();
	/// 
	///	// no need to call Verifiable() on the setup 
	///	// as we'll be validating all of them anyway.
	/// foo.Setup(f => f.Do());
	/// bar.Setup(b => b.Redo());
	/// 
	///	// exercise the mocks here
	/// 
	/// factory.VerifyAll(); 
	/// // At this point all setups are already checked 
	/// // and an optional MockException might be thrown. 
	/// // Note also that because the mocks are strict, any invocation 
	/// // that doesn't have a matching setup will also throw a MockException.
	/// </code>
	/// The following examples shows how to setup the factory 
	/// to create loose mocks and later verify only verifiable setups:
	/// <code>
	/// var factory = new MockFactory(MockBehavior.Loose);
	/// 
	/// var foo = factory.Create&lt;IFoo&gt;();
	/// var bar = factory.Create&lt;IBar&gt;();
	/// 
	/// // this setup will be verified when we verify the factory
	/// foo.Setup(f => f.Do()).Verifiable();
	/// 	
	/// // this setup will NOT be verified 
	/// foo.Setup(f => f.Calculate());
	/// 	
	/// // this setup will be verified when we verify the factory
	/// bar.Setup(b => b.Redo()).Verifiable();
	/// 
	///	// exercise the mocks here
	///	// note that because the mocks are Loose, members 
	///	// called in the interfaces for which no matching
	///	// setups exist will NOT throw exceptions, 
	///	// and will rather return default values.
	///	
	/// factory.Verify();
	/// // At this point verifiable setups are already checked 
	/// // and an optional MockException might be thrown.
	/// </code>
	/// The following examples shows how to setup the factory with a 
	/// default strict behavior, overriding that default for a 
	/// specific mock:
	/// <code>
	/// var factory = new MockFactory(MockBehavior.Strict);
	/// 
	/// // this particular one we want loose
	/// var foo = factory.Create&lt;IFoo&gt;(MockBehavior.Loose);
	/// var bar = factory.Create&lt;IBar&gt;();
	/// 
	/// // specify setups
	/// 
	///	// exercise the mocks here
	///	
	/// factory.Verify();
	/// </code>
	/// </example>
	/// <seealso cref="MockBehavior"/>
	[EditorBrowsable(EditorBrowsableState.Never)]
	[Obsolete("This class has been renamed to MockRepository. MockFactory will be retired in v5.", false)]
	public partial class MockFactory
	{
		List<Mock> mocks = new List<Mock>();
		MockBehavior defaultBehavior;
		DefaultValueProvider defaultValueProvider;
		private Switches switches;

		/// <summary>
		/// Initializes the factory with the given <paramref name="defaultBehavior"/> 
		/// for newly created mocks from the factory.
		/// </summary>
		/// <param name="defaultBehavior">The behavior to use for mocks created 
		/// using the <see cref="Create{T}()"/> factory method if not overridden
		/// by using the <see cref="Create{T}(MockBehavior)"/> overload.</param>
		public MockFactory(MockBehavior defaultBehavior)
		{
			this.defaultBehavior = defaultBehavior;
			this.defaultValueProvider = DefaultValueProvider.Empty;
			this.switches = Switches.Default;
		}

		/// <summary>
		///   Gets the default <see cref="MockBehavior"/> of mocks created by this repository.
		/// </summary>
		internal MockBehavior Behavior => this.defaultBehavior;

		/// <summary>
		/// Whether the base member virtual implementation will be called 
		/// for mocked classes if no setup is matched. Defaults to <see langword="false"/>.
		/// </summary>
		public bool CallBase { get; set; }

		/// <summary>
		/// Specifies the behavior to use when returning default values for 
		/// unexpected invocations on loose mocks.
		/// </summary>
		public DefaultValue DefaultValue
		{
			get
			{
				return this.DefaultValueProvider.Kind;
			}
			set
			{
				this.DefaultValueProvider = value switch
				{
					DefaultValue.Empty => DefaultValueProvider.Empty,
					DefaultValue.Mock  => DefaultValueProvider.Mock,
					_                  => throw new ArgumentOutOfRangeException(nameof(value)),
				};
			}
		}

		/// <summary>
		/// Gets or sets the <see cref="Moq.DefaultValueProvider"/> instance that will be used
		/// e. g. to produce default return values for unexpected invocations.
		/// </summary>
		public DefaultValueProvider DefaultValueProvider
		{
			get => this.defaultValueProvider;
			set => this.defaultValueProvider = value ?? throw new ArgumentNullException(nameof(value));
		}

		/// <summary>
		/// Gets the mocks that have been created by this factory and 
		/// that will get verified together.
		/// </summary>
		protected internal IEnumerable<Mock> Mocks { get { return mocks; } }

		/// <summary>
		/// A set of switches that influence how mocks created by this factory will operate.
		/// You can opt in or out of certain features via this property.
		/// </summary>
		public Switches Switches
		{
			get => this.switches;
			set => this.switches = value;
		}

		/// <summary>
		/// Creates a new mock with the default <see cref="MockBehavior"/> 
		/// specified at factory construction time.
		/// </summary>
		/// <typeparam name="T">Type to mock.</typeparam>
		/// <returns>A new <see cref="Mock{T}"/>.</returns>
		/// <example ignore="true">
		/// <code>
		/// var factory = new MockFactory(MockBehavior.Strict);
		/// 
		/// var foo = factory.Create&lt;IFoo&gt;();
		/// // use mock on tests
		/// 
		/// factory.VerifyAll();
		/// </code>
		/// </example>
		public Mock<T> Create<T>()
			where T : class
		{
			return CreateMock<T>(defaultBehavior, new object[0]);
		}

		/// <summary>
		/// Creates a new mock with the default <see cref="MockBehavior"/> 
		/// specified at factory construction time and with the 
		/// the given constructor arguments for the class.
		/// </summary>
		/// <remarks>
		/// The mock will try to find the best match constructor given the 
		/// constructor arguments, and invoke that to initialize the instance. 
		/// This applies only to classes, not interfaces.
		/// </remarks>
		/// <typeparam name="T">Type to mock.</typeparam>
		/// <param name="args">Constructor arguments for mocked classes.</param>
		/// <returns>A new <see cref="Mock{T}"/>.</returns>
		/// <example ignore="true">
		/// <code>
		/// var factory = new MockFactory(MockBehavior.Default);
		/// 
		/// var mock = factory.Create&lt;MyBase&gt;("Foo", 25, true);
		/// // use mock on tests
		/// 
		/// factory.Verify();
		/// </code>
		/// </example>
		public Mock<T> Create<T>(params object[] args)
			where T : class
		{
			// "fix" compiler picking this overload instead of 
			// the one receiving the mock behavior.
			if (args != null && args.Length > 0 && args[0] is MockBehavior)
			{
				return CreateMock<T>((MockBehavior)args[0], args.Skip(1).ToArray());
			}

			return CreateMock<T>(defaultBehavior, args);
		}

		/// <summary>
		/// Creates a new mock with the given <paramref name="behavior"/>.
		/// </summary>
		/// <typeparam name="T">Type to mock.</typeparam>
		/// <param name="behavior">Behavior to use for the mock, which overrides 
		/// the default behavior specified at factory construction time.</param>
		/// <returns>A new <see cref="Mock{T}"/>.</returns>
		/// <example group="factory">
		/// The following example shows how to create a mock with a different 
		/// behavior to that specified as the default for the factory:
		/// <code>
		/// var factory = new MockFactory(MockBehavior.Strict);
		/// 
		/// var foo = factory.Create&lt;IFoo&gt;(MockBehavior.Loose);
		/// </code>
		/// </example>
		public Mock<T> Create<T>(MockBehavior behavior)
			where T : class
		{
			return CreateMock<T>(behavior, new object[0]);
		}

		/// <summary>
		/// Creates a new mock with the given <paramref name="behavior"/> 
		/// and with the given constructor arguments for the class.
		/// </summary>
		/// <remarks>
		/// The mock will try to find the best match constructor given the 
		/// constructor arguments, and invoke that to initialize the instance. 
		/// This applies only to classes, not interfaces.
		/// </remarks>
		/// <typeparam name="T">Type to mock.</typeparam>
		/// <param name="behavior">Behavior to use for the mock, which overrides 
		/// the default behavior specified at factory construction time.</param>
		/// <param name="args">Constructor arguments for mocked classes.</param>
		/// <returns>A new <see cref="Mock{T}"/>.</returns>
		/// <example group="factory">
		/// The following example shows how to create a mock with a different 
		/// behavior to that specified as the default for the factory, passing 
		/// constructor arguments:
		/// <code>
		/// var factory = new MockFactory(MockBehavior.Default);
		/// 
		/// var mock = factory.Create&lt;MyBase&gt;(MockBehavior.Strict, "Foo", 25, true);
		/// </code>
		/// </example>
		public Mock<T> Create<T>(MockBehavior behavior, params object[] args)
			where T : class
		{
			return CreateMock<T>(behavior, args);
		}

		/// <summary>
		/// Creates an instance of the mock using the given constructor call including its
		/// argument values and with a specific <see cref="MockBehavior"/> behavior.
		/// </summary>
		/// <typeparam name="T">Type to mock.</typeparam>
		/// <param name="newExpression">Lambda expression that creates an instance of <typeparamref name="T"/>.</param>
		/// <param name="behavior">Behavior of the mock.</param>
		/// <returns>A new <see cref="Mock{T}"/>.</returns>
		/// <example ignore="true">
		/// <code>
		/// var factory = new MockFactory(MockBehavior.Default);
		/// 
		/// var mock = factory.Create&lt;MyClass&gt;(() => new MyClass("Foo", 25, true), MockBehavior.Loose);
		/// // use mock on tests
		/// 
		/// factory.Verify();
		/// </code>
		/// </example>
		public Mock<T> Create<T>(Expression<Func<T>> newExpression, MockBehavior behavior = MockBehavior.Default)
			where T : class
		{
			return Create<T>(behavior, Expressions.Visitors.ConstructorCallVisitor.ExtractArgumentValues(newExpression));
		}

		/// <summary>
		/// Implements creation of a new mock within the factory.
		/// </summary>
		/// <typeparam name="T">Type to mock.</typeparam>
		/// <param name="behavior">The behavior for the new mock.</param>
		/// <param name="args">Optional arguments for the construction of the mock.</param>
		protected virtual Mock<T> CreateMock<T>(MockBehavior behavior, object[] args)
			where T : class
		{
			var mock = new Mock<T>(behavior, args);
			mocks.Add(mock);

			mock.CallBase = this.CallBase;
			mock.DefaultValueProvider = this.DefaultValueProvider;
			mock.Switches = this.switches;

			return mock;
		}

		/// <summary>
		/// Verifies all verifiable setups on all mocks created by this factory.
		/// </summary>
		/// <seealso cref="Mock.Verify()"/>
		/// <exception cref="MockException">One or more mocks had setups that were not satisfied.</exception>
		public virtual void Verify()
		{
			VerifyMocks(verifiable => verifiable.Verify());
		}

		/// <summary>
		/// Verifies all setups on all mocks created by this factory.
		/// </summary>
		/// <seealso cref="Mock.Verify()"/>
		/// <exception cref="MockException">One or more mocks had setups that were not satisfied.</exception>
		public virtual void VerifyAll()
		{
			VerifyMocks(verifiable => verifiable.VerifyAll());
		}

		/// <summary>
		/// Calls <see cref="Mock{T}.VerifyNoOtherCalls()"/> on all mocks created by this factory.
		/// </summary>
		/// <seealso cref="Mock{T}.VerifyNoOtherCalls()"/>
		/// <exception cref="MockException">One or more mocks had invocations that were not verified.</exception>
		public void VerifyNoOtherCalls()
		{
			VerifyMocks(mock => Mock.VerifyNoOtherCalls(mock));
		}

		/// <summary>
		/// Invokes <paramref name="verifyAction"/> for each mock
		/// in <see cref="Mocks"/>, and accumulates the resulting
		/// verification exceptions that might be
		/// thrown from the action.
		/// </summary>
		/// <param name="verifyAction">The action to execute against 
		/// each mock.</param>
		protected virtual void VerifyMocks(Action<Mock> verifyAction)
		{
			Guard.NotNull(verifyAction, nameof(verifyAction));

			var errors = new List<MockException>();

			foreach (var mock in mocks)
			{
				try
				{
					verifyAction(mock);
				}
				catch (MockException error) when (error.IsVerificationError)
				{
					errors.Add(error);
				}
			}

			if (errors.Count > 0)
			{
				throw MockException.Combined(errors, preamble: Resources.VerificationErrorsOfMockRepository);
			}
		}
	}
}
