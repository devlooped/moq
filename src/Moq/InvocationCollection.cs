//Copyright (c) 2007. Clarius Consulting, Manas Technology Solutions, InSTEDD
//https://github.com/moq/moq4
//All rights reserved.

//Redistribution and use in source and binary forms, 
//with or without modification, are permitted provided 
//that the following conditions are met:

//    * Redistributions of source code must retain the 
//    above copyright notice, this list of conditions and 
//    the following disclaimer.

//    * Redistributions in binary form must reproduce 
//    the above copyright notice, this list of conditions 
//    and the following disclaimer in the documentation 
//    and/or other materials provided with the distribution.

//    * Neither the name of Clarius Consulting, Manas Technology Solutions or InSTEDD nor the 
//    names of its contributors may be used to endorse 
//    or promote products derived from this software 
//    without specific prior written permission.

//THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND 
//CONTRIBUTORS "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, 
//INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF 
//MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE 
//DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR 
//CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, 
//SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, 
//BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR 
//SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS 
//INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, 
//WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING 
//NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE 
//OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF 
//SUCH DAMAGE.

//[This is the BSD license, see
// http://www.opensource.org/licenses/bsd-license.php]

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Moq
{
	internal sealed class InvocationCollection : IInvocationList
	{
		private Invocation[] invocations;

		private int capacity = 0;
		private int count = 0;

		private readonly object invocationsLock = new object();

		public int Count
		{
			get
			{
				lock (this.invocationsLock)
				{
					return count;
				}
			}
		}

		public IInvocation this[int index]
		{
			get
			{
				lock (this.invocationsLock)
				{
					if (this.count <= index || index < 0)
					{
						throw new IndexOutOfRangeException();
					}

					return this.invocations[index];
				}
			}
		}

		public void Add(Invocation invocation)
		{
			lock (this.invocationsLock)
			{
				if (this.count == this.capacity)
				{
					var targetCapacity = this.capacity == 0 ? 4 : (this.capacity * 2);
					Array.Resize(ref this.invocations, targetCapacity);
					this.capacity = targetCapacity;
				}

				this.invocations[this.count] = invocation;
				this.count++;
			}
		}

		public void Clear()
		{
			lock (this.invocationsLock)
			{
				// Replace the collection so readers with a reference to the old collection aren't interrupted
				this.invocations = null;
				this.count = 0;
				this.capacity = 0;
			}
		}

		public Invocation[] ToArray()
		{
			lock (this.invocationsLock)
			{
				if (this.count == 0)
				{
					return new Invocation[0];
				}

				var result = new Invocation[this.count];

				Array.Copy(this.invocations, result, this.count);

				return result;
			}
		}

		public Invocation[] ToArray(Func<Invocation, bool> predicate)
		{
			lock (this.invocationsLock)
			{
				if (this.count == 0)
				{
					return new Invocation[0];
				}
				
				var result = new List<Invocation>(this.count);

				for (var i = 0; i < this.count; i++)
				{
					var invocation = this.invocations[i];
					if (predicate(invocation))
					{
						result.Add(invocation);
					}
				}

				return result.ToArray();
			}
		}

		public IEnumerator<IInvocation> GetEnumerator()
		{
			// Take local copies of collection and count so they are isolated from changes by other threads.
			Invocation[] collection;
			int count;

			lock (this.invocationsLock)
			{
				collection = this.invocations;
				count = this.count;
			}

			for (var i = 0; i < count; i++)
			{
				yield return collection[i];
			}
		}

		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
	}
}
