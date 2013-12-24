using System.Reflection;

namespace Moq.Proxy.Factory
{
	internal static class MemberBindingFlags
	{
		public const BindingFlags InstanceMembers = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
	}
}