Imports System.Collections.Specialized
Imports System.Net

Public MustInherit Class EndpointBase

    Public ReadOnly Property Server As Server

    Public Sub New(ByRef Server As Server)
        _Server = Server
    End Sub

    Public MustOverride Sub ProcessRequest(ByRef context As HttpListenerContext, ByVal params As NameValueCollection)

End Class

Public Class EndpointAttribute
    Inherits Attribute
    Public ReadOnly Property UrlPattern As String
    Public Sub New(pattern As String)
        _UrlPattern = pattern
    End Sub
End Class