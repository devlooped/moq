//Copyright (c) 2007. Clarius Consulting, Manas Technology Solutions, InSTEDD
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

//    * Neither the name of Clarius Consulting, Manas Technology Solutions or InSTEDD nor the 
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
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using Moq.Language.Flow;
using Moq.Proxy;
using Moq.Language;
using Moq.Properties;

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

		/// <include file='Mock.Generic.xdoc' path='docs/doc[@for="Mock{T}.ctor()"]/*'/>
		[SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
		public Mock()
			: this(MockBehavior.Default)
		{
		}

		/// <include file='Mock.Generic.xdoc' path='docs/doc[@for="Mock{T}.ctor(object[])"]/*'/>
		[SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
		public Mock(params object[] args)
			: this(MockBehavior.Default, args)
		{
		}

		/// <include file='Mock.Generic.xdoc' path='docs/doc[@for="Mock{T}.ctor(MockBehavior)"]/*'/>
		[SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
		public Mock(MockBehavior behavior)
			: this(behavior, new object[0])
		{
		}

		/// <include file='Mock.Generic.xdoc' path='docs/doc[@for="Mock{T}.ctor(MockBehavior,object[])"]/*'/>
		[SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
		public Mock(MockBehavior behavior, params object[] args)
		{
			if (args == null)
			{
				args = new object[] { null };
			}

			this.Behavior = behavior;
			this.Interceptor = new Interceptor(behavior, typeof(T), this);
			this.constructorArguments = args;
			this.ImplementedInterfaces.Add(typeof(IMocked<T>));

			this.CheckParameters();
		}

		private void CheckParameters()
		{
			typeof(T).ThrowIfNotMockeable();

			if (typeof(T).IsInterface && this.constructorArguments.Length > 0)
			{
				throw new ArgumentException(Resources.ConstructorArgsForInterface);
			}
		}

		#endregion

		#region Properties

		/// <include file='Mock.Generic.xdoc' path='docs/doc[@for="Mock{T}.Object"]/*'/>
		[SuppressMessage("Microsoft.Naming", "CA1716:IdentifiersShouldNotMatchKeywords", MessageId = "Object", Justification = "Exposes the mocked object instance, so it's appropriate.")]
		[SuppressMessage("Microsoft.Naming", "CA1721:PropertyNamesShouldNotMatchGetMethods", Justification = "The public Object property is the only one visible to Moq consumers. The protected member is for internal use only.")]
		public virtual new T Object
		{
			get { return (T)base.Object; }
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

		#endregion

		#region Setup

		/// <include file='Mock.Generic.xdoc' path='docs/doc[@for="Mock{T}.Setup"]/*'/>
		[SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "By design")]
		public ISetup<T> Setup(Expression<Action<T>> expression)
		{
			return Mock.Setup<T>(this, expression, null);
		}

		/// <include file='Mock.Generic.xdoc' path='docs/doc[@for="Mock{T}.Setup{TResult}"]/*'/>
		[SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "By design")]
		public ISetup<T, TResult> Setup<TResult>(Expression<Func<T, TResult>> expression)
		{
			return Mock.Setup(this, expression, null);
		}

		/// <include file='Mock.Generic.xdoc' path='docs/doc[@for="Mock{T}.SetupGet"]/*'/>
		[SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "By design")]
		public ISetupGetter<T, TProperty> SetupGet<TProperty>(Expression<Func<T, TProperty>> expression)
		{
			return Mock.SetupGet(this, expression, null);
		}

		/// <include file='Mock.Generic.xdoc' path='docs/doc[@for="Mock{T}.SetupSet{TProperty}"]/*'/>
		public ISetupSetter<T, TProperty> SetupSet<TProperty>(Action<T> setterExpression)
		{
			return Mock.SetupSet<T, TProperty>(this, setterExpression, null);
		}

		/// <include file='Mock.Generic.xdoc' path='docs/doc[@for="Mock{T}.SetupSet"]/*'/>
		public ISetup<T> SetupSet(Action<T> setterExpression)
		{
			return Mock.SetupSet<T>(this, setterExpression, null);
		}

		/// <include file='Mock.Generic.xdoc' path='docs/doc[@for="Mock{T}.SetupProperty(property)"]/*'/>
		[SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "By design")]
		[SuppressMessage("Microsoft.Naming", "CA1716:IdentifiersShouldNotMatchKeywords", MessageId = "Property", Justification = "This sets properties, so it's appropriate.")]
		public Mock<T> SetupProperty<TProperty>(Expression<Func<T, TProperty>> property)
		{
			return this.SetupProperty(property, default(TProperty));
		}

		/// <include file='Mock.Generic.xdoc' path='docs/doc[@for="Mock{T}.SetupProperty(property,initialValue)"]/*'/>
		[SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "By design")]
		[SuppressMessage("Microsoft.Naming", "CA1716:IdentifiersShouldNotMatchKeywords", MessageId = "Property", Justification = "We're setting up a property, so it's appropriate.")]
		public Mock<T> SetupProperty<TProperty>(Expression<Func<T, TProperty>> property, TProperty initialValue)
		{
			TProperty value = initialValue;
			this.SetupGet(property).Returns(() => value);
			SetupSet<T, TProperty>(this, property).Callback(p => value = p);
			return this;
		}

		/// <include file='Mock.Generic.xdoc' path='docs/doc[@for="Mock{T}.SetupAllProperties"]/*'/>
		public Mock<T> SetupAllProperties()
		{
			SetupAllProperties(this);
			return this;
		}

		#endregion

		#region When

		/// <include file='Mock.Generic.xdoc' path='docs/doc[@for="Mock{T}.When"]/*'/>
		public ISetupConditionResult<T> When(Func<bool> condition)
		{
			return new ConditionalContext<T>(this, condition);
		}

		#endregion

		#region Verify

		/// <include file='Mock.Generic.xdoc' path='docs/doc[@for="Mock{T}.Verify(expression)"]/*'/>
		[SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "By design")]
		public void Verify(Expression<Action<T>> expression)
		{
			Mock.Verify(this, expression, Times.AtLeastOnce(), null);
		}

		/// <include file='Mock.Generic.xdoc' path='docs/doc[@for="Mock{T}.Verify(expression,times)"]/*'/>
		[SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "By design")]
		public void Verify(Expression<Action<T>> expression, Times times)
		{
			Mock.Verify(this, expression, times, null);
		}

		/// <include file='Mock.Generic.xdoc' path='docs/doc[@for="Mock{T}.Verify(expression,times)"]/*'/>
		[SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "By design")]
		public void Verify(Expression<Action<T>> expression, Func<Times> times)
		{
			Verify(expression, times());
		}

		/// <include file='Mock.Generic.xdoc' path='docs/doc[@for="Mock{T}.Verify(expression,failMessage)"]/*'/>
		[SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "By design")]
		public void Verify(Expression<Action<T>> expression, string failMessage)
		{
			Mock.Verify(this, expression, Times.AtLeastOnce(), failMessage);
		}

		/// <include file='Mock.Generic.xdoc' path='docs/doc[@for="Mock{T}.Verify(expression,times,failMessage)"]/*'/>
		[SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "By design")]
		public void Verify(Expression<Action<T>> expression, Times times, string failMessage)
		{
			Mock.Verify(this, expression, times, failMessage);
		}

		/// <include file='Mock.Generic.xdoc' path='docs/doc[@for="Mock{T}.Verify(expression,times,failMessage)"]/*'/>
		[SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "By design")]
		public void Verify(Expression<Action<T>> expression, Func<Times> times, string failMessage)
		{
			Verify(this, expression, times(), failMessage);
		}

		/// <include file='Mock.Generic.xdoc' path='docs/doc[@for="Mock{T}.Verify{TResult}(expression)"]/*'/>
		[SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "By design")]
		public void Verify<TResult>(Expression<Func<T, TResult>> expression)
		{
			Mock.Verify(this, expression, Times.AtLeastOnce(), null);
		}

		/// <include file='Mock.Generic.xdoc' path='docs/doc[@for="Mock{T}.Verify{TResult}(expression,times)"]/*'/>
		[SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "By design")]
		public void Verify<TResult>(Expression<Func<T, TResult>> expression, Times times)
		{
			Mock.Verify(this, expression, times, null);
		}

		/// <include file='Mock.Generic.xdoc' path='docs/doc[@for="Mock{T}.Verify{TResult}(expression,times)"]/*'/>
		[SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "By design")]
		public void Verify<TResult>(Expression<Func<T, TResult>> expression, Func<Times> times)
		{
			Verify(this, expression, times(), null);
		}

		/// <include file='Mock.Generic.xdoc' path='docs/doc[@for="Mock{T}.Verify{TResult}(expression,failMessage)"]/*'/>
		[SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "By design")]
		public void Verify<TResult>(Expression<Func<T, TResult>> expression, string failMessage)
		{
			Mock.Verify(this, expression, Times.AtLeastOnce(), failMessage);
		}

		/// <include file='Mock.Generic.xdoc' path='docs/doc[@for="Mock{T}.Verify{TResult}(expression,times,failMessage)"]/*'/>
		[SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "By design")]
		public void Verify<TResult>(Expression<Func<T, TResult>> expression, Times times, string failMessage)
		{
			Mock.Verify(this, expression, times, failMessage);
		}

		/// <include file='Mock.Generic.xdoc' path='docs/doc[@for="Mock{T}.VerifyGet(expression)"]/*'/>
		[SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "By design")]
		public void VerifyGet<TProperty>(Expression<Func<T, TProperty>> expression)
		{
			Mock.VerifyGet(this, expression, Times.AtLeastOnce(), null);
		}

		/// <include file='Mock.Generic.xdoc' path='docs/doc[@for="Mock{T}.VerifyGet(expression,times)"]/*'/>
		[SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "By design")]
		public void VerifyGet<TProperty>(Expression<Func<T, TProperty>> expression, Times times)
		{
			Mock.VerifyGet(this, expression, times, null);
		}

		/// <include file='Mock.Generic.xdoc' path='docs/doc[@for="Mock{T}.VerifyGet(expression,times)"]/*'/>
		[SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "By design")]
		public void VerifyGet<TProperty>(Expression<Func<T, TProperty>> expression, Func<Times> times)
		{
			VerifyGet(this, expression, times(), null);
		}

		/// <include file='Mock.Generic.xdoc' path='docs/doc[@for="Mock{T}.VerifyGet(expression,failMessage)"]/*'/>
		[SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "By design")]
		public void VerifyGet<TProperty>(Expression<Func<T, TProperty>> expression, string failMessage)
		{
			Mock.VerifyGet(this, expression, Times.AtLeastOnce(), failMessage);
		}

		/// <include file='Mock.Generic.xdoc' path='docs/doc[@for="Mock{T}.VerifyGet(expression,times,failMessage)"]/*'/>
		[SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "By design")]
		public void VerifyGet<TProperty>(Expression<Func<T, TProperty>> expression, Times times, string failMessage)
		{
			Mock.VerifyGet(this, expression, times, failMessage);
		}

		/// <include file='Mock.Generic.xdoc' path='docs/doc[@for="Mock{T}.VerifyGet(expression,times,failMessage)"]/*'/>
		[SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "By design")]
		public void VerifyGet<TProperty>(Expression<Func<T, TProperty>> expression, Func<Times> times, string failMessage)
		{
			VerifyGet(this, expression, times(), failMessage);
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

		/// <include file='Mock.Generic.xdoc' path='docs/doc[@for="Mock{T}.VerifySet(expression,times)"]/*'/>
		public void VerifySet(Action<T> setterExpression, Func<Times> times)
		{
			Mock.VerifySet(this, setterExpression, times(), null);
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

		/// <include file='Mock.Generic.xdoc' path='docs/doc[@for="Mock{T}.VerifySet(expression,times,failMessage)"]/*'/>
		public void VerifySet(Action<T> setterExpression, Func<Times> times, string failMessage)
		{
			Mock.VerifySet(this, setterExpression, times(), failMessage);
		}

		#endregion

		#region Raise

		/// <include file='Mock.Generic.xdoc' path='docs/doc[@for="Mock{T}.Raise"]/*'/>
		[SuppressMessage("Microsoft.Design", "CA1030:UseEventsWhereAppropriate", Justification = "Raises the event, rather than being one.")]
		[SuppressMessage("Microsoft.Usage", "CA2200:RethrowToPreserveStackDetails", Justification = "We want to reset the stack trace to avoid Moq noise in it.")]
		public void Raise(Action<T> eventExpression, EventArgs args)
		{
			var ev = eventExpression.GetEvent(this.Object);

			try
			{
				this.DoRaise(ev, args);
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

			try
			{
				this.DoRaise(ev, args);
			}
			catch (Exception e)
			{
				// Reset stacktrace so user gets this call site only.
				throw e;
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