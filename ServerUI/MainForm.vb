Public Class MainForm
    Private WithEvents Server As New Server.Core
    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        If Server.Running = False Then Server.Start(19462)
    End Sub
    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        If Server.Running = True Then Server.Stop()
    End Sub
End Class
