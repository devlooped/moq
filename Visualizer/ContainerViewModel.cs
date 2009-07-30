using System;
using System.Collections.Generic;

namespace Moq.Visualizer
{
	[Serializable]
	public abstract class ContainerViewModel
	{
		protected ContainerViewModel(string label)
		{
			this.Label = label;
		}

		public string Label { get; private set; }
	}

	[Serializable]
	public class ContainerViewModel<T> : ContainerViewModel
	{
		public ContainerViewModel(string label, IEnumerable<T> children)
			: base(label)
		{
			this.Children = children;
		}

		public bool IsExpanded { get; set; }

		public IEnumerable<T> Children { get; private set; }
	}
}