using System;
using System.Threading.Tasks;
using Xunit;

namespace Moq.Tests
{
    public class ReturnsExtensionsFixture
    {
        public interface IAsyncInterface
        {
            Task<string> NoParametersRefReturnType();
            
            Task<int> NoParametersValueReturnType();

            Task<string> RefParameterRefReturnType(string value);
            
            Task<int> RefParameterValueReturnType(string value);

            Task<string> ValueParameterRefReturnType(int value);
         
            Task<int> ValueParameterValueReturnType(int value);
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
    }
}
