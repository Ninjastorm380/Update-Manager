Public Class Form1
    Private Shared Function ArrayEquals(ByVal a As Byte(), ByVal b As Byte()) As Boolean
        Dim x As Integer = a.Length Xor b.Length
        Dim i As Integer = 0

        While i < a.Length AndAlso i < b.Length
            x = x Or a(i) Xor b(i)
            i += 1
        End While

        Return x = 0
    End Function

    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Debug.Print(ArrayEquals({0, 0, 0, 0}, {0, 0, 0, 0}))
    End Sub
End Class
