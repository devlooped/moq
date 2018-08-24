using System;
using System.Diagnostics.CodeAnalysis;
using Moq.Language;
using Moq.Language.Flow;

namespace Moq
{
	internal partial class MethodCall<TMock>
	{
		public void Raises<T>(Action<TMock> eventExpression, Func<T, EventArgs> func)
		{
			RaisesImpl(eventExpression, func);
		}

		public void Raises<T1, T2>(Action<TMock> eventExpression, Func<T1, T2, EventArgs> func)
		{
			RaisesImpl(eventExpression, func);
		}

		public void Raises<T1, T2, T3>(Action<TMock> eventExpression, Func<T1, T2, T3, EventArgs> func)
		{
			RaisesImpl(eventExpression, func);
		}

		public void Raises<T1, T2, T3, T4>(Action<TMock> eventExpression, Func<T1, T2, T3, T4, EventArgs> func)
		{
			RaisesImpl(eventExpression, func);
		}

		public void Raises<T1, T2, T3, T4, T5>(Action<TMock> eventExpression, Func<T1, T2, T3, T4, T5, EventArgs> func)
		{
			RaisesImpl(eventExpression, func);
		}

		public void Raises<T1, T2, T3, T4, T5, T6>(Action<TMock> eventExpression, Func<T1, T2, T3, T4, T5, T6, EventArgs> func)
		{
			RaisesImpl(eventExpression, func);
		}

		public void Raises<T1, T2, T3, T4, T5, T6, T7>(Action<TMock> eventExpression, Func<T1, T2, T3, T4, T5, T6, T7, EventArgs> func)
		{
			RaisesImpl(eventExpression, func);
		}

		public void Raises<T1, T2, T3, T4, T5, T6, T7, T8>(Action<TMock> eventExpression, Func<T1, T2, T3, T4, T5, T6, T7, T8, EventArgs> func)
		{
			RaisesImpl(eventExpression, func);
		}

		public void Raises<T1, T2, T3, T4, T5, T6, T7, T8, T9>(Action<TMock> eventExpression, Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, EventArgs> func)
		{
			RaisesImpl(eventExpression, func);
		}

		public void Raises<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(Action<TMock> eventExpression, Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, EventArgs> func)
		{
			RaisesImpl(eventExpression, func);
		}

		public void Raises<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(Action<TMock> eventExpression, Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, EventArgs> func)
		{
			RaisesImpl(eventExpression, func);
		}

		public void Raises<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(Action<TMock> eventExpression, Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, EventArgs> func)
		{
			RaisesImpl(eventExpression, func);
		}

		public void Raises<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(Action<TMock> eventExpression, Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, EventArgs> func)
		{
			RaisesImpl(eventExpression, func);
		}

		public void Raises<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(Action<TMock> eventExpression, Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, EventArgs> func)
		{
			RaisesImpl(eventExpression, func);
		}

		public void Raises<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(Action<TMock> eventExpression, Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, EventArgs> func)
		{
			RaisesImpl(eventExpression, func);
		}

		public void Raises<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16>(Action<TMock> eventExpression, Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, EventArgs> func)
		{
			RaisesImpl(eventExpression, func);
		}
	}
}
