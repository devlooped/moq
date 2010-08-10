using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace Moq.Tests.Linq
{
	public class UnsupportedQuerying
	{
		public class GivenAReadonlyNonVirtualProperty
		{
			[Fact]
			public void WhenQueryingDirect_ThenThrowsNotSupportedException()
			{
				Assert.Throws<NotSupportedException>(() => Mock.Of<Bar>(x => x.NonVirtualValue == "bar"));
			}

			[Fact]
			public void WhenQueryingOnFluent_ThenThrowsNotSupportedException()
			{
				Assert.Throws<NotSupportedException>(() => Mock.Of<Foo>(x => x.VirtualBar.NonVirtualValue == "bar"));
			}

			[Fact]
			public void WhenQueryingOnIntermediateFluentReadonly_ThenThrowsNotSupportedException()
			{
				Assert.Throws<NotSupportedException>(() => Mock.Of<Foo>(x => x.NonVirtualBar.VirtualValue == "bar"));
			}

			public class Bar
			{
				public string NonVirtualValue { get { return "foo"; } }
				public virtual string VirtualValue { get; set; }
			}

			public class Foo
			{
				public virtual Bar VirtualBar { get; set; }
				public Bar NonVirtualBar { get; set; }
			}
		}

		public class GivenAField
		{
			[Fact]
			public void WhenQueryingField_ThenThrowsNotSupportedException()
			{
				Assert.Throws<NotSupportedException>(() => Mock.Of<Bar>(x => x.FieldValue == "bar"));
			}

			[Fact]
			public void WhenQueryingOnFluent_ThenThrowsNotSupportedException()
			{
				Assert.Throws<NotSupportedException>(() => Mock.Of<Foo>(x => x.VirtualBar.FieldValue == "bar"));
			}

			[Fact]
			public void WhenIntermediateFluentReadonly_ThenThrowsNotSupportedException()
			{
				Assert.Throws<NotSupportedException>(() => Mock.Of<Foo>(x => x.Bar.VirtualValue == "bar"));
			}

			public class Bar
			{
				public string FieldValue = "foo";
				public virtual string VirtualValue { get; set; }
			}

			public class Foo
			{
				public Bar Bar = new Bar();

				public virtual Bar VirtualBar { get; set; }
			}
		}

		public class GivenANonVirtualMethod
		{
			[Fact]
			public void WhenQueryingDirect_ThenThrowsNotSupportedException()
			{
				Assert.Throws<NotSupportedException>(() => Mock.Of<Bar>(x => x.NonVirtual() == "foo"));
			}

			[Fact]
			public void WhenQueryingOnFluent_ThenThrowsNotSupportedException()
			{
				Assert.Throws<NotSupportedException>(() => Mock.Of<Foo>(x => x.Virtual().NonVirtual() == "foo"));
			}

			[Fact]
			public void WhenQueryingOnIntermediateFluentNonVirtual_ThenThrowsNotSupportedException()
			{
				Assert.Throws<NotSupportedException>(() => Mock.Of<Foo>(x => x.NonVirtual().Virtual() == "foo"));
			}

			public class Bar
			{
				public string NonVirtual()
				{
					return string.Empty;
				}

				public virtual string Virtual()
				{
					return string.Empty;
				}
			}

			public class Foo
			{
				public Bar NonVirtual()
				{
					return new Bar();
				}

				public virtual Bar Virtual()
				{
					return new Bar();
				}
			}
		}

		public class GivenAnInterface
		{
			[Fact]
			public void WhenQueryingSingle_ThenThrowsNotSupportedException()
			{
				Assert.Throws<NotSupportedException>(() => Mocks.Of<IFoo>().Single());
			}

			[Fact]
			public void WhenQueryingSingleOrDefault_ThenThrowsNotSupportedException()
			{
				Assert.Throws<NotSupportedException>(() => Mocks.Of<IFoo>().SingleOrDefault());
			}

			[Fact]
			public void WhenQueryingAll_ThenThrowsNotSupportedException()
			{
				Assert.Throws<NotSupportedException>(() => Mocks.Of<IFoo>().All(x => x.Value == "Foo"));
			}

			[Fact]
			public void WhenQueryingAny_ThenThrowsNotSupportedException()
			{
				Assert.Throws<NotSupportedException>(() => Mocks.Of<IFoo>().Any());
			}

			[Fact]
			public void WhenQueryingLast_ThenThrowsNotSupportedException()
			{
				Assert.Throws<NotSupportedException>(() => Mocks.Of<IFoo>().Last());
			}

			[Fact]
			public void WhenQueryingLastOrDefault_ThenThrowsNotSupportedException()
			{
				Assert.Throws<NotSupportedException>(() => Mocks.Of<IFoo>().LastOrDefault());
			}

			[Fact]
			public void WhenOperatorIsNotEqual_ThenThrowsNotSupportedException()
			{
				Assert.Throws<NotSupportedException>(() => Mock.Of<IFoo>(x => x.Value != "foo"));
			}

			[Fact]
			public void WhenOperatorIsGreaterThan_ThenThrowsNotSupportedException()
			{
				Assert.Throws<NotSupportedException>(() => Mock.Of<IFoo>(x => x.Count > 5));
			}

			[Fact]
			public void WhenOperatorIsGreaterThanOrEqual_ThenThrowsNotSupportedException()
			{
				Assert.Throws<NotSupportedException>(() => Mock.Of<IFoo>(x => x.Count >= 5));
			}

			[Fact]
			public void WhenOperatorIsLessThan_ThenThrowsNotSupportedException()
			{
				Assert.Throws<NotSupportedException>(() => Mock.Of<IFoo>(x => x.Count < 5));
			}

			[Fact]
			public void WhenOperatorIsLessThanOrEqual_ThenThrowsNotSupportedException()
			{
				Assert.Throws<NotSupportedException>(() => Mock.Of<IFoo>(x => x.Count <= 5));
			}

			[Fact]
			public void WhenCombiningWithOrRatherThanLogicalAnd_ThenThrowsNotSupportedException()
			{
				Assert.Throws<NotSupportedException>(() => Mock.Of<IFoo>(x => x.Count == 5 || x.Value == "foo"));
			}

			public interface IFoo
			{
				string Value { get; set; }
				int Count { get; set; }
			}
		}
	}
}