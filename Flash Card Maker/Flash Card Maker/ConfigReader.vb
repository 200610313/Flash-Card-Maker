Imports System.IO
Module ConfigReader
    Public sr As StreamReader
    Public line As String
    Public found1stHash As New Boolean
    Public found2ndHash As New Boolean
    Public lineNumber As Integer
    Private collection As New List(Of StudySet)
    Public enabledCreationFromFile As Boolean

    'gets created according to the number of study sets detected
    Private currStudySet As StudySet
    Private currItems As List(Of Item)
    Private currTitle, currDate As String

    Public Function getCollectionFromFile() As List(Of StudySet)
        Return collection
    End Function
    Public Function readConfig()
        If willCreate() Then
            enabledCreationFromFile = True
            collection = New List(Of StudySet) 'create a new collection
            readToCreate()
        Else
            enabledCreationFromFile = False
        End If

        If willDelete() Then

        End If
    End Function

    Private Sub readToCreate()
        sr = New StreamReader("creates.ini")
        line = New String("")
        line = sr.ReadLine

        Dim lineNumber As Integer
        lineNumber = 0
        found1stHash = False
        found2ndHash = False
        makeSet(line)
    End Sub
    Function makeSet(line As String) As String
        If line Is Nothing Then
            Return 1
        End If

        If foundFirstHash(line) Then 'if we found first hash, we token the next two lines as title and date respectively
            currItems = New List(Of Item)
            lineNumber = lineNumber + 1
            makeSet(sr.ReadLine)
        End If

        If foundSecondHash(line) Then 'if we found second hash, that concludes one set
            'Reset counter
            currStudySet = New StudySet(currTitle, currDate, currItems)
            collection.Add(currStudySet)
            Dim x As StudySet

            lineNumber = 0
            makeSet(sr.ReadLine)
        End If

        If lineNumber = 1 Then 'token as title
            currTitle = New String(line) 'save title

            lineNumber = lineNumber + 1
            makeSet(sr.ReadLine)
        End If

        If lineNumber = 2 Then 'token as date
            currDate = New String(line) 'save date
            lineNumber = lineNumber + 1
            makeSet(sr.ReadLine)
        End If

        If lineNumber = 3 Then
            Dim comma As Integer
            comma = getIndexOf(line)
            Dim term, def As String
            term = getTerm(line, comma)
            def = getDef(line, comma)
            currItems.Add(New Item(def, term)) 'Add the definition and term to currItems

            makeSet(sr.ReadLine)
        End If
        If line.Contains("Set creation: enabled") Then
            makeSet(sr.ReadLine)
        Else
            Return 1
        End If
    End Function

    Private Function getDef(line As String, comma As Integer) As String
        Return line.Substring(comma + 1, line.Length - comma - 1)
    End Function

    Private Function getTerm(line As String, comma As Integer) As String
        Return line.Substring(0, comma) 'from index 0 to the left of comma
    End Function

    Private Function getIndexOf(line As String) As Integer
        Dim i As Integer
        For i = 0 To line.Length - 1
            If line.Chars(i) = "," Then
                Return i
            End If
        Next
    End Function

    Private Function willDelete() As Boolean

    End Function

    Private Function willCreate() As Boolean
        sr = New StreamReader("creates.ini")
        line = New String("")
        line = sr.ReadLine
        If line.Contains("Set creation: enabled") Then
            sr.Close()
            Return True
        Else
            sr.Close()
            Return False
        End If
    End Function

    Private Function foundSecondHash(ByRef line As String) As Boolean
        If line.Contains("#") And found1stHash = True And found2ndHash = False Then
            'Reset flags
            found1stHash = False
            Return True
        Else
            Return False
        End If
    End Function

    Private Function foundFirstHash(line As String) As Boolean
        If line.Contains("#") And found1stHash = False And found2ndHash = False Then
            'Correcting flags
            found1stHash = True
            Return True
        Else
            Return False
        End If
    End Function
End Module
