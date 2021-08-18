// Copyright (c) 2007, Clarius Consulting, Manas Technology Solutions, InSTEDD, and Contributors.
// All rights reserved. Licensed under the BSD 3-Clause License; see License.txt.

using System;

namespace Moq
{
	/// <summary>
	/// A class member with this attribute will be treated automatically like <see cref="It.IsAny{TValue}"/>.
	/// Supports https://github.com/moq/moq4/issues/1199
	/// </summary>
	[AttributeUsage(AttributeTargets.Property, Inherited = true)]   //TODO consider and test `| AttributeTargets.Method | AttributeTargets.Field`
	public class AnyValueAttribute : Attribute
	{
		
	}
}
