using System;
using System.Collections;
using System.Linq;
using System.Reflection;

namespace Moq.Visualizer.ViewModel
{
	[Serializable]
	public class CallViewModel
	{
		internal CallViewModel(MethodInfo methodInfo, object[] arguments, object returnValue)
		{
			this.Method = methodInfo.GetName();
			// TODO maiby I will need an ArgumentViewModel
			//methodInfo.GetParameters().Select(pi => pi.Name);
			this.Arguments = arguments.Select(a => this.GetValue(a)).ToArray();
			this.ReturnValue = this.GetValue(returnValue);
			this.MethodCall = this.GetMethodCall(methodInfo);
		}

		internal CallViewModel(string method, object[] arguments, object returnValue, string methodCall)
		{
			this.Method = method;
			this.Arguments = arguments;
			this.ReturnValue = returnValue;
		}

		private object GetValue(object value)
		{
			if (value != null)
			{
				// TODO get type from ParameterInfo, because of possible conversion errors
				var type = value.GetType();
				if (type.IsPrimitive || type.IsEnum)
				{
					return value;
				}

				if (value is string)
				{
					return "\"" + value + "\"";
				}

				return "<" + value.GetType().GetFullName() + ">";
			}

			return value;
		}

		public IEnumerable Arguments { get; private set; }

		public bool IsExpanded
		{
			get { return false; }
		}

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
			if (method.IsSpecialName && method.Name.StartsWith("get_"))
			{
				return method.Name.Substring(4) + " --> " + this.ReturnValue;
			}

			if (method.IsSpecialName && method.Name.StartsWith("set_"))
			{
				return method.Name.Substring(4) + " = " + this.GetValue(this.Arguments.Cast<object>().ElementAt(0));
			}

			var methodCall = this.Method + "(" +
				string.Join(",", this.Arguments.Cast<object>().Select(a => a.ToString()).ToArray()) + ")";
			if (this.ReturnValue == null)
			{
				return methodCall;
			}

			return " --> " + this.ReturnValue.ToString();
		}
	}
}