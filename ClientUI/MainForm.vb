Public Class MainForm
    Private WithEvents Client As New Client.Core
    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        If Client.Running = False Then Client.Connect("127.0.0.1:19462")
    End Sub

    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        If Client.Running = True Then Client.Disconnect()
    End Sub

    Private Sub MainForm_FormClosing(sender As Object, e As FormClosingEventArgs) Handles MyBase.FormClosing
        If Client.Running = True Then Client.Disconnect()
    End Sub

    Private Sub MainForm_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        If Client.Running = False Then Client.Connect("127.0.0.1:19462")
    End Sub
End Class
