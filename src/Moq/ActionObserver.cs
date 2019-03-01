// Copyright (c) 2007, Clarius Consulting, Manas Technology Solutions, InSTEDD.
// All rights reserved. Licensed under the BSD 3-Clause License; see License.txt.

using System;
using System.Globalization;
using System.Linq.Expressions;
using System.Reflection;

using Moq.Internals;
using Moq.Properties;

namespace Moq
{
	/// <summary>
	///   <see cref="ActionObserver"/> is a kind of <see cref="ExpressionReconstructor"/> that works by
	///   applying a <see cref="Action{T}"/> delegate to a light-weight proxy that records the invocation
	///   happening to it, and auto-generates the same kind of recording proxy for its return value.
	///   That way, a chain of invocation records is generated from which a LINQ expression tree can be
	///   reconstructed.
	/// </summary>
	internal sealed class ActionObserver : ExpressionReconstructor
	{
		public override Expression<Action<T>> ReconstructExpression<T>(Action<T> action)
		{
			// Create the root recording proxy:
			var root = (T)CreateProxy(typeof(T), out var rootRecorder);

			Exception error = null;
			try
			{
				// Execute the delegate. The root recorder will automatically "mock" return values
				// and so build a chain of recorders, whereby each one records a single invocation
				// in a method chain `o.X.Y.Z`:
				action.Invoke(root);
			}
			catch (Exception ex)
			{
				// Something went wrong. We don't return this error right away. We want to
				// rebuild the expression tree as far as possible for diagnostic purposes.
				error = ex;
			}

			// Start the expression tree with a parameter of type `T`:
			var actionParameters = action.GetMethodInfo().GetParameters();
			var actionParameterName = actionParameters[actionParameters.Length - 1].Name;
			var rootExpression = Expression.Parameter(typeof(T), actionParameterName);
			Expression body = rootExpression;

			// Then step through one recorded invocation at a time:
			for (var recorder = rootRecorder; recorder != null; recorder = recorder.Next)
			{
				var invocation = recorder.Invocation;
				if (invocation != null)
				{
					body = Expression.Call(body, invocation.Method, GetArgumentExpressions(invocation));
				}
				else
				{
					// A recorder was set up, but it recorded no invocation. This means
					// that the invocation could not be intercepted:
					throw new ArgumentException(
						string.Format(
							CultureInfo.CurrentCulture,
							Resources.UnsupportedExpressionWithHint,
							$"{actionParameterName} => {body.ToStringFixed()}...",
							Resources.NextMemberNonInterceptable));
				}
			}

			// Now we've either got no error and a completely reconstructed expression, or
			// we have an error and a partially reconstructed expression which we can use for
			// diagnostic purposes:
			if (error == null)
			{
				return Expression.Lambda<Action<T>>(Prettifier.Instance.Visit(body), rootExpression);
			}
			else
			{
				throw new ArgumentException(
					string.Format(
						CultureInfo.CurrentCulture,
						Resources.UnsupportedExpressionWithHint,
						$"{actionParameterName} => {body.ToStringFixed()}...",
						error.Message));
			}

			Expression[] GetArgumentExpressions(Invocation invocation)
			{
				var parameterTypes = invocation.Method.GetParameterTypes();
				var parameterCount = parameterTypes.Count;
				var expressions = new Expression[parameterCount];
				for (int i = 0; i < parameterCount; ++i)
				{
					expressions[i] = Expression.Constant(invocation.Arguments[i], parameterTypes[i]);

					// Add a `Convert` node (like the C# compiler does) in places where type coercion happens:
					if (invocation.Arguments[i] != null && invocation.Arguments[i].GetType() != parameterTypes[i])
					{
						expressions[i] = Expression.Convert(expressions[i], parameterTypes[i]);
					}
				}
				return expressions;
			}
		}

		// Creates a proxy (way more light-weight than a `Mock<T>`!) with an invocation `Recorder` attached to it.
		private static IProxy CreateProxy(Type type, out Recorder recorder)
		{
			recorder = new Recorder();
			return (IProxy)ProxyFactory.Instance.CreateProxy(type, recorder, Type.EmptyTypes, new object[0]);
		}

		// Records an invocation, mocks return values, and builds a chain to the return value's recorder.
		// This record represents the basis for reconstructing an expression tree.
		private sealed class Recorder : IInterceptor
		{
			private Invocation invocation;
			private IProxy returnValue;

			public Invocation Invocation => this.invocation;

			public Recorder Next => this.returnValue?.Interceptor as Recorder;

			public void Intercept(Invocation invocation)
			{
				var returnType = invocation.Method.ReturnType;

#if DEBUG
				// In theory, each recorder receives exactly one invocation.
				// We put the following guard here for debugging purposes, since your IDE's
				// "Watch" window might cause additional calls that normally shouldn't happen.
				if (this.invocation == null)
#endif
				{
					this.invocation = invocation;
					if (returnType == typeof(void))
					{
						this.returnValue = null;
					}
					else if (returnType.IsMockeable())
					{
						this.returnValue = CreateProxy(returnType, out _);
					}
					else
					{
						throw new NotSupportedException(Resources.LastMemberHasNonInterceptableReturnType);
					}
				}

				if (returnType != typeof(void))
				{
					invocation.Return(this.returnValue);
				}
				else
				{
					invocation.Return();
				}
			}
		}

		// Post-processes a reconstructed expression to...
		//  * convert property accessor method calls into property member accesses (`.get_X()` -> `.X`)
		private sealed class Prettifier : ExpressionVisitor
		{
			public static readonly Prettifier Instance = new Prettifier();

			private Prettifier()
			{
			}

			protected override Expression VisitMethodCall(MethodCallExpression node)
			{
				var obj = this.Visit(node.Object);

				if (node.Method.IsPropertyGetter())
				{
					var propertyName = node.Method.Name.Substring(4);
					var property = node.Method.DeclaringType.GetProperty(propertyName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
					return Expression.MakeMemberAccess(obj, property);
				}
				else if (obj != node.Object)
				{
					return Expression.Call(obj, node.Method, node.Arguments);
				}
				else
				{
					return node;
				}
			}
		}
	}
}
