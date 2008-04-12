using System;
using System.Reflection;

namespace Moq
{
	[AttributeUsage(
		AttributeTargets.Method /*|
		AttributeTargets.Property*/
		, Inherited = true)]
	internal class AdvancedMatcherAttribute : Attribute
	{
		Type matcherType;

		public AdvancedMatcherAttribute(Type matcherType)
		{
			Guard.ArgumentNotNull(matcherType, "matcherType");
			Guard.CanBeAssigned(matcherType, typeof(IMatcher), "matcherType");

			this.matcherType = matcherType;
		}

		public Type MatcherType { get { return matcherType; } }

		public virtual IMatcher CreateMatcher()
		{
			try
			{
				return (IMatcher)Activator.CreateInstance(matcherType);

			}
			catch (TargetInvocationException tie)
			{
				throw tie.InnerException;
			}
		}
	}
}
