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
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using System.Reflection;
using Moq.Language.Flow;
using Moq.Proxy;

namespace Moq
{
	/// <include file='Mock.Generic.xdoc' path='docs/doc[@for="Mock{T}"]/*'/>
	public partial class Mock<T> : Mock where T : class
	{
		private static IProxyFactory proxyFactory = new CastleProxyFactory();
		private T instance;
		private object[] constructorArguments;

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
		[SuppressMessage("Microsoft.Naming", "CA1716:IdentifiersShouldNotMatchKeywords", MessageId = "Object", Justification = "Exposes the mocked object instance, so it's appropriate.")]
		[SuppressMessage("Microsoft.Naming", "CA1721:PropertyNamesShouldNotMatchGetMethods", Justification = "The public Object property is the only one visible to Moq consumers. The protected member is for internal use only.")]
		public virtual new T Object
		{
			get
			{
				if (this.instance == null)
				{
					this.InitializeInstance();
				}

				return this.instance;
			}
		}

		private void InitializeInstance()
		{
			PexProtector.Invoke(() =>
			{
				this.instance = proxyFactory.CreateProxy<T>(
					this.Interceptor,
					this.ImplementedInterfaces.ToArray(),
					this.constructorArguments);
			});
		}

		/// <summary>
		/// Returns the mocked object value.
		/// </summary>
		[SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate", Justification = "This is actually the protected virtual implementation of the property Object.")]
		protected override object GetObject()
		{
			return this.Object;
		}

		internal override Type MockedType
		{
			get { return typeof(T); }
		}

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
		public void SetupAllProperties()
		{
			SetupAllProperties(this);
		}
#endif

		#endregion

		#region Verify

		/// <include file='Mock.Generic.xdoc' path='docs/doc[@for="Mock{T}.Verify(expression)"]/*'/>
		public void Verify(Expression<Action<T>> expression)
		{
			Mock.Verify(this, expression, Times.AtLeastOnce(), null);
		}

		/// <include file='Mock.Generic.xdoc' path='docs/doc[@for="Mock{T}.Verify(expression,times)"]/*'/>
		public void Verify(Expression<Action<T>> expression, Times times)
		{
			Mock.Verify(this, expression, times, null);
		}

		/// <include file='Mock.Generic.xdoc' path='docs/doc[@for="Mock{T}.Verify(expression,failMessage)"]/*'/>
		public void Verify(Expression<Action<T>> expression, string failMessage)
		{
			Mock.Verify(this, expression, Times.AtLeastOnce(), failMessage);
		}

		/// <include file='Mock.Generic.xdoc' path='docs/doc[@for="Mock{T}.Verify(expression,times,failMessage)"]/*'/>
		public void Verify(Expression<Action<T>> expression, Times times, string failMessage)
		{
			Mock.Verify(this, expression, times, failMessage);
		}

		/// <include file='Mock.Generic.xdoc' path='docs/doc[@for="Mock{T}.Verify{TResult}(expression)"]/*'/>
		public void Verify<TResult>(Expression<Func<T, TResult>> expression)
		{
			Mock.Verify(this, expression, Times.AtLeastOnce(), null);
		}

		/// <include file='Mock.Generic.xdoc' path='docs/doc[@for="Mock{T}.Verify{TResult}(expression,times)"]/*'/>
		public void Verify<TResult>(Expression<Func<T, TResult>> expression, Times times)
		{
			Mock.Verify(this, expression, times, null);
		}

		/// <include file='Mock.Generic.xdoc' path='docs/doc[@for="Mock{T}.Verify{TResult}(expression,failMessage)"]/*'/>
		public void Verify<TResult>(Expression<Func<T, TResult>> expression, string failMessage)
		{
			Mock.Verify(this, expression, Times.AtLeastOnce(), failMessage);
		}

		/// <include file='Mock.Generic.xdoc' path='docs/doc[@for="Mock{T}.Verify{TResult}(expression,times,failMessage)"]/*'/>
		public void Verify<TResult>(Expression<Func<T, TResult>> expression, Times times, string failMessage)
		{
			Mock.Verify(this, expression, times, failMessage);
		}

		/// <include file='Mock.Generic.xdoc' path='docs/doc[@for="Mock{T}.VerifyGet(expression)"]/*'/>
		public void VerifyGet<TProperty>(Expression<Func<T, TProperty>> expression)
		{
			Mock.VerifyGet(this, expression, Times.AtLeastOnce(), null);
		}

		/// <include file='Mock.Generic.xdoc' path='docs/doc[@for="Mock{T}.VerifyGet(expression,times)"]/*'/>
		public void VerifyGet<TProperty>(Expression<Func<T, TProperty>> expression, Times times)
		{
			Mock.VerifyGet(this, expression, times, null);
		}

		/// <include file='Mock.Generic.xdoc' path='docs/doc[@for="Mock{T}.VerifyGet(expression,failMessage)"]/*'/>
		public void VerifyGet<TProperty>(Expression<Func<T, TProperty>> expression, string failMessage)
		{
			Mock.VerifyGet(this, expression, Times.AtLeastOnce(), failMessage);
		}

		/// <include file='Mock.Generic.xdoc' path='docs/doc[@for="Mock{T}.VerifyGet(expression,times,failMessage)"]/*'/>
		public void VerifyGet<TProperty>(Expression<Func<T, TProperty>> expression, Times times, string failMessage)
		{
			Mock.VerifyGet(this, expression, times, failMessage);
		}

		/// <include file='Mock.Generic.xdoc' path='docs/doc[@for="Mock{T}.VerifySet(expression)"]/*'/>
		public void VerifySet(Action<T> setterExpression)
		{
			Mock.VerifySet<T>(this, setterExpression, Times.AtLeastOnce(), null);
		}

		/// <include file='Mock.Generic.xdoc' path='docs/doc[@for="Mock{T}.VerifySet(expression,times)"]/*'/>
		public void VerifySet(Action<T> setterExpression, Times times)
		{
			Mock.VerifySet(this, setterExpression, times, null);
		}

		/// <include file='Mock.Generic.xdoc' path='docs/doc[@for="Mock{T}.VerifySet(expression,failMessage)"]/*'/>
		public void VerifySet(Action<T> setterExpression, string failMessage)
		{
			Mock.VerifySet(this, setterExpression, Times.AtLeastOnce(), failMessage);
		}

		/// <include file='Mock.Generic.xdoc' path='docs/doc[@for="Mock{T}.VerifySet(expression,times,failMessage)"]/*'/>
		public void VerifySet(Action<T> setterExpression, Times times, string failMessage)
		{
			Mock.VerifySet(this, setterExpression, times, failMessage);
		}

		#endregion

		#region As<TInterface>

		/// <include file='Mock.Generic.xdoc' path='docs/doc[@for="Mock{T}.As{TInterface}"]/*'/>
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

		/// <include file='Mock.Generic.xdoc' path='docs/doc[@for="Mock{T}.Raise"]/*'/>
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

		/// <include file='Mock.Generic.xdoc' path='docs/doc[@for="Mock{T}.Raise(args)"]/*'/>
		[SuppressMessage("Microsoft.Design", "CA1030:UseEventsWhereAppropriate", Justification = "Raises the event, rather than being one.")]
		[SuppressMessage("Microsoft.Usage", "CA2200:RethrowToPreserveStackDetails", Justification = "We want to reset the stack trace to avoid Moq noise in it.")]
		public void Raise(Action<T> eventExpression, params object[] args)
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
