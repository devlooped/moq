using System.ComponentModel;
using Moq.Sdk;

namespace Moq
{
    /// <summary>
    /// Extensions for calling base member virtual implementation.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static class CallBaseExtension
    {
        /// <summary>
        /// Specifies to call the base member virtual implementation when the setup is matched. 
        /// To call base members by default, use <see cref="IMock.CallBase"/> instead.
        /// </summary>
        public static TSetup CallBase<TSetup>(this TSetup setup) where TSetup : ISetup
        {
            setup.GetPipeline().Behaviors.Add(new AnonymousMockBehavior(
                (m, i, next) =>
                {
                    i.Context[nameof(IMock.CallBase)] = true;
                    return next().Invoke(m, i, next);
                },
                nameof(IMock.CallBase)));

            return setup;
        }
    }
}
