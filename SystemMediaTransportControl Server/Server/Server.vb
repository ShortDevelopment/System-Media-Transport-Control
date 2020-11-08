Imports System.Collections.ObjectModel
Imports System.Collections.Specialized
Imports System.IO
Imports System.Net
Imports Newtonsoft.Json
Imports Newtonsoft.Json.Converters
Imports Windows.Media.Control

Public Class Server

    Public ReadOnly Property Settings As Settings
    Public ReadOnly Property Server As New HttpListener
    Public ReadOnly Property Endpoints As ReadOnlyDictionary(Of UrlParser, Type)

    Public Sub New(Settings As Settings)
        Me.Settings = Settings

        JsonConvert.DefaultSettings = (Function()
                                           Dim DefaultSettings = New JsonSerializerSettings()
                                           DefaultSettings.Converters.Add(New StringEnumConverter With {
                                                                                      .CamelCaseText = True
                                                                                  })
                                           Return DefaultSettings
                                       End Function)

        Dim EndpointTypes = GetType(EndpointBase).Assembly.GetTypes().Where(Function(x) GetType(EndpointBase).IsAssignableFrom(x) AndAlso x.CustomAttributes.Where(Function(attr) attr.AttributeType = GetType(EndpointAttribute)).Count() = 1).ToList()
        Dim EndpointList As New Dictionary(Of UrlParser, Type)
        For Each type In EndpointTypes
            Dim Pattern = CType(type.GetCustomAttributes(False).Where(Function(x) x.GetType() = GetType(EndpointAttribute))(0), EndpointAttribute).UrlPattern
            EndpointList.Add(New UrlParser(Pattern), type)
        Next
        _Endpoints = New ReadOnlyDictionary(Of UrlParser, Type)(EndpointList)
    End Sub

    Public Sub StartServer()
        Server.Prefixes.Add($"http://localhost:{Settings.Port}/")
        Server.Prefixes.Add($"http://127.0.0.1:{Settings.Port}/")
        Try
            Server.Start()
        Catch ex As Exception
            Throw New Exception("Try another port!", ex)
        End Try
        Server.BeginGetContext(AddressOf OnRequest, Nothing)
    End Sub

    Private Sub OnRequest(result As IAsyncResult)
        Dim Context = Server.EndGetContext(result)
        Dim url = Context.Request.Url.LocalPath
        Dim found As Boolean = False
        For Each EndpointType In Endpoints
            Dim params As NameValueCollection
            If EndpointType.Key.TryParse(Context.Request.Url, params) Then
                found = True
                Try
                    CType(Activator.CreateInstance(EndpointType.Value, {Me}), EndpointBase).ProcessRequest(Context, params)
                Catch ex As Exception
                    Context.SendException(ex)
                End Try
                Exit For
            End If
        Next
        If Not found Then Context.SendException(New Exception("Bad Request"))
        Server.BeginGetContext(AddressOf OnRequest, Nothing)
    End Sub

    Public Sub StopServer()
        Server.Stop()
    End Sub

End Class
