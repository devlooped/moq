// Copyright (c) 2007, Clarius Consulting, Manas Technology Solutions, InSTEDD, and Contributors.
// All rights reserved. Licensed under the BSD 3-Clause License; see License.txt.

using System;
using System.ComponentModel;

namespace Moq.Language
{
    /// <summary>
    /// Defines the <c>Verifiable</c> verb.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public interface IVerifies : IFluentInterface
    {
        /// <summary>
        /// Marks the expectation as verifiable, meaning that a call 
        /// to <see cref="Mock.Verify()"/> will check if this particular 
        /// expectation was met.
        /// </summary>
        /// <example>
        /// The following example marks the expectation as verifiable:
        /// <code>
        /// mock.Setup(x => x.Execute("ping"))
        ///     .Returns(true)
        ///     .Verifiable();
        /// </code>
        /// </example>
        void Verifiable();

        /// <summary>
        /// Marks the expectation as verifiable, meaning that a call 
        /// to <see cref="Mock.Verify()"/> will check if this particular 
        /// expectation was met, and specifies a message for failures.
        /// </summary>
        /// <example>
        /// The following example marks the expectation as verifiable:
        /// <code>
        /// mock.Setup(x => x.Execute("ping"))
        ///     .Returns(true)
        ///     .Verifiable("Ping should be executed always!");
        /// </code>
        /// </example>
        void Verifiable(string failMessage);

        /// <summary>
        /// Marks the setup as verifiable and specifies the number of expected calls.
        /// <see cref="Mock.Verify()"/> and <see cref="Mock.VerifyAll()"/> will later verify
        /// that the setup was called the correct number of times.
        /// </summary>
        void Verifiable(Times times);

        /// <summary>
        /// Marks the setup as verifiable and specifies the number of expected calls.
        /// <see cref="Mock.Verify()"/> and <see cref="Mock.VerifyAll()"/> will later verify
        /// that the setup was called the correct number of times.
        /// </summary>
        void Verifiable(Func<Times> times);

        /// <summary>
        /// Marks the setup as verifiable and specifies the number of expected calls
        /// and a message for failures.
        /// <see cref="Mock.Verify()"/> and <see cref="Mock.VerifyAll()"/> will later verify
        /// that the setup was called the correct number of times.
        /// </summary>
        void Verifiable(Times times, string failMessage);

        /// <summary>
        /// Marks the setup as verifiable and specifies the number of expected calls
        /// and a message for failures.
        /// <see cref="Mock.Verify()"/> and <see cref="Mock.VerifyAll()"/> will later verify
        /// that the setup was called the correct number of times.
        /// </summary>
        void Verifiable(Func<Times> times, string failMessage);
    }
}
