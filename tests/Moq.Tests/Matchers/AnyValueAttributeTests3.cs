using Xunit;
using System;

/// <summary>
/// This is my favorite potential solution so far. Nothing that looks too weird to users. Fairly understandable.
/// 
/// This example allows using `_` for any type including interfaces, but it has the downside of requiring some
/// extra user maintennance when an implemented interface is changed. IDE based auto explicit interface implementation
/// also makes this pretty easy.
/// 
/// This could be eased with a Roslyn analyzer/code fix.
/// 
/// Tests for https://github.com/moq/moq4/issues/1199
/// </summary>

namespace Moq.Tests.Matchers.AnyValueAttribute3
{
	using static AutoIsAny;   // note using static to simplify syntax

	/// <summary>
	/// Helper class provided by user
	/// </summary>
	public abstract class AutoIsAny
	{
		[AnyValue] //new Attribute that allows this to work
		public static AnyValue _ => default;
		[AnyValue]
		public static AnyValue Any => default;	//alternative syntax to `_`
	}

	/// <summary>
	/// Helper class provided by user. Interfaces implemented via IDE auto explicit interface implementation
	/// or Roslyn analyzer/code fix.
	/// </summary>
	public abstract class AnyValue : ICar
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

		public static implicit operator int(AnyValue _) => default;
		public static implicit operator byte(AnyValue _) => default;
		public static implicit operator GearId(AnyValue _) => default;
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


	public class Tests
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
