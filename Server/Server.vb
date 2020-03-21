Public Delegate Sub ClientConnectedEventHandler(Client As Net.Sockets.TcpClient)
Public Delegate Sub UpdateLogEventHandler(ByVal Message As String)
Public Class Core : Inherits ServerBase
    Public Shared Property ServerMetadataSaveDirectory As String
    Public Shared Property ServerUpdateFilesDirectory As String
    Public Event UpdateLog As UpdateLogEventHandler

    Private Sub Connected(Client As Net.Sockets.TcpClient) Handles Me.ClientConnected
        Dim Thread As New Threading.Thread(Sub() ServerMain(New Cryptography.TcpClient(Client, "f1G5S#8gvs@")))
        Thread.Name = "Update server thread"
        Thread.Start()
    End Sub
    Private Sub ServerMain(Client As Cryptography.TcpClient)
        Dim Reader As New Cryptography.StreamReader(Client)
        Dim Writer As New Cryptography.StreamWriter(Client)
        Dim RemoteEndpoint As Net.IPEndPoint = Client.Client.RemoteEndPoint
        Dim RemoteAddress As String = RemoteEndpoint.Address.ToString + ":" + RemoteEndpoint.Port.ToString
        Dim TTL As Integer = 30 : Dim PingMode As Boolean = True : Dim Connected As Boolean = True : Dim PingThread As New Threading.Thread(Sub()
                                                                                                                                                While Connected = True
                                                                                                                                                    If PingMode = True Then
                                                                                                                                                        Try
                                                                                                                                                            Writer.WriteStream(SerializeArray({"server.connections.ping"}))
                                                                                                                                                            Connected = True
                                                                                                                                                        Catch ex As Exception
                                                                                                                                                            Connected = False
                                                                                                                                                            Exit While
                                                                                                                                                        End Try
                                                                                                                                                    Else
                                                                                                                                                        TTL -= 1
                                                                                                                                                        If TTL = 0 Then
                                                                                                                                                            Connected = False
                                                                                                                                                            Exit While
                                                                                                                                                        End If
                                                                                                                                                    End If
                                                                                                                                                    Threading.Thread.Sleep(1000)
                                                                                                                                                End While
                                                                                                                                            End Sub) : PingThread.Start()

        RaiseEvent UpdateLog(Date.Now.ToShortDateString + " @ " + Date.Now.ToShortTimeString + ": Client[" + RemoteAddress + "] connected")
        While Running = True
            If Connected = True Then
                If Reader.EndOfStream = False Then
                    TTL = 30
                    If PingMode = False Then
                        Writer.WriteStream(SerializeArray({"server.connections.command.acknowledged", "0"}))
                    End If
                    Dim Data As String() = DeserializeArray(Reader.ReadStream)
                    Select Case Data(0)
                        Case "server.connections.ping"
                        Case "server.connections.disconnect"
                            Exit While
                    End Select
                End If
            Else
                Exit While
            End If
            Threading.Thread.Sleep(10)
        End While
        Try : Writer.WriteStream(SerializeArray({"server.connections.disconnect.return", "0"})) : Catch : End Try
        Try : Client.Close() : Catch : End Try
        Try : Reader.Close() : Catch : End Try
        Try : Writer.Close() : Catch : End Try
        RaiseEvent UpdateLog(Date.Now.ToShortDateString + " @ " + Date.Now.ToShortTimeString + ": Client[" + RemoteAddress + "] disconnected")
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
            Return Decrypt(DeserializeString(Me.ReadLine), CryptographicKey)
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