Imports System.Collections.Specialized
Imports System.IO
Imports System.Net
Imports System.Reflection
Imports System.Runtime.CompilerServices
Imports Newtonsoft.Json

Module ServerExtensions

    <Extension>
    Public Sub SendException(ByRef context As HttpListenerContext, ex As Exception)
        context.Response.SetStatus(HttpStatusCode.InternalServerError)
        context.SendJson(ex)
    End Sub

    <Extension>
    Public Sub SendJson(ByRef context As HttpListenerContext, obj As Object)
        SendJson(context, JsonConvert.SerializeObject(obj))
    End Sub
    <Extension>
    Public Sub SendJson(ByRef context As HttpListenerContext, json As String)
        context.Response.ContentType = "application/json"
        Using stream = context.Response.OutputStream
            Using writer As New StreamWriter(stream)
                writer.Write(json)
            End Using
        End Using
    End Sub

    <Extension>
    Public Sub SetStatus(ByRef Response As HttpListenerResponse, status As HttpStatusCode)
        Response.StatusCode = status
        Response.StatusDescription = status.ToString()
    End Sub

    <Extension>
    Public Function ParseQuery(ByRef Response As Uri) As NameValueCollection
        Dim out As New NameValueCollection
        Dim query = Response.Query.Replace("?", "")
        For Each section In Split(query, "&")
            Dim parts = Split(section, "=")
            out.Add(WebUtility.UrlDecode(parts(0)), If(parts.Count = 2, WebUtility.HtmlDecode(parts(1)), ""))
        Next
        Return out
    End Function

End Module
