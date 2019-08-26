// Copyright (c) 2007, Clarius Consulting, Manas Technology Solutions, InSTEDD.
// All rights reserved. Licensed under the BSD 3-Clause License; see License.txt.

using System;
using System.Globalization;

using Moq.Properties;

using TypeNameFormatter;

namespace Moq
{
	/// <summary>
	///   Marks a type as a type matcher, specifying another <see cref="ITypeMatcher"/> type that will perform the matching.
	///   <para>
	///     Type matchers preferably implement <see cref="ITypeMatcher"/> directly. Use this attribute only in situations
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
		/// </summary>
		/// <param name="type">The <see cref="Type"/> of a type that implements <see cref="ITypeMatcher"/>.</param>
		public TypeMatcherAttribute(Type type)
		{
			if (type == null)
			{
				throw new ArgumentNullException(nameof(type));
			}

			if (typeof(ITypeMatcher).IsAssignableFrom(type) == false)
			{
				throw new ArgumentException(
					string.Format(
						CultureInfo.CurrentCulture,
						Resources.TypeNotImplementInterface,
						type.GetFormattedName(),
						typeof(ITypeMatcher).GetFormattedName()));
			}

			Guard.CanCreateInstance(type);

			this.type = type;
		}

		internal Type Type => this.type;
	}
}
