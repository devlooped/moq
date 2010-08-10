//Copyright (c) 2007. Clarius Consulting, Manas Technology Solutions, InSTEDD
//http://code.google.com/p/moq/
//All rights reserved.

//Redistribution and use in source and binary forms, 
//with or without modification, are permitted provided 
//that the following conditions are met:

//    * Redistributions of source code must retain the 
//    above copyright notice, this list of conditions and 
//    the following disclaimer.

//    * Redistributions in binary form must reproduce 
//    the above copyright notice, this list of conditions 
//    and the following disclaimer in the documentation 
//    and/or other materials provided with the distribution.

//    * Neither the name of Clarius Consulting, Manas Technology Solutions or InSTEDD nor the 
//    names of its contributors may be used to endorse 
//    or promote products derived from this software 
//    without specific prior written permission.

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

//[This is the BSD license, see
// http://www.opensource.org/licenses/bsd-license.php]

using System;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Moq.Language;
using Moq.Properties;

namespace Moq.Linq
{
	internal class MockSetupsBuilder : ExpressionVisitor
	{
		private static readonly string[] queryableMethods = new[] { "First", "Where", "FirstOrDefault" };
		private static readonly string[] unsupportedMethods = new[] { "All", "Any", "Last", "LastOrDefault", "Single", "SingleOrDefault" };

		private int stackIndex;
		private MethodCallExpression underlyingCreateMocks;

		public MockSetupsBuilder(MethodCallExpression underlyingCreateMocks)
		{
			this.underlyingCreateMocks = underlyingCreateMocks;
		}

		protected override Expression VisitBinary(BinaryExpression node)
		{
			if (node != null && this.stackIndex > 0)
			{
				if (node.NodeType != ExpressionType.Equal && node.NodeType != ExpressionType.AndAlso)
					throw new NotSupportedException(string.Format(CultureInfo.CurrentCulture, Resources.LinqBinaryOperatorNotSupported, node.ToStringFixed()));

				if (node.NodeType == ExpressionType.Equal)
				{
					// TODO: throw if a matcher is used on either side of the expression.
					//ThrowIfMatcherIsUsed(

					// Account for the inverted assignement/querying like "false == foo.IsValid" scenario
					if (node.Left.NodeType == ExpressionType.Constant)
						// Invert left & right nodes in this case.
						return ConvertToSetup(node.Right, node.Left) ?? base.VisitBinary(node);
					else
						// Perform straight conversion where the right handside will be the setup return value.
						return ConvertToSetup(node.Left, node.Right) ?? base.VisitBinary(node);
				}
			}

			return base.VisitBinary(node);
		}

		protected override Expression VisitConstant(ConstantExpression node)
		{
			if (node != null && node.Type.IsGenericType && node.Type.GetGenericTypeDefinition() == typeof(MockQueryable<>))
			{
				//var asQueryableMethod = createQueryableMethod.MakeGenericMethod(node.Type.GetGenericArguments()[0]);

				//return Expression.Call(null, asQueryableMethod);
				return this.underlyingCreateMocks;
			}

			return base.VisitConstant(node);
		}

		protected override Expression VisitMember(MemberExpression node)
		{
			if (node != null && this.stackIndex > 0 && node.Type == typeof(bool))
			{
				return ConvertToSetup(node.Expression, node, Expression.Constant(true));
			}

			return base.VisitMember(node);
		}

		protected override Expression VisitMethodCall(MethodCallExpression node)
		{
			if (node != null)
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
					return ConvertToSetup(node.Object, node, Expression.Constant(true));
				}
			}

			return base.VisitMethodCall(node);
		}

		protected override Expression VisitUnary(UnaryExpression node)
		{
			if (node != null && this.stackIndex > 0 && node.NodeType == ExpressionType.Not)
			{
				return ConvertToSetup(node.Operand, Expression.Constant(false)) ?? base.VisitUnary(node);
			}

			return base.VisitUnary(node);
		}

		private static Expression ConvertToSetup(Expression left, Expression right)
		{
			switch (left.NodeType)
			{
				case ExpressionType.MemberAccess:
					var member = (MemberExpression)left;
					member.ThrowIfNotMockeable();

					return ConvertToSetupProperty(member.Expression, member, right);

				case ExpressionType.Call:
					var method = (MethodCallExpression)left;

					if (!method.Method.CanOverride())
						throw new NotSupportedException(string.Format(
							CultureInfo.CurrentCulture,
							Resources.LinqMethodNotVirtual,
							method.ToStringFixed()));

					return ConvertToSetup(method.Object, method, right);

				case ExpressionType.Convert:
					var left1 = (UnaryExpression)left;
					return ConvertToSetup(left1.Operand, Expression.Convert(right, left1.Operand.Type));
			}

			return null;
		}

		private static Expression ConvertToSetupProperty(Expression targetObject, Expression left, Expression right)
		{
			// TODO: throw if target is a static class?
			var sourceType = targetObject.Type;
			var propertyInfo = (PropertyInfo)((MemberExpression)left).Member;
			var propertyType = propertyInfo.PropertyType;

			// where foo.Name == "bar"
			// becomes:	
			// where Mock.Get(foo).SetupProperty(mock => mock.Name, "bar") != null

			// if the property is readonly, we can only do a Setup(...) which is the same as a method setup.
			if (!propertyInfo.CanWrite)
				return ConvertToSetup(targetObject, left, right);

			// This will get up to and including the Mock.Get(foo).Setup(mock => mock.Name) call.
			var propertySetup = FluentMockVisitor.Accept(left);
			// We need to go back one level, to the target expression of the Setup call, 
			// which would be the Mock.Get(foo), where we will actually invoke SetupProperty instead.
			if (propertySetup.NodeType != ExpressionType.Call)
				throw new NotSupportedException("Unexpected translation of a member access: " + propertySetup.ToStringFixed());

			var propertyCall = (MethodCallExpression)propertySetup;
			var mockExpression = propertyCall.Object;
			var propertyExpression = propertyCall.Arguments.First().StripQuotes();

			// Because Mocks.CreateMocks (the underlying implementation of the IQueryable provider
			// already sets up all properties as stubs, we can safely just set the value here, 
			// which also allows the use of this querying capability against plain DTO even 
			// if their properties are not virtual.
			var setPropertyMethod = typeof(Mocks)
				.GetMethod("SetPropery", BindingFlags.Static | BindingFlags.NonPublic)
				.MakeGenericMethod(mockExpression.Type.GetGenericArguments().First(), propertyInfo.PropertyType);

			return Expression.Equal(
				Expression.Call(setPropertyMethod, mockExpression, propertyCall.Arguments.First(), right),
				Expression.Constant(true));
		}

		private static Expression ConvertToSetup(Expression targetObject, Expression left, Expression right)
		{
			// TODO: throw if target is a static class?
			var sourceType = targetObject.Type;
			var returnType = left.Type;

			var returnsMethod = typeof(IReturns<,>)
				.MakeGenericType(sourceType, returnType)
				.GetMethod("Returns", new[] { returnType });

			return Expression.NotEqual(
				Expression.Call(FluentMockVisitor.Accept(left), returnsMethod, right),
				Expression.Constant(null));
		}
	}
}
