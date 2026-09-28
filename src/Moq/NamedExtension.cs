using System.ComponentModel;
using Moq.Sdk;

namespace Moq
{
    /// <summary>
    /// Extension for naming mocks.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static class NamedExtension
    {
        /// <summary>
        /// Names the mock.
        /// </summary>
        public static T Named<T>(this T target, string name) where T : class
        {
            MockRuntime.Get(target).State.Set("Name", name);
            return target;
        }
    }
}