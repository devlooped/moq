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

internal static class Guard
{
	/// <summary>
	/// Checks an argument to ensure it isn't null.
	/// </summary>
	/// <param name="value">The argument value to check.</param>
	/// <param name="argumentName">The name of the argument.</param>
	public static void ArgumentNotNull(object value, string argumentName)
	{
		if (value == null)
			throw new ArgumentNullException(argumentName);
	}

	/// <summary>
	/// Checks a string argument to ensure it isn't null or empty.
	/// </summary>
	/// <param name="argumentValue">The argument value to check.</param>
	/// <param name="argumentName">The name of the argument.</param>
	public static void ArgumentNotNullOrEmptyString(string argumentValue, string argumentName)
	{
		ArgumentNotNull(argumentValue, argumentName);

		if (argumentValue.Length == 0)
			throw new ArgumentException(
				"Value cannot be null or an empty string.",
				argumentName);
	}

	public static void CanBeAssigned(Type typeToAssign, Type targetType)
	{
		if (!targetType.IsAssignableFrom(typeToAssign))
		{
			if (targetType.IsInterface)
				throw new ArgumentException(String.Format(
					"Type {0} does not implement required interface {1}",
					typeToAssign, targetType));
			else
				throw new ArgumentException(String.Format(
					"Type {0} does not from required type {1}",
					typeToAssign, targetType));
		}
	}

	public static void CanBeAssigned(Type typeToAssign, Type targetType, string argumentName)
	{
		if (!targetType.IsAssignableFrom(typeToAssign))
		{
			if (targetType.IsInterface)
				throw new ArgumentException(String.Format(
					"Type {0} does not implement required interface {1}",
					typeToAssign, targetType), argumentName);
			else
				throw new ArgumentException(String.Format(
					"Type {0} does not from required type {1}",
					typeToAssign, targetType), argumentName);
		}
	}
}
