using Xunit;
using System;
using Moq.Tests.Matchers.AnyAttribute1.SomeOtherLib;

/// <summary>
/// This example allows using `_` for any type including interfaces, but it has the downside of requiring some
/// extra user maintennance when refactoring the interfaces. This is overcome in <see cref="Moq.Tests.Matchers.AnyAttribute2.AnyValueAttributeTests2"/>.
/// Tests for https://github.com/moq/moq4/issues/1199
/// </summary>

namespace Moq.Tests.Matchers.AnyAttribute1.SomeOtherLib
{
	/// <summary>
	/// Helper class probably provided by user/3rd party lib and not Moq
	/// </summary>
	public class AnyValueHelper<T> where T : AnyValueBase, new()
	{
		[AnyValue] //new Attribute that allows this to work
		public static T _ => default;
		[AnyValue]
		public static T any => default;
	}

	/// <summary>
	/// Helper class probably provided by user/3rd party lib and not Moq
	/// </summary>
	public class AnyValueBase
	{
		public static implicit operator int(AnyValueBase _) => default;
		public static implicit operator byte(AnyValueBase _) => default;
		//TODO all the built-in value types
	}
}


namespace Moq.Tests.Matchers.AnyAttribute1
{
	using static AnyValueHelper<AdamsAnyHelper>;   // note using static to simplify syntax

	/// <summary>
	/// Helper class probably provided by user (Adam) for any types not handled by <see cref="AnyValueBase"/>.
	/// </summary>
	public class AdamsAnyHelper : AnyValueBase, ICar
	{
		#region boilerplate

		int ICar.Calc(int a, int b, int c, int d)
		{
			throw new NotImplementedException();
		}

		int ICar.DoSomething(ICar a, GearId b, int c)
		{
			throw new NotImplementedException();
		}

		int ICar.Echo(int a)
		{
			throw new NotImplementedException();
		}

		int ICar.Race(ICar a)
		{
			throw new NotImplementedException();
		}

		public static implicit operator GearId(AdamsAnyHelper _) => default;

		#endregion
	}

	/// <summary>
	/// Example enum
	/// </summary>
	public enum GearId
	{
		Reverse,
		Neutral,
		Gear1,
	}

	/// <summary>
	/// Example interface
	/// </summary>
	public interface ICar
	{
		int Echo(int a);
		int Calc(int a, int b, int c, int d);
		int Race(ICar a);
		int DoSomething(ICar a, GearId b, int c);
	}

	public class AnyValueAttributeTests2
	{
		[Fact]
		public void Echo_1Primitive()
		{
			var mockCar = new Mock<ICar>();
			var car = mockCar.Object;

			mockCar.Setup(car => car.Echo(_)).Returns(0x68);
			Assert.Equal(0x68, car.Echo(default));
		}

		[Fact]
		public void Calc_4Primitives()
		{
			var mockCar = new Mock<ICar>();
			var car = mockCar.Object;

			mockCar.Setup(car => car.Calc(_, _, _, _)).Returns(123456);
			Assert.Equal(123456, car.Calc(default, default, default, default));
		}

		[Fact]
		public void Race_1Interface()
		{
			var mockCar = new Mock<ICar>();
			var car = mockCar.Object;

			mockCar.Setup(car => car.Race(_)).Returns(0x68);
			Assert.Equal(0x68, car.Race(default));
		}

		[Fact]
		public void DoSomething_MixedTypes()
		{
			var mockCar = new Mock<ICar>();
			var car = mockCar.Object;

			mockCar.Setup(car => car.DoSomething(_, _, _)).Returns(0x68);
			Assert.Equal(0x68, car.DoSomething(default, default, default));
		}
	}
}
