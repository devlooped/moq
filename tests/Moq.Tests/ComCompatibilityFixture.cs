// Copyright (c) 2007, Clarius Consulting, Manas Technology Solutions, InSTEDD.
// All rights reserved. Licensed under the BSD 3-Clause License; see License.txt.

#if FEATURE_COM

using Moq.Tests.ComTypes;

using Xunit;

namespace Moq.Tests
{
	public class ComCompatibilityFixture
	{
		[Fact]
		public void COM_interop_type_event_has_accessors_that_are_not_marked_as_specialname()
		{
			// If this test starts failing, we need to review our extension methods that check
			// whether a method is an event `add` or `remove` accessor. It might mean we can
			// do a quick test for `method.IsSpecialName` before performing more costly checks.

			var @event = typeof(IButtonEvents_Event).GetEvent(nameof(IButtonEvents_Event.Click));
			Assert.All(@event.GetAccessors(), accessor => Assert.False(accessor.IsSpecialName, "Accessor is marked as `specialname`."));
		}

		// The following two tests verify whether Moq can deal with events that are defined in
		// a COM interop type. This test is relevant because the Type Library Importer (`tlbimp`)
		// and the CLR runtime perform some magic to make COM connection points look like .NET
		// events. Notably, the generated .NET event accessor methods (`add_X` and `remove_X`)
		// are not marked with the `specialname` IL flag as is usual for .NET event accessors.

		[Fact]
		public void Can_subscribe_to_and_raise_COM_interop_type_event()
		{
			var mock = new Mock<IButtonEvents_Event>();
			var eventRaiseCount = 0;
			IButtonEvents_ClickEventHandler handler = () => ++eventRaiseCount;

			mock.Object.Click += handler;
			mock.Raise(x => x.Click += null);

			Assert.Equal(1, eventRaiseCount);
		}

		[Fact]
		public void Can_unsubscribe_from_COM_interop_type_event()
		{
			var mock = new Mock<IButtonEvents_Event>();
			var eventRaiseCount = 0;
			IButtonEvents_ClickEventHandler handler = () => ++eventRaiseCount;

			mock.Object.Click += handler;
			mock.Raise(x => x.Click += null);

			mock.Object.Click -= handler;
			mock.Raise(x => x.Click += null);

			Assert.Equal(1, eventRaiseCount);
		}

		[Fact]
		public void COM_interop_type_property_has_accessors_that_are_marked_as_specialname()
		{
			var property = typeof(IHasProperty).GetProperty(nameof(IHasProperty.Property));
			Assert.All(property.GetAccessors(), accessor => Assert.True(accessor.IsSpecialName, "Accessor is not marked as `specialname`."));
		}

		[Fact]
		public void COM_interop_type_property_getter_is_recognized_as_such()
		{
			var property = typeof(IHasProperty).GetProperty(nameof(IHasProperty.Property));
			var getter = property.GetGetMethod(true);
			Assert.True(getter.IsPropertyGetter());
		}

		[Fact]
		public void COM_interop_type_property_setter_is_recognized_as_such()
		{
			var property = typeof(IHasProperty).GetProperty(nameof(IHasProperty.Property));
			var setter = property.GetSetMethod(true);
			Assert.True(setter.IsPropertySetter());
		}

		[Fact]
		public void COM_interop_type_indexer_has_accessors_that_are_marked_as_specialname()
		{
			var indexer = typeof(IHasIndexer).GetProperty(nameof(IHasIndexer.Item));
			Assert.All(indexer.GetAccessors(), accessor => Assert.True(accessor.IsSpecialName, "Accessor is not marked as `specialname`."));
		}

		[Fact]
		public void COM_interop_type_indexer_getter_is_recognized_as_such()
		{
			var indexer = typeof(IHasIndexer).GetProperty(nameof(IHasIndexer.Item));
			var getter = indexer.GetGetMethod(true);
			Assert.True(getter.IsPropertyIndexerGetter());
		}

		[Fact]
		public void COM_interop_type_indexer_setter_is_recognized_as_such()
		{
			var indexer = typeof(IHasIndexer).GetProperty(nameof(IHasIndexer.Item));
			var setter = indexer.GetSetMethod(true);
			Assert.True(setter.IsPropertyIndexerSetter());
		}
	}
}

#endif
