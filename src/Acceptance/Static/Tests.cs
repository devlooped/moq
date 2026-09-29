using System;
using Moq;
using Moq.Sdk;
using Sample;
using Xunit;
using static Moq.Syntax;

/// <summary>
/// Exercises the typed setups generated for C# 14+ against the packaged Moq, 
/// using the statically generated mocks.
/// </summary>
public class Tests
{
    [Fact]
    public void UsesStaticMocks()
    {
        var calc = new Mock<ICalculator>();

        Assert.StartsWith(MockNaming.DefaultRootNamespace, calc.Object.GetType().Namespace, StringComparison.Ordinal);
    }

    [Fact]
    public void StubProperties()
    {
        var calc = Mock.Of<ICalculator>();

        calc.Mode = CalculatorMode.Scientific;

        Assert.Equal(CalculatorMode.Scientific, calc.Mode);

        calc.Mode = CalculatorMode.Standard;

        Assert.Equal(CalculatorMode.Standard, calc.Mode);
    }

    [Fact]
    public void TypedSetups()
    {
        var calc = new Mock<ICalculator>();

        calc.Add(2, 3).Returns(5);
        calc.Add(Any<int>(), Any<int>(), Any<int>()).Returns((x, y, z) => x + y + z);
        calc.Mode.Returns(CalculatorMode.Scientific);
        calc.Item("memory").Returns(42);

        Assert.Equal(5, calc.Object.Add(2, 3));
        Assert.Equal(6, calc.Object.Add(1, 2, 3));
        Assert.Equal(CalculatorMode.Scientific, calc.Object.Mode);
        Assert.Equal(42, calc.Object["memory"]);
    }

    [Fact]
    public void TypedRefOutSetup()
    {
        var calc = new Mock<ICalculator>();
        int x = Any<int>(), y = Any<int>();

        calc.TryAdd(ref x, ref y, out _).Returns((ref a, ref b, out sum) =>
        {
            sum = a + b;
            return true;
        });

        x = 2;
        y = 3;
        Assert.True(calc.Object.TryAdd(ref x, ref y, out var z));
        Assert.Equal(5, z);
    }

    [Fact]
    public void RecursiveSetup()
    {
        var calc = new Mock<ICalculator, IDisposable>();

        Setup(() => calc.Object.Memory.Recall()).Returns(5);

        Assert.Equal(5, calc.Object.Memory.Recall());
        Assert.IsAssignableFrom<IDisposable>(calc.Object);
        Assert.Same(calc.Object, calc.As<IDisposable>().Object);
    }

    [Fact]
    public void RecursiveSetupBase()
    {
        var calc = new Mock<CalculatorBase, IDisposable>();

        Setup(() => calc.Object.Memory.Recall()).Returns(5);

        Assert.Equal(5, calc.Object.Memory.Recall());
        Assert.IsAssignableFrom<IDisposable>(calc.Object);
    }

    [Fact]
    public void GetsMockFromObject()
    {
        var calc = Mock.Of<ICalculator>();

        Mock.Get(calc).Add(1, 1).Returns(3);

        Assert.Equal(3, calc.Add(1, 1));
    }

    [Fact]
    public void DelegateOut()
    {
        var mock = new Mock<IParser>();

        SetupRef<TryParse>(mock.Object.TryParse)
            .Returns((string input, out DateTimeOffset date) => DateTimeOffset.TryParse(input, out date));

        var expected = DateTimeOffset.Now;
        var value = expected.ToString("O");

        Assert.True(mock.Object.TryParse(value, out var actual));
        Assert.Equal(expected, actual);
    }

    [Fact]
    public void RecursiveDelegateOut()
    {
        var mock = new Mock<IEnvironment>();

        var expected = DateTimeOffset.Now;
        var value = expected.ToString("O");

        SetupRef<TryParse>(() => mock.Object.Parser.TryParse)
            .Returns((string input, out DateTimeOffset date) => DateTimeOffset.TryParse(value, out date));

        Assert.True(mock.Object.Parser.TryParse(value, out var actual));
        Assert.Equal(expected, actual);
    }

    [Fact]
    public void RaisesEvents()
    {
        var calc = new Mock<ICalculator>();
        var raised = false;
        calc.Object.TurnedOn += (sender, args) => raised = true;

        calc.RaiseTurnedOn();

        Assert.True(raised);
    }

    [Fact]
    public void StrictMock()
    {
        var calc = new Mock<ICalculator>(MockBehavior.Strict);

        calc.Add(1, 1).Returns(2);

        Assert.Equal(2, calc.Object.Add(1, 1));
        Assert.Throws<StrictMockException>(() => calc.Object.TurnOn());
    }

    [Fact]
    public void Verification()
    {
        var calc = new Mock<ICalculator>();

        calc.Object.Add(2, 3);

        Verify.Called(calc).Add(2, 3).Once();
        Verify.NotCalled(calc).TurnOn();
        Verify.Called(() => calc.Object.Add(2, 3), 1);
        Assert.Throws<VerifyException>(() => Verify.Called(calc).TurnOn());
    }

    [Fact]
    public void CustomizesMockCreation()
    {
        var mock = new Mock<ICustomized>();

        Assert.Equal("customized", mock.Sdk.Name);
    }

    delegate bool TryParse(string input, out DateTimeOffset date);
}

public interface IEnvironment
{
    IParser Parser { get; }
}

public interface IParser
{
    bool TryParse(string input, out DateTimeOffset date);
}

public interface ICustomized { }

namespace Moq
{
    partial class Mock<T>
    {
        partial void OnCreated()
        {
            if (Object is ICustomized)
                this.Sdk.Name = "customized";
        }
    }
}
