using System;
using System.Reflection;

namespace Moq
{
	/// <summary>
	/// A <see cref="IDefaultValueProvider"/> that returns an empty default value 
	/// for non-mockeable types, and mocks for all other types (interfaces and 
	/// non-sealed classes) that can be mocked.
	/// </summary>
	internal class MockDefaultValueProvider : EmptyDefaultValueProvider
	{
		Mock owner;

		public MockDefaultValueProvider(Mock owner)
		{
			this.owner = owner;
		}

		public override object ProvideDefault(MethodInfo member, object[] arguments)
		{
			var value = base.ProvideDefault(member, arguments);
			Mock mock = null;

			if (value == null &&
				member.ReturnType.IsMockeable() &&
				!owner.InnerMocks.TryGetValue(member, out mock))
			{
				var mockType = typeof(Mock<>).MakeGenericType(member.ReturnType);
				mock = (Mock)Activator.CreateInstance(mockType, owner.Behavior);
				mock.DefaultValue = owner.DefaultValue;
				
				owner.InnerMocks.Add(member, mock);
			}

			if (mock != null)
				return mock.Object;
			else
				return value;
		}
	}
}
