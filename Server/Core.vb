



Public Delegate Sub ClientConnectedEventHandler(Client As Net.Sockets.TcpClient)
Friend Module SharedItems
    Public PairedConnectionDictionary As New Dictionary(Of String, PairedConnection)
End Module
Public Class PairedConnection
    Dim Server As Core
    Dim PingClient As Cryptography.TcpClient
    Dim DataClient As Cryptography.TcpClient
    Dim PingReader As Cryptography.StreamReader
    Dim PingWriter As Cryptography.StreamWriter
    Dim DataReader As Cryptography.StreamReader
    Dim DataWriter As Cryptography.StreamWriter
    Dim RemoteEndpoint As Net.IPEndPoint
    Dim RemoteAddress As String = ""
    Dim Connected As Boolean = True
    Sub New(ServerCore As Core)
        Server = ServerCore
    End Sub
    Public ReadOnly Property Address As String
        Get
            Return RemoteAddress
        End Get
    End Property
    Sub InitalizeData(Client As Cryptography.TcpClient)
        RemoteEndpoint = Client.Client.RemoteEndPoint
        RemoteAddress = RemoteEndpoint.Address.ToString + ":" + RemoteEndpoint.Port.ToString
        DataClient = Client
        DataReader = New Cryptography.StreamReader(DataClient)
        DataWriter = New Cryptography.StreamWriter(DataClient)
        Dim Thread As New Threading.Thread(AddressOf DataThread) : Thread.Start()
    End Sub
    Sub InitalizePing(Client As Cryptography.TcpClient)
        PingClient = Client
        PingReader = New Cryptography.StreamReader(PingClient)
        PingWriter = New Cryptography.StreamWriter(PingClient)
        Dim Thread As New Threading.Thread(AddressOf PingThread) : Thread.Start()
    End Sub

    Private Sub PingThread()
        Debug.Print("ping start")
        While Connected = True And Server.Running = True
            Try
                PingWriter.WriteStream(SerializeArray({"server.connections.ping"}))
                Connected = True
            Catch ex As Exception
                Exit While
            End Try
            Threading.Thread.Sleep(100)
        End While
        Connected = False
        If PairedConnectionDictionary.ContainsKey(RemoteAddress) = True Then
            PairedConnectionDictionary.Remove(RemoteAddress)
        End If
        Debug.Print("ping end")
    End Sub
    Private Sub DataThread()
        While Server.Running = True
            If Connected = True Then
                If DataReader.EndOfStream = False Then
                    Dim Data As String() = DeserializeArray(DataReader.ReadStream)
                    Select Case Data(0)
                        Case "server.connections.disconnect"
                            Connected = False
                            Exit While
                    End Select
                End If
            Else
                Exit While
            End If
            Threading.Thread.Sleep(10)
        End While
        Try : DataWriter.WriteStream(SerializeArray({"server.connections.disconnect.return", "0"})) : Catch : End Try
        Try : PingClient.Close() : Catch : End Try
        Try : PingReader.Close() : Catch : End Try
        Try : PingWriter.Close() : Catch : End Try
        Try : DataClient.Close() : Catch : End Try
        Try : DataReader.Close() : Catch : End Try
        Try : DataWriter.Close() : Catch : End Try
    End Sub

End Class

Public Class Core : Inherits ServerBase
    Private Sub Connected(Client As Net.Sockets.TcpClient) Handles Me.ClientConnected
        Dim Thread As New Threading.Thread(Sub() Gateway(New Cryptography.TcpClient(Client, "f1G5S#8gvs@")))
        Thread.Name = "Update server gateway thread"
        Thread.Start()
    End Sub
    Private Sub Gateway(Client As Cryptography.TcpClient)
        Dim Reader As New Cryptography.StreamReader(Client)
        Dim Writer As New Cryptography.StreamWriter(Client)
        Dim ModuleRunning As Boolean = True : Dim Connected As Boolean = True
        Dim RemoteEndpoint As Net.IPEndPoint = Client.Client.RemoteEndPoint
        Dim RemoteAddress As String = RemoteEndpoint.Address.ToString + ":" + RemoteEndpoint.Port.ToString
        Dim PingThread As New Threading.Thread(Sub()
                                                   While Connected = True And ModuleRunning = True
                                                       Try
                                                           Writer.WriteStream(SerializeArray({"server.connections.ping"}))
                                                           Connected = True
                                                       Catch ex As Exception
                                                           Connected = False
                                                           Exit While
                                                       End Try
                                                       Threading.Thread.Sleep(100)
                                                   End While
                                               End Sub) : PingThread.Start()
        Dim ModuleThread As New Threading.Thread(Sub()
                                                     While Running = True And ModuleRunning = True
                                                         If Connected = True Then
                                                             If Reader.EndOfStream = False Then
                                                                 Dim Data As String() = DeserializeArray(Reader.ReadStream)

                                                                 Select Case Data(0)
                                                                     Case "connection.mode.set"
                                                                         Debug.Print("connection.mode.set")
                                                                         If Data(1) = "0" Then
                                                                             Debug.Print("data")
                                                                             Dim NewConnection As New PairedConnection(Me)
                                                                             NewConnection.InitalizeData(Client)
                                                                             PairedConnectionDictionary.Add(NewConnection.Address, NewConnection)
                                                                             Writer.WriteStream(SerializeArray({"connection.mode.set", "0", "0", NewConnection.Address}))
                                                                             ModuleRunning = False
                                                                             Exit While
                                                                         ElseIf Data(1) = "1" Then
                                                                             Debug.Print("ping")
                                                                             If PairedConnectionDictionary.ContainsKey(Data(2)) = True Then
                                                                                 Debug.Print("data exists")
                                                                                 Dim Connection As PairedConnection = PairedConnectionDictionary(Data(2))
                                                                                 Connection.InitalizePing(Client)
                                                                                 Writer.WriteStream(SerializeArray({"connection.mode.set", "0", "1"}))
                                                                                 ModuleRunning = False
                                                                                 Exit While
                                                                             Else
                                                                                 Debug.Print("data doesn't exists")

                                                                                 Writer.WriteStream(SerializeArray({"connection.mode.set", "1"}))
                                                                             End If
                                                                         Else
                                                                             Debug.Print("command error")

                                                                             Writer.WriteStream(SerializeArray({"connection.mode.set", "2"}))
                                                                         End If
                                                                 End Select
                                                             End If
                                                         Else
                                                             ModuleRunning = False
                                                             Exit While
                                                         End If
                                                         Threading.Thread.Sleep(10)
                                                     End While
                                                 End Sub) : ModuleThread.Start()
    End Sub
End Class
Public MustInherit Class ServerBase
    Private listener As Net.Sockets.TcpListener
    Private ListenerThread As Threading.Thread
    Private IsRunning As Boolean
    Public Event ClientConnected As ClientConnectedEventHandler
    Public ReadOnly Property Running() As Boolean
        Get
            Return Me.IsRunning
        End Get
    End Property

    Protected Sub New()
        Me.IsRunning = False
    End Sub
    Public Sub Start(Port As Integer)
        Me.listener = New Net.Sockets.TcpListener(Port)
        Me.ListenerThread = New Threading.Thread(AddressOf ListenerSub)
        Me.ListenerThread.Name = "GDSM Server Connection Broker"
        Me.IsRunning = True
        Me.listener.Start()
        Me.ListenerThread.Start()
    End Sub
    Public Sub [Stop]()
        Me.IsRunning = False
        Me.listener.Stop()
        Me.listener = Nothing
        Me.ListenerThread.Abort()
        Me.ListenerThread = Nothing
    End Sub
    Private Sub ListenerSub()
        While IsRunning : If listener.Pending() Then : Dim thread As New Threading.Thread(Sub() RaiseEvent ClientConnected(Me.listener.AcceptTcpClient)) : thread.Start() : End If
            Threading.Thread.Sleep(10)
        End While
    End Sub
End Class
Public Class Cryptography
    Public Class TcpClient : Inherits Net.Sockets.TcpClient
        Private CryptographicKey As String
        Sub New(Client As Net.Sockets.TcpClient, Key As String)
            MyBase.New()
            Me.Client = Client.Client
            CryptographicKey = Key
        End Sub
        Sub New(IP As String, Port As String, Key As String)
            MyBase.New(IP, Port)
            CryptographicKey = Key
        End Sub
        Function Getkey() As String
            Return CryptographicKey
        End Function
        Shadows Function Connected(PingData As String()) As Boolean
            Try
                Dim writer As New Cryptography.StreamWriter(Me.GetStream, CryptographicKey)
                writer.WriteStream(SerializeArray(PingData))
                Return True
            Catch
                Return False
            End Try
        End Function
        Shadows ReadOnly Property EndOfStream() As Boolean
            Get
                Return Not CType(Me.GetStream, Net.Sockets.NetworkStream).DataAvailable
            End Get
        End Property
    End Class
    Public Class StreamReader : Inherits IO.StreamReader
        Private CryptographicKey As String
        Sub New(S As IO.Stream, Key As String)
            MyBase.New(S)
            CryptographicKey = Key
        End Sub
        Sub New(CrypticClient As Cryptography.TcpClient)
            MyBase.New(CrypticClient.GetStream)
            CryptographicKey = CrypticClient.Getkey
        End Sub
        Function ReadStream() As String
            Dim Output As String = Decrypt(DeserializeString(Me.ReadLine), CryptographicKey)
            Return Output
        End Function
        Shadows ReadOnly Property EndOfStream() As Boolean
            Get
                Return Not CType(Me.BaseStream, Net.Sockets.NetworkStream).DataAvailable
            End Get
        End Property
    End Class
    Public Class StreamWriter : Inherits IO.StreamWriter
        Private CryptographicKey As String
        Sub New(s As IO.Stream, Key As String)
            MyBase.New(s)
            CryptographicKey = Key
        End Sub
        Sub New(CrypticClient As Cryptography.TcpClient)
            MyBase.New(CrypticClient.GetStream)
            CryptographicKey = CrypticClient.Getkey
        End Sub
        Sub WriteStream(Data As String)
            Me.WriteLine(SerializeString(Encrypt(Data, CryptographicKey)))
            Me.Flush()
        End Sub
        Shadows ReadOnly Property EndOfStream() As Boolean
            Get
                Return Not CType(Me.BaseStream, Net.Sockets.NetworkStream).DataAvailable
            End Get
        End Property
    End Class
    Public Shared Function Encrypt(ByVal UnencryptedData As String, ByVal Key As String) As String
        Dim AES As New System.Security.Cryptography.RijndaelManaged
        Dim SHA256 As System.Security.Cryptography.SHA256Managed
        AES.GenerateIV()
        SHA256 = System.Security.Cryptography.HashAlgorithm.Create("SHA256")
        AES.Key = SHA256.ComputeHash(System.Text.ASCIIEncoding.ASCII.GetBytes(Key))
        AES.Mode = Security.Cryptography.CipherMode.CBC
        Dim DESEncrypter As System.Security.Cryptography.ICryptoTransform = AES.CreateEncryptor
        Dim Buffer As Byte() = System.Text.ASCIIEncoding.ASCII.GetBytes(UnencryptedData)
        Return Convert.ToBase64String(AES.IV) + "|" + Convert.ToBase64String(DESEncrypter.TransformFinalBlock(Buffer, 0, Buffer.Length))
    End Function
    Public Shared Function Decrypt(ByVal EncryptedData As String, ByVal Key As String) As String
        Dim AES As New System.Security.Cryptography.RijndaelManaged
        Dim SHA256 As System.Security.Cryptography.SHA256Managed

        Dim ivct = EncryptedData.Split({"|"}, 2, StringSplitOptions.None)
        Dim iv As String = ivct(0)
        EncryptedData = ivct(1)
        SHA256 = System.Security.Cryptography.HashAlgorithm.Create("SHA256")
        AES.Key = SHA256.ComputeHash(System.Text.ASCIIEncoding.ASCII.GetBytes(Key))
        AES.IV = Convert.FromBase64String(iv)
        AES.Mode = Security.Cryptography.CipherMode.CBC
        Dim DESDecrypter As System.Security.Cryptography.ICryptoTransform = AES.CreateDecryptor

        Dim Buffer As Byte() = Convert.FromBase64String(EncryptedData)

        Return System.Text.ASCIIEncoding.ASCII.GetString(DESDecrypter.TransformFinalBlock(Buffer, 0, Buffer.Length))
    End Function
End Class
Module Serialization
    Public Function SerializeString(Data As String) As String
        Return Convert.ToBase64String(System.Text.ASCIIEncoding.ASCII.GetBytes(Data))
    End Function
    Public Function DeserializeString(Data As String) As String
        Return System.Text.ASCIIEncoding.ASCII.GetString(Convert.FromBase64String(Data))
    End Function

    Public Function SerializeArray(Data As String()) As String
        Dim ConsolidatedString As String = ""
        For Each x In Data
            ConsolidatedString += SerializeString(x) + ":"
        Next
        Return SerializeString(ConsolidatedString.Remove(ConsolidatedString.Count - 1, 1))
    End Function
    Public Function DeserializeArray(Data As String) As String()
        Dim DataArray() As String = DeserializeString(Data).Split(":")
        For x = 0 To DataArray.Length - 1
            DataArray(x) = DeserializeString(DataArray(x))
        Next
        Return DataArray
    End Function









End Module