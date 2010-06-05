namespace Xunit.Sdk
{
    /// <summary>
    /// Exception thrown when two values are unexpectedly not equal.
    /// </summary>
    public class EqualException : AssertActualExpectedException
    {
        /// <summary>
        /// Creates a new instance of the <see cref="EqualException"/> class.
        /// </summary>
        /// <param name="expected">The expected object value</param>
        /// <param name="actual">The actual object value</param>
        public EqualException(object expected,
                              object actual)
            : base(expected, actual, "Assert.Equal() Failure") {}
    }
}