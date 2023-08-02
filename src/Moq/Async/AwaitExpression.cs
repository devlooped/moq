// Copyright (c) 2007, Clarius Consulting, Manas Technology Solutions, InSTEDD, and Contributors.
// All rights reserved. Licensed under the BSD 3-Clause License; see License.txt.

using System;
using System.Diagnostics;
using System.Linq.Expressions;

namespace Moq.Async
{

    /* Unmerged change from project 'Moq(netstandard2.0)'
    Before:
        internal sealed class AwaitExpression : Expression
    After:
        sealed class AwaitExpression : Expression
    */

    /* Unmerged change from project 'Moq(netstandard2.1)'
    Before:
        internal sealed class AwaitExpression : Expression
    After:
        sealed class AwaitExpression : Expression
    */

    /* Unmerged change from project 'Moq(net6.0)'
    Before:
        internal sealed class AwaitExpression : Expression
    After:
        sealed class AwaitExpression : Expression
    */
    sealed class AwaitExpression : Expression

    /* Unmerged change from project 'Moq(netstandard2.0)'
    Before:
            private readonly IAwaitableFactory awaitableFactory;
            private readonly Expression operand;
    After:
            readonly IAwaitableFactory awaitableFactory;
            readonly Expression operand;
    */

    /* Unmerged change from project 'Moq(netstandard2.1)'
    Before:
            private readonly IAwaitableFactory awaitableFactory;
            private readonly Expression operand;
    After:
            readonly IAwaitableFactory awaitableFactory;
            readonly Expression operand;
    */

    /* Unmerged change from project 'Moq(net6.0)'
    Before:
            private readonly IAwaitableFactory awaitableFactory;
            private readonly Expression operand;
    After:
            readonly IAwaitableFactory awaitableFactory;
            readonly Expression operand;
    */
    {
        readonly IAwaitableFactory awaitableFactory;
        readonly Expression operand;

        public AwaitExpression(Expression operand, IAwaitableFactory awaitableFactory)
        {
            Debug.Assert(awaitableFactory != null);
            Debug.Assert(operand != null);

            this.awaitableFactory = awaitableFactory;
            this.operand = operand;
        }

        public override bool CanReduce => false;

        public override ExpressionType NodeType => ExpressionType.Extension;

        public Expression Operand => this.operand;

        public override Type Type => this.awaitableFactory.ResultType;

        public override string ToString()
        {
            return this.awaitableFactory.ResultType == typeof(void) ? $"await {this.operand}"
                                                                    : $"(await {this.operand})";
        }

        protected override Expression VisitChildren(ExpressionVisitor visitor) => this;
    }
}
