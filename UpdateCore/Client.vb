Public Class Client
    Private WithEvents Client As Networking.QueuedTcpClient
    Public Event ClientConnected As EventHandler(Of Networking.QueuedTcpClient)
    Public Event ClientDisconnected As EventHandler(Of Networking.QueuedTcpClient)
    Public Event ConnectionFailedEvent As EventHandler(Of System.Net.Sockets.SocketException)
    Public Event UpdateCheckCompleted As EventHandler(Of Boolean)
    Private ProgramUpdateState As MetadataGenerator.MetaStateObject()
    Private CryptographicKey As String
    Private Address As String
    Public ReadOnly Property Connected As Boolean
        Get
            If Client IsNot Nothing Then Return Client.Connected Else Return False
        End Get
    End Property
    Private Sub ClientKickstart()
        Try
            Dim AddressData As String() = Address.Split(":")
            If Client IsNot Nothing Then Client.Dispose()
            Client = New Networking.QueuedTcpClient(AddressData(0), AddressData(1), CryptographicKey)
            Dim ClientMainMethodThread As New Threading.Thread(AddressOf Me.ClientMain) : ClientMainMethodThread.Start(Client)
            Dim ClientDataMethodThread As New Threading.Thread(AddressOf Me.ClientData) : ClientDataMethodThread.Start(Client)
        Catch ex As System.Net.Sockets.SocketException
            RaiseEvent ConnectionFailedEvent(Me, ex)
        End Try
    End Sub
    Public Sub Connect(NetworkAddress As String, Key As String)
        If Connected = False Then
            Address = NetworkAddress
            CryptographicKey = Key
            Dim ConnectThread As New Threading.Thread(AddressOf ClientKickstart) : ConnectThread.Start()
        End If
    End Sub
    Public Sub Disconnect()
        If Client.Connected = True Then If Client IsNot Nothing AndAlso Client.Connected = True Then Client.Write("COMMAND", Serialization.SerializeArray({System.Text.ASCIIEncoding.ASCII.GetBytes("connection.disconnect")}))
    End Sub
    Public Sub GetMetadataAsync(ID As String)
        If Client.Connected = True Then Client.Write("COMMAND", Serialization.SerializeArray({System.Text.ASCIIEncoding.ASCII.GetBytes("update.metadata.get"),
                                                              System.Text.ASCIIEncoding.ASCII.GetBytes(ID)}))
    End Sub
    Public Sub StartFileDownloadAsync()
        If Client.Connected = True Then Client.Write("DATA", Serialization.SerializeArray({System.Text.ASCIIEncoding.ASCII.GetBytes("update.file.download.start")}))
    End Sub
    Public Sub StopFileDownloadAsync()
        If Client.Connected = True Then Client.Write("DATA", Serialization.SerializeArray({System.Text.ASCIIEncoding.ASCII.GetBytes("update.file.download.stop")}))
    End Sub

    Public Sub SelectFileAsync()
        If Client.Connected = True Then Client.Write("COMMAND", Serialization.SerializeArray({System.Text.ASCIIEncoding.ASCII.GetBytes("update.file.select")}))
    End Sub
    Public Sub SetMetastateAsync()
        Dim MS As New IO.MemoryStream
        MetadataGenerator.WriteMetastateData(ProgramUpdateState.ToArray, MS)
        If Client.Connected = True Then Client.Write("COMMAND", Serialization.SerializeArray({System.Text.ASCIIEncoding.ASCII.GetBytes("update.metastate.set"), MS.ToArray}))
        MS.Close()
        MS.Dispose()
    End Sub
    Public Sub GetMetastateAsync()
        If Client.Connected = True Then    Client.Write("COMMAND", Serialization.SerializeArray({System.Text.ASCIIEncoding.ASCII.GetBytes("update.metastate.get")}))
    End Sub
    Private Sub ClientData(Client As Networking.QueuedTcpClient)
        Dim DataLimiter As New ThreadLimiter(100)
        Client.CreateQueue("DATA")
        While Client.Connected = True
            If Client.HasData("DATA") = True Then
                Dim Data As Byte()() = Serialization.DeserializeArray(Client.Read("DATA"))
                Select Case System.Text.ASCIIEncoding.ASCII.GetString(Data(0))
                    Case "update.file.download.start"


                    Case "update.file.download.stop"

                    Case "update.file.download.begin"
                    Case "update.file.download.part"
                    Case "update.file.download.end"

                        GetMetastateAsync()
                    Case "update.end"

                        ProgramUpdateState = Nothing
                End Select
            End If
            DataLimiter.Limit()
        End While
    End Sub
    Private Sub ClientMain(Client As Networking.QueuedTcpClient)
        Dim CommandLimiter As New ThreadLimiter(5)
        Client.CreateQueue("COMMAND")
        RaiseEvent ClientConnected(Me, Client)
        While Client.Connected = True
            If Client.HasData("COMMAND") = True Then
                Dim Data As Byte()() = Serialization.DeserializeArray(Client.Read("COMMAND"))
                Select Case System.Text.ASCIIEncoding.ASCII.GetString(Data(0))
                    Case "connection.disconnect"
                        Exit While
                    Case "update.metadata.get"
                        Dim Result As String = System.Text.ASCIIEncoding.ASCII.GetString(Data(1))
                        If Result = "0" Then
                            Dim CheckerThread As New Threading.Thread(Sub()

                                                                          ProgramUpdateState = CheckForProgramUpdates(Data(2))

                                                                          If ProgramUpdateState.Length > 1 Then
                                                                              RaiseEvent UpdateCheckCompleted(Me, True)
                                                                          Else
                                                                              RaiseEvent UpdateCheckCompleted(Me, False)
                                                                          End If


                                                                      End Sub) : CheckerThread.Start()

                        ElseIf Result = "1" Then

                        End If
                    Case "update.metastate.get"
                        Dim MS As New IO.MemoryStream(Data(1))
                        ProgramUpdateState = MetadataGenerator.ReadMetastateData(MS)
                        MS.Dispose()


                    Case "update.file.select"

                End Select
            End If
            CommandLimiter.Limit()
        End While
        Client.Close()
        If ProgramUpdateState IsNot Nothing Then
            'TODO: Add code for saving Program update state
        End If
        RaiseEvent ClientDisconnected(Me, Client)
    End Sub
    Private Function CheckForProgramUpdates(Data As Byte()) As MetadataGenerator.MetaStateObject()
        Dim ServerDataInput As New IO.MemoryStream(Data)
        Dim ServerMetaEntries As MetadataGenerator.MetaObject() = MetadataGenerator.ReadMetadata(ServerDataInput)
        ServerDataInput.Dispose()

        If IO.Directory.Exists("./Programs/" + ServerMetaEntries(0).ProgramName) = False Then IO.Directory.CreateDirectory(".\Programs\" + ServerMetaEntries(0).ProgramName)

        Dim LocalMetaEntries As MetadataGenerator.MetaObject() = UpdateCore.MetadataGenerator.GenerateMetadata(".\Programs\" + ServerMetaEntries(0).ProgramName, ServerMetaEntries(0).ProgramName, ServerMetaEntries(0).ProgramID)

        Dim ItemsToFetch As New List(Of MetadataGenerator.MetaObject)
        Dim ItemsToRemove As New List(Of MetadataGenerator.MetaObject)

        For ServerMetaIndex = 1 To ServerMetaEntries.Length - 1
            Dim EntryFound As Boolean = False
            For LocalMetaIndex = 1 To LocalMetaEntries.Length - 1
                If ServerMetaEntries(ServerMetaIndex).RelativePath = LocalMetaEntries(LocalMetaIndex).RelativePath Then
                    If IsEqual(ServerMetaEntries(ServerMetaIndex).SHA256Hash, LocalMetaEntries(LocalMetaIndex).SHA256Hash) = False Then
                        ItemsToRemove.Add(LocalMetaEntries(LocalMetaIndex))

                        EntryFound = False
                    Else
                        EntryFound = True
                    End If
                    Exit For
                End If
            Next
            If EntryFound = False Then ItemsToFetch.Add(ServerMetaEntries(ServerMetaIndex))
        Next

        For LocalMetaIndex = 1 To LocalMetaEntries.Length - 1
            Dim EntryFound As Boolean = False
            For ServerMetaIndex = 1 To ServerMetaEntries.Length - 1
                If ServerMetaEntries(ServerMetaIndex).RelativePath = LocalMetaEntries(LocalMetaIndex).RelativePath Then
                    EntryFound = True
                    Exit For
                End If
            Next
            If EntryFound = False Then ItemsToRemove.Add(LocalMetaEntries(LocalMetaIndex))
        Next
        Dim RemoveList As New List(Of MetadataGenerator.MetaObject)
        Dim AddList As New List(Of MetadataGenerator.MetaObject)
        Dim StateList As New List(Of MetadataGenerator.MetaStateObject)
        StateList.Add(New MetadataGenerator.MetaStateObject With {.IsHeader = True, .MetaObject = ServerMetaEntries(0)})
        For Each x In ItemsToFetch
            Dim State As New MetadataGenerator.MetaStateObject
            State.MetaObject = x
            State.IsCompleted = False
            State.PendingActionType = MetadataGenerator.ActionType.CreateOrDownload
            StateList.Add(State)
        Next

        For Each x In ItemsToRemove
            Dim State As New MetadataGenerator.MetaStateObject
            State.MetaObject = x
            State.IsCompleted = False
            State.PendingActionType = MetadataGenerator.ActionType.DestroyOrRemove
            StateList.Add(State)
        Next
        Return StateList.ToArray

    End Function
    Private Sub DownloadUpdates()

    End Sub
    Private Shared Function IsEqual(ByVal a As Byte(), ByVal b As Byte()) As Boolean
        Dim x As Integer = a.Length Xor b.Length
        Dim i As Integer = 0

        While i < a.Length AndAlso i < b.Length
            x = x Or a(i) Xor b(i)
            i += 1
        End While

        Return x = 0
    End Function
End Class
