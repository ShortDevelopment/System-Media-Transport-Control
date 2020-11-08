Imports System.Collections.Specialized
Imports System.Text.RegularExpressions

Public Class UrlParser

    Public ReadOnly Property UrlPattern As String

    Dim NewRegex As String
    Dim keys As New List(Of String)

    Public Sub New(pattern As String)
        UrlPattern = pattern

        Dim regex1 As New Regex("{([a-z]*)}")
        NewRegex = UrlPattern
        For Each match As Match In regex1.Matches(UrlPattern)
            keys.Add(match.Groups(1).Value)
            NewRegex = NewRegex.Replace(match.Value, $"(?<{match.Groups(1).Value}>([a-z]|[0-9]|\.|-)+)")
        Next
        NewRegex = "^" + NewRegex.Replace("/", "\/") + "$"
    End Sub

    Public Function TryParse(ByVal uri As Uri, ByRef params As NameValueCollection) As Boolean
        Dim out As New NameValueCollection()
        Dim Match = Regex.Match(uri.AbsolutePath, NewRegex, RegexOptions.IgnoreCase)
        If Not Match.Success Then Return False
        For Each key In keys
            out.Add(key, Match.Groups.Item(key).Value.ToLower())
        Next
        params = out
        Return True
    End Function

End Class