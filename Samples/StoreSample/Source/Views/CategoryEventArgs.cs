using System;

namespace Store
{
	public class CategoryEventArgs : EventArgs
	{
		public CategoryEventArgs(Category category)
		{
			this.Category = category;
		}

		public Category Category { get; private set; }
	}
}
