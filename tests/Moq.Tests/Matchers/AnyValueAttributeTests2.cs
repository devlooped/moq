using Xunit;
using Moq.Tests.Matchers.AnyValueAttribute2.SomeOtherLib;

/// <summary>
/// This example usage is OK, but not ideal. See <see cref="Moq.Tests.Matchers.AnyValueAttribute3.AutoIsAny"/> example instead.
/// 
/// This example allows using `Any` for any non-interface type and `AnyI` otherwise.
/// This avoids extra user maintennance when the interfaces are refactored.
/// Tests for https://github.com/moq/moq4/issues/1199
/// </summary>

namespace Moq.Tests.Matchers.AnyValueAttribute2.SomeOtherLib
{
	/// <summary>
	/// Helper class probably provided by user/3rd party lib and not Moq
	/// </summary>
	public class AnyValueHelper<T, IType> where T : AnyValueBase
	{
		/// <summary>
		/// Any non-interface type. Class or primitive type.
		/// </summary>
		[AnyValue] //new Attribute that allows this to work
		public static T Any => default;

		/// <summary>
		/// Any Interface
		/// </summary>
		[AnyValue]
		public static IType AnyI => default;
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


namespace Moq.Tests.Matchers.AnyValueAttribute2
{
	using static AnyValueHelper<AdamsAnyHelper, IAdamsAnyInterfaceHelper>;   // note using static to simplify syntax

	/// <summary>
	/// Helper class provided by user (Adam) for any types not handled by <see cref="AnyValueBase"/>.
	/// </summary>
	public class AdamsAnyHelper : AnyValueBase
	{
		public static implicit operator GearId(AdamsAnyHelper _) => default;
	}

	/// <summary>
	/// Helper interface provided by user (Adam) for any interfaces
	/// </summary>
	public interface IAdamsAnyInterfaceHelper : ICar
	{
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

			mockCar.Setup(car => car.Echo(Any)).Returns(0x68);
			Assert.Equal(0x68, car.Echo(default));
		}

		[Fact]
		public void Calc_4Primitives()
		{
			var mockCar = new Mock<ICar>();
			var car = mockCar.Object;

			mockCar.Setup(car => car.Calc(Any, Any, Any, Any)).Returns(123456);
			Assert.Equal(123456, car.Calc(default, default, default, default));
		}

		[Fact]
		public void Race_1Interface()
		{
			var mockCar = new Mock<ICar>();
			var car = mockCar.Object;

			mockCar.Setup(car => car.Race(AnyI)).Returns(0x68);
			Assert.Equal(0x68, car.Race(default));
		}

		[Fact]
		public void DoSomething_MixedTypes()
		{
			var mockCar = new Mock<ICar>();
			var car = mockCar.Object;

			mockCar.Setup(car => car.DoSomething(AnyI, Any, Any)).Returns(0x68);
			Assert.Equal(0x68, car.DoSomething(default, default, default));
		}
	}
}
