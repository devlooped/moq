using System;
using System.Collections.Generic;

namespace Moq.Visualizer
{
	[Serializable]
	public class SetupViewModel
	{
		internal SetupViewModel(
			string setupExpression,
			bool isVerifiable,
			bool isNever,
			params ContainerViewModel[] containers)
		{
			this.IsNever = isNever;
			this.IsVerifiable = isVerifiable;
			this.SetupExpression = setupExpression;
			this.Containers = containers;
		}

		public IEnumerable<ContainerViewModel> Containers { get; private set; }

		public bool IsExpanded
		{
			get { return false; }
		}

		public bool IsNever { get; private set; }

		public bool IsVerifiable { get; private set; }

		public string SetupExpression { get; private set; }
	}
}