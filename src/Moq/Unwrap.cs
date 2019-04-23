// Copyright (c) 2007, Clarius Consulting, Manas Technology Solutions, InSTEDD.
// All rights reserved. Licensed under the BSD 3-Clause License; see License.txt.

using System.Threading.Tasks;

namespace Moq
{
	internal static class Unwrap
	{
		/// <summary>
		///   Recursively unwraps the result of completed <see cref="Task{TResult}"/> or <see cref="ValueTask{TResult}"/> instances.
		///   If the given value is not a task, the value itself is returned.
		/// </summary>
		/// <param name="obj">The value to be unwrapped.</param>
		public static object ResultIfCompletedTask(object obj)
		{
			if (obj != null)
			{
				var objType = obj.GetType();
				if (objType.IsGenericType)
				{
					var genericTypeDefinition = objType.GetGenericTypeDefinition();
					if (genericTypeDefinition == typeof(Task<>) || genericTypeDefinition == typeof(ValueTask<>))
					{
						var isCompleted = (bool)objType.GetProperty("IsCompleted").GetValue(obj, null);
						if (isCompleted)
						{
							var innerObj = objType.GetProperty("Result").GetValue(obj, null);
							return Unwrap.ResultIfCompletedTask(innerObj);
						}
					}
				}
			}

			return obj;
		}
	}
}
