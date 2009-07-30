using System;
using System.Collections.Generic;

namespace Moq.Visualizer
{
	[Serializable]
	public class MockViewModel
	{
		internal MockViewModel(Type mockedType, params ContainerViewModel[] containers)
		{
			this.MockedType = mockedType.GetFullName();
			this.Containers = containers;
		}

		public bool IsExpanded { get; set; }

		public IEnumerable<ContainerViewModel> Containers { get; private set; }

		public string MockedType { get; private set; }
	}
}