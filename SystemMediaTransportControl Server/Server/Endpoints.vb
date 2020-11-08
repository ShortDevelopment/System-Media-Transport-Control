Imports Newtonsoft.Json.Linq
Imports Windows.Media.Control

<Endpoint("/sessions/current")>
Public Class GetCurrentSessionEndpoint
    Inherits EndpointBase

    Public Sub New(Server As Server)
        MyBase.New(Server)
    End Sub

    Public Overrides Sub ProcessRequest(ByRef context As Net.HttpListenerContext, params As Specialized.NameValueCollection)
        Dim SessionManager = GlobalSystemMediaTransportControlsSessionManager.RequestAsync().GetAwaiter().GetResult()
        context.SendJson(Session.FromGlobalSystemMediaTransportControlsSession(SessionManager.GetCurrentSession(), True))
    End Sub
End Class

<Endpoint("/sessions/list")>
Public Class ListSessionsEndpoint
    Inherits EndpointBase

    Public Sub New(Server As Server)
        MyBase.New(Server)
    End Sub

    Public Overrides Sub ProcessRequest(ByRef context As Net.HttpListenerContext, params As Specialized.NameValueCollection)
        Dim SessionManager = GlobalSystemMediaTransportControlsSessionManager.RequestAsync().GetAwaiter().GetResult()
        Dim current = SessionManager.GetCurrentSession()
        context.SendJson(SessionManager.GetSessions().Select(Function(x) Session.FromGlobalSystemMediaTransportControlsSession(x, If(x.SourceAppUserModelId = current.SourceAppUserModelId, True, False))).ToArray())
    End Sub
End Class

<Endpoint("/sessions/ctrl/{session}/{method}")>
Public Class ControlSession
    Inherits EndpointBase

    Public Sub New(Server As Server)
        MyBase.New(Server)
    End Sub

    Public Overrides Sub ProcessRequest(ByRef context As Net.HttpListenerContext, params As Specialized.NameValueCollection)
        If Not Server.Settings.CanControl Then Throw New AccessViolationException("CanControl is disabled in settings!")
        Dim SessionManager = GlobalSystemMediaTransportControlsSessionManager.RequestAsync().GetAwaiter().GetResult()
        Dim sess As GlobalSystemMediaTransportControlsSession
        If params("session") = "current" Then
            sess = SessionManager.GetCurrentSession()
        Else
            Dim index As Integer
            Try
                index = Int(params("session"))
            Catch ex As Exception
                Throw New Exception("You have to specify a number for 'session'!", ex)
            End Try
            Try
                sess = SessionManager.GetSessions()(index)
            Catch ex As Exception
                Throw New Exception("Could not find session!", ex)
            End Try
        End If
        Select Case params("method")
            Case "play"
                sess.TryPlayAsync()
            Case "pause"
                sess.TryPauseAsync()
            Case "stop"
                sess.TryStopAsync()
            Case "next"
                sess.TrySkipNextAsync()
            Case "previous"
                sess.TrySkipPreviousAsync()
            Case Else
                Throw New NotImplementedException("Method unkown!")
        End Select
        context.SendJson(Session.FromGlobalSystemMediaTransportControlsSession(sess, True))
    End Sub
End Class
