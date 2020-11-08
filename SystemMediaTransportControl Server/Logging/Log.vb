Public Class Log

    Public Shared Sub Write(text As String, Optional color As ConsoleColor = ConsoleColor.Gray, Optional PostFormat As Boolean = True)
        Dim oldColor = Console.ForegroundColor
        Console.ForegroundColor = color
        Console.Write(text)
        Debug.Write(text)
        Console.ForegroundColor = oldColor
    End Sub
    Public Shared Sub WriteLine(text As String, Optional color As ConsoleColor = ConsoleColor.Gray, Optional PostFormat As Boolean = True)
        Dim oldColor = Console.ForegroundColor
        Console.ForegroundColor = color
        Console.WriteLine(text)
        Debug.WriteLine(text)
        Console.ForegroundColor = oldColor
    End Sub

    Public Shared Sub LogException(ex As Exception, Optional Handled As Boolean = False, Optional indent As String = "")
        If indent = "" Then Log.WriteLine(indent + $"==================", ConsoleColor.White, False)
        Log.WriteLine(indent + $"[{ex.GetType().Name}]: {ex.Message}", ConsoleColor.Red, False)
        Log.WriteLine(indent + String.Join(vbNewLine + indent, Split(ex.StackTrace, vbNewLine)), ConsoleColor.Yellow, False)
        If Not ex.InnerException Is Nothing Then
            Log.WriteLine("")
            LogException(ex.InnerException, Handled, indent + "   ")
        End If
        If indent = "" Then Log.WriteLine($"==================", ConsoleColor.White, False)
    End Sub

End Class
