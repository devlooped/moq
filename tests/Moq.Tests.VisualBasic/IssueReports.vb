' Copyright (c) 2007, Clarius Consulting, Manas Technology Solutions, InSTEDD, and Contributors.
' All rights reserved. Licensed under the BSD 3-Clause License; see License.txt.

Imports System.Linq.Expressions

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

End Class
