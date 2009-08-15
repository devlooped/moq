using System;
using System.Collections.Generic;

namespace Moq.Visualizer
{
	[Serializable]
	public abstract class ContainerViewModel
	{
		protected ContainerViewModel(string name)
		{
			this.Name = name;
		}

		public string Name { get; private set; }
	}

	[Serializable]
	public class ContainerViewModel<T> : ContainerViewModel
	{
		public ContainerViewModel(string name, IEnumerable<T> children)
			: base(name)
		{
			this.Children = children;
		}

		public bool IsExpanded { get; set; }

		public IEnumerable<T> Children { get; private set; }
	}
}