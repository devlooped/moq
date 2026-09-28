using System;

namespace Moq
{
    /// <summary>
    /// Annotates a method that is a factory for mocks, so that a 
    /// compile-time or design-time generator can generate them ahead of time.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Constructor | AttributeTargets.Class, Inherited = true)]
    public class MockGeneratorAttribute : Attribute
    {
    }
}