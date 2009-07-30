using System;
using System.Reflection;

namespace Moq.Visualizer
{
	[Serializable]
	public class ParameterViewModel
	{
		public ParameterViewModel(ParameterInfo parameterInfo, object value)
		{
			this.Name = parameterInfo.Name;
			this.Type = parameterInfo.ParameterType.GetFullName();
			this.IsRef = parameterInfo.ParameterType.IsByRef && parameterInfo.Attributes != ParameterAttributes.Out;
			this.IsOut = parameterInfo.ParameterType.IsByRef && parameterInfo.Attributes == ParameterAttributes.Out;
			this.IsIn = !parameterInfo.ParameterType.IsByRef;
			this.Value = value;
		}

		public bool IsIn { get; private set; }

		public bool IsOut { get; private set; }

		public bool IsRef { get; private set; }
		
		public string Name { get; private set; }

		public string Type { get; private set; }

		public object Value { get; private set; }
	}
}