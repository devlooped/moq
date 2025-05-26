// Copyright (c) 2007, Clarius Consulting, Manas Technology Solutions, InSTEDD, and Contributors.
// All rights reserved. Licensed under the BSD 3-Clause License; see License.txt.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Reflection;

using Moq.Properties;

namespace Moq
{
    abstract class SetupWithOutParameterSupport : MethodSetup
    {
        readonly List<KeyValuePair<int, object?>>? outValues;

        protected SetupWithOutParameterSupport(Expression? originalExpression, Mock mock, MethodExpectation expectation)
            : base(originalExpression, mock, expectation)
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

        static List<KeyValuePair<int, object?>>? GetOutValues(IReadOnlyList<Expression> arguments, ParameterInfo[] parameters)
        {
            List<KeyValuePair<int, object?>>? outValues = null;
            for (int i = 0, n = parameters.Length; i < n; ++i)
            {
                var parameter = parameters[i];
                if (parameter.ParameterType.IsByRef)
                {
                    if ((parameter.Attributes & (ParameterAttributes.In | ParameterAttributes.Out)) == ParameterAttributes.Out)
                    {
                        if (arguments[i].PartialEval() is not ConstantExpression constant)
                        {
                            throw new NotSupportedException(Resources.OutExpressionMustBeConstantValue);
                        }

                        if (outValues == null)
                        {
                            outValues = new List<KeyValuePair<int, object?>>();
                        }

                        outValues.Add(new KeyValuePair<int, object?>(i, constant.Value));
                    }
                }
            }
            return outValues;
        }
    }
}
