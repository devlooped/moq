// Copyright (c) 2007, Clarius Consulting, Manas Technology Solutions, InSTEDD, and Contributors.
// All rights reserved. Licensed under the BSD 3-Clause License; see License.txt.

using System.Collections.Generic;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Reflection;

namespace Moq
{

    /* Unmerged change from project 'Moq(netstandard2.0)'
    Before:
        internal sealed class StubbedPropertySetup : Setup
    After:
        sealed class StubbedPropertySetup : Setup
    */

    /* Unmerged change from project 'Moq(netstandard2.1)'
    Before:
        internal sealed class StubbedPropertySetup : Setup
    After:
        sealed class StubbedPropertySetup : Setup
    */

    /* Unmerged change from project 'Moq(net6.0)'
    Before:
        internal sealed class StubbedPropertySetup : Setup
    After:
        sealed class StubbedPropertySetup : Setup
    */
    sealed class StubbedPropertySetup : Setup

    /* Unmerged change from project 'Moq(netstandard2.0)'
    Before:
            private object value;
    After:
            object value;
    */

    /* Unmerged change from project 'Moq(netstandard2.1)'
    Before:
            private object value;
    After:
            object value;
    */

    /* Unmerged change from project 'Moq(net6.0)'
    Before:
            private object value;
    After:
            object value;
    */
    {
        object value;

        public StubbedPropertySetup(Mock mock, LambdaExpression expression, MethodInfo getter, MethodInfo setter, object initialValue)
            : base(originalExpression: null, mock, new PropertyAccessorExpectation(expression, getter, setter))
        {
            // NOTE:
            //
            // At this time, this setup type does not require both a `getter` and a `setter` to be present,
            // even though a stubbed property doesn't make much sense if either one is missing.
            //
            // This supports the `HandleAutoSetupProperties` interception step backing `SetupAllProperties`.
            // Once there is another dedicated setup type for `SetupAllProperties`, we may want to require
            // both accessors here.

            this.value = initialValue;

            this.MarkAsVerifiable();
        }

        public override IEnumerable<Mock> InnerMocks
        {
            get
            {
                var innerMock = TryGetInnerMockFrom(this.value);
                if (innerMock != null)
                {
                    yield return innerMock;
                }
            }
        }

        protected override void ExecuteCore(Invocation invocation)
        {
            if (invocation.Method.ReturnType == typeof(void))
            {
                Debug.Assert(invocation.Method.IsSetAccessor());
                Debug.Assert(invocation.Arguments.Length == 1);

                this.value = invocation.Arguments[0];
            }
            else
            {
                Debug.Assert(invocation.Method.IsGetAccessor());

                invocation.ReturnValue = this.value;
            }
        }

        public override string ToString()
        {
            return base.ToString() + " (stubbed)";
        }

        protected override void VerifySelf()

        /* Unmerged change from project 'Moq(netstandard2.0)'
        Before:
                private sealed class PropertyAccessorExpectation : Expectation
        After:
                sealed class PropertyAccessorExpectation : Expectation
        */

        /* Unmerged change from project 'Moq(netstandard2.1)'
        Before:
                private sealed class PropertyAccessorExpectation : Expectation
        After:
                sealed class PropertyAccessorExpectation : Expectation
        */

        /* Unmerged change from project 'Moq(net6.0)'
        Before:
                private sealed class PropertyAccessorExpectation : Expectation
        After:
                sealed class PropertyAccessorExpectation : Expectation
        */
        {
        }

        sealed class PropertyAccessorExpectation : Expectation

        /* Unmerged change from project 'Moq(netstandard2.0)'
        Before:
                    private readonly LambdaExpression expression;
                    private readonly MethodInfo getter;
                    private readonly MethodInfo setter;
        After:
                    readonly LambdaExpression expression;
                    readonly MethodInfo getter;
                    readonly MethodInfo setter;
        */

        /* Unmerged change from project 'Moq(netstandard2.1)'
        Before:
                    private readonly LambdaExpression expression;
                    private readonly MethodInfo getter;
                    private readonly MethodInfo setter;
        After:
                    readonly LambdaExpression expression;
                    readonly MethodInfo getter;
                    readonly MethodInfo setter;
        */

        /* Unmerged change from project 'Moq(net6.0)'
        Before:
                    private readonly LambdaExpression expression;
                    private readonly MethodInfo getter;
                    private readonly MethodInfo setter;
        After:
                    readonly LambdaExpression expression;
                    readonly MethodInfo getter;
                    readonly MethodInfo setter;
        */
        {
            readonly LambdaExpression expression;
            readonly MethodInfo getter;
            readonly MethodInfo setter;

            public PropertyAccessorExpectation(LambdaExpression expression, MethodInfo getter, MethodInfo setter)
            {
                Debug.Assert(expression != null);
                Debug.Assert(expression.IsProperty());
                Debug.Assert(getter != null || setter != null);

                this.expression = expression;
                this.getter = getter;
                this.setter = setter;
            }

            public override LambdaExpression Expression => this.expression;

            public override bool Equals(Expectation obj)
            {
                return obj is PropertyAccessorExpectation other
                    && other.getter == this.getter
                    && other.setter == this.setter;
            }

            public override int GetHashCode()
            {
                return unchecked((this.getter?.GetHashCode() ?? 0) + 103 * (this.setter?.GetHashCode() ?? 0));
            }

            public override bool IsMatch(Invocation invocation)
            {
                var methodName = invocation.Method.Name;
                return methodName == this.getter.Name || methodName == this.setter.Name;
            }
        }
    }
}
