// Copyright (c) 2007, Clarius Consulting, Manas Technology Solutions, InSTEDD, and Contributors.
// All rights reserved. Licensed under the BSD 3-Clause License; see License.txt.

using System;

using Xunit;

namespace Moq.Tests
{
	public class InterceptorFixture
	{
		/// <summary>
		///   These tests document which methods can be intercepted (i.e. seen) by <see cref="IInterceptor"/>s.
		/// </summary>
		public class Method_interceptability
		{
			[Theory]
			[InlineData(typeof(ClassType))]
			[InlineData(typeof(IInterfaceType))]
			public void Object_Equals(Type typeToProxy)
			{
				var proxy = CreateProxy(typeToProxy, new Echo(true));
				var returnValue = proxy.Equals(42);
				Assert.True(returnValue);
			}

			[Theory]
			[InlineData(typeof(ClassType))]
			[InlineData(typeof(IInterfaceType))]
			public void Object_GetHashCode(Type typeToProxy)
			{
				var proxy = CreateProxy(typeToProxy, new Echo(42));
				var returnValue = proxy.GetHashCode();
				Assert.Equal(42, returnValue);
			}

			[Theory]
			[InlineData(typeof(ClassType))]
			[InlineData(typeof(IInterfaceType))]
			public void Object_ToString(Type typeToProxy)
			{
				var proxy = CreateProxy(typeToProxy, new Echo("42"));
				var returnValue = proxy.ToString();
				Assert.Equal("42", returnValue);
			}

			public abstract class ClassType { }

			public interface IInterfaceType { }
		}

		private static object CreateProxy(Type type, IInterceptor interceptor)
		{
			return ProxyFactory.Instance.CreateProxy(type, interceptor, Type.EmptyTypes, new object[0]);
		}

		private sealed class Echo : IInterceptor
		{
			private readonly object returnValue;

			public Echo(object returnValue)
			{
				this.returnValue = returnValue;
			}

			public void Intercept(IInvocation invocation)
			{
				invocation.ReturnValue = this.returnValue;
			}
		}
	}
}
