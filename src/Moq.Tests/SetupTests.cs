using System;
using System.Linq;
using System.Threading.Tasks;
using Moq.Sdk;
using Sample;
using Stunts;
using Xunit;
using static Moq.Syntax;

namespace Moq.Tests
{
    public class SetupTests
    {
        public delegate bool TryAddHandler(ref int x, ref int y, out int? z);

        public interface IAsyncService
        {
            Task<int> GetAsync(int id);
            ValueTask<int> GetLengthAsync(int id, string prefix);
            Task RunAsync();
        }

        static IMock<T> Create<T>() where T : class => Mock.Get(Mock.Of<T>());

        static ISetup<TDelegate, TResult> Typed<TDelegate, TResult>(Func<TResult> member)
        {
            using (SetupFactory.Begin())
            {
                member();
                return SetupFactory.Create<TDelegate, TResult>();
            }
        }

        static ISetup<TDelegate> Typed<TDelegate>(Action member)
        {
            using (SetupFactory.Begin())
            {
                member();
                return SetupFactory.Create<TDelegate>();
            }
        }

        [Fact]
        public void SetupDoesNotRecordInvocation()
        {
            var calculator = Create<ICalculator>();

            Setup(() => calculator.Object.Add(2, 3));
            Setup(() => calculator.Object.TurnOn());

            Assert.Empty(calculator.Sdk.Invocations);
        }

        [Fact]
        public void SetupExposesSdkSetup()
        {
            var calculator = Create<ICalculator>();

            var setup = Setup(() => calculator.Object.Add(2, 3));

            Assert.Equal(nameof(ICalculator.Add), setup.Sdk.Invocation.MethodBase.Name);
            Assert.Same(calculator.Object, setup.Sdk.Invocation.Target);
        }

        [Fact]
        public void SetupThrowsIfNoMockMemberInvoked()
            => Assert.Throws<InvalidOperationException>(() => Setup(() => 42));

        [Fact]
        public void CanSetMockName()
        {
            var calculator = Create<ICalculator>();

            calculator.Sdk.Name = "calc";

            Assert.Equal("calc", calculator.Sdk.Name);
        }

        [Fact]
        public void ReturnsValue()
        {
            var calculator = Create<ICalculator>();

            Setup(() => calculator.Object.Add(2, 3)).Returns(5);

            Assert.Equal(5, calculator.Object.Add(2, 3));
            Assert.Equal(0, calculator.Object.Add(1, 1));
        }

        [Fact]
        public void ReturnsLazyValue()
        {
            var calculator = Create<ICalculator>();
            var value = 1;

            Setup(() => calculator.Object.Add(2, 3)).Returns(() => value);

            Assert.Equal(1, calculator.Object.Add(2, 3));
            value = 2;
            Assert.Equal(2, calculator.Object.Add(2, 3));
        }

        [Fact]
        public void ReturnsLastValue()
        {
            var calculator = Create<ICalculator>();

            Setup(() => calculator.Object.Add(2, 3)).Returns(5).Returns(6);

            Assert.Equal(6, calculator.Object.Add(2, 3));
        }

        [Fact]
        public void ReturnsNull()
        {
            var calculator = Create<ICalculator>();

            Setup(() => calculator.Object.Recall("x")).Returns(5).Returns(null);

            Assert.Null(calculator.Object.Recall("x"));
        }

        [Fact]
        public void ReturnsFromUntypedHandler()
        {
            var calculator = Create<ICalculator>();

            Setup(() => calculator.Object.Add(Any<int>(), Any<int>()))
                .Returns(args => args.Get<int>(0) * args.Get<int>(1));

            Assert.Equal(6, calculator.Object.Add(2, 3));
        }

        [Fact]
        public void ReturnsFromTypedHandler()
        {
            var calculator = Create<ICalculator>();

            Typed<Func<int, int, int>, int>(() => calculator.Object.Add(Any<int>(), Any<int>()))
                .Returns((x, y) => x * y);

            Assert.Equal(6, calculator.Object.Add(2, 3));
        }

        [Fact]
        public void ReturnsSetsOutputsFromTypedRefOutHandler()
        {
            var calculator = Create<ICalculator>();
            int x = 0, y = 0;

            Typed<TryAddHandler, bool>(() => calculator.Object.TryAdd(ref x, ref y, out _))
                .Returns((ref x, ref y, out z) =>
                {
                    z = x + y;
                    x = 0;
                    return true;
                });

            x = 2;
            y = 3;
            Assert.False(calculator.Object.TryAdd(ref x, ref y, out var z));

            x = 0;
            y = 0;
            Assert.True(calculator.Object.TryAdd(ref x, ref y, out z));
            Assert.Equal(0, z);
        }

        [Fact]
        public void SetupRefMatchesAnyArguments()
        {
            var calculator = Create<ICalculator>();

            SetupRef<TryAddHandler>(calculator.Object.TryAdd)
                .Returns((ref x, ref y, out z) =>
                {
                    z = x + y;
                    x = 10;
                    return true;
                });

            int a = 2, b = 3;
            Assert.True(calculator.Object.TryAdd(ref a, ref b, out var result));
            Assert.Equal(5, result);
            Assert.Equal(10, a);
        }

        [Fact]
        public void SetupRefSupportsRecursiveMocks()
        {
            var mock = Create<IRecursiveRoot>();

            SetupRef<TryAddHandler>(() => mock.Object.Calculator.TryAdd)
                .Returns((ref x, ref y, out z) =>
                {
                    z = 42;
                    return true;
                });

            int a = 0, b = 0;
            Assert.True(mock.Object.Calculator.TryAdd(ref a, ref b, out var result));
            Assert.Equal(42, result);
        }

        [Fact]
        public void SetupRefCallbackSetsOutputs()
        {
            var calculator = Create<ICalculator>();

            SetupRef<TryAddHandler>(calculator.Object.TryAdd)
                .Callback((ref x, ref y, out z) =>
                {
                    z = x * y;
                    return true;
                });

            int a = 2, b = 3;
            calculator.Object.TryAdd(ref a, ref b, out var result);
            Assert.Equal(6, result);
        }

        [Fact]
        public void ReturnsAsyncValue()
        {
            var service = Create<IAsyncService>();

            Setup(() => service.Object.GetAsync(1)).Returns(5);
            Setup(() => service.Object.GetLengthAsync(1, "a")).Returns(3);

            Assert.Equal(5, service.Object.GetAsync(1).Result);
            Assert.Equal(3, service.Object.GetLengthAsync(1, "a").Result);
        }

        [Fact]
        public void ReturnsAsyncLazyValue()
        {
            var service = Create<IAsyncService>();
            var value = 1;

            Setup(() => service.Object.GetAsync(1)).Returns(() => value);
            value = 2;

            Assert.Equal(2, service.Object.GetAsync(1).Result);
        }

        [Fact]
        public void ReturnsAsyncTaskValue()
        {
            var service = Create<IAsyncService>();

            Setup(() => service.Object.GetAsync(1)).Returns(Task.FromResult(5));

            Assert.Equal(5, service.Object.GetAsync(1).Result);
        }

        [Fact]
        public void ReturnsAsyncFromUntypedHandler()
        {
            var service = Create<IAsyncService>();

            Setup(() => service.Object.GetAsync(Any<int>())).Returns(args => args.Get<int>(0) * 2);

            Assert.Equal(10, service.Object.GetAsync(5).Result);
        }

        [Fact]
        public void ReturnsAsyncFromTypedHandler()
        {
            var service = Create<IAsyncService>();

            Typed<Func<int, Task<int>>, Task<int>>(() => service.Object.GetAsync(Any<int>()))
                .Returns(id => id * 2);
            Typed<Func<int, string, ValueTask<int>>, ValueTask<int>>(() => service.Object.GetLengthAsync(Any<int>(), Any<string>()))
                .Returns((id, prefix) => prefix.Length + id);

            Assert.Equal(10, service.Object.GetAsync(5).Result);
            Assert.Equal(6, service.Object.GetLengthAsync(5, "a").Result);
        }

        [Fact]
        public void ReturnsAsyncFromTypedAsyncHandler()
        {
            var service = Create<IAsyncService>();

            Typed<Func<int, Task<int>>, Task<int>>(() => service.Object.GetAsync(Any<int>()))
                .Returns(async id =>
                {
                    await Task.Yield();
                    return id * 2;
                });

            Assert.Equal(10, service.Object.GetAsync(5).Result);
        }

        [Fact]
        public void CallbackOnVoidMember()
        {
            var calculator = Create<ICalculator>();
            var called = 0;

            Setup(() => calculator.Object.TurnOn()).Callback(() => called++);

            calculator.Object.TurnOn();

            Assert.Equal(1, called);
        }

        [Fact]
        public void CallbackReceivesUntypedArguments()
        {
            var calculator = Create<ICalculator>();
            string? name = null;
            int? value = null;

            Setup(() => calculator.Object.Store(Any<string>(), Any<int>())).Callback(args =>
            {
                name = args.Get<string>(0);
                value = args.Get<int>(1);
            });
            Setup(() => calculator.Object.Add(Any<int>(), Any<int>()))
                .Callback(args => value = args.Get<int>(0))
                .Returns(1);

            calculator.Object.Store("x", 5);
            Assert.Equal("x", name);
            Assert.Equal(5, value);

            Assert.Equal(1, calculator.Object.Add(3, 4));
            Assert.Equal(3, value);
        }

        [Fact]
        public void CallbackReceivesTypedArguments()
        {
            var calculator = Create<ICalculator>();
            string? name = null;
            int? value = null;

            Typed<Action<string, int>>(() => calculator.Object.Store(Any<string>(), Any<int>()))
                .Callback((n, v) => (name, value) = (n, v));
            Typed<Func<int, int, int>, int>(() => calculator.Object.Add(Any<int>(), Any<int>()))
                .Callback((x, y) => value = x + y)
                .Returns(1);

            calculator.Object.Store("x", 5);
            Assert.Equal("x", name);
            Assert.Equal(5, value);

            Assert.Equal(1, calculator.Object.Add(3, 4));
            Assert.Equal(7, value);
        }

        [Fact]
        public void CallbacksRunInOrderBeforeReturns()
        {
            var calculator = Create<ICalculator>();
            var calls = "";

            Setup(() => calculator.Object.Add(2, 3))
                .Returns(() => { calls += "r"; return 5; })
                .Callback(() => calls += "1")
                .Callback(() => calls += "2");

            Assert.Equal(5, calculator.Object.Add(2, 3));
            Assert.Equal("12r", calls);
        }

        [Fact]
        public void ThrowsException()
        {
            var calculator = Create<ICalculator>();

            Setup(() => calculator.Object.TurnOn()).Throws(new InvalidOperationException());
            Setup(() => calculator.Object.Add(2, 3)).Throws<ArgumentException>();

            Assert.Throws<InvalidOperationException>(() => calculator.Object.TurnOn());
            Assert.Throws<ArgumentException>(() => calculator.Object.Add(2, 3));
        }

        [Fact]
        public void ThrowsReplacesReturns()
        {
            var calculator = Create<ICalculator>();

            var setup = Setup(() => calculator.Object.Add(2, 3)).Returns(5);
            setup.Throws(new InvalidOperationException());

            Assert.Throws<InvalidOperationException>(() => calculator.Object.Add(2, 3));

            setup.Returns(5);
            Assert.Equal(5, calculator.Object.Add(2, 3));
        }

        [Fact]
        public async Task ThrowsFaultsAsyncMembers()
        {
            var service = Create<IAsyncService>();

            Setup(() => service.Object.GetAsync(1)).Throws(new InvalidOperationException());
            Setup(() => service.Object.GetLengthAsync(1, "a")).Throws(new InvalidOperationException());
            Setup(() => service.Object.RunAsync()).Throws(new InvalidOperationException());

            var task = service.Object.GetAsync(1);
            Assert.True(task.IsFaulted);
            await Assert.ThrowsAsync<InvalidOperationException>(() => task);
            await Assert.ThrowsAsync<InvalidOperationException>(() => service.Object.GetLengthAsync(1, "a").AsTask());
            await Assert.ThrowsAsync<InvalidOperationException>(() => service.Object.RunAsync());
        }

        [Fact]
        public void SetupRecursiveMember()
        {
            var calculator = Create<ICalculator>();

            Setup(() => calculator.Object.Memory.Recall()).Returns(42);

            Assert.Equal(42, calculator.Object.Memory.Recall());
        }

        [Fact]
        public void CallBaseOnSetup()
        {
            var calculator = Create<Calculator>();

            Setup(() => calculator.Object.Add(2, 3)).CallBase();

            Assert.Equal(5, calculator.Object.Add(2, 3));
            Assert.Equal(0, calculator.Object.Add(1, 1));
        }

        [Fact]
        public void SetupPropertyHandle()
        {
            var calculator = Create<ICalculator>();
            var mode = SetupFactory.Property<ICalculator, Func<CalculatorMode>, Action<CalculatorMode>, CalculatorMode>(
                calculator, nameof(ICalculator.Mode), c => _ = c.Mode, (c, v) => c.Mode = v);

            mode.Returns(CalculatorMode.Scientific);

            Assert.Equal(CalculatorMode.Scientific, calculator.Object.Mode);
        }

        [Fact]
        public void SetupPropertyHandleIsLazy()
        {
            var calculator = Create<ICalculator>();
            var mode = SetupFactory.Property<ICalculator, Func<CalculatorMode>, Action<CalculatorMode>, CalculatorMode>(
                calculator, nameof(ICalculator.Mode), c => _ = c.Mode, (c, v) => c.Mode = v);

            Assert.Empty(calculator.Sdk.Setups);

            var called = false;
            mode.Set(CalculatorMode.Scientific).Callback(() => called = true);

            Assert.Equal("set_Mode", Assert.Single(calculator.Sdk.Setups).Setup.Invocation.MethodBase.Name);

            calculator.Object.Mode = CalculatorMode.Standard;
            Assert.False(called);

            calculator.Object.Mode = CalculatorMode.Scientific;
            Assert.True(called);
        }

        [Fact]
        public void SetupIndexerHandle()
        {
            var calculator = Create<ICalculator>();
            var key = Any<string>(x => x.StartsWith("a"));
            var item = SetupFactory.Indexer<ICalculator, Func<string, int?>, Action<string, int?>, int?>(
                calculator, "Item", c => _ = c[key], (c, v) => c[key] = v);

            item.Returns(10);
            string? stored = null;
            item.Set(Any<int?>()).Callback(() => stored = "set");

            Assert.Equal(10, calculator.Object["a1"]);
            Assert.Null(calculator.Object["b"]);

            calculator.Object["b"] = 5;
            Assert.Null(stored);
            calculator.Object["a2"] = 5;
            Assert.Equal("set", stored);
        }

        [Fact]
        public void VerifyCalledLambda()
        {
            var calculator = Create<ICalculator>();

            calculator.Object.Add(2, 3);
            calculator.Object.Add(2, 3);

            Verify.Called(() => calculator.Object.Add(2, 3));
            Verify.Called(() => calculator.Object.Add(2, 3), 2);
            Verify.NotCalled(() => calculator.Object.Add(1, 1));
            Assert.Throws<VerifyException>(() => Verify.Called(() => calculator.Object.Add(1, 1)));
            Assert.Throws<VerifyException>(() => Verify.Called(() => calculator.Object.Add(2, 3), 1));
        }

        [Fact]
        public void VerifyCalledOnVerifier()
        {
            var calculator = Create<ICalculator>();

            calculator.Object.TurnOn();

            Verify.Called(calculator).Object.TurnOn();
            Assert.Throws<VerifyException>(() => Verify.Called(calculator).Object.Add(1, 1));
        }

        [Fact]
        public void VerifyNotCalledOnVerifier()
        {
            var calculator = Create<ICalculator>();

            calculator.Object.TurnOn();

            Verify.NotCalled(calculator).Object.Add(1, 1);
            Assert.Throws<VerifyException>(() => Verify.NotCalled(calculator).Object.TurnOn());
        }

        [Fact]
        public void VerifyExactlyOnVerifier()
        {
            var calculator = Create<ICalculator>();

            calculator.Object.Add(2, 3);
            calculator.Object.Add(2, 3);

            var verifier = Verify.Called(calculator);
            Setup(() => verifier.Object.Add(2, 3)).Exactly(2);
            Assert.Throws<VerifyException>(() => Setup(() => verifier.Object.Add(2, 3)).Once());
        }

        [Fact]
        public void VerifierDoesNotRecordInvocations()
        {
            var calculator = Create<ICalculator>();

            calculator.Object.TurnOn();
            Verify.Called(calculator).Object.TurnOn();

            Assert.Single(calculator.Sdk.Invocations);
        }

        [Fact]
        public void VerifyChecksOccurrenceConstraints()
        {
            var calculator = Create<ICalculator>();

            Setup(() => calculator.Object.TurnOn()).Once();
            Setup(() => calculator.Object.Add(1, 1)).Never();

            Assert.Throws<VerifyException>(() => Syntax.Verify(calculator));

            calculator.Object.TurnOn();
            Syntax.Verify(calculator);

            calculator.Object.Add(1, 1);
            Assert.Throws<VerifyException>(() => Syntax.Verify(calculator));
        }

        [Fact]
        public void VerifyCallsProvidesMatchingInvocations()
        {
            var calculator = Create<ICalculator>();

            calculator.Object.Store("a", 1);
            calculator.Object.Store("b", 2);
            calculator.Object.Store("a", 3);

            Verify.Calls(() => calculator.Object.Store("a", Any<int>()),
                calls => Assert.Equal(new[] { 1, 3 }, calls.Select(x => x.Arguments.Get<int>(1))));
        }

        [Fact]
        public void SetupScopeIsReentrant()
        {
            var calculator = Create<ICalculator>();

            using (Setup())
            {
                Setup(() => calculator.Object.Add(2, 3)).Returns(5);
                calculator.Object.TurnOn();
            }

            Assert.Empty(calculator.Sdk.Invocations);
            Assert.Equal(5, calculator.Object.Add(2, 3));
            Assert.Single(calculator.Sdk.Invocations);
        }

        public interface IRecursiveRoot
        {
            ICalculator Calculator { get; }
        }
    }
}
