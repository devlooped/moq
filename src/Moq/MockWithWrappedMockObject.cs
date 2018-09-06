// Copyright (c) 2007, Clarius Consulting, Manas Technology Solutions, InSTEDD.
// All rights reserved. Licensed under the BSD 3-Clause License; see License.txt.

using System.Threading.Tasks;

namespace Moq
{
	/// <summary>
	///   <para>
	///     Helper type required for <see cref="MockDefaultValueProvider"/> to efficiently support <see cref="Task{TResult}"/> and
	///     <see cref="ValueTask{TResult}"/>.
	///   </para>
	/// </summary>
	/// <remarks>
	///   <para>
	///     Mocking a type, then wrapping it in a task inside <see cref="MockDefaultValueProvider"/>, then unwrapping the task
	///     to get at the mock so it can be stored in <see cref="Mock.InnerMocks"/>, then wrapping it again each time a return
	///     value is created (e. g. in <see cref="Return"/>) is a lot of work.
	///   </para>
	///   <para>
	///     What we do instead is to hold the actual mock and the "wrapped" mocked object (used for return values) side by side
	///     to avoid the repeated wrapping and unwrapping.
	///   </para>
	/// </remarks>
	internal struct MockWithWrappedMockObject
	{
		public Mock Mock;
		public object WrappedMockObject;

		public MockWithWrappedMockObject(Mock mock, object wrappedMockObject)
		{
			this.Mock = mock;
			this.WrappedMockObject = wrappedMockObject;
		}
	}
}
