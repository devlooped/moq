using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace Moq
{
	internal class AsInterface<TInterface> : Mock<TInterface>
		where TInterface : class
	{
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields", Justification = "It's used right below!")]
		Mock owner;

		public AsInterface(Mock owner)
			: base(true)
		{
			this.owner = owner;
		}

		internal override Dictionary<MethodInfo, Mock> InnerMocks
		{
			get { return owner.InnerMocks; }
		}

		internal override Interceptor Interceptor
		{
			get { return owner.Interceptor; }
			set { owner.Interceptor = value; }
		}

		internal override Type MockedType { get { return typeof(TInterface); } }

		public override MockBehavior Behavior
		{
			get { return owner.Behavior; }
			internal set { owner.Behavior = value; }
		}

		public override bool CallBase
		{
			get { return owner.CallBase; }
			set { owner.CallBase = value; }
		}

		public override DefaultValue DefaultValue
		{
			get { return owner.DefaultValue; }
			set { owner.DefaultValue = value; }
		}

		public override TInterface Object
		{
			get { return owner.Object as TInterface; }
		}

		public override Mock<TNewInterface> As<TNewInterface>()
		{
			return owner.As<TNewInterface>();
		}

		public override MockedEvent<TEventArgs> CreateEventHandler<TEventArgs>()
		{
			return owner.CreateEventHandler<TEventArgs>();
		}

		public override MockedEvent<EventArgs> CreateEventHandler()
		{
			return owner.CreateEventHandler();
		}
	}
}
