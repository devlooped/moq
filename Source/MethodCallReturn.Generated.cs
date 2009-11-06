using System;
using System.Diagnostics.CodeAnalysis;
using Moq.Language;
using Moq.Language.Flow;

namespace Moq
{
	internal sealed partial class MethodCallReturn<TMock, TResult>
	{
		public IVerifies Raises<T>(Action<TMock> eventExpression, Func<T, EventArgs> func)
		{
			return this.RaisesImpl(eventExpression, func);
		}

		public IReturnsResult<TMock> Returns<T>(Func<T, TResult> valueExpression)
		{
			this.SetReturnDelegate(valueExpression);
			return this;
		}
		
		[SuppressMessage("Microsoft.Design", "CA1061:DoNotHideBaseClassMethods", Justification = "This class provides typed members for the method-returning interfaces. It's never used through the base class type.")]
		public new IReturnsThrows<TMock, TResult> Callback<T>(Action<T> callback)
		{
			base.Callback(callback);
			return this;
		}

		public IVerifies Raises<T1, T2>(Action<TMock> eventExpression, Func<T1, T2, EventArgs> func)
		{
			return this.RaisesImpl(eventExpression, func);
		}

		public IReturnsResult<TMock> Returns<T1, T2>(Func<T1, T2, TResult> valueExpression)
		{
			this.SetReturnDelegate(valueExpression);
			return this;
		}

		[SuppressMessage("Microsoft.Design", "CA1061:DoNotHideBaseClassMethods", Justification = "This class provides typed members for the method-returning interfaces. It's never used through the base class type.")]
		public new IReturnsThrows<TMock, TResult> Callback<T1, T2>(Action<T1, T2> callback)
		{
			base.Callback(callback);
			return this;
		}

		public IVerifies Raises<T1, T2, T3>(Action<TMock> eventExpression, Func<T1, T2, T3, EventArgs> func)
		{
			return this.RaisesImpl(eventExpression, func);
		}

		public IReturnsResult<TMock> Returns<T1, T2, T3>(Func<T1, T2, T3, TResult> valueExpression)
		{
			this.SetReturnDelegate(valueExpression);
			return this;
		}

		[SuppressMessage("Microsoft.Design", "CA1061:DoNotHideBaseClassMethods", Justification = "This class provides typed members for the method-returning interfaces. It's never used through the base class type.")]
		public new IReturnsThrows<TMock, TResult> Callback<T1, T2, T3>(Action<T1, T2, T3> callback)
		{
			base.Callback(callback);
			return this;
		}

		public IVerifies Raises<T1, T2, T3, T4>(Action<TMock> eventExpression, Func<T1, T2, T3, T4, EventArgs> func)
		{
			return this.RaisesImpl(eventExpression, func);
		}

		public IReturnsResult<TMock> Returns<T1, T2, T3, T4>(Func<T1, T2, T3, T4, TResult> valueExpression)
		{
			this.SetReturnDelegate(valueExpression);
			return this;
		}

		[SuppressMessage("Microsoft.Design", "CA1061:DoNotHideBaseClassMethods", Justification = "This class provides typed members for the method-returning interfaces. It's never used through the base class type.")]
		public new IReturnsThrows<TMock, TResult> Callback<T1, T2, T3, T4>(Action<T1, T2, T3, T4> callback)
		{
			base.Callback(callback);
			return this;
		}

		public IVerifies Raises<T1, T2, T3, T4, T5>(Action<TMock> eventExpression, Func<T1, T2, T3, T4, T5, EventArgs> func)
		{
			return this.RaisesImpl(eventExpression, func);
		}

		public IReturnsResult<TMock> Returns<T1, T2, T3, T4, T5>(Func<T1, T2, T3, T4, T5, TResult> valueExpression)
		{
			this.SetReturnDelegate(valueExpression);
			return this;
		}

		[SuppressMessage("Microsoft.Design", "CA1061:DoNotHideBaseClassMethods", Justification = "This class provides typed members for the method-returning interfaces. It's never used through the base class type.")]
		public new IReturnsThrows<TMock, TResult> Callback<T1, T2, T3, T4, T5>(Action<T1, T2, T3, T4, T5> callback)
		{
			base.Callback(callback);
			return this;
		}

		public IVerifies Raises<T1, T2, T3, T4, T5, T6>(Action<TMock> eventExpression, Func<T1, T2, T3, T4, T5, T6, EventArgs> func)
		{
			return this.RaisesImpl(eventExpression, func);
		}

		public IReturnsResult<TMock> Returns<T1, T2, T3, T4, T5, T6>(Func<T1, T2, T3, T4, T5, T6, TResult> valueExpression)
		{
			this.SetReturnDelegate(valueExpression);
			return this;
		}

		[SuppressMessage("Microsoft.Design", "CA1061:DoNotHideBaseClassMethods", Justification = "This class provides typed members for the method-returning interfaces. It's never used through the base class type.")]
		public new IReturnsThrows<TMock, TResult> Callback<T1, T2, T3, T4, T5, T6>(Action<T1, T2, T3, T4, T5, T6> callback)
		{
			base.Callback(callback);
			return this;
		}

		public IVerifies Raises<T1, T2, T3, T4, T5, T6, T7>(Action<TMock> eventExpression, Func<T1, T2, T3, T4, T5, T6, T7, EventArgs> func)
		{
			return this.RaisesImpl(eventExpression, func);
		}

		public IReturnsResult<TMock> Returns<T1, T2, T3, T4, T5, T6, T7>(Func<T1, T2, T3, T4, T5, T6, T7, TResult> valueExpression)
		{
			this.SetReturnDelegate(valueExpression);
			return this;
		}

		[SuppressMessage("Microsoft.Design", "CA1061:DoNotHideBaseClassMethods", Justification = "This class provides typed members for the method-returning interfaces. It's never used through the base class type.")]
		public new IReturnsThrows<TMock, TResult> Callback<T1, T2, T3, T4, T5, T6, T7>(Action<T1, T2, T3, T4, T5, T6, T7> callback)
		{
			base.Callback(callback);
			return this;
		}

		public IVerifies Raises<T1, T2, T3, T4, T5, T6, T7, T8>(Action<TMock> eventExpression, Func<T1, T2, T3, T4, T5, T6, T7, T8, EventArgs> func)
		{
			return this.RaisesImpl(eventExpression, func);
		}

		public IReturnsResult<TMock> Returns<T1, T2, T3, T4, T5, T6, T7, T8>(Func<T1, T2, T3, T4, T5, T6, T7, T8, TResult> valueExpression)
		{
			this.SetReturnDelegate(valueExpression);
			return this;
		}

		[SuppressMessage("Microsoft.Design", "CA1061:DoNotHideBaseClassMethods", Justification = "This class provides typed members for the method-returning interfaces. It's never used through the base class type.")]
		public new IReturnsThrows<TMock, TResult> Callback<T1, T2, T3, T4, T5, T6, T7, T8>(Action<T1, T2, T3, T4, T5, T6, T7, T8> callback)
		{
			base.Callback(callback);
			return this;
		}

		public IVerifies Raises<T1, T2, T3, T4, T5, T6, T7, T8, T9>(Action<TMock> eventExpression, Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, EventArgs> func)
		{
			return this.RaisesImpl(eventExpression, func);
		}

		public IReturnsResult<TMock> Returns<T1, T2, T3, T4, T5, T6, T7, T8, T9>(Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TResult> valueExpression)
		{
			this.SetReturnDelegate(valueExpression);
			return this;
		}

		[SuppressMessage("Microsoft.Design", "CA1061:DoNotHideBaseClassMethods", Justification = "This class provides typed members for the method-returning interfaces. It's never used through the base class type.")]
		public new IReturnsThrows<TMock, TResult> Callback<T1, T2, T3, T4, T5, T6, T7, T8, T9>(Action<T1, T2, T3, T4, T5, T6, T7, T8, T9> callback)
		{
			base.Callback(callback);
			return this;
		}

		public IVerifies Raises<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(Action<TMock> eventExpression, Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, EventArgs> func)
		{
			return this.RaisesImpl(eventExpression, func);
		}

		public IReturnsResult<TMock> Returns<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TResult> valueExpression)
		{
			this.SetReturnDelegate(valueExpression);
			return this;
		}

		[SuppressMessage("Microsoft.Design", "CA1061:DoNotHideBaseClassMethods", Justification = "This class provides typed members for the method-returning interfaces. It's never used through the base class type.")]
		public new IReturnsThrows<TMock, TResult> Callback<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10> callback)
		{
			base.Callback(callback);
			return this;
		}

		public IVerifies Raises<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(Action<TMock> eventExpression, Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, EventArgs> func)
		{
			return this.RaisesImpl(eventExpression, func);
		}

		public IReturnsResult<TMock> Returns<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, TResult> valueExpression)
		{
			this.SetReturnDelegate(valueExpression);
			return this;
		}

		[SuppressMessage("Microsoft.Design", "CA1061:DoNotHideBaseClassMethods", Justification = "This class provides typed members for the method-returning interfaces. It's never used through the base class type.")]
		public new IReturnsThrows<TMock, TResult> Callback<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> callback)
		{
			base.Callback(callback);
			return this;
		}

		public IVerifies Raises<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(Action<TMock> eventExpression, Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, EventArgs> func)
		{
			return this.RaisesImpl(eventExpression, func);
		}

		public IReturnsResult<TMock> Returns<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, TResult> valueExpression)
		{
			this.SetReturnDelegate(valueExpression);
			return this;
		}

		[SuppressMessage("Microsoft.Design", "CA1061:DoNotHideBaseClassMethods", Justification = "This class provides typed members for the method-returning interfaces. It's never used through the base class type.")]
		public new IReturnsThrows<TMock, TResult> Callback<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> callback)
		{
			base.Callback(callback);
			return this;
		}

		public IVerifies Raises<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(Action<TMock> eventExpression, Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, EventArgs> func)
		{
			return this.RaisesImpl(eventExpression, func);
		}

		public IReturnsResult<TMock> Returns<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TResult> valueExpression)
		{
			this.SetReturnDelegate(valueExpression);
			return this;
		}

		[SuppressMessage("Microsoft.Design", "CA1061:DoNotHideBaseClassMethods", Justification = "This class provides typed members for the method-returning interfaces. It's never used through the base class type.")]
		public new IReturnsThrows<TMock, TResult> Callback<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13> callback)
		{
			base.Callback(callback);
			return this;
		}

		public IVerifies Raises<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(Action<TMock> eventExpression, Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, EventArgs> func)
		{
			return this.RaisesImpl(eventExpression, func);
		}

		public IReturnsResult<TMock> Returns<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, TResult> valueExpression)
		{
			this.SetReturnDelegate(valueExpression);
			return this;
		}

		[SuppressMessage("Microsoft.Design", "CA1061:DoNotHideBaseClassMethods", Justification = "This class provides typed members for the method-returning interfaces. It's never used through the base class type.")]
		public new IReturnsThrows<TMock, TResult> Callback<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14> callback)
		{
			base.Callback(callback);
			return this;
		}

		public IVerifies Raises<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(Action<TMock> eventExpression, Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, EventArgs> func)
		{
			return this.RaisesImpl(eventExpression, func);
		}

		public IReturnsResult<TMock> Returns<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, TResult> valueExpression)
		{
			this.SetReturnDelegate(valueExpression);
			return this;
		}

		[SuppressMessage("Microsoft.Design", "CA1061:DoNotHideBaseClassMethods", Justification = "This class provides typed members for the method-returning interfaces. It's never used through the base class type.")]
		public new IReturnsThrows<TMock, TResult> Callback<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15> callback)
		{
			base.Callback(callback);
			return this;
		}

		public IVerifies Raises<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16>(Action<TMock> eventExpression, Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, EventArgs> func)
		{
			return this.RaisesImpl(eventExpression, func);
		}

		public IReturnsResult<TMock> Returns<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16>(Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, TResult> valueExpression)
		{
			this.SetReturnDelegate(valueExpression);
			return this;
		}

		[SuppressMessage("Microsoft.Design", "CA1061:DoNotHideBaseClassMethods", Justification = "This class provides typed members for the method-returning interfaces. It's never used through the base class type.")]
		public new IReturnsThrows<TMock, TResult> Callback<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16>(Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16> callback)
		{
			base.Callback(callback);
			return this;
		}
	}
}
