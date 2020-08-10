Public Class MetadataGenForm
    Public Property ProgramName As String
    Public Property ProgramID As String
    Public Property SourceDirectory As String
    Public Property MetadataFileName As String
    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        DialogResult = DialogResult.OK
    End Sub
    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        DialogResult = DialogResult.Cancel
    End Sub

    Private Sub TextBox1_TextChanged(sender As Object, e As EventArgs) Handles TextBox1.TextChanged
        ProgramName = TextBox1.Text
        TextBox4.Text = (ProgramName + ".meta").ToLowerInvariant
    End Sub

    Private Sub TextBox2_TextChanged(sender As Object, e As EventArgs) Handles TextBox2.TextChanged
        ProgramID = TextBox2.Text
    End Sub

    Private Sub TextBox3_TextChanged(sender As Object, e As EventArgs) Handles TextBox3.TextChanged
        SourceDirectory = TextBox3.Text
    End Sub

    Private Sub TextBox4_TextChanged(sender As Object, e As EventArgs) Handles TextBox4.TextChanged
        MetadataFileName = TextBox4.Text
    End Sub

    Private Sub Button5_Click(sender As Object, e As EventArgs) Handles Button5.Click
        With New FolderBrowserDialog
            If .ShowDialog = DialogResult.OK Then
                TextBox3.Text = .SelectedPath
            End If

        End With
    End Sub
    Private Sub Button4_Click(sender As Object, e As EventArgs) Handles Button4.Click
        Dim R As New Random
        Dim RNGBuffer(31) As Byte
        R.NextBytes(RNGBuffer)
        TextBox2.Text = Convert.ToBase64String(RNGBuffer)
        R = Nothing
        RNGBuffer = Nothing
    End Sub
    Private Sub Button3_Click(sender As Object, e As EventArgs) Handles Button3.Click
        With New OpenFileDialog
            If .ShowDialog = DialogResult.OK Then
                Dim Data As Byte()() = UpdateCore.Serialization.DeserializeArray(IO.File.ReadAllBytes(.FileName))
                Dim HeaderData As Byte()() = UpdateCore.Serialization.DeserializeArray(Data(0))
                Dim MetaSourceDirectory As String = System.Text.ASCIIEncoding.ASCII.GetString(HeaderData(0))
                Dim MetaProgramName As String = System.Text.ASCIIEncoding.ASCII.GetString(HeaderData(1))
                Dim MetaProgramID As String = System.Text.ASCIIEncoding.ASCII.GetString(HeaderData(2))
                Dim Filename As String = IO.Path.GetFileName(.FileName)
                TextBox1.Text = MetaProgramName
                TextBox2.Text = MetaProgramID
                TextBox3.Text = MetaSourceDirectory
                TextBox4.Text = Filename
                Data = Nothing
                HeaderData = Nothing
            End If
        End With
    End Sub
End Class