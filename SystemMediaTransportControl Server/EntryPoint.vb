Imports System.IO
Imports System.Runtime.InteropServices

Public Class EntryPoint

    Public Shared ReadOnly Property Settings As Settings

    <STAThread>
    Public Shared Sub Main()

        _Settings = Settings.FromFile(Path.Combine(Application.StartupPath, "settings.json"))
        Settings.Save()

        Dim cmd = My.Application.CommandLineArgs
        If cmd.Count = 1 Then
            Select Case cmd(0).Replace("-", "").Replace("/", "").ToLower()
                Case "ui"
                    RunOnNewThread(Sub()
                                       Application.EnableVisualStyles()
                                       Application.SetCompatibleTextRenderingDefault(False)
                                       Application.Run(New Form1())
                                   End Sub).IsBackground = False
                    FreeConsole()
                    Exit Sub
                Case "help"
                    StartConsole()
                    Console.WriteLine()
                    ShowHelp($"http://localhost:{Settings.Port}/")
                    Exit Sub
                Case Else
                    StartConsole()
                    SendCommandLineUnkown()
                    Exit Sub
            End Select
        ElseIf cmd.Count = 0 Then
            If Not Settings.ShowConsole Then
                FreeConsole()
            Else
                StartConsole()
                InitConsole()
            End If
            Dim server As New Server(Settings)
            server.StartServer()
            If Settings.ShowConsole Then
                AppReady($"http://localhost:{Settings.Port}/")
                ShowHelp($"http://localhost:{Settings.Port}/")
                AppReady($"http://localhost:{Settings.Port}/")
            End If
            Application.Run()
        Else
            StartConsole()
            SendCommandLineUnkown()
            Exit Sub
        End If
    End Sub

#Region "Console"
#Region "Methods"

    Public Shared Sub InitConsole()
        Log.WriteLine($"{My.Application.Info.Title} [Version {My.Application.Info.Version}]")
        Log.WriteLine($"{My.Application.Info.Copyright}, {My.Application.Info.CompanyName}")
        Log.WriteLine("")
    End Sub

    Public Shared Sub AppReady(root As String)
        Log.Write("Application ready: ") : Log.WriteLine(root, ConsoleColor.Green)
        Log.WriteLine("")
    End Sub

    Public Shared Sub ShowHelp(root As String)
        Log.Write("== ") : Log.Write("Web Help", ConsoleColor.White) : Log.WriteLine(" ==================")
        Log.Write("  ") : Log.WriteLine($"{root}sessions/current")
        Log.Write("  ") : Log.WriteLine($"{root}sessions/list")
        Log.Write("  ") : Log.WriteLine("")
        Log.Write("  ") : Log.WriteLine($"{root}sessions/ctrl/current/play")
        Log.Write("  ") : Log.WriteLine($"{root}sessions/ctrl/current/pause")
        Log.Write("  ") : Log.WriteLine($"{root}sessions/ctrl/current/stop")
        Log.Write("  ") : Log.WriteLine($"{root}sessions/ctrl/current/next")
        Log.Write("  ") : Log.WriteLine($"{root}sessions/ctrl/current/previous")
        Log.Write("  ") : Log.WriteLine("")
        Log.Write("  ") : Log.WriteLine($"{root}sessions/ctrl/{{index}}/play")
        Log.Write("  ") : Log.WriteLine($"{root}sessions/ctrl/{{index}}/pause")
        Log.Write("  ") : Log.WriteLine($"{root}sessions/ctrl/{{index}}/stop")
        Log.Write("  ") : Log.WriteLine($"{root}sessions/ctrl/{{index}}/next")
        Log.Write("  ") : Log.WriteLine($"{root}sessions/ctrl/{{index}}/previous")
        Log.WriteLine("==============================")

        Log.WriteLine("")

        Log.Write("== ") : Log.Write("Cmd Help", ConsoleColor.White) : Log.WriteLine(" ==================")
        Log.Write("  ") : Log.Write($"/ui") : Log.Write("          ") : Log.WriteLine("Shows UI")
        Log.Write("  ") : Log.Write($"/help") : Log.Write("        ") : Log.WriteLine("Shows this")
        Log.WriteLine("==============================")

        Log.WriteLine("")

        Log.Write("== ") : Log.Write("Settings Help", ConsoleColor.White) : Log.WriteLine(" =============")
        Log.Write("  ") : Log.Write($"Port") : Log.Write("         ") : Log.WriteLine("Server Port")
        Log.Write("  ") : Log.Write($"CanControl") : Log.Write("   ") : Log.WriteLine("Enables '/sessions/ctrl' endpoint")
        Log.Write("  ") : Log.Write($"ShowConsole") : Log.Write("  ") : Log.WriteLine("Wether the console should be visible during normal operation")
        Log.WriteLine("==============================")

        Log.WriteLine("")

    End Sub

    Public Shared Sub StartConsole()
        If Not AttachConsole(-1) Then
            AllocConsole()
            AttachConsole(Process.GetCurrentProcess().Id)
        End If
        Console.Title = My.Application.Info.Title
    End Sub

    Private Shared Sub SendCommandLineUnkown()
        Console.WriteLine("CommandLine unkown!")
        Console.WriteLine("Run /help to get help...")
        Console.WriteLine()
        Pause()
    End Sub

    Public Shared Sub Pause()
        Console.WriteLine("Press any key to exit...")
        Console.ReadKey()
    End Sub
#End Region
#Region "API"
    <DllImport("kernel32.dll")>
    Private Shared Function FreeConsole() As Boolean : End Function
    <DllImport("kernel32.dll")>
    Private Shared Function AllocConsole() As Boolean : End Function
    <DllImport("kernel32.dll")>
    Private Shared Function AttachConsole(dwProcessId As Integer) As Boolean : End Function
#End Region
#End Region

End Class
