using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.Silverlight.Testing.Harness;

namespace Microsoft.Silverlight.Testing.UnitTesting.Metadata.XunitLight
{
	/// <summary>
	/// Test class wrapper.
	/// </summary>
	public class TestClass : ITestClass
	{
		/// <summary>
		/// Construct a new test class metadata interface.
		/// </summary>
		/// <param name="assembly">Assembly metadata interface object.</param>
		private TestClass(IAssembly assembly)
		{
			_tests = new List<ITestMethod>();

			//_m = new Dictionary<Methods, LazyMethodInfo>(4);
			//_m[Methods.ClassCleanup] = null;
			//_m[Methods.ClassInitialize] = null;
			//_m[Methods.TestCleanup] = null;
			//_m[Methods.TestInitialize] = null;

			Assembly = assembly;
		}

		/// <summary>
		/// Creates a new test class wrapper.
		/// </summary>
		/// <param name="assembly">Assembly metadata object.</param>
		/// <param name="testClassType">Type of the class.</param>
		public TestClass(IAssembly assembly, Type testClassType)
			: this(assembly)
		{
			_type = testClassType;

			if (_type == null)
			{
				throw new ArgumentNullException("testClassType");
			}

			//_m[Methods.ClassCleanup] = new LazyMethodInfo(_type, ProviderAttributes.ClassCleanup);
			//_m[Methods.ClassInitialize] = new LazyMethodInfo(_type, ProviderAttributes.ClassInitialize);
			//_m[Methods.TestCleanup] = new LazyMethodInfo(_type, ProviderAttributes.TestCleanup);
			//_m[Methods.TestInitialize] = new LazyMethodInfo(_type, ProviderAttributes.TestInitialize);
		}

		/// <summary>
		/// Methods enum.
		/// </summary>
		internal enum Methods
		{
			/// <summary>
			/// Initialize method.
			/// </summary>
			ClassInitialize,

			/// <summary>
			/// Cleanup method.
			/// </summary>
			ClassCleanup,

			/// <summary>
			/// Test init method.
			/// </summary>
			TestInitialize,

			/// <summary>
			/// Test cleanup method.
			/// </summary>
			TestCleanup,
		}

		/// <summary>
		/// Test Type.
		/// </summary>
		private Type _type;

		/// <summary>
		/// Collection of test method interface objects.
		/// </summary>
		private ICollection<ITestMethod> _tests;

		/// <summary>
		/// A value indicating whether tests are loaded.
		/// </summary>
		private bool _testsLoaded;

		/// <summary>
		/// A dictionary of method types and method interface objects.
		/// </summary>
		private IDictionary<Methods, LazyMethodInfo> _m;

		/// <summary>
		/// Gets the test assembly metadata.
		/// </summary>
		public IAssembly Assembly
		{
			get;
			protected set;
		}

		/// <summary>
		/// Gets the underlying Type of the test class.
		/// </summary>
		public Type Type
		{
			get { return _type; }
		}

		/// <summary>
		/// Gets the name of the test class.
		/// </summary>
		public string Name
		{
			get { return _type.Name; }
		}

		/// <summary>
		/// Gets a collection of test method  wrapper instances.
		/// </summary>
		/// <returns>A collection of test method interface objects.</returns>
		public ICollection<ITestMethod> GetTestMethods()
		{
			if (!_testsLoaded)
			{
				ICollection<MethodInfo> methods =
					ReflectionUtility.GetMethodsWithAttribute(
					_type, typeof(Xunit.FactAttribute));
				_tests = new List<ITestMethod>(methods.Count);
				foreach (MethodInfo method in methods)
				{
					_tests.Add(new TestMethod(method));
				}
				_testsLoaded = true;
			}
			return _tests;
		}

		/// <summary>
		/// Gets a value indicating whether an Ignore attribute present 
		/// on the class.
		/// </summary>
		public bool Ignore
		{
			get
			{
				return false;
				//return ReflectionUtility.HasAttribute(_type, ProviderAttributes.IgnoreAttribute); 
			}
		}

		/// <summary>
		/// Gets any test initialize method.
		/// </summary>
		public MethodInfo TestInitializeMethod
		{
			get
			{
				return null;
				//return _m[Methods.TestInitialize] == null ? null : _m[Methods.TestInitialize].GetMethodInfo(); 
			}
		}

		/// <summary>
		/// Gets any test cleanup method.
		/// </summary>
		public MethodInfo TestCleanupMethod
		{
			get
			{
				return null;
				//return _m[Methods.TestCleanup] == null ? null : _m[Methods.TestCleanup].GetMethodInfo();
			}
		}

		/// <summary>
		/// Gets any class initialize method.
		/// </summary>
		public MethodInfo ClassInitializeMethod
		{
			get
			{
				return null;
				//return _m[Methods.ClassInitialize] == null ? null : _m[Methods.ClassInitialize].GetMethodInfo(); 
			}
		}

		/// <summary>
		/// Gets any class cleanup method.
		/// </summary>
		public MethodInfo ClassCleanupMethod
		{
			get
			{
				return null;
				//return _m[Methods.ClassCleanup] == null ? null : _m[Methods.ClassCleanup].GetMethodInfo(); 
			}
		}
	}
}