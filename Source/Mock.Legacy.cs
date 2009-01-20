using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using Moq.Language.Flow;
using System.Linq.Expressions;

namespace Moq
{
	// Keeps legacy members that hidden and are provided 
	// for backwards compatibility (so that existing projects 
	// still compile, but people don't see them).
	// When a reference to Moq.Legacy.dll is added to a projects, 
	public partial class Mock
	{
	}
}
