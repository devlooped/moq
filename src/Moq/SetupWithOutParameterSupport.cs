// Copyright (c) 2007, Clarius Consulting, Manas Technology Solutions, InSTEDD.
// All rights reserved. Licensed under the BSD 3-Clause License; see License.txt.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Reflection;

using Moq.Properties;

namespace Moq
{
	internal abstract class SetupWithOutParameterSupport : Setup
	{
		private readonly List<KeyValuePair<int, object>> outValues;

		protected SetupWithOutParameterSupport(FluentSetup fluentSetup, Mock mock, InvocationShape expectation)
			: base(fluentSetup, mock, expectation)
		{
			Debug.Assert(expectation != null);

			this.outValues = GetOutValues(expectation.Arguments, expectation.Method.GetParameters());
		}

		public sealed override void SetOutParameters(Invocation invocation)
		{
			if (this.outValues != null)
			{
				foreach (var item in this.outValues)
				{
					invocation.Arguments[item.Key] = item.Value;
				}
			}
		}

		private static List<KeyValuePair<int, object>> GetOutValues(IReadOnlyList<Expression> arguments, ParameterInfo[] parameters)
		{
			List<KeyValuePair<int, object>> outValues = null;
			for (int i = 0, n = parameters.Length; i < n; ++i)
			{
				var parameter = parameters[i];
				if (parameter.ParameterType.IsByRef)
				{
					if ((parameter.Attributes & (ParameterAttributes.In | ParameterAttributes.Out)) == ParameterAttributes.Out)
					{
						var constant = arguments[i].PartialEval() as ConstantExpression;
						if (constant == null)
						{
							throw new NotSupportedException(Resources.OutExpressionMustBeConstantValue);
						}

						if (outValues == null)
						{
							outValues = new List<KeyValuePair<int, object>>();
						}

						outValues.Add(new KeyValuePair<int, object>(i, constant.Value));
					}
				}
			}
			return outValues;
		}
	}
}
