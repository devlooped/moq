using System;
using System.Linq.Expressions;
using System.Reflection;

namespace Moq
{
	[AttributeUsage(
		AttributeTargets.Method |
		AttributeTargets.Field |
		AttributeTargets.Property, Inherited = true)]
	public class MatcherAttribute : Attribute
	{
		Type matcherType;

		public MatcherAttribute(Type matcherType)
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
