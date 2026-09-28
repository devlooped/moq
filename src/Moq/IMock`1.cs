namespace Moq
{
    /// <summary>
    /// Provides configuration information for a mock of <typeparamref name="T"/>, 
    /// and serves as the receiver of the generated setup extensions for its members.
    /// </summary>
    public interface IMock<out T> : IMock where T : class
    {
        /// <summary>
        /// The mocked object instance.
        /// </summary>
        new T Object { get; }
    }
}
