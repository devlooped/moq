// Copyright (c) 2007, Clarius Consulting, Manas Technology Solutions, InSTEDD, and Contributors.
// All rights reserved. Licensed under the BSD 3-Clause License; see License.txt.

using System;

using Xunit;

namespace Moq.Tests
{
	public class EventHandlerTypesMustMatchFixture
	{
		[Fact]
		public void CLI_requires_event_handlers_to_have_the_exact_same_type()
		{
			var mouse = new Mouse();
			var result = 2;

			mouse.LeftButtonClicked += new Action<LeftButton>(_ => result += 3);
			mouse.LeftButtonClicked += new Action<LeftButton>(_ => result *= 4);
			mouse.RaiseLeftButtonClicked(new LeftButton());

			Assert.Equal(20, result);
		}

		[Fact]
		public void CLI_throws_if_event_handlers_do_not_have_the_exact_same_type()
		{
			var mouse = new Mouse();
			mouse.LeftButtonClicked += new Action<Button>(delegate { });
			Assert.Throws<ArgumentException>(() => mouse.LeftButtonClicked += new Action<LeftButton>(delegate { }));
		}

		[Fact]
		public void Moq_requires_event_handlers_to_have_the_exact_same_type()
		{
			var mouseMock = new Mock<Mouse>();
			var mouse = mouseMock.Object;
			var result = 2;

			mouse.LeftButtonClicked += new Action<Button>(_ => result += 3);
			mouse.LeftButtonClicked += new Action<Button>(_ => result *= 4);
			mouseMock.Raise(m => m.LeftButtonClicked += null, new LeftButton());

			Assert.Equal(20, result);
		}

		[Fact]
		public void Moq_throws_if_event_handlers_do_not_have_the_exact_same_type()
		{
			var mouseMock = new Mock<Mouse>();
			var mouse = mouseMock.Object;

			mouse.LeftButtonClicked += new Action<Button>(delegate { });
			Assert.Throws<ArgumentException>(() => mouse.LeftButtonClicked += new Action<LeftButton>(delegate { }));
		}

		public class Mouse
		{
			public virtual event Action<LeftButton> LeftButtonClicked;

			public void RaiseLeftButtonClicked(LeftButton button)
			{
				this.LeftButtonClicked?.Invoke(button);
			}
		}

		public abstract class Button { }

		public class LeftButton : Button { }
	}
}
