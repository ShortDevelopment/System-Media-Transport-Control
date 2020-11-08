Imports Windows.UI.Xaml.Data

Public Class TimeStampThumbConverter
    Implements IValueConverter

    Public Function Convert(value As Object, targetType As Type, parameter As Object, language As String) As Object Implements IValueConverter.Convert
        Dim t = value.GetType()
        Return TimeSpan.FromSeconds(value).ToString("hh\:mm\:ss")
    End Function

    Public Function ConvertBack(value As Object, targetType As Type, parameter As Object, language As String) As Object Implements IValueConverter.ConvertBack
        Return Double.Parse(value)
    End Function
End Class