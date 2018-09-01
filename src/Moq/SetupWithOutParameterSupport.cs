//Copyright (c) 2007. Clarius Consulting, Manas Technology Solutions, InSTEDD
//https://github.com/moq/moq4
//All rights reserved.
//
//Redistribution and use in source and binary forms,
//with or without modification, are permitted provided
//that the following conditions are met:
//
//    * Redistributions of source code must retain the
//    above copyright notice, this list of conditions and
//    the following disclaimer.
//
//    * Redistributions in binary form must reproduce
//    the above copyright notice, this list of conditions
//    and the following disclaimer in the documentation
//    and/or other materials provided with the distribution.
//
//    * Neither the name of Clarius Consulting, Manas Technology Solutions or InSTEDD nor the
//    names of its contributors may be used to endorse
//    or promote products derived from this software
//    without specific prior written permission.
//
//THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND
//CONTRIBUTORS "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES,
//INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF
//MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
//DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR
//CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL,
//SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING,
//BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR
//SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS
//INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY,
//WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING
//NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE
//OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF
//SUCH DAMAGE.
//
//[This is the BSD license, see
// http://www.opensource.org/licenses/bsd-license.php]

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

		protected SetupWithOutParameterSupport(MethodInfo method, IReadOnlyList<Expression> arguments, LambdaExpression expression)
			: base(new InvocationShape(method, arguments), expression)
		{
			Debug.Assert(arguments != null);

			this.outValues = GetOutValues(arguments, method.GetParameters());
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
