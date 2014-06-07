Public Class SharedMethods

    ''' <summary>
    ''' Takes an object, and returns a valid string
    ''' </summary>
    ''' <param name="stringValue">the object in question</param>
    ''' <returns>a string object</returns>
    ''' <remarks></remarks>
    Public Shared Function FormatString(ByVal stringValue As Object, treatZeroAsEmpty As Boolean) As String
        If (stringValue Is Nothing) Or (stringValue Is DBNull.Value) Then
            Return String.Empty
        Else
            If stringValue.ToString.Trim.Length > 0 Then
                If (stringValue.GetType Is GetType(Integer)) AndAlso stringValue.ToString.Trim = "0" AndAlso treatZeroAsEmpty Then   'treating zeros as nulls
                    Return String.Empty
                Else
                    Return stringValue.ToString.Trim
                End If
            Else
                Return String.Empty
            End If
        End If
    End Function
    Public Shared Function FormatString(ByVal stringValue As Object) As String
        Return FormatString(stringValue, True)
    End Function

    ''' <summary>
    ''' Takes an object, and returns a valid boolean value
    ''' </summary>
    ''' <param name="boolValue">the object in question</param>
    ''' <returns>nullable boolean object</returns>
    ''' <remarks>check HasValue to make sure the conversion was possible</remarks>
    Public Shared Function FormatBooleanObject(ByVal boolValue As Object) As Nullable(Of Boolean)
        Dim theReturn As Nullable(Of Boolean)
        If Not boolValue Is Nothing AndAlso boolValue IsNot DBNull.Value AndAlso boolValue.ToString.Length > 0 AndAlso boolValue IsNot Windows.DependencyProperty.UnsetValue Then
            theReturn = CBool(boolValue)
        Else
            theReturn = Nothing
        End If
        Return theReturn
    End Function

End Class
