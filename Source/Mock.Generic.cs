//Copyright (c) 2007, Moq Team 
//http://code.google.com/p/moq/
//All rights reserved.

//Redistribution and use in source and binary forms, 
//with or without modification, are permitted provided 
//that the following conditions are met:

//    * Redistributions of source code must retain the 
//    above copyright notice, this list of conditions and 
//    the following disclaimer.

//    * Redistributions in binary form must reproduce 
//    the above copyright notice, this list of conditions 
//    and the following disclaimer in the documentation 
//    and/or other materials provided with the distribution.

//    * Neither the name of the Moq Team nor the 
//    names of its contributors may be used to endorse 
//    or promote products derived from this software 
//    without specific prior written permission.

//THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND 
//CONTRIBUTORS "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, 
//INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF 
//MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE 
//DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR 
//CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, 
//SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, 
//BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR 
//SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS 
//INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, 
//WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING 
//NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE 
//OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF 
//SUCH DAMAGE.

//[This is the BSD license, see
// http://www.opensource.org/licenses/bsd-license.php]

using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Castle.Core.Interceptor;
using Castle.DynamicProxy;
using Moq.Language.Flow;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics;
using System.ComponentModel;

namespace Moq
{
	/// <include file='Mock.Generic.xdoc' path='docs/doc[@for="Mock{T}"]/*'/>
	public partial class Mock<T> : Mock
		where T : class
	{
		static readonly ProxyGenerator generator = new ProxyGenerator();
		T instance;
		object[] constructorArguments;

		#region Ctors

		/// <summary>
		/// Ctor invoked by AsTInterface exclusively.
		/// </summary>
		private Mock(bool skipInitialize)
		{
			// HACK: this is quick hackish. 
			// In order to avoid having an IMock<T> I relevant members 
			// virtual so that As<TInterface> overrides them (i.e. Interceptor).
			// The skipInitialize parameter is not used at all, and it's 
			// just to differentiate this ctor that should do nothing 
			// from the regular ones which initializes the proxy, etc.
		}

		/// <include file='Mock.Generic.xdoc' path='docs/doc[@for="Mock{T}.ctor()"]/*'/>
		public Mock() : this(MockBehavior.Default) { }

		/// <include file='Mock.Generic.xdoc' path='docs/doc[@for="Mock{T}.ctor(object[])"]/*'/>
		public Mock(params object[] args) : this(MockBehavior.Default, args) { }

		/// <include file='Mock.Generic.xdoc' path='docs/doc[@for="Mock{T}.ctor(MockBehavior)"]/*'/>
		public Mock(MockBehavior behavior) : this(behavior, new object[0]) { }

		/// <include file='Mock.Generic.xdoc' path='docs/doc[@for="Mock{T}.ctor(MockBehavior,object[])"]/*'/>
		public Mock(MockBehavior behavior, params object[] args)
		{
			if (args == null) args = new object[] { null };

			this.Behavior = behavior;
			this.Interceptor = new Interceptor(behavior, typeof(T), this);
			this.constructorArguments = args;
			this.ImplementedInterfaces.Add(typeof(IMocked<T>));

			CheckParameters();
		}

		private void CheckParameters()
		{
			if (!typeof(T).IsMockeable())
				throw new ArgumentException(Properties.Resources.InvalidMockClass);

			if (typeof(T).IsInterface && this.constructorArguments.Length > 0)
				throw new ArgumentException(Properties.Resources.ConstructorArgsForInterface);
		}

		#endregion

		#region Properties

		/// <include file='Mock.Generic.xdoc' path='docs/doc[@for="Mock{T}.Object"]/*'/>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1716:IdentifiersShouldNotMatchKeywords", MessageId = "Object", Justification = "Exposes the mocked object instance, so it's appropriate.")]
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1721:PropertyNamesShouldNotMatchGetMethods", Justification = "The public Object property is the only one visible to Moq consumers. The protected member is for internal use only.")]
		public virtual new T Object
		{
			get
			{
				if (this.instance == null)
				{
					InitializeInstance();
				}
				return instance;
			}
		}

		private void InitializeInstance()
		{
			PexProtector.Invoke(() =>
			{
				var mockType = typeof(T);

				try
				{
					if (mockType.IsInterface)
					{
						instance
							= (T)generator.CreateInterfaceProxyWithoutTarget(mockType, base.ImplementedInterfaces.ToArray(), Interceptor);
					}
					else
					{
						try
						{
							if (constructorArguments.Length > 0)
							{
								var generatedType = generator.ProxyBuilder.CreateClassProxy(mockType, base.ImplementedInterfaces.ToArray(), new ProxyGenerationOptions());
								instance
									= (T)Activator.CreateInstance(generatedType,
										new object[] { new IInterceptor[] { Interceptor } }.Concat(constructorArguments).ToArray());
							}
							else
							{
								instance = (T)generator.CreateClassProxy(mockType, base.ImplementedInterfaces.ToArray(), Interceptor);
							}
						}
						catch (TypeLoadException tle)
						{
							throw new ArgumentException(Properties.Resources.InvalidMockClass, tle);
						}
					}

				}
				catch (MissingMethodException mme)
				{
					throw new ArgumentException(Properties.Resources.ConstructorNotFound, mme);
				}
			});
		}

		/// <summary>
		/// Returns the mocked object value.
		/// </summary>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate", Justification = "This is actually the protected virtual implementation of the property Object.")]
		protected override object GetObject()
		{
			return Object;
		}

		internal override Type MockedType { get { return typeof(T); } }

		#endregion

		#region Setup

		/// <include file='Mock.Generic.xdoc' path='docs/doc[@for="Mock{T}.Setup"]/*'/>
		public ISetup<T> Setup(Expression<Action<T>> expression)
		{
			return Mock.Setup<T>(this, expression);
		}

		/// <include file='Mock.Generic.xdoc' path='docs/doc[@for="Mock{T}.Setup{TResult}"]/*'/>
		public ISetup<T, TResult> Setup<TResult>(Expression<Func<T, TResult>> expression)
		{
			return Mock.Setup(this, expression);
		}

		/// <include file='Mock.Generic.xdoc' path='docs/doc[@for="Mock{T}.SetupGet"]/*'/>
		public ISetupGetter<T, TProperty> SetupGet<TProperty>(Expression<Func<T, TProperty>> expression)
		{
			return Mock.SetupGet(this, expression);
		}

		/// <include file='Mock.Generic.xdoc' path='docs/doc[@for="Mock{T}.SetupSet{TProperty}"]/*'/>
		public ISetupSetter<T, TProperty> SetupSet<TProperty>(Action<T> setterExpression)
		{
			return Mock.SetupSet<T, TProperty>(this, setterExpression);
		}

		/// <include file='Mock.Generic.xdoc' path='docs/doc[@for="Mock{T}.SetupSet"]/*'/>
		public ISetup<T> SetupSet(Action<T> setterExpression)
		{
			return Mock.SetupSet<T>(this, setterExpression);
		}

		/// <include file='Mock.Generic.xdoc' path='docs/doc[@for="Mock{T}.SetupProperty(property)"]/*'/>
		[SuppressMessage("Microsoft.Naming", "CA1716:IdentifiersShouldNotMatchKeywords", MessageId = "Property", Justification = "This sets properties, so it's appropriate.")]
		public void SetupProperty<TProperty>(Expression<Func<T, TProperty>> property)
		{
			SetupProperty(property, default(TProperty));
		}

		/// <include file='Mock.Generic.xdoc' path='docs/doc[@for="Mock{T}.SetupProperty(property,initialValue)"]/*'/>
		[SuppressMessage("Microsoft.Naming", "CA1716:IdentifiersShouldNotMatchKeywords", MessageId = "Property", Justification = "We're setting up a property, so it's appropriate.")]
		public void SetupProperty<TProperty>(Expression<Func<T, TProperty>> property, TProperty initialValue)
		{
			TProperty value = initialValue;
			SetupGet(property).Returns(() => value);
			SetupSet<T, TProperty>(this, property).Callback(p => value = p);
		}

#if !SILVERLIGHT
		/// <include file='Mock.Generic.xdoc' path='docs/doc[@for="Mock{T}.SetupAllProperties"]/*'/>
		[Conditional("DESKTOP")]
		public void SetupAllProperties()
		{
			SetupAllProperties(this);
		}
#endif

		#endregion

		#region Verify

		/// <summary>
		/// Verifies that a specific invocation matching the given expression was performed on the mock. Use
		/// in conjuntion with the default <see cref="MockBehavior.Loose"/>.
		/// </summary>
		/// <example group="verification">
		/// This example assumes that the mock has been used, and later we want to verify that a given
		/// invocation with specific parameters was performed:
		/// <code>
		/// var mock = new Mock&lt;IProcessor&gt;();
		/// // exercise mock
		/// //...
		/// // Will throw if the test code didn't call Execute with a "ping" string argument.
		/// mock.Verify(proc =&gt; proc.Execute("ping"));
		/// </code>
		/// </example>
		/// <exception cref="MockException">The invocation was not performed on the mock.</exception>
		/// <param name="expression">Expression to verify.</param>
		public void Verify(Expression<Action<T>> expression)
		{
			Mock.Verify(this, expression, Times.AtLeastOnce(), null);
		}

		/// <summary>
		/// Verifies that a specific invocation matching the given expression was performed on the mock. Use
		/// in conjuntion with the default <see cref="MockBehavior.Loose"/>.
		/// </summary>
		/// <exception cref="MockException">The invocation was not call the times specified by
		/// <paramref name="times"/>.</exception>
		/// <param name="expression">Expression to verify.</param>
		/// <param name="times">The number of times a method is allowed to be called.</param>
		public void Verify(Expression<Action<T>> expression, Times times)
		{
			Mock.Verify(this, expression, times, null);
		}

		/// <summary>
		/// Verifies that a specific invocation matching the given expression was performed on the mock,
		/// specifying a failure error message. Use in conjuntion with the default
		/// <see cref="MockBehavior.Loose"/>.
		/// </summary>
		/// <example group="verification">
		/// This example assumes that the mock has been used, and later we want to verify that a given
		/// invocation with specific parameters was performed:
		/// <code>
		/// var mock = new Mock&lt;IProcessor&gt;();
		/// // exercise mock
		/// //...
		/// // Will throw if the test code didn't call Execute with a "ping" string argument.
		/// mock.Verify(proc =&gt; proc.Execute("ping"));
		/// </code>
		/// </example>
		/// <exception cref="MockException">The invocation was not performed on the mock.</exception>
		/// <param name="expression">Expression to verify.</param>
		/// <param name="failMessage">Message to show if verification fails.</param>
		public void Verify(Expression<Action<T>> expression, string failMessage)
		{
			Mock.Verify(this, expression, Times.AtLeastOnce(), failMessage);
		}

		/// <summary>
		/// Verifies that a specific invocation matching the given expression was performed on the mock,
		/// specifying a failure error message. Use in conjuntion with the default
		/// <see cref="MockBehavior.Loose"/>.
		/// </summary>
		/// <exception cref="MockException">The invocation was not call the times specified by
		/// <paramref name="times"/>.</exception>
		/// <param name="expression">Expression to verify.</param>
		/// <param name="times">The number of times a method is allowed to be called.</param>
		/// <param name="failMessage">Message to show if verification fails.</param>
		public void Verify(Expression<Action<T>> expression, Times times, string failMessage)
		{
			Mock.Verify(this, expression, times, failMessage);
		}

		/// <summary>
		/// Verifies that a specific invocation matching the given expression was performed on the mock. Use
		/// in conjuntion with the default <see cref="MockBehavior.Loose"/>.
		/// </summary>
		/// <example group="verification">
		/// This example assumes that the mock has been used, and later we want to verify that a given
		/// invocation with specific parameters was performed:
		/// <code>
		/// var mock = new Mock&lt;IWarehouse&gt;();
		/// // exercise mock
		/// //...
		/// // Will throw if the test code didn't call HasInventory.
		/// mock.Verify(warehouse =&gt; warehouse.HasInventory(TALISKER, 50));
		/// </code>
		/// </example>
		/// <exception cref="MockException">The invocation was not performed on the mock.</exception>
		/// <param name="expression">Expression to verify.</param>
		/// <typeparam name="TResult">Type of return value from the expression.</typeparam>
		public void Verify<TResult>(Expression<Func<T, TResult>> expression)
		{
			Mock.Verify(this, expression, Times.AtLeastOnce(), null);
		}

		/// <summary>
		/// Verifies that a specific invocation matching the given 
		/// expression was performed on the mock. Use in conjuntion 
		/// with the default <see cref="MockBehavior.Loose"/>.
		/// </summary>
		/// <exception cref="MockException">The invocation was not call the times specified by
		/// <paramref name="times"/>.</exception>
		/// <param name="expression">Expression to verify.</param>
		/// <param name="times">The number of times a method is allowed to be called.</param>
		/// <typeparam name="TResult">Type of return value from the expression.</typeparam>
		public void Verify<TResult>(Expression<Func<T, TResult>> expression, Times times)
		{
			Mock.Verify(this, expression, times, null);
		}

		/// <summary>
		/// Verifies that a specific invocation matching the given 
		/// expression was performed on the mock, specifying a failure  
		/// error message.
		/// Use in conjuntion with the default <see cref="MockBehavior.Loose"/>.
		/// </summary>
		/// <example group="verification">
		/// This example assumes that the mock has been used, 
		/// and later we want to verify that a given invocation 
		/// with specific parameters was performed:
		/// <code>
		/// var mock = new Mock&lt;IWarehouse&gt;();
		/// // exercise mock
		/// //...
		/// // Will throw if the test code didn't call HasInventory.
		/// mock.Verify(warehouse =&gt; warehouse.HasInventory(TALISKER, 50), "When filling orders, inventory has to be checked");
		/// </code>
		/// </example>
		/// <exception cref="MockException">The invocation was not performed on the mock.</exception>
		/// <param name="expression">Expression to verify.</param>
		/// <param name="failMessage">Message to show if verification fails.</param>
		/// <typeparam name="TResult">Type of return value from the expression.</typeparam>
		public void Verify<TResult>(Expression<Func<T, TResult>> expression, string failMessage)
		{
			Mock.Verify(this, expression, Times.AtLeastOnce(), failMessage);
		}

		/// <summary>
		/// Verifies that a specific invocation matching the given 
		/// expression was performed on the mock, specifying a failure  
		/// error message.
		/// Use in conjuntion with the default <see cref="MockBehavior.Loose"/>.
		/// </summary>
		/// <exception cref="MockException">The invocation was not call the times specified by
		/// <paramref name="times"/>.</exception>
		/// <param name="expression">Expression to verify.</param>
		/// <param name="times">The number of times a method is allowed to be called.</param>
		/// <param name="failMessage">Message to show if verification fails.</param>
		/// <typeparam name="TResult">Type of return value from the expression.</typeparam>
		public void Verify<TResult>(Expression<Func<T, TResult>> expression, Times times, string failMessage)
		{
			Mock.Verify(this, expression, times, failMessage);
		}

		/// <summary>
		/// Verifies that a property was read on the mock. 
		/// Use in conjuntion with the default <see cref="MockBehavior.Loose"/>.
		/// </summary>
		/// <example group="verification">
		/// This example assumes that the mock has been used, 
		/// and later we want to verify that a given property 
		/// was retrieved from it:
		/// <code>
		/// var mock = new Mock&lt;IWarehouse&gt;();
		/// // exercise mock
		/// //...
		/// // Will throw if the test code didn't retrieve the IsClosed property.
		/// mock.VerifyGet(warehouse =&gt; warehouse.IsClosed);
		/// </code>
		/// </example>
		/// <exception cref="MockException">The invocation was not performed on the mock.</exception>
		/// <param name="expression">Expression to verify.</param>
		/// <typeparam name="TProperty">Type of the property to verify. Typically omitted as it can 
		/// be inferred from the expression's return type.</typeparam>
		public void VerifyGet<TProperty>(Expression<Func<T, TProperty>> expression)
		{
			Mock.VerifyGet(this, expression, Times.AtLeastOnce(), null);
		}

		/// <summary>
		/// Verifies that a property was read on the mock. 
		/// Use in conjuntion with the default <see cref="MockBehavior.Loose"/>.
		/// </summary>
		/// <exception cref="MockException">The invocation was not call the times specified by
		/// <paramref name="times"/>.</exception>
		/// <param name="times">The number of times a method is allowed to be called.</param>
		/// <param name="expression">Expression to verify.</param>
		/// <typeparam name="TProperty">Type of the property to verify. Typically omitted as it can 
		/// be inferred from the expression's return type.</typeparam>
		public void VerifyGet<TProperty>(Expression<Func<T, TProperty>> expression, Times times)
		{
			Mock.VerifyGet(this, expression, times, null);
		}

		/// <summary>
		/// Verifies that a property was read on the mock, specifying a failure  
		/// error message. 
		/// Use in conjuntion with the default <see cref="MockBehavior.Loose"/>.
		/// </summary>
		/// <example group="verification">
		/// This example assumes that the mock has been used, 
		/// and later we want to verify that a given property 
		/// was retrieved from it:
		/// <code>
		/// var mock = new Mock&lt;IWarehouse&gt;();
		/// // exercise mock
		/// //...
		/// // Will throw if the test code didn't retrieve the IsClosed property.
		/// mock.VerifyGet(warehouse =&gt; warehouse.IsClosed);
		/// </code>
		/// </example>
		/// <exception cref="MockException">The invocation was not performed on the mock.</exception>
		/// <param name="expression">Expression to verify.</param>
		/// <param name="failMessage">Message to show if verification fails.</param>
		/// <typeparam name="TProperty">Type of the property to verify. Typically omitted as it can 
		/// be inferred from the expression's return type.</typeparam>
		public void VerifyGet<TProperty>(Expression<Func<T, TProperty>> expression, string failMessage)
		{
			Mock.VerifyGet(this, expression, Times.AtLeastOnce(), failMessage);
		}

		/// <summary>
		/// Verifies that a property was read on the mock, specifying a failure  
		/// error message. 
		/// Use in conjuntion with the default <see cref="MockBehavior.Loose"/>.
		/// </summary>
		/// <exception cref="MockException">The invocation was not call the times specified by
		/// <paramref name="times"/>.</exception>
		/// <param name="times">The number of times a method is allowed to be called.</param>
		/// <param name="expression">Expression to verify.</param>
		/// <param name="failMessage">Message to show if verification fails.</param>
		/// <typeparam name="TProperty">Type of the property to verify. Typically omitted as it can 
		/// be inferred from the expression's return type.</typeparam>
		public void VerifyGet<TProperty>(
			Expression<Func<T, TProperty>> expression,
			Times times,
			string failMessage)
		{
			Mock.VerifyGet(this, expression, times, failMessage);
		}

		/// <summary>
		/// Verifies that a property was set on the mock.
		/// Use in conjuntion with the default <see cref="MockBehavior.Loose"/>.
		/// </summary>
		/// <example group="verification">
		/// This example assumes that the mock has been used, 
		/// and later we want to verify that a given property 
		/// was set on it:
		/// <code>
		/// var mock = new Mock&lt;IWarehouse&gt;();
		/// // exercise mock
		/// //...
		/// // Will throw if the test code didn't set the IsClosed property.
		/// mock.VerifySet(warehouse =&gt; warehouse.IsClosed = true);
		/// </code>
		/// </example>
		/// <exception cref="MockException">The invocation was not performed on the mock.</exception>
		/// <param name="setterExpression">Expression to verify.</param>
		public void VerifySet(Action<T> setterExpression)
		{
			Mock.VerifySet<T>(this, setterExpression, Times.AtLeastOnce(), null);
		}

		/// <summary>
		/// Verifies that a property was set on the mock.
		/// Use in conjuntion with the default <see cref="MockBehavior.Loose"/>.
		/// </summary>
		/// <exception cref="MockException">The invocation was not call the times specified by
		/// <paramref name="times"/>.</exception>
		/// <param name="times">The number of times a method is allowed to be called.</param>
		/// <param name="setterExpression">Expression to verify.</param>
		public void VerifySet(Action<T> setterExpression, Times times)
		{
			Mock.VerifySet(this, setterExpression, times, null);
		}

		/// <summary>
		/// Verifies that a property was set on the mock, specifying 
		/// a failure message.
		/// Use in conjuntion with the default <see cref="MockBehavior.Loose"/>.
		/// </summary>
		/// <example group="verification">
		/// This example assumes that the mock has been used, 
		/// and later we want to verify that a given property 
		/// was set on it:
		/// <code>
		/// var mock = new Mock&lt;IWarehouse&gt;();
		/// // exercise mock
		/// //...
		/// // Will throw if the test code didn't set the IsClosed property.
		/// mock.VerifySet(warehouse =&gt; warehouse.IsClosed = true, "Warehouse should always be closed after the action");
		/// </code>
		/// </example>
		/// <exception cref="MockException">The invocation was not performed on the mock.</exception>
		/// <param name="setterExpression">Expression to verify.</param>
		/// <param name="failMessage">Message to show if verification fails.</param>
		public void VerifySet(Action<T> setterExpression, string failMessage)
		{
			Mock.VerifySet(this, setterExpression, Times.AtLeastOnce(), failMessage);
		}

		/// <summary>
		/// Verifies that a property was set on the mock, specifying 
		/// a failure message.
		/// </summary>
		/// <exception cref="MockException">The invocation was not call the times specified by
		/// <paramref name="times"/>.</exception>
		/// <param name="times">The number of times a method is allowed to be called.</param>
		/// <param name="setterExpression">Expression to verify.</param>
		/// <param name="failMessage">Message to show if verification fails.</param>
		public void VerifySet(Action<T> setterExpression, Times times, string failMessage)
		{
			Mock.VerifySet(this, setterExpression, times, failMessage);
		}

		#endregion

		#region As<TInterface>

		/// <summary>
		/// Adds an interface implementation to the mock, 
		/// allowing setups to be specified for it.
		/// </summary>
		/// <remarks>
		/// This method can only be called before the first use 
		/// of the mock <see cref="Object"/> property, at which 
		/// point the runtime type has already been generated 
		/// and no more interfaces can be added to it.
		/// <para>
		/// Also, <typeparamref name="TInterface"/> must be an 
		/// interface and not a class, which must be specified 
		/// when creating the mock instead.
		/// </para>
		/// </remarks>
		/// <exception cref="InvalidOperationException">The mock type 
		/// has already been generated by accessing the <see cref="Object"/> property.</exception>
		/// <exception cref="ArgumentException">The <typeparamref name="TInterface"/> specified 
		/// is not an interface.</exception>
		/// <example>
		/// The following example creates a mock for the main interface 
		/// and later adds <see cref="IDisposable"/> to it to verify 
		/// it's called by the consumer code:
		/// <code>
		/// var mock = new Mock&lt;IProcessor&gt;();
		/// mock.Setup(x =&gt; x.Execute("ping"));
		/// 
		/// // add IDisposable interface
		/// var disposable = mock.As&lt;IDisposable&gt;();
		/// disposable.Setup(d => d.Dispose()).Verifiable();
		/// </code>
		/// </example>
		/// <typeparam name="TInterface">Type of interface to cast the mock to.</typeparam>
		[SuppressMessage("Microsoft.Naming", "CA1716:IdentifiersShouldNotMatchKeywords", MessageId = "As", Justification = "We want the method called exactly as the keyword because that's what it does, it adds an implemented interface so that you can cast it later.")]
		public virtual Mock<TInterface> As<TInterface>()
			where TInterface : class
		{
			if (this.instance != null && !base.ImplementedInterfaces.Contains(typeof(TInterface)))
			{
				throw new InvalidOperationException(Properties.Resources.AlreadyInitialized);
			}
			if (!typeof(TInterface).IsInterface)
			{
				throw new ArgumentException(Properties.Resources.AsMustBeInterface);
			}

			if (!base.ImplementedInterfaces.Contains(typeof(TInterface)))
			{
				base.ImplementedInterfaces.Add(typeof(TInterface));
			}

			return new AsInterface<TInterface>(this);
		}

		private class AsInterface<TInterface> : Mock<TInterface>
			where TInterface : class
		{
			[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields", Justification = "It's used right below!")]
			Mock<T> owner;

			public AsInterface(Mock<T> owner)
				: base(true)
			{
				this.owner = owner;
			}

			internal override Dictionary<MethodInfo, Mock> InnerMocks
			{
				get { return owner.InnerMocks; }
				set { owner.InnerMocks = value; }
			}

			internal override Interceptor Interceptor
			{
				get { return owner.Interceptor; }
				set { owner.Interceptor = value; }
			}

			internal override Type MockedType { get { return typeof(TInterface); } }

			public override MockBehavior Behavior
			{
				get { return owner.Behavior; }
				internal set { owner.Behavior = value; }
			}

			public override bool CallBase
			{
				get { return owner.CallBase; }
				set { owner.CallBase = value; }
			}

			public override DefaultValue DefaultValue
			{
				get { return owner.DefaultValue; }
				set { owner.DefaultValue = value; }
			}

			public override TInterface Object
			{
				get { return owner.Object as TInterface; }
			}

			public override Mock<TNewInterface> As<TNewInterface>()
			{
				return owner.As<TNewInterface>();
			}

			public override MockedEvent<TEventArgs> CreateEventHandler<TEventArgs>()
			{
				return owner.CreateEventHandler<TEventArgs>();
			}

			public override MockedEvent<EventArgs> CreateEventHandler()
			{
				return owner.CreateEventHandler();
			}
		}

		#endregion

		/// <summary>
		/// Raises the event referenced in <paramref name="eventExpression"/> using 
		/// the given <paramref name="sender"/> and <paramref name="args"/> arguments.
		/// </summary>
		/// <exception cref="ArgumentException">The <paramref name="args"/> arguments are 
		/// invalid for the target event invocation, or the <paramref name="eventExpression"/> is 
		/// not an event attach or detach expression.</exception>
		/// <example>
		/// The following example shows how to raise a <see cref="System.ComponentModel.INotifyPropertyChanged.PropertyChanged"/> event:
		/// <code>
		/// var mock = new Mock&lt;IViewModel&gt;();
		/// 
		/// mock.Raise(x => x.PropertyChanged -= null, new PropertyChangedEventArgs("Name"));
		/// </code>
		/// </example>
		/// <example>
		/// This example shows how to invoke an event with a custom event arguments 
		/// class in a view that will cause its corresponding presenter to 
		/// react by changing its state:
		/// <code>
		/// var mockView = new Mock&lt;IOrdersView&gt;();
		/// var presenter = new OrdersPresenter(mockView.Object);
		/// 
		/// // Check that the presenter has no selection by default
		/// Assert.Null(presenter.SelectedOrder);
		/// 
		/// // Raise the event with a specific arguments data
		/// mockView.Raise(v => v.SelectionChanged += null, new OrderEventArgs { Order = new Order("moq", 500) });
		/// 
		/// // Now the presenter reacted to the event, and we have a selected order
		/// Assert.NotNull(presenter.SelectedOrder);
		/// Assert.Equal("moq", presenter.SelectedOrder.ProductName);
		/// </code>
		/// </example>
		[SuppressMessage("Microsoft.Design", "CA1030:UseEventsWhereAppropriate", Justification = "Raises the event, rather than being one.")]
		[SuppressMessage("Microsoft.Usage", "CA2200:RethrowToPreserveStackDetails", Justification = "We want to reset the stack trace to avoid Moq noise in it.")]
		public void Raise(Action<T> eventExpression, EventArgs args)
		{
			var ev = eventExpression.GetEvent(this.Object);

			var me = new MockedEvent(this);
			me.Event = ev;

			try
			{
				me.DoRaise(args);
			}
			catch (Exception e)
			{
				// Reset stacktrace so user gets this call site only.
				throw e;
			}
		}

		// NOTE: known issue. See https://connect.microsoft.com/VisualStudio/feedback/ViewFeedback.aspx?FeedbackID=318122
		//public static implicit operator TInterface(Mock<T> mock)
		//{
		//    // TODO: doesn't work as expected but ONLY with interfaces :S
		//    return mock.Object;
		//}

		//public static explicit operator TInterface(Mock<T> mock)
		//{
		//    // TODO: doesn't work as expected but ONLY with interfaces :S
		//    throw new NotImplementedException();
		//}
	}

	internal class ValueClosure<TValue>
	{
		public ValueClosure(TValue initialValue)
		{
			Value = initialValue;
		}

		public TValue Value { get; set; }

		public TValue GetValue() { return Value; }
		public void SetValue(TValue value) { Value = value; }
	}
}
