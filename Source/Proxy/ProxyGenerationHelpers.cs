using System;
using System.ComponentModel;
using System.Reflection;
using Castle.DynamicProxy;

namespace Moq.Proxy
{
	/// <summary>
	/// Hook used to tells Castle which methods to proxy in mocked classes.
	/// 
	/// Here we proxy the default methods Castle suggests (everything Object's methods)
	/// plus Object.ToString(), so we can give mocks useful default names.
	/// 
	/// This is required to allow Moq to mock ToString on proxy *class* implementations.
	/// </summary>
	internal class ProxyMethodHook : AllMethodsHook
	{
		/// <summary>
		/// Extends AllMethodsHook.ShouldInterceptMethod to also intercept Object.ToString().
		/// </summary>
		public override bool ShouldInterceptMethod(Type type, MethodInfo methodInfo)
		{
			var isObjectToString = methodInfo.DeclaringType == typeof(Object) && methodInfo.Name == "ToString";
			return base.ShouldInterceptMethod(type, methodInfo) || isObjectToString;
		}
	}

	/// <summary>
	/// <para>The base class used for all our interface-inheriting proxies, which overrides the default
	/// Object.ToString() behavior, to route it via the mock by default, unless overriden by a
	/// real implementation.</para>
	/// 
	/// <para>This is required to allow Moq to mock ToString on proxy *interface* implementations.</para>
	/// </summary>
	/// <remarks>
	/// <para><strong>This is internal to Moq and should not be generally used.</strong></para>
	/// 
	/// <para>Unfortunately it must be public, due to cross-assembly visibility issues with reflection, 
	/// see github.com/Moq/moq4/issues/98 for details.</para>
	/// </remarks>
	[EditorBrowsable(EditorBrowsableState.Never)]
	public abstract class InterfaceProxy
	{
		/// <summary>
		/// Overrides the default ToString implementation to instead find the mock for this mock.Object,
		/// and return MockName + '.Object' as the mocked object's ToString, to make it easy to relate
		/// mocks and mock object instances in error messages.
		/// </summary>
		public override string ToString()
		{
			return ((IMocked)this).Mock.ToString() + ".Object";
		}
	}
}
