using Xunit;
using System;

/// <summary>
/// Tests for https://github.com/moq/moq4/issues/1199
/// </summary>

namespace Moq.Tests.Matchers.Wildcard
{
	using static AutoIsAny;   // note using static to simplify syntax

	/// <summary>
	/// Helper class provided by user
	/// </summary>
	public abstract class AutoIsAny
	{
		public static AnyValue _
		{
			get
			{
				It.IsAny<object>();
				return new AnyValue();
			}
		}
	}

	/// <summary>
	/// Helper class provided by user. Interfaces implemented via IDE auto explicit interface implementation
	/// or Roslyn analyzer/code fix.
	/// </summary>
	public class AnyValue : ISomeService
	{
		int ISomeService.Calc(int a, int b, int c, int d)
		{
			throw new NotImplementedException();
		}

		int ISomeService.DoSomething(ISomeService a, GearId b, int c)
		{
			throw new NotImplementedException();
		}

		int ISomeService.Echo(int a)
		{
			throw new NotImplementedException();
		}

		int ISomeService.UseAnimal(Animal a)
		{
			throw new NotImplementedException();
		}

		int ISomeService.UseDolphin(Dolphin a)
		{
			throw new NotImplementedException();
		}

		int ISomeService.UseInterface(ISomeService a)
		{
			throw new NotImplementedException();
		}

		public static implicit operator int(AnyValue _) => default;
		public static implicit operator byte(AnyValue _) => default;
		public static implicit operator GearId(AnyValue _) => default;
		public static implicit operator Animal(AnyValue _) => default;
		public static implicit operator Dolphin(AnyValue _) => default;
	}


	public class Tests
	{
		Mock<ISomeService> mock;
		ISomeService obj;

		public Tests()
		{
			mock = new Mock<ISomeService>();
			obj = mock.Object;
		}

		[Fact]
		public void Echo_1Primitive()
		{
			mock.Setup(obj => obj.Echo(_)).Returns(777);
			Assert.Equal(777, obj.Echo(1));
		}

		[Fact]
		public void Calc_4Primitives()
		{
			mock.Setup(obj => obj.Calc(_, _, _, _)).Returns(999);
			Assert.Equal(999, obj.Calc(1, 2, 3, 4));
		}

		[Fact]
		public void UseInterface()
		{
			mock.Setup(obj => obj.UseInterface(_)).Returns(555);
			Assert.Equal(555, obj.UseInterface(null));

			var realService = new SomeService();
			Assert.Equal(555, obj.UseInterface(realService));
		}

		[Fact]
		public void DoSomething_MixedTypes()
		{
			mock.Setup(obj => obj.DoSomething(_, _, _)).Returns(444);
			Assert.Equal(444, obj.DoSomething(null, GearId.Neutral, 1));
		}

		[Fact]
		public void UseAnimal()
		{
			mock.Setup(obj => obj.UseAnimal(_)).Returns(777);
			Assert.Equal(777, obj.UseAnimal(new Animal()));
			Assert.Equal(777, obj.UseAnimal(new Dolphin()));
		}

		[Fact]
		public void UseDolphin()
		{
			mock.Setup(obj => obj.UseDolphin(_)).Returns(888);
			Assert.Equal(888, obj.UseDolphin(new Dolphin()));
		}
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
	public interface ISomeService
	{
		int Echo(int a);
		int Calc(int a, int b, int c, int d);
		int UseInterface(ISomeService a);
		int DoSomething(ISomeService a, GearId b, int c);
		int UseAnimal(Animal a);
		int UseDolphin(Dolphin a);
	}


	/// <summary>
	/// just a class that implements interface
	/// </summary>
	public class SomeService : ISomeService
	{
		public int Calc(int a, int b, int c, int d)
		{
			throw new NotImplementedException();
		}

		public int DoSomething(ISomeService a, GearId b, int c)
		{
			throw new NotImplementedException();
		}

		public int Echo(int a)
		{
			throw new NotImplementedException();
		}

		public int UseAnimal(Animal a)
		{
			throw new NotImplementedException();
		}

		public int UseDolphin(Dolphin a)
		{
			throw new NotImplementedException();
		}

		public int UseInterface(ISomeService a)
		{
			throw new NotImplementedException();
		}
	}


	public class Animal
	{

	}


	public class Dolphin : Animal
	{

	}
}
