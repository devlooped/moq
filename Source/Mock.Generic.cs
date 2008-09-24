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

namespace Moq
{
	/// <summary>
	/// Core implementation of the <see cref="IMock{T}"/> interface. 
	/// </summary>
	/// <seealso cref="IMock{T}"/>
	/// <typeparam name="T">Type to mock.</typeparam>
	public class Mock<T> : Mock, IMock<T>
		where T : class
	{
		static readonly ProxyGenerator generator = new ProxyGenerator();
		T instance;
		MockBehavior behavior;
		object[] constructorArguments;

		#region Ctors

		/// <summary>
		/// Initializes an instance of the mock with <see cref="MockBehavior.Default">default behavior</see> and with 
		/// the given constructor arguments for the class. (Only valid when <typeparamref name="T"/> is a class)
		/// </summary>
		/// <remarks>
		/// The mock will try to find the best match constructor given the constructor arguments, and invoke that 
		/// to initialize the instance. This applies only for classes, not interfaces.
		/// </remarks>
		/// <example>
		/// <code>var mock = new Mock&lt;MyProvider&gt;(someArgument, 25);</code>
		/// </example>
		/// <param name="args">Optional constructor arguments if the mocked type is a class.</param>
		public Mock(params object[] args) : this(MockBehavior.Default, args) { }

		/// <summary>
		/// Initializes an instance of the mock with <see cref="MockBehavior.Default">default behavior</see>.
		/// </summary>
		/// <example>
		/// <code>var mock = new Mock&lt;IFormatProvider&gt;();</code>
		/// </example>
		public Mock() : this(MockBehavior.Default) { }

		/// <summary>
		/// Initializes an instance of the mock with the specified <see cref="MockBehavior">behavior</see>.
		/// </summary>
		/// <example>
		/// <code>var mock = new Mock&lt;IFormatProvider&gt;(MockBehavior.Relaxed);</code>
		/// </example>
		/// <param name="behavior">Behavior of the mock.</param>
		public Mock(MockBehavior behavior) : this(behavior, new object[0]) { }

		/// <summary>
		/// Initializes an instance of the mock with a specific <see cref="MockBehavior">behavior</see> with 
		/// the given constructor arguments for the class.
		/// </summary>
		/// <remarks>
		/// The mock will try to find the best match constructor given the constructor arguments, and invoke that 
		/// to initialize the instance. This applies only to classes, not interfaces.
		/// </remarks>
		/// <example>
		/// <code>var mock = new Mock&lt;MyProvider&gt;(someArgument, 25);</code>
		/// </example>
		/// <param name="behavior">Behavior of the mock.</param>
		/// <param name="args">Optional constructor arguments if the mocked type is a class.</param>
		public Mock(MockBehavior behavior, params object[] args)
		{
			if (args == null) args = new object[0];

			this.behavior = behavior;
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

		/// <summary>
		/// Exposes the mocked object instance.
		/// </summary>
		public new T Object
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
		}

		/// <summary>
		/// Returns the mocked object value.
		/// </summary>
		protected override object GetObject()
		{
			return Object;
		}

		internal override Type MockedType { get { return typeof(T); } }

		/// <devdoc>
		/// Used for testing the mock factory.
		/// </devdoc>
		internal MockBehavior Behavior { get { return behavior; } }

		#endregion

		#region Expect

		/// <summary>
		/// Implements <see cref="IMock{T}.Expect(Expression{Action{T}})"/>.
		/// </summary>
		/// <param name="expression">Lambda expression that specifies the expected method invocation.</param>
		public IExpect Expect(Expression<Action<T>> expression)
		{
			return Mock.SetUpExpect<T>(expression, this.Interceptor);
		}

		/// <summary>
		/// Implements <see cref="IMock{T}.Expect{TResult}(Expression{Func{T, TResult}})"/>.
		/// </summary>
		/// <typeparam name="TResult">Type of the return value. Typically omitted as it can be inferred from the expression.</typeparam>
		/// <param name="expression">Lambda expression that specifies the expected method invocation.</param>
		public IExpect<TResult> Expect<TResult>(Expression<Func<T, TResult>> expression)
		{
			return SetUpExpect(expression, this.Interceptor);
		}

		/// <summary>
		/// Implements <see cref="IMock{T}.ExpectGet{TProperty}(Expression{Func{T, TProperty}})"/>.
		/// </summary>
		/// <typeparam name="TProperty">Type of the property. Typically omitted as it can be inferred from the expression.</typeparam>
		/// <param name="expression">Lambda expression that specifies the expected property getter.</param>
		public IExpectGetter<TProperty> ExpectGet<TProperty>(Expression<Func<T, TProperty>> expression)
		{
			return SetUpExpectGet(expression, this.Interceptor);
		}

		/// <summary>
		/// Implements <see cref="IMock{T}.ExpectSet{TProperty}(Expression{Func{T, TProperty}})"/>.
		/// </summary>
		/// <typeparam name="TProperty">Type of the property. Typically omitted as it can be inferred from the expression.</typeparam>
		/// <param name="expression">Lambda expression that specifies the expected property setter.</param>
		public IExpectSetter<TProperty> ExpectSet<TProperty>(Expression<Func<T, TProperty>> expression)
		{
			return SetUpExpectSet<T, TProperty>(expression, this.Interceptor);
		}

		/// <summary>
		/// Implements <see cref="IMock{T}.ExpectSet{TProperty}(Expression{Func{T, TProperty}}, TProperty)"/>.
		/// </summary>
		/// <typeparam name="TProperty">Type of the property. Typically omitted as it can be inferred from the expression.</typeparam>
		/// <param name="expression">Lambda expression that specifies the expected property setter.</param>
		/// <param name="value">The value expected to be set for the property.</param>
		public IExpectSetter<TProperty> ExpectSet<TProperty>(Expression<Func<T, TProperty>> expression, TProperty value)
		{
			return SetUpExpectSet(expression, value, this.Interceptor);
		}

		#endregion

		#region Verify

		/// <summary>
		/// Implements <see cref="IMock{T}.Verify(Expression{Action{T}})"/>.
		/// </summary>
		/// <param name="expression">Expression to verify.</param>
		public virtual void Verify(Expression<Action<T>> expression)
		{
			Verify(expression, Interceptor);
		}

		/// <summary>
		/// Implements <see cref="IMock{T}.Verify{TResult}(Expression{Func{T, TResult}})"/>.
		/// </summary>
		/// <param name="expression">Expression to verify.</param>
		/// <typeparam name="TResult">Type of return value from the expression.</typeparam>
		public virtual void Verify<TResult>(Expression<Func<T, TResult>> expression)
		{
			Verify(expression, Interceptor);
		}

		/// <summary>
		/// Implements <see cref="IMock{T}.VerifyGet{TProperty}(Expression{Func{T, TProperty}})"/>.
		/// </summary>
		/// <param name="expression">Expression to verify.</param>
		/// <typeparam name="TProperty">Type of the property to verify. Typically omitted as it can 
		/// be inferred from the expression's return type.</typeparam>
		public virtual void VerifyGet<TProperty>(Expression<Func<T, TProperty>> expression)
		{
			VerifyGet(expression, Interceptor);
		}

		/// <summary>
		/// Implements <see cref="IMock{T}.VerifySet{TProperty}(Expression{Func{T, TProperty}})"/>.
		/// </summary>
		/// <param name="expression">Expression to verify.</param>
		/// <typeparam name="TProperty">Type of the property to verify. Typically omitted as it can 
		/// be inferred from the expression's return type.</typeparam>
		public virtual void VerifySet<TProperty>(Expression<Func<T, TProperty>> expression)
		{
			VerifySet(expression, Interceptor);
		}

		/// <summary>
		/// Implements <see cref="IMock{T}.VerifySet{TProperty}(Expression{Func{T, TProperty}}, TProperty)"/>.
		/// </summary>
		/// <param name="expression">Expression to verify.</param>
		/// <param name="value">The value that should have been set on the property.</param>
		/// <typeparam name="TProperty">Type of the property to verify. Typically omitted as it can 
		/// be inferred from the expression's return type.</typeparam>
		public virtual void VerifySet<TProperty>(Expression<Func<T, TProperty>> expression, TProperty value)
		{
			VerifySet(expression, value, Interceptor);
		}

		/// <summary>
		/// Implements <see cref="IMock{T}.Verify()"/>.
		/// </summary>
		public override void Verify()
		{
			Mock.Verify(Interceptor);
		}

		/// <summary>
		/// Implements <see cref="IMock{T}.VerifyAll()"/>.
		/// </summary>
		public override void VerifyAll()
		{
			Mock.VerifyAll(Interceptor);
		}

		#endregion

		#region As<TInterface>

		/// <summary>
		/// Implements <see cref="IMock{T}.As{TInterface}"/>.
		/// </summary>
		/// <typeparam name="TInterface">Type of interface to cast the mock to.</typeparam>
		public virtual IMock<TInterface> As<TInterface>()
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

		private class AsInterface<TInterface> : IMock<TInterface>
			where TInterface : class
		{
			Mock<T> owner;

			public AsInterface(Mock<T> owner)
			{
				this.owner = owner;
			}

			public IMock<TNewInterface> As<TNewInterface>() where TNewInterface : class
			{
				return owner.As<TNewInterface>();
			}

			public TInterface Object
			{
				get { return owner.Object as TInterface; }
			}

			public IExpect<TResult> Expect<TResult>(Expression<Func<TInterface, TResult>> expression)
			{
				return Mock.SetUpExpect(expression, this.owner.Interceptor);
			}

			public IExpect Expect(Expression<Action<TInterface>> expression)
			{
				return Mock.SetUpExpect(expression, this.owner.Interceptor);
			}

			public IExpectGetter<TProperty> ExpectGet<TProperty>(Expression<Func<TInterface, TProperty>> expression)
			{
				return Mock.SetUpExpectGet(expression, this.owner.Interceptor);
			}

			public IExpectSetter<TProperty> ExpectSet<TProperty>(Expression<Func<TInterface, TProperty>> expression)
			{
				return Mock.SetUpExpectSet(expression, this.owner.Interceptor);
			}

			public IExpectSetter<TProperty> ExpectSet<TProperty>(Expression<Func<TInterface, TProperty>> expression, TProperty value)
			{
				return Mock.SetUpExpectSet(expression, value, this.owner.Interceptor);
			}

			public void Verify()
			{
				Mock.Verify(owner.Interceptor);
			}

			public void VerifyAll()
			{
				Mock.VerifyAll(owner.Interceptor);
			}

			public void Verify(Expression<Action<TInterface>> expression)
			{
				Mock.Verify(expression, owner.Interceptor);
			}

			public void Verify<TResult>(Expression<Func<TInterface, TResult>> expression)
			{
				Mock.Verify<TInterface, TResult>(expression, owner.Interceptor);
			}

			public void VerifyGet<TProperty>(Expression<Func<TInterface, TProperty>> expression)
			{
				Mock.VerifyGet<TInterface, TProperty>(expression, owner.Interceptor);
			}

			public void VerifySet<TProperty>(Expression<Func<TInterface, TProperty>> expression)
			{
				Mock.VerifySet<TInterface, TProperty>(expression, owner.Interceptor);
			}

			public void VerifySet<TProperty>(Expression<Func<TInterface, TProperty>> expression, TProperty value)
			{
				Mock.VerifySet<TInterface, TProperty>(expression, value, owner.Interceptor);
			}
		}

		#endregion

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
}
