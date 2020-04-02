// Copyright (c) 2007, Clarius Consulting, Manas Technology Solutions, InSTEDD.
// All rights reserved. Licensed under the BSD 3-Clause License; see License.txt.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.Serialization;
using System.Security;
using System.Text;

using Moq.Language;
using Moq.Properties;

namespace Moq
{
	/// <summary>
	/// Exception thrown by mocks when they are not properly set up,
	/// when setups are not matched, when verification fails, etc.
	/// </summary>
	/// <remarks>
	/// A distinct exception type is provided so that exceptions
	/// thrown by a mock can be distinguished from other exceptions
	/// that might be thrown in tests.
	/// <para>
	/// Moq does not provide a richer hierarchy of exception types, as
	/// tests typically should <em>not</em> catch or expect exceptions
	/// from mocks. These are typically the result of changes
	/// in the tested class or its collaborators' implementation, and
	/// result in fixes in the mock setup so that they disappear and
	/// allow the test to pass.
	/// </para>
	/// </remarks>
	[SuppressMessage("Microsoft.Design", "CA1032:ImplementStandardExceptionConstructors", Justification = "It's only initialized internally.")]
	[Serializable]
	public class MockException : Exception
	{
		/// <summary>
		///   Returns the exception to be thrown when a setup limited by <see cref="IOccurrence.AtMostOnce()"/> is matched more often than once.
		/// </summary>
		internal static MockException MoreThanOneCall(MethodCall setup, int invocationCount)
		{
			var message = new StringBuilder();
			message.AppendLine(setup.FailMessage ?? "")
			       .Append(Times.AtMostOnce().GetExceptionMessage(invocationCount))
			       .AppendLine(setup.Expression.ToStringFixed());

			return new MockException(MockExceptionReasons.MoreThanOneCall, message.ToString());
		}

		/// <summary>
		///   Returns the exception to be thrown when a setup limited by <see cref="IOccurrence.AtMost(int)"/> is matched more often than the specified maximum number of times.
		/// </summary>
		internal static MockException MoreThanNCalls(MethodCall setup, int maxInvocationCount, int invocationCount)
		{
			var message = new StringBuilder();
			message.AppendLine(setup.FailMessage ?? "")
			       .Append(Times.AtMost(maxInvocationCount).GetExceptionMessage(invocationCount))
			       .AppendLine(setup.Expression.ToStringFixed());

			return new MockException(MockExceptionReasons.MoreThanNCalls, message.ToString());
		}

		/// <summary>
		///   Returns the exception to be thrown when <see cref="Mock.Verify"/> finds no invocations (or the wrong number of invocations) that match the specified expectation.
		/// </summary>
		internal static MockException NoMatchingCalls(
			Mock rootMock,
			LambdaExpression expression,
			string failMessage,
			Times times,
			int callCount)
		{
			var message = new StringBuilder();
			message.AppendLine(failMessage ?? "")
			       .Append(times.GetExceptionMessage(callCount))
			       .AppendLine(expression.PartialMatcherAwareEval().ToStringFixed())
			       .AppendLine()
			       .AppendLine(Resources.PerformedInvocations)
			       .AppendLine();

			var visitedMocks = new HashSet<Mock>();

			var mocks = new Queue<Mock>();
			mocks.Enqueue(rootMock);

			while (mocks.Any())
			{
				var mock = mocks.Dequeue();

				if (visitedMocks.Contains(mock)) continue;
				visitedMocks.Add(mock);

				message.AppendLine(mock == rootMock ? $"   {mock} ({expression.Parameters[0].Name}):"
					                                : $"   {mock}:");

				var invocations = mock.MutableInvocations.ToArray();
				if (invocations.Any())
				{
					message.AppendLine();
					foreach (var invocation in invocations)
					{
						message.Append($"      {invocation}");

						if (invocation.Method.ReturnType != typeof(void) && Unwrap.ResultIfCompletedTask(invocation.ReturnValue) is IMocked mocked)
						{
							var innerMock = mocked.Mock;
							mocks.Enqueue(innerMock);
							message.Append($"  => {innerMock}");
						}

						message.AppendLine();
					}
				}
				else
				{
					message.AppendLine($"   {Resources.NoInvocationsPerformed}");
				}

				message.AppendLine();
			}

			return new MockException(MockExceptionReasons.NoMatchingCalls, message.TrimEnd().AppendLine().ToString());
		}

		/// <summary>
		///   Returns the exception to be thrown when a strict mock has no setup corresponding to the specified invocation.
		/// </summary>
		internal static MockException NoSetup(Invocation invocation)
		{
			return new MockException(
				MockExceptionReasons.NoSetup,
				string.Format(
					CultureInfo.CurrentCulture,
					Resources.MockExceptionMessage,
					invocation.ToString(),
					MockBehavior.Strict,
					Resources.NoSetup));
		}

		/// <summary>
		///   Returns the exception to be thrown when a strict mock has no setup that provides a return value for the specified invocation.
		/// </summary>
		internal static MockException ReturnValueRequired(Invocation invocation)
		{
			return new MockException(
				MockExceptionReasons.ReturnValueRequired,
				string.Format(
					CultureInfo.CurrentCulture,
					Resources.MockExceptionMessage,
					invocation.ToString(),
					MockBehavior.Strict,
					Resources.ReturnValueRequired));
		}

		/// <summary>
		///   Returns the exception to be thrown when a setup was not matched.
		/// </summary>
		internal static MockException UnmatchedSetup(Setup setup)
		{
			return new MockException(
				MockExceptionReasons.UnmatchedSetup,
				string.Format(
					CultureInfo.CurrentCulture,
					Resources.UnmatchedSetup,
					setup));
		}

		internal static MockException FromInnerMockOf(ISetup setup, MockException error)
		{
			var message = new StringBuilder();

			message.AppendLine(string.Format(CultureInfo.CurrentCulture, Resources.VerificationErrorsOfInnerMock, setup)).TrimEnd().AppendLine()
			       .AppendLine();

			message.AppendIndented(error.Message, count: 3);

			return new MockException(error.Reasons, message.ToString());
		}

		/// <summary>
		///   Returns an exception whose message is the concatenation of the given <paramref name="errors"/>' messages
		///   and whose reason(s) is the combination of the given <paramref name="errors"/>' reason(s).
		///   Used by <see cref="MockFactory.VerifyMocks(Action{Mock})"/> when it finds one or more mocks with verification errors.
		/// </summary>
		internal static MockException Combined(IEnumerable<MockException> errors, string preamble)
		{
			Debug.Assert(errors != null);
			Debug.Assert(errors.Any());

			var reasons = default(MockExceptionReasons);
			var message = new StringBuilder();

			if (preamble != null)
			{
				message.Append(preamble).TrimEnd().AppendLine()
				       .AppendLine();
			}

			foreach (var error in errors)
			{
				reasons |= error.Reasons;
				message.AppendIndented(error.Message, count: 3).TrimEnd().AppendLine()
				       .AppendLine();
			}

			return new MockException(reasons, message.TrimEnd().ToString());
		}

		/// <summary>
		///   Returns the exception to be thrown when <see cref="Mock.VerifyNoOtherCalls(Mock)"/> finds invocations that have not been verified.
		/// </summary>
		internal static MockException UnverifiedInvocations(Mock mock, IEnumerable<Invocation> invocations)
		{
			var message = new StringBuilder();

			message.AppendLine(string.Format(CultureInfo.CurrentCulture, Resources.UnverifiedInvocations, mock)).TrimEnd().AppendLine()
			       .AppendLine();

			foreach (var invocation in invocations)
			{
				message.AppendIndented(invocation.ToString(), count: 3).TrimEnd().AppendLine();
			}

			return new MockException(MockExceptionReasons.UnverifiedInvocations, message.TrimEnd().ToString());
		}

		private readonly MockExceptionReasons reasons;

		private MockException(MockExceptionReasons reasons, string message)
			: base(message)
		{
			this.reasons = reasons;
		}

		internal MockExceptionReasons Reasons => this.reasons;

		/// <summary>
		/// Indicates whether this exception is a verification fault raised by Verify()
		/// </summary>
		public bool IsVerificationError
		{
			get
			{
				const MockExceptionReasons verificationErrorMask = MockExceptionReasons.NoMatchingCalls
				                                                 | MockExceptionReasons.UnmatchedSetup
				                                                 | MockExceptionReasons.UnverifiedInvocations;
				return (this.reasons & verificationErrorMask) != 0;
			}
		}

		/// <summary>
		/// Supports the serialization infrastructure.
		/// </summary>
		/// <param name="info">Serialization information.</param>
		/// <param name="context">Streaming context.</param>
		protected MockException(
		  System.Runtime.Serialization.SerializationInfo info,
		  System.Runtime.Serialization.StreamingContext context)
			: base(info, context)
		{
			this.reasons = (MockExceptionReasons)info.GetValue(nameof(this.reasons), typeof(MockExceptionReasons));
		}

		/// <summary>
		/// Supports the serialization infrastructure.
		/// </summary>
		/// <param name="info">Serialization information.</param>
		/// <param name="context">Streaming context.</param>
		[SecurityCritical]
		[SuppressMessage("Microsoft.Security", "CA2123:OverrideLinkDemandsShouldBeIdenticalToBase")]
		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue(nameof(this.reasons), this.reasons);
		}
	}
}
