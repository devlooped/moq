// Copyright (c) 2007, Clarius Consulting, Manas Technology Solutions, InSTEDD, and Contributors.
// All rights reserved. Licensed under the BSD 3-Clause License; see License.txt.

using Xunit;

namespace Moq.Tests
{
	public class MatcherObserverFixture
	{
		[Fact]
		public void Activations_can_be_nested()
		{
			Assert.False(MatcherObserver.IsActive(out var active));
			using (var outer = MatcherObserver.Activate())
			{
				Assert.True(MatcherObserver.IsActive(out active));
				Assert.Same(outer, active);
				using (var inner = MatcherObserver.Activate())
				{
					Assert.True(MatcherObserver.IsActive(out active));
					Assert.Same(inner, active);
				}
				Assert.True(MatcherObserver.IsActive(out active));
				Assert.Same(outer, active);
			}
			Assert.False(MatcherObserver.IsActive(out active));
		}

		[Fact]
		public void Nested_observers_do_not_share_their_observations()
		{
			using (var outer = MatcherObserver.Activate())
			{
				_ = CreateMatch();
				Assert.True(outer.TryGetLastMatch(out var outerLastMatchBeforeInner));
				using (var inner = MatcherObserver.Activate())
				{
					// The inner observer should not see the outer observer's match:
					Assert.False(inner.TryGetLastMatch(out _));
					_ = CreateMatch();
				}
				// And the outer observer should not see the (disposed) inner observer's match.
				// Instead, it should still see the same match as the last one as before `inner`:
				Assert.True(outer.TryGetLastMatch(out var outerLastMatchAfterInnerDisposed));
				Assert.Same(outerLastMatchBeforeInner, outerLastMatchAfterInnerDisposed);
			}
		}

		[Fact]
		public void Nested_observers_when_disposed_dont_interrupt_outer_observers()
		{
			using (var outer = MatcherObserver.Activate())
			{
				using (var inner = MatcherObserver.Activate())
				{
				}
				_ = CreateMatch();
				Assert.True(outer.TryGetLastMatch(out var match));
			}
		}

		private int CreateMatch()
		{
			return Match.Create<int>(i => i != 0, () => It.Is<int>(i => i != 0));
		}

		[Fact]
		public void IsActive_returns_false_when_no_MatcherObserver_instantiated()
		{
			Assert.False(MatcherObserver.IsActive(out _));
		}

		[Fact]
		public void IsActive_returns_true_when_MatcherObserver_instantiated()
		{
			using (var observer = MatcherObserver.Activate())
			{
				Assert.True(MatcherObserver.IsActive(out _));
			}
		}

		[Fact]
		public void IsActive_returns_right_MatcherObserver_when_MatcherObserver_instantiated()
		{
			using (var observer = MatcherObserver.Activate())
			{
				Assert.True(MatcherObserver.IsActive(out var active));
				Assert.Same(observer, active);
			}
		}

		[Fact]
		public void TryGetLastMatch_returns_false_after_no_invocations()
		{
			using (var observer = MatcherObserver.Activate())
			{
				Assert.False(observer.TryGetLastMatch(out _));
			}
		}

		[Fact]
		public void TryGetLastMatch_returns_false_after_a_mock_invocation()
		{
			var mock = Mock.Of<IMockable>();

			using (var observer = MatcherObserver.Activate())
			{
				mock.Method(default, default);

				Assert.False(observer.TryGetLastMatch(out _));
			}
		}

		[Fact]
		public void TryGetLastMatch_returns_true_after_a_matcher_invocation()
		{
			using (var observer = MatcherObserver.Activate())
			{
				_ = It.IsAny<int>();

				Assert.True(observer.TryGetLastMatch(out _));
			}
		}

		[Fact]
		public void TryGetLastMatch_returns_right_matcher_after_several_matcher_invocations()
		{
			using (var observer = MatcherObserver.Activate())
			{
				_ = It.IsAny<int>();
				_ = It.IsRegex(".*");

				Assert.True(observer.TryGetLastMatch(out var last));
				Assert.True(last.Matches("abc", typeof(string)));
			}
		}

		public interface IMockable
		{
			void Method(int arg1, string arg2);
		}
	}
}
