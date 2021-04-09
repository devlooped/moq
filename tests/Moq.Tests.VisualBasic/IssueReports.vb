' Copyright (c) 2007, Clarius Consulting, Manas Technology Solutions, InSTEDD, and Contributors.
' All rights reserved. Licensed under the BSD 3-Clause License; see License.txt.

Imports Xunit

Public Class IssueReports

	Public Class Issue278

		<Fact()>
		Public Sub SetupsForPropertiesWithMultipleArgsDoNotOverwriteEachOther()
			Dim mock As New Mock(Of ISimpleInterface)()

			mock.Setup(Function(m) m.PropertyWithMultipleArgs(1, 1)).Returns(1)
			mock.Setup(Function(m) m.PropertyWithMultipleArgs(1, 2)).Returns(2)

			Assert.Equal(1, mock.Object.PropertyWithMultipleArgs(1, 1))
			Assert.Equal(2, mock.Object.PropertyWithMultipleArgs(1, 2))

		End Sub

		Public Interface ISimpleInterface

			ReadOnly Property PropertyWithMultipleArgs(setting As Integer, setting2 As Integer) As Integer

		End Interface
	End Class

	Public Class Issue1067

		<Fact>
		Public Sub Test_NonGeneric()
			Dim userManagerMock = New Mock(Of IUserManager)()
			Setup_NonGeneric(userManagerMock, 42)

			Dim user As New User()
			userManagerMock.Object.Create(user)

			Assert.Equal(42, user.Id)
		End Sub

		<Fact>
		Public Sub Test_Generic()
			Dim userManagerMock = New Mock(Of IUserManager)()
			Setup_Generic(Of User)(userManagerMock, 42)

			Dim user As New User()
			userManagerMock.Object.Create(user)

			Assert.Equal(42, user.Id)
		End Sub

		Public Class User
			Property Id As Integer
		End Class

		Public Interface IUserManager
			Sub Create(User As User)
		End Interface

		Protected Sub Setup_NonGeneric(userManagerMock As Mock(Of IUserManager), expectedId As Integer)
			userManagerMock.Setup(Sub(manager) manager.Create(It.IsAny(Of User))).Callback(Sub(user) user.Id = expectedId)
		End Sub

		Protected Sub Setup_Generic(Of TUser As User)(userManagerMock As Mock(Of IUserManager), expectedId As Integer)
			userManagerMock.Setup(Sub(manager) manager.Create(It.IsAny(Of TUser))).Callback(Sub(user) user.Id = expectedId)
			'                                                             ^
			' The use of generics will cause the VB.NET compiler to wrap the `It.IsAny<>` call with two `Convert` nodes.
			' The inner conversion will convert to `Object`, and the outer conversion will convert to `User` (i.e. the type that
			' `TUser` is constrained to). `MatcherFactory` needs to be able to recognize the `It.IsAny<>` matcher even if it
			' is doubly wrapped!
		End Sub

	End Class

	Public Class Issue1129

		<Fact>
		Public Sub Test()
			Dim classMock = New Mock(Of IndexerInterface)()

			classMock.SetupAllProperties()

			Assert.False(classMock.Object.Value)
		End Sub
		Public Interface IndexerInterface
			ReadOnly Property SystemDefault() As Boolean
			Property Value() As Boolean
			Property Value(ByVal OverrideLevel As Integer) As Boolean
			Property Value(ByVal OverrideLevel As Integer, ByVal OverrideID As String) As Boolean
		End Interface
	End Class

	Public Class Issue1153

		<Fact>
		Public Sub Indexer_overload_can_be_distinguished_from_property_when_mocking_declaring_class()
			Dim mock = New Mock(Of MyVBClassBase)()
			mock.Setup(Function(m) m.Prop).Returns(True)
		End Sub

		<Fact>
		Public Sub Indexer_overload_can_be_distinguished_from_property_when_mocking_subclass_of_declaring_class()
			Dim mock = New Mock(Of MyVBClass)()
			mock.Setup(Function(m) m.Prop).Returns(True)
		End Sub

		Public Class MyVBClassBase
			Public Overridable ReadOnly Property Prop() As Boolean
				Get
					Return True
				End Get
			End Property
			Public Overridable ReadOnly Property Prop(ByVal userID As Guid) As Boolean
				Get
					Return False
				End Get
			End Property
		End Class

		Public Class MyVBClass
			Inherits MyVBClassBase
		End Class

	End Class

End Class
