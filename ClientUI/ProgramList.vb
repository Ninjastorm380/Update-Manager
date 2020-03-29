Public Class ProgramList
    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        Dim NewItem As New ProgramListItem
        NewItem.Dock = DockStyle.Top
        Panel2.Controls.Add(NewItem)
    End Sub
End Class
