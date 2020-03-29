Public Class MainForm
    Private WithEvents Client As New Client.Core
    Private Sub MainForm_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Label1.Text = "Connecting to server..."
        If Client.Running = False Then Client.Connect("127.0.0.1:19462")
    End Sub
    Private Sub ConnectionFailed() Handles Client.ConnectionFailed

        Invoke(Sub()
                   Label1.Text = "Unable to establish a connection with the update server. Would you like to retry?"
                   Button4.Visible = True
               End Sub)
    End Sub
    Private Sub MainForm_FormClosing(sender As Object, e As FormClosingEventArgs) Handles MyBase.FormClosing
        If Client.Running = True Then Client.Disconnect()
    End Sub
    Private Sub Button4_Click(sender As Object, e As EventArgs) Handles Button4.Click
        Button4.Visible = False
        Label1.Text = "Connecting to server..."
        If Client.Running = False Then Client.Connect("127.0.0.1:19462")
    End Sub
End Class
