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

using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Text;

namespace Moq
{
	internal abstract class Invocation : IReadOnlyInvocation
	{
		private object[] arguments;
		private MethodInfo method;
		private bool verified;

		/// <summary>
		/// Initializes a new instance of the <see cref="Invocation"/> class.
		/// </summary>
		/// <param name="method">The method being invoked.</param>
		/// <param name="arguments">The arguments with which the specified <paramref name="method"/> is being invoked.</param>
		protected Invocation(MethodInfo method, params object[] arguments)
		{
			Debug.Assert(arguments != null);
			Debug.Assert(method != null);

			this.arguments = arguments;
			this.method = method;
		}

		/// <summary>
		/// Gets the method of the invocation.
		/// </summary>
		public MethodInfo Method => this.method;

		/// <summary>
		/// Gets the arguments of the invocation.
		/// </summary>
		/// <remarks>
		/// Arguments may be modified. Derived classes must ensure that by-reference parameters are written back
		/// when the invocation is ended by a call to any of the three <c>Returns</c> methods.
		/// </remarks>
		public object[] Arguments => this.arguments;

		IReadOnlyList<object> IReadOnlyInvocation.Arguments => this.arguments;

		internal bool Verified => this.verified;

		/// <summary>
		/// Ends the invocation as if a <see langword="return"/> statement occurred.
		/// </summary>
		/// <remarks>
		/// Implementations may assume that this method is only called for a <see langword="void"/> method,
		/// and that no more calls to any of the three <c>Return</c> methods will be made.
		/// <para>
		/// Implementations must ensure that any by-reference parameters are written back from <see cref="Arguments"/>.
		/// </para>
		/// </remarks>
		public abstract void Return();

		/// <summary>
		/// Ends the invocation as if a tail call to the base method were made.
		/// </summary>
		/// <remarks>
		/// Implementations may assume that this method is only called for a method having a callable (non-<see langword="abstract"/>) base method,
		/// and that no more calls to any of the three <c>Return</c> methods will be made.
		/// <para>
		/// Implementations must ensure that any by-reference parameters are written back from <see cref="Arguments"/>.
		/// </para>
		/// </remarks>
		public abstract void ReturnBase();

		/// <summary>
		/// Ends the invocation as if a <see langword="return"/> statement with the specified return value occurred.
		/// </summary>
		/// <remarks>
		/// Implementations may assume that this method is only called for a non-<see langword="void"/> method,
		/// and that no more calls to any of the three <c>Return</c> methods will be made.
		/// <para>
		/// Implementations must ensure that any by-reference parameters are written back from <see cref="Arguments"/>.
		/// </para>
		/// </remarks>
		public abstract void Return(object value);

		internal void MarkAsVerified() => this.verified = true;

		/// <inheritdoc/>
		public override string ToString()
		{
			var method = this.Method;

			var builder = new StringBuilder();
			builder.Append(method.DeclaringType.Name);
			builder.Append('.');

			if (method.IsPropertyGetter())
			{
				builder.Append(method.Name, 4, method.Name.Length - 4);
			}
			else if (method.IsPropertySetter())
			{
				builder.Append(method.Name, 4, method.Name.Length - 4);
				builder.Append(" = ");
				builder.AppendValueOf(this.Arguments[0]);
			}
			else
			{
				builder.AppendNameOf(method, includeGenericArgumentList: true);

				// append argument list:
				builder.Append('(');
				for (int i = 0, n = this.Arguments.Length; i < n; ++i)
				{
					if (i > 0)
					{
						builder.Append(", ");
					}
					builder.AppendValueOf(this.Arguments[i]);
				}

				builder.Append(')');
			}

			return builder.ToString();
		}
	}
}
