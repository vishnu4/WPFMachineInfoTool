Imports System.Management
Imports System.Text
Imports System.Data

Class Window1

    Public Enum InfoType
        BIOS
        HD
        MAC
    End Enum

    Private Sub LoadGrid(ByVal theInfo As Specialized.StringDictionary)
        Dim theObjProv As ObjectDataProvider = CType(Me.FindResource("infoList"), ObjectDataProvider)
        theObjProv.ObjectInstance = theInfo
        theObjProv.Refresh()
        'now select the default
        grdInfo.UnselectAll()
        For Each dr As DictionaryEntry In grdInfo.Items
            Select Case Me.SelectedInfoType
                Case InfoType.BIOS
                    If String.Compare("serialnumber", SharedMethods.FormatString(dr.Key), True) = 0 Then
                        grdInfo.SelectedItem = dr
                        grdInfo.ScrollIntoView(dr)
                        Exit For
                    End If
                Case InfoType.HD
                    If String.Compare("serialnumber", SharedMethods.FormatString(dr.Key), True) = 0 Then
                        grdInfo.SelectedItem = dr
                        grdInfo.ScrollIntoView(dr)
                        Exit For
                    End If
                Case InfoType.MAC
                    If String.Compare("MacAddress", SharedMethods.FormatString(dr.Key), True) = 0 Then
                        grdInfo.SelectedItem = dr
                        grdInfo.ScrollIntoView(dr)
                        Exit For
                    End If
            End Select
        Next
    End Sub

    Private Function GetBIOSInfo() As Specialized.StringDictionary
        Dim theReturn As New Specialized.StringDictionary
        Try
            Dim objBIOS As ManagementClass = New ManagementClass("Win32_Bios")
            Dim mocA As Management.ManagementObjectCollection = objBIOS.GetInstances()

            If mocA.Count > 1 Then
                WriteStatus("More than 1 BIOS information found.  Topmost listed.")
            End If
            For Each a As ManagementObject In mocA
                For Each p As PropertyData In a.Properties
                    theReturn.Add(p.Name, SharedMethods.FormatString(p.Value))
                Next
            Next
        Catch ex As Exception
            WriteStatus(ex.Message)
        End Try
        Return theReturn
    End Function

    Private Function GetHDCode() As Specialized.StringDictionary
        Dim theReturn As New Specialized.StringDictionary

        Try
            Dim objHD As ManagementClass = New ManagementClass("Win32_PhysicalMedia")
            Dim mocC As Management.ManagementObjectCollection = objHD.GetInstances()

            If mocC.Count > 1 Then
                WriteStatus("More than 1 HD information found.  Topmost listed.")
            End If

            For Each c As ManagementObject In mocC
                If Not CType(c.Properties("Removable").Value, Boolean) Then
                    'only care about the first available HD
                    For Each p As PropertyData In c.Properties
                        theReturn.Add(p.Name, SharedMethods.FormatString(p.Value))
                    Next
                    Exit For
                End If
            Next

        Catch ex As Exception
            WriteStatus(ex.Message)
        End Try

        Return theReturn
    End Function

    Public Function GetMACAddress() As Specialized.StringDictionary
        Dim theReturn As New Specialized.StringDictionary
        Try
            Dim objMAC As Management.ManagementClass = New Management.ManagementClass("Win32_NetworkAdapterConfiguration")
            Dim mocMAC As Management.ManagementObjectCollection
            mocMAC = objMAC.GetInstances()

            For Each mo As Management.ManagementObject In mocMAC
                If SharedMethods.FormatBooleanObject(mo("IPEnabled")).Value Then
                    For Each p As PropertyData In mo.Properties
                        theReturn.Add(p.Name, SharedMethods.FormatString(p.Value))
                    Next
                    Exit For
                End If
            Next
        Catch ex As Exception
            WriteStatus(ex.Message)
        End Try
        Return theReturn
    End Function

    Private Shared Function RemoveNonAlphaNumeric(ByVal str As String) As String
        Dim sb As StringBuilder
        Dim chr As Char
        sb = New StringBuilder(str.Length)

        Try
            For Each chr In str
                If Char.IsLetterOrDigit(chr) Then
                    sb.Append(chr)
                End If
            Next chr

            Return sb.ToString()
        Finally
            sb = Nothing
        End Try
    End Function

    Delegate Sub WriteStatusDel(ByVal theStatus As String)

    Private Sub WriteStatus(ByVal theStatus As String)
        If Not Me.Dispatcher.CheckAccess Then
            Me.Dispatcher.Invoke(Windows.Threading.DispatcherPriority.Normal, New WriteStatusDel(AddressOf WriteStatus), theStatus)
        Else
            If curStat IsNot Nothing Then
                curStat.Content = theStatus
            End If
        End If
    End Sub

    Private ReadOnly Property SelectedInfoType() As Nullable(Of InfoType)
        Get
            Dim cmbItem As ComboBoxItem = CType(cmbInfoType.SelectedValue, ComboBoxItem)
            If cmbItem IsNot Nothing Then
                Select Case cmbItem.Content.ToString
                    Case "BIOS"
                        Return InfoType.BIOS
                    Case "HD"
                        Return InfoType.HD
                    Case "MAC"
                        Return InfoType.MAC
                    Case Else
                        Return Nothing
                End Select
            Else
                Return Nothing
            End If
        End Get
    End Property

    Private Sub cmbInfoType_SelectionChanged(ByVal sender As Object, ByVal e As System.Windows.Controls.SelectionChangedEventArgs) Handles cmbInfoType.SelectionChanged
        Try
            If curStat IsNot Nothing Then
                Select Case SelectedInfoType
                    Case InfoType.BIOS
                        LoadGrid(GetBIOSInfo())
                        WriteStatus("BIOS Information")
                    Case InfoType.HD
                        LoadGrid(GetHDCode)
                        WriteStatus("Hard Drive information")
                    Case InfoType.MAC
                        LoadGrid(GetMACAddress)
                        WriteStatus("MAC Address information")
                    Case Else
                        WriteStatus("Type not recognized")
                End Select
            End If
        Catch ex As Exception
            WriteStatus(ex.Message)
        End Try
    End Sub


End Class
