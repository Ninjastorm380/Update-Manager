Public Class MetadataGenerator
    Public Delegate Sub ProgressCompletedCallback(ByRef MetaObjects As MetaObject())
    Public Delegate Sub ProgressChangedCallback(ByVal Current As Integer, ByVal Maximum As Integer)
    Public Structure MetaObject

        Public IsHeader As Boolean
        Public ProgramName As String
        Public ProgramID As String
        Public BasePath As String
        Public RelativePath As String
        Public SHA256Hash As Byte()
        Public Size As Long
        Public IsDirectory As Boolean
    End Structure
    Public Enum ActionType
        CreateOrDownload = 0
        DestroyOrRemove = 1
    End Enum
    Public Structure MetaStateObject
        Public MetaObject As MetaObject
        Public IsCompleted As Boolean
        Public PendingActionType As ActionType
        Public IsHeader As Boolean
    End Structure

    Public Shared Sub GenerateMetadataAsync(SourceDirectory As String, ProgramName As String, ProgramID As String, Completed As ProgressCompletedCallback, Changed As ProgressChangedCallback)
        Dim AsyncThread As New Threading.Thread(Sub()
                                                    Dim SHAEngine As New System.Security.Cryptography.SHA256Managed
                                                    Dim FSEntries As New List(Of String)
                                                    FSEntries.AddRange(IO.Directory.GetDirectories(SourceDirectory, "*", IO.SearchOption.AllDirectories))
                                                    FSEntries.AddRange(IO.Directory.GetFiles(SourceDirectory, "*", IO.SearchOption.AllDirectories))
                                                    Dim SeperatorIndex As Integer = Array.LastIndexOf(SourceDirectory.Split({"\"c}), IO.Path.GetFileName(SourceDirectory)) + 2
                                                    Dim CurrentProgress As Integer = 0
                                                    Dim TotalProgress As Integer = FSEntries.Count
                                                    Dim position As Integer = 0
                                                    Dim SHAEngineBufferSize As Integer = 1048576

                                                    Dim MetaEntries As New List(Of MetaObject)
                                                    Dim MetaHeaderEntry As New MetaObject
                                                    MetaHeaderEntry.IsHeader = True
                                                    MetaHeaderEntry.BasePath = SourceDirectory
                                                    MetaHeaderEntry.ProgramName = ProgramName
                                                    MetaHeaderEntry.ProgramID = ProgramID
                                                    MetaEntries.Add(MetaHeaderEntry)

                                                    For Each x In FSEntries
                                                        Dim EntryPath As String = x
                                                        Dim RelativePath As String = EntryPath.Split({"\"c}, SeperatorIndex)(SeperatorIndex - 1)
                                                        Dim AttributeInfo As IO.FileAttributes = System.IO.File.GetAttributes(EntryPath)
                                                        Dim IsDirectory As Boolean = (IO.File.GetAttributes(EntryPath) And IO.FileAttributes.Directory) = IO.FileAttributes.Directory
                                                        Dim Size As Long = 0 : If IsDirectory = False Then Size = New IO.FileInfo(EntryPath).Length
                                                        Dim SHAHash As Byte() = {} : If IsDirectory = False Then Dim SHAStream As New IO.FileStream(EntryPath, IO.FileMode.Open, IO.FileAccess.Read, IO.FileShare.None, SHAEngineBufferSize) : SHAHash = SHAEngine.ComputeHash(SHAStream) : SHAStream.Dispose()

                                                        Dim MO As New MetaObject
                                                        MO.IsHeader = False
                                                        MO.IsDirectory = IsDirectory
                                                        MO.RelativePath = RelativePath
                                                        MO.SHA256Hash = SHAHash
                                                        MO.Size = Size
                                                        MetaEntries.Add(MO)
                                                        CurrentProgress += 1
                                                        Dim ChangedInvokeThread As New Threading.Thread(Sub() Changed.Invoke(CurrentProgress, TotalProgress)) : ChangedInvokeThread.Start()
                                                    Next
                                                    SHAEngine.Dispose()
                                                    Dim CompletedInvokeThread As New Threading.Thread(Sub() Completed.Invoke(MetaEntries.ToArray)) : CompletedInvokeThread.Start()
                                                End Sub)
        AsyncThread.Start()
    End Sub
    Public Shared Function GenerateMetadata(SourceDirectory As String, ProgramName As String, ProgramID As String, Optional Completed As ProgressCompletedCallback = Nothing, Optional Changed As ProgressChangedCallback = Nothing) As MetaObject()

        Dim SHAEngine As New System.Security.Cryptography.SHA256Managed
        Dim FSEntries As New List(Of String)
        FSEntries.AddRange(IO.Directory.GetDirectories(SourceDirectory, "*", IO.SearchOption.AllDirectories))
        FSEntries.AddRange(IO.Directory.GetFiles(SourceDirectory, "*", IO.SearchOption.AllDirectories))
        Dim SeperatorIndex As Integer = Array.LastIndexOf(SourceDirectory.Split({"\"c}), IO.Path.GetFileName(SourceDirectory)) + 2
        Dim CurrentProgress As Integer = 0
        Dim TotalProgress As Integer = FSEntries.Count
        Dim position As Integer = 0
        Dim SHAEngineBufferSize As Integer = 1048576

        Dim MetaEntries As New List(Of MetaObject)
        Dim MetaHeaderEntry As New MetaObject
        MetaHeaderEntry.IsHeader = True
        MetaHeaderEntry.BasePath = SourceDirectory
        MetaHeaderEntry.ProgramName = ProgramName
        MetaHeaderEntry.ProgramID = ProgramID
        MetaEntries.Add(MetaHeaderEntry)

        For Each x In FSEntries
            Dim EntryPath As String = x
            Dim RelativePath As String = EntryPath.Split({"\"c}, SeperatorIndex)(SeperatorIndex - 1)
            Dim AttributeInfo As IO.FileAttributes = System.IO.File.GetAttributes(EntryPath)
            Dim IsDirectory As Boolean = (IO.File.GetAttributes(EntryPath) And IO.FileAttributes.Directory) = IO.FileAttributes.Directory
            Dim Size As Long = 0 : If IsDirectory = False Then Size = New IO.FileInfo(EntryPath).Length
            Dim SHAHash As Byte() = {} : If IsDirectory = False Then Dim SHAStream As New IO.FileStream(EntryPath, IO.FileMode.Open, IO.FileAccess.Read, IO.FileShare.None, SHAEngineBufferSize) : SHAHash = SHAEngine.ComputeHash(SHAStream) : SHAStream.Dispose()

            Dim MO As New MetaObject
            MO.IsHeader = False
            MO.IsDirectory = IsDirectory
            MO.RelativePath = RelativePath
            MO.SHA256Hash = SHAHash
            MO.Size = Size
            MetaEntries.Add(MO)
            CurrentProgress += 1
            If Changed IsNot Nothing Then Dim ChangedInvokeThread As New Threading.Thread(Sub() Changed.Invoke(CurrentProgress, TotalProgress)) : ChangedInvokeThread.Start()
        Next
        SHAEngine.Dispose()
        Dim MetaArray As MetaObject() = MetaEntries.ToArray
        If Completed IsNot Nothing Then Dim CompletedInvokeThread As New Threading.Thread(Sub() Completed.Invoke(MetaArray)) : CompletedInvokeThread.Start()
        Return MetaArray
    End Function
    Public Shared Function WriteMetadata(Input As MetaObject(), Stream As IO.Stream)
        Dim ReadiedData(Input.Length - 1)() As Byte
        ReadiedData(0) = Serialization.SerializeArray({System.Text.ASCIIEncoding.ASCII.GetBytes(Input(0).BasePath),
                                                       System.Text.ASCIIEncoding.ASCII.GetBytes(Input(0).ProgramName),
                                                       System.Text.ASCIIEncoding.ASCII.GetBytes(Input(0).ProgramID)})

        For x = 1 To Input.Length - 1
            Dim Data As Byte() = UpdateCore.Serialization.SerializeArray({BitConverter.GetBytes(Input(x).IsDirectory),
                                 System.Text.ASCIIEncoding.ASCII.GetBytes(Input(x).RelativePath),
                                 BitConverter.GetBytes(Input(x).Size),
                                 Input(x).SHA256Hash})
            ReadiedData(x) = Data
        Next
        Dim Output As Byte() = UpdateCore.Serialization.SerializeArray(ReadiedData)
        Stream.Write(Output, 0, Output.Length)
        Stream.Flush()
    End Function
    Public Shared Function ReadMetadata(Stream As IO.Stream) As MetaObject()
        Dim Input(Stream.Length - 1) As Byte
        Stream.Read(Input, 0, Input.Length)
        Dim MetaByteArrays As Byte()() = Serialization.DeserializeArray(Input)
        Dim MetaList As New List(Of MetaObject)
        Dim MetaHeaderEntryBytes As Byte()() = Serialization.DeserializeArray(MetaByteArrays(0))

        Dim MetaHeaderEntry As New MetaObject
        MetaHeaderEntry.BasePath = System.Text.ASCIIEncoding.ASCII.GetString(MetaHeaderEntryBytes(0))
        MetaHeaderEntry.ProgramName = System.Text.ASCIIEncoding.ASCII.GetString(MetaHeaderEntryBytes(1))
        MetaHeaderEntry.ProgramID = System.Text.ASCIIEncoding.ASCII.GetString(MetaHeaderEntryBytes(2))
        MetaList.Add(MetaHeaderEntry)
        For x = 1 To MetaByteArrays.Length - 1
            Dim MetaEntryBytes As Byte()() = Serialization.DeserializeArray(MetaByteArrays(x))
            Dim MetaEntry As New MetaObject
            MetaEntry.IsDirectory = BitConverter.ToBoolean(MetaEntryBytes(0), 0)
            MetaEntry.RelativePath = System.Text.ASCIIEncoding.ASCII.GetString(MetaEntryBytes(1))
            MetaEntry.Size = BitConverter.ToInt64(MetaEntryBytes(2), 0)
            MetaEntry.SHA256Hash = MetaEntryBytes(3)
            MetaEntry.IsHeader = False
            MetaList.Add(MetaEntry)
        Next
        Return MetaList.ToArray
    End Function
    Public Shared Sub WriteMetastateData(Input As MetaStateObject(), Stream As IO.Stream)
        If Input.Length > 2 Then
            Dim ReadiedData(Input.Length - 1)() As Byte

            ReadiedData(0) = Serialization.SerializeArray({System.Text.ASCIIEncoding.ASCII.GetBytes(Input(0).MetaObject.BasePath),
                                                           System.Text.ASCIIEncoding.ASCII.GetBytes(Input(0).MetaObject.ProgramName),
                                                           System.Text.ASCIIEncoding.ASCII.GetBytes(Input(0).MetaObject.ProgramID)})
            For x = 1 To Input.Length - 1
                Dim MetaObjectData As Byte() = UpdateCore.Serialization.SerializeArray({BitConverter.GetBytes(Input(x).MetaObject.IsDirectory),
                                               System.Text.ASCIIEncoding.ASCII.GetBytes(Input(x).MetaObject.RelativePath),
                                               BitConverter.GetBytes(Input(x).MetaObject.Size),
                                               Input(x).MetaObject.SHA256Hash})


                Dim Data As Byte() = UpdateCore.Serialization.SerializeArray({MetaObjectData, BitConverter.GetBytes(Input(x).PendingActionType), BitConverter.GetBytes(Input(x).IsCompleted)})

                ReadiedData(x) = Data
            Next
            Dim Output As Byte() = UpdateCore.Serialization.SerializeArray(ReadiedData)
            Stream.Write(Output, 0, Output.Length)
            Stream.Flush()
        End If
    End Sub
    Public Shared Function ReadMetastateData(Stream As IO.Stream) As MetaStateObject()
        Dim Input(Stream.Length - 1) As Byte
        Stream.Read(Input, 0, Input.Length)
        Dim MetaByteArrays As Byte()() = Serialization.DeserializeArray(Input)
        Dim MetaList(MetaByteArrays.Count - 1) As MetaStateObject
        Dim MetaHeaderEntryBytes As Byte()() = Serialization.DeserializeArray(MetaByteArrays(0))

        Dim MetaHeaderEntry As New MetaObject
        MetaHeaderEntry.BasePath = System.Text.ASCIIEncoding.ASCII.GetString(MetaHeaderEntryBytes(0))
        MetaHeaderEntry.ProgramName = System.Text.ASCIIEncoding.ASCII.GetString(MetaHeaderEntryBytes(1))
        MetaHeaderEntry.ProgramID = System.Text.ASCIIEncoding.ASCII.GetString(MetaHeaderEntryBytes(2))
        MetaHeaderEntry.IsHeader = True
        Dim MetastateObjectHeader As New MetaStateObject
        MetastateObjectHeader.IsHeader = True
        MetastateObjectHeader.MetaObject = MetaHeaderEntry
        MetaList(0) = MetastateObjectHeader
        For x = 1 To MetaByteArrays.Length - 1
            Dim MetaStateEntryBytes As Byte()() = Serialization.DeserializeArray(MetaByteArrays(x))
            Dim MetaEntryBytes As Byte()() = Serialization.DeserializeArray(MetaStateEntryBytes(0))

            Dim MetaStateEntry As New MetaStateObject
            Dim MetaEntry As New MetaObject

            MetaEntry.IsHeader = False
            MetaEntry.IsDirectory = BitConverter.ToBoolean(MetaEntryBytes(0), 0)
            MetaEntry.RelativePath = System.Text.ASCIIEncoding.ASCII.GetString(MetaEntryBytes(1))
            MetaEntry.Size = BitConverter.ToInt64(MetaEntryBytes(2), 0)
            MetaEntry.SHA256Hash = MetaEntryBytes(3)

            MetaStateEntry.MetaObject = MetaEntry
            MetaStateEntry.PendingActionType = BitConverter.ToInt32(MetaStateEntryBytes(1), 0)
            MetaStateEntry.IsCompleted = BitConverter.ToBoolean(MetaStateEntryBytes(2), 0)

            MetaList(x) = MetaStateEntry
        Next
        Return MetaList
    End Function
End Class
