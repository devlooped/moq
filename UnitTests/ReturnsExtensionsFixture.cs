using System;
using System.IO;
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

	}
}
