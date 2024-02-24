﻿Imports System.IO
Imports System.Net
Imports Newtonsoft.Json
Imports Newtonsoft.Json.Linq
Imports System.Text
Imports System.Windows.Forms.VisualStyles.VisualStyleElement
Imports System.Xml
Imports System.Runtime.CompilerServices.RuntimeHelpers
Imports System.Security.AccessControl
Imports System.Net.WebRequestMethods

Public Class MainForm
    Dim LastScanID As Integer = 0
    ReadOnly WebUrl As String = "https://music.163.com/#/song?id="
    ReadOnly FileUrl As String = "http://music.163.com/song/media/outer/url?id="
    ReadOnly ListUrl As String = "https://music.163.com/api/playlist/detail?id="
    ReadOnly TargetPath As String = Directory.GetCurrentDirectory & "\DownloadFloders\"
    ReadOnly IniPath As String = Directory.GetCurrentDirectory & "\ScanID.ini"
    ReadOnly LogPath As String = Directory.GetCurrentDirectory & "\ScanLog.Log"
    ReadOnly ClockPath As String = Directory.GetCurrentDirectory & "\Clock.Ini"
    Private Sub MainForm_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Me.CenterToScreen()
        CheckAutoRecommand()
        Me.Text = My.Application.Info.ProductName.ToString & "[Ver." & My.Application.Info.Version.ToString & "]"
        If IO.File.Exists(IniPath) = False Then
            IO.File.WriteAllText(IniPath, 0)
        Else
            Try
                LastScanID = IO.File.ReadAllText(IniPath)
            Catch ex As Exception
            End Try
        End If
        ScanWebClient = New WebClient
        RecommandWebClient = New WebClient
        AddHandler ScanWebClient.DownloadProgressChanged, AddressOf ShowScanDownProgress
        AddHandler ScanWebClient.DownloadFileCompleted, AddressOf ScanDownloadCompleted
        AddHandler RecommandWebClient.DownloadProgressChanged, AddressOf ShowRecommandDownProgress
        AddHandler RecommandWebClient.DownloadFileCompleted, AddressOf RecommandDownloadCompleted
        AddHandler RecommandWebClient.DownloadProgressChanged, AddressOf ShowListDownProgress
        AddHandler RecommandWebClient.DownloadFileCompleted, AddressOf ListDownloadCompleted
        ScanDelayTimer.Interval = 1000
        FreshTimer.Interval = 60000
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
    Dim TrueClose As Boolean = False
    Private Sub MainForm_FormClosing(sender As Object, e As FormClosingEventArgs) Handles MyBase.FormClosing
        If TrueClose Then
            IO.File.WriteAllText(IniPath, LastScanID)
            IO.File.AppendAllText(LogPath, vbCrLf & TextBox_Log.Text & vbCrLf & "  - -  " & Format(Now, "yyyy-MM-dd HH:mm"))
            NotifyIcon1.Dispose()
        Else
            e.Cancel = True
            NotifyIcon1.Visible = True
            Me.WindowState = FormWindowState.Minimized
        End If
    End Sub
#Region "Ui"
    Private Sub 打开软件目录ToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles 打开软件目录ToolStripMenuItem.Click
        Diagnostics.Process.Start(Directory.GetCurrentDirectory & "\")
    End Sub
    Private Sub 打卡下载目录ToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles 打卡下载目录ToolStripMenuItem.Click
        Diagnostics.Process.Start(TargetPath)
    End Sub
    Private Sub 退出XToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles 退出XToolStripMenuItem.Click
        TrueClose = True
        Me.Close()
    End Sub
    Private Sub 设置开机启动ToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles 设置开机启动ToolStripMenuItem.Click
        Try
            CreateStartup()
            设置开机启动ToolStripMenuItem.Text = "设置开机启动√(&O)"
            MsgBox("设置成功.")
        Catch ex As Exception
            设置开机启动ToolStripMenuItem.Text = "设置开机启动(&O)"
            MsgBox("设置失败.")
        End Try
    End Sub
    Private Sub 设置自动解析每日歌曲时间ToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles 设置自动解析每日歌曲时间ToolStripMenuItem.Click
        设置自动解析每日歌曲时间ToolStripMenuItem.Visible = False
        IO.File.WriteAllText(ClockPath, 7)
        Diagnostics.Process.Start(ClockPath)
        LogText("* 已建立自动解析每日歌曲时间,重启后生效.")
    End Sub
    Private Sub CreateStartup()
        Dim WScript_T As Object = CreateObject("WScript.Shell")
        Dim StartupPath = Environment.GetFolderPath(Environment.SpecialFolder.Startup)
        Dim AppName = My.Application.Info.ProductName
        'Dim desk As String = WScript_T.SpecialFolders("Desktop")
        Dim AppInk As Object = WScript_T.CreateShortcut(StartupPath & "\" & AppName & ".exe.lnk")
        'If Not IO.File.Exists(AppInk) Then
        With AppInk
            .Description = "Music163_analyzeDownload.Ink"
            .IconLocation = Application.StartupPath + "\" + My.Application.Info.AssemblyName & ".exe,0"
            .TargetPath = Application.StartupPath + "\" + My.Application.Info.AssemblyName & ".exe "
            .WindowStyle = 7 '打开窗体的风格，最小化
            .WorkingDirectory = Application.StartupPath '工作路径
            .Save() '保存快捷方式
        End With
        'End If
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
    Dim OnContinueScan As Boolean = False
    Dim RecommandStep As Integer = 0
    Private Sub ToolStripMenuItem_Scan_Click(sender As Object, e As EventArgs) Handles ToolStripMenuItem_ScanButton.Click
        ScanFlag = Not ScanFlag
        If ScanFlag Then
            ToolStripTextBox_Changeid.Text = ""
            ToolStripMenuItem_ScanButton.Text = "停止扫描"
            ScanDelayTimer.Enabled = True
        Else
            ToolStripMenuItem_ScanButton.Text = "扫描"
        End If
    End Sub
    Dim StopFlag_GetRecommandIDTimer, StopFlag_DownloadRecommandTimer, StopFlag_DownloadListTimer As Integer
    Private Sub ToolStripSplitButton_Recommand_ButtonClick(sender As Object, e As EventArgs) Handles ToolStripSplitButton_Recommand.ButtonClick
        RecommandFlag = Not RecommandFlag
        If RecommandFlag Then
            ToolStripSplitButton_Recommand.Text = "停止解析每日歌单."
            If ScanFlag Then
                ToolStripMenuItem_Scan_Click(Nothing, Nothing)
                OnContinueScan = True
            End If
            ToolStripSplitButton1.Enabled = False
            ToolStripMenuItem_ScanButton.Enabled = False
            更改IDToolStripMenuItem.Enabled = False
            RecommandStep = 1
            StopFlag_GetRecommandIDTimer = 0
            StopFlag_DownloadRecommandTimer = 0
            GetRecommand()
        Else
            ToolStripSplitButton_Recommand.Text = "解析并下载每日歌单."
            Dim StopMsgResult As Integer = MsgBox("是否下载已解析的歌曲?", MsgBoxStyle.YesNo, "已解析:" & GetRecommandIDIndex & "首歌曲.")
            ToolStripSplitButton_Recommand.Enabled = False
            Select Case RecommandStep
                Case 2
                    StopFlag_GetRecommandIDTimer = -1
                    GetRecommandIDTimer.Interval = 100
                    GetRecommandIDTimer.Enabled = True
                    If StopMsgResult <> MsgBoxResult.Yes Then
                        StopFlag_DownloadRecommandTimer = -1 '//不能填StopFlag_DownloadRecommandTimer = -1,否则不下载
                        DownloadRecommandTimer.Interval = 100
                    End If
                Case 3
                    If StopMsgResult <> MsgBoxResult.Yes Then
                        StopFlag_DownloadRecommandTimer = -1
                        DownloadRecommandTimer.Interval = 100
                        DownloadRecommandTimer.Enabled = True
                    End If
            End Select
        End If
    End Sub
    Private Sub TextBox_Log_TextChanged(sender As Object, e As EventArgs) Handles TextBox_Log.TextChanged
        TextBox_Log.SelectionStart = TextBox_Log.TextLength
        TextBox_Log.ScrollToCaret()
    End Sub
    Private Sub MainForm_Resize(sender As Object, e As EventArgs) Handles MyBase.Resize
        ToolStripProgressBar_Update.Size = New Size(Me.Size.Width - 250, 20)
    End Sub
    Sub LogText(ByVal LogStr As String, Optional LogFlag As Boolean = True)
        ToolStripStatusLabel_St.Text = LogStr
        If LogFlag Then
            TextBox_Log.Text &= LogStr & vbCrLf
        End If
    End Sub
#End Region
#Region "Notify"
    Private Sub FormMain_MinimumSizeChanged(sender As Object, e As EventArgs) Handles MyBase.Resize
        If Me.WindowState = FormWindowState.Minimized Then
            Me.ShowInTaskbar = False
            NotifyIcon1.Visible = True
        Else
            Me.ShowInTaskbar = True
        End If
    End Sub '最小化
    Private Sub NotifyIcon1_MouseClick(sender As Object, e As MouseEventArgs) Handles NotifyIcon1.MouseClick
        If e.Button = Windows.Forms.MouseButtons.Left Then
            Me.WindowState = FormWindowState.Normal
            Me.TopMost = False
            Me.TopMost = True
            Me.ShowInTaskbar = True
            NotifyIcon1.Visible = False
        End If
    End Sub
#End Region
#Region "下载文件"
    Friend ScanWebClient As New System.Net.WebClient
    Friend RecommandWebClient As New System.Net.WebClient
    Friend ListWebClient As New System.Net.WebClient
    Public Sub DownloadFiles(ByVal UrlStr As String, ByVal TargetPath As String, ByVal ScanDownloadType As Boolean)
        ToolStripProgressBar_Update.Value = 0
        ToolStripStatusLabel_UpdatePer.Text = "0%[ / ]"
        If ScanDownloadType Then
            ScanWebClient.DownloadFileAsync(New Uri(UrlStr), TargetPath)
        Else
            RecommandWebClient.DownloadFileAsync(New Uri(UrlStr), TargetPath)
        End If
    End Sub '读取更新配置url
    Public Sub ScanDownloadCompleted(ByVal sender As Object, ByVal e As System.ComponentModel.AsyncCompletedEventArgs)
        ToolStripProgressBar_Update.Value = 100
        LogText("ID=" & LastScanID & "的歌曲[" & FileNameStr & "] - 下载完成!")
        NextID(True)
    End Sub '下载完成->启动子
    Private Sub ShowScanDownProgress(ByVal sender As Object, ByVal e As System.Net.DownloadProgressChangedEventArgs)
        ToolStripProgressBar_Update.Value = e.ProgressPercentage
        ToolStripStatusLabel_UpdatePer.Text = e.ProgressPercentage & "%[" & FileSize(e.BytesReceived) & "/" & FileSize(e.TotalBytesToReceive) & "]"
    End Sub 'ProgressShow
    Public Sub RecommandDownloadCompleted(ByVal sender As Object, ByVal e As System.ComponentModel.AsyncCompletedEventArgs)
        ToolStripProgressBar_Update.Value = 100
        RecommandDownloadSuccessSum += 1
        LogText("(" & RecommandDownloadSuccessSum & "/" & GetSingleRecommandCount & ").""ID=" & NowDownloadRecommandId & "的歌曲[" & FileNameStr & "] - 下载完成!")
        SuccessDownloadKey = NowDownloadKey
        NextID(False)
    End Sub '下载完成->启动子
    Private Sub ShowRecommandDownProgress(ByVal sender As Object, ByVal e As System.Net.DownloadProgressChangedEventArgs)
        ToolStripProgressBar_Update.Value = e.ProgressPercentage
        ToolStripStatusLabel_UpdatePer.Text = e.ProgressPercentage & "%[" & FileSize(e.BytesReceived) & "/" & FileSize(e.TotalBytesToReceive) & "]"
    End Sub 'ProgressShow
    Public Sub ListDownloadCompleted(ByVal sender As Object, ByVal e As System.ComponentModel.AsyncCompletedEventArgs)
        ToolStripProgressBar_Update.Value = 100
        ListIDIndex += 1
        LogText("歌单(ID=" & ListId & ")(" & ListIDIndex & "/" & ListArr.Count & ").""ID=" & NowDownloadListId & "的歌曲[" & ListFileNameStr & "] - 下载完成!")
        DownloadListTimer.Enabled = True
    End Sub '下载完成->启动子
    Private Sub ShowListDownProgress(ByVal sender As Object, ByVal e As System.Net.DownloadProgressChangedEventArgs)
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
#Region "ID削刮"
    Public Structure MInfo
        Dim Name As String
        Dim ID As String
        Dim Artist As String
        Dim code As String
    End Structure
    Dim FileNameStr, ListFileNameStr As String
    Dim ScanFlag As Boolean = False
    Dim RecommandFlag As Boolean = False
    Sub GetPageInfo(ByVal ID As String, ByVal ScanGetType As Boolean)
        Dim UrlCode As String = GetWebCode(WebUrl & ID)
        If InStr(UrlCode, "很抱歉，你要查找的网页找不到") > 0 Then
            LogText("无ID=" & ID & "的歌曲信息(网页404).")
            NextID(ScanGetType)
        Else
            LogText("正在获取ID=" & ID & "的歌曲信息.", False)
            Dim FileName As MInfo = JsonRead(ID)
            FileNameStr = ReturnFileNameStr(FileName)
            If FileName.code = "-1" Then
                LogText("无ID=" & ID & "的歌曲信息.(无Info信息)")
                NextID(ScanGetType)
            Else
                LogText("正在下载ID=" & ID & "的歌曲信息.", False)
                If IO.File.Exists(TargetPath & FileNameStr) Then
                    LogText("已存在ID=" & ID & "的歌曲[" & FileNameStr & "].")
                    NextID(ScanGetType)
                Else
                    If ScanGetType = False Then
                        NowDownloadRecommandId = ID
                    End If
                    DownloadFiles(FileUrl & ID, TargetPath & FileNameStr & ".Mp3", ScanGetType)
                End If
            End If
        End If
    End Sub
    Function ReturnFileNameStr(ByVal TempMInfo As MInfo) As String
        If TempMInfo.code = "200" AndAlso TempMInfo.Name <> "" Then
            If TempMInfo.Artist = "" Then
                TempMInfo.Artist = "Unknown"
            End If
            Return TempMInfo.Artist & " - " & TempMInfo.Name
        Else
            Return TempMInfo.ID
        End If
    End Function
    Sub NextID(ByVal ScanType As Boolean)
        If ScanType Then
            LastScanID += 1
            GroupBox_Log.Text = "当前扫描ID:" & LastScanID
            If ScanFlag Then
                ScanDelayTimer.Enabled = True
            End If
        Else
            RecommandIDIndex += 1
            DownloadRecommandTimer.Enabled = True
        End If
    End Sub
    Function JsonRead(ByVal Id As String) As MInfo
        Dim jsonUrl As String = "http://music.163.com/api/song/detail/?id=" & Id & "&ids=%5B" & Id & "%5D"
        Dim UrlCode As String = GetWebCode(jsonUrl)
        Dim TempMInfo As New MInfo
        Dim JsonObj_Code As New With {.code = ""}
        Try
            JsonObj_Code = JsonConvert.DeserializeAnonymousType(UrlCode, JsonObj_Code) '//->Code
        Catch ex As Exception
            JsonObj_Code.code = "-1"
        End Try
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
    Private WithEvents ScanDelayTimer As New System.Windows.Forms.Timer
    Private Sub ScanDelayTimer_Tick(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ScanDelayTimer.Tick
        ScanDelayTimer.Enabled = False
        ScanDelayTimer.Interval = 1000 + Math.Round(Rnd(), 1) * 2000
        GetPageInfo(LastScanID, True)
    End Sub
#End Region
#Region "每日歌曲"
    ReadOnly Api_Recommand As String = "http://openapi.music.163.com/openapi/music/basic/recommend/songlist/get/v2?appId=a301010000000000aadb4e5a28b45a67&bizContent=%7B%22limit%22%3A100%7D&signType=RSA_SHA256&accessToken=x46c13d33a898ad1d257c5009a1daadfced5a1160176c2309y&device=%7B%22deviceType%22%3A%22andrwear%22%2C%22os%22%3A%22andrwear%22%2C%22appVer%22%3A%220.1%22%2C%22channel%22%3A%22hm%22%2C%22model%22%3A%22kys%22%2C%22deviceId%22%3A%22321%22%2C%22brand%22%3A%22hm%22%2C%22osVer%22%3A%228.1.0%22%7D&timestamp="
    Dim RecommandNameArr As New ArrayList
    Dim RecommandIDArr As New ArrayList
    Dim ListArr As New ArrayList
    Sub GetRecommand()
        SuccessDownloadKey = ""
        NowDownloadKey = ""
        RecommandNameArr.Clear()
        RecommandIDArr.Clear()
        Dim Api_Recommand_Now As String = Api_Recommand & DateTimeOffset.UtcNow.ToUnixTimeSeconds.ToString & "000"
        Dim UrlCode As String = GetWebCode(Api_Recommand_Now)
        Dim JsonObj_Code As New With {.code = ""}
        Try
            JsonObj_Code = JsonConvert.DeserializeAnonymousType(UrlCode, JsonObj_Code) '//->Code
        Catch ex As Exception
            JsonObj_Code.code = "-1"
        End Try
        If JsonObj_Code.code = "200" Then
            Dim JsonObjStr As JObject = CType(JsonConvert.DeserializeObject(UrlCode), JObject)
            Dim MCount As Integer = CType(JsonObjStr("data"), JArray).Count
            If MCount > 0 Then
                For i = 0 To MCount - 1
                    UrlCode = CType(JsonObjStr("data"), JArray).Item(i).ToString '//->data[]
                    Dim JsonObj_Name As New With {.name = ""}
                    Dim MName As String = JsonConvert.DeserializeAnonymousType(UrlCode, JsonObj_Name).name.ToString
                    RecommandNameArr.Add(MName)
                Next
                LogText("正在解析今日歌单:" & RecommandNameArr.Count & "首.")
                GetRecommandIDIndex = 0
                GetRecommandIDIndexCount = RecommandNameArr.Count
                GetSingleRecommandCount = 0
                RecommandIDIndex = 0
                RecommandDownloadSuccessSum = 0
                GetRecommandIDTimer.Interval = 1000
                RecommandStep = 2
                StopFlag_GetRecommandIDTimer = GetRecommandIDIndexCount
                GetRecommandIDTimer.Enabled = True
            Else
                RecommandStep = 0
                LogText("未获取到今日歌单.")
            End If
        Else
            RecommandStep = 0
            LogText("今日歌单获取失败.")
        End If
    End Sub
    Private WithEvents GetRecommandIDTimer As New System.Windows.Forms.Timer
    Private Sub GetRecommandIDTimer_Tick(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles GetRecommandIDTimer.Tick
        GetRecommandIDTimer.Enabled = False
        If GetRecommandIDIndex < 4 Then ' Math.Min(GetRecommandIDIndexCount, StopFlag_GetRecommandIDTimer) Then
            If Delay_Flag Then
                LogText("正在解析今日歌单:(" & GetRecommandIDIndex + 1 & "/" & GetRecommandIDIndexCount & ")首. - " & "[" & RecommandNameArr(GetRecommandIDIndex) & "]" & "延迟等待:" & Int(GetRecommandIDTimer.Interval / 1000) & "s.", Not Delay_Flag)
            Else
                LogText("正在解析今日歌单:(" & GetRecommandIDIndex + 1 & "/" & GetRecommandIDIndexCount & ")首. - " & "[" & RecommandNameArr(GetRecommandIDIndex) & "]")
            End If
            GetRecommandMId(RecommandNameArr(GetRecommandIDIndex))
        Else
            Delay_Plus = 0
            LogText("解析今日歌单:" & GetSingleRecommandCount & "首.(" & RecommandIDArr.Count & "条歌曲数据)")
            RecommandStep = 3
            If StopFlag_DownloadRecommandTimer = -1 Then
                RecommandStep = 0
                RecommandIDArr.Clear()
                LogText(" - - 今日歌单解析已中止!.")
                ToolStripSplitButton_Recommand.Enabled = True
                ToolStripMenuItem_ScanButton.Enabled = True
                更改IDToolStripMenuItem.Enabled = True
                ToolStripSplitButton1.Enabled = True
                If OnContinueScan Then
                    OnContinueScan = False
                    ToolStripMenuItem_ScanButton.Text = "停止扫描"
                    ScanDelayTimer.Enabled = True
                    ScanFlag = True
                End If
            Else
                DownloadRecommand()
            End If
        End If
    End Sub
    Public Structure SearchInfo
        Dim Key As String
        Dim ID As String
    End Structure
    Dim GetRecommandIDIndex As Integer '//NameArr 当前第几条
    Dim GetRecommandIDIndexCount As Integer '//NameArr 词条总数(需要下载的歌曲数量)
    Dim GetSingleRecommandCount As Integer '//实际下载歌曲数(与GetRecommandIDIndexCount差在有Key但没有搜索到)
    Dim RecommandIDIndex As Integer '//获取链接条目数
    Dim RecommandDownloadSuccessSum As Integer '//成功下载数
    Dim ListIDIndex As Integer '//
    Dim ListDownloadSuccessSum As Integer '//
    Dim Delay_Plus As Integer
    Dim Delay_Flag As Boolean
    Sub GetRecommandMId(ByVal SearchKey As String)
        Dim SearchUrl As String = "https://music.163.com/api/search/get/web?csrf_token=hlpretag=&hlposttag=&s=" & SearchKey & "&type=1&offset=0&total=true&limit=5"
        Dim UrlCode As String = GetWebCode(SearchUrl)
        Dim JsonObj_Code As New With {.code = ""}
        JsonObj_Code = JsonConvert.DeserializeAnonymousType(UrlCode, JsonObj_Code) '//->Code
        Delay_Flag = False
        If JsonObj_Code.code = "200" Then
            Dim JsonObj_res As JObject = CType(JsonConvert.DeserializeObject(UrlCode), JObject)
            Dim JsonObj_songs As JObject = CType(JsonConvert.DeserializeObject(JsonObj_res("result").ToString), JObject)
            Dim MCount As Integer = CType(JsonObj_songs("songs"), JArray).Count
            If MCount > 0 Then
                GetSingleRecommandCount += 1
                For i = 0 To MCount - 1
                    Dim JsonObj_ID As JObject = CType(JsonConvert.DeserializeObject(JsonObj_songs("songs")(i).ToString), JObject)
                    Dim TempSearchInfo As SearchInfo
                    TempSearchInfo.ID = JsonObj_ID("id")
                    TempSearchInfo.Key = SearchKey
                    If TempSearchInfo.ID.Length > 0 Then
                        RecommandIDArr.Add(TempSearchInfo)
                    End If
                Next
            End If
            GetRecommandIDIndex += 1
            GetRecommandIDTimer.Interval = 1000 + Math.Round(Rnd(), 1) * 2000
        ElseIf JsonObj_Code.code = "406" OrElse JsonObj_Code.code = "405" OrElse JsonObj_Code.code = "-447" Then
            Delay_Flag = True
            Delay_Plus += 15000
            If Delay_Plus >= 3600000 Then
                GetRecommandIDIndex += 1
                Delay_Plus = 1800000
            End If
            Delay_Plus = Math.Min(1800000, Delay_Plus)
            GetRecommandIDTimer.Interval = 1000 + Math.Round(Rnd(), 1) * 2000 + Delay_Plus
        Else
            GetRecommandIDIndex += 1
            GetRecommandIDTimer.Interval = 1000 + Math.Round(Rnd(), 1) * 2000
        End If
        GetRecommandIDTimer.Enabled = True
    End Sub
    Sub DownloadRecommand()
        LogText("开始下载今日歌单:" & GetSingleRecommandCount & "首.(" & RecommandIDArr.Count & "条歌曲数据)")
        StopFlag_DownloadRecommandTimer = RecommandIDArr.Count
        DownloadRecommandTimer.Interval = 1000
        DownloadRecommandTimer.Enabled = True
    End Sub
    Private WithEvents DownloadRecommandTimer As New System.Windows.Forms.Timer
    Dim SuccessDownloadKey, NowDownloadKey As String
    Dim NowDownloadRecommandId, NowDownloadListId As String
    Private Sub DownloadRecommandTimer_Tick(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles DownloadRecommandTimer.Tick
        DownloadRecommandTimer.Enabled = False
        DownloadRecommandTimer.Interval = 1000 + Math.Round(Rnd(), 1) * 2000
        If RecommandIDIndex < Math.Min(RecommandIDArr.Count - 1, StopFlag_DownloadRecommandTimer) Then
            Dim TempSearchInfo As SearchInfo = RecommandIDArr(RecommandIDIndex)
            If SuccessDownloadKey <> TempSearchInfo.Key OrElse (SuccessDownloadKey = "" AndAlso TempSearchInfo.Key = "") Then
                NowDownloadKey = TempSearchInfo.Key
                GetPageInfo(TempSearchInfo.ID, False)
            Else
                LogText("已跳过类似[ " & TempSearchInfo.Key & " ]歌曲:ID=" & TempSearchInfo.ID, False)
                DownloadRecommandTimer.Interval = 100
                NextID(False)
            End If
        Else
            RecommandStep = 0
            RecommandIDArr.Clear()
            LogText(" - - 今日歌单下载结束!总计下载:" & RecommandDownloadSuccessSum & "首.")
            ToolStripMenuItem_ScanButton.Enabled = True
            ToolStripSplitButton_Recommand.Enabled = True
            更改IDToolStripMenuItem.Enabled = True
            ToolStripSplitButton1.Enabled = True
            If OnContinueScan Then
                OnContinueScan = False
                ToolStripMenuItem_ScanButton.Text = "停止扫描"
                ScanDelayTimer.Enabled = True
                ScanFlag = True
            End If
        End If
    End Sub
#End Region
#Region "定时"
    Dim ClockTime As Integer
    Dim FreshDate As Date
    Private WithEvents FreshTimer As New System.Windows.Forms.Timer
    Sub CheckAutoRecommand()
        If IO.File.Exists(ClockPath) Then
            设置自动解析每日歌曲时间ToolStripMenuItem.Visible = False
            Try
                ClockTime = Math.Max(Math.Min(23, Convert.ToInt32(IO.File.ReadAllText(ClockPath))), 0)
            Catch ex As Exception
                ClockTime = 7
            End Try
            LogText("*已开启自动下载每日歌曲.@" & ClockTime & ":00")
        Else
            设置自动解析每日歌曲时间ToolStripMenuItem.Visible = True
            ClockTime = -1
            LogText("*未开启自动下载每日歌曲.")
        End If
    End Sub
    Private Sub FreshTimer_Tick(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles FreshTimer.Tick
        If ClockTime > -1 AndAlso Now.Minute = 0 AndAlso Now.Hour = ClockTime AndAlso Now.Date > FreshDate.Date Then
            FreshDate = Now.Date
            If RecommandFlag = False Then
                ToolStripSplitButton_Recommand_ButtonClick(Nothing, Nothing)
            End If
        End If
    End Sub
#End Region
#Region "歌单"
    Dim ListId As String
    Dim ListFlag As Boolean = False
    Private Sub ToolStripSplitButton1_ButtonClick(sender As Object, e As EventArgs) Handles ToolStripSplitButton1.ButtonClick
        ListFlag = Not ListFlag
        If ListFlag Then
            ListId = ToolStripTextBox_ListId.Text
            ToolStripTextBox_ListId.Text = ""
            StopFlag_DownloadListTimer = 0
            If ListId.Length > 0 AndAlso IsNumeric(ListId) Then
                更改IDToolStripMenuItem.Enabled = False
                ToolStripMenuItem_ScanButton.Enabled = False
                ToolStripSplitButton_Recommand.Enabled = False
                ToolStripSplitButton1.Enabled = False
                If ScanFlag Then
                    ToolStripMenuItem_Scan_Click(Nothing, Nothing)
                    OnContinueScan = True
                End If
                LogText("开始解析歌单(ID=" & ListId & ").")
                DownloadList()
            Else
                ToolStripTextBox_ListId.Select()
            End If
        Else
            LogText("歌单(ID=" & ListId & ")解析下载中止.")
            StopFlag_DownloadListTimer = -1
            DownloadListTimer.Interval = 100
            DownloadListTimer.Enabled = True
        End If
    End Sub
    Sub DownloadList()
        ListIDIndex = 0
        ListArr.Clear()
        Dim ListUrlStr As String = ListUrl & ListId
        Dim UrlCode As String = GetWebCode(ListUrlStr)
        Dim JsonObj_Code As New With {.code = ""}
        Try
            JsonObj_Code = JsonConvert.DeserializeAnonymousType(UrlCode, JsonObj_Code) '//->Code
        Catch ex As Exception
            JsonObj_Code.code = "-1"
        End Try
        If JsonObj_Code.code = "200" Then
            Dim JsonObjStr As JObject = CType(JsonConvert.DeserializeObject(UrlCode), JObject)
            Dim Jarr As JArray = JsonObjStr("result")("tracks")
            If Jarr.Count > 0 Then
                Dim TempMinfo As New MInfo
                For i = 0 To Jarr.Count - 1
                    TempMinfo.code = "200"
                    TempMinfo.ID = Jarr(i)("id").ToString
                    TempMinfo.Name = Jarr(i)("name").ToString
                    Dim ArtNum As Integer = Jarr(i)("artists").Count
                    If ArtNum > 0 Then
                        For ii = 0 To ArtNum - 1
                            TempMinfo.Artist &= Jarr(i)("artists")(ii)("name").ToString
                            If ii < ArtNum - 1 Then
                                TempMinfo.Artist &= "&"
                            End If
                        Next
                    End If
                    ListArr.Add(TempMinfo)
                Next
                LogText("歌单(ID=" & ListId & ")解析:" & ListArr.Count & "首歌曲.")
                StopFlag_DownloadListTimer = ListArr.Count
                DownloadListTimer.Interval = 1000
                DownloadListTimer.Enabled = True
            Else
                LogText("歌单(ID=" & ListId & ")未解析到歌曲.")
                StopFlag_DownloadListTimer = -1
                DownloadListTimer.Interval = 100
                DownloadListTimer.Enabled = True
            End If
        Else
            LogText("歌单(ID=" & ListId & ")抓取错误." & JsonObj_Code.ToString)
            StopFlag_DownloadListTimer = -1
            DownloadListTimer.Interval = 100
            DownloadListTimer.Enabled = True
        End If
    End Sub
    Private WithEvents DownloadListTimer As New System.Windows.Forms.Timer
    Private Sub DownloadListTimer_Tick(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles DownloadListTimer.Tick
        DownloadListTimer.Enabled = False
        If ListIDIndex < Math.Min(ListArr.Count - 1, StopFlag_DownloadListTimer) Then
            DownloadListTimer.Interval = 1000 + Math.Round(Rnd(), 1) * 2000
            Dim TempMInfo As MInfo = ListArr(ListIDIndex)
            NowDownloadListId = TempMInfo.ID
            ListFileNameStr = ReturnFileNameStr(TempMInfo)
            LogText("正在下载歌单(ID=" & ListId & ")歌曲[" & ListFileNameStr & "](" & ListIDIndex + 1 & "/" & ListArr.Count & ")", False)
            ListWebClient.DownloadFileAsync(New Uri(FileUrl & TempMInfo.ID), TargetPath & ListFileNameStr & ".Mp3")
        Else
            StopFlag_DownloadListTimer = 0
            LogText(" - - 歌单(ID=" & ListId & ")下载结束!总计下载:" & RecommandDownloadSuccessSum & "首.")
            ToolStripMenuItem_ScanButton.Enabled = True
            ToolStripSplitButton_Recommand.Enabled = True
            更改IDToolStripMenuItem.Enabled = True
            ToolStripSplitButton1.Enabled = True
            If OnContinueScan Then
                OnContinueScan = False
                ToolStripMenuItem_ScanButton.Text = "停止扫描"
                ScanDelayTimer.Enabled = True
                ScanFlag = True
            End If
        End If
    End Sub
#End Region
End Class