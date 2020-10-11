// Copyright (c) 2007, Clarius Consulting, Manas Technology Solutions, InSTEDD, and Contributors.
// All rights reserved. Licensed under the BSD 3-Clause License; see License.txt.

using System;
using System.Collections.Generic;
using System.Reflection;

using Moq.Tests.FSharpTypes;

using Xunit;

namespace Moq.Tests
{
	public class FSharpCompatibilityFixture
	{
		public static IEnumerable<object[]> AbstractFSharpEvents
		{
			get
			{
				yield return new object[] { typeof(HasAbstractActionEvent).GetEvent(nameof(HasAbstractActionEvent.Event)) };
				yield return new object[] { typeof(HasAbstractEventHandlerEvent).GetEvent(nameof(HasAbstractEventHandlerEvent.Event)) };
				yield return new object[] { typeof(IHasActionEvent).GetEvent(nameof(IHasActionEvent.Event)) };
				yield return new object[] { typeof(IHasEventHandlerEvent).GetEvent(nameof(IHasEventHandlerEvent.Event)) };
			}
		}

		public static IEnumerable<object[]> NonAbstractFSharpEvents
		{
			get
			{
				yield return new object[] { typeof(HasActionEvent).GetEvent(nameof(HasActionEvent.Event)) };
			}
		}

		[Theory(Skip = "See https://github.com/Microsoft/visualfsharp/issues/5834.")]
		[MemberData(nameof(AbstractFSharpEvents))]
		public void Abstract_FSharp_event_has_accessors_marked_as_specialname(EventInfo @event)
		{
			Assert.All(@event.GetAccessors(), accessor => Assert.True(accessor.IsAbstract));
			Assert.All(@event.GetAccessors(), accessor => Assert.True(accessor.IsSpecialName, "Accessor not marked as `specialname`."));
		}

		[Theory]
		[MemberData(nameof(NonAbstractFSharpEvents))]
		public void Non_abstract_FSharp_event_has_accessors_marked_as_specialname(EventInfo @event)
		{
			Assert.All(@event.GetAccessors(), accessor => Assert.False(accessor.IsAbstract));
			Assert.All(@event.GetAccessors(), accessor => Assert.True(accessor.IsSpecialName, "Accessor not marked as `specialname`."));
		}

		[Fact]
		public void Can_subscribe_to_and_raise_abstract_FSharp_event()
		{
			var mock = new Mock<IHasEventHandlerEvent>();
			var eventRaiseCount = 0;
			EventHandler handler = (sender, e) => ++eventRaiseCount;

			mock.Object.Event += handler;
			mock.Raise(x => x.Event += null, EventArgs.Empty);

			Assert.Equal(1, eventRaiseCount);
		}

		[Fact]
		public void Can_unsubscribe_from_FSharp_event()
		{
			var mock = new Mock<IHasEventHandlerEvent>();
			var eventRaiseCount = 0;
			EventHandler handler = (sender, e) => ++eventRaiseCount;

			mock.Object.Event += handler;
			mock.Raise(x => x.Event += null, EventArgs.Empty);

			mock.Object.Event -= handler;
			mock.Raise(x => x.Event += null, EventArgs.Empty);

			Assert.Equal(1, eventRaiseCount);
		}

		public static IEnumerable<object[]> FSharpIndexers
		{
			get
			{
				yield return new object[] { typeof(IHasIndexer).GetProperty("Item") };
				yield return new object[] { typeof(HasAbstractIndexer).GetProperty("Item") };
				yield return new object[] { typeof(HasIndexer).GetProperty("Item") };
			}
		}

		[Theory]
		[MemberData(nameof(FSharpIndexers))]
		public void All_FSharp_indexers_have_accessors_marked_as_specialname(PropertyInfo indexer)
		{
			Assert.All(indexer.GetAccessors(), accessor => Assert.True(accessor.IsSpecialName, "Accessor not marked as `specialname`."));
		}

		[Theory]
		[MemberData(nameof(FSharpIndexers))]
		public void All_FSharp_indexer_getters_are_recognized_as_such(PropertyInfo indexer)
		{
			var getter = indexer.GetGetMethod(true);
			Assert.True(getter.IsGetAccessor());
		}

		[Theory]
		[MemberData(nameof(FSharpIndexers))]
		public void All_FSharp_indexer_setters_are_recognized_as_such(PropertyInfo indexer)
		{
			var setter = indexer.GetSetMethod(true);
			Assert.True(setter.IsSetAccessor());
		}

		public static IEnumerable<object[]> FSharpProperties
		{
			get
			{
				yield return new object[] { typeof(IHasProperty).GetProperty(nameof(IHasProperty.Property)) };
				yield return new object[] { typeof(HasAbstractProperty).GetProperty(nameof(HasAbstractProperty.Property)) };
				yield return new object[] { typeof(HasProperty).GetProperty(nameof(HasProperty.Property)) };
			}
		}

		[Theory]
		[MemberData(nameof(FSharpProperties))]
		public void All_FSharp_properties_have_accessors_marked_as_specialname(PropertyInfo property)
		{
			Assert.All(@property.GetAccessors(), accessor => Assert.True(accessor.IsSpecialName, "Accessor not marked as `specialname`."));
		}

		[Theory]
		[MemberData(nameof(FSharpProperties))]
		public void All_FSharp_property_getters_are_recognized_as_such(PropertyInfo property)
		{
			var getter = property.GetGetMethod(true);
			Assert.True(getter.IsGetAccessor());
		}

		[Theory]
		[MemberData(nameof(FSharpProperties))]
		public void All_FSharp_property_setters_are_recognized_as_such(PropertyInfo property)
		{
			var setter = property.GetSetMethod(true);
			Assert.True(setter.IsSetAccessor());
		}
	}
}
