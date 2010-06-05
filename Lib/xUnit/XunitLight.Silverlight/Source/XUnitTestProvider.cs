using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.Silverlight.Testing.Harness;

namespace Microsoft.Silverlight.Testing.UnitTesting.Metadata.XunitLight
{
	/// <summary>
	/// Provider xUnit metadata.
	/// </summary>
	public class XUnitTestProvider : IUnitTestProvider
	{
		/// <summary>
		/// Name of this provider.
		/// </summary>
		private const string ProviderName = "xUnitLite";

		/// <summary>
		/// The capabilities of the xUnit provider.
		/// </summary>
		private const UnitTestProviderCapabilities MyCapabilities =
			UnitTestProviderCapabilities.MethodCanIgnore;

		/// <summary>
		/// Whether the capability is supported by this provider.
		/// </summary>
		/// <param name="capability">Capability type.</param>
		/// <returns>A value indicating whether the capability is available.</returns>
		public bool HasCapability(UnitTestProviderCapabilities capability)
		{
			return ((capability & MyCapabilities) == capability);
		}

		/// <summary>
		/// Create a new Visual Studio Team Test unit test framework provider 
		/// instance.
		/// </summary>
		public XUnitTestProvider()
		{
			_assemblyCache = new Dictionary<Assembly, IAssembly>(2);
		}

		/// <summary>
		/// Cache of assemblies and assembly unit test interface objects.
		/// </summary>
		private Dictionary<Assembly, IAssembly> _assemblyCache;

		/// <summary>
		/// VSTT unit test provider constructor; takes an assembly reference to 
		/// perform reflection on to retrieve all test class types. In this 
		/// implementation of an engine for the VSTT metadata, only a single 
		/// test Assembly can be utilized at a time for simplicity.
		/// </summary>
		/// <param name="testHarness">The unit test harness.</param>
		/// <param name="assemblyReference">Assembly reflection object.</param>
		/// <returns>Returns the assembly metadata interface.</returns>
		public IAssembly GetUnitTestAssembly(UnitTestHarness testHarness, Assembly assemblyReference)
		{
			if (_assemblyCache.ContainsKey(assemblyReference))
			{
				return _assemblyCache[assemblyReference];
			}
			else
			{
				_assemblyCache[assemblyReference] = new UnitTestFrameworkAssembly(this, testHarness, assemblyReference);
				return _assemblyCache[assemblyReference];
			}
		}

		/// <summary>
		/// Check if the Exception is actually a failed assertion.
		/// </summary>
		/// <param name="exception">Exception object to check.</param>
		/// <returns>True if the exception is actually an assert failure.</returns>
		public bool IsFailedAssert(Exception exception)
		{
			throw new NotImplementedException();
			//Type et = exception.GetType();
			//Type nuAsserts = typeof(NU.AssertionException);
			//return (et == nuAsserts || et.IsSubclassOf(nuAsserts));
		}

		/// <summary>
		/// Gets the name of the provider.
		/// </summary>
		public string Name
		{
			get { return ProviderName; }
		}

		/// <summary>
		/// Gets the specialized capability descriptor.
		/// </summary>
		public UnitTestProviderCapabilities Capabilities
		{
			get { return MyCapabilities; }
		}
	}
}