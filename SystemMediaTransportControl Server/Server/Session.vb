Imports Windows.Media
Imports Windows.Media.Control
Public Class Session
    Public Property Application As String
    Public Property IsCurrent As Boolean
    Public Property Timeline As GlobalSystemMediaTransportControlsSessionTimelineProperties
    Public Property Media As GlobalSystemMediaTransportControlsSessionMediaProperties
    Public Property Playback As GlobalSystemMediaTransportControlsSessionPlaybackInfo

    Public Shared Function FromGlobalSystemMediaTransportControlsSession(session As GlobalSystemMediaTransportControlsSession, Optional IsCurrent As Boolean = False) As Session
        Dim out As New Session
        out.Application = session.SourceAppUserModelId
        out.Timeline = session.GetTimelineProperties()
        out.Media = session.TryGetMediaPropertiesAsync().GetAwaiter().GetResult()
        out.Playback = session.GetPlaybackInfo()
        out.IsCurrent = IsCurrent
        Return out
    End Function

End Class
