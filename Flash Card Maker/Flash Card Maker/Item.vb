<Serializable()>
Public Class Item
    Private definition As String
    Private term As String

    Sub New(d As String, t As String)
        Me.definition = d
        Me.term = t
    End Sub

    Function getDefinition() As String
        Return definition
    End Function

    Function getTerm() As String
        Return term
    End Function
End Class
