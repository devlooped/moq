using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Moq.Language.Flow;

namespace Moq.Language
{
   /// <summary>
   /// 
   /// </summary>
   /// <typeparam name="TMock"></typeparam>
    public partial interface ISingleThread<TMock> where TMock:class
    {
        /// <summary>
        /// Expect the method to be call in the same thread of Setup()
        /// </summary>
        ISetup<TMock> SingleThread();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TMock"></typeparam>
    /// <typeparam name="TResult"></typeparam>
    public partial interface ISingleThread<TMock, TResult> where TMock : class
    {
        /// <summary>
        /// Expect the method to be call in the same thread of Setup()
        /// </summary>
        /// <returns></returns>
        ISetup<TMock,TResult> SingleThread();
    }
    /// <summary>
    ///
    /// </summary>
    /// <typeparam name="TMock"></typeparam>
    /// <typeparam name="TProperty"></typeparam>
    public partial interface ISingleThreadGetter<TMock, TProperty> where TMock : class
    {
        /// <summary>
        ///  Expect the proeprty getter to be call in the same thread of Setup()
        /// </summary>
        /// <returns></returns>
        ISetupGetter<TMock, TProperty> SingleThread();
    }
    /// <summary>
    ///  
    /// </summary>
    /// <typeparam name="TMock"></typeparam>
    /// <typeparam name="TProperty"></typeparam>
    public partial interface ISingleThreadSetter<TMock, TProperty> where TMock : class
    {
        /// <summary>
        /// Expect the proeprty getter to be call in the same thread of Setup()
        /// </summary>
        /// <returns></returns>
        ISetupSetter<TMock, TProperty> SingleThread();
    }

}
