// Copyright (c) 2007, Clarius Consulting, Manas Technology Solutions, InSTEDD.
// All rights reserved. Licensed under the BSD 3-Clause License; see License.txt.

using Xunit;

namespace Moq.Tests
{
	public class AmbientObserverFixture
	{
		[Fact]
		public void Activations_can_be_nested()
		{
			Assert.False(AmbientObserver.IsActive(out var active));
			using (var outer = AmbientObserver.Activate())
			{
				Assert.True(AmbientObserver.IsActive(out active));
				Assert.Same(outer, active);
				using (var inner = AmbientObserver.Activate())
				{
					Assert.True(AmbientObserver.IsActive(out active));
					Assert.Same(inner, active);
				}
				Assert.True(AmbientObserver.IsActive(out active));
				Assert.Same(outer, active);
			}
			Assert.False(AmbientObserver.IsActive(out active));
		}

		[Fact]
		public void Nested_observers_do_not_share_their_observations()
		{
			using (var outer = AmbientObserver.Activate())
			{
				_ = CreateMatch();
				Assert.True(outer.LastIsMatch(out var outerLastMatchBeforeInner));
				using (var inner = AmbientObserver.Activate())
				{
					// The inner observer should not see the outer observer's match:
					Assert.False(inner.LastIsMatch(out _));
					_ = CreateMatch();
				}
				// And the outer observer should not see the (disposed) inner observer's match.
				// Instead, it should still see the same match as the last one as before `inner`:
				Assert.True(outer.LastIsMatch(out var outerLastMatchAfterInnerDisposed));
				Assert.Same(outerLastMatchBeforeInner, outerLastMatchAfterInnerDisposed);
			}
		}

		[Fact]
		public void Nested_observers_when_disposed_dont_interrupt_outer_observers()
		{
			using (var outer = AmbientObserver.Activate())
			{
				using (var inner = AmbientObserver.Activate())
				{
				}
				_ = CreateMatch();
				Assert.True(outer.LastIsMatch(out var match));
			}
		}

		private int CreateMatch()
		{
			return Match.Create<int>(i => i != 0, () => It.Is<int>(i => i != 0));
		}

		[Fact]
		public void IsActive_returns_false_when_no_AmbientObserver_instantiated()
		{
			Assert.False(AmbientObserver.IsActive(out _));
		}

		[Fact]
		public void IsActive_returns_true_when_AmbientObserver_instantiated()
		{
			using (var observer = AmbientObserver.Activate())
			{
				Assert.True(AmbientObserver.IsActive(out _));
			}
		}

		[Fact]
		public void IsActive_returns_right_AmbientObserver_when_AmbientObserver_instantiated()
		{
			using (var observer = AmbientObserver.Activate())
			{
				Assert.True(AmbientObserver.IsActive(out var active));
				Assert.Same(observer, active);
			}
		}

		[Fact]
		public void LastObservationWasMatcher_returns_false_after_no_invocations()
		{
			using (var observer = AmbientObserver.Activate())
			{
				Assert.False(observer.LastIsMatch(out _));
			}
		}

		[Fact]
		public void LastObservationWasMatcher_returns_false_after_a_mock_invocation()
		{
			var mock = Mock.Of<IMockable>();

			using (var observer = AmbientObserver.Activate())
			{
				mock.Method(default, default);

				Assert.False(observer.LastIsMatch(out _));
			}
		}

		[Fact]
		public void LastObservationWasMatcher_returns_true_after_a_matcher_invocation()
		{
			using (var observer = AmbientObserver.Activate())
			{
				_ = It.IsAny<int>();

				Assert.True(observer.LastIsMatch(out _));
			}
		}

		[Fact]
		public void LastObservationWasMatcher_returns_right_matcher_after_several_matcher_invocations()
		{
			using (var observer = AmbientObserver.Activate())
			{
				_ = It.IsAny<int>();
				_ = It.IsRegex(".*");

				Assert.True(observer.LastIsMatch(out var last));
				Assert.True(last.Matches("abc"));
			}
		}

		[Fact]
		public void LastObservationWasMockInvocation_returns_false_after_no_invocations()
		{
			using (var observer = AmbientObserver.Activate())
			{
				Assert.False(observer.LastIsInvocation(out _, out _, out _));
			}
		}

		[Fact]
		public void LastObservationWasMockInvocation_returns_true_after_a_mock_invocation()
		{
			var mock = Mock.Of<IMockable>();

			using (var observer = AmbientObserver.Activate())
			{
				mock.Method(default, default);

				Assert.True(observer.LastIsInvocation(out _, out _, out _));
			}
		}

		[Fact]
		public void LastObservationWasMockInvocation_returns_last_invoation_after_several_mock_invocations()
		{
			var mock = Mock.Of<IMockable>();

			using (var observer = AmbientObserver.Activate())
			{
				mock.Method(default, default);
				mock.Method(42, "*");

				Assert.True(observer.LastIsInvocation(out _, out var last, out _));
				Assert.Equal(42, last.Arguments[0]);
				Assert.Equal("*", last.Arguments[1]);
			}
		}

		[Fact]
		public void LastObservationWasMockInvocation_returns_right_matchers_after_mock_invocation()
		{
			var mock = Mock.Of<IMockable>();

			using (var observer = AmbientObserver.Activate())
			{
				mock.Method(It.IsAny<int>(), It.IsRegex("abc"));

				Assert.True(observer.LastIsInvocation(out _, out var _, out var matches));
				Assert.Equal(2, matches.Count);
				Assert.True(matches[0].Matches(42));
				Assert.True(matches[1].Matches("abc"));
			}
		}

		[Fact]
		public void LastObservationWasMockInvocation_does_not_return_matchers_of_previous_mock_invocation()
		{
			var mock = Mock.Of<IMockable>();

			using (var observer = AmbientObserver.Activate())
			{
				mock.Method(It.IsInRange(1, 10, Range.Inclusive), "*");
				mock.Method(It.IsAny<int>(), It.IsRegex("abc"));

				Assert.True(observer.LastIsInvocation(out _, out var _, out var matches));
				Assert.Equal(2, matches.Count);
				Assert.True(matches[0].Matches(42));
				Assert.True(matches[1].Matches("abc"));
			}
		}

		public interface IMockable
		{
			void Method(int arg1, string arg2);
		}
	}
}
