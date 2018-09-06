// Copyright (c) 2007, Clarius Consulting, Manas Technology Solutions, InSTEDD.
// All rights reserved. Licensed under the BSD 3-Clause License; see License.txt.

using System;
using System.Linq.Expressions;
using System.Reflection;

namespace Moq.Matchers
{
	internal class RefMatcher : IMatcher
	{
		private Func<object, bool> equals;

		public RefMatcher(object reference)
		{
			if (reference != null && reference.GetType().GetTypeInfo().IsValueType)
			{
				equals = value => object.Equals(reference, value);
			}
			else
			{
				equals = value => object.ReferenceEquals(reference, value);
			}
		}

		public bool Matches(object value)
		{
			return equals(value);
		}
	}
}
