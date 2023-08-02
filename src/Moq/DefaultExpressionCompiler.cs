// Copyright (c) 2007, Clarius Consulting, Manas Technology Solutions, InSTEDD, and Contributors.
// All rights reserved. Licensed under the BSD 3-Clause License; see License.txt.

using System;
using System.Linq.Expressions;

namespace Moq
{

    /* Unmerged change from project 'Moq(netstandard2.0)'
    Before:
        internal sealed class DefaultExpressionCompiler : ExpressionCompiler
    After:
        sealed class DefaultExpressionCompiler : ExpressionCompiler
    */

    /* Unmerged change from project 'Moq(netstandard2.1)'
    Before:
        internal sealed class DefaultExpressionCompiler : ExpressionCompiler
    After:
        sealed class DefaultExpressionCompiler : ExpressionCompiler
    */

    /* Unmerged change from project 'Moq(net6.0)'
    Before:
        internal sealed class DefaultExpressionCompiler : ExpressionCompiler
    After:
        sealed class DefaultExpressionCompiler : ExpressionCompiler
    */
    sealed class DefaultExpressionCompiler : ExpressionCompiler
    {
        new public static readonly DefaultExpressionCompiler Instance = new DefaultExpressionCompiler();


        /* Unmerged change from project 'Moq(netstandard2.0)'
        Before:
                private DefaultExpressionCompiler()
        After:
                DefaultExpressionCompiler()
        */

        /* Unmerged change from project 'Moq(netstandard2.1)'
        Before:
                private DefaultExpressionCompiler()
        After:
                DefaultExpressionCompiler()
        */

        /* Unmerged change from project 'Moq(net6.0)'
        Before:
                private DefaultExpressionCompiler()
        After:
                DefaultExpressionCompiler()
        */
        DefaultExpressionCompiler()
        {
        }

        public override Delegate Compile(LambdaExpression expression)
        {
            return expression.Compile();
        }

        public override TDelegate Compile<TDelegate>(Expression<TDelegate> expression)
        {
            return expression.Compile();
        }
    }
}
