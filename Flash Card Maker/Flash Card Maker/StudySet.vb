<Serializable()>
Public Class StudySet
    Private title As String
    Private date_ As String
    Private set_ As New List(Of Item)

    Sub New(t As String, d As String, s As List(Of Item))
        title = t
        date_ = d
        set_ = s
    End Sub

    Function getTitle()
        Return title
    End Function
    Function getDate_()
        Return date_
    End Function

    Function getSet() As List(Of Item)
        Return set_
    End Function
End Class
