// Copyright (c) 2007, Clarius Consulting, Manas Technology Solutions, InSTEDD, and Contributors.
// All rights reserved. Licensed under the BSD 3-Clause License; see License.txt.

using System;
using System.Collections.Generic;

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

		internal override List<Type> AdditionalInterfaces => this.owner.AdditionalInterfaces;

		internal override Dictionary<Type, object> ConfiguredDefaultValues => this.owner.ConfiguredDefaultValues;

		internal override object[] ConstructorArguments => this.owner.ConstructorArguments;

		internal override InvocationCollection MutableInvocations => this.owner.MutableInvocations;

		internal override void AddInvocationListener(Action<Invocation> listener)
		{
			owner.AddInvocationListener(listener);
		}

		internal override bool IsObjectInitialized => this.owner.IsObjectInitialized;

		internal override Type MockedType => this.owner.MockedType;

		public override MockBehavior Behavior => this.owner.Behavior;

		public override bool CallBase
		{
			get { return this.owner.CallBase; }
			set { this.owner.CallBase = value; }
		}

		public override DefaultValueProvider DefaultValueProvider
		{
			get => this.owner.DefaultValueProvider;
			set => this.owner.DefaultValueProvider = value;
		}

		internal override DefaultValueProvider AutoSetupPropertiesDefaultValueProvider
		{
			get => this.owner.AutoSetupPropertiesDefaultValueProvider;
			set => this.owner.AutoSetupPropertiesDefaultValueProvider = value;
		}

		internal override EventHandlerCollection EventHandlers => this.owner.EventHandlers;

		internal override Type[] InheritedInterfaces => this.owner.InheritedInterfaces;

		public override TInterface Object
		{
			get { return this.owner.Object as TInterface; }
		}

		internal override SetupCollection MutableSetups => this.owner.MutableSetups;

		public override Switches Switches
		{
			get => this.owner.Switches;
			set => this.owner.Switches = value;
		}

		public override Mock<TNewInterface> As<TNewInterface>()
		{
			return this.owner.As<TNewInterface>();
		}

		protected override object OnGetObject()
		{
			return this.owner.Object;
		}

		public override string ToString()
		{
			return this.owner.ToString();
		}
	}
}
