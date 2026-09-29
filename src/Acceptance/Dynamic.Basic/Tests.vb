Option Strict On
Option Infer On
Imports System.Runtime.InteropServices
Imports Dynamic.Basic.Model
Imports Moq
Imports Moq.Sdk
Imports Moq.Syntax
Imports Xunit

''' <summary>
''' Exercises the setup API available to Visual Basic against the packaged Moq.
''' </summary>
Public Class Tests
    <Fact>
    Public Sub StubProperties()
        Dim calc = Mock.[Of](Of ICalculator)()
        calc.Mode = CalculatorMode.Scientific
        Assert.Equal(CalculatorMode.Scientific, calc.Mode)
        calc.Mode = CalculatorMode.Standard
        Assert.Equal(CalculatorMode.Standard, calc.Mode)
    End Sub

    <Fact>
    Public Sub Setups()
        Dim calc = New Mock(Of ICalculator)()

        Setup(Function() calc.Object.Add(2, 3)).Returns(5)
        Setup(Function() calc.Object.Mode).Returns(CalculatorMode.Scientific)

        Assert.Equal(5, calc.Object.Add(2, 3))
        Assert.Equal(CalculatorMode.Scientific, calc.Object.Mode)
    End Sub

    <Fact>
    Public Sub RecursiveSetup()
        Dim calc = New Mock(Of ICalculator, IDisposable)()

        Setup(Function() calc.Object.Memory.Recall()).Returns(5)

        Assert.Equal(5, calc.Object.Memory.Recall())
        Assert.IsAssignableFrom(Of IDisposable)(calc.As(Of IDisposable)().Object)
    End Sub

    <Fact>
    Public Sub GetsMockFromObject()
        Dim calc = Mock.[Of](Of ICalculator)()

        Setup(Function() Mock.Get(calc).Object.Add(1, 1)).Returns(3)

        Assert.Equal(3, calc.Add(1, 1))
    End Sub

    <Fact>
    Public Sub DelegateOut()
        Dim mock = New Mock(Of IParser)()

        SetupRef(Of TryParse)(AddressOf mock.Object.TryParse) _
            .Returns(Function(input As String, ByRef result As DateTimeOffset) DateTimeOffset.TryParse(input, result))

        Dim expected = DateTimeOffset.Now
        Dim actual As DateTimeOffset

        Assert.True(mock.Object.TryParse(expected.ToString("O"), actual))
        Assert.Equal(expected, actual)
    End Sub

    <Fact>
    Public Sub StrictMock()
        Dim calc = New Mock(Of ICalculator)(MockBehavior.Strict)

        Setup(Function() calc.Object.Add(1, 1)).Returns(2)

        Assert.Equal(2, calc.Object.Add(1, 1))
        Assert.Throws(Of StrictMockException)(Sub() calc.Object.TurnOn())
    End Sub

    <Fact>
    Public Sub Verification()
        Dim calc = New Mock(Of ICalculator)()

        calc.Object.Add(2, 3)

        Verify.Called(Function() calc.Object.Add(2, 3), 1)
        Verify.NotCalled(Sub() calc.Object.TurnOn())
        Assert.Throws(Of VerifyException)(Sub() Verify.Called(Sub() calc.Object.TurnOn()))
    End Sub

    Delegate Function TryParse(input As String, <Out> ByRef result As DateTimeOffset) As Boolean

End Class
