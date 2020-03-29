<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class MainForm
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()>
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
    <System.Diagnostics.DebuggerStepThrough()>
    Private Sub InitializeComponent()
        Me.HtmlPanel1 = New TheArtOfDev.HtmlRenderer.WinForms.HtmlPanel()
        Me.Panel1 = New System.Windows.Forms.Panel()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.RenderedDualProgressbar1 = New ClientUI.RenderedDualProgressbar()
        Me.Button4 = New System.Windows.Forms.Button()
        Me.Button5 = New System.Windows.Forms.Button()
        Me.Button3 = New System.Windows.Forms.Button()
        Me.Button1 = New System.Windows.Forms.Button()
        Me.ProgramList1 = New ClientUI.ProgramList()
        Me.Panel1.SuspendLayout()
        Me.SuspendLayout()
        '
        'HtmlPanel1
        '
        Me.HtmlPanel1.AutoScroll = True
        Me.HtmlPanel1.BackColor = System.Drawing.SystemColors.Window
        Me.HtmlPanel1.BaseStylesheet = Nothing
        Me.HtmlPanel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.HtmlPanel1.Cursor = System.Windows.Forms.Cursors.IBeam
        Me.HtmlPanel1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.HtmlPanel1.Location = New System.Drawing.Point(256, 0)
        Me.HtmlPanel1.Name = "HtmlPanel1"
        Me.HtmlPanel1.Size = New System.Drawing.Size(698, 487)
        Me.HtmlPanel1.TabIndex = 5
        Me.HtmlPanel1.Text = Nothing
        '
        'Panel1
        '
        Me.Panel1.BackColor = System.Drawing.Color.White
        Me.Panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.Panel1.Controls.Add(Me.Label1)
        Me.Panel1.Controls.Add(Me.Label2)
        Me.Panel1.Controls.Add(Me.RenderedDualProgressbar1)
        Me.Panel1.Controls.Add(Me.Button4)
        Me.Panel1.Controls.Add(Me.Button5)
        Me.Panel1.Controls.Add(Me.Button3)
        Me.Panel1.Controls.Add(Me.Button1)
        Me.Panel1.Dock = System.Windows.Forms.DockStyle.Bottom
        Me.Panel1.Location = New System.Drawing.Point(256, 487)
        Me.Panel1.Margin = New System.Windows.Forms.Padding(0)
        Me.Panel1.Name = "Panel1"
        Me.Panel1.Size = New System.Drawing.Size(698, 24)
        Me.Panel1.TabIndex = 4
        '
        'Label1
        '
        Me.Label1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.Label1.Location = New System.Drawing.Point(0, 0)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(410, 15)
        Me.Label1.TabIndex = 3
        Me.Label1.Text = "Connecting to server..."
        Me.Label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'Label2
        '
        Me.Label2.Dock = System.Windows.Forms.DockStyle.Right
        Me.Label2.Location = New System.Drawing.Point(410, 0)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(82, 15)
        Me.Label2.TabIndex = 7
        Me.Label2.Text = " 000% complete"
        Me.Label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        Me.Label2.Visible = False
        '
        'RenderedDualProgressbar1
        '
        Me.RenderedDualProgressbar1.Dock = System.Windows.Forms.DockStyle.Bottom
        Me.RenderedDualProgressbar1.Location = New System.Drawing.Point(0, 15)
        Me.RenderedDualProgressbar1.Name = "RenderedDualProgressbar1"
        Me.RenderedDualProgressbar1.ProgressBarColor1 = System.Drawing.Color.ForestGreen
        Me.RenderedDualProgressbar1.ProgressBarColor2 = System.Drawing.Color.ForestGreen
        Me.RenderedDualProgressbar1.ProgressBarText1 = ""
        Me.RenderedDualProgressbar1.ProgressBarText2 = ""
        Me.RenderedDualProgressbar1.ProgressBorderColor = System.Drawing.Color.Silver
        Me.RenderedDualProgressbar1.ProgressMaximum1 = 100
        Me.RenderedDualProgressbar1.ProgressMaximum2 = 100
        Me.RenderedDualProgressbar1.ProgressMinimum1 = 0
        Me.RenderedDualProgressbar1.ProgressMinimum2 = 0
        Me.RenderedDualProgressbar1.ProgressValue1 = 0
        Me.RenderedDualProgressbar1.ProgressValue2 = 0
        Me.RenderedDualProgressbar1.Size = New System.Drawing.Size(492, 7)
        Me.RenderedDualProgressbar1.TabIndex = 8
        '
        'Button4
        '
        Me.Button4.Dock = System.Windows.Forms.DockStyle.Right
        Me.Button4.Location = New System.Drawing.Point(492, 0)
        Me.Button4.Name = "Button4"
        Me.Button4.Size = New System.Drawing.Size(51, 22)
        Me.Button4.TabIndex = 5
        Me.Button4.Text = "Retry"
        Me.Button4.UseVisualStyleBackColor = True
        Me.Button4.Visible = False
        '
        'Button5
        '
        Me.Button5.Dock = System.Windows.Forms.DockStyle.Right
        Me.Button5.Location = New System.Drawing.Point(543, 0)
        Me.Button5.Name = "Button5"
        Me.Button5.Size = New System.Drawing.Size(51, 22)
        Me.Button5.TabIndex = 6
        Me.Button5.Text = "Update"
        Me.Button5.UseVisualStyleBackColor = True
        Me.Button5.Visible = False
        '
        'Button3
        '
        Me.Button3.Dock = System.Windows.Forms.DockStyle.Right
        Me.Button3.Location = New System.Drawing.Point(594, 0)
        Me.Button3.Name = "Button3"
        Me.Button3.Size = New System.Drawing.Size(51, 22)
        Me.Button3.TabIndex = 4
        Me.Button3.Text = "Options"
        Me.Button3.UseVisualStyleBackColor = True
        '
        'Button1
        '
        Me.Button1.Dock = System.Windows.Forms.DockStyle.Right
        Me.Button1.Location = New System.Drawing.Point(645, 0)
        Me.Button1.Name = "Button1"
        Me.Button1.Size = New System.Drawing.Size(51, 22)
        Me.Button1.TabIndex = 2
        Me.Button1.Text = "Launch"
        Me.Button1.UseVisualStyleBackColor = True
        '
        'ProgramList1
        '
        Me.ProgramList1.BackColor = System.Drawing.Color.White
        Me.ProgramList1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.ProgramList1.Dock = System.Windows.Forms.DockStyle.Left
        Me.ProgramList1.Location = New System.Drawing.Point(0, 0)
        Me.ProgramList1.Name = "ProgramList1"
        Me.ProgramList1.Size = New System.Drawing.Size(256, 511)
        Me.ProgramList1.TabIndex = 6
        '
        'MainForm
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.BackColor = System.Drawing.Color.White
        Me.ClientSize = New System.Drawing.Size(954, 511)
        Me.Controls.Add(Me.HtmlPanel1)
        Me.Controls.Add(Me.Panel1)
        Me.Controls.Add(Me.ProgramList1)
        Me.MinimumSize = New System.Drawing.Size(970, 550)
        Me.Name = "MainForm"
        Me.Text = "Update Client"
        Me.Panel1.ResumeLayout(False)
        Me.ResumeLayout(False)

    End Sub

    Friend WithEvents HtmlPanel1 As TheArtOfDev.HtmlRenderer.WinForms.HtmlPanel
    Friend WithEvents Panel1 As Panel
    Friend WithEvents Label1 As Label
    Friend WithEvents Label2 As Label
    Friend WithEvents RenderedDualProgressbar1 As RenderedDualProgressbar
    Friend WithEvents Button4 As Button
    Friend WithEvents Button5 As Button
    Friend WithEvents Button3 As Button
    Friend WithEvents Button1 As Button
    Friend WithEvents ProgramList1 As ProgramList
End Class
