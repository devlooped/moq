using System;
using Xunit;

namespace Moq.Tests
{
	public class ConditionalSetupFixture
	{
		[Fact]
		public void ChooseAffirmativeExpectationOnMethod()
		{
			var mock = new Mock<IFoo>();

			var when = true;

			mock.When(() => when).Setup(x => x.Foo()).Returns("bar");
			mock.When(() => !when).Setup(x => x.Foo()).Returns("no bar");

			Assert.Equal("bar", mock.Object.Foo());

			when = false;
			Assert.Equal("no bar", mock.Object.Foo());

			when = true;
			Assert.Equal("bar", mock.Object.Foo());
		}

		[Fact]
		public void ChooseAffirmativeExpetationOnVoidMethod()
		{
			var mock = new Mock<IFoo>();

			var when = true;
			var positive = false;
			var negative = false;

			mock.When(() => when).Setup(x => x.Bar()).Callback(() => positive = true);
			mock.When(() => !when).Setup(x => x.Bar()).Callback(() => negative = true);

			mock.Object.Bar();

			Assert.True(positive);
			Assert.False(negative);

			when = false;
			positive = false;
			mock.Object.Bar();

			Assert.False(positive);
			Assert.True(negative);

			when = true;
			negative = false;
			mock.Object.Bar();

			Assert.True(positive);
			Assert.False(negative);
		}

		[Fact]
		public void ChooseAffirmativeExpectationOnPropertyGetter()
		{
			var mock = new Mock<IFoo>();

			var first = true;

			mock.When(() => first).SetupGet(x => x.Value).Returns("bar");
			mock.When(() => !first).SetupGet(x => x.Value).Returns("no bar");

			Assert.Equal("bar", mock.Object.Value);
			first = false;
			Assert.Equal("no bar", mock.Object.Value);
			first = true;
			Assert.Equal("bar", mock.Object.Value);
		}

		[Fact]
		public void ChooseAffirmativeExpetationOnPropertySetter()
		{
			var mock = new Mock<IFoo>();

			var when = true;
			var positive = false;
			var negative = false;

			mock.When(() => when).SetupSet(x => x.Value = "foo").Callback(() => positive = true);
			mock.When(() => !when).SetupSet(x => x.Value = "foo").Callback(() => negative = true);

			mock.Object.Value = "foo";

			Assert.True(positive);
			Assert.False(negative);

			when = false;
			positive = false;
			mock.Object.Value = "foo";

			Assert.False(positive);
			Assert.True(negative);

			when = true;
			negative = false;
			mock.Object.Value = "foo";

			Assert.True(positive);
			Assert.False(negative);
		}

		[Fact]
		public void ChooseAffirmativeExpetationOnTypedPropertySetter()
		{
			var mock = new Mock<IFoo>();

			var when = true;
			var positive = false;
			var negative = false;

			mock.When(() => when).SetupSet<string>(x => x.Value = "foo").Callback(s => positive = true);
			mock.When(() => !when).SetupSet<string>(x => x.Value = "foo").Callback(s => negative = true);

			mock.Object.Value = "foo";

			Assert.True(positive);
			Assert.False(negative);

			when = false;
			positive = false;
			mock.Object.Value = "foo";

			Assert.False(positive);
			Assert.True(negative);

			when = true;
			negative = false;
			mock.Object.Value = "foo";

			Assert.True(positive);
			Assert.False(negative);
		}

		[Fact]
		public void ChooseAffirmativeExpectationOnPropertyIndexer()
		{
			var mock = new Mock<IFoo>();

			var first = true;

			mock.When(() => first).Setup(x => x[0]).Returns("bar");
			mock.When(() => !first).Setup(x => x[0]).Returns("no bar");

			Assert.Equal("bar", mock.Object[0]);
			first = false;
			Assert.Equal("no bar", mock.Object[0]);
			first = true;
			Assert.Equal("bar", mock.Object[0]);
		}

		public interface IFoo
		{
			string Value { get; set; }
			string this[int index] { get; }
			void Bar();
			string Foo();
		}
	}
}