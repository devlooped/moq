Option Strict On

Imports System
Imports System.Diagnostics.CodeAnalysis
Imports System.Reflection
Imports System.Runtime.CompilerServices
Imports Moq.Sdk

Namespace Global.Moq

    <ExcludeFromCodeCoverage>
    <CompilerGenerated>
    Partial Friend Class Mock

        ''' <summary>
        ''' Gets the configuration and introspection for the given mocked instance.
        ''' </summary>
        ''' <exception cref="ArgumentException">The <paramref name="instance"/> is not a mock.</exception>
        Public Shared Function [Get](Of T As Class)(instance As T) As Mock(Of T)
            Return New Mock(Of T)(New MockView(Of T)(instance), False)
        End Function

        ''' <summary>
        ''' Gets the configuration and introspection for the given mocked instance 
        ''' as the <typeparamref name="T"/> it implements.
        ''' </summary>
        ''' <exception cref="ArgumentException">The <paramref name="instance"/> is not a mock.</exception>
        ''' <exception cref="InvalidCastException">The mock does not implement <typeparamref name="T"/>.</exception>
        Public Shared Function [Get](Of T As Class)(instance As Object) As Mock(Of T)
            Return New Mock(Of T)(New MockView(Of T)(DirectCast(instance, T)), False)
        End Function

        Friend Shared Function Create(Of T As Class)(ByVal behavior As MockBehavior, ByVal constructorArgs As Object(), ParamArray interfaces As Type()) As T
            Dim mock = MockFactory.[Default].CreateMock(GetType(Mock).Assembly, GetType(T), interfaces, constructorArgs)
            ' Delegate mocks are delegates bound to the mocked instance.
            Dim [delegate] = TryCast(mock, [Delegate])
            Dim mocked = DirectCast(If([delegate] IsNot Nothing, [delegate].Target, mock), IMocked)

            mocked.Initialize(behavior)

            Return DirectCast(mock, T)
        End Function

    End Class

End Namespace
