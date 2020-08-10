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
            Dim Data As Byte()() = UpdateCore.Serialization.DeserializeArray(IO.File.ReadAllBytes(FilePath))
            Dim HeaderData As Byte()() = UpdateCore.Serialization.DeserializeArray(Data(0))
            Dim MetaSourceDirectory As String = System.Text.ASCIIEncoding.ASCII.GetString(HeaderData(0))
            Dim MetaProgramName As String = System.Text.ASCIIEncoding.ASCII.GetString(HeaderData(1))
            Dim MetaProgramID As String = System.Text.ASCIIEncoding.ASCII.GetString(HeaderData(2))
            Dim Filename As String = IO.Path.GetFileName(FilePath)
            Dim Header As New MetaHeader With {.Filename = Filename, .ProgramID = MetaProgramID, .ProgramName = MetaProgramName, .SourcePath = MetaSourceDirectory}
            MetaHeaderList.Add(Header)
        Next


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
                End Select
            End If
            CommandLimiter.Limit()
        End While
        If Client.Connected = True Then Client.Write("COMMAND", Serialization.SerializeArray({System.Text.ASCIIEncoding.ASCII.GetBytes("connection.disconnect")}))
        Client.Close()
        RaiseEvent ClientDisconnected(Me, Client)
    End Sub
End Class