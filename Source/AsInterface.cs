//Copyright (c) 2007. Clarius Consulting, Manas Technology Solutions, InSTEDD
//http://code.google.com/p/moq/
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
using System.Collections.Generic;
using System.Reflection;

namespace Moq
{
	internal class AsInterface<TInterface> : Mock<TInterface>
		where TInterface : class
	{
		private Mock owner;

		public AsInterface(Mock owner)
			: base(true)
		{
			this.owner = owner;
		}

		internal override Dictionary<MethodInfo, Mock> InnerMocks
		{
			get { return this.owner.InnerMocks; }
		}

		internal override Interceptor Interceptor
		{
			get { return this.owner.Interceptor; }
			set { this.owner.Interceptor = value; }
		}

		internal override Type MockedType
		{
			get { return typeof(TInterface); }
		}

		public override MockBehavior Behavior
		{
			get { return this.owner.Behavior; }
			internal set { this.owner.Behavior = value; }
		}

		public override bool CallBase
		{
			get { return this.owner.CallBase; }
			set { this.owner.CallBase = value; }
		}

		public override DefaultValue DefaultValue
		{
			get { return this.owner.DefaultValue; }
			set { this.owner.DefaultValue = value; }
		}

		public override TInterface Object
		{
			get { return this.owner.Object as TInterface; }
		}

		public override Mock<TNewInterface> As<TNewInterface>()
		{
			return this.owner.As<TNewInterface>();
		}
	}
}