// Copyright (c) 2007, Clarius Consulting, Manas Technology Solutions, InSTEDD, and Contributors.
// All rights reserved. Licensed under the BSD 3-Clause License; see License.txt.

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
	/// <para>
	/// <b>This feature has been deprecated in favor of the new 
	/// and simpler <see cref="Match{T}"/>.
	/// </b>
	/// </para>
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
	[EditorBrowsable(EditorBrowsableState.Never)]
	[Obsolete("This feature has been deprecated in favor of `Match.Create`.")]
	public sealed class MatcherAttribute : Attribute
	{
	}
}
