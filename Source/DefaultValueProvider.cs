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
using System.Reflection;

namespace Moq
{
	/// <summary>
	/// <see cref="DefaultValueProvider"/> is the abstract base class for default value providers.
	/// These are responsible for producing e. g. return values when mock methods or properties get invoked unexpectedly.
	/// In other words, whenever there is no setup that would determine the return value for a particular invocation,
	/// Moq asks the mock's default value provider to produce a return value.
	/// </summary>
	public abstract class DefaultValueProvider
    {
		/// <summary>
		/// Initializes a new instance of the <see cref="DefaultValueProvider"/> class.
		/// </summary>
		protected DefaultValueProvider()
		{
		}

		/// <summary>
		/// Gets the <see cref="DefaultValue"/> enumeration value that corresponds to this default value provider.
		/// Must be overridden by Moq's internal providers that have their own corresponding <see cref="DefaultValue"/>.
		/// </summary>
		internal virtual DefaultValue Kind => DefaultValue.Custom;

		/// <summary>
		/// Produces a default value of the specified type.
		/// </summary>
		/// <param name="type">The <see cref="Type"/> of the requested default value.</param>
		/// <param name="mock">The <see cref="Mock"/> on which an unexpected invocation has occurred.</param>
		public object GetDefaultValue(Type type, Mock mock)
		{
			if (type == null)
			{
				throw new ArgumentNullException(nameof(type));
			}

			if (type == typeof(void))
			{
				throw new ArgumentOutOfRangeException(nameof(type));
			}

			if (mock == null)
			{
				throw new ArgumentNullException(nameof(mock));
			}

			return this.GetDefaultValueImpl(type, mock);
		}

		/// <summary>
		/// Produces a default argument value for the specified method parameter.
		/// </summary>
		/// <param name="parameter">The <see cref="ParameterInfo"/> describing the method parameter for which a default argument value should be produced.</param>
		/// <param name="mock">The <see cref="Mock"/> on which an unexpected invocation has occurred.</param>
		public object GetDefaultParameterValue(ParameterInfo parameter, Mock mock)
		{
			if (parameter == null)
			{
				throw new ArgumentNullException(nameof(parameter));
			}

			Debug.Assert(parameter.ParameterType != null);

			if (parameter.ParameterType == typeof(void))
			{
				throw new ArgumentOutOfRangeException(nameof(parameter));
			}

			if (mock == null)
			{
				throw new ArgumentNullException(nameof(mock));
			}

			return this.GetDefaultParameterValueImpl(parameter, mock);
		}

		/// <summary>
		/// Produces a default return value for the specified method.
		/// </summary>
		/// <param name="method">The <see cref="MethodInfo"/> describing the method for which a default return value should be produced.</param>
		/// <param name="mock">The <see cref="Mock"/> on which an unexpected invocation has occurred.</param>
		public object GetDefaultReturnValue(MethodInfo method, Mock mock)
		{
			if (method == null)
			{
				throw new ArgumentNullException(nameof(method));
			}

			Debug.Assert(method.ReturnType != null);

			if (method.ReturnType == typeof(void))
			{
				throw new ArgumentOutOfRangeException(nameof(method));
			}

			if (mock == null)
			{
				throw new ArgumentNullException(nameof(mock));
			}

			return this.GetDefaultReturnValueImpl(method, mock);
		}

		/// <summary>
		/// Produces a default value of the specified type.
		/// Must be overridden in derived classes.
		/// </summary>
		/// <param name="type">The <see cref="Type"/> of the requested default value.</param>
		/// <param name="mock">The <see cref="Mock"/> on which an unexpected invocation has occurred.</param>
		/// <remarks>
		/// This method gets called e. g. by <see cref="GetDefaultValue"/> after all parameters have been validated.
		/// All parameters may be assumed to have valid, non-<see langword="null"/>, non-<see langword="void"/> values.
		/// </remarks>
		protected internal abstract object GetDefaultValueImpl(Type type, Mock mock);

		/// <summary>
		/// Produces a default argument value for the specified method parameter.
		/// May be overridden in derived classes.
		/// By default, this method will delegate to <see cref="GetDefaultValueImpl"/>.
		/// </summary>
		/// <param name="parameter">The <see cref="ParameterInfo"/> describing the method parameter for which a default argument value should be produced.</param>
		/// <param name="mock">The <see cref="Mock"/> on which an unexpected invocation has occurred.</param>
		/// <remarks>
		/// This method gets called e. g. by <see cref="GetDefaultParameterValue"/> after all parameters have been validated.
		/// All parameters may be assumed to have valid, non-<see langword="null"/>, non-<see langword="void"/> values.
		/// </remarks>
		protected internal virtual object GetDefaultParameterValueImpl(ParameterInfo parameter, Mock mock)
		{
			Debug.Assert(parameter != null);
			Debug.Assert(parameter.ParameterType != typeof(void));
			Debug.Assert(mock != null);

			return this.GetDefaultValueImpl(parameter.ParameterType, mock);
		}

		/// <summary>
		/// Produces a default return value for the specified method.
		/// May be overridden in derived classes.
		/// By default, this method will delegate to <see cref="GetDefaultValueImpl"/>.
		/// </summary>
		/// <param name="method">The <see cref="MethodInfo"/> describing the method for which a default return value should be produced.</param>
		/// <param name="mock">The <see cref="Mock"/> on which an unexpected invocation has occurred.</param>
		/// <remarks>
		/// This method gets called e. g. by <see cref="GetDefaultReturnValue"/> after all parameters have been validated.
		/// All parameters may be assumed to have valid, non-<see langword="null"/>, non-<see langword="void"/> values.
		/// </remarks>
		protected internal virtual object GetDefaultReturnValueImpl(MethodInfo method, Mock mock)
		{
			Debug.Assert(method != null);
			Debug.Assert(method.ReturnType != typeof(void));
			Debug.Assert(mock != null);

			return this.GetDefaultValueImpl(method.ReturnType, mock);
		}
    }
}
