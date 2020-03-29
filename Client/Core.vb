Public Class Core
    Public Event ConnectionFailed As EventHandler(Of Exception)
    Dim PingClient As Cryptography.TcpClient
    Dim DataClient As Cryptography.TcpClient
    Dim PingReader As Cryptography.StreamReader
    Dim PingWriter As Cryptography.StreamWriter
    Dim DataReader As Cryptography.StreamReader
    Dim DataWriter As Cryptography.StreamWriter
    Dim Connected As Boolean = False
    Dim IsRunning As Boolean = False
    Public ReadOnly Property Running As Boolean
        Get
            Return IsRunning
        End Get
    End Property
    Sub Connect(Address As String)
        Dim InvokeThread As New Threading.Thread(Sub()
                                                     Try
                                                         Dim AddressData As String() = Address.Split(":")
                                                         PingClient = New Cryptography.TcpClient(AddressData(0), AddressData(1), "f1G5S#8gvs@")
                                                         DataClient = New Cryptography.TcpClient(AddressData(0), AddressData(1), "f1G5S#8gvs@")
                                                         PingReader = New Cryptography.StreamReader(PingClient)
                                                         PingWriter = New Cryptography.StreamWriter(PingClient)
                                                         DataReader = New Cryptography.StreamReader(DataClient)
                                                         DataWriter = New Cryptography.StreamWriter(DataClient)
                                                         Dim InitOK As Boolean = False
                                                         Dim Output As String() = {}
                                                         DataWriter.WriteStream(SerializeArray({"connection.mode.set", "0"}))
                                                         Do While True
                                                             If DataReader.EndOfStream = False Then
                                                                 Output = DeserializeArray(DataReader.ReadStream)
                                                                 If Output(0) = "connection.mode.set" Then
                                                                     InitOK = True
                                                                     Exit Do
                                                                 Else
                                                                     InitOK = False
                                                                     Threading.Thread.Sleep(10)
                                                                 End If
                                                             End If
                                                         Loop
                                                         If Output(0) = "connection.mode.set" AndAlso Output(1) = "0" And InitOK = True Then
                                                             PingWriter.WriteStream(SerializeArray({"connection.mode.set", "1", Output(3)}))
                                                             Do While True
                                                                 If PingReader.EndOfStream = False Then
                                                                     Output = DeserializeArray(PingReader.ReadStream)
                                                                     If Output(0) = "connection.mode.set" Then
                                                                         InitOK = True
                                                                         Exit Do
                                                                     Else
                                                                         InitOK = False
                                                                         Threading.Thread.Sleep(10)
                                                                     End If
                                                                 End If
                                                             Loop
                                                             If Output(0) = "connection.mode.set" AndAlso Output(1) = "0" And InitOK = True Then
                                                                 InitOK = True
                                                             Else
                                                                 InitOK = False
                                                             End If
                                                         Else
                                                             InitOK = False
                                                         End If
                                                         If InitOK = True Then
                                                             IsRunning = True
                                                             Connected = True
                                                             Dim HeartbeatThread As New Threading.Thread(AddressOf PingThread) : HeartbeatThread.Start()
                                                             Dim Thread As New Threading.Thread(AddressOf DataThread) : Thread.Start()
                                                         End If
                                                     Catch ex As Exception
                                                         RaiseEvent ConnectionFailed(Me, ex)
                                                     End Try
                                                 End Sub) : InvokeThread.Start()
    End Sub
    Sub Disconnect()
        If IsRunning = True And Connected = True Then DataWriter.WriteStream(SerializeArray({"server.connections.disconnect"}))
    End Sub
    Private Sub PingThread()
        While Connected = True And IsRunning = True
            Try
                PingWriter.WriteStream(SerializeArray({"server.connections.ping"}))
                Connected = True
            Catch ex As Exception
                Exit While
            End Try
            Threading.Thread.Sleep(100)
        End While
        Connected = False
    End Sub
    Private Sub DataThread()
        DataWriter.WriteStream(SerializeArray({"server.updates.check"}))
        While IsRunning = True
            If Connected = True Then
                If DataReader.EndOfStream = False Then
                    Dim Data As String() = DeserializeArray(DataReader.ReadStream)
                    Select Case Data(0)
                        Case "server.updates.check.return"
                            Debug.Print("check returned " + Data(1))
                        Case "server.connections.disconnect.return"
                            If Data(1) = "0" Then
                                Try : PingClient.Close() : Catch : End Try
                                Try : PingReader.Close() : Catch : End Try
                                Try : PingWriter.Close() : Catch : End Try
                                Try : DataClient.Close() : Catch : End Try
                                Try : DataReader.Close() : Catch : End Try
                                Try : DataWriter.Close() : Catch : End Try
                                IsRunning = False
                                Exit While
                            End If
                    End Select
                End If
            Else
                Try : PingClient.Close() : Catch : End Try
                Try : PingReader.Close() : Catch : End Try
                Try : PingWriter.Close() : Catch : End Try
                Try : DataClient.Close() : Catch : End Try
                Try : DataReader.Close() : Catch : End Try
                Try : DataWriter.Close() : Catch : End Try
                IsRunning = False
                Exit While
            End If
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
