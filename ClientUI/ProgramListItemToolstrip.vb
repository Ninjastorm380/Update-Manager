Public Class ProgramListItemToolstrip : Inherits ToolStrip
    Sub New()
        MyBase.New
        Me.Renderer = New ProgramListItemToolstripRenderer
    End Sub
End Class
Public Class ProgramListItemToolstripRenderer : Inherits ToolStripRenderer

End Class