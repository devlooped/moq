using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Moq.Visualizer
{
	[Serializable]
	public class CallViewModel
	{
		internal CallViewModel(MethodInfo methodInfo, object[] arguments, object returnValue)
		{
			this.Method = methodInfo.GetName();
			this.IsVoid = methodInfo.ReturnType == typeof(void);

			var index = 0;
			this.Arguments = methodInfo.GetParameters()
				.Select(pi => new ParameterViewModel(pi, this.GetValue(arguments[index++]))).ToArray();

			this.ReturnValue = this.GetValue(returnValue);
			this.MethodCall = this.GetMethodCall(methodInfo);
		}

		private object GetValue(object value)
		{
			if (value == null)
			{
				return "<null>";
			}

			// TODO get type from ParameterInfo, because of possible conversion errors
			// NOTE Primitive types are: Boolean, Byte, SByte, Int16, UInt16, Int32, UInt32, Int64, UInt64, IntPtr, UIntPtr, Char, Double, and Single
			var type = value.GetType();
			if (type.IsPrimitive || value is decimal || value is DateTime || value is TimeSpan)
			{
				return value;
			}

			// TODO take care about BitMap enums
			if (type.IsEnum)
			{
				var values = value.ToString().Split(
					new char[] { ',', ' ' },
					StringSplitOptions.RemoveEmptyEntries);
				var typeName = type.GetName();
				return string.Join(" | ", values.Select(v => typeName + "." + v).ToArray());
			}

			if (value is string)
			{
				return "\"" + value + "\"";
			}

			return "<" + type.GetName() + ">";
		}

		public IEnumerable<ParameterViewModel> Arguments { get; private set; }

		public bool IsExpanded
		{
			get { return false; }
		}

		public bool IsVoid { get; private set; }

		public bool HasSetup { get; set; }

		public string Method { get; private set; }

		public object ReturnValue { get; private set; }

		public string MethodCall { get; private set; }

		// TODO Property formating
		// TODO ParamArray method
		// TODO Void members
		// TODO indexers
		private string GetMethodCall(MethodInfo method)
		{
			if (method.IsPropertyGetter())
			{
				return method.Name.Substring(4) + " → " + this.ReturnValue;
			}

			if (method.IsPropertySetter())
			{
				return method.Name.Substring(4) + " = " + this.GetValue(this.Arguments.ElementAt(0).Value);
			}

			var methodCall = this.Method + "(" +
				string.Join(",", this.Arguments.Select(a => GetKind(a) + a.Name + ": " + a.Value.ToString()).ToArray()) + ")";
			if (this.IsVoid)
			{
				return methodCall;
			}

			return " → " + this.ReturnValue.ToString();
		}

		private static string GetKind(ParameterViewModel a)
		{
			if (a.IsIn)
			{
				return string.Empty;
			}

			if (a.IsOut)
			{
				return "out ";
			}

			if (a.IsRef)
			{
				return "ref ";
			}

			throw new Exception();
		}
	}
}