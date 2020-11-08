Imports System.Threading

Module Helper
    Public Function RunOnNewThread(code As Action) As Thread
        Dim t As New Thread(Sub()
                                code?.Invoke()
                            End Sub)
        t.IsBackground = True
        t.Start()
        Return t
    End Function
End Module
