Imports System.Runtime.CompilerServices

Module Extensions
    <Extension>
    Public Function ToUWPColor(ByRef color As Color) As Windows.UI.Color
        Return Windows.UI.Color.FromArgb(color.A, color.R, color.G, color.B)
    End Function
End Module