Public Class MetadataGenerator
    Public Delegate Sub ProgressCompletedAsyncCallback()
    Public Delegate Sub ProgressUpdateAsyncCallback(ByVal State As Integer, ByVal Current As Integer, ByVal Maximum As Integer)
    Public Structure MetaObject
        Public BasePath As String
        Public RelativePath As String
        Public SHA256Hash As Byte()
        Public Size As Integer
        Public IsDirectory As Boolean
    End Structure
    Private Class MetaThread
        Private Shared SHAEngine As New System.Security.Cryptography.SHA256Managed
        Private BaseDirectory As String
        Private IsRunning As Boolean = False : Public ReadOnly Property Running As Boolean
            Get
                Return IsRunning
            End Get
        End Property
        Private WorkingSetInput As New List(Of String)
        Private WorkingSetOutput As New List(Of MetaObject)
        Public Sub New(Directory As String)
            If SHAEngine Is Nothing Then SHAEngine = New System.Security.Cryptography.SHA256Managed
            BaseDirectory = Directory
        End Sub
        Public Sub Dispose()
            If SHAEngine IsNot Nothing Then
                SHAEngine.Dispose()
                SHAEngine = Nothing
            End If
            Clear()
        End Sub
        Public Sub Add(Path As String)
            WorkingSetInput.Add(Path)
        End Sub
        Public Sub Run(UpdateCallback As ProgressUpdateAsyncCallback, Optional ThreadLimit As Integer = 250)
            If IsRunning = False Then
                IsRunning = True

                Dim AsyncThread As New Threading.Thread(Sub()
                                                            Dim MetaThreadLimiter As New ThreadLimiter(ThreadLimit)
                                                            Dim SeperatorIndex As Integer = Array.LastIndexOf(BaseDirectory.Split({"\"c}), IO.Path.GetFileName(BaseDirectory)) + 2
                                                            Dim SHAEngineBufferSize As Integer = 1048576
                                                            Do Until WorkingSetInput.Count = 0
                                                                Dim EntryPath As String = WorkingSetInput(0)
                                                                Dim RelativePath As String = EntryPath.Split({"\"c}, SeperatorIndex)(SeperatorIndex - 1)
                                                                Dim AttributeInfo As IO.FileAttributes = System.IO.File.GetAttributes(EntryPath)
                                                                Dim IsDirectory As Boolean = (IO.File.GetAttributes(EntryPath) And IO.FileAttributes.Directory) = IO.FileAttributes.Directory
                                                                Dim Size As Integer = 0 : If IsDirectory = False Then Size = New IO.FileInfo(EntryPath).Length
                                                                Dim SHAHash As Byte() = {} : If IsDirectory = False Then Dim SHAStream As New IO.FileStream(EntryPath, IO.FileMode.Open, IO.FileAccess.Read, IO.FileShare.None, SHAEngineBufferSize) : SHAHash = SHAEngine.ComputeHash(SHAStream) : SHAStream.Dispose()

                                                                Dim MO As New MetaObject
                                                                MO.BasePath = BaseDirectory
                                                                MO.IsDirectory = IsDirectory
                                                                MO.RelativePath = RelativePath
                                                                MO.SHA256Hash = SHAHash
                                                                WorkingSetOutput.Add(MO)
                                                                WorkingSetInput.RemoveAt(0)

                                                                Dim InvokeThread1 As New Threading.Thread(Sub() UpdateCallback.Invoke(0, 0, 0)) : InvokeThread1.Start()
                                                                MetaThreadLimiter.Limit()
                                                            Loop
                                                            IsRunning = False
                                                        End Sub)
                AsyncThread.Start()
            End If
        End Sub
        Public Function GetMetadata() As MetaObject()
            Return WorkingSetOutput.ToArray
        End Function
        Public Sub Clear()
            WorkingSetInput.Clear()
            WorkingSetOutput.Clear()
        End Sub
    End Class
    Public Shared Sub GenerateMetadataAsync(MetaSaveDirectory As String, SourceDirectory As String, ProgramName As String, ProgramID As String, FileName As String,
                                            ProgressUpdateCallback As ProgressUpdateAsyncCallback,
                                            ProgressCompletedCallback As ProgressCompletedAsyncCallback,
                                            Optional ThreadLimit As Integer = 250,
                                            Optional Threads As Integer = 8)
        Dim AsyncThread As New Threading.Thread(Sub()
                                                    If IO.Directory.Exists(MetaSaveDirectory) = False Then IO.Directory.CreateDirectory(MetaSaveDirectory)
                                                    Dim FSEntries As New List(Of String)
                                                    FSEntries.AddRange(IO.Directory.GetDirectories(SourceDirectory, "*", IO.SearchOption.AllDirectories))
                                                    FSEntries.AddRange(IO.Directory.GetFiles(SourceDirectory, "*", IO.SearchOption.AllDirectories))
                                                    Dim CurrentProgress As Integer = 0
                                                    Dim TotalProgress As Integer = FSEntries.Count
                                                    Dim ProgressState As Integer = 0


                                                    Dim KeepPrinterThreadActive As Boolean = True
                                                    Dim PrinterThread As New Threading.Thread(Sub()
                                                                                                  Dim PrinterLimiter As New ThreadLimiter(60)
                                                                                                  While KeepPrinterThreadActive = True
                                                                                                      Dim InvokeThread1 As New Threading.Thread(Sub() ProgressUpdateCallback.Invoke(ProgressState, CurrentProgress, TotalProgress)) : InvokeThread1.Start()
                                                                                                      PrinterLimiter.Limit()
                                                                                                  End While
                                                                                              End Sub)
                                                    PrinterThread.Start()




                                                    Dim position As Integer = 0
                                                    Dim MetaThreads As New List(Of MetaThread)
                                                    If Threads < 1 Then
                                                        Threads = 1
                                                    End If
                                                    For x = 0 To Threads - 1
                                                        MetaThreads.Add(New MetaThread(SourceDirectory))
                                                    Next
                                                    Do Until FSEntries.Count = 0
                                                        Dim FSPath As String = FSEntries(0)
                                                        MetaThreads(position).Add(FSPath)
                                                        position += 1
                                                        If position = Threads Then position = 0
                                                        FSEntries.Remove(FSPath)
                                                    Loop

                                                    For x = 0 To MetaThreads.Count - 1
                                                        MetaThreads(x).Run(Sub()
                                                                               CurrentProgress += 1
                                                                           End Sub, ThreadLimit)
                                                    Next

                                                    Dim Finished As Boolean = False
                                                    Do Until Finished = True
                                                        Finished = True
                                                        For x = 0 To MetaThreads.Count - 1
                                                            Dim ThreadStatus As Boolean = MetaThreads(x).Running
                                                            If ThreadStatus = True Then
                                                                Finished = False
                                                                Exit For
                                                            End If
                                                        Next
                                                    Loop


                                                    Dim MetaObjects As New List(Of MetaObject)
                                                    For x = 0 To MetaThreads.Count - 1
                                                        MetaObjects.AddRange(MetaThreads(x).GetMetadata)
                                                        MetaThreads(x).Dispose()
                                                    Next
                                                    MetaObjects = MetaObjects.OrderBy(Function(x) x.IsDirectory = True).ToList
                                                    MetaThreads.Clear()



                                                    Dim ToBeSerialized As New List(Of Byte())
                                                    ToBeSerialized.Add(UpdateCore.Serialization.SerializeArray({System.Text.ASCIIEncoding.ASCII.GetBytes(SourceDirectory),
                                                                       System.Text.ASCIIEncoding.ASCII.GetBytes(ProgramName),
                                                                       System.Text.ASCIIEncoding.ASCII.GetBytes(ProgramID)}))
                                                    ProgressState = 1
                                                    CurrentProgress = 0

                                                    For x = 0 To MetaObjects.Count - 1
                                                        Dim Data As Byte() = UpdateCore.Serialization.SerializeArray({BitConverter.GetBytes(MetaObjects(x).IsDirectory),
                                                                             System.Text.ASCIIEncoding.ASCII.GetBytes(MetaObjects(x).RelativePath),
                                                                             BitConverter.GetBytes(MetaObjects(x).Size),
                                                                             MetaObjects(x).SHA256Hash})
                                                        ToBeSerialized.Add(Data)
                                                        CurrentProgress += 1
                                                    Next

                                                    Dim Output As Byte() = UpdateCore.Serialization.SerializeArray(ToBeSerialized.ToArray)
                                                    Dim Stream As New IO.FileStream(MetaSaveDirectory + "/" + FileName, IO.FileMode.Create)
                                                    Stream.Write(Output, 0, Output.Length)
                                                    Stream.Flush()
                                                    Stream.Close()
                                                    Stream.Dispose()
                                                    Stream = Nothing
                                                    ToBeSerialized.Clear()
                                                    ToBeSerialized = Nothing
                                                    Threading.Thread.Sleep(100)
                                                    KeepPrinterThreadActive = False
                                                    Dim InvokeThread2 As New Threading.Thread(Sub() ProgressCompletedCallback.Invoke) : InvokeThread2.Start()


                                                End Sub) : AsyncThread.Start()
    End Sub
End Class
