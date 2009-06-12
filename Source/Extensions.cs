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
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Reflection;
using Moq.Proxy;

namespace Moq
{
	internal static class Extensions
	{
		static readonly FieldInfo remoteStackTraceString = typeof(Exception).GetField("_remoteStackTraceString",
										 BindingFlags.Instance | BindingFlags.NonPublic);

		public static TAttribute GetCustomAttribute<TAttribute>(this ICustomAttributeProvider source, bool inherit)
			where TAttribute : Attribute
		{
			object[] attrs = source.GetCustomAttributes(typeof(TAttribute), inherit);

			if (attrs.Length == 0)
			{
				return default(TAttribute);
			}
			else
			{
				return (TAttribute)attrs[0];
			}
		}

		[SuppressMessage("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily", Justification="The linq expression is way more readable this way.")]
		public static string Format(this ICallContext invocation)
		{
			// Special-case for getters && setters
			if (invocation.Method.IsSpecialName)
			{
				if (invocation.Method.Name.StartsWith("get_", StringComparison.Ordinal))
					return
						invocation.Method.DeclaringType.Name + "." +
						invocation.Method.Name.Substring(4);
				else if (invocation.Method.Name.StartsWith("set_", StringComparison.Ordinal))
					return
						invocation.Method.DeclaringType.Name + "." +
						invocation.Method.Name.Substring(4) + " = " +
						(from x in invocation.Arguments
						 select x == null ?
								"null" :
								x is string ?
									"\"" + (string)x + "\"" :
									x.ToString()).First();
			}

			return
				invocation.Method.DeclaringType.Name + "." +
				invocation.Method.Name + "(" +
				String.Join(", ",
					(from x in invocation.Arguments
					 select x == null ?
						"null" :
						x is string ?
							"\"" + (string)x + "\"" :
							x.ToString())
					.ToArray()
				) + ")";
		}

		public static object InvokePreserveStack(this Delegate del, params object[] args)
		{
			try
			{
				return del.DynamicInvoke(args);
			}
			catch (TargetInvocationException ex)
			{
#if SILVERLIGHT
			/* The test listed below fails when we call the setValue in silverlight...
			 * 
			 * 
			 * Assembly:
			 *    Moq.Tests.Silverlight.MSTest
			 * Namespace:
			 *    Moq.Tests
			 * Test class:
			 *    MockedEventsFixture
			 * Test method:
			 *    ShouldPreserveStackTraceWhenRaisingEvent
			 * at System.Reflection.RtFieldInfo.PerformVisibilityCheckOnField(IntPtr field, Object target, IntPtr declaringType, FieldAttributes attr, UInt32 invocationFlags) at System.Reflection.RtFieldInfo.InternalSetValue(Object obj, Object value, BindingFlags invokeAttr, Binder binder, CultureInfo culture, Boolean doVisibilityCheck, Boolean doCheckConsistency) at System.Reflection.RtFieldInfo.InternalSetValue(Object obj, Object value, BindingFlags invokeAttr, Binder binder, CultureInfo culture, Boolean doVisibilityCheck) at System.Reflection.RtFieldInfo.SetValue(Object obj, Object value, BindingFlags invokeAttr, Binder binder, CultureInfo culture) at System.Reflection.FieldInfo.SetValue(Object obj, Object value) at Moq.Extensions.InvokePreserveStack(Delegate del, Object[] args) at Moq.MockedEvent.DoRaise(EventArgs args) at Moq.MockedEvent`1.Raise(TEventArgs args) at Moq.Tests.MockedEventsFixture.<>c__DisplayClass16.<ShouldPreserveStackTraceWhenRaisingEvent>b__14() at Xunit.Record.Exception(ThrowsDelegate code)
			 */
#else
				remoteStackTraceString.SetValue(ex.InnerException, ex.InnerException.StackTrace);
				ex.InnerException.SetStackTrace(ex.InnerException.StackTrace);
#endif
				throw ex.InnerException;
			}
		}

		public static void SetStackTrace(this Exception exception, string stackTrace)
		{
			remoteStackTraceString.SetValue(exception, stackTrace);
		}

		public static bool IsMockeable(this Type typeToMock)
		{
			return 
				typeToMock.IsInterface ||
				typeToMock.IsAbstract ||
				!typeToMock.IsSealed;
		}

		public static bool CanOverrideGet(this PropertyInfo property)
		{
			if (property.CanRead)
			{
				var getter = property.GetGetMethod();
				return getter != null && getter.IsVirtual && !getter.IsFinal;
			}

			return false;
		}

		public static bool CanOverrideSet(this PropertyInfo property)
		{
			if (property.CanWrite)
			{
				var setter = property.GetSetMethod();
				return setter != null && setter.IsVirtual && !setter.IsFinal;
			}

			return false;
		}

		public static EventInfo GetEvent<TMock>(this Action<TMock> eventExpression, TMock mock)
			where TMock : class
		{
			Guard.ArgumentNotNull(eventExpression, "eventExpression");

			MethodBase addRemove = null;
			using (var context = new FluentMockContext())
			{
				eventExpression(mock);

				if (context.LastInvocation == null)
					throw new ArgumentException("Expression is not an event attach or detach, or the event is declared in a class but not marked virtual.");

				addRemove = context.LastInvocation.Invocation.Method;
			}

			var ev = addRemove.DeclaringType.GetEvent(
					addRemove.Name.Replace("add_", "").Replace("remove_", ""));

			if (ev == null)
			{
				throw new ArgumentException(String.Format(CultureInfo.CurrentCulture,
					"Could not locate event for attach or detach method {0}.", addRemove.ToString()));
			}

			return ev;
		}
	}
}
