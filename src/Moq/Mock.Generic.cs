// Copyright (c) 2007, Clarius Consulting, Manas Technology Solutions, InSTEDD.
// All rights reserved. Licensed under the BSD 3-Clause License; see License.txt.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading;

using Moq.Language;
using Moq.Language.Flow;
using Moq.Properties;

namespace Moq
{
	/// <summary>
	///   Provides a mock implementation of <typeparamref name="T"/>.
	/// </summary>
	/// <typeparam name="T">Type to mock, which can be an interface, a class, or a delegate.</typeparam>
	/// <remarks>
	///   Any interface type can be used for mocking, but for classes, only abstract and virtual members can be mocked.
	///   <para>
	///     The behavior of the mock with regards to the setups and the actual calls is determined by the optional
	///     <see cref = "MockBehavior" /> that can be passed to the<see cref="Mock{T}(MockBehavior)"/> constructor.
	///   </para>
	/// </remarks>
	/// <include file='Mock.Generic.xdoc' path='docs/doc[@for="Mock{T}"]/*'/>
	public partial class Mock<T> : Mock, IMock<T> where T : class
	{
		private static Type[] inheritedInterfaces;
		private static int serialNumberCounter;

		static Mock()
		{
			inheritedInterfaces =
				typeof(T)
				.GetInterfaces()
				.Where(i => ProxyFactory.Instance.IsTypeVisible(i) && !i.IsImport)
				.ToArray();

			serialNumberCounter = 0;
		}

		private T instance;
		private List<Type> additionalInterfaces;
		private Dictionary<Type, object> configuredDefaultValues;
		private object[] constructorArguments;
		private DefaultValueProvider defaultValueProvider;
		private EventHandlerCollection eventHandlers;
		private InvocationCollection invocations;
		private string name;
		private SetupCollection setups;

		private MockBehavior behavior;
		private bool callBase;
		private Switches switches;

#region Ctors

		/// <summary>
		/// Ctor invoked by AsTInterface exclusively.
		/// </summary>
		[SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "skipInitialize")]
		internal Mock(bool skipInitialize)
		{
			// HACK: this is quick hackish. 
			// In order to avoid having an IMock<T> I relevant members 
			// virtual so that As<TInterface> overrides them (i.e. Interceptor).
			// The skipInitialize parameter is not used at all, and it's 
			// just to differentiate this ctor that should do nothing 
			// from the regular ones which initializes the proxy, etc.
		}

		/// <summary>
		///   Initializes an instance of the mock with <see cref="MockBehavior.Default"/> behavior.
		/// </summary>
		/// <include file='Mock.Generic.xdoc' path='docs/doc[@for="Mock{T}.ctor()"]/*'/>
		[SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
		public Mock()
			: this(MockBehavior.Default)
		{
		}

		/// <summary>
		///   Initializes an instance of the mock with <see cref="MockBehavior.Default"/> behavior
		///   and with the given constructor arguments for the class. (Only valid when <typeparamref name="T"/> is a class.)
		/// </summary>
		/// <param name="args">Optional constructor arguments if the mocked type is a class.</param>
		/// <remarks>
		///   The mock will try to find the best match constructor given the constructor arguments,
		///   and invoke that to initialize the instance.This applies only for classes, not interfaces.
		/// </remarks>
		/// <include file='Mock.Generic.xdoc' path='docs/doc[@for="Mock{T}.ctor(object[])"]/*'/>
		[SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
		public Mock(params object[] args)
			: this(MockBehavior.Default, args)
		{
		}

		/// <summary>
		///   Initializes an instance of the mock with the specified <see cref="MockBehavior"/> behavior.
		/// </summary>
		/// <param name="behavior">Behavior of the mock.</param>
		/// <include file='Mock.Generic.xdoc' path='docs/doc[@for="Mock{T}.ctor(MockBehavior)"]/*'/>
		[SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
		public Mock(MockBehavior behavior)
			: this(behavior, new object[0])
		{
		}

		/// <summary>
		///   Initializes an instance of the mock with a specific <see cref="MockBehavior"/> behavior
		///   and with the given constructor arguments for the class.
		/// </summary>
		/// <param name="behavior">Behavior of the mock.</param>
		/// <param name="args">Optional constructor arguments if the mocked type is a class.</param>
		/// <remarks>
		///   The mock will try to find the best match constructor given the constructor arguments,
		///   and invoke that to initialize the instance. This applies only to classes, not interfaces.
		/// </remarks>
		/// <include file='Mock.Generic.xdoc' path='docs/doc[@for="Mock{T}.ctor(MockBehavior,object[])"]/*'/>
		[SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
		public Mock(MockBehavior behavior, params object[] args)
		{
			Guard.IsMockable(typeof(T));

			if (args == null)
			{
				args = new object[] { null };
			}

			this.additionalInterfaces = new List<Type>();
			this.behavior = behavior;
			this.configuredDefaultValues = new Dictionary<Type, object>();
			this.constructorArguments = args;
			this.defaultValueProvider = DefaultValueProvider.Empty;
			this.eventHandlers = new EventHandlerCollection();
			this.invocations = new InvocationCollection(this);
			this.name = CreateUniqueDefaultMockName();
			this.setups = new SetupCollection();
			this.switches = Switches.Default;

			this.CheckParameters();
		}

		/// <summary>
		///   Initializes an instance of the mock using the given constructor call including its
		///   argument values and with a specific <see cref="MockBehavior"/> behavior.
		/// </summary>
		/// <param name="newExpression">Lambda expression that creates an instance of <typeparamref name="T"/> T.</param>
		/// <param name="behavior">Behavior of the mock.</param>
		/// <example>
		/// <code>var mock = new Mock&lt;MyProvider&gt;(() => new MyProvider(someArgument, 25));</code>
		/// </example>
		[SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
		public Mock(Expression<Func<T>> newExpression, MockBehavior behavior = MockBehavior.Strict)
			: this(behavior, Expressions.Visitors.ConstructorCallVisitor.ExtractArgumentValues(newExpression))
		{
		}

		private static string CreateUniqueDefaultMockName()
		{
			var serialNumber = Interlocked.Increment(ref serialNumberCounter);

			var name = new StringBuilder();
			name.Append("Mock<").AppendNameOf(typeof(T)).Append(':').Append(serialNumber).Append('>');
			return name.ToString();
		}

		private void CheckParameters()
		{
			if (this.constructorArguments.Length > 0)
			{
				if (typeof(T).IsInterface)
				{
					throw new ArgumentException(Properties.Resources.ConstructorArgsForInterface);
				}
				if (typeof(T).IsDelegateType())
				{
					throw new ArgumentException(Properties.Resources.ConstructorArgsForDelegate);
				}
			}
		}

#endregion

#region Properties

		/// <inheritdoc/>
		public override MockBehavior Behavior => this.behavior;

		/// <inheritdoc/>
		public override bool CallBase
		{
			get => this.callBase;
			set
			{
				if (value && this.MockedType.IsDelegateType())
				{
					throw new NotSupportedException(string.Format(CultureInfo.CurrentCulture, Resources.CallBaseCannotBeUsedWithDelegateMocks));
				}

				this.callBase = value;
			}
		}

		internal override object[] ConstructorArguments => this.constructorArguments;

		internal override Dictionary<Type, object> ConfiguredDefaultValues => this.configuredDefaultValues;

		/// <summary>
		/// Gets or sets the <see cref="DefaultValueProvider"/> instance that will be used
		/// e. g. to produce default return values for unexpected invocations.
		/// </summary>
		public override DefaultValueProvider DefaultValueProvider
		{
			get => this.defaultValueProvider;
			set => this.defaultValueProvider = value ?? throw new ArgumentNullException(nameof(value));
		}

		internal override DefaultValueProvider AutoSetupPropertiesDefaultValueProvider { get; set; }

		internal override EventHandlerCollection EventHandlers => this.eventHandlers;

		internal override List<Type> AdditionalInterfaces => this.additionalInterfaces;

		internal override InvocationCollection MutableInvocations => this.invocations;

		internal override bool IsObjectInitialized => this.instance != null;

		/// <summary>
		///   Exposes the mocked object instance.
		/// </summary>
		[SuppressMessage("Microsoft.Naming", "CA1716:IdentifiersShouldNotMatchKeywords", MessageId = "Object", Justification = "Exposes the mocked object instance, so it's appropriate.")]
		[SuppressMessage("Microsoft.Naming", "CA1721:PropertyNamesShouldNotMatchGetMethods", Justification = "The public Object property is the only one visible to Moq consumers. The protected member is for internal use only.")]
		public virtual new T Object
		{
			get { return (T)base.Object; }
		}

		/// <summary>
		///   Allows naming of your mocks, so they can be easily identified in error messages (e.g. from failed assertions).
		/// </summary>
		public string Name
		{
			get => this.name;
			set => this.name = value;
		}

		/// <summary>
		///   Returns the name of the mock.
		/// </summary>
		public override string ToString()
		{
			return this.Name;
		}

		private void InitializeInstance()
		{
			// Determine the set of interfaces that the proxy object should additionally implement.
			var additionalInterfaceCount = this.AdditionalInterfaces.Count;
			var interfaces = new Type[1 + additionalInterfaceCount];
			interfaces[0] = typeof(IMocked<T>);
			this.AdditionalInterfaces.CopyTo(0, interfaces, 1, additionalInterfaceCount);

			this.instance = (T)ProxyFactory.Instance.CreateProxy(
				typeof(T),
				this,
				interfaces,
				this.constructorArguments);
		}

		/// <summary>
		/// Returns the mocked object value.
		/// </summary>
		[SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate", Justification = "This is actually the protected virtual implementation of the property Object.")]
		protected override object OnGetObject()
		{
			if (this.instance == null)
			{
				this.InitializeInstance();
			}

			return this.instance;
		}

		internal override Type MockedType
		{
			get { return typeof(T); }
		}

		internal override SetupCollection Setups => this.setups;

		internal override Type TargetType => typeof(T);

		internal override Type[] InheritedInterfaces => Mock<T>.inheritedInterfaces;

		/// <summary>
		/// A set of switches that influence how this mock will operate.
		/// You can opt in or out of certain features via this property.
		/// </summary>
		public override Switches Switches
		{
			get => this.switches;
			set => this.switches = value;
		}

#endregion

#region Setup

		/// <summary>
		///   Specifies a setup on the mocked type for a call to a <see langword="void"/> method.
		/// </summary>
		/// <param name="expression">Lambda expression that specifies the expected method invocation.</param>
		/// <remarks>
		///   If more than one setup is specified for the same method or property,
		///   the latest one wins and is the one that will be executed.
		/// </remarks>
		/// <include file='Mock.Generic.xdoc' path='docs/doc[@for="Mock{T}.Setup"]/*'/>
		[SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "By design")]
		public ISetup<T> Setup(Expression<Action<T>> expression)
		{
			var setup = Mock.Setup(this, expression, null);
			return new VoidSetupPhrase<T>(setup);
		}

		/// <summary>
		///   Specifies a setup on the mocked type for a call to a non-<see langword="void"/> (value-returning) method.
		/// </summary>
		/// <param name="expression">Lambda expression that specifies the method invocation.</param>
		/// <typeparam name="TResult">Type of the return value. Typically omitted as it can be inferred from the expression.</typeparam>
		/// <remarks>
		///   If more than one setup is specified for the same method or property,
		///   the latest one wins and is the one that will be executed.
		/// </remarks>
		/// <include file='Mock.Generic.xdoc' path='docs/doc[@for="Mock{T}.Setup{TResult}"]/*'/>
		[SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "By design")]
		public ISetup<T, TResult> Setup<TResult>(Expression<Func<T, TResult>> expression)
		{
			var setup = Mock.Setup(this, expression, null);
			return new NonVoidSetupPhrase<T, TResult>(setup);
		}

		/// <summary>
		///   Specifies a setup on the mocked type for a call to a property getter.
		/// </summary>
		/// <param name="expression">Lambda expression that specifies the property getter.</param>
		/// <typeparam name="TProperty">Type of the property. Typically omitted as it can be inferred from the expression.</typeparam>
		/// <remarks>
		///   If more than one setup is set for the same property getter,
		///   the latest one wins and is the one that will be executed.
		/// </remarks>
		/// <include file='Mock.Generic.xdoc' path='docs/doc[@for="Mock{T}.SetupGet"]/*'/>
		[SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "By design")]
		public ISetupGetter<T, TProperty> SetupGet<TProperty>(Expression<Func<T, TProperty>> expression)
		{
			var setup = Mock.SetupGet(this, expression, null);
			return new NonVoidSetupPhrase<T, TProperty>(setup);
		}

		/// <summary>
		///   Specifies a setup on the mocked type for a call to a property setter.
		/// </summary>
		/// <param name="setterExpression">The Lambda expression that sets a property to a value.</param>
		/// <typeparam name="TProperty">Type of the property. Typically omitted as it can be inferred from the expression.</typeparam>
		/// <remarks>
		///   If more than one setup is set for the same property setter,
		///   the latest one wins and is the one that will be executed.
		///   <para>
		///     This overloads allows the use of a callback already typed for the property type.
		///   </para>
		/// </remarks>
		/// <include file='Mock.Generic.xdoc' path='docs/doc[@for="Mock{T}.SetupSet{TProperty}"]/*'/>
		public ISetupSetter<T, TProperty> SetupSet<TProperty>(Action<T> setterExpression)
		{
			Guard.NotNull(setterExpression, nameof(setterExpression));
			var expression = ExpressionReconstructor.Instance.ReconstructExpression(setterExpression, this.ConstructorArguments);

			var setup = Mock.SetupSet(this, expression, condition: null);
			return new SetterSetupPhrase<T, TProperty>(setup);
		}

		/// <summary>
		///   Specifies a setup on the mocked type for a call to a property setter.
		/// </summary>
		/// <param name="setterExpression">Lambda expression that sets a property to a value.</param>
		/// <remarks>
		///   If more than one setup is set for the same property setter,
		///   the latest one wins and is the one that will be executed.
		/// </remarks>
		/// <include file='Mock.Generic.xdoc' path='docs/doc[@for="Mock{T}.SetupSet"]/*'/>
		public ISetup<T> SetupSet(Action<T> setterExpression)
		{
			Guard.NotNull(setterExpression, nameof(setterExpression));
			var expression = ExpressionReconstructor.Instance.ReconstructExpression(setterExpression, this.ConstructorArguments);

			var setup = Mock.SetupSet(this, expression, condition: null);
			return new VoidSetupPhrase<T>(setup);
		}

		/// <summary>
		///   Specifies a setup on the mocked type for a call to an event add.
		/// </summary>
		/// <param name="addExpression">Lambda expression that adds an event.</param>
		/// <remarks>
		///   If more than one setup is set for the same event add,
		///   the latest one wins and is the one that will be executed.
		/// </remarks>
		/// <include file='Mock.Generic.xdoc' path='docs/doc[@for="Mock{T}.SetupAdd"]/*'/>
		public ISetup<T> SetupAdd(Action<T> addExpression)
		{
			Guard.NotNull(addExpression, nameof(addExpression));

			var expression = ExpressionReconstructor.Instance.ReconstructExpression(addExpression, this.ConstructorArguments);

			var setup = Mock.SetupAdd(this, expression, condition: null);
			return new VoidSetupPhrase<T>(setup);
		}

		/// <summary>
		///   Specifies a setup on the mocked type for a call to an event 'remove.
		/// </summary>
		/// <param name="removeExpression">Lambda expression that removes an event.</param>
		/// <remarks>
		///   If more than one setup is set for the same event remove,
		///   the latest one wins and is the one that will be executed.
		/// </remarks>
		/// <include file='Mock.Generic.xdoc' path='docs/doc[@for="Mock{T}.SetupRemove"]/*'/>
		public ISetup<T> SetupRemove(Action<T> removeExpression)
		{
			Guard.NotNull(removeExpression, nameof(removeExpression));

			var expression = ExpressionReconstructor.Instance.ReconstructExpression(removeExpression, this.ConstructorArguments);

			var setup = Mock.SetupRemove(this, expression, condition: null);
			return new VoidSetupPhrase<T>(setup);
		}

		/// <summary>
		///   Specifies that the given property should have "property behavior",
		///   meaning that setting its value will cause it to be saved and later returned when the property is requested.
		///   (This is also known as "stubbing".)
		/// </summary>
		/// <param name="property">Property expression to stub.</param>
		/// <typeparam name="TProperty">
		///   Type of the property, inferred from the property expression (does not need to be specified).
		/// </typeparam>
		/// <include file='Mock.Generic.xdoc' path='docs/doc[@for="Mock{T}.SetupProperty(property)"]/*'/>
		[SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "By design")]
		[SuppressMessage("Microsoft.Naming", "CA1716:IdentifiersShouldNotMatchKeywords", MessageId = "Property", Justification = "This sets properties, so it's appropriate.")]
		public Mock<T> SetupProperty<TProperty>(Expression<Func<T, TProperty>> property)
		{
			return this.SetupProperty(property, default(TProperty));
		}

		/// <summary>
		///   Specifies that the given property should have "property behavior",
		///   meaning that setting its value will cause it to be saved and later returned when the property is requested.
		///   This overload allows setting the initial value for the property.
		///   (This is also known as "stubbing".)
		/// </summary>
		/// <param name="property">Property expression to stub.</param>
		/// <param name="initialValue">Initial value for the property.</param>
		/// <typeparam name="TProperty">
		///   Type of the property, inferred from the property expression (does not need to be specified).
		/// </typeparam>
		/// <include file='Mock.Generic.xdoc' path='docs/doc[@for="Mock{T}.SetupProperty(property,initialValue)"]/*'/>
		[SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "By design")]
		[SuppressMessage("Microsoft.Naming", "CA1716:IdentifiersShouldNotMatchKeywords", MessageId = "Property", Justification = "We're setting up a property, so it's appropriate.")]
		public Mock<T> SetupProperty<TProperty>(Expression<Func<T, TProperty>> property, TProperty initialValue)
		{
			Guard.NotNull(property, nameof(property));

			var pi = property.ToPropertyInfo();
			Guard.CanRead(pi);
			Guard.CanWrite(pi);

			TProperty value = initialValue;
			this.SetupGet(property).Returns(() => value);
			Mock.SetupSet(this, property.AssignItIsAny(), condition: null).SetCallbackResponse(new Action<TProperty>(p => value = p));
			return this;
		}

		/// <summary>
		///   Specifies that the all properties on the mock should have "property behavior",
		///   meaning that setting their value will cause them to be saved and later returned when the properties is requested.
		///   (This is also known as "stubbing".)
		///   The default value for each property will be the one generated as specified by the <see cref="Mock.DefaultValue"/>
		///   property for the mock.
		/// </summary>
		/// <remarks>
		///   If the mock's <see cref="Mock.DefaultValue"/> is set to <see cref="DefaultValue.Mock"/>,
		///   the mocked default values will also get all properties setup recursively.
		/// </remarks>
		public Mock<T> SetupAllProperties()
		{
			SetupAllProperties(this);
			return this;
		}

		/// <summary>
		/// Return a sequence of values, once per call.
		/// </summary>
		[SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "By Design")]
		public ISetupSequentialResult<TResult> SetupSequence<TResult>(Expression<Func<T, TResult>> expression)
		{
			var setup = Mock.SetupSequence(this, expression);
			return new SetupSequencePhrase<TResult>(setup);
		}

		/// <summary>
		/// Performs a sequence of actions, one per call.
		/// </summary>
		[SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "By Design")]
		public ISetupSequentialAction SetupSequence(Expression<Action<T>> expression)
		{
			var setup = Mock.SetupSequence(this, expression);
			return new SetupSequencePhrase(setup);
		}

#endregion

#region When

		/// <summary>
		///   Allows setting up a conditional setup.
		///   Conditional setups are only matched by an invocation
		///   when the specified condition evaluates to <see langword="true"/>
		///   at the time when the invocation occurs.
		/// </summary>
		/// <param name="condition">
		///   The condition that should be checked
		///   when a setup is being matched against an invocation.
		/// </param>
		public ISetupConditionResult<T> When(Func<bool> condition)
		{
			return new WhenPhrase<T>(this, new Condition(condition));
		}

#endregion

#region Verify

		/// <summary>
		///   Verifies that a specific invocation matching the given expression was performed on the mock.
		///   Use in conjunction with the default <see cref="MockBehavior.Loose"/>.
		/// </summary>
		/// <param name="expression">Expression to verify.</param>
		/// <exception cref="MockException">The invocation was not performed on the mock.</exception>
		/// <include file='Mock.Generic.xdoc' path='docs/doc[@for="Mock{T}.Verify(expression)"]/*'/>
		[SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "By design")]
		public void Verify(Expression<Action<T>> expression)
		{
			Mock.Verify(this, expression, Times.AtLeastOnce(), null);
		}

		/// <summary>
		///   Verifies that a specific invocation matching the given expression was performed on the mock.
		///   Use in conjunction with the default <see cref="MockBehavior.Loose"/>.
		/// </summary>
		/// <param name="expression">Expression to verify.</param>
		/// <param name="times">The number of times a method is expected to be called.</param>
		/// <exception cref="MockException">
		///   The invocation was not called the number of times specified by <paramref name="times"/>.
		/// </exception>
		[SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "By design")]
		public void Verify(Expression<Action<T>> expression, Times times)
		{
			Mock.Verify(this, expression, times, null);
		}

		/// <summary>
		///   Verifies that a specific invocation matching the given expression was performed on the mock.
		///   Use in conjunction with the default <see cref="MockBehavior.Loose"/>.
		/// </summary>
		/// <param name="expression">Expression to verify.</param>
		/// <param name="times">The number of times a method is expected to be called.</param>
		/// <exception cref="MockException">
		///   The invocation was not called the number of times specified by <paramref name="times"/>.
		/// </exception>
		[SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "By design")]
		public void Verify(Expression<Action<T>> expression, Func<Times> times)
		{
			Verify(expression, times());
		}

		/// <summary>
		///   Verifies that a specific invocation matching the given expression was performed on the mock,
		///   specifying a failure error message.
		///   Use in conjunction with the default <see cref="MockBehavior.Loose"/>.
		/// </summary>
		/// <param name="expression">Expression to verify.</param>
		/// <param name="failMessage">Message to show if verification fails.</param>
		/// <exception cref="MockException">The invocation was not performed on the mock.</exception>
		/// <include file='Mock.Generic.xdoc' path='docs/doc[@for="Mock{T}.Verify(expression,failMessage)"]/*'/>
		[SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "By design")]
		public void Verify(Expression<Action<T>> expression, string failMessage)
		{
			Mock.Verify(this, expression, Times.AtLeastOnce(), failMessage);
		}

		/// <summary>
		///   Verifies that a specific invocation matching the given expression was performed on the mock,
		///   specifying a failure error message.
		///   Use in conjunction with the default <see cref="MockBehavior.Loose"/>.
		/// </summary>
		/// <param name="expression">Expression to verify.</param>
		/// <param name="times">The number of times a method is expected to be called.</param>
		/// <param name="failMessage">Message to show if verification fails.</param>
		/// <exception cref="MockException">
		///   The invocation was not called the number of times specified by <paramref name="times"/>.
		/// </exception>
		[SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "By design")]
		public void Verify(Expression<Action<T>> expression, Times times, string failMessage)
		{
			Mock.Verify(this, expression, times, failMessage);
		}

		/// <summary>
		///   Verifies that a specific invocation matching the given expression was performed on the mock,
		///   specifying a failure error message.
		///   Use in conjunction with the default <see cref="MockBehavior.Loose"/>.
		/// </summary>
		/// <param name="expression">Expression to verify.</param>
		/// <param name="times">The number of times a method is expected to be called.</param>
		/// <param name="failMessage">Message to show if verification fails.</param>
		/// <exception cref="MockException">
		///   The invocation was not called the number of times specified by <paramref name="times"/>.
		/// </exception>
		[SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "By design")]
		public void Verify(Expression<Action<T>> expression, Func<Times> times, string failMessage)
		{
			Mock.Verify(this, expression, times(), failMessage);
		}

		/// <summary>
		///   Verifies that a specific invocation matching the given expression was performed on the mock.
		///   Use in conjunction with the default <see cref="MockBehavior.Loose"/>.
		/// </summary>
		/// <param name="expression">Expression to verify.</param>
		/// <typeparam name="TResult">Type of return value from the expression.</typeparam>
		/// <exception cref="MockException">The invocation was not performed on the mock.</exception>
		/// <include file='Mock.Generic.xdoc' path='docs/doc[@for="Mock{T}.Verify{TResult}(expression)"]/*'/>
		[SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "By design")]
		public void Verify<TResult>(Expression<Func<T, TResult>> expression)
		{
			Mock.Verify(this, expression, Times.AtLeastOnce(), null);
		}

		/// <summary>
		///   Verifies that a specific invocation matching the given expression was performed on the mock.
		///   Use in conjunction with the default <see cref="MockBehavior.Loose"/>.
		/// </summary>
		/// <param name="expression">Expression to verify.</param>
		/// <param name="times">The number of times a method is expected to be called.</param>
		/// <typeparam name="TResult">Type of return value from the expression.</typeparam>
		/// <exception cref="MockException">
		///   The invocation was not called the number of times specified by <paramref name="times"/>.
		/// </exception>
		[SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "By design")]
		public void Verify<TResult>(Expression<Func<T, TResult>> expression, Times times)
		{
			Mock.Verify(this, expression, times, null);
		}

		/// <summary>
		///   Verifies that a specific invocation matching the given expression was performed on the mock.
		///   Use in conjunction with the default <see cref="MockBehavior.Loose"/>.
		/// </summary>
		/// <param name="expression">Expression to verify.</param>
		/// <param name="times">The number of times a method is expected to be called.</param>
		/// <typeparam name="TResult">Type of return value from the expression.</typeparam>
		/// <exception cref="MockException">
		///   The invocation was not called the number of times specified by <paramref name="times"/>.
		/// </exception>
		[SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "By design")]
		public void Verify<TResult>(Expression<Func<T, TResult>> expression, Func<Times> times)
		{
			Mock.Verify(this, expression, times(), null);
		}

		/// <summary>
		///   Verifies that a specific invocation matching the given expression was performed on the mock,
		///   specifying a failure error message.
		/// </summary>
		/// <param name="expression">Expression to verify.</param>
		/// <param name="failMessage">Message to show if verification fails.</param>
		/// <typeparam name="TResult">Type of return value from the expression.</typeparam>
		/// <exception cref="MockException">The invocation was not performed on the mock.</exception>
		/// <include file='Mock.Generic.xdoc' path='docs/doc[@for="Mock{T}.Verify{TResult}(expression,failMessage)"]/*'/>
		[SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "By design")]
		public void Verify<TResult>(Expression<Func<T, TResult>> expression, string failMessage)
		{
			Mock.Verify(this, expression, Times.AtLeastOnce(), failMessage);
		}

		/// <summary>
		///   Verifies that a specific invocation matching the given expression was performed on the mock,
		///   specifying a failure error message.
		/// </summary>
		/// <param name="expression">Expression to verify.</param>
		/// <param name="times">The number of times a method is expected to be called.</param>
		/// <param name="failMessage">Message to show if verification fails.</param>
		/// <typeparam name="TResult">Type of return value from the expression.</typeparam>
		/// <exception cref="MockException">
		///   The invocation was not called the number times specified by <paramref name="times"/>.
		/// </exception>
		[SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "By design")]
		public void Verify<TResult>(Expression<Func<T, TResult>> expression, Times times, string failMessage)
		{
			Mock.Verify(this, expression, times, failMessage);
		}

		/// <summary>
		///   Verifies that a property was read on the mock.
		/// </summary>
		/// <param name="expression">Expression to verify.</param>
		/// <typeparam name="TProperty">
		///   Type of the property to verify. Typically omitted as it can be inferred from the expression's return type.
		/// </typeparam>
		/// <exception cref="MockException">The invocation was not performed on the mock.</exception>
		/// <include file='Mock.Generic.xdoc' path='docs/doc[@for="Mock{T}.VerifyGet(expression)"]/*'/>
		[SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "By design")]
		public void VerifyGet<TProperty>(Expression<Func<T, TProperty>> expression)
		{
			Mock.VerifyGet(this, expression, Times.AtLeastOnce(), null);
		}

		/// <summary>
		///   Verifies that a property was read on the mock.
		/// </summary>
		/// <param name="times">The number of times a method is expected to be called.</param>
		/// <param name="expression">Expression to verify.</param>
		/// <typeparam name="TProperty">
		///   Type of the property to verify. Typically omitted as it can be inferred from the expression's return type.
		/// </typeparam>
		/// <exception cref="MockException">
		///   The invocation was not called the number times specified by <paramref name="times"/>.
		/// </exception>
		[SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "By design")]
		public void VerifyGet<TProperty>(Expression<Func<T, TProperty>> expression, Times times)
		{
			Mock.VerifyGet(this, expression, times, null);
		}

		/// <summary>
		///   Verifies that a property was read on the mock.
		/// </summary>
		/// <param name="times">The number of times a method is expected to be called.</param>
		/// <param name="expression">Expression to verify.</param>
		/// <typeparam name="TProperty">
		///   Type of the property to verify. Typically omitted as it can be inferred from the expression's return type.
		/// </typeparam>
		/// <exception cref="MockException">
		///   The invocation was not called the number times specified by <paramref name="times"/>.
		/// </exception>
		[SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "By design")]
		public void VerifyGet<TProperty>(Expression<Func<T, TProperty>> expression, Func<Times> times)
		{
			VerifyGet(this, expression, times(), null);
		}

		/// <summary>
		///   Verifies that a property was read on the mock, specifying a failure error message.
		/// </summary>
		/// <param name="expression">Expression to verify.</param>
		/// <param name="failMessage">Message to show if verification fails.</param>
		/// <typeparam name="TProperty">
		///   Type of the property to verify. Typically omitted as it can be inferred from the expression's return type.
		/// </typeparam>
		/// <exception cref="MockException">The invocation was not performed on the mock.</exception>
		/// <include file='Mock.Generic.xdoc' path='docs/doc[@for="Mock{T}.VerifyGet(expression,failMessage)"]/*'/>
		[SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "By design")]
		public void VerifyGet<TProperty>(Expression<Func<T, TProperty>> expression, string failMessage)
		{
			Mock.VerifyGet(this, expression, Times.AtLeastOnce(), failMessage);
		}

		/// <summary>
		///   Verifies that a property was read on the mock, specifying a failure error message.
		/// </summary>
		/// <param name="times">The number of times a method is expected to be called.</param>
		/// <param name="expression">Expression to verify.</param>
		/// <param name="failMessage">Message to show if verification fails.</param>
		/// <typeparam name="TProperty">
		///   Type of the property to verify. Typically omitted as it can be inferred from the expression's return type.
		/// </typeparam>
		/// <exception cref="MockException">
		///   The invocation was not called the number times specified by <paramref name="times"/>.
		/// </exception>
		[SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "By design")]
		public void VerifyGet<TProperty>(Expression<Func<T, TProperty>> expression, Times times, string failMessage)
		{
			Mock.VerifyGet(this, expression, times, failMessage);
		}

		/// <summary>
		///   Verifies that a property was read on the mock, specifying a failure error message.
		/// </summary>
		/// <param name="times">The number of times a method is expected to be called.</param>
		/// <param name="expression">Expression to verify.</param>
		/// <param name="failMessage">Message to show if verification fails.</param>
		/// <typeparam name="TProperty">
		///   Type of the property to verify. Typically omitted as it can be inferred from the expression's return type.
		/// </typeparam>
		/// <exception cref="MockException">
		///   The invocation was not called the number times specified by <paramref name="times"/>.
		/// </exception>
		[SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "By design")]
		public void VerifyGet<TProperty>(Expression<Func<T, TProperty>> expression, Func<Times> times, string failMessage)
		{
			VerifyGet(this, expression, times(), failMessage);
		}

		/// <summary>
		///   Verifies that a property was set on the mock.
		/// </summary>
		/// <param name="setterExpression">Expression to verify.</param>
		/// <exception cref="MockException">The invocation was not performed on the mock.</exception>
		/// <include file='Mock.Generic.xdoc' path='docs/doc[@for="Mock{T}.VerifySet(expression)"]/*'/>
		public void VerifySet(Action<T> setterExpression)
		{
			Guard.NotNull(setterExpression, nameof(setterExpression));

			var expression = ExpressionReconstructor.Instance.ReconstructExpression(setterExpression, this.ConstructorArguments);
			Mock.VerifySet(this, expression, Times.AtLeastOnce(), null);
		}

		/// <summary>
		///   Verifies that a property was set on the mock.
		/// </summary>
		/// <param name="times">The number of times a method is expected to be called.</param>
		/// <param name="setterExpression">Expression to verify.</param>
		/// <exception cref="MockException">
		///   The invocation was not called the number of times specified by <paramref name="times"/>.
		/// </exception>
		public void VerifySet(Action<T> setterExpression, Times times)
		{
			Guard.NotNull(setterExpression, nameof(setterExpression));

			var expression = ExpressionReconstructor.Instance.ReconstructExpression(setterExpression, this.ConstructorArguments);
			Mock.VerifySet(this, expression, times, null);
		}

		/// <summary>
		///   Verifies that a property was set on the mock.
		/// </summary>
		/// <param name="times">The number of times a method is expected to be called.</param>
		/// <param name="setterExpression">Expression to verify.</param>
		/// <exception cref="MockException">
		///   The invocation was not called the number of times specified by <paramref name="times"/>.
		/// </exception>
		public void VerifySet(Action<T> setterExpression, Func<Times> times)
		{
			Guard.NotNull(setterExpression, nameof(setterExpression));

			var expression = ExpressionReconstructor.Instance.ReconstructExpression(setterExpression, this.ConstructorArguments);
			Mock.VerifySet(this, expression, times(), null);
		}

		/// <summary>
		///   Verifies that a property was set on the mock, specifying a failure message.
		/// </summary>
		/// <param name="setterExpression">Expression to verify.</param>
		/// <param name="failMessage">Message to show if verification fails.</param>
		/// <exception cref="MockException">The invocation was not performed on the mock.</exception>
		/// <include file='Mock.Generic.xdoc' path='docs/doc[@for="Mock{T}.VerifySet(expression,failMessage)"]/*'/>
		public void VerifySet(Action<T> setterExpression, string failMessage)
		{
			Guard.NotNull(setterExpression, nameof(setterExpression));

			var expression = ExpressionReconstructor.Instance.ReconstructExpression(setterExpression, this.ConstructorArguments);
			Mock.VerifySet(this, expression, Times.AtLeastOnce(), failMessage);
		}

		/// <summary>
		///   Verifies that a property was set on the mock, specifying a failure message.
		/// </summary>
		/// <param name="times">The number of times a method is expected to be called.</param>
		/// <param name="setterExpression">Expression to verify.</param>
		/// <param name="failMessage">Message to show if verification fails.</param>
		/// <exception cref="MockException">
		///   The invocation was not called the number of times specified by <paramref name="times"/>.
		/// </exception>
		public void VerifySet(Action<T> setterExpression, Times times, string failMessage)
		{
			Guard.NotNull(setterExpression, nameof(setterExpression));

			var expression = ExpressionReconstructor.Instance.ReconstructExpression(setterExpression, this.ConstructorArguments);
			Mock.VerifySet(this, expression, times, failMessage);
		}

		/// <summary>
		///   Verifies that a property was set on the mock, specifying a failure message.
		/// </summary>
		/// <param name="times">The number of times a method is expected to be called.</param>
		/// <param name="setterExpression">Expression to verify.</param>
		/// <param name="failMessage">Message to show if verification fails.</param>
		/// <exception cref="MockException">
		///   The invocation was not called the number of times specified by <paramref name="times"/>.
		/// </exception>
		public void VerifySet(Action<T> setterExpression, Func<Times> times, string failMessage)
		{
			Guard.NotNull(setterExpression, nameof(setterExpression));

			var expression = ExpressionReconstructor.Instance.ReconstructExpression(setterExpression, this.ConstructorArguments);
			Mock.VerifySet(this, expression , times(), failMessage);
		}

		/// <summary>
		///   Verifies that an event was added to the mock.
		/// </summary>
		/// <param name="addExpression">Expression to verify.</param>
		/// <exception cref="MockException">The invocation was not performed on the mock.</exception>
		/// <include file='Mock.Generic.xdoc' path='docs/doc[@for="Mock{T}.VerifyAdd(addExpression)"]/*'/>
		public void VerifyAdd(Action<T> addExpression)
		{
			Guard.NotNull(addExpression, nameof(addExpression));

			var expression = ExpressionReconstructor.Instance.ReconstructExpression(addExpression, this.ConstructorArguments);
			Mock.VerifyAdd(this, expression, Times.AtLeastOnce(), null);
		}

		/// <summary>
		///   Verifies that an event was added to the mock.
		/// </summary>
		/// <param name="times">The number of times a method is expected to be called.</param>
		/// <param name="addExpression">Expression to verify.</param>
		/// <exception cref="MockException">
		///   The invocation was not called the number of times specified by <paramref name="times"/>.
		/// </exception>
		public void VerifyAdd(Action<T> addExpression, Times times)
		{
			Guard.NotNull(addExpression, nameof(addExpression));

			var expression = ExpressionReconstructor.Instance.ReconstructExpression(addExpression, this.ConstructorArguments);
			Mock.VerifyAdd(this, expression, times, null);
		}

		/// <summary>
		///   Verifies that an event was added to the mock.
		/// </summary>
		/// <param name="times">The number of times a method is expected to be called.</param>
		/// <param name="addExpression">Expression to verify.</param>
		/// <exception cref="MockException">
		///   The invocation was not called the number of times specified by <paramref name="times"/>.
		/// </exception>
		public void VerifyAdd(Action<T> addExpression, Func<Times> times)
		{
			Guard.NotNull(addExpression, nameof(addExpression));

			var expression = ExpressionReconstructor.Instance.ReconstructExpression(addExpression, this.ConstructorArguments);
			Mock.VerifyAdd(this, expression, times(), null);
		}

		/// <summary>
		///   Verifies that an event was added to the mock, specifying a failure message.
		/// </summary>
		/// <param name="addExpression">Expression to verify.</param>
		/// <param name="failMessage">Message to show if verification fails.</param>
		/// <exception cref="MockException">The invocation was not performed on the mock.</exception>
		/// <include file='Mock.Generic.xdoc' path='docs/doc[@for="Mock{T}.VerifyAdd(addExpression,failMessage)"]/*'/>
		public void VerifyAdd(Action<T> addExpression, string failMessage)
		{
			Guard.NotNull(addExpression, nameof(addExpression));

			var expression = ExpressionReconstructor.Instance.ReconstructExpression(addExpression, this.ConstructorArguments);
			Mock.VerifyAdd(this, expression, Times.AtLeastOnce(), failMessage);
		}

		/// <summary>
		///   Verifies that an event was added to the mock, specifying a failure message.
		/// </summary>
		/// <param name="times">The number of times a method is expected to be called.</param>
		/// <param name="addExpression">Expression to verify.</param>
		/// <param name="failMessage">Message to show if verification fails.</param>
		/// <exception cref="MockException">
		///   The invocation was not called the number of times specified by <paramref name="times"/>.
		/// </exception>
		public void VerifyAdd(Action<T> addExpression, Times times, string failMessage)
		{
			Guard.NotNull(addExpression, nameof(addExpression));

			var expression = ExpressionReconstructor.Instance.ReconstructExpression(addExpression, this.ConstructorArguments);
			Mock.VerifyAdd(this, expression, times, failMessage);
		}

		/// <summary>
		///   Verifies that an event was added to the mock, specifying a failure message.
		/// </summary>
		/// <param name="times">The number of times a method is expected to be called.</param>
		/// <param name="addExpression">Expression to verify.</param>
		/// <param name="failMessage">Message to show if verification fails.</param>
		/// <exception cref="MockException">
		///   The invocation was not called the number of times specified by <paramref name="times"/>.
		/// </exception>
		public void VerifyAdd(Action<T> addExpression, Func<Times> times, string failMessage)
		{
			Guard.NotNull(addExpression, nameof(addExpression));

			var expression = ExpressionReconstructor.Instance.ReconstructExpression(addExpression, this.ConstructorArguments);
			Mock.VerifyAdd(this, expression, times(), failMessage);
		}

		/// <summary>
		///   Verifies that an event was removed from the mock.
		/// </summary>
		/// <param name="removeExpression">Expression to verify.</param>
		/// <exception cref="MockException">The invocation was not performed on the mock.</exception>
		/// <include file='Mock.Generic.xdoc' path='docs/doc[@for="Mock{T}.VerifyRemove(removeExpression)"]/*'/>
		public void VerifyRemove(Action<T> removeExpression)
		{
			Guard.NotNull(removeExpression, nameof(removeExpression));

			var expression = ExpressionReconstructor.Instance.ReconstructExpression(removeExpression, this.ConstructorArguments);
			Mock.VerifyRemove(this, expression, Times.AtLeastOnce(), null);
		}

		/// <summary>
		///   Verifies that an event was removed from the mock.
		/// </summary>
		/// <param name="times">The number of times a method is expected to be called.</param>
		/// <param name="removeExpression">Expression to verify.</param>
		/// <exception cref="MockException">
		///   The invocation was not called the number of times specified by <paramref name="times"/>.
		/// </exception>
		public void VerifyRemove(Action<T> removeExpression, Times times)
		{
			Guard.NotNull(removeExpression, nameof(removeExpression));

			var expression = ExpressionReconstructor.Instance.ReconstructExpression(removeExpression, this.ConstructorArguments);
			Mock.VerifyRemove(this, expression, times, null);
		}

		/// <summary>
		///   Verifies that an event was removed from the mock.
		/// </summary>
		/// <param name="times">The number of times a method is expected to be called.</param>
		/// <param name="removeExpression">Expression to verify.</param>
		/// <exception cref="MockException">
		///   The invocation was not called the number of times specified by <paramref name="times"/>.
		/// </exception>
		public void VerifyRemove(Action<T> removeExpression, Func<Times> times)
		{
			Guard.NotNull(removeExpression, nameof(removeExpression));

			var expression = ExpressionReconstructor.Instance.ReconstructExpression(removeExpression, this.ConstructorArguments);
			Mock.VerifyRemove(this, expression, times(), null);
		}

		/// <summary>
		///   Verifies that an event was removed from the mock, specifying a failure message.
		/// </summary>
		/// <param name="removeExpression">Expression to verify.</param>
		/// <param name="failMessage">Message to show if verification fails.</param>
		/// <exception cref="MockException">The invocation was not performed on the mock.</exception>
		/// <include file='Mock.Generic.xdoc' path='docs/doc[@for="Mock{T}.VerifyRemove(removeExpression,failMessage)"]/*'/>
		public void VerifyRemove(Action<T> removeExpression, string failMessage)
		{
			Guard.NotNull(removeExpression, nameof(removeExpression));

			var expression = ExpressionReconstructor.Instance.ReconstructExpression(removeExpression, this.ConstructorArguments);
			Mock.VerifyRemove(this, expression, Times.AtLeastOnce(), failMessage);
		}

		/// <summary>
		///   Verifies that an event was removed from the mock, specifying a failure message.
		/// </summary>
		/// <param name="times">The number of times a method is expected to be called.</param>
		/// <param name="removeExpression">Expression to verify.</param>
		/// <param name="failMessage">Message to show if verification fails.</param>
		/// <exception cref="MockException">
		///   The invocation was not called the number of times specified by <paramref name="times"/>.
		/// </exception>
		public void VerifyRemove(Action<T> removeExpression, Times times, string failMessage)
		{
			Guard.NotNull(removeExpression, nameof(removeExpression));

			var expression = ExpressionReconstructor.Instance.ReconstructExpression(removeExpression, this.ConstructorArguments);
			Mock.VerifyRemove(this, expression, times, failMessage);
		}

		/// <summary>
		///   Verifies that an event was removed from the mock, specifying a failure message.
		/// </summary>
		/// <param name="times">The number of times a method is expected to be called.</param>
		/// <param name="removeExpression">Expression to verify.</param>
		/// <param name="failMessage">Message to show if verification fails.</param>
		/// <exception cref="MockException">
		///   The invocation was not called the number of times specified by <paramref name="times"/>.
		/// </exception>
		public void VerifyRemove(Action<T> removeExpression, Func<Times> times, string failMessage)
		{
			Guard.NotNull(removeExpression, nameof(removeExpression));

			var expression = ExpressionReconstructor.Instance.ReconstructExpression(removeExpression, this.ConstructorArguments);
			Mock.VerifyRemove(this, expression, times(), failMessage);
		}

		/// <summary>
		/// Verifies that there were no calls other than those already verified.
		/// </summary>
		/// <exception cref="MockException">There was at least one invocation not previously verified.</exception>
		public void VerifyNoOtherCalls()
		{
			Mock.VerifyNoOtherCalls(this);
		}

#endregion

#region Raise

		/// <summary>
		///   Raises the event referenced in <paramref name="eventExpression"/> using the given <paramref name="args"/> argument.
		/// </summary>
		/// <exception cref="ArgumentException">
		///   The <paramref name="args"/> argument is invalid for the target event invocation,
		///   or the <paramref name="eventExpression"/> is not an event attach or detach expression.
		/// </exception>
		/// <include file='Mock.Generic.xdoc' path='docs/doc[@for="Mock{T}.Raise"]/*'/>
		[SuppressMessage("Microsoft.Design", "CA1030:UseEventsWhereAppropriate", Justification = "Raises the event, rather than being one.")]
		public void Raise(Action<T> eventExpression, EventArgs args)
		{
			Mock.RaiseEvent(this, eventExpression, new object[] { this.Object, args });
		}

		/// <summary>
		///   Raises the event referenced in <paramref name="eventExpression"/> using the given <paramref name="args"/> argument for a non-<see cref="EventHandler"/>-typed event.
		/// </summary>
		/// <exception cref="ArgumentException">
		///   The <paramref name="args"/> arguments are invalid for the target event invocation,
		///   or the <paramref name="eventExpression"/> is not an event attach or detach expression.
		/// </exception>
		/// <include file='Mock.Generic.xdoc' path='docs/doc[@for="Mock{T}.Raise(args)"]/*'/>
		[SuppressMessage("Microsoft.Design", "CA1030:UseEventsWhereAppropriate", Justification = "Raises the event, rather than being one.")]
		public void Raise(Action<T> eventExpression, params object[] args)
		{
			Mock.RaiseEvent(this, eventExpression, args);
		}

#endregion
	}
}
