using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Moq.Sdk
{
    /// <summary>
    /// Interface implemented by mocks that allows accessing 
    /// the <see cref="IMockRuntime"/> for introspecting 
    /// a mock instance.
    /// </summary>
    [CompilerGenerated]
    public interface IMocked
    {
        /// <summary>
        /// The runtime information for the current mock.
        /// </summary>
        [DebuggerDisplay("Invocations = {Runtime.Invocations.Count}", Name = nameof(IMocked) + "." + nameof(Runtime))]
        [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
        IMockRuntime Runtime { get; }
    }
}
