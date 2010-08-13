using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ClassLibrary1;

namespace ClassLibrary2
{
	public interface IBar : IFoo
	{
		DateTimeOffset When { get; set; }
	}

	public class Bar : Foo
	{
		public DateTimeOffset When { get; set; }
	}
}
