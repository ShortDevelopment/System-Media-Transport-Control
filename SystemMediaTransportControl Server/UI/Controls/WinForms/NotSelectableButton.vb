Public Class NotSelectableButton
    Inherits Button

    Public Sub New()
        SetStyle(ControlStyles.Selectable, False)
    End Sub
End Class
