// Copyright (c) 2007, Clarius Consulting, Manas Technology Solutions, InSTEDD, and Contributors.
// All rights reserved. Licensed under the BSD 3-Clause License; see License.txt.

using System;

namespace Moq.Language.Flow
{

    /* Unmerged change from project 'Moq(netstandard2.0)'
    Before:
        internal class SetterSetupPhrase<T, TProperty> : VoidSetupPhrase<T>, ISetupSetter<T, TProperty> where T : class
    After:
        class SetterSetupPhrase<T, TProperty> : VoidSetupPhrase<T>, ISetupSetter<T, TProperty> where T : class
    */

    /* Unmerged change from project 'Moq(netstandard2.1)'
    Before:
        internal class SetterSetupPhrase<T, TProperty> : VoidSetupPhrase<T>, ISetupSetter<T, TProperty> where T : class
    After:
        class SetterSetupPhrase<T, TProperty> : VoidSetupPhrase<T>, ISetupSetter<T, TProperty> where T : class
    */

    /* Unmerged change from project 'Moq(net6.0)'
    Before:
        internal class SetterSetupPhrase<T, TProperty> : VoidSetupPhrase<T>, ISetupSetter<T, TProperty> where T : class
    After:
        class SetterSetupPhrase<T, TProperty> : VoidSetupPhrase<T>, ISetupSetter<T, TProperty> where T : class
    */
    class SetterSetupPhrase<T, TProperty> : VoidSetupPhrase<T>, ISetupSetter<T, TProperty> where T : class
    {
        public SetterSetupPhrase(MethodCall setup) : base(setup)
        {
        }

        public ICallbackResult Callback(Action<TProperty> callback)
        {
            this.Setup.SetCallbackBehavior(callback);
            return this;
        }
    }
}
