using System;
namespace Xunit
{
	[AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
	public class FactAttribute : Attribute
	{
		public FactAttribute()
		{
		}

		public string Name { get; protected set; }
		public string Skip { get; set; }
		public int Timeout { get; set; }
	}
}
