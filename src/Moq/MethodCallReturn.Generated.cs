using System;
using System.Diagnostics.CodeAnalysis;
using Moq.Language;
using Moq.Language.Flow;

namespace Moq
{
	internal sealed partial class MethodCallReturn<TMock, TResult>
	{
		public void Raises<T>(Action<TMock> eventExpression, Func<T, EventArgs> func)
		{
			this.RaisesImpl(eventExpression, func);
		}

		public void Returns<T>(Func<T, TResult> valueExpression)
		{
			this.SetReturnDelegate(valueExpression);
		}

		public void Raises<T1, T2>(Action<TMock> eventExpression, Func<T1, T2, EventArgs> func)
		{
			this.RaisesImpl(eventExpression, func);
		}

		public void Returns<T1, T2>(Func<T1, T2, TResult> valueExpression)
		{
			this.SetReturnDelegate(valueExpression);
		}

		public void Raises<T1, T2, T3>(Action<TMock> eventExpression, Func<T1, T2, T3, EventArgs> func)
		{
			this.RaisesImpl(eventExpression, func);
		}

		public void Returns<T1, T2, T3>(Func<T1, T2, T3, TResult> valueExpression)
		{
			this.SetReturnDelegate(valueExpression);
		}

		public void Raises<T1, T2, T3, T4>(Action<TMock> eventExpression, Func<T1, T2, T3, T4, EventArgs> func)
		{
			this.RaisesImpl(eventExpression, func);
		}

		public void Returns<T1, T2, T3, T4>(Func<T1, T2, T3, T4, TResult> valueExpression)
		{
			this.SetReturnDelegate(valueExpression);
		}

		public void Raises<T1, T2, T3, T4, T5>(Action<TMock> eventExpression, Func<T1, T2, T3, T4, T5, EventArgs> func)
		{
			this.RaisesImpl(eventExpression, func);
		}

		public void Returns<T1, T2, T3, T4, T5>(Func<T1, T2, T3, T4, T5, TResult> valueExpression)
		{
			this.SetReturnDelegate(valueExpression);
		}

		public void Raises<T1, T2, T3, T4, T5, T6>(Action<TMock> eventExpression, Func<T1, T2, T3, T4, T5, T6, EventArgs> func)
		{
			this.RaisesImpl(eventExpression, func);
		}

		public void Returns<T1, T2, T3, T4, T5, T6>(Func<T1, T2, T3, T4, T5, T6, TResult> valueExpression)
		{
			this.SetReturnDelegate(valueExpression);
		}

		public void Raises<T1, T2, T3, T4, T5, T6, T7>(Action<TMock> eventExpression, Func<T1, T2, T3, T4, T5, T6, T7, EventArgs> func)
		{
			this.RaisesImpl(eventExpression, func);
		}

		public void Returns<T1, T2, T3, T4, T5, T6, T7>(Func<T1, T2, T3, T4, T5, T6, T7, TResult> valueExpression)
		{
			this.SetReturnDelegate(valueExpression);
		}

		public void Raises<T1, T2, T3, T4, T5, T6, T7, T8>(Action<TMock> eventExpression, Func<T1, T2, T3, T4, T5, T6, T7, T8, EventArgs> func)
		{
			this.RaisesImpl(eventExpression, func);
		}

		public void Returns<T1, T2, T3, T4, T5, T6, T7, T8>(Func<T1, T2, T3, T4, T5, T6, T7, T8, TResult> valueExpression)
		{
			this.SetReturnDelegate(valueExpression);
		}

		public void Raises<T1, T2, T3, T4, T5, T6, T7, T8, T9>(Action<TMock> eventExpression, Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, EventArgs> func)
		{
			this.RaisesImpl(eventExpression, func);
		}

		public void Returns<T1, T2, T3, T4, T5, T6, T7, T8, T9>(Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TResult> valueExpression)
		{
			this.SetReturnDelegate(valueExpression);
		}

		public void Raises<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(Action<TMock> eventExpression, Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, EventArgs> func)
		{
			this.RaisesImpl(eventExpression, func);
		}

		public void Returns<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TResult> valueExpression)
		{
			this.SetReturnDelegate(valueExpression);
		}

		public void Raises<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(Action<TMock> eventExpression, Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, EventArgs> func)
		{
			this.RaisesImpl(eventExpression, func);
		}

		public void Returns<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, TResult> valueExpression)
		{
			this.SetReturnDelegate(valueExpression);
		}

		public void Raises<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(Action<TMock> eventExpression, Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, EventArgs> func)
		{
			this.RaisesImpl(eventExpression, func);
		}

		public void Returns<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, TResult> valueExpression)
		{
			this.SetReturnDelegate(valueExpression);
		}

		public void Raises<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(Action<TMock> eventExpression, Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, EventArgs> func)
		{
			this.RaisesImpl(eventExpression, func);
		}

		public void Returns<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TResult> valueExpression)
		{
			this.SetReturnDelegate(valueExpression);
		}

		public void Raises<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(Action<TMock> eventExpression, Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, EventArgs> func)
		{
			this.RaisesImpl(eventExpression, func);
		}

		public void Returns<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, TResult> valueExpression)
		{
			this.SetReturnDelegate(valueExpression);
		}

		public void Raises<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(Action<TMock> eventExpression, Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, EventArgs> func)
		{
			this.RaisesImpl(eventExpression, func);
		}

		public void Returns<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, TResult> valueExpression)
		{
			this.SetReturnDelegate(valueExpression);
		}

		public void Raises<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16>(Action<TMock> eventExpression, Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, EventArgs> func)
		{
			this.RaisesImpl(eventExpression, func);
		}

		public void Returns<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16>(Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, TResult> valueExpression)
		{
			this.SetReturnDelegate(valueExpression);
		}
	}
}
