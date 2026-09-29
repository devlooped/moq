using System;
using Moq;
using Moq.Sdk;
using Sample;
using Stunts;
using Xunit;
using static Moq.Syntax;

/// <summary>
/// Exercises the setup API available below C# 14, where there are no generated typed 
/// setups nor static mocks, against the packaged Moq using dynamic mocks.
/// </summary>
public class Tests
{
    [Fact]
    public void UsesDynamicMocks()
    {
        var calc = new Mock<ICalculator>();

        Assert.StartsWith("Castle.Proxies", calc.Object.GetType().FullName, StringComparison.Ordinal);
    }

    [Fact]
    public void StubProperties()
    {
        var calc = Mock.Of<ICalculator>();

        calc.Mode = CalculatorMode.Scientific;

        Assert.Equal(CalculatorMode.Scientific, calc.Mode);
    }

    [Fact]
    public void Setups()
    {
        var calc = new Mock<ICalculator>();

        Setup(() => calc.Object.Add(2, 3)).Returns(5);
        Setup(() => calc.Object.Add(Any<int>(), Any<int>(), Any<int>())).Returns(args => args.Get<int>(0) + args.Get<int>(1) + args.Get<int>(2));
        Setup(() => calc.Object.Mode).Returns(CalculatorMode.Scientific);

        Assert.Equal(5, calc.Object.Add(2, 3));
        Assert.Equal(6, calc.Object.Add(1, 2, 3));
        Assert.Equal(CalculatorMode.Scientific, calc.Object.Mode);
    }

    [Fact]
    public void RecursiveSetup()
    {
        var calc = new Mock<ICalculator, IDisposable>();

        Setup(() => calc.Object.Memory.Recall()).Returns(5);

        Assert.Equal(5, calc.Object.Memory.Recall());
        Assert.IsAssignableFrom<IDisposable>(calc.As<IDisposable>().Object);
    }

    [Fact]
    public void DelegateOut()
    {
        var mock = new Mock<IParser>();

        SetupRef<TryParse>(mock.Object.TryParse)
            .Returns((string input, out DateTimeOffset date) => DateTimeOffset.TryParse(input, out date));

        var expected = DateTimeOffset.Now;

        Assert.True(mock.Object.TryParse(expected.ToString("O"), out var actual));
        Assert.Equal(expected, actual);
    }

    [Fact]
    public void StrictMock()
    {
        var calc = new Mock<ICalculator>(MockBehavior.Strict);

        Setup(() => calc.Object.Add(1, 1)).Returns(2);

        Assert.Equal(2, calc.Object.Add(1, 1));
        Assert.Throws<StrictMockException>(() => calc.Object.TurnOn());
    }

    [Fact]
    public void Verification()
    {
        var calc = new Mock<ICalculator>();

        calc.Object.Add(2, 3);

        Verify.Called(() => calc.Object.Add(2, 3), 1);
        Verify.NotCalled(() => calc.Object.TurnOn());
        Assert.Throws<VerifyException>(() => Verify.Called(() => calc.Object.TurnOn()));
    }

    delegate bool TryParse(string input, out DateTimeOffset date);
}

public interface IParser
{
    bool TryParse(string input, out DateTimeOffset date);
}
