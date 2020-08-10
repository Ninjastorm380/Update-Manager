Public Class RenderedProgressbar
    Private _MinimumValue1 As Integer = 0 : Public Property ProgressMinimum1 As Integer
            Get
                Return _MinimumValue1
            End Get
            Set(ByVal Value As Integer)
                If (Value < 0) Then
                    _MinimumValue1 = 0
                End If
                If (Value > _MaximumValue1) Then
                    _MinimumValue1 = Value
                    _MinimumValue1 = Value
                End If
                If (_CurrentValue1 < _MinimumValue1) Then
                    _CurrentValue1 = _MinimumValue1
                End If
                Me.Invalidate()
            End Set
        End Property
        Private _MaximumValue1 As Integer = 100 : Public Property ProgressMaximum1 As Integer
            Get
                Return _MaximumValue1
            End Get
            Set(ByVal Value As Integer)
                If (Value < _MinimumValue1) Then
                    _MinimumValue1 = Value
                End If
                _MaximumValue1 = Value
                If (_CurrentValue1 > _MaximumValue1) Then
                    _CurrentValue1 = _MaximumValue1
                End If
                Me.Invalidate()
            End Set
        End Property
        Private _CurrentValue1 As Integer = 0 : Public Property ProgressValue1 As Integer
            Get
                Return _CurrentValue1
            End Get
            Set(ByVal Value As Integer)
                If (Value < _MinimumValue1) Then
                    _CurrentValue1 = _MinimumValue1
                ElseIf (Value > _MaximumValue1) Then
                    _CurrentValue1 = _MaximumValue1
                Else
                    _CurrentValue1 = Value
                End If
                Me.Invalidate()
            End Set
        End Property
        Private _ProgressColor1 As Color = Color.LimeGreen : Public Property ProgressBarColor1 As Color
            Get
                Return _ProgressColor1
            End Get
            Set(ByVal Value As Color)
                _ProgressColor1 = Value
                Me.Invalidate()
            End Set
        End Property
        Private _BorderColor1 As Color = Color.Silver : Public Property ProgressBorderColor As Color
            Get
                Return _BorderColor1
            End Get
            Set(value As Color)
                _BorderColor1 = value
                Me.Invalidate()
            End Set
        End Property
        Private _ProgressText1 As String = "" : Public Property ProgressBarText1 As String
            Get
                Return _ProgressText1
            End Get
            Set(value As String)
                _ProgressText1 = value
                Me.Invalidate()
            End Set
        End Property










        Private _MinimumValue2 As Integer = 0 : Public Property ProgressMinimum2 As Integer
            Get
                Return _MinimumValue2
            End Get
            Set(ByVal Value As Integer)
                If (Value < 0) Then
                    _MinimumValue2 = 0
                End If
                If (Value > _MaximumValue2) Then
                    _MinimumValue2 = Value
                    _MinimumValue2 = Value
                End If
                If (_CurrentValue2 < _MinimumValue2) Then
                    _CurrentValue2 = _MinimumValue2
                End If
                Me.Invalidate()
            End Set
        End Property
        Private _MaximumValue2 As Integer = 100 : Public Property ProgressMaximum2 As Integer
            Get
                Return _MaximumValue2
            End Get
            Set(ByVal Value As Integer)
                If (Value < _MinimumValue2) Then
                    _MinimumValue2 = Value
                End If
                _MaximumValue2 = Value
                If (_CurrentValue2 > _MaximumValue2) Then
                    _CurrentValue2 = _MaximumValue2
                End If
                Me.Invalidate()
            End Set
        End Property
        Private _CurrentValue2 As Integer = 0 : Public Property ProgressValue2 As Integer
            Get
                Return _CurrentValue2
            End Get
            Set(ByVal Value As Integer)
                If (Value < _MinimumValue2) Then
                    _CurrentValue2 = _MinimumValue2
                ElseIf (Value > _MaximumValue1) Then
                    _CurrentValue2 = _MaximumValue2
                Else
                    _CurrentValue2 = Value
                End If
                Me.Invalidate()
            End Set
        End Property
        Private _ProgressColor2 As Color = Color.LimeGreen : Public Property ProgressBarColor2 As Color
            Get
                Return _ProgressColor2
            End Get
            Set(ByVal Value As Color)
                _ProgressColor2 = Value
                Me.Invalidate()
            End Set
        End Property
        Private _ProgressText2 As String = "" : Public Property ProgressBarText2 As String
            Get
                Return _ProgressText2
            End Get
            Set(value As String)
                _ProgressText2 = value
                Me.Invalidate()
            End Set
        End Property








        Protected Overrides Sub OnResize(ByVal e As EventArgs)
            Me.Invalidate()
        End Sub
        Private Sub CustomProgressbar_Paint(sender As Object, e As PaintEventArgs) Handles MyBase.Paint
            Dim Height As Integer = Math.Floor((Me.Height / 2))
            Dim brush1 As SolidBrush = New SolidBrush(_ProgressColor1)
            Dim percent1 As Double = (_CurrentValue1 - _MinimumValue1) / (_MaximumValue1 - _MinimumValue1)
            Dim rect1 As New Rectangle(1, 1, Me.Width - 2, Height - 1)
            Dim Width1 As Integer = CType(rect1.Width * percent1, Integer)
            rect1.Width = Width1
            e.Graphics.FillRectangle(brush1, rect1)
            brush1.Dispose()

            Dim brush2 As SolidBrush = New SolidBrush(_ProgressColor2)
            Dim percent2 As Double = (_CurrentValue2 - _MinimumValue2) / (_MaximumValue2 - _MinimumValue2)
        Dim rect2 As New Rectangle(1, (Me.Height / 2), Me.Width - 2, Height)
        Dim Width2 As Integer = CType(rect2.Width * percent2, Integer)
            rect2.Width = Width2

            e.Graphics.FillRectangle(brush2, rect2)
            brush1.Dispose()




            'Dim StringSize As SizeF = e.Graphics.MeasureString(_ProgressText1, Me.Font)
            'If StringSize.Height < Me.Height - 2 Then
            '    e.Graphics.DrawString(_ProgressText1, Me.Font, Brushes.Black, CType((Me.Width / 2) - (StringSize.Width / 2), Integer), CType((Me.Height / 2) - (StringSize.Height / 2), Integer))
            'End If

            Dim P As New Pen(_BorderColor1)
            e.Graphics.DrawLine(P, 0, Height, Me.Width, Height)
            e.Graphics.DrawRectangle(P, 0, 0, Me.Width - 1, Me.Height - 1)

            P.Dispose()
        End Sub


    End Class
