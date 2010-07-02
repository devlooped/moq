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
using System.ComponentModel;

namespace Moq
{
	/// <summary>
	/// Marks a method as a matcher, which allows complete replacement 
	/// of the built-in <see cref="It"/> class with your own argument 
	/// matching rules.
	/// </summary>
	/// <remarks>
	/// <b>This feature has been deprecated in favor of the new 
	/// and simpler <see cref="Match{T}"/>.
	/// </b>
	/// <para>
	/// The argument matching is used to determine whether a concrete 
	/// invocation in the mock matches a given setup. This 
	/// matching mechanism is fully extensible. 
	/// </para>
	/// <para>
	/// There are two parts of a matcher: the compiler matcher 
	/// and the runtime matcher.
	/// <list type="bullet">
	/// <item>
	/// <term>Compiler matcher</term>
	/// <description>Used to satisfy the compiler requirements for the 
	/// argument. Needs to be a method optionally receiving any arguments 
	/// you might need for the matching, but with a return type that 
	/// matches that of the argument. 
	/// <para>
	/// Let's say I want to match a lists of orders that contains 
	/// a particular one. I might create a compiler matcher like the following:
	/// </para>
	/// <code>
	/// public static class Orders
	/// {
	///   [Matcher]
	///   public static IEnumerable&lt;Order&gt; Contains(Order order)
	///   {
	///     return null;
	///   }
	/// }
	/// </code>
	/// Now we can invoke this static method instead of an argument in an 
	/// invocation:
	/// <code>
	/// var order = new Order { ... };
	/// var mock = new Mock&lt;IRepository&lt;Order&gt;&gt;();
	/// 
	/// mock.Setup(x =&gt; x.Save(Orders.Contains(order)))
	///     .Throws&lt;ArgumentException&gt;();
	/// </code>
	/// Note that the return value from the compiler matcher is irrelevant. 
	/// This method will never be called, and is just used to satisfy the 
	/// compiler and to signal Moq that this is not a method that we want 
	/// to be invoked at runtime.
	/// </description>
	/// </item>
	/// <item>
	/// <term>Runtime matcher</term>
	/// <description>
	/// The runtime matcher is the one that will actually perform evaluation 
	/// when the test is run, and is defined by convention to have the 
	/// same signature as the compiler matcher, but where the return 
	/// value is the first argument to the call, which contains the 
	/// object received by the actual invocation at runtime:
	/// <code>
	///   public static bool Contains(IEnumerable&lt;Order&gt; orders, Order order)
	///   {
	///     return orders.Contains(order);
	///   }
	/// </code>
	/// At runtime, the mocked method will be invoked with a specific 
	/// list of orders. This value will be passed to this runtime 
	/// matcher as the first argument, while the second argument is the 
	/// one specified in the setup (<c>x.Save(Orders.Contains(order))</c>).
	/// <para>
	/// The boolean returned determines whether the given argument has been 
	/// matched. If all arguments to the expected method are matched, then 
	/// the setup matches and is evaluated.
	/// </para>
	/// </description>
	/// </item>
	/// </list>
	/// </para>
	/// Using this extensible infrastructure, you can easily replace the entire 
	/// <see cref="It"/> set of matchers with your own. You can also avoid the 
	/// typical (and annoying) lengthy expressions that result when you have 
	/// multiple arguments that use generics.
	/// </remarks>
	/// <example>
	/// The following is the complete example explained above:
	/// <code>
	/// public static class Orders
	/// {
	///   [Matcher]
	///   public static IEnumerable&lt;Order&gt; Contains(Order order)
	///   {
	///     return null;
	///   }
	///   
	///   public static bool Contains(IEnumerable&lt;Order&gt; orders, Order order)
	///   {
	///     return orders.Contains(order);
	///   }
	/// }
	/// </code>
	/// And the concrete test using this matcher:
	/// <code>
	/// var order = new Order { ... };
	/// var mock = new Mock&lt;IRepository&lt;Order&gt;&gt;();
	/// 
	/// mock.Setup(x =&gt; x.Save(Orders.Contains(order)))
	///     .Throws&lt;ArgumentException&gt;();
	///     
	/// // use mock, invoke Save, and have the matcher filter.
	/// </code>
	/// </example>
	[AttributeUsage(AttributeTargets.Method, Inherited = true)]
	public sealed class MatcherAttribute : Attribute
	{
	}
}
