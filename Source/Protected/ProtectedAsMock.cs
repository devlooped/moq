//Copyright (c) 2007. Clarius Consulting, Manas Technology Solutions, InSTEDD
//https://github.com/moq/moq4
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
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

using Moq.Language;
using Moq.Language.Flow;
using Moq.Properties;

namespace Moq.Protected
{
	internal sealed class ProtectedAsMock<T, TDuck> : IProtectedAsMock<T, TDuck>
		where T : class
		where TDuck : class
    {
		private Mock<T> mock;

		private static DuckReplacer DuckReplacerInstance = new DuckReplacer(typeof(TDuck), typeof(T));

		public ProtectedAsMock(Mock<T> mock)
		{
			Debug.Assert(mock != null);

			this.mock = mock;
		}

		public ISetup<T> Setup(Expression<Action<TDuck>> expression)
		{
			Guard.NotNull(expression, nameof(expression));

			Expression<Action<T>> rewrittenExpression;
			try
			{
				rewrittenExpression = (Expression<Action<T>>)ReplaceDuck(expression);
			}
			catch (ArgumentException ex)
			{
				throw new ArgumentException(ex.Message, nameof(expression));
			}

			return Mock.Setup(this.mock, rewrittenExpression, null);
		}

		public ISetup<T, TResult> Setup<TResult>(Expression<Func<TDuck, TResult>> expression)
		{
			Guard.NotNull(expression, nameof(expression));

			Expression<Func<T, TResult>> rewrittenExpression;
			try
			{
				rewrittenExpression = (Expression<Func<T, TResult>>)ReplaceDuck(expression);
			}
			catch (ArgumentException ex)
			{
				throw new ArgumentException(ex.Message, nameof(expression));
			}

			return Mock.Setup(this.mock, rewrittenExpression, null);
		}

		public ISetupGetter<T, TProperty> SetupGet<TProperty>(Expression<Func<TDuck, TProperty>> expression)
		{
			Guard.NotNull(expression, nameof(expression));

			Expression<Func<T, TProperty>> rewrittenExpression;
			try
			{
				rewrittenExpression = (Expression<Func<T, TProperty>>)ReplaceDuck(expression);
			}
			catch (ArgumentException ex)
			{
				throw new ArgumentException(ex.Message, nameof(expression));
			}

			return Mock.SetupGet(this.mock, rewrittenExpression, null);
		}

		public Mock<T> SetupProperty<TProperty>(Expression<Func<TDuck, TProperty>> expression, TProperty initialValue = default(TProperty))
		{
			Guard.NotNull(expression, nameof(expression));

			Expression<Func<T, TProperty>> rewrittenExpression;
			try
			{
				rewrittenExpression = (Expression<Func<T, TProperty>>)ReplaceDuck(expression);
			}
			catch (ArgumentException ex)
			{
				throw new ArgumentException(ex.Message, nameof(expression));
			}

			return this.mock.SetupProperty<TProperty>(rewrittenExpression, initialValue);
		}

		public ISetupSequentialResult<TResult> SetupSequence<TResult>(Expression<Func<TDuck, TResult>> expression)
		{
			Guard.NotNull(expression, nameof(expression));

			Expression<Func<T, TResult>> rewrittenExpression;
			try
			{
				rewrittenExpression = (Expression<Func<T, TResult>>)ReplaceDuck(expression);
			}
			catch (ArgumentException ex)
			{
				throw new ArgumentException(ex.Message, nameof(expression));
			}

			return Mock.SetupSequence<TResult>(this.mock, rewrittenExpression);
		}

		public ISetupSequentialAction SetupSequence(Expression<Action<TDuck>> expression)
		{
			Guard.NotNull(expression, nameof(expression));

			Expression<Action<T>> rewrittenExpression;
			try
			{
				rewrittenExpression = (Expression<Action<T>>)ReplaceDuck(expression);
			}
			catch (ArgumentException ex)
			{
				throw new ArgumentException(ex.Message, nameof(expression));
			}

			return Mock.SetupSequence(this.mock, rewrittenExpression);
		}

		public void Verify(Expression<Action<TDuck>> expression, Times? times = null, string failMessage = null)
		{
			Guard.NotNull(expression, nameof(expression));

			Expression<Action<T>> rewrittenExpression;
			try
			{
				rewrittenExpression = (Expression<Action<T>>)ReplaceDuck(expression);
			}
			catch (ArgumentException ex)
			{
				throw new ArgumentException(ex.Message, nameof(expression));
			}

			Mock.Verify(this.mock, rewrittenExpression, times ?? Times.AtLeastOnce(), failMessage);
		}

		public void Verify<TResult>(Expression<Func<TDuck, TResult>> expression, Times? times = null, string failMessage = null)
		{
			Guard.NotNull(expression, nameof(expression));

			Expression<Func<T, TResult>> rewrittenExpression;
			try
			{
				rewrittenExpression = (Expression<Func<T, TResult>>)ReplaceDuck(expression);
			}
			catch (ArgumentException ex)
			{
				throw new ArgumentException(ex.Message, nameof(expression));
			}

			Mock.Verify(this.mock, rewrittenExpression, times ?? Times.AtLeastOnce(), failMessage);
		}

		public void VerifyGet<TProperty>(Expression<Func<TDuck, TProperty>> expression, Times? times = null, string failMessage = null)
		{
			Guard.NotNull(expression, nameof(expression));

			Expression<Func<T, TProperty>> rewrittenExpression;
			try
			{
				rewrittenExpression = (Expression<Func<T, TProperty>>)ReplaceDuck(expression);
			}
			catch (ArgumentException ex)
			{
				throw new ArgumentException(ex.Message, nameof(expression));
			}

			Mock.VerifyGet(this.mock, rewrittenExpression, times ?? Times.AtLeastOnce(), failMessage);
		}

		private static LambdaExpression ReplaceDuck(LambdaExpression expression)
		{
			Debug.Assert(expression.Parameters.Count == 1);

			var targetParameter = Expression.Parameter(typeof(T), expression.Parameters[0].Name);
			return Expression.Lambda(DuckReplacerInstance.Visit(expression.Body), targetParameter);
		}

		/// <summary>
		/// <see cref="ExpressionVisitor"/> used to replace occurrences of `TDuck.Member` sub-expressions with `T.Member`.
		/// </summary>
		private sealed class DuckReplacer : ExpressionVisitor
		{
			private Type duckType;
			private Type targetType;

			public DuckReplacer(Type duckType, Type targetType)
			{
				this.duckType = duckType;
				this.targetType = targetType;
			}

			protected override Expression VisitMethodCall(MethodCallExpression node)
			{
				if (node.Object is ParameterExpression left && left.Type == this.duckType)
				{
					var targetParameter = Expression.Parameter(this.targetType, left.Name);
					return Expression.Call(targetParameter, FindCorrespondingMethod(node.Method), node.Arguments);
				}
				else
				{
					return node;
				}
			}

			protected override Expression VisitMember(MemberExpression node)
			{
				if (node.Expression is ParameterExpression left && left.Type == this.duckType)
				{
					var targetParameter = Expression.Parameter(this.targetType, left.Name);
					return Expression.MakeMemberAccess(targetParameter, FindCorrespondingMember(node.Member));
				}
				else
				{
					return node;
				}
			}

			private MemberInfo FindCorrespondingMember(MemberInfo duckMember)
			{
				if (duckMember is MethodInfo duckMethod)
				{
					return FindCorrespondingMethod(duckMethod);
				}
				else if (duckMember is PropertyInfo duckProperty)
				{
					return FindCorrespondingProperty(duckProperty);
				}
				else
				{
					throw new NotSupportedException();
				}
			}

			private MethodInfo FindCorrespondingMethod(MethodInfo duckMethod)
			{
				var candidateTargetMethods =
				    this.targetType
				    .GetMethods(BindingFlags.NonPublic | BindingFlags.Instance)
				    .Where(ctm => IsCorrespondingMethod(duckMethod, ctm))
				    .ToArray();

				if (candidateTargetMethods.Length == 0)
				{
					throw new ArgumentException(string.Format(Resources.ProtectedMemberNotFound, this.targetType, duckMethod));
				}

				Debug.Assert(candidateTargetMethods.Length == 1);

				var targetMethod = candidateTargetMethods[0];

				if (targetMethod.IsGenericMethodDefinition)
				{
					var duckGenericArgs = duckMethod.GetGenericArguments();
					targetMethod = targetMethod.MakeGenericMethod(duckGenericArgs);
				}

				return targetMethod;
			}

			private PropertyInfo FindCorrespondingProperty(PropertyInfo duckProperty)
			{
				var candidateTargetProperties =
				    this.targetType
				    .GetProperties(BindingFlags.NonPublic | BindingFlags.Instance)
				    .Where(ctp => IsCorrespondingProperty(duckProperty, ctp))
				    .ToArray();

				if (candidateTargetProperties.Length == 0)
				{
					throw new ArgumentException(string.Format(Resources.ProtectedMemberNotFound, this.targetType, duckProperty));
				}

				Debug.Assert(candidateTargetProperties.Length == 1);

				return candidateTargetProperties[0];
			}

			private static bool IsCorrespondingMethod(MethodInfo duckMethod, MethodInfo candidateTargetMethod)
			{
				if (candidateTargetMethod.Name != duckMethod.Name)
				{
					return false;
				}

				if (candidateTargetMethod.IsGenericMethod != duckMethod.IsGenericMethod)
				{
					return false;
				}

				if (candidateTargetMethod.IsGenericMethodDefinition)
				{
					// when both methods are generic, then the candidate method should be an open generic method
					// while the duck-type method should be a closed one. in this case, we close the former
					// over the same generic type arguments that the latter uses.

					Debug.Assert(!duckMethod.IsGenericMethodDefinition);

					var candidateGenericArgs = candidateTargetMethod.GetGenericArguments();
					var duckGenericArgs = duckMethod.GetGenericArguments();

					if (candidateGenericArgs.Length != duckGenericArgs.Length)
					{
						return false;
					}

					// this could perhaps go wrong due to generic type parameter constraints; if it does
					// go wrong, then obviously the duck-type method doesn't correspond to the candidate.
					try
					{
						candidateTargetMethod = candidateTargetMethod.MakeGenericMethod(duckGenericArgs);
					}
					catch
					{
						return false;
					}
				}

				var duckParameters = duckMethod.GetParameters();
				var candidateParameters = candidateTargetMethod.GetParameters();

				if (candidateParameters.Length != duckParameters.Length)
				{
					return false;
				}

				for (int i = 0, n = candidateParameters.Length; i < n; ++i)
				{
					if (candidateParameters[i].ParameterType != duckParameters[i].ParameterType)
					{
						return false;
					}
				}

				return true;
			}

			private static bool IsCorrespondingProperty(PropertyInfo duckProperty, PropertyInfo candidateTargetProperty)
			{
				return candidateTargetProperty.Name == duckProperty.Name
				    && candidateTargetProperty.PropertyType == duckProperty.PropertyType
				    && candidateTargetProperty.CanRead == duckProperty.CanRead
				    && candidateTargetProperty.CanWrite == duckProperty.CanWrite;

				// TODO: parameter lists should be compared, too, to properly support indexers.
			}
		}
    }
}
