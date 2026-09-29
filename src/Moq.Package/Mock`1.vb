Option Strict On

Imports System
Imports System.ComponentModel
Imports System.Diagnostics
Imports System.Diagnostics.CodeAnalysis
Imports System.Runtime.CompilerServices
Imports Moq.Sdk
Imports Stunts

Namespace Global.Moq

    ''' <summary>
    ''' Creates a mock that inherits or implements the type <typeparamref name="T"/>, 
    ''' which is set up by invoking its members on this mock.
    ''' </summary>
    ''' <remarks>
    ''' This class is provided as source, so it can be customized by adding members 
    ''' and constructors to it in a partial class, and by implementing 
    ''' <see cref="OnCreated"/> to configure every mock it creates.
    ''' </remarks>
    <ExcludeFromCodeCoverage>
    <CompilerGenerated>
    Partial Friend Class Mock(Of T As Class)
        Implements IMock(Of T)

        <DebuggerBrowsable(DebuggerBrowsableState.Never)>
        Private ReadOnly view As IMock(Of T)

        ''' <summary>
        ''' Creates a mock that inherits or implements the type <typeparamref name="T"/>.
        ''' </summary>
        ''' <param name="constructorArgs">Optional constructor arguments for a mocked class.</param>
        <MockGenerator>
        Public Sub New(ParamArray constructorArgs As Object())
            Me.New(MockBehavior.Loose, constructorArgs)
        End Sub

        ''' <summary>
        ''' Creates a mock that inherits or implements the type <typeparamref name="T"/>.
        ''' </summary>
        ''' <param name="behavior">Whether the mock is loose or strict.</param>
        ''' <param name="constructorArgs">Optional constructor arguments for a mocked class.</param>
        <MockGenerator>
        Public Sub New(behavior As MockBehavior, ParamArray constructorArgs As Object())
            Me.New(New MockView(Of T)(Global.Moq.Mock.Create(Of T)(behavior, constructorArgs)), True)
        End Sub

        ''' <summary>
        ''' Initializes the mock over the <paramref name="mockView"/> of a mocked instance, 
        ''' invoking <see cref="OnCreated"/> if it was just <paramref name="created"/>.
        ''' </summary>
        Friend Sub New(mockView As MockView(Of T), created As Boolean)
            view = mockView
            If created Then OnCreated()
        End Sub

        ''' <summary>
        ''' Invoked after a new mock is created, to customize it.
        ''' </summary>
        Partial Private Sub OnCreated()
        End Sub

        ''' <summary>
        ''' The mocked object instance.
        ''' </summary>
        Public ReadOnly Property [Object] As T Implements IMock(Of T).Object
            Get
                Return view.Object
            End Get
        End Property

        <DebuggerBrowsable(DebuggerBrowsableState.Never)>
        Private ReadOnly Property UntypedObject As Object Implements IMock.Object
            Get
                Return view.Object
            End Get
        End Property

        ''' <summary>
        ''' The behavior of the mock.
        ''' </summary>
        Public Property Behavior As MockBehavior Implements IMock.Behavior
            Get
                Return view.Behavior
            End Get
            Set(value As MockBehavior)
                view.Behavior = value
            End Set
        End Property

        ''' <summary>
        ''' The provider of default values for members that are not set up.
        ''' </summary>
        Public Property DefaultValue As DefaultValueProvider Implements IMock.DefaultValue
            Get
                Return view.DefaultValue
            End Get
            Set(value As DefaultValueProvider)
                view.DefaultValue = value
            End Set
        End Property

        ''' <summary>
        ''' Whether the base member implementation is invoked by default 
        ''' for members that are not set up.
        ''' </summary>
        Public Property CallBase As Boolean Implements IMock.CallBase
            Get
                Return view.CallBase
            End Get
            Set(value As Boolean)
                view.CallBase = value
            End Set
        End Property

        ''' <summary>
        ''' Gets a mock for setting up the <typeparamref name="TInterface"/> 
        ''' implemented by the same mocked object.
        ''' </summary>
        ''' <exception cref="InvalidCastException">The mock does not implement <typeparamref name="TInterface"/>.</exception>
        Public Function [As](Of TInterface As Class)() As Mock(Of TInterface)
            Return Global.Moq.Mock.Get(Of TInterface)(CObj(view.Object))
        End Function

        <EditorBrowsable(EditorBrowsableState.Never)>
        Private Function IFluentInterface_GetType() As Type Implements IFluentInterface.GetType
            Return [GetType]()
        End Function

        <EditorBrowsable(EditorBrowsableState.Never)>
        Private Function IFluentInterface_GetHashCode() As Integer Implements IFluentInterface.GetHashCode
            Return GetHashCode()
        End Function

        <EditorBrowsable(EditorBrowsableState.Never)>
        Private Function IFluentInterface_ToString() As String Implements IFluentInterface.ToString
            Return ToString()
        End Function

        <EditorBrowsable(EditorBrowsableState.Never)>
        Private Function IFluentInterface_Equals(obj As Object) As Boolean Implements IFluentInterface.Equals
            Return Equals(obj)
        End Function

    End Class

End Namespace
