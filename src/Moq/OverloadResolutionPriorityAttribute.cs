namespace System.Runtime.CompilerServices
{
    /// <summary>
    /// Specifies the priority of a member in overload resolution. When unspecified, the default priority is 0.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Constructor | AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
    sealed class OverloadResolutionPriorityAttribute(int priority) : Attribute
    {
        /// <summary>
        /// The priority of the member.
        /// </summary>
        public int Priority => priority;
    }
}
