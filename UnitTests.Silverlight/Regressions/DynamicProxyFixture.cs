using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Xunit;
using Castle.DynamicProxy;

namespace Moq.Tests.Regressions
{
	public class DynamicProxyFixture
	{
		[Fact]
		public void CanProxyClassWithProtectedAbstractMethod()
		{
			new ProxyGenerator().CreateClassProxy<TestClass>(new ProxyGenerationOptions());
		}

		[Fact]
		public void CanMockClassWithProtectedAbstractMethods()
		{
			var mock = new Mock<TestClass>();

			Assert.NotNull(mock.Object);
		}

		public abstract class TestClass
		{
			protected abstract void Foo();
		}
	}
}