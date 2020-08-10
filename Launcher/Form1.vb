Public Class Form1
    Private WithEvents Client As New UpdateCore.Client
    Private Sub ClientConnected(sender As Object, e As UpdateCore.Networking.QueuedTcpClient) Handles Client.ClientConnected
        If Me.IsDisposed = False Then
            Invoke(Sub()
                       Label2.Text = "- Connected to update Server, checking for updates -"
                   End Sub)
        End If
        Client.GetMetadataAsync("h25RFjyfnWKl4sGY8F4A6p4q2PgwzIk+a1D7VL7Hhy8=")
    End Sub
    Private Sub ClientDisconnected(sender As Object, e As UpdateCore.Networking.QueuedTcpClient) Handles Client.ClientDisconnected
        If Me.IsDisposed = False Then
            Invoke(Sub()
                       Label2.Text = "- Disconnected From Update Server -"
                   End Sub)
        End If
    End Sub
    Private Sub ConnectionFailed(sender As Object, e As System.Net.Sockets.SocketException) Handles Client.ConnectionFailedEvent
        If Me.IsDisposed = False Then
            Invoke(Sub()
                       Button1.Visible = True
                       Label2.Text = "- Connection Failed. Retry? -"
                   End Sub)
        End If
    End Sub
    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Client.Connect("127.0.0.1:19462", "F4Sx5%vyW11Al':,_f")
    End Sub

    Private Sub Form1_FormClosing(sender As Object, e As FormClosingEventArgs) Handles MyBase.FormClosing
        Client.Disconnect()
    End Sub

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        Button1.Visible = False
        Client.Connect("127.0.0.1:19462", "F4Sx5%vyW11Al':,_f")
        Label2.Text = "- Connecting to server -"
    End Sub
End Class
