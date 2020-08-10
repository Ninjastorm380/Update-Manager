Public Class Form1
    Private WithEvents Server As New UpdateCore.Server
    Private MetaSaveDirectory As String = "./metadata"
    Private Sub ServerStarted() Handles Server.ServerStarted
        Invoke(Sub()
                   Label1.Text = "server status: online"
               End Sub)
    End Sub
    Private Sub ServerStopped() Handles Server.ServerStopped
        Invoke(Sub()
                   Label1.Text = "server status: offline"
               End Sub)
    End Sub
    Private Sub ClientConnected(sender As Object, e As UpdateCore.Networking.QueuedTcpClient) Handles Server.ClientConnected
        Invoke(Sub()
                   ListBox1.Items.Add(e.RemoteEndpoint.Address.ToString + ":" + e.RemoteEndpoint.Port.ToString)
               End Sub)
    End Sub
    Private Sub ClientDisconnected(sender As Object, e As UpdateCore.Networking.QueuedTcpClient) Handles Server.ClientDisconnected
        Invoke(Sub()
                   ListBox1.Items.Remove(e.RemoteEndpoint.Address.ToString + ":" + e.RemoteEndpoint.Port.ToString)
               End Sub)
    End Sub
    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        Server.Start(TextBox1.Text, "F4Sx5%vyW11Al':,_f", MetaSaveDirectory)
    End Sub

    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        Server.Stop()
    End Sub

    Private Sub Form1_FormClosing(sender As Object, e As FormClosingEventArgs) Handles MyBase.FormClosing
        Server.Stop()
    End Sub

    Private Sub Button3_Click(sender As Object, e As EventArgs)
        With New FolderBrowserDialog
            If .ShowDialog = DialogResult.OK Then
                Label2.Text = "Generating Metadata: 0%"
                UpdateCore.MetadataGenerator.GenerateMetadataAsync(MetaSaveDirectory, .SelectedPath, "", "", "", AddressOf MetadataGenProgressUpdate, AddressOf MetadataGenComplete)
            End If
        End With
    End Sub
    Private Sub MetadataGenProgressUpdate(state As Integer, currentprogress As Integer, maximumprogress As Integer)
        Invoke(Sub()

                   Dim Progressdouble As Double = currentprogress / maximumprogress
                   Dim ProgressString As String = CInt(Progressdouble * 100).ToString
                   RenderedProgressbar1.ProgressMaximum1 = maximumprogress
                   RenderedProgressbar1.ProgressMaximum2 = maximumprogress
                   RenderedProgressbar1.ProgressValue1 = currentprogress
                   RenderedProgressbar1.ProgressValue2 = currentprogress
                   If state = 0 Then
                       Label2.Text = "Generating Metadata: " + ProgressString + "%"
                   ElseIf state = 1 Then
                       Label2.Text = "Serializing Metadata: " + ProgressString + "%"
                   End If
               End Sub)
    End Sub
    Private Sub MetadataGenComplete()
        Invoke(Sub()
                   Label2.Text = "Metadata Generation Complete!"
                   RenderedProgressbar1.ProgressMaximum1 = 1
                   RenderedProgressbar1.ProgressMaximum2 = 1
                   RenderedProgressbar1.ProgressValue1 = 0
                   RenderedProgressbar1.ProgressValue2 = 0
               End Sub)
        ListBox2.Items.Clear()
        For Each x In IO.Directory.GetFiles(MetaSaveDirectory)
            ListBox2.Items.Add(IO.Path.GetFileName(x))
        Next
    End Sub

    Private Sub Button8_Click(sender As Object, e As EventArgs) Handles Button8.Click
        With New MetadataGenForm
            If .ShowDialog = DialogResult.OK Then
                UpdateCore.MetadataGenerator.GenerateMetadataAsync(MetaSaveDirectory, .SourceDirectory, .ProgramName, .ProgramID, .MetadataFileName, AddressOf MetadataGenProgressUpdate, AddressOf MetadataGenComplete)
            End If
        End With
    End Sub

    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        For Each x In IO.Directory.GetFiles(MetaSaveDirectory)
            ListBox2.Items.Add(IO.Path.GetFileName(x))
        Next
    End Sub

    Private Sub Button7_Click(sender As Object, e As EventArgs) Handles Button7.Click
        Dim FilePath As String = MetaSaveDirectory + "/" + ListBox2.SelectedItem.ToString
        Dim Data As Byte()() = UpdateCore.Serialization.DeserializeArray(IO.File.ReadAllBytes(FilePath))
        Dim HeaderData As Byte()() = UpdateCore.Serialization.DeserializeArray(Data(0))
        Dim MetaSourceDirectory As String = System.Text.ASCIIEncoding.ASCII.GetString(HeaderData(0))
        Dim MetaProgramName As String = System.Text.ASCIIEncoding.ASCII.GetString(HeaderData(1))
        Dim MetaProgramID As String = System.Text.ASCIIEncoding.ASCII.GetString(HeaderData(2))
        Dim Filename As String = IO.Path.GetFileName(FilePath)
        TextBox4.Text = MetaProgramName
        TextBox2.Text = MetaProgramID
        TextBox5.Text = MetaSourceDirectory
        TextBox3.Text = Filename
        Data = Nothing
        HeaderData = Nothing
    End Sub

    Private Sub Button3_Click_1(sender As Object, e As EventArgs) Handles Button3.Click
        UpdateCore.MetadataGenerator.GenerateMetadataAsync(MetaSaveDirectory, TextBox5.Text, TextBox4.Text, TextBox2.Text, TextBox3.Text, AddressOf MetadataGenProgressUpdate, AddressOf MetadataGenComplete)

    End Sub
End Class
