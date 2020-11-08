Imports System.Reflection
Imports Newtonsoft.Json

Public Class Settings

#Region "Base"
    <JsonIgnore>
    Public ReadOnly Property Path As String

    Private Sub SetPath(path As String)
        _Path = path
    End Sub

    Public Shared Function FromFile(file As String) As Settings
        Dim obj As Settings
        If IO.File.Exists(file) Then
            obj = JsonConvert.DeserializeObject(Of Settings)(IO.File.ReadAllText(file))
        Else
            obj = Activator.CreateInstance(Of Settings)
        End If
        GetType(Settings).GetMethod("SetPath", BindingFlags.CreateInstance Or BindingFlags.Instance Or BindingFlags.NonPublic).Invoke(obj, {file})
        Return obj
    End Function

    Public Sub Save()
        IO.File.WriteAllText(Path, JsonConvert.SerializeObject(Me))
    End Sub
#End Region

    Property Port As Integer = 8080
    Property CanControl As Boolean = False
    Property ShowConsole As Boolean = True

End Class
