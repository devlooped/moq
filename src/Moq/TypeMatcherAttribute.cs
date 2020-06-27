// Copyright (c) 2007, Clarius Consulting, Manas Technology Solutions, InSTEDD, and Contributors.
// All rights reserved. Licensed under the BSD 3-Clause License; see License.txt.

using System;

namespace Moq
{
	/// <summary>
	///   Marks a type as a type matcher, optionally specifying another <see cref="ITypeMatcher"/> type that will perform the matching.
	///   <para>
	///     Type matchers preferably implement <see cref="ITypeMatcher"/> themselves. Use the parameterized form of this attribute
	///     where this is not possible, such as when the type matcher needs to be a <see langword="delegate"/> or
	///     <see langword="enum"/> type in order to satisfy generic type constraints of the method where it is used.
	///   </para>
	/// </summary>
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Delegate | AttributeTargets.Enum | AttributeTargets.Interface | AttributeTargets.Struct, AllowMultiple = false, Inherited = true)]
	public class TypeMatcherAttribute : Attribute
	{
		private readonly Type type;

		/// <summary>
		///   Initializes a new instance of the <see cref="TypeMatcherAttribute"/> class.
		///   <para>
		///     Use this constructor overload if the type on which this attribute is placed implements <see cref="ITypeMatcher"/> itself.
		///   </para>
		/// </summary>
		public TypeMatcherAttribute()
		{
			this.type = null;
		}

		/// <summary>
		///   Initializes a new instance of the <see cref="TypeMatcherAttribute"/> class.
		///   <para>
		///     Use this constructor overload if the type on which this attribute is placed does not implement <see cref="ITypeMatcher"/>.
		///     The specified type will instead provide the implementation of <see cref="ITypeMatcher"/>.
		///   </para>
		/// </summary>
		/// <param name="type">The <see cref="Type"/> of a type that implements <see cref="ITypeMatcher"/>.</param>
		public TypeMatcherAttribute(Type type)
		{
			if (type == null)
			{
				throw new ArgumentNullException(nameof(type));
			}

			this.type = type;
		}

		internal Type Type => this.type;
	}
}
