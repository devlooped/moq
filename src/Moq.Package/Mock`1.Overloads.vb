Option Strict On

Imports System.Diagnostics.CodeAnalysis
Imports System.Runtime.CompilerServices
Imports Moq.Sdk

Namespace Global.Moq
    <ExcludeFromCodeCoverage>
    <CompilerGenerated>
    Partial Friend Class Mock(Of T As Class, T1)
        Inherits Mock(Of T)

        <MockGenerator>
        Public Sub New(ParamArray constructorArgs As Object())
            Me.New(MockBehavior.Loose, constructorArgs)
        End Sub

        <MockGenerator>
        Public Sub New(behavior As MockBehavior, ParamArray constructorArgs As Object())
            MyBase.New(New MockView(Of T)(Global.Moq.Mock.Create(Of T)(behavior, constructorArgs, GetType(T1))), True)
        End Sub

    End Class

    <ExcludeFromCodeCoverage>
    <CompilerGenerated>
    Partial Friend Class Mock(Of T As Class, T1, T2)
        Inherits Mock(Of T)

        <MockGenerator>
        Public Sub New(ParamArray constructorArgs As Object())
            Me.New(MockBehavior.Loose, constructorArgs)
        End Sub

        <MockGenerator>
        Public Sub New(behavior As MockBehavior, ParamArray constructorArgs As Object())
            MyBase.New(New MockView(Of T)(Global.Moq.Mock.Create(Of T)(behavior, constructorArgs, GetType(T1), GetType(T2))), True)
        End Sub

    End Class

    <ExcludeFromCodeCoverage>
    <CompilerGenerated>
    Partial Friend Class Mock(Of T As Class, T1, T2, T3)
        Inherits Mock(Of T)

        <MockGenerator>
        Public Sub New(ParamArray constructorArgs As Object())
            Me.New(MockBehavior.Loose, constructorArgs)
        End Sub

        <MockGenerator>
        Public Sub New(behavior As MockBehavior, ParamArray constructorArgs As Object())
            MyBase.New(New MockView(Of T)(Global.Moq.Mock.Create(Of T)(behavior, constructorArgs, GetType(T1), GetType(T2), GetType(T3))), True)
        End Sub

    End Class

    <ExcludeFromCodeCoverage>
    <CompilerGenerated>
    Partial Friend Class Mock(Of T As Class, T1, T2, T3, T4)
        Inherits Mock(Of T)

        <MockGenerator>
        Public Sub New(ParamArray constructorArgs As Object())
            Me.New(MockBehavior.Loose, constructorArgs)
        End Sub

        <MockGenerator>
        Public Sub New(behavior As MockBehavior, ParamArray constructorArgs As Object())
            MyBase.New(New MockView(Of T)(Global.Moq.Mock.Create(Of T)(behavior, constructorArgs, GetType(T1), GetType(T2), GetType(T3), GetType(T4))), True)
        End Sub

    End Class

    <ExcludeFromCodeCoverage>
    <CompilerGenerated>
    Partial Friend Class Mock(Of T As Class, T1, T2, T3, T4, T5)
        Inherits Mock(Of T)

        <MockGenerator>
        Public Sub New(ParamArray constructorArgs As Object())
            Me.New(MockBehavior.Loose, constructorArgs)
        End Sub

        <MockGenerator>
        Public Sub New(behavior As MockBehavior, ParamArray constructorArgs As Object())
            MyBase.New(New MockView(Of T)(Global.Moq.Mock.Create(Of T)(behavior, constructorArgs, GetType(T1), GetType(T2), GetType(T3), GetType(T4), GetType(T5))), True)
        End Sub

    End Class

    <ExcludeFromCodeCoverage>
    <CompilerGenerated>
    Partial Friend Class Mock(Of T As Class, T1, T2, T3, T4, T5, T6)
        Inherits Mock(Of T)

        <MockGenerator>
        Public Sub New(ParamArray constructorArgs As Object())
            Me.New(MockBehavior.Loose, constructorArgs)
        End Sub

        <MockGenerator>
        Public Sub New(behavior As MockBehavior, ParamArray constructorArgs As Object())
            MyBase.New(New MockView(Of T)(Global.Moq.Mock.Create(Of T)(behavior, constructorArgs, GetType(T1), GetType(T2), GetType(T3), GetType(T4), GetType(T5), GetType(T6))), True)
        End Sub

    End Class

    <ExcludeFromCodeCoverage>
    <CompilerGenerated>
    Partial Friend Class Mock(Of T As Class, T1, T2, T3, T4, T5, T6, T7)
        Inherits Mock(Of T)

        <MockGenerator>
        Public Sub New(ParamArray constructorArgs As Object())
            Me.New(MockBehavior.Loose, constructorArgs)
        End Sub

        <MockGenerator>
        Public Sub New(behavior As MockBehavior, ParamArray constructorArgs As Object())
            MyBase.New(New MockView(Of T)(Global.Moq.Mock.Create(Of T)(behavior, constructorArgs, GetType(T1), GetType(T2), GetType(T3), GetType(T4), GetType(T5), GetType(T6), GetType(T7))), True)
        End Sub

    End Class

    <ExcludeFromCodeCoverage>
    <CompilerGenerated>
    Partial Friend Class Mock(Of T As Class, T1, T2, T3, T4, T5, T6, T7, T8)
        Inherits Mock(Of T)

        <MockGenerator>
        Public Sub New(ParamArray constructorArgs As Object())
            Me.New(MockBehavior.Loose, constructorArgs)
        End Sub

        <MockGenerator>
        Public Sub New(behavior As MockBehavior, ParamArray constructorArgs As Object())
            MyBase.New(New MockView(Of T)(Global.Moq.Mock.Create(Of T)(behavior, constructorArgs, GetType(T1), GetType(T2), GetType(T3), GetType(T4), GetType(T5), GetType(T6), GetType(T7), GetType(T8))), True)
        End Sub

    End Class

End Namespace
