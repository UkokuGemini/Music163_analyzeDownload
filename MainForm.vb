Imports System.IO
Imports System.Net
Imports Newtonsoft.Json
Imports Newtonsoft.Json.Linq
Imports System.Text
Imports System.Windows.Forms.VisualStyles.VisualStyleElement
Imports System.Xml

Public Class MainForm
    'Friend TempWebbroswer As New WebBrowser
    'Dim SysDB As DataSet
    'Dim MInfoDB As DataSet
    'Dim LastScanID, LastDownloadID As Integer
    'Dim TryDownloadArr As New ArrayList
    Dim LastScanID As Integer = 0
    Private Sub MainForm_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Me.Text = My.Application.Info.ProductName.ToString & "[Ver." & My.Application.Info.Version.ToString & "]"
        If IO.File.Exists(IniPath) = False Then
            IO.File.WriteAllText(IniPath, 0)
        Else
            Try
                LastScanID = IO.File.ReadAllText(IniPath)
            Catch ex As Exception
            End Try
        End If
        UpdateWebClient = New WebClient
        AddHandler UpdateWebClient.DownloadProgressChanged, AddressOf ShowDownProgress
        AddHandler UpdateWebClient.DownloadFileCompleted, AddressOf ConfigDownloadCompleted
        ScanDelayTimer.Interval = 1000
        GroupBox_Log.Text = "当前扫描ID:" & LastScanID
        ToolStripStatusLabel_St.Text = ""
#Region "SQL暂不使用"
        'DataBaseConnection.ConnectionString = "Data Source=" & DataBasePath
        'SysDB = SQLDataBaseQeury("SELECT * From Sys", DataBaseConnection)
        'MInfoDB = SQLDataBaseQeury("SELECT * From MInfo", DataBaseConnection)
        'Try
        '    LastScanID = Convert.ToInt32(SysDB.Tables(0).Rows(0).Item("LastScanID"))
        'Catch ex As Exception
        '    LastScanID = 0
        'End Try
        'For i = 0 To MInfoDB.Tables(0).Rows.Count - 1
        '    If MInfoDB.Tables(0).Rows(i).Item("DownloadFlag") = False Then
        '        TryDownloadArr.Add(MInfoDB.Tables(0).Rows(i).Item("ID"))
        '    End If
        'Next

#End Region
    End Sub
    Friend IniPath As String = Directory.GetCurrentDirectory & "\ScanID.ini"
    Friend LogPath As String = Directory.GetCurrentDirectory & "\ScanLog.Log"
    Private Sub MainForm_FormClosing(sender As Object, e As FormClosingEventArgs) Handles MyBase.FormClosing
        IO.File.WriteAllText(IniPath, LastScanID)
        IO.File.AppendAllText(LogPath, vbCrLf & TextBox_Log.Text & vbCrLf & "  --  " & Format(Now, "yyyy-MM-dd HH:mm"))
    End Sub
    Sub LogText(ByVal LogStr As String, Optional LogFlag As Boolean = True)
        ToolStripStatusLabel_St.Text = LogStr
        If LogFlag Then
            TextBox_Log.Text &= LogStr & vbCrLf
        End If
    End Sub
    Dim ScanFlag As Boolean = False
    Private WithEvents ScanDelayTimer As New System.Windows.Forms.Timer
    Private Sub ToolStripMenuItem_Scan_Click(sender As Object, e As EventArgs) Handles ToolStripMenuItem_Scan.Click
        ScanFlag = Not ScanFlag
        If ScanFlag Then
            ToolStripMenuItem_Scan.Text = "停止扫描"
            ScanDelayTimer.Enabled = True
        Else
            ToolStripMenuItem_Scan.Text = "扫描"
        End If
    End Sub
    Private Sub ScanDelayTimer_Tick(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ScanDelayTimer.Tick
        ScanDelayTimer.Enabled = False
        ScanDelayTimer.Interval = 1000 + Math.Round(Rnd(), 1) * 2000
        GetPageInfo(LastScanID)
    End Sub
#Region "SQl代码(SQL暂不使用)"
    'Friend DataBasePath As String = Directory.GetCurrentDirectory & "\Music163_analyzeDownload.DB"
    'Friend DataBaseConnection As New SQLite.SQLiteConnection
    'Friend DataBaseDispose As Boolean = False
    'Private ReadOnly DataBaseCommand As New SQLite.SQLiteCommand
    'Public Function SQLDataBaseQeury(ByVal SQLCommand As String, ByVal DataBaseConnection As SQLite.SQLiteConnection) As DataSet
    '    If DataBaseDispose = False Then
    '        Dim DataSetTemp As New DataSet
    '        Try
    '            If DataBaseConnection.State = System.Data.ConnectionState.Closed Then
    '                DataBaseConnection.Open()
    '            End If
    '            DataBaseCommand.Connection = DataBaseConnection
    '            DataBaseCommand.CommandText = SQLCommand
    '            Dim DataBaseAdapter As New SQLite.SQLiteDataAdapter(DataBaseCommand)
    '            DataBaseAdapter.Fill(DataSetTemp)
    '            If DataBaseConnection.State = System.Data.ConnectionState.Open Then
    '                DataBaseConnection.Close()
    '            End If
    '            Return DataSetTemp
    '            DataSetTemp.Dispose()
    '        Catch Ex As Exception
    '            If DataBaseConnection.State = System.Data.ConnectionState.Open Then
    '                DataBaseConnection.Close()
    '            End If
    '            Return DataSetTemp
    '        End Try
    '    Else
    '        Dim NullDataSet As New DataSet
    '        Return NullDataSet
    '    End If
    'End Function '数据库查询
    'Public Sub SQLDataBaseExecute(ByVal SQLCommand As String, ByVal DataBaseConnection As SQLite.SQLiteConnection)
    '    If DataBaseDispose = False Then
    '        Try
    '            If DataBaseConnection.State = System.Data.ConnectionState.Closed Then
    '                DataBaseConnection.Open()
    '            End If
    '            DataBaseCommand.Connection = DataBaseConnection
    '            DataBaseCommand.CommandText = SQLCommand
    '            DataBaseCommand.ExecuteNonQuery()
    '        Catch Ex As Exception
    '        End Try
    '        If DataBaseConnection.State = System.Data.ConnectionState.Open Then
    '            DataBaseConnection.Close()
    '        End If
    '    End If
    'End Sub '数据库操作指令
#End Region
#Region "#Region 下载文件"
    Friend UpdateWebClient As New System.Net.WebClient
    Public Sub DownloadFiles(ByVal UrlStr As String, ByVal TargetPath As String)
        ToolStripProgressBar_Update.Value = 0
        ToolStripStatusLabel_UpdatePer.Text = "0%[ / ]"
        UpdateWebClient.DownloadFileAsync(New Uri(UrlStr), TargetPath)
    End Sub '读取更新配置url
    Public Sub ConfigDownloadCompleted(ByVal sender As Object, ByVal e As System.ComponentModel.AsyncCompletedEventArgs)
        ToolStripProgressBar_Update.Value = 100
        LogText("ID=" & LastScanID & "的歌曲[" & FileNameStr & "] - 下载完成!")
        NextID()
    End Sub '下载完成->启动子
    Private Sub ShowDownProgress(ByVal sender As Object, ByVal e As System.Net.DownloadProgressChangedEventArgs)
        ToolStripProgressBar_Update.Value = e.ProgressPercentage
        ToolStripStatusLabel_UpdatePer.Text = e.ProgressPercentage & "%[" & FileSize(e.BytesReceived) & "/" & FileSize(e.TotalBytesToReceive) & "]"
    End Sub 'ProgressShow
    Function FileSize(ByVal FileByte As Long) As String
        If FileByte < 1048576 Then
            Return Math.Round(FileByte / 1024, 2) & "KB"
        ElseIf FileByte < 1024 Then
            Return Math.Round(FileByte / 1024, 2) & "B"
        Else
            Return Math.Round(FileByte / 1048576, 2) & "MB"
        End If
    End Function
#End Region
    ReadOnly TargetPath As String = Directory.GetCurrentDirectory & "\DownloadFloders\"
#Region "GetCode"
    ReadOnly WebUrl As String = "https://music.163.com/#/song?id="
    ReadOnly FileUrl As String = "http://music.163.com/song/media/outer/url?id="
    Public Structure MInfo
        Dim Name As String
        Dim ID As String
        Dim Artist As String
        Dim code As String
    End Structure
#Region "获取网页源码"
    Function GetWebCode(ByVal strURL As String) As String
        Dim httpReq As System.Net.HttpWebRequest
        Dim httpResp As System.Net.HttpWebResponse
        Dim httpURL As New System.Uri(strURL)
        Dim ioS As System.IO.Stream, charSet As String, tCode As String
        Dim k() As Byte
        ReDim k(0)
        Dim dataQue As New Queue(Of Byte)
        httpReq = CType(WebRequest.Create(httpURL), HttpWebRequest)
        Dim sTime As Date = CDate("1990-09-21 00:00:00")
        httpReq.IfModifiedSince = sTime
        httpReq.Method = "GET"
        httpReq.Timeout = 7000
        Try
            httpResp = CType(httpReq.GetResponse(), HttpWebResponse)
        Catch
            Debug.Print("weberror")
            GetWebCode = "<title>no thing found</title>" : Exit Function
        End Try
        '以上为网络数据获取
        ioS = CType(httpResp.GetResponseStream, Stream)
        Do While ioS.CanRead = True
            Try
                dataQue.Enqueue(ioS.ReadByte)
            Catch
                Debug.Print("read error")
                Exit Do
            End Try
        Loop
        ReDim k(dataQue.Count - 1)
        For j As Integer = 0 To dataQue.Count - 1
            k(j) = dataQue.Dequeue
        Next
        '以上，为获取流中的的二进制数据
        tCode = Encoding.GetEncoding("UTF-8").GetString(k) '获取特定编码下的情况，毕竟UTF-8支持英文正常的显示
        charSet = Replace(GetByDiv2(tCode, "charset=", """"), """", "") '进行编码类型识别
        '以上，获取编码类型
        If charSet = "" Then 'defalt
            If httpResp.CharacterSet = "" Then
                tCode = Encoding.GetEncoding("UTF-8").GetString(k)
            Else
                tCode = Encoding.GetEncoding(httpResp.CharacterSet).GetString(k)
            End If
        Else
            tCode = Encoding.GetEncoding(charSet).GetString(k)
        End If
        Debug.Print(charSet)
        'Stop
        '以上，按照获得的编码类型进行数据转换
        '将得到的内容进行最后处理，比如判断是不是有出现字符串为空的情况
        GetWebCode = tCode
        If tCode = "" Then GetWebCode = "<title>no thing found</title>"
    End Function 'Tools
    Function GetByDiv2(ByVal code As String, ByVal divBegin As String, ByVal divEnd As String)  '获取分隔符所夹的内容[完成，未测试]
        '仅用于获取编码数据
        Dim lgStart As Integer
        Dim lens As Integer
        Dim lgEnd As Integer
        lens = Len(divBegin)
        If InStr(1, code, divBegin) = 0 Then GetByDiv2 = "" : Exit Function
        lgStart = InStr(1, code, divBegin) + CInt(lens)

        lgEnd = InStr(lgStart + 1, code, divEnd)
        If lgEnd = 0 Then GetByDiv2 = "" : Exit Function
        GetByDiv2 = Mid(code, lgStart, lgEnd - lgStart)
    End Function
#End Region
    Dim FileNameStr As String
    Sub GetPageInfo(ByVal ID As String)
        Dim UrlCode As String = GetWebCode(WebUrl & ID)
        If InStr(UrlCode, "很抱歉，你要查找的网页找不到") > 0 Then
            LogText("无ID=" & ID & "的歌曲信息(网页404).")
            NextID()
        Else
            LogText("正在获取ID=" & ID & "的歌曲信息.", False)
            Dim FileName As MInfo = JsonRead(ID)
            If FileName.code = "200" AndAlso FileName.Name <> "" Then
                If FileName.Artist = "" Then
                    FileName.Artist = "UnknownArtists"
                End If
                FileNameStr = FileName.Artist & " - " & FileName.Name
            Else
                FileNameStr = FileName.ID
            End If
            If FileName.code = "-1" Then
                LogText("无ID=" & ID & "的歌曲信息.(无Info信息)")
                NextID()
            Else
                LogText("正在下载ID=" & LastScanID & "的歌曲信息.", False)
                If IO.File.Exists(TargetPath & FileNameStr) Then
                    LogText("已存在ID=" & ID & "的歌曲[" & FileNameStr & "].")
                    NextID()
                Else
                    DownloadFiles(FileUrl & ID, TargetPath & FileNameStr & ".Mp3")
                End If
            End If
        End If
    End Sub
    Sub NextID()
        LastScanID += 1
        GroupBox_Log.Text = "当前扫描ID:" & LastScanID
        If ScanFlag Then
            ScanDelayTimer.Enabled = True
        End If
    End Sub
    Function JsonRead(ByVal Id As String) As MInfo
        Dim jsonUrl As String = "http://music.163.com/api/song/detail/?id=" & Id & "&ids=%5B" & Id & "%5D"
        Dim UrlCode As String = GetWebCode(jsonUrl)
        Dim TempMInfo As New MInfo
        Dim JsonObj_Code As New With {.code = ""}
        JsonObj_Code = JsonConvert.DeserializeAnonymousType(UrlCode, JsonObj_Code) '//->Code
        TempMInfo.code = JsonObj_Code.code
        If JsonObj_Code.code = "200" Then
            Dim JsonObjStr As JObject = CType(JsonConvert.DeserializeObject(UrlCode), JObject)
            If CType(JsonObjStr("songs"), JArray).Count > 0 Then
                UrlCode = CType(JsonObjStr("songs"), JArray).Item(0).ToString '//->songs[]
                Dim JsonObj_Name As New With {.name = ""}
                TempMInfo.Name = JsonConvert.DeserializeAnonymousType(UrlCode, JsonObj_Name).name.ToString
                JsonObjStr = CType(JsonConvert.DeserializeObject(UrlCode), JObject)
                Dim ArtstCount As Integer = CType(JsonObjStr("artists"), JArray).Count
                For i = 0 To ArtstCount - 1
                    UrlCode = CType(JsonObjStr("artists"), JArray).Item(i).ToString '//->artists[]
                    Dim JsonObj_ArtistName As New With {.name = ""}
                    TempMInfo.Artist &= JsonConvert.DeserializeAnonymousType(UrlCode, JsonObj_ArtistName).name.ToString
                    If i < ArtstCount - 1 Then
                        TempMInfo.Artist &= "&"
                    End If
                Next
                TempMInfo.ID = Id
            Else
                TempMInfo.code = "-1"
            End If
        End If
        Return TempMInfo
    End Function
#End Region
    Private Sub 打开目录ToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles 打开目录ToolStripMenuItem.Click
        Diagnostics.Process.Start(Directory.GetCurrentDirectory & "\")
    End Sub
    Private Sub 打开下载目录ToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles 打开下载目录ToolStripMenuItem.Click
        Diagnostics.Process.Start(TargetPath)
    End Sub
    Private Sub 更改IDToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles 更改IDToolStripMenuItem.Click
        ScanDelayTimer.Enabled = False
        Try
            LastScanID = Convert.ToInt32(ToolStripTextBox_Changeid.Text)
        Catch ex As Exception
            ToolStripTextBox_Changeid.Text = ""
        End Try
        ScanDelayTimer.Enabled = True
    End Sub
    Private Sub TextBox_Log_TextChanged(sender As Object, e As EventArgs) Handles TextBox_Log.TextChanged
        TextBox_Log.SelectionStart = TextBox_Log.TextLength
        TextBox_Log.ScrollToCaret()
    End Sub
    Private Sub MainForm_Resize(sender As Object, e As EventArgs) Handles MyBase.Resize
        ToolStripProgressBar_Update.Size = New Size(Me.Size.Width - 250, 20)
    End Sub
End Class