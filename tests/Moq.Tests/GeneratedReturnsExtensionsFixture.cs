// Copyright (c) 2007, Clarius Consulting, Manas Technology Solutions, InSTEDD, and Contributors.
// All rights reserved. Licensed under the BSD 3-Clause License; see License.txt.

using System;
using System.Linq;
using System.Threading.Tasks;

using Xunit;

#if FEATURE_ASYNC_ENUMERABLE
using System.Collections.Generic;
#endif

namespace Moq.Tests
{
	public class GeneratedReturnsExtensionsFixture
	{
		public interface IAsyncInterface
		{
			Task<int> WithSingleParameterAsync(int parameter);
			Task<string> WithMultiParameterAsync(string firstParameter, string secondParameter);
			Task<DateTime> WithParamsAsync(params DateTime[] dateTimes);

#if FEATURE_ASYNC_ENUMERABLE
			IAsyncEnumerable<int> AsyncEnumerableWithSingleParameterAsync(int parameter);
			IAsyncEnumerable<string> AsyncEnumerableWithMultiParameterAsync(string firstParameter, string secondParameter);
			IAsyncEnumerable<DateTime> AsyncEnumerableWithParamsAsync(params DateTime[] dateTimes);
#endif
		}

		[Fact]
		public void ReturnsAsync_onSingleParameter_ParameterUsedForCalculationOfTheResult()
		{
			var mock = new Mock<IAsyncInterface>();
			mock.Setup(x => x.WithSingleParameterAsync(It.IsAny<int>())).ReturnsAsync((int x) => x * x);

			int evaluationResult = mock.Object.WithSingleParameterAsync(2).Result;

			Assert.Equal(4, evaluationResult);
		}

		[Fact]
		public void ReturnsAsync_onSingleParameter_LazyEvaluationOfTheResult()
		{
			int coefficient = 5;
			var mock = new Mock<IAsyncInterface>();
			mock.Setup(x => x.WithSingleParameterAsync(It.IsAny<int>())).ReturnsAsync((int x) => x * coefficient);

			int firstEvaluationResult = mock.Object.WithSingleParameterAsync(2).Result;

			coefficient = 10;
			int secondEvaluationResult = mock.Object.WithSingleParameterAsync(2).Result;

			Assert.NotEqual(firstEvaluationResult, secondEvaluationResult);
		}

		[Fact]
		public void ReturnsAsync_onMultiParameter_ParametersUsedForCalculationOfTheResult()
		{
			var mock = new Mock<IAsyncInterface>();
			mock.Setup(x => x.WithMultiParameterAsync(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync((string first, string second) => first + second);

			string evaluationResult = mock.Object.WithMultiParameterAsync("Moq", "4").Result;

			Assert.Equal("Moq4", evaluationResult);
		}

		[Fact]
		public void ReturnsAsync_onMultiParameter_LazyEvaluationOfTheResult()
		{
			var mock = new Mock<IAsyncInterface>();
			mock.Setup(x => x.WithMultiParameterAsync(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync((string first, string second) => first + second);

			string firstEvaluationResult = mock.Object.WithMultiParameterAsync("Moq", "4").Result;
			string secondEvaluationResult = mock.Object.WithMultiParameterAsync("Moq", "4").Result;

			Assert.NotSame(firstEvaluationResult, secondEvaluationResult);
		}

		[Fact]
		public void ReturnsAsync_onParams_AllParametersUsedForCalculationOfTheResult()
		{
			var mock = new Mock<IAsyncInterface>();
			mock.Setup(x => x.WithParamsAsync(It.IsAny<DateTime[]>()))
				.ReturnsAsync((DateTime[] dateTimes) => dateTimes.Max());

			DateTime evaluationResult = mock.Object.WithParamsAsync(DateTime.MinValue, DateTime.Now, DateTime.MaxValue).Result;

			Assert.Equal(DateTime.MaxValue, evaluationResult);
		}

		[Fact]
		public void ReturnsAsync_onParams_LazyEvaluationOfTheResult()
		{
			DateTime comparedDateTime = DateTime.MinValue;
			var mock = new Mock<IAsyncInterface>();
			mock.Setup(x => x.WithParamsAsync(It.IsAny<DateTime[]>()))
				.ReturnsAsync((DateTime[] dateTimes) => dateTimes.Concat(new[] { comparedDateTime }).Max());

			DateTime now = DateTime.Now;
			DateTime firstEvaluationResult = mock.Object.WithParamsAsync(DateTime.MinValue, now).Result;

			comparedDateTime = DateTime.MaxValue;
			DateTime secondEvaluationResult = mock.Object.WithParamsAsync(DateTime.MinValue, now).Result;

			Assert.NotEqual(firstEvaluationResult, secondEvaluationResult);
		}

#if FEATURE_ASYNC_ENUMERABLE

		[Fact]
		public void AsyncEnumerableReturnsAsync_onSingleParameter_ParameterUsedForCalculationOfTheResult()
		{
			var mock = new Mock<IAsyncInterface>();
			mock.Setup(x => x.AsyncEnumerableWithSingleParameterAsync(It.IsAny<int>())).ReturnsAsync((int x) => new[] { x * x });

			int[] evaluationResult = mock.Object.AsyncEnumerableWithSingleParameterAsync(2).ToArrayAsync().Result;

			Assert.Equal(new[] { 4 }, evaluationResult);
		}

		[Fact]
		public void AsyncEnumerableReturnsAsync_onSingleParameter_LazyEvaluationOfTheResult()
		{
			int coefficient = 5;
			var mock = new Mock<IAsyncInterface>();
			mock.Setup(x => x.AsyncEnumerableWithSingleParameterAsync(It.IsAny<int>())).ReturnsAsync((int x) => new[] { x * coefficient });

			int[] firstEvaluationResult = mock.Object.AsyncEnumerableWithSingleParameterAsync(2).ToArrayAsync().Result;

			coefficient = 10;
			int[] secondEvaluationResult = mock.Object.AsyncEnumerableWithSingleParameterAsync(2).ToArrayAsync().Result;

			Assert.NotEqual(firstEvaluationResult, secondEvaluationResult);
		}

		[Fact]
		public void AsyncEnumerableReturnsAsync_onMultiParameter_ParametersUsedForCalculationOfTheResult()
		{
			var mock = new Mock<IAsyncInterface>();
			mock.Setup(x => x.AsyncEnumerableWithMultiParameterAsync(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync((string first, string second) => new[] { first + second });

			string[] evaluationResult = mock.Object.AsyncEnumerableWithMultiParameterAsync("Moq", "4").ToArrayAsync().Result;

			Assert.Equal(new[] { "Moq4" }, evaluationResult);
		}

		[Fact]
		public void AsyncEnumerableReturnsAsync_onMultiParameter_LazyEvaluationOfTheResult()
		{
			var mock = new Mock<IAsyncInterface>();
			mock.Setup(x => x.AsyncEnumerableWithMultiParameterAsync(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync((string first, string second) => new[] { first + second });

			string[] firstEvaluationResult = mock.Object.AsyncEnumerableWithMultiParameterAsync("Moq", "4").ToArrayAsync().Result;
			string[] secondEvaluationResult = mock.Object.AsyncEnumerableWithMultiParameterAsync("Moq", "4").ToArrayAsync().Result;

			Assert.NotSame(firstEvaluationResult, secondEvaluationResult);
		}

		[Fact]
		public void AsyncEnumerableReturnsAsync_onParams_AllParametersUsedForCalculationOfTheResult()
		{
			var mock = new Mock<IAsyncInterface>();
			mock.Setup(x => x.AsyncEnumerableWithParamsAsync(It.IsAny<DateTime[]>()))
				.ReturnsAsync((DateTime[] dateTimes) => dateTimes);

			var now = DateTime.Now;

			DateTime[] evaluationResult = mock.Object.AsyncEnumerableWithParamsAsync(DateTime.MinValue, now, DateTime.MaxValue).ToArrayAsync().Result;

			Assert.Equal(new[] { DateTime.MinValue, now, DateTime.MaxValue }, evaluationResult);
		}

		[Fact]
		public void AsyncEnumerableReturnsAsync_onParams_LazyEvaluationOfTheResult()
		{
			DateTime comparedDateTime = DateTime.MinValue;
			var mock = new Mock<IAsyncInterface>();
			mock.Setup(x => x.AsyncEnumerableWithParamsAsync(It.IsAny<DateTime[]>()))
				.ReturnsAsync((DateTime[] dateTimes) => dateTimes.Concat(new[] { comparedDateTime }));

			DateTime now = DateTime.Now;
			DateTime[] firstEvaluationResult = mock.Object.AsyncEnumerableWithParamsAsync(DateTime.MinValue, now).ToArrayAsync().Result;

			comparedDateTime = DateTime.MaxValue;
			DateTime[] secondEvaluationResult = mock.Object.AsyncEnumerableWithParamsAsync(DateTime.MinValue, now).ToArrayAsync().Result;

			Assert.NotEqual(firstEvaluationResult, secondEvaluationResult);
		}

#endif
	}
}
