using System;
using System.Collections.Generic;
using Avatars;
using Xunit;

namespace Moq.Sdk.Tests
{
    public class EqualityTests
    {
        [Fact]
        public void MethodInvocationEqualsByTargetMethodArgs()
        {
            var method = new Action<bool, string, PlatformID>(AMethod).Method;
            var args = new object[] { true, "foo", PlatformID.Win32NT };
            var target = this;

            IEquatable<IMethodInvocation> equatable = Invocation(target, method, args);
            IEquatable<IMethodInvocation> other = Invocation(target, method, args);

            Assert.True(equatable.Equals(other));
            Assert.Equal(equatable.GetHashCode(), other.GetHashCode());
        }

        [Fact]
        public void MethodInvocationEqualsEvenIfContextDiffers()
        {
            var method = new Action<bool, string, PlatformID>(AMethod).Method;
            var args = new object[] { true, "foo", PlatformID.Win32NT };
            var target = this;

            var equatable = Invocation(target, method, args);
            equatable.Context["foo"] = "bar";
            var other = Invocation(target, method, args);

            Assert.True(equatable.Equals(other));
        }

        [Fact]
        public void MethodInvocationDoesNotEqualNull()
        {
            var method = new Action<bool, string, PlatformID>(AMethod).Method;
            var args = new object[] { true, "foo", PlatformID.Win32NT };
            var target = this;

            var equatable = Invocation(target, method, args);

            Assert.False(equatable.Equals(null));
        }

        [Fact]
        public void MethodInvocationDoesNotEqualOtherObject()
        {
            var method = new Action<bool, string, PlatformID>(AMethod).Method;
            var args = new object[] { true, "foo", PlatformID.Win32NT };
            var target = this;

            var equatable = Invocation(target, method, args);

            Assert.False(equatable.Equals(new object()));
        }

        [Fact]
        public void ValueMatcherEqualsByTypeAndValue()
        {
            var matcher = new ValueMatcher(typeof(string), "foo");
            var other = new ValueMatcher(typeof(string), "foo");

            Assert.True(matcher.Equals(other));
            Assert.Equal(matcher.GetHashCode(), other.GetHashCode());

            Assert.False(matcher.Equals(new ValueMatcher(typeof(string), "bar")));
            Assert.False(matcher.Equals(new ValueMatcher(typeof(object), "foo")));
        }

        [Fact]
        public void TupleOfEqualMatchersAreEqual()
        {
            var any = AnyMatcher<string>.Default;
            Func<string, bool> condition = s => s.Length > 0;
            var conditional = new ConditionalMatcher<string>(condition, "foo");
            var value = new ValueMatcher(typeof(string), "foo");

            var t1 = Tuple.Create(any, conditional, value);
            var t2 = Tuple.Create(any, conditional, value);

            Assert.True(t1.Equals(t2));
            Assert.Equal(t1.GetHashCode(), t2.GetHashCode());
        }

        [Fact]
        public void MockSetupEqualsByInvocationAndMatchers()
        {
            var method = new Action<bool, string, PlatformID>(AMethod).Method;
            var args = new object[] { true, "foo", PlatformID.Win32NT };
            var target = this;

            var any = AnyMatcher<string>.Default;
            Func<string, bool> condition = s => s.Length > 0;
            var conditional = new ConditionalMatcher<string>(condition, "foo");
            var value = new ValueMatcher(typeof(string), "foo");

            var setup = new MockSetup(Invocation(target, method, args), new[] { any, conditional, value });
            var other = new MockSetup(Invocation(target, method, args), new[] { any, conditional, value });

            Assert.True(setup.Equals(other));
            Assert.Equal(setup.GetHashCode(), other.GetHashCode());

            var hash = new HashSet<IMockSetup>();
            Assert.True(hash.Add(setup));
            Assert.False(hash.Add(other));
            Assert.Contains(setup, hash);
            Assert.Contains(other, hash);

            Assert.False(setup.Equals(
                new MockSetup(Invocation(new object(), method, args), new[] { any, conditional, value })));
        }

        [Fact]
        public void Matchers()
        {
            var hash = new HashSet<IArgumentMatcher>();
            var value1 = new ValueMatcher(typeof(string), "foo");
            var value2 = new ValueMatcher(typeof(string), "foo");

            Assert.True(hash.Add(value1));
            Assert.False(hash.Add(value2));
        }

        void AMethod(bool b, string s, PlatformID p) { }

        static MethodInvocation Invocation(object target, System.Reflection.MethodBase method, object[] args)
            => new MethodInvocation(target, method, ArgumentCollection.Create(
                method.GetParameters(),
                (bool)args[0],
                (string)args[1],
                (PlatformID)args[2]));
    }
}
