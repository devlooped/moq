using System;
using System.Diagnostics.CodeAnalysis;
using Moq.Language;
using Moq.Language.Flow;

namespace Moq
{
	internal sealed partial class MethodCallReturn<TMock, TResult>
	{
		public void Returns<T>(Func<T, TResult> valueExpression)
		{
			this.SetReturnDelegate(valueExpression);
		}

		public void Returns<T1, T2>(Func<T1, T2, TResult> valueExpression)
		{
			this.SetReturnDelegate(valueExpression);
		}

		public void Returns<T1, T2, T3>(Func<T1, T2, T3, TResult> valueExpression)
		{
			this.SetReturnDelegate(valueExpression);
		}

		public void Returns<T1, T2, T3, T4>(Func<T1, T2, T3, T4, TResult> valueExpression)
		{
			this.SetReturnDelegate(valueExpression);
		}

		public void Returns<T1, T2, T3, T4, T5>(Func<T1, T2, T3, T4, T5, TResult> valueExpression)
		{
			this.SetReturnDelegate(valueExpression);
		}

		public void Returns<T1, T2, T3, T4, T5, T6>(Func<T1, T2, T3, T4, T5, T6, TResult> valueExpression)
		{
			this.SetReturnDelegate(valueExpression);
		}

		public void Returns<T1, T2, T3, T4, T5, T6, T7>(Func<T1, T2, T3, T4, T5, T6, T7, TResult> valueExpression)
		{
			this.SetReturnDelegate(valueExpression);
		}

		public void Returns<T1, T2, T3, T4, T5, T6, T7, T8>(Func<T1, T2, T3, T4, T5, T6, T7, T8, TResult> valueExpression)
		{
			this.SetReturnDelegate(valueExpression);
		}

		public void Returns<T1, T2, T3, T4, T5, T6, T7, T8, T9>(Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TResult> valueExpression)
		{
			this.SetReturnDelegate(valueExpression);
		}

		public void Returns<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TResult> valueExpression)
		{
			this.SetReturnDelegate(valueExpression);
		}

		public void Returns<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, TResult> valueExpression)
		{
			this.SetReturnDelegate(valueExpression);
		}

		public void Returns<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, TResult> valueExpression)
		{
			this.SetReturnDelegate(valueExpression);
		}

		public void Returns<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TResult> valueExpression)
		{
			this.SetReturnDelegate(valueExpression);
		}

		public void Returns<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, TResult> valueExpression)
		{
			this.SetReturnDelegate(valueExpression);
		}

		public void Returns<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, TResult> valueExpression)
		{
			this.SetReturnDelegate(valueExpression);
		}

		public void Returns<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16>(Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, TResult> valueExpression)
		{
			this.SetReturnDelegate(valueExpression);
		}
	}
}
