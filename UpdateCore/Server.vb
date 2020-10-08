Public Class Server : Inherits ServerBase
    Public Event ClientConnected As EventHandler(Of Networking.QueuedTcpClient)
    Public Event ClientDisconnected As EventHandler(Of Networking.QueuedTcpClient)

    Private Structure MetaHeader
        Public Filename As String
        Public ProgramName As String
        Public ProgramID As String
        Public SourcePath As String
    End Structure
    Friend Overrides Sub ServerMain(Client As Networking.QueuedTcpClient)

        Dim CommandLimiter As New ThreadLimiter(5)
        Client.CreateQueue("COMMAND")
        RaiseEvent ClientConnected(Me, Client)
        Dim MetaHeaderList As New List(Of MetaHeader)
        For Each x In IO.Directory.GetFiles(MetaSaveDirectory)
            Dim FilePath As String = x
            Dim MetaStream As New IO.FileStream(FilePath, IO.FileMode.Open)
            Dim MetaFileHeader = MetadataGenerator.ReadMetadata(MetaStream)(0)
            MetaStream.Close()
            Dim MetaSourceDirectory As String = MetaFileHeader.BasePath
            Dim MetaProgramName As String = MetaFileHeader.ProgramName
            Dim MetaProgramID As String = MetaFileHeader.ProgramID
            Dim Filename As String = IO.Path.GetFileName(FilePath)
            MetaHeaderList.Add(New MetaHeader With {.Filename = Filename, .ProgramID = MetaProgramID, .ProgramName = MetaProgramName, .SourcePath = MetaSourceDirectory})
        Next

        Dim ClientState As MetadataGenerator.MetaStateObject() = {}
        While Client.Connected = True And Running = True
            If Client.HasData("COMMAND") = True Then
                Dim Data As Byte()() = Serialization.DeserializeArray(Client.Read("COMMAND"))
                Dim Command As String = System.Text.ASCIIEncoding.ASCII.GetString(Data(0))
                Select Case Command
                    Case "connection.disconnect"
                        Exit While
                    Case "update.metadata.get"
                        Dim ID As String = System.Text.ASCIIEncoding.ASCII.GetString(Data(1))
                        Dim exists As Boolean = False
                        Dim FoundHeader As New MetaHeader
                        For Each x In MetaHeaderList
                            If x.ProgramID = ID Then
                                exists = True
                                FoundHeader = x
                                Exit For
                            End If
                        Next
                        If exists = True Then
                            Client.Write("COMMAND", Serialization.SerializeArray({System.Text.ASCIIEncoding.ASCII.GetBytes("update.metadata.get"),
                                                                                  System.Text.ASCIIEncoding.ASCII.GetBytes("0"),
                                                                                  IO.File.ReadAllBytes(MetaSaveDirectory + "/" + FoundHeader.Filename)}))
                        Else
                            Client.Write("COMMAND", Serialization.SerializeArray({System.Text.ASCIIEncoding.ASCII.GetBytes("update.metadata.get"),
                                                                                  System.Text.ASCIIEncoding.ASCII.GetBytes("1")}))
                        End If
                    Case "update.metastate.set"
                        Dim MS As New IO.MemoryStream(Data(1))
                        ClientState = MetadataGenerator.ReadMetastateData(MS)
                        MS.Dispose()
                    Case "update.metastate.get"
                        Dim MS As New IO.MemoryStream()
                        MetadataGenerator.WriteMetastateData(ClientState, MS)
                        Client.Write("COMMAND", Serialization.SerializeArray({System.Text.ASCIIEncoding.ASCII.GetBytes("update.metastate.get"), MS.ToArray}))
                        MS.Dispose()

                End Select
            End If
            CommandLimiter.Limit()
        End While
        If Client.Connected = True Then Client.Write("COMMAND", Serialization.SerializeArray({System.Text.ASCIIEncoding.ASCII.GetBytes("connection.disconnect")}))
        Client.Close()
        RaiseEvent ClientDisconnected(Me, Client)
    End Sub
End Class