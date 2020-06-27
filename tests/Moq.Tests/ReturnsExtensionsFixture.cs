// Copyright (c) 2007, Clarius Consulting, Manas Technology Solutions, InSTEDD, and Contributors.
// All rights reserved. Licensed under the BSD 3-Clause License; see License.txt.

using System;
using System.Threading.Tasks;

using Xunit;

namespace Moq.Tests
{
	public class ReturnsExtensionsFixture
	{
		public interface IAsyncInterface
		{
			Task NoParametersNonGenericTaskReturnType();

			Task<string> NoParametersRefReturnType();

			Task<int> NoParametersValueReturnType();

			Task<string> RefParameterRefReturnType(string value);

			Task<int> RefParameterValueReturnType(string value);

			Task<string> ValueParameterRefReturnType(int value);

			Task<int> ValueParameterValueReturnType(int value);

			Task<Guid> NewGuidAsync();

			Task<object> NoParametersObjectReturnType();

			Task<object> OneParameterObjectReturnType(string value);

			Task<object> ManyParametersObjectReturnType(string arg1, bool arg2, float arg3);
		}

		public interface IValueTaskAsyncInterface
		{
			ValueTask<string> NoParametersRefReturnType();

			ValueTask<int> NoParametersValueReturnType();

			ValueTask<string> RefParameterRefReturnType(string value);

			ValueTask<int> RefParameterValueReturnType(string value);

			ValueTask<string> ValueParameterRefReturnType(int value);

			ValueTask<int> ValueParameterValueReturnType(int value);

			ValueTask<Guid> NewGuidAsync();

			ValueTask<object> NoParametersObjectReturnType();

			ValueTask<object> OneParameterObjectReturnType(string value);

			ValueTask<object> ManyParametersObjectReturnType(string arg1, bool arg2, float arg3);
		}

		[Fact]
		public void ReturnsAsync_on_NoParametersRefReturnType()
		{
			var mock = new Mock<IAsyncInterface>();
			mock.Setup(x => x.NoParametersRefReturnType()).ReturnsAsync("TestString");

			var task = mock.Object.NoParametersRefReturnType();

			Assert.True(task.IsCompleted);
			Assert.Equal("TestString", task.Result);
		}

		[Fact]
		public void ReturnsAsync_on_NoParametersValueReturnType()
		{
			var mock = new Mock<IAsyncInterface>();
			mock.Setup(x => x.NoParametersValueReturnType()).ReturnsAsync(36);

			var task = mock.Object.NoParametersValueReturnType();

			Assert.True(task.IsCompleted);
			Assert.Equal(36, task.Result);
		}

		[Fact]
		public void ReturnsAsync_on_RefParameterRefReturnType()
		{
			var mock = new Mock<IAsyncInterface>();
			mock.Setup(x => x.RefParameterRefReturnType("Param1")).ReturnsAsync("TestString");

			var task = mock.Object.RefParameterRefReturnType("Param1");

			Assert.True(task.IsCompleted);
			Assert.Equal("TestString", task.Result);
		}

		[Fact]
		public void ReturnsAsync_on_RefParameterValueReturnType()
		{
			var mock = new Mock<IAsyncInterface>();
			mock.Setup(x => x.RefParameterValueReturnType("Param1")).ReturnsAsync(36);

			var task = mock.Object.RefParameterValueReturnType("Param1");

			Assert.True(task.IsCompleted);
			Assert.Equal(36, task.Result);
		}

		[Fact]
		public void ReturnsAsync_on_ValueParameterRefReturnType()
		{
			var mock = new Mock<IAsyncInterface>();
			mock.Setup(x => x.ValueParameterRefReturnType(36)).ReturnsAsync("TestString");

			var task = mock.Object.ValueParameterRefReturnType(36);

			Assert.True(task.IsCompleted);
			Assert.Equal("TestString", task.Result);
		}

		[Fact]
		public void ReturnsAsync_on_ValueParameterValueReturnType()
		{
			var mock = new Mock<IAsyncInterface>();
			mock.Setup(x => x.ValueParameterValueReturnType(36)).ReturnsAsync(37);

			var task = mock.Object.ValueParameterValueReturnType(36);

			Assert.True(task.IsCompleted);
			Assert.Equal(37, task.Result);
		}

		[Fact]
		public void ReturnsAsyncFunc_on_NoParametersRefReturnType()
		{
			var mock = new Mock<IAsyncInterface>();
			mock.Setup(x => x.NoParametersRefReturnType()).ReturnsAsync(() => "TestString");

			var task = mock.Object.NoParametersRefReturnType();

			Assert.True(task.IsCompleted);
			Assert.Equal("TestString", task.Result);
		}

		[Fact]
		public void ReturnsAsyncFunc_on_NoParametersValueReturnType()
		{
			var mock = new Mock<IAsyncInterface>();
			mock.Setup(x => x.NoParametersValueReturnType()).ReturnsAsync(() => 36);

			var task = mock.Object.NoParametersValueReturnType();

			Assert.True(task.IsCompleted);
			Assert.Equal(36, task.Result);
		}

		[Fact]
		public void ReturnsAsyncFunc_on_RefParameterRefReturnType()
		{
			var mock = new Mock<IAsyncInterface>();
			mock.Setup(x => x.RefParameterRefReturnType("Param1")).ReturnsAsync(() => "TestString");

			var task = mock.Object.RefParameterRefReturnType("Param1");

			Assert.True(task.IsCompleted);
			Assert.Equal("TestString", task.Result);
		}

		[Fact]
		public void ReturnsAsyncFunc_on_RefParameterValueReturnType()
		{
			var mock = new Mock<IAsyncInterface>();
			mock.Setup(x => x.RefParameterValueReturnType("Param1")).ReturnsAsync(() => 36);

			var task = mock.Object.RefParameterValueReturnType("Param1");

			Assert.True(task.IsCompleted);
			Assert.Equal(36, task.Result);
		}

		[Fact]
		public void ReturnsAsyncFunc_on_ValueParameterRefReturnType()
		{
			var mock = new Mock<IAsyncInterface>();
			mock.Setup(x => x.ValueParameterRefReturnType(36)).ReturnsAsync(() => "TestString");

			var task = mock.Object.ValueParameterRefReturnType(36);

			Assert.True(task.IsCompleted);
			Assert.Equal("TestString", task.Result);
		}

		[Fact]
		public void ReturnsAsyncFunc_on_ValueParameterValueReturnType()
		{
			var mock = new Mock<IAsyncInterface>();
			mock.Setup(x => x.ValueParameterValueReturnType(36)).ReturnsAsync(() => 37);

			var task = mock.Object.ValueParameterValueReturnType(36);

			Assert.True(task.IsCompleted);
			Assert.Equal(37, task.Result);
		}

		[Fact]
		public void ReturnsAsyncFunc_onEachInvocation_ValueReturnTypeLazyEvaluation()
		{
			var mock = new Mock<IAsyncInterface>();
			mock.Setup(x => x.NewGuidAsync()).ReturnsAsync(Guid.NewGuid);

			Guid firstEvaluationResult = mock.Object.NewGuidAsync().Result;
			Guid secondEvaluationResult = mock.Object.NewGuidAsync().Result;

			Assert.NotEqual(firstEvaluationResult, secondEvaluationResult);
		}

		[Fact]
		public void ReturnsAsyncFunc_onEachInvocation_RefReturnTypeLazyEvaluation()
		{
			var mock = new Mock<IAsyncInterface>();
			mock.Setup(x => x.ValueParameterRefReturnType(36)).ReturnsAsync(() => new string(new[] { 'M', 'o', 'q', '4' }));

			string firstEvaluationResult = mock.Object.ValueParameterRefReturnType(36).Result;
			string secondEvaluationResult = mock.Object.ValueParameterRefReturnType(36).Result;

			Assert.NotSame(firstEvaluationResult, secondEvaluationResult);
		}

		[Fact]
		public void ThrowsAsync_on_NoParametersNonGenericTaskReturnType()
		{
			var mock = new Mock<IAsyncInterface>();
			var exception = new InvalidOperationException();
			mock.Setup(x => x.NoParametersNonGenericTaskReturnType()).ThrowsAsync(exception);

			var task = mock.Object.NoParametersNonGenericTaskReturnType();

			Assert.True(task.IsFaulted);
			Assert.Equal(exception, task.Exception.InnerException);
		}

		[Fact]
		public void ThrowsAsync_on_NoParametersRefReturnType()
		{
			var mock = new Mock<IAsyncInterface>();
			var exception = new InvalidOperationException();
			mock.Setup(x => x.NoParametersRefReturnType()).ThrowsAsync(exception);

			var task = mock.Object.NoParametersRefReturnType();

			Assert.True(task.IsFaulted);
			Assert.Equal(exception, task.Exception.InnerException);
		}

		[Fact]
		public void ThrowsAsync_on_NoParametersValueReturnType()
		{
			var mock = new Mock<IAsyncInterface>();
			var exception = new InvalidOperationException();
			mock.Setup(x => x.NoParametersValueReturnType()).ThrowsAsync(exception);

			var task = mock.Object.NoParametersValueReturnType();

			Assert.True(task.IsFaulted);
			Assert.Equal(exception, task.Exception.InnerException);
		}

		[Fact]
		public void ThrowsAsync_on_RefParameterRefReturnType()
		{
			var mock = new Mock<IAsyncInterface>();
			var exception = new InvalidOperationException();
			mock.Setup(x => x.RefParameterRefReturnType("Param1")).ThrowsAsync(exception);

			var task = mock.Object.RefParameterRefReturnType("Param1");

			Assert.True(task.IsFaulted);
			Assert.Equal(exception, task.Exception.InnerException);
		}

		[Fact]
		public void ThrowsAsync_on_RefParameterValueReturnType()
		{
			var mock = new Mock<IAsyncInterface>();
			var exception = new InvalidOperationException();
			mock.Setup(x => x.RefParameterValueReturnType("Param1")).ThrowsAsync(exception);

			var task = mock.Object.RefParameterValueReturnType("Param1");

			Assert.True(task.IsFaulted);
			Assert.Equal(exception, task.Exception.InnerException);
		}

		[Fact]
		public void ThrowsAsync_on_ValueParameterRefReturnType()
		{
			var mock = new Mock<IAsyncInterface>();
			var exception = new InvalidOperationException();
			mock.Setup(x => x.ValueParameterRefReturnType(36)).ThrowsAsync(exception);

			var task = mock.Object.ValueParameterRefReturnType(36);

			Assert.True(task.IsFaulted);
			Assert.Equal(exception, task.Exception.InnerException);
		}

		[Fact]
		public void ThrowsAsync_on_ValueParameterValueReturnType()
		{
			var mock = new Mock<IAsyncInterface>();
			var exception = new InvalidOperationException();
			mock.Setup(x => x.ValueParameterValueReturnType(36)).ThrowsAsync(exception);

			var task = mock.Object.ValueParameterValueReturnType(36);

			Assert.True(task.IsFaulted);
			Assert.Equal(exception, task.Exception.InnerException);
		}

		// The test below is dependent on the timings (too much of a 'works-on-my-machine' smell)
		//[Theory]
		//[InlineData(true)]
		//[InlineData(false)]
		//public async Task ReturnsAsyncWithDelayTriggersRealAsyncBehaviour(bool useDelay)
		//{
		//    var mock = new Mock<IAsyncInterface>();

		//    var setup = mock.Setup(x => x.RefParameterValueReturnType("test"));

		//    if (useDelay)
		//        setup.ReturnsAsync(5, TimeSpan.FromMilliseconds(1));
		//    else
		//        setup.ReturnsAsync(5);

		//    var thread1 = Thread.CurrentThread;
		//    await mock.Object.RefParameterValueReturnType("test");
		//    var thread2 = Thread.CurrentThread;

		//    if (useDelay)
		//        Assert.NotEqual(thread1, thread2);
		//    else
		//        Assert.Equal(thread1, thread2);
		//}

		[Fact]
		public void ReturnsAsyncWithDelayDoesNotImmediatelyComplete()
		{
			var longEnoughForAnyBuildServer = TimeSpan.FromSeconds(5);

			var mock = new Mock<IAsyncInterface>();
			mock.Setup(x => x.RefParameterValueReturnType("test")).ReturnsAsync(5, longEnoughForAnyBuildServer);

			var task = mock.Object.RefParameterValueReturnType("test");

			Assert.False(task.IsCompleted);
		}

		[Theory]
		[InlineData(-1, true)]
		[InlineData(0, true)]
		[InlineData(1, false)]
		public void DelayMustBePositive(int ticks, bool mustThrow)
		{
			var mock = new Mock<IAsyncInterface>();

			Action setup = () => mock
				.Setup(x => x.RefParameterValueReturnType("test"))
				.ReturnsAsync(5, TimeSpan.FromTicks(ticks));

			if (mustThrow)
				Assert.Throws<ArgumentException>(setup);
			else
				setup();
		}

		[Fact]
		public async Task ReturnsAsyncWithDelayReturnsValue()
		{
			var mock = new Mock<IAsyncInterface>();
			mock.Setup(x => x.RefParameterValueReturnType("test")).ReturnsAsync(5, TimeSpan.FromMilliseconds(1));

			var value = await mock.Object.RefParameterValueReturnType("test");

			Assert.Equal(5, value);
		}

		[Fact]
		public async Task ReturnsAsyncWithMinAndMaxDelayReturnsValue()
		{
			var mock = new Mock<IAsyncInterface>();
			mock.Setup(x => x.RefParameterValueReturnType("test")).ReturnsAsync(5, TimeSpan.FromMilliseconds(1), TimeSpan.FromMilliseconds(2));

			var value = await mock.Object.RefParameterValueReturnType("test");

			Assert.Equal(5, value);
		}

		[Fact]
		public async Task ReturnsAsyncWithMinAndMaxDelayAndOwnRandomGeneratorReturnsValue()
		{
			var mock = new Mock<IAsyncInterface>();
			mock.Setup(x => x.RefParameterValueReturnType("test")).ReturnsAsync(5, TimeSpan.FromMilliseconds(1), TimeSpan.FromMilliseconds(2), new Random());

			var value = await mock.Object.RefParameterValueReturnType("test");

			Assert.Equal(5, value);
		}

		[Fact]
		public void ReturnsAsyncWithNullRandomGenerator()
		{
			var mock = new Mock<IAsyncInterface>();

			Action setup = () => mock
				.Setup(x => x.RefParameterValueReturnType("test"))
				.ReturnsAsync(5, TimeSpan.FromMilliseconds(1), TimeSpan.FromMilliseconds(2), null);

			var paramName = Assert.Throws<ArgumentNullException>(setup).ParamName;
			Assert.Equal("random", paramName);
		}

		[Fact]
		public async Task ThrowsWithDelay()
		{
			var mock = new Mock<IAsyncInterface>();

			mock
				.Setup(x => x.RefParameterValueReturnType("test"))
				.ThrowsAsync(new ArithmeticException("yikes"), TimeSpan.FromMilliseconds(1));

			Func<Task<int>> test = () => mock.Object.RefParameterValueReturnType("test");

			var exception = await Assert.ThrowsAsync<ArithmeticException>(test);
			Assert.Equal("yikes", exception.Message);
		}

		[Fact]
		public async Task ThrowsWithRandomDelay()
		{
			var mock = new Mock<IAsyncInterface>();

			var minDelay = TimeSpan.FromMilliseconds(1);
			var maxDelay = TimeSpan.FromMilliseconds(2);

			mock
				.Setup(x => x.RefParameterValueReturnType("test"))
				.ThrowsAsync(new ArithmeticException("yikes"), minDelay, maxDelay);

			Func<Task<int>> test = () => mock.Object.RefParameterValueReturnType("test");

			var exception = await Assert.ThrowsAsync<ArithmeticException>(test);
			Assert.Equal("yikes", exception.Message);
		}

		[Fact]
		public async Task ThrowsWithRandomDelayAndOwnRandomGenerator()
		{
			var mock = new Mock<IAsyncInterface>();

			var minDelay = TimeSpan.FromMilliseconds(1);
			var maxDelay = TimeSpan.FromMilliseconds(2);

			mock
				.Setup(x => x.RefParameterValueReturnType("test"))
				.ThrowsAsync(new ArithmeticException("yikes"), minDelay, maxDelay, new Random());

			Func<Task<int>> test = () => mock.Object.RefParameterValueReturnType("test");

			var exception = await Assert.ThrowsAsync<ArithmeticException>(test);
			Assert.Equal("yikes", exception.Message);
		}

		[Fact]
		public void ThrowsAsyncWithNullRandomGenerator()
		{
			var mock = new Mock<IAsyncInterface>();

			Action setup = () =>
			{
				var anyException = new Exception();
				var minDelay = TimeSpan.FromMilliseconds(1);
				var maxDelay = TimeSpan.FromMilliseconds(2);

				mock
					.Setup(x => x.RefParameterValueReturnType("test"))
					.ThrowsAsync(anyException, minDelay, maxDelay, null);
			};

			var paramName = Assert.Throws<ArgumentNullException>(setup).ParamName;
			Assert.Equal("random", paramName);
		}

		[Fact]
		public void ValueTaskReturnsAsync_on_NoParametersRefReturnType()
		{
			var mock = new Mock<IValueTaskAsyncInterface>();
			mock.Setup(x => x.NoParametersRefReturnType()).ReturnsAsync("TestString");

			var task = mock.Object.NoParametersRefReturnType();

			Assert.IsType<ValueTask<string>>(task);
			Assert.True(task.IsCompleted);
			Assert.Equal("TestString", task.Result);
		}

		[Fact]
		public void ValueTaskReturnsAsync_on_NoParametersValueReturnType()
		{
			var mock = new Mock<IValueTaskAsyncInterface>();
			mock.Setup(x => x.NoParametersValueReturnType()).ReturnsAsync(36);

			var task = mock.Object.NoParametersValueReturnType();

			Assert.IsType<ValueTask<int>>(task);
			Assert.True(task.IsCompleted);
			Assert.Equal(36, task.Result);
		}

		[Fact]
		public void ValueTaskReturnsAsync_on_RefParameterRefReturnType()
		{
			var mock = new Mock<IValueTaskAsyncInterface>();
			mock.Setup(x => x.RefParameterRefReturnType("Param1")).ReturnsAsync("TestString");

			var task = mock.Object.RefParameterRefReturnType("Param1");

			Assert.IsType<ValueTask<string>>(task);
			Assert.True(task.IsCompleted);
			Assert.Equal("TestString", task.Result);
		}

		[Fact]
		public void ValueTaskReturnsAsync_on_RefParameterValueReturnType()
		{
			var mock = new Mock<IValueTaskAsyncInterface>();
			mock.Setup(x => x.RefParameterValueReturnType("Param1")).ReturnsAsync(36);

			var task = mock.Object.RefParameterValueReturnType("Param1");

			Assert.IsType<ValueTask<int>>(task);
			Assert.True(task.IsCompleted);
			Assert.Equal(36, task.Result);
		}

		[Fact]
		public void ValueTaskReturnsAsync_on_ValueParameterRefReturnType()
		{
			var mock = new Mock<IValueTaskAsyncInterface>();
			mock.Setup(x => x.ValueParameterRefReturnType(36)).ReturnsAsync("TestString");

			var task = mock.Object.ValueParameterRefReturnType(36);

			Assert.IsType<ValueTask<string>>(task);
			Assert.True(task.IsCompleted);
			Assert.Equal("TestString", task.Result);
		}

		[Fact]
		public void ValueTaskReturnsAsync_on_ValueParameterValueReturnType()
		{
			var mock = new Mock<IValueTaskAsyncInterface>();
			mock.Setup(x => x.ValueParameterValueReturnType(36)).ReturnsAsync(37);

			var task = mock.Object.ValueParameterValueReturnType(36);

			Assert.IsType<ValueTask<int>>(task);
			Assert.True(task.IsCompleted);
			Assert.Equal(37, task.Result);
		}

		[Fact]
		public void ValueTaskReturnsAsyncFunc_on_NoParametersRefReturnType()
		{
			var mock = new Mock<IValueTaskAsyncInterface>();
			mock.Setup(x => x.NoParametersRefReturnType()).ReturnsAsync(() => "TestString");

			var task = mock.Object.NoParametersRefReturnType();

			Assert.IsType<ValueTask<string>>(task);
			Assert.True(task.IsCompleted);
			Assert.Equal("TestString", task.Result);
		}

		[Fact]
		public void ValueTaskReturnsAsyncFunc_on_NoParametersValueReturnType()
		{
			var mock = new Mock<IValueTaskAsyncInterface>();
			mock.Setup(x => x.NoParametersValueReturnType()).ReturnsAsync(() => 36);

			var task = mock.Object.NoParametersValueReturnType();

			Assert.IsType<ValueTask<int>>(task);
			Assert.True(task.IsCompleted);
			Assert.Equal(36, task.Result);
		}

		[Fact]
		public void ValueTaskReturnsAsyncFunc_on_RefParameterRefReturnType()
		{
			var mock = new Mock<IValueTaskAsyncInterface>();
			mock.Setup(x => x.RefParameterRefReturnType("Param1")).ReturnsAsync(() => "TestString");

			var task = mock.Object.RefParameterRefReturnType("Param1");

			Assert.IsType<ValueTask<string>>(task);
			Assert.True(task.IsCompleted);
			Assert.Equal("TestString", task.Result);
		}

		[Fact]
		public void ValueTaskReturnsAsyncFunc_on_RefParameterValueReturnType()
		{
			var mock = new Mock<IValueTaskAsyncInterface>();
			mock.Setup(x => x.RefParameterValueReturnType("Param1")).ReturnsAsync(() => 36);

			var task = mock.Object.RefParameterValueReturnType("Param1");

			Assert.IsType<ValueTask<int>>(task);
			Assert.True(task.IsCompleted);
			Assert.Equal(36, task.Result);
		}

		[Fact]
		public void ValueTaskReturnsAsyncFunc_on_ValueParameterRefReturnType()
		{
			var mock = new Mock<IValueTaskAsyncInterface>();
			mock.Setup(x => x.ValueParameterRefReturnType(36)).ReturnsAsync(() => "TestString");

			var task = mock.Object.ValueParameterRefReturnType(36);

			Assert.IsType<ValueTask<string>>(task);
			Assert.True(task.IsCompleted);
			Assert.Equal("TestString", task.Result);
		}

		[Fact]
		public void ValueTaskReturnsAsyncFunc_on_ValueParameterValueReturnType()
		{
			var mock = new Mock<IValueTaskAsyncInterface>();
			mock.Setup(x => x.ValueParameterValueReturnType(36)).ReturnsAsync(() => 37);

			var task = mock.Object.ValueParameterValueReturnType(36);

			Assert.IsType<ValueTask<int>>(task);
			Assert.True(task.IsCompleted);
			Assert.Equal(37, task.Result);
		}

		[Fact]
		public void ValueTaskReturnsAsyncFunc_onEachInvocation_ValueReturnTypeLazyEvaluation()
		{
			var mock = new Mock<IValueTaskAsyncInterface>();
			mock.Setup(x => x.NewGuidAsync()).ReturnsAsync(Guid.NewGuid);

			var firstTask = mock.Object.NewGuidAsync();
			var secondTask = mock.Object.NewGuidAsync();

			Assert.IsType<ValueTask<Guid>>(firstTask);
			Assert.IsType<ValueTask<Guid>>(secondTask);
			Assert.NotEqual(firstTask.Result, secondTask.Result);
		}

		[Fact]
		public void ValueTaskReturnsAsyncFunc_onEachInvocation_RefReturnTypeLazyEvaluation()
		{
			var mock = new Mock<IValueTaskAsyncInterface>();
			mock.Setup(x => x.ValueParameterRefReturnType(36)).ReturnsAsync(() => new string(new[] { 'M', 'o', 'q', '4' }));

			var firstTask = mock.Object.ValueParameterRefReturnType(36);
			var secondTask = mock.Object.ValueParameterRefReturnType(36);
			
			Assert.IsType<ValueTask<string>>(firstTask);
			Assert.IsType<ValueTask<string>>(secondTask);
			Assert.NotSame(firstTask.Result, secondTask.Result);
		}

		[Fact]
		public void ValueTaskThrowsAsync_on_NoParametersRefReturnType()
		{
			var mock = new Mock<IValueTaskAsyncInterface>();
			var exception = new InvalidOperationException();
			mock.Setup(x => x.NoParametersRefReturnType()).ThrowsAsync(exception);

			var task = mock.Object.NoParametersRefReturnType();

			Assert.IsType<ValueTask<string>>(task);
			Assert.True(task.IsFaulted);
			Assert.Equal(exception, task.AsTask().Exception.InnerException);
		}

		[Fact]
		public void ValueTaskThrowsAsync_on_NoParametersValueReturnType()
		{
			var mock = new Mock<IValueTaskAsyncInterface>();
			var exception = new InvalidOperationException();
			mock.Setup(x => x.NoParametersValueReturnType()).ThrowsAsync(exception);

			var task = mock.Object.NoParametersValueReturnType();

			Assert.IsType<ValueTask<int>>(task);
			Assert.True(task.IsFaulted);
			Assert.Equal(exception, task.AsTask().Exception.InnerException);
		}

		[Fact]
		public void ValueTaskThrowsAsync_on_RefParameterRefReturnType()
		{
			var mock = new Mock<IValueTaskAsyncInterface>();
			var exception = new InvalidOperationException();
			mock.Setup(x => x.RefParameterRefReturnType("Param1")).ThrowsAsync(exception);

			var task = mock.Object.RefParameterRefReturnType("Param1");

			Assert.IsType<ValueTask<string>>(task);
			Assert.True(task.IsFaulted);
			Assert.Equal(exception, task.AsTask().Exception.InnerException);
		}

		[Fact]
		public void ValueTaskThrowsAsync_on_RefParameterValueReturnType()
		{
			var mock = new Mock<IValueTaskAsyncInterface>();
			var exception = new InvalidOperationException();
			mock.Setup(x => x.RefParameterValueReturnType("Param1")).ThrowsAsync(exception);

			var task = mock.Object.RefParameterValueReturnType("Param1");

			Assert.IsType<ValueTask<int>>(task);
			Assert.True(task.IsFaulted);
			Assert.Equal(exception, task.AsTask().Exception.InnerException);
		}

		[Fact]
		public void ValueTaskThrowsAsync_on_ValueParameterRefReturnType()
		{
			var mock = new Mock<IValueTaskAsyncInterface>();
			var exception = new InvalidOperationException();
			mock.Setup(x => x.ValueParameterRefReturnType(36)).ThrowsAsync(exception);

			var task = mock.Object.ValueParameterRefReturnType(36);

			Assert.IsType<ValueTask<string>>(task);
			Assert.True(task.IsFaulted);
			Assert.Equal(exception, task.AsTask().Exception.InnerException);
		}

		[Fact]
		public void ValueTaskThrowsAsync_on_ValueParameterValueReturnType()
		{
			var mock = new Mock<IValueTaskAsyncInterface>();
			var exception = new InvalidOperationException();
			mock.Setup(x => x.ValueParameterValueReturnType(36)).ThrowsAsync(exception);

			var task = mock.Object.ValueParameterValueReturnType(36);

			Assert.IsType<ValueTask<int>>(task);
			Assert.True(task.IsFaulted);
			Assert.Equal(exception, task.AsTask().Exception.InnerException);
		}
		
		[Fact]
		public void ValueTaskReturnsAsyncWithDelayDoesNotImmediatelyComplete()
		{
			var longEnoughForAnyBuildServer = TimeSpan.FromSeconds(5);

			var mock = new Mock<IValueTaskAsyncInterface>();
			mock.Setup(x => x.RefParameterValueReturnType("test")).ReturnsAsync(5, longEnoughForAnyBuildServer);

			var task = mock.Object.RefParameterValueReturnType("test");

			Assert.IsType<ValueTask<int>>(task);
			Assert.False(task.IsCompleted);
		}

		[Theory]
		[InlineData(-1, true)]
		[InlineData(0, true)]
		[InlineData(1, false)]
		public void ValueTaskDelayMustBePositive(int ticks, bool mustThrow)
		{
			var mock = new Mock<IValueTaskAsyncInterface>();

			Action setup = () => mock
				.Setup(x => x.RefParameterValueReturnType("test"))
				.ReturnsAsync(5, TimeSpan.FromTicks(ticks));

			if (mustThrow)
				Assert.Throws<ArgumentException>(setup);
			else
				setup();
		}

		[Fact]
		public async Task ValueTaskReturnsAsyncWithDelayReturnsValue()
		{
			var mock = new Mock<IValueTaskAsyncInterface>();
			mock.Setup(x => x.RefParameterValueReturnType("test")).ReturnsAsync(5, TimeSpan.FromMilliseconds(1));

			var task = mock.Object.RefParameterValueReturnType("test");
			var value = await Assert.IsType<ValueTask<int>>(task);

			Assert.Equal(5, value);
		}

		[Fact]
		public async Task ValueTaskReturnsAsyncWithMinAndMaxDelayReturnsValue()
		{
			var mock = new Mock<IValueTaskAsyncInterface>();
			mock.Setup(x => x.RefParameterValueReturnType("test")).ReturnsAsync(5, TimeSpan.FromMilliseconds(1), TimeSpan.FromMilliseconds(2));

			var task = mock.Object.RefParameterValueReturnType("test");
			var value = await Assert.IsType<ValueTask<int>>(task);

			Assert.Equal(5, value);
		}

		[Fact]
		public async Task ValueTaskReturnsAsyncWithMinAndMaxDelayAndOwnRandomGeneratorReturnsValue()
		{
			var mock = new Mock<IValueTaskAsyncInterface>();
			mock.Setup(x => x.RefParameterValueReturnType("test")).ReturnsAsync(5, TimeSpan.FromMilliseconds(1), TimeSpan.FromMilliseconds(2), new Random());

			var task = mock.Object.RefParameterValueReturnType("test");
			var value = await Assert.IsType<ValueTask<int>>(task);

			Assert.Equal(5, value);
		}

		[Fact]
		public void ValueTaskReturnsAsyncWithNullRandomGenerator()
		{
			var mock = new Mock<IValueTaskAsyncInterface>();

			Action setup = () => mock
				.Setup(x => x.RefParameterValueReturnType("test"))
				.ReturnsAsync(5, TimeSpan.FromMilliseconds(1), TimeSpan.FromMilliseconds(2), null);

			var paramName = Assert.Throws<ArgumentNullException>(setup).ParamName;
			Assert.Equal("random", paramName);
		}

		[Fact]
		public async Task ValueTaskThrowsWithDelay()
		{
			var mock = new Mock<IValueTaskAsyncInterface>();

			mock
				.Setup(x => x.RefParameterValueReturnType("test"))
				.ThrowsAsync(new ArithmeticException("yikes"), TimeSpan.FromMilliseconds(1));

			Func<ValueTask<int>> test = () => mock.Object.RefParameterValueReturnType("test");

			var exception = await Assert.ThrowsAsync<ArithmeticException>(() => test().AsTask());
			Assert.Equal("yikes", exception.Message);
		}

		[Fact]
		public async Task ValueTaskThrowsWithRandomDelay()
		{
			var mock = new Mock<IValueTaskAsyncInterface>();

			var minDelay = TimeSpan.FromMilliseconds(1);
			var maxDelay = TimeSpan.FromMilliseconds(2);

			mock
				.Setup(x => x.RefParameterValueReturnType("test"))
				.ThrowsAsync(new ArithmeticException("yikes"), minDelay, maxDelay);

			Func<ValueTask<int>> test = () => mock.Object.RefParameterValueReturnType("test");

			var exception = await Assert.ThrowsAsync<ArithmeticException>(() => test().AsTask());
			Assert.Equal("yikes", exception.Message);
		}

		[Fact]
		public async Task ValueTaskThrowsWithRandomDelayAndOwnRandomGenerator()
		{
			var mock = new Mock<IValueTaskAsyncInterface>();

			var minDelay = TimeSpan.FromMilliseconds(1);
			var maxDelay = TimeSpan.FromMilliseconds(2);

			mock
				.Setup(x => x.RefParameterValueReturnType("test"))
				.ThrowsAsync(new ArithmeticException("yikes"), minDelay, maxDelay, new Random());

			Func<ValueTask<int>> test = () => mock.Object.RefParameterValueReturnType("test");

			var exception = await Assert.ThrowsAsync<ArithmeticException>(() => test().AsTask());
			Assert.Equal("yikes", exception.Message);
		}

		[Fact]
		public void ValueTaskThrowsAsyncWithNullRandomGenerator()
		{
			var mock = new Mock<IValueTaskAsyncInterface>();

			Action setup = () =>
			{
				var anyException = new Exception();
				var minDelay = TimeSpan.FromMilliseconds(1);
				var maxDelay = TimeSpan.FromMilliseconds(2);

				mock
					.Setup(x => x.RefParameterValueReturnType("test"))
					.ThrowsAsync(anyException, minDelay, maxDelay, null);
			};

			var paramName = Assert.Throws<ArgumentNullException>(setup).ParamName;
			Assert.Equal("random", paramName);
		}

		[Fact]
		public async void No_parameters_object_return_type__ReturnsAsync_null__returns_completed_Task_with_null_result()
		{
			var mock = new Mock<IAsyncInterface>();
			mock.Setup(m => m.NoParametersObjectReturnType()).ReturnsAsync(null);

			var result = await mock.Object.NoParametersObjectReturnType();

			Assert.Null(result);
		}

		[Fact]
		public async void One_parameter_object_return_type__ReturnsAsync_null__returns_completed_Task_with_null_result()
		{
			var mock = new Mock<IAsyncInterface>();
			mock.Setup(m => m.OneParameterObjectReturnType("")).ReturnsAsync(null);

			var result = await mock.Object.OneParameterObjectReturnType("");

			Assert.Null(result);
		}

		[Fact]
		public async void Many_parameters_object_return_type__ReturnsAsync_null__returns_completed_Task_with_null_result()
		{
			var mock = new Mock<IAsyncInterface>();
			mock.Setup(m => m.ManyParametersObjectReturnType("", false, 0f)).ReturnsAsync(null);

			var result = await mock.Object.ManyParametersObjectReturnType("", false, 0f);

			Assert.Null(result);
		}

		[Fact]
		public async void No_parameters_object_return_type__ReturnsAsync_null__returns_completed_ValueTask_with_null_result()
		{
			var mock = new Mock<IValueTaskAsyncInterface>();
			mock.Setup(m => m.NoParametersObjectReturnType()).ReturnsAsync(null);

			var result = await mock.Object.NoParametersObjectReturnType();

			Assert.Null(result);
		}

		[Fact]
		public async void One_parameter_object_return_type__ReturnsAsync_null__returns_completed_ValueTask_with_null_result()
		{
			var mock = new Mock<IValueTaskAsyncInterface>();
			mock.Setup(m => m.OneParameterObjectReturnType("")).ReturnsAsync(null);

			var result = await mock.Object.OneParameterObjectReturnType("");

			Assert.Null(result);
		}

		[Fact]
		public async void Many_parameters_object_return_type__ReturnsAsync_null__returns_completed_ValueTask_with_null_result()
		{
			var mock = new Mock<IValueTaskAsyncInterface>();
			mock.Setup(m => m.ManyParametersObjectReturnType("", false, 0f)).ReturnsAsync(null);

			var result = await mock.Object.ManyParametersObjectReturnType("", false, 0f);

			Assert.Null(result);
		}
	}
}
