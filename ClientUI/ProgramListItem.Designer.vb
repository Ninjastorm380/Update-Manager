<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class ProgramListItem
    Inherits System.Windows.Forms.UserControl

    'UserControl overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(ProgramListItem))
        Me.ProgramListItemToolstrip1 = New ClientUI.ProgramListItemToolstrip()
        Me.ToolStripLabel1 = New System.Windows.Forms.ToolStripLabel()
        Me.ToolStripLabel3 = New System.Windows.Forms.ToolStripLabel()
        Me.ToolStripLabel2 = New System.Windows.Forms.ToolStripLabel()
        Me.RenderedProgressbar1 = New ClientUI.RenderedProgressbar()
        Me.ProgramListItemToolstrip1.SuspendLayout()
        Me.SuspendLayout()
        '
        'ProgramListItemToolstrip1
        '
        Me.ProgramListItemToolstrip1.AutoSize = False
        Me.ProgramListItemToolstrip1.BackColor = System.Drawing.Color.Silver
        Me.ProgramListItemToolstrip1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.ProgramListItemToolstrip1.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden
        Me.ProgramListItemToolstrip1.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.ToolStripLabel1, Me.ToolStripLabel3, Me.ToolStripLabel2})
        Me.ProgramListItemToolstrip1.Location = New System.Drawing.Point(1, 1)
        Me.ProgramListItemToolstrip1.Name = "ProgramListItemToolstrip1"
        Me.ProgramListItemToolstrip1.Size = New System.Drawing.Size(253, 20)
        Me.ProgramListItemToolstrip1.TabIndex = 3
        Me.ProgramListItemToolstrip1.Text = "ProgramListItemToolstrip1"
        '
        'ToolStripLabel1
        '
        Me.ToolStripLabel1.Name = "ToolStripLabel1"
        Me.ToolStripLabel1.Size = New System.Drawing.Size(85, 17)
        Me.ToolStripLabel1.Text = "ProgramName"
        '
        'ToolStripLabel3
        '
        Me.ToolStripLabel3.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right
        Me.ToolStripLabel3.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
        Me.ToolStripLabel3.Image = CType(resources.GetObject("ToolStripLabel3.Image"), System.Drawing.Image)
        Me.ToolStripLabel3.Name = "ToolStripLabel3"
        Me.ToolStripLabel3.Size = New System.Drawing.Size(16, 17)
        Me.ToolStripLabel3.Text = "ToolStripLabel2"
        Me.ToolStripLabel3.Visible = False
        '
        'ToolStripLabel2
        '
        Me.ToolStripLabel2.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right
        Me.ToolStripLabel2.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
        Me.ToolStripLabel2.Image = CType(resources.GetObject("ToolStripLabel2.Image"), System.Drawing.Image)
        Me.ToolStripLabel2.Name = "ToolStripLabel2"
        Me.ToolStripLabel2.Size = New System.Drawing.Size(16, 17)
        Me.ToolStripLabel2.Text = "ToolStripLabel2"
        Me.ToolStripLabel2.Visible = False
        '
        'RenderedProgressbar1
        '
        Me.RenderedProgressbar1.BackColor = System.Drawing.Color.White
        Me.RenderedProgressbar1.Dock = System.Windows.Forms.DockStyle.Bottom
        Me.RenderedProgressbar1.Location = New System.Drawing.Point(1, 18)
        Me.RenderedProgressbar1.Name = "RenderedProgressbar1"
        Me.RenderedProgressbar1.ProgressBarColor = System.Drawing.Color.ForestGreen
        Me.RenderedProgressbar1.ProgressBarStyle = ClientUI.RenderedProgressbar.ProgressStyle.Progress
        Me.RenderedProgressbar1.ProgressBarText = ""
        Me.RenderedProgressbar1.ProgressBorderColor = System.Drawing.Color.Silver
        Me.RenderedProgressbar1.ProgressMaximum = 100
        Me.RenderedProgressbar1.ProgressMinimum = 0
        Me.RenderedProgressbar1.ProgressValue = 0
        Me.RenderedProgressbar1.Size = New System.Drawing.Size(253, 3)
        Me.RenderedProgressbar1.TabIndex = 4
        '
        'ProgramListItem
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.BackColor = System.Drawing.Color.Transparent
        Me.Controls.Add(Me.RenderedProgressbar1)
        Me.Controls.Add(Me.ProgramListItemToolstrip1)
        Me.DoubleBuffered = True
        Me.Name = "ProgramListItem"
        Me.Padding = New System.Windows.Forms.Padding(1)
        Me.Size = New System.Drawing.Size(255, 22)
        Me.ProgramListItemToolstrip1.ResumeLayout(False)
        Me.ProgramListItemToolstrip1.PerformLayout()
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents ProgramListItemToolstrip1 As ProgramListItemToolstrip
    Friend WithEvents ToolStripLabel1 As ToolStripLabel
    Friend WithEvents ToolStripLabel3 As ToolStripLabel
    Friend WithEvents ToolStripLabel2 As ToolStripLabel
    Friend WithEvents RenderedProgressbar1 As RenderedProgressbar
End Class
