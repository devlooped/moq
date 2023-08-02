// Copyright (c) 2007, Clarius Consulting, Manas Technology Solutions, InSTEDD, and Contributors.
// All rights reserved. Licensed under the BSD 3-Clause License; see License.txt.

using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Moq
{

    /* Unmerged change from project 'Moq(netstandard2.0)'
    Before:
        internal readonly struct ImmutablePopOnlyStack<T>
    After:
        readonly struct ImmutablePopOnlyStack<T>
    */

    /* Unmerged change from project 'Moq(netstandard2.1)'
    Before:
        internal readonly struct ImmutablePopOnlyStack<T>
    After:
        readonly struct ImmutablePopOnlyStack<T>
    */

    /* Unmerged change from project 'Moq(net6.0)'
    Before:
        internal readonly struct ImmutablePopOnlyStack<T>
    After:
        readonly struct ImmutablePopOnlyStack<T>
    */
    readonly struct ImmutablePopOnlyStack<T>

    /* Unmerged change from project 'Moq(netstandard2.0)'
    Before:
            private readonly T[] items;
            private readonly int index;
    After:
            readonly T[] items;
            readonly int index;
    */

    /* Unmerged change from project 'Moq(netstandard2.1)'
    Before:
            private readonly T[] items;
            private readonly int index;
    After:
            readonly T[] items;
            readonly int index;
    */

    /* Unmerged change from project 'Moq(net6.0)'
    Before:
            private readonly T[] items;
            private readonly int index;
    After:
            readonly T[] items;
            readonly int index;
    */
    {
        readonly T[] items;
        readonly int index;

        public ImmutablePopOnlyStack(IEnumerable<T> items)
        {
            Debug.Assert(items != null);

            this.items = items.ToArray();
            this.index = 0;

            /* Unmerged change from project 'Moq(netstandard2.0)'
            Before:
                    private ImmutablePopOnlyStack(T[] items, int index)
            After:
                    ImmutablePopOnlyStack(T[] items, int index)
            */

            /* Unmerged change from project 'Moq(netstandard2.1)'
            Before:
                    private ImmutablePopOnlyStack(T[] items, int index)
            After:
                    ImmutablePopOnlyStack(T[] items, int index)
            */

            /* Unmerged change from project 'Moq(net6.0)'
            Before:
                    private ImmutablePopOnlyStack(T[] items, int index)
            After:
                    ImmutablePopOnlyStack(T[] items, int index)
            */
        }

        ImmutablePopOnlyStack(T[] items, int index)
        {
            Debug.Assert(items != null);
            Debug.Assert(0 <= index && index <= items.Length);

            this.items = items;
            this.index = index;
        }

        public bool Empty => this.index == this.items.Length;

        public T Pop(out ImmutablePopOnlyStack<T> stackBelowTop)
        {
            Debug.Assert(this.index < this.items.Length);

            var top = this.items[this.index];
            stackBelowTop = new ImmutablePopOnlyStack<T>(this.items, this.index + 1);
            return top;
        }
    }
}
