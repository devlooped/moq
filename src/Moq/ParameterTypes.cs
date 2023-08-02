// Copyright (c) 2007, Clarius Consulting, Manas Technology Solutions, InSTEDD, and Contributors.
// All rights reserved. Licensed under the BSD 3-Clause License; see License.txt.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace Moq
{

    /* Unmerged change from project 'Moq(netstandard2.0)'
    Before:
        internal readonly struct ParameterTypes : IReadOnlyList<Type>
    After:
        readonly struct ParameterTypes : IReadOnlyList<Type>
    */

    /* Unmerged change from project 'Moq(netstandard2.1)'
    Before:
        internal readonly struct ParameterTypes : IReadOnlyList<Type>
    After:
        readonly struct ParameterTypes : IReadOnlyList<Type>
    */

    /* Unmerged change from project 'Moq(net6.0)'
    Before:
        internal readonly struct ParameterTypes : IReadOnlyList<Type>
    After:
        readonly struct ParameterTypes : IReadOnlyList<Type>
    */
    /// <summary>
    ///   Allocation-free adapter type for treating a `ParameterInfo[]` array like a `Type[]` array.
    /// </summary>
    readonly struct ParameterTypes : IReadOnlyList<Type>

    /* Unmerged change from project 'Moq(netstandard2.0)'
    Before:
            private readonly ParameterInfo[] parameters;
    After:
            readonly ParameterInfo[] parameters;
    */

    /* Unmerged change from project 'Moq(netstandard2.1)'
    Before:
            private readonly ParameterInfo[] parameters;
    After:
            readonly ParameterInfo[] parameters;
    */

    /* Unmerged change from project 'Moq(net6.0)'
    Before:
            private readonly ParameterInfo[] parameters;
    After:
            readonly ParameterInfo[] parameters;
    */
    {
        readonly ParameterInfo[] parameters;

        public ParameterTypes(ParameterInfo[] parameters)
        {
            this.parameters = parameters;
        }

        public Type this[int index] => this.parameters[index].ParameterType;

        public int Count => this.parameters.Length;

        public IEnumerator<Type> GetEnumerator()
        {
            for (int i = 0, n = this.Count; i < n; ++i)
            {
                yield return this[i];
            }
        }

        IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();
    }
}
