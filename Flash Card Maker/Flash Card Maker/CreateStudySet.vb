<Serializable()>
Class CreateStudySet
    Private title As String
    Private date_ As String
    Private set_ As New List(Of Item)
    Private currStudySet As StudySet

    Function getCurrStudySet()
        Return currStudySet
    End Function
    'to add: make use of MyStudySets; Serialization and Deserialization

    Function setTitleAndDate(ByVal t As String, ByVal d As String)
        title = t
        date_ = d
    End Function

    Function getTitle() As String
        Return title
    End Function

    Function getDate() As String
        Return date_
    End Function

    'Adds an item object to the current studyset
    Function addItem(ByVal d As String, ByVal t As String)
        set_.Add(New Item(d, t))
    End Function
    'Clear study set when user abandons operation
    '1) user clicks delete or study or create again

    'Run when finish button is clicked
    Function finalizeStudySet()
        'Make an object out of the title, date, and items
        currStudySet = New StudySet(title, date_, set_)
    End Function

    Function getCurrCount() As Integer
        Return set_.Count()
    End Function
End Class
