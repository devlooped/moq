//Copyright (c) 2007, Moq Team 
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

//    * Neither the name of the Moq Team nor the 
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
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Moq.Matchers
{
	/// <summary>
	/// Matcher to treat static functions as matchers.
	/// 
	/// mock.Expect(x => x.StringMethod(A.MagicString()));
	/// 
	/// pbulic static class A 
	/// {
	///     [Matcher]
	///     public static string MagicString() { return null; }
	///     public static bool MagicString(string arg)
	///     {
	///         return arg == "magic";
	///     }
	/// }
	/// 
	/// Will success if: mock.Object.StringMethod("magic");
	/// and fail with any other call.
	/// </summary>
	internal class MatcherAttributeMatcher : IMatcher
	{
		MethodInfo validatorMethod;
		Expression matcherExpression;

		public void Initialize(Expression matcherExpression)
		{
			this.validatorMethod = ResolveValidatorMethod(matcherExpression);
			this.matcherExpression = matcherExpression;
		}

		private MethodInfo ResolveValidatorMethod(Expression expression)
		{
			MethodCallExpression call = (MethodCallExpression) expression;
			var expectedParametersTypes = new[] { call.Method.ReturnType }.Concat(call.Method.GetParameters().Select(p => p.ParameterType)).ToArray();

			MethodInfo method = null;

			if (call.Method.IsGenericMethod)
			{
				// This is the "hard" way in .NET 3.5 as GetMethod does not support
				// passing generic type arguments for the query.
				var candidates = from m in call.Method.DeclaringType.GetMethods(
									BindingFlags.Public |
									BindingFlags.NonPublic |
									BindingFlags.Static | 
									BindingFlags.Instance)
								 where
									m.Name == call.Method.Name &&
									m.IsGenericMethodDefinition &&
									m.GetGenericArguments().Length ==
									call.Method.GetGenericMethodDefinition().GetGenericArguments().Length && 
									AreEqual(expectedParametersTypes, 
											m.MakeGenericMethod(call.Method.GetGenericArguments())
											 .GetParameters()
											 .Select(p => p.ParameterType))
								 select m.MakeGenericMethod(call.Method.GetGenericArguments());

				method = candidates.FirstOrDefault();
			}
			else
			{
				method = call.Method.DeclaringType.GetMethod(call.Method.Name, expectedParametersTypes);
			}

			// throw if validatorMethod doesn't exists			
			if (method == null)
			{
				throw new MissingMethodException(string.Format(
					"public {0}bool {1}({2}) in class {3}.",
					call.Method.IsStatic ? "static " : string.Empty,
					call.Method.Name, 
					string.Join(", ", expectedParametersTypes.Select(x => x.ToString()).ToArray()),
					call.Method.DeclaringType.ToString()));
			}
			return method;
		}

		public bool Matches(object value)
		{
			// use matcher Expression to get extra arguments
			MethodCallExpression call = (MethodCallExpression)matcherExpression;
			var extraArgs = call.Arguments.Select(ae => ((ConstantExpression)ae.PartialEval()).Value);
			var args = new[] { value }.Concat(extraArgs).ToArray();
			// for static and non-static method
			var instance = call.Object == null ? null : (call.Object.PartialEval() as ConstantExpression).Value;
			return (bool) validatorMethod.Invoke( instance, args );
		}

		// TODO: move to EnumerableExtensions in NetFx?
		private bool AreEqual<T>(IEnumerable<T> first, IEnumerable<T> second)
		{
			var f = first.ToList();
			var s = second.ToList();

			if (f.Count != s.Count) return false;

			for (int i = 0; i < f.Count; i++)
			{
				if (!f[i].Equals(s[i])) return false;
			}
			
			return true;
		}

	}
}
