using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Moq.Language;

namespace Moq
{
	/// <summary>
	/// Helper for sequencing return values in the same method.
	/// </summary>
	public static class SequenceExtensions
	{
		/// <summary>
		/// Return a sequence of values, once per call.
		/// </summary>
		[SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "By Design")]
		public static ISetupSequentialResult<TResult> SetupSequence<TMock, TResult>(
			this Mock<TMock> mock,
			Expression<Func<TMock, TResult>> expression)
			where TMock : class
		{
			return new SetupSequentialContext<TMock, TResult>(mock, expression);
		}

		/// <summary>
		/// Return a sequence of tasks, once per call.
		/// </summary>
		public static ISetupSequentialResult<Task<TResult>> ReturnsAsync<TResult>(this ISetupSequentialResult<Task<TResult>> setup, TResult value)
		{
			var tcs = new TaskCompletionSource<TResult>();
			tcs.SetResult(value);

			return setup.Returns(tcs.Task);
		}

		/// <summary>
		/// Throws a sequence of exceptions, once per call.
		/// </summary>
		public static ISetupSequentialResult<Task<TResult>> ThrowsAsync<TResult>(this ISetupSequentialResult<Task<TResult>> setup, Exception exception)
		{
			var tcs = new TaskCompletionSource<TResult>();
			tcs.SetException(exception);

			return setup.Returns(tcs.Task);
		}
	}
}