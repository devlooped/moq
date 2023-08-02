// Copyright (c) 2007, Clarius Consulting, Manas Technology Solutions, InSTEDD, and Contributors.
// All rights reserved. Licensed under the BSD 3-Clause License; see License.txt.

using System;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;

using Moq.Properties;

namespace Moq.Linq
{

    /* Unmerged change from project 'Moq(netstandard2.0)'
    Before:
        internal class MockSetupsBuilder : ExpressionVisitor
    After:
        class MockSetupsBuilder : ExpressionVisitor
    */

    /* Unmerged change from project 'Moq(netstandard2.1)'
    Before:
        internal class MockSetupsBuilder : ExpressionVisitor
    After:
        class MockSetupsBuilder : ExpressionVisitor
    */

    /* Unmerged change from project 'Moq(net6.0)'
    Before:
        internal class MockSetupsBuilder : ExpressionVisitor
    After:
        class MockSetupsBuilder : ExpressionVisitor
    */
    class MockSetupsBuilder : ExpressionVisitor

    /* Unmerged change from project 'Moq(netstandard2.0)'
    Before:
            private static readonly string[] queryableMethods = new[] { "First", "Where", "FirstOrDefault" };
            private static readonly string[] unsupportedMethods = new[] { "All", "Any", "Last", "LastOrDefault", "Single", "SingleOrDefault" };
    After:
            static readonly string[] queryableMethods = new[] { "First", "Where", "FirstOrDefault" };
            static readonly string[] unsupportedMethods = new[] { "All", "Any", "Last", "LastOrDefault", "Single", "SingleOrDefault" };
    */

    /* Unmerged change from project 'Moq(netstandard2.1)'
    Before:
            private static readonly string[] queryableMethods = new[] { "First", "Where", "FirstOrDefault" };
            private static readonly string[] unsupportedMethods = new[] { "All", "Any", "Last", "LastOrDefault", "Single", "SingleOrDefault" };
    After:
            static readonly string[] queryableMethods = new[] { "First", "Where", "FirstOrDefault" };
            static readonly string[] unsupportedMethods = new[] { "All", "Any", "Last", "LastOrDefault", "Single", "SingleOrDefault" };
    */

    /* Unmerged change from project 'Moq(net6.0)'
    Before:
            private static readonly string[] queryableMethods = new[] { "First", "Where", "FirstOrDefault" };
            private static readonly string[] unsupportedMethods = new[] { "All", "Any", "Last", "LastOrDefault", "Single", "SingleOrDefault" };
    After:
            static readonly string[] queryableMethods = new[] { "First", "Where", "FirstOrDefault" };
            static readonly string[] unsupportedMethods = new[] { "All", "Any", "Last", "LastOrDefault", "Single", "SingleOrDefault" };
    */
    {
        static readonly string[] queryableMethods = new[] { "First", "Where", "FirstOrDefault" };
        static readonly string[] unsupportedMethods = new[] { "All", "Any", "Last", "LastOrDefault", "Single", "SingleOrDefault" };


        /* Unmerged change from project 'Moq(netstandard2.0)'
        Before:
                private int stackIndex;
                private int quoteDepth;
        After:
                int stackIndex;
                int quoteDepth;
        */

        /* Unmerged change from project 'Moq(netstandard2.1)'
        Before:
                private int stackIndex;
                private int quoteDepth;
        After:
                int stackIndex;
                int quoteDepth;
        */

        /* Unmerged change from project 'Moq(net6.0)'
        Before:
                private int stackIndex;
                private int quoteDepth;
        After:
                int stackIndex;
                int quoteDepth;
        */
        int stackIndex;
        int quoteDepth;

        public MockSetupsBuilder()
        {
        }

        protected override Expression VisitBinary(BinaryExpression node)
        {
            if (this.stackIndex > 0)
            {
                if (node.NodeType != ExpressionType.Equal && node.NodeType != ExpressionType.AndAlso)
                    throw new NotSupportedException(string.Format(CultureInfo.CurrentCulture, Resources.LinqBinaryOperatorNotSupported, node.ToStringFixed()));

                if (node.NodeType == ExpressionType.Equal)
                {
                    // TODO: throw if a matcher is used on either side of the expression.
                    //ThrowIfMatcherIsUsed(

                    // Account for the inverted assignment/querying like "false == foo.IsValid" scenario
                    if (node.Left.NodeType == ExpressionType.Constant)
                        // Invert left & right nodes in this case.
                        return ConvertToSetup(node.Right, node.Left) ?? base.VisitBinary(node);
                    else
                        // Perform straight conversion where the right-hand side will be the setup return value.
                        return ConvertToSetup(node.Left, node.Right) ?? base.VisitBinary(node);
                }
            }

            return base.VisitBinary(node);
        }

        protected override Expression VisitMember(MemberExpression node)
        {
            if (this.stackIndex > 0 && node.Type == typeof(bool))
            {
                return ConvertToSetupReturns(node, Expression.Constant(true));
            }

            return base.VisitMember(node);
        }

        protected override Expression VisitMethodCall(MethodCallExpression node)
        {
            if (node.Method.DeclaringType == typeof(Queryable) && queryableMethods.Contains(node.Method.Name))
            {
                this.stackIndex++;
                var result = base.VisitMethodCall(node);
                this.stackIndex--;
                return result;
            }

            if (unsupportedMethods.Contains(node.Method.Name))
            {
                throw new NotSupportedException(string.Format(
                    CultureInfo.CurrentCulture,
                    Resources.LinqMethodNotSupported,
                    node.Method.Name));
            }

            if (this.stackIndex > 0 && node.Type == typeof(bool))
            {
                return ConvertToSetupReturns(node, Expression.Constant(true));
            }

            return base.VisitMethodCall(node);
        }

        protected override Expression VisitUnary(UnaryExpression node)
        {
            if (this.stackIndex > 0 && node.NodeType == ExpressionType.Not)
            {
                return ConvertToSetup(node.Operand, Expression.Constant(false)) ?? base.VisitUnary(node);
            }

            if (node.NodeType == ExpressionType.Quote)
            {
                this.quoteDepth++;
                var result = this.quoteDepth > 1 ? node : base.VisitUnary(node);
                this.quoteDepth--;
                return result;
            }

            return base.VisitUnary(node);

            /* Unmerged change from project 'Moq(netstandard2.0)'
            Before:
                    private static Expression ConvertToSetup(Expression left, Expression right)
            After:
                    static Expression ConvertToSetup(Expression left, Expression right)
            */

            /* Unmerged change from project 'Moq(netstandard2.1)'
            Before:
                    private static Expression ConvertToSetup(Expression left, Expression right)
            After:
                    static Expression ConvertToSetup(Expression left, Expression right)
            */

            /* Unmerged change from project 'Moq(net6.0)'
            Before:
                    private static Expression ConvertToSetup(Expression left, Expression right)
            After:
                    static Expression ConvertToSetup(Expression left, Expression right)
            */
        }

        static Expression ConvertToSetup(Expression left, Expression right)
        {
            switch (left.NodeType)
            {
                case ExpressionType.Call:
                case ExpressionType.Invoke:
                case ExpressionType.MemberAccess:
                    return ConvertToSetupReturns(left, right);

                case ExpressionType.Convert:
                    var left1 = (UnaryExpression)left;
                    return ConvertToSetup(left1.Operand, Expression.Convert(right, left1.Operand.Type));
            }

            return null;

            /* Unmerged change from project 'Moq(netstandard2.0)'
            Before:
                    private static Expression ConvertToSetupReturns(Expression left, Expression right)
            After:
                    static Expression ConvertToSetupReturns(Expression left, Expression right)
            */

            /* Unmerged change from project 'Moq(netstandard2.1)'
            Before:
                    private static Expression ConvertToSetupReturns(Expression left, Expression right)
            After:
                    static Expression ConvertToSetupReturns(Expression left, Expression right)
            */

            /* Unmerged change from project 'Moq(net6.0)'
            Before:
                    private static Expression ConvertToSetupReturns(Expression left, Expression right)
            After:
                    static Expression ConvertToSetupReturns(Expression left, Expression right)
            */
        }

        /// <summary>
        ///   Converts a taken-apart binary expression such as `m.A.B` (==) `x` to
        ///   `Mocks.SetupReturns(Mock.Get(m), m' => m'.A.B, (object)x)`.
        /// </summary>
        /// <param name="left">Body of the expression to set up.</param>
        /// <param name="right">Return value to be configured for <paramref name="left"/>.</param>
        static Expression ConvertToSetupReturns(Expression left, Expression right)
        {
            var v = new ReplaceMockObjectWithParameter();
            var rewrittenLeft = v.Visit(left);

            return Expression.Call(
                Mock.SetupReturnsMethod,
                // mock:
                Expression.Call(
                    Mock.GetMethod.MakeGenericMethod(v.MockObject.Type),
                    v.MockObject),
                // expression:
                Expression.Lambda(
                    rewrittenLeft,
                    v.MockObjectParameter),
                // value:
                Expression.Convert(right, typeof(object)));  // explicit boxing operation required for value types

            /* Unmerged change from project 'Moq(netstandard2.0)'
            Before:
                    private sealed class ReplaceMockObjectWithParameter : ExpressionVisitor
            After:
                    sealed class ReplaceMockObjectWithParameter : ExpressionVisitor
            */

            /* Unmerged change from project 'Moq(netstandard2.1)'
            Before:
                    private sealed class ReplaceMockObjectWithParameter : ExpressionVisitor
            After:
                    sealed class ReplaceMockObjectWithParameter : ExpressionVisitor
            */

            /* Unmerged change from project 'Moq(net6.0)'
            Before:
                    private sealed class ReplaceMockObjectWithParameter : ExpressionVisitor
            After:
                    sealed class ReplaceMockObjectWithParameter : ExpressionVisitor
            */
        }

        /// <summary>
        ///   Locates the root mock object in a setup expression (which is usually, but not always, a <see cref="ParameterExpression"/>),
        ///   stores a reference to it, and finally replaces it with a new <see cref="ParameterExpression"/>.
        /// </summary>
        sealed class ReplaceMockObjectWithParameter : ExpressionVisitor

        /* Unmerged change from project 'Moq(netstandard2.0)'
        Before:
                    private Expression mockObject;
                    private ParameterExpression mockObjectParameter;
        After:
                    Expression mockObject;
                    ParameterExpression mockObjectParameter;
        */

        /* Unmerged change from project 'Moq(netstandard2.1)'
        Before:
                    private Expression mockObject;
                    private ParameterExpression mockObjectParameter;
        After:
                    Expression mockObject;
                    ParameterExpression mockObjectParameter;
        */

        /* Unmerged change from project 'Moq(net6.0)'
        Before:
                    private Expression mockObject;
                    private ParameterExpression mockObjectParameter;
        After:
                    Expression mockObject;
                    ParameterExpression mockObjectParameter;
        */
        {
            Expression mockObject;
            ParameterExpression mockObjectParameter;

            public Expression MockObject => this.mockObject;

            public ParameterExpression MockObjectParameter => this.mockObjectParameter;

            protected override Expression VisitMember(MemberExpression node)
            {
                if (node.Expression is ParameterExpression pe && pe.Type.IsDefined(typeof(CompilerGeneratedAttribute)) && pe.Type.Name.Contains("f__AnonymousType"))
                {
                    // In LINQ query expressions with more than one `from` clause such as:
                    //
                    //   from a in Mocks.Of<A>()
                    //   from b in Mocks.Of<B>()
                    //   where ...
                    //
                    // The `.Where()` operator will have a parameter `p` that is neither of type `A` nor `B`,
                    // but of a compiler-generated anonymous type (essentially that of `new { a, b }`).
                    //
                    // In such cases, the mock object is not referenced by `p` itself, but rather by any
                    // member of `p`, that is, `p.a` or `p.b`:
                    mockObject = node;
                    mockObjectParameter = Expression.Parameter(node.Type, pe.Name);
                    return mockObjectParameter;
                }
                else
                {
                    return base.VisitMember(node);
                }
            }

            protected override Expression VisitParameter(ParameterExpression node)
            {
                // Regular case: The parameter `p` directly references the mock object:
                mockObject = node;
                mockObjectParameter = Expression.Parameter(node.Type, node.Name);
                return mockObjectParameter;
            }

            protected override Expression VisitUnary(UnaryExpression node)
            {
                // An expression may contain nested expressions, such as:
                //
                //     m => m.GetObjectById(It.IsAny<int>(id => id > 0))
                //          ^                                   ^^
                //   We're only interested            ...but not in this one!
                //   in this parameter...
                //
                // Therefore, for the nested (quoted) expression, we short-circuit:
                return node.NodeType == ExpressionType.Quote ? node : base.VisitUnary(node);
            }
        }
    }
}
