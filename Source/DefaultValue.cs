using System;
using System.Collections.Generic;

namespace Moq
{
	/// <summary>
	/// Represents the default value returned from invocations that 
	/// do not have expectations or return values, with loose mocks.
	/// </summary>
	internal class DefaultValue
	{
		public DefaultValue(Type valueType)
		{
			// Return default value.
			if (valueType.IsValueType)
			{
				if (valueType.IsAssignableFrom(typeof(int)))
					this.Value = 0;
				else
					this.Value = Activator.CreateInstance(valueType);
			}
			else
			{
				if (valueType.IsArray)
				{
					this.Value = Activator.CreateInstance(valueType, 0);
				}
				else if (valueType == typeof(System.Collections.IEnumerable))
				{
					this.Value = new object[0];
				}
				else if (valueType.IsGenericType &&
					valueType.GetGenericTypeDefinition() == typeof(IEnumerable<>))
				{
					var genericListType = typeof(List<>).MakeGenericType(
						valueType.GetGenericArguments()[0]);
					this.Value = Activator.CreateInstance(genericListType);
				}
				else
				{
					this.Value = null;
				}
			}
		}

		public object Value { get; private set; }
	}
}
