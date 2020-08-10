Public Class Client
    Private WithEvents Client As Networking.QueuedTcpClient
    Public Event ClientConnected As EventHandler(Of Networking.QueuedTcpClient)
    Public Event ClientDisconnected As EventHandler(Of Networking.QueuedTcpClient)
    Public Event ConnectionFailedEvent As EventHandler(Of System.Net.Sockets.SocketException)
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
        If Client IsNot Nothing AndAlso Client.Connected = True Then Client.Write("COMMAND", Serialization.SerializeArray({System.Text.ASCIIEncoding.ASCII.GetBytes("connection.disconnect")}))
    End Sub
    Public Sub GetMetadataAsync(ID As String)
        Client.Write("COMMAND", Serialization.SerializeArray({System.Text.ASCIIEncoding.ASCII.GetBytes("update.metadata.get"),
                                                              System.Text.ASCIIEncoding.ASCII.GetBytes(ID)}))
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
                            Debug.Print("metadata received.")
                            Serialization.DeserializeArray(Data(2))
                        ElseIf Result = "1" Then
                            Debug.Print("program does not exist!")
                        End If
                End Select
            End If
            CommandLimiter.Limit()
        End While
        Client.Close()
        RaiseEvent ClientDisconnected(Me, Client)
    End Sub

End Class
