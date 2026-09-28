using Moq.Sdk;

namespace Moq
{
    /// <summary>
    /// A setup of a mock member invocation, which serves as the receiver 
    /// of the extension methods that configure its behavior.
    /// </summary>
    public interface ISetup : IFluentInterface
    {
        /// <summary>
        /// The low-level setup, for use by extenders.
        /// </summary>
        IMockSetup Sdk { get; }
    }

    /// <summary>
    /// A setup of a void member, whose shape is described by <typeparamref name="TDelegate"/>.
    /// </summary>
    /// <typeparam name="TDelegate">The delegate type matching the member signature.</typeparam>
    public interface ISetup<TDelegate> : ISetup
    {
    }

    /// <summary>
    /// A setup of a member returning <typeparamref name="TResult"/>, whose shape 
    /// is described by <typeparamref name="TDelegate"/>.
    /// </summary>
    /// <typeparam name="TDelegate">The delegate type matching the member signature.</typeparam>
    /// <typeparam name="TResult">The type of the member return value.</typeparam>
    public interface ISetup<TDelegate, TResult> : ISetup
    {
    }

    /// <summary>
    /// A setup of a member through a custom delegate matching its signature, typically 
    /// used to access and set ref/out arguments. See <see cref="Syntax.SetupRef{TDelegate}(TDelegate)"/>.
    /// </summary>
    /// <typeparam name="TDelegate">The delegate type matching the member signature.</typeparam>
    public interface ISetupRef<TDelegate> : ISetup<TDelegate>
    {
    }

    /// <summary>
    /// A setup of a property (or indexer) getter. The getter is set up on first use, 
    /// so merely accessing the property setup has no side effects.
    /// </summary>
    /// <typeparam name="TGetter">The delegate type matching the getter signature.</typeparam>
    /// <typeparam name="TValue">The type of the property.</typeparam>
    public interface IPropertySetup<TGetter, TValue> : ISetup<TGetter, TValue>
    {
        /// <summary>
        /// Sets up the property getter.
        /// </summary>
        ISetup<TGetter, TValue> Get();
    }

    /// <summary>
    /// A setup of a property (or indexer) getter and setter. The getter is set up on 
    /// first use, so merely accessing the property setup has no side effects.
    /// </summary>
    /// <typeparam name="TGetter">The delegate type matching the getter signature.</typeparam>
    /// <typeparam name="TSetter">The delegate type matching the setter signature.</typeparam>
    /// <typeparam name="TValue">The type of the property.</typeparam>
    public interface IPropertySetup<TGetter, TSetter, TValue> : IPropertySetup<TGetter, TValue>
    {
        /// <summary>
        /// Sets up the property setter for the given value, which can be an argument matcher.
        /// </summary>
        ISetup<TSetter> Set(TValue value);
    }
}
