' Copyright (c) 2007, Clarius Consulting, Manas Technology Solutions, InSTEDD.
' All rights reserved. Licensed under the BSD 3-Clause License; see License.txt.

Imports Moq
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

End Class
