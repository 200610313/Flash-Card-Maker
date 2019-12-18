Imports System.IO
Imports System.Runtime.InteropServices
Imports System.Runtime.Serialization.Formatters.Binary

<Serializable()>
Public Class Main
    Private collection As List(Of StudySet)
    Private currStudySet As CreateStudySet
    Private stream As FileStream
    Private formatter As BinaryFormatter
    Private selectedStudySet As StudySet
    Private index As Integer
    Private seen As Boolean
    Private atFlashCards As Boolean

    'This function is run at startup
    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        'Retrieve data of collection from saved file at first load
        'restore the class from a file
        atFlashCards = False
        Me.KeyPreview = True 'to capture key presses
        formatter = New BinaryFormatter()
        stream = File.OpenRead("data.txt")
        collection = formatter.Deserialize(stream)
        stream.Close()
        index = 0 'used for flashcards
        seen = False 'used for flashcards
        Create_Click(sender, e)
    End Sub

    'To enable window repositioning
    Public Const WM_NCLBUTTONDOWN As Integer = &HA1
    Public Const HT_CAPTION As Integer = &H2

    <DllImportAttribute("user32.dll")>
    Public Shared Function SendMessage(ByVal hWnd As IntPtr, ByVal Msg As Integer, ByVal wParam As Integer, ByVal lParam As Integer) As Integer
    End Function

    <DllImportAttribute("user32.dll")>
    Public Shared Function ReleaseCapture() As Boolean
    End Function

    Private Sub Form1_MouseDown(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles Me.MouseDown, LeftPanel.MouseDown
        If e.Button = Windows.Forms.MouseButtons.Left Then
            ReleaseCapture()
            SendMessage(Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0)
        End If
    End Sub


    'Minimize button
    Public Sub minimizeBtn_Click(sender As Object, e As EventArgs) Handles minimizeBtn.Click
        Me.WindowState = System.Windows.Forms.FormWindowState.Minimized
    End Sub
    'Exit button
    Private Sub exitBtn_Click(sender As Object, e As EventArgs) Handles exitBtn.Click
        'When exit, save to file.
        formatter = New BinaryFormatter()
        'create fileStream
        stream = File.Create("data.txt")
        'create binary formatter
        formatter.Serialize(stream, collection)
        stream.Close()
        Me.Close()
    End Sub
    'Create button
    Private Sub Create_Click(sender As Object, e As EventArgs) Handles Create.Click
        atFlashCards = False
        currStudySet = New CreateStudySet()
        Stdy.SendToBack()
        Crt.BringToFront()
        CreatePanel_setTitle.BringToFront()
    End Sub
    'Submit button
    Private Sub submitTitleBtn_Click(sender As Object, e As EventArgs) Handles submitTitleBtn.Click
        If String.IsNullOrEmpty(chosenTitle.Text) = False Then
            currStudySet.setTitleAndDate(chosenTitle.Text, chosenDate.Value.Date)
            setTitle_label.Text = currStudySet.getTitle()
            selectedDate.Text = currStudySet.getDate()
            CreatePanel_setItems.BringToFront()
            'setting the number counter: at first it is 1 ( 0 + 1 )
            itemCtr.Text = currStudySet.getCurrCount() + 1
            chosenTitle.Text = ""
        End If
    End Sub
    'Finish button
    Private Sub finish_btn_Click(sender As Object, e As EventArgs) Handles finish_btn.Click
        If currStudySet.getCurrCount > 0 Then
            CreatePanel_confirmItems.BringToFront()
            summaryTitle.Text = currStudySet.getTitle()
            summaryDate.Text = currStudySet.getDate()
            numOItems.Text = Convert.ToString(currStudySet.getCurrCount()) + " Items"
            If String.IsNullOrEmpty(term_.Text) = False Or String.IsNullOrEmpty(def_.Text) = False Then
                currStudySet.addItem(def_.Text, term_.Text)
                numOItems.Text = Convert.ToString(currStudySet.getCurrCount()) + " Items"
            End If
            'Reset counter
            itemCtr.Text = 0
        End If


    End Sub
    'ConfirmButton
    'Success feedback panel
    Private Sub BunifuImageButton3_Click(sender As Object, e As EventArgs) Handles Confirm.Click
        'Create study set object from input 
        currStudySet.finalizeStudySet()
        'Then add that study set to the collection
        collection.Add(currStudySet.getCurrStudySet)

        CreatePanel_SuccessFeedBck.BringToFront()
        'reset first so set to false
        CreatePanel_SuccessFeedBck.Visible = False
        CreatePanel_SuccessFeedBck.Visible = True
        Timer1.Interval = 4500
        Timer1.Start()
    End Sub
    'Stops the gif, then redirects to Study Panel
    Private Sub Timer1_Tick(ByVal sender As Object, ByVal e As EventArgs) Handles Timer1.Tick
        CreatePanel_SuccessFeedBck.Visible = False
        Timer1.Stop()
        Study_Click(sender, e)
    End Sub
    'Study button
    Public Sub Study_Click(sender As Object, e As EventArgs) Handles Study.Click
        atFlashCards = False
        Crt.SendToBack()
        slct.BringToFront()
        loadListView()
        index = 0
        seen = False
    End Sub
    'Add more button
    Private Sub addMore_btn_Click(sender As Object, e As EventArgs) Handles addMore_btn.Click
        If String.IsNullOrEmpty(term_.Text) = False And String.IsNullOrEmpty(def_.Text) = False Then
            currStudySet.addItem(def_.Text, term_.Text)
            itemCtr.Text = Convert.ToInt32(itemCtr.Text) + 1
            'Clear textboxes for term and definition for the next item to be added
            term_.Text = ""
            def_.Text = ""

        End If
    End Sub
    'Delete button
    Private Sub Delete_Click(sender As Object, e As EventArgs) Handles Delete.Click
        atFlashCards = False
        Crt.SendToBack()
        deletionPanel.BringToFront()
        loadListView2()
        index = 0
        seen = False
    End Sub

    Private Sub loadListView()
        Dim i As Integer
        'clear first so no appends happen
        ListOfSets.Items.Clear()

        For i = 0 To collection.Count - 1
            Dim array() As String = {collection.Item(i).getTitle, collection.Item(i).getDate_, collection.Item(i).getSet.Count}
            Dim temp As New ListViewItem(array)
            ListOfSets.Items.Add(temp)

        Next
    End Sub

    Private Sub loadListView2()
        Dim i As Integer
        'clear first so no appends happen
        ListOfSetsDupe.Items.Clear()

        For i = 0 To collection.Count - 1
            Dim array() As String = {collection.Item(i).getTitle, collection.Item(i).getDate_, collection.Item(i).getSet.Count}
            Dim temp As New ListViewItem(array)
            ListOfSetsDupe.Items.Add(temp)
        Next
    End Sub



    Private Sub selectBtn_Click(sender As Object, e As EventArgs) Handles selectBtn.Click
        atFlashCards = True
        selectedStudySet = findSetWithTitle(ListOfSets.SelectedItems(0).Text)
        Stdy.BringToFront()
        progressbar.Value = ((index + 1) / selectedStudySet.getSet.Count) * 100
        output.Text = selectedStudySet.getSet.Item(index).getDefinition
        termOrDef.Text = "Definition"
        output.BackColor = Color.FromArgb(255, 255, 192)
    End Sub

    Private Function findSetWithTitle(text As String) As StudySet
        Dim i As Integer
        For i = 0 To collection.Count - 1
            If text.Equals(collection.Item(i).getTitle) Then
                Return collection.Item(i)
                'MessageBox.Show(collection.Item(i).getTitle)
            End If
        Next
    End Function

    Private Sub righty_Click(sender As Object, e As EventArgs) Handles righty.Click
        seen = False
        progressbar.Value = ((index + 1) / selectedStudySet.getSet.Count) * 100
        output.Text = selectedStudySet.getSet.Item(index).getDefinition
        termOrDef.Text = "Definition"
        If index + 1 <> selectedStudySet.getSet.Count Then
            index = index + 1
        End If
    End Sub

    Private Sub lefty_Click(sender As Object, e As EventArgs) Handles lefty.Click
        seen = False
        progressbar.Value = ((index + 1) / selectedStudySet.getSet.Count) * 100
        output.Text = selectedStudySet.getSet.Item(index).getDefinition
        termOrDef.Text = "Definition"
        If index - 1 <> -1 Then
            index = index - 1
        End If
    End Sub

    Private Sub reveal_Click(sender As Object, e As EventArgs) Handles reveal.Click
        If seen = False Then
            output.Text = selectedStudySet.getSet.Item(index).getTerm
            termOrDef.Text = "Term"
            output.BackColor = Color.FromArgb(192, 255, 192)
            seen = True
        Else
            output.Text = selectedStudySet.getSet.Item(index).getDefinition
            termOrDef.Text = "Definition"
            output.BackColor = Color.FromArgb(255, 255, 192)
            seen = False
        End If

    End Sub

    Private Sub deleteButton_Click(sender As Object, e As EventArgs) Handles deleteButton.Click
        selectedStudySet = findSetWithTitle(ListOfSetsDupe.SelectedItems(0).Text)
        collection.Remove(selectedStudySet)
        loadListView2()
    End Sub

    Private Sub Main_KeyDown(sender As Object, e As KeyEventArgs) Handles Me.KeyDown
        If atFlashCards And (e.KeyCode = Keys.W Or e.KeyCode = Keys.S) Then
            reveal_Click(sender, e)
        End If

        If atFlashCards And e.KeyCode = Keys.A Then
            righty_Click(sender, e)
        End If

        If atFlashCards And e.KeyCode = Keys.D Then
            lefty_Click(sender, e)
        End If
    End Sub
End Class
