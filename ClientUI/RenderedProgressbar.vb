Public Class RenderedProgressbar
    Public Enum ProgressStyle
        MarqueeAndProgress = 0
        Marquee = 1
        Progress = 2
    End Enum
    Private _MinimumValue As Integer = 0 : Public Property ProgressMinimum() As Integer
        Get
            Return _MinimumValue
        End Get
        Set(ByVal Value As Integer)
            If (Value < 0) Then
                _MinimumValue = 0
            End If
            If (Value > _MaximumValue) Then
                _MinimumValue = Value
                _MinimumValue = Value
            End If
            If (_CurrentValue < _MinimumValue) Then
                _CurrentValue = _MinimumValue
            End If
            Me.Invalidate()
        End Set
    End Property
    Private _MaximumValue As Integer = 100 : Public Property ProgressMaximum() As Integer
        Get
            Return _MaximumValue
        End Get
        Set(ByVal Value As Integer)
            If (Value < _MinimumValue) Then
                _MinimumValue = Value
            End If
            _MaximumValue = Value
            If (_CurrentValue > _MaximumValue) Then
                _CurrentValue = _MaximumValue
            End If
            Me.Invalidate()
        End Set
    End Property
    Private _CurrentValue As Integer = 0 : Public Property ProgressValue() As Integer
        Get
            Return _CurrentValue
        End Get
        Set(ByVal Value As Integer)
            If (Value < _MinimumValue) Then
                _CurrentValue = _MinimumValue
            ElseIf (Value > _MaximumValue) Then
                _CurrentValue = _MaximumValue
            Else
                _CurrentValue = Value
            End If
            Me.Invalidate()
        End Set
    End Property
    Private _ProgressColor As Color = Color.LimeGreen : Public Property ProgressBarColor() As Color
        Get
            Return _ProgressColor
        End Get
        Set(ByVal Value As Color)
            _ProgressColor = Value
            Me.Invalidate()
        End Set
    End Property
    Private _BorderColor As Color = Color.Silver : Public Property ProgressBorderColor As Color
        Get
            Return _BorderColor
        End Get
        Set(value As Color)
            _BorderColor = value
            Me.Invalidate()
        End Set
    End Property
    Private _ProgressText As String = "" : Public Property ProgressBarText As String
        Get
            Return _ProgressText
        End Get
        Set(value As String)
            _ProgressText = value
            Me.Invalidate()
        End Set
    End Property
    Public Property ProgressBarStyle As ProgressStyle = ProgressStyle.Progress
    Protected Overrides Sub OnResize(ByVal e As EventArgs)
        Me.Invalidate()
    End Sub
    Private Sub CustomProgressbar_Paint(sender As Object, e As PaintEventArgs) Handles MyBase.Paint
        Dim brush As SolidBrush = New SolidBrush(_ProgressColor)
        Dim percent As Double = (_CurrentValue - _MinimumValue) / (_MaximumValue - _MinimumValue)
        Dim rect As New Rectangle(1, 1, Me.Width - 2, Me.Height - 2)
        Dim Width As Integer = CType(rect.Width * percent, Integer)
        rect.Width = Width
        e.Graphics.FillRectangle(brush, rect)
        Dim StringSize As SizeF = e.Graphics.MeasureString(_ProgressText, Me.Font)
        If StringSize.Height < Me.Height - 2 Then
            e.Graphics.DrawString(_ProgressText, Me.Font, Brushes.Black, CType((Me.Width / 2) - (StringSize.Width / 2), Integer), CType((Me.Height / 2) - (StringSize.Height / 2), Integer))
        End If
        brush.Dispose()
        Dim P As New Pen(_BorderColor)
        e.Graphics.DrawRectangle(P, 0, 0, Me.Width - 1, Me.Height - 1)
        P.Dispose()
    End Sub
End Class
