Public Class Form1
    Private WithEvents Client As New UpdateCore.Client
    Private Sub ClientConnected(sender As Object, e As UpdateCore.Networking.QueuedTcpClient) Handles Client.ClientConnected
        If Me.IsDisposed = False Then
            Invoke(Sub()
                       Label2.Text = "- Checking for updates -"
                   End Sub)
        End If
        Client.GetMetadataAsync("LZUf6IVeH6lTk278k8RoylRWgTCjCIdkE14cxw3RCnw=")
    End Sub
    Private Sub ClientDisconnected(sender As Object, e As UpdateCore.Networking.QueuedTcpClient) Handles Client.ClientDisconnected
        If Me.IsDisposed = False Then
            Invoke(Sub()
                       Label2.Text = "- Disconnected from update server -"
                   End Sub)
        End If
    End Sub
    Private Sub ConnectionFailed(sender As Object, e As System.Net.Sockets.SocketException) Handles Client.ConnectionFailedEvent
        If Me.IsDisposed = False Then
            Invoke(Sub()
                       Button1.Visible = True
                       Label2.Text = "- Connection failed -"
                   End Sub)
        End If
    End Sub
    Private Sub UpdateCheckCompleted(sender As Object, e As Boolean) Handles Client.UpdateCheckCompleted
        If e = True Then
            If MsgBox("Updates are available for download. Would you like to download them?", MsgBoxStyle.YesNo) = MsgBoxResult.Yes Then
                If Me.IsDisposed = False Then
                    Invoke(Sub()
                               Label2.Text = "- Downloading updates -"
                           End Sub)
                End If
                Client.SetMetastateAsync()
            Else
                If Me.IsDisposed = False Then
                    Invoke(Sub()
                               Label2.Text = "- Update check complete -"
                           End Sub)
                End If
            End If
        Else
            If Me.IsDisposed = False Then
                Invoke(Sub()
                           Label2.Text = "- Update check complete -"
                       End Sub)
            End If
        End If


        'If Me.IsDisposed = False Then
        '    Invoke(Sub()
        '               Label2.Text = "- Update check complete. Your program is up to date. -"
        '           End Sub)
        'End If
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
