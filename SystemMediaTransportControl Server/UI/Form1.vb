Imports System.ComponentModel
Imports System.IO
Imports System.Runtime.InteropServices
Imports Microsoft.Toolkit.Forms.UI.XamlHost
Imports Windows.Media.Control
Imports Windows.UI.Xaml
Imports Windows.Storage.Streams
Imports Image = System.Drawing.Image
Imports Windows.UI.Xaml.Data

Public Class Form1

    Public Sub New()

        Marshal.GetLastWin32Error()
        SetThreadDpiAwarenessContext(-1)
        Debug.Print(New Win32Exception(Marshal.GetLastWin32Error()).Message)

        ' Dieser Aufruf ist für den Designer erforderlich.
        InitializeComponent()

        ' Fügen Sie Initialisierungen nach dem InitializeComponent()-Aufruf hinzu.

    End Sub

    Public ReadOnly Property CurrentSession As GlobalSystemMediaTransportControlsSession

    <DllImport("SHCore.dll", SetLastError:=True)>
    Private Shared Function SetProcessDpiAwareness(ByVal awareness As PROCESS_DPI_AWARENESS) As Boolean : End Function
    <DllImport("SHCore.dll", SetLastError:=True)>
    Private Shared Sub GetProcessDpiAwareness(ByVal hprocess As IntPtr, <Out> ByRef awareness As PROCESS_DPI_AWARENESS) : End Sub

    <DllImport("User32.dll", SetLastError:=True)>
    Private Shared Function SetThreadDpiAwarenessContext(ByVal awareness As PROCESS_DPI_AWARENESS) As Boolean : End Function

    Private Enum PROCESS_DPI_AWARENESS
        Process_DPI_Unaware = 0
        Process_System_DPI_Aware = 1
        Process_Per_Monitor_DPI_Aware = 2
    End Enum

    Public ReadOnly Property Slider As Controls.Slider

    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load

        Dim HostControl As New WindowsXamlHost()
        HostControl.Dock = DockStyle.Fill

        Dim grid As New Controls.Grid()
        grid.Background = New Media.SolidColorBrush(Panel3.BackColor.ToUWPColor())

        _Slider = New Controls.Slider()
        Slider.Width = 200
        Slider.ThumbToolTipValueConverter = New TimeStampThumbConverter()
        Slider.Background = New Media.SolidColorBrush(Color.White.ToUWPColor())

        AddHandler Slider.ValueChanged, Sub()
                                            CurrentSession.TryChangePlaybackPositionAsync(Slider.Value)
                                        End Sub

        grid.Children.Add(Slider)

        HostControl.Child = grid

        Application.Current.RequestedTheme = ApplicationTheme.Dark

        Panel3.Controls.Add(HostControl)


        ' https://stackoverflow.com/a/63099881

        Dim SessionManager = GlobalSystemMediaTransportControlsSessionManager.RequestAsync().GetAwaiter().GetResult()
        _CurrentSession = SessionManager.GetCurrentSession()

        Setup()

        AddHandler CurrentSession.MediaPropertiesChanged, Sub()
                                                              Me.Invoke(Sub() Setup())
                                                          End Sub
        AddHandler CurrentSession.PlaybackInfoChanged, Sub()
                                                           Me.Invoke(Sub() Setup())
                                                       End Sub
        AddHandler CurrentSession.TimelinePropertiesChanged, Sub()
                                                                 Me.Invoke(Sub() Setup())
                                                             End Sub

    End Sub

    Sub Setup()
        Dim Properties = CurrentSession.TryGetMediaPropertiesAsync().GetAwaiter().GetResult()
        If Not Properties.Thumbnail Is Nothing Then
            Dim Thumbnail As Image
            Using stream = Properties.Thumbnail.OpenReadAsync().GetAwaiter().GetResult().AsStream()
                Thumbnail = Image.FromStream(stream)
            End Using
            PictureBox1.Image = Thumbnail
        Else
            PictureBox1.Image = My.Resources.MissingAlbumArt
        End If
        Label1.Text = Properties.Title
        Label2.Text = Properties.Artist

        If CurrentSession.GetPlaybackInfo().PlaybackStatus = GlobalSystemMediaTransportControlsSessionPlaybackStatus.Playing Then
            Button1.Tag = "pause"
            Button1.BackgroundImage = My.Resources.baseline_pause_white_18dp
        Else
            Button1.Tag = "play"
            Button1.BackgroundImage = My.Resources.baseline_play_arrow_white_18dp
        End If

        Dim Timeline = CurrentSession.GetTimelineProperties()

        Slider.Maximum = Timeline.EndTime.TotalSeconds
        Slider.Value = Timeline.Position.TotalSeconds
    End Sub

    Private Sub Button1_Click(sender As Button, e As EventArgs) Handles Button1.Click
        If sender.Tag = "pause" Then
            CurrentSession.TryPauseAsync()
            Button1.Tag = "play"
            Button1.BackgroundImage = My.Resources.baseline_play_arrow_white_18dp
        Else
            CurrentSession.TryPlayAsync()
            Button1.Tag = "pause"
            Button1.BackgroundImage = My.Resources.baseline_pause_white_18dp
        End If
    End Sub

    Private Sub Button3_Click(sender As Object, e As EventArgs) Handles Button3.Click
        CurrentSession.TrySkipPreviousAsync()
    End Sub

    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        CurrentSession.TrySkipPreviousAsync()
    End Sub
End Class
