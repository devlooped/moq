namespace Xunit.Sdk
{
    /// <summary>
    /// Exception thrown when an object is unexpectedly null.
    /// </summary>
    public class NotNullException : AssertException
    {
        /// <summary>
        /// Creates a new instance of the <see cref="NotNullException"/> class.
        /// </summary>
        public NotNullException()
            : base("Assert.NotNull() Failure") {}
    }
}