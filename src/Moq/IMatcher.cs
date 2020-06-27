// Copyright (c) 2007, Clarius Consulting, Manas Technology Solutions, InSTEDD, and Contributors.
// All rights reserved. Licensed under the BSD 3-Clause License; see License.txt.

using System;

namespace Moq
{
	internal interface IMatcher
	{
		bool Matches(object argument, Type parameterType);

		void SetupEvaluatedSuccessfully(object argument, Type parameterType);
	}
}
