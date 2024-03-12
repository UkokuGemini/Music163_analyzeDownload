Imports System.IO
Imports System.Text
Imports System.Threading
Imports System.Xml
Imports Newtonsoft.Json
Imports Newtonsoft.Json.Linq

Public Class MainForm
    Dim ScanID As Integer = 0
    Dim AutoDailyClock As Integer = -1
    Dim AutoListClock As Integer = -1
    ReadOnly WebUrl As String = "https://music.163.com/#/song?id="
    ReadOnly FileUrl As String = "http://music.163.com/song/media/outer/url?id="
    ReadOnly ListUrl As String = "https://music.163.com/api/playlist/detail?id="
    ReadOnly ListOriUrl As String = "https://music.163.com/#/playlist?id="
    ReadOnly TargetPath As String = Directory.GetCurrentDirectory & "\DownloadDirs\"
    ReadOnly LogPath As String = Directory.GetCurrentDirectory & "\ScanLog.Log"
    ReadOnly XmlSettingPath As String = Directory.GetCurrentDirectory & "\Music163_analyzeDownload_Setting.Xml"
    ReadOnly RepeatPath As String = Directory.GetCurrentDirectory & "\RepeatName.ini"
    Dim DownLoadPath As String
    Private Sub MainForm_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Me.CenterToScreen()
        ShowSetting()
        Me.Text = My.Application.Info.ProductName.ToString & "[Ver." & My.Application.Info.Version.ToString & "]"
        AddHandler ScanWebClient.DownloadProgressChanged, AddressOf ShowDownProgress
        AddHandler ScanWebClient.DownloadFileCompleted, AddressOf ScanDownloadCompleted
        AddHandler RecommandWebClient.DownloadProgressChanged, AddressOf ShowDownProgress
        AddHandler RecommandWebClient.DownloadFileCompleted, AddressOf RecommandDownloadCompleted
        AddHandler ListWebClient.DownloadProgressChanged, AddressOf ShowDownProgress
        AddHandler ListWebClient.DownloadFileCompleted, AddressOf ListDownloadCompleted
        AddHandler AlbumWebClient.DownloadProgressChanged, AddressOf ShowDownProgress
        AddHandler AlbumWebClient.DownloadFileCompleted, AddressOf AlbumDownloadCompleted
        ScanDelayTimer.Interval = 1000
        FreshTimer.Interval = 60000
        GroupBox_Log.Text = "当前扫描ID:" & ScanID
        ToolStripStatusLabel_St.Text = ""
        AutoAlbum = Not AutoAlbum
        ToolStripLabel_Album_Click(Nothing, Nothing)
    End Sub
    Dim ReadThread As Thread
    Sub ShowSetting()
        LogText(">>")
        LogText(Format(Now, "yyyy-MM-dd HH:mm >>"))
        ReadXmlSetting() '//读出设置数据
        CheckAutoRecommand() 'AutoDailyClock
        DownloadDirCheck() 'DownloadDir
        LogText("*当前下载文件夹位置:" & DownLoadPath)
        'LogText("*当前AppId:" & Api_appId)
        'LogText("*当前accessToken:" & Api_accessToken)
        LogText("*设置单次扫描下载上限:" & ScanMax)
        LogText("*歌曲避免重复下载:" & NoRepeat)
        LogText("*上次扫描到歌曲(ID=" & ScanID & ")")
        FreshDownloaded()
        LogText("")
    End Sub
    Sub FreshDownloaded()
        Try
            ReadThread = New Thread(AddressOf ReadDownloaded)
            If NoRepeat AndAlso IO.File.Exists(RepeatPath) Then
                ReadThread.Start()
            End If
        Catch ex As Exception
            ReadDownloaded()
        End Try
    End Sub
    Sub ReadDownloaded()
        RepeatArr.Clear()
        RepeatArr.AddRange(IO.File.ReadAllLines(RepeatPath))
        ToolStripMenuItem1.Text = "Downloaded:" & RepeatArr.Count
        Try
            ReadThread.Abort()
        Catch ex As Exception
        End Try
    End Sub
    Private Sub ToolStripMenuItem1_Click(sender As Object, e As EventArgs) Handles ToolStripMenuItem1.Click
        ToolStripMenuItem1.Text = "Downloaded:"
        If ReadThread.IsAlive = False Then
            If RepeatArr.Count > 0 Then
                Dim AllRecord As String = ""
                For Each Mstr As String In RepeatArr
                    AllRecord &= Mstr & vbCrLf
                Next
                IO.File.WriteAllText(RepeatPath, AllRecord)
            End If
            FreshDownloaded()
        End If
    End Sub
    Dim TrueClose As Boolean = False
    Private Sub MainForm_FormClosing(sender As Object, e As FormClosingEventArgs) Handles MyBase.FormClosing
        If TrueClose Then
            AutoSave()
            NotifyIcon1.Dispose()
            Try
                ReadThread.Abort()
            Catch ex As Exception
            End Try
        Else
            e.Cancel = True
            NotifyIcon1.Visible = True
            Me.WindowState = FormWindowState.Minimized
        End If
    End Sub
    Sub AutoSave()
        WriteXml("ScanId", ScanID)
        WriteXml("AutoAlbum", AutoAlbum)
        If RepeatArr.Count > 0 Then
            Dim AllRecord As String = ""
            For Each Mstr As String In RepeatArr
                AllRecord &= Mstr & vbCrLf
            Next
            IO.File.WriteAllText(RepeatPath, AllRecord)
        End If
        System.IO.File.AppendAllText(LogPath, vbCrLf & TextBox_Log.Text & vbCrLf & "  --  " & Format(Now, "yyyy-MM-dd HH:mm"))
    End Sub
    Sub Toinitial()
        ToolStripMenuItem_ScanButton.Enabled = True
        ToolStripSplitButton_Daily.Enabled = True
        更改IDToolStripMenuItem.Enabled = True
        ToolStripSplitButton_List.Enabled = True
        ToolStripSplitButton_ContinueList.Enabled = True
        If OnContinueScan Then
            OnContinueScan = False
            ToolStripMenuItem_ScanButton.Text = "停止扫描"
            ScanDelayTimer.Enabled = True
            ScanFlag = True
        End If
    End Sub
#Region "Ui"
    Private Sub ToolStripButton1_Click(sender As Object, e As EventArgs) Handles ToolStripButton1.Click
        Diagnostics.Process.Start("https://music.163.com/")
    End Sub
    Private Sub 打开软件目录ToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles 打开软件目录ToolStripMenuItem.Click
        Diagnostics.Process.Start(Directory.GetCurrentDirectory & "\")
    End Sub
    Private Sub 打卡下载目录ToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles 打卡下载目录ToolStripMenuItem.Click
        Diagnostics.Process.Start(DownLoadPath)
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
    Private Sub 设置文件配置ToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles 设置文件配置ToolStripMenuItem.Click
        TrueClose = True
        Diagnostics.Process.Start(XmlSettingPath)
        Me.Close()
    End Sub
    Private Sub 选择下载文件夹ToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles 选择下载文件夹ToolStripMenuItem.Click
        Dim FloderPath As String = DownLoadPath
        Dim FolderRes As DialogResult = FolderBrowserDialog1.ShowDialog()
        If FolderRes <> DialogResult.Cancel Then
            FloderPath = FolderBrowserDialog1.SelectedPath & "\"
        End If
        If System.IO.Directory.Exists(FloderPath) = False Then
            Try
                System.IO.Directory.CreateDirectory(FloderPath)
            Catch ex As Exception
                FloderPath = TargetPath
            End Try
        End If
        DownLoadPath = FloderPath
        WriteXml("DownloadDir", DownLoadPath)
    End Sub
    Sub DownloadDirCheck()
        If Strings.Mid(DownLoadPath, DownLoadPath.Length, 1) <> "\" Then
            DownLoadPath &= "\"
        End If
        If System.IO.Directory.Exists(DownLoadPath) = False Then
            Try
                System.IO.Directory.CreateDirectory(DownLoadPath)
            Catch ex As Exception
            End Try
        End If
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
        DownloadDirCheck()
        ScanDelayTimer.Enabled = False
        LogText(vbCrLf & "【歌曲扫描】ID跳转.From:ID=" & ScanID & ">> ID=" & ToolStripTextBox_Changeid.Text & vbCrLf)
        Try
            ScanID = Convert.ToInt32(ToolStripTextBox_Changeid.Text)
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
            DownloadDirCheck()
            ScanIndex = 0
            ToolStripTextBox_Changeid.Text = ""
            ToolStripMenuItem_ScanButton.Text = "停止扫描"
            LogText(vbCrLf & "【歌曲扫描】开始.@" & Format(Now, "yyyy-MM-dd HH:mm"))
            ScanDelayTimer.Enabled = True
        Else
            ToolStripMenuItem_ScanButton.Text = "扫描"
            LogText(" -- 【歌曲扫描】中止.@" & Format(Now, "yyyy-MM-dd HH:mm"))
        End If
    End Sub
    Dim StopFlag_GetDailyIDTimer, StopFlag_DownloadDailyTimer, StopFlag_DownloadListTimer, StopFlag_ContinueList As Integer
    Private Sub ToolStripSplitButton_Recommand_ButtonClick(sender As Object, e As EventArgs) Handles ToolStripSplitButton_Daily.ButtonClick
        GoDaily()
    End Sub
    Sub GoDaily()
        DailyFlag = Not DailyFlag
        If DailyFlag Then
            DownloadDirCheck()
            ToolStripSplitButton_Daily.Text = "停止解析[每日歌单]."
            If ScanFlag Then
                ToolStripMenuItem_Scan_Click(Nothing, Nothing)
                OnContinueScan = True
            End If
            ToolStripSplitButton_List.Enabled = False
            ToolStripMenuItem_ScanButton.Enabled = False
            更改IDToolStripMenuItem.Enabled = False
            ToolStripSplitButton_ContinueList.Enabled = False
            RecommandStep = 1
            StopFlag_GetDailyIDTimer = 0
            StopFlag_DownloadDailyTimer = 0
            GetRecommandDaily()
        Else
            ToolStripSplitButton_Daily.Text = "每日歌单."
            Dim StopMsgResult As Integer = MsgBox("是否下载已解析的歌曲?", MsgBoxStyle.YesNo, "已解析:" & GetDaily_ID_Index & "首歌曲.")
            ToolStripSplitButton_Daily.Enabled = False
            Select Case RecommandStep
                Case 2
                    StopFlag_GetDailyIDTimer = -1
                    GetDailyIDTimer.Interval = 100
                    GetDailyIDTimer.Enabled = True
                    If StopMsgResult <> MsgBoxResult.Yes Then
                        StopFlag_DownloadDailyTimer = -1 '//不能填StopFlag_DownloadDailyTimer = -1,否则不下载
                        DownloadDailyTimer.Interval = 100
                    End If
                Case 3
                    If StopMsgResult <> MsgBoxResult.Yes Then
                        StopFlag_DownloadDailyTimer = -1
                        DownloadDailyTimer.Interval = 100
                        DownloadDailyTimer.Enabled = True
                    End If
            End Select
        End If
    End Sub
    Private Sub TextBox_Log_TextChanged(sender As Object, e As EventArgs)
        TextBox_Log.SelectionStart = TextBox_Log.TextLength
        TextBox_Log.ScrollToCaret()
    End Sub
    Private Sub MainForm_Resize(sender As Object, e As EventArgs) Handles MyBase.Resize
        ToolStripProgressBar_Update.Size = New Size(Me.Size.Width - 250, 20)
    End Sub
    Sub LogText(ByVal LogStr As String, Optional LogFlag As Boolean = True)
        ToolStripStatusLabel_St.Text = Replace(LogStr, vbCrLf, "")
        If LogFlag Then
            TextBox_Log.Text &= LogStr & vbCrLf
            TextBox_Log.SelectionStart = TextBox_Log.Text.Length
            TextBox_Log.ScrollToCaret()
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
            Me.TopMost = True
            Me.ShowInTaskbar = True
            NotifyIcon1.Visible = False
            TextBox_Log.SelectionStart = TextBox_Log.Text.Length
            TextBox_Log.ScrollToCaret()
            Me.TopMost = False
        End If
    End Sub
#End Region
#Region "重复检查"
    Dim RepeatArr As New ArrayList
    Dim NoRepeat As Boolean = False
    Function RepeatCheck(ByVal NameStr As String, Optional NoRecord As Boolean = False) As Boolean
        If NoRepeat Then
            If RepeatArr.Contains(NameStr) Then
                Return True
            Else
                If NoRecord = False Then
                    RepeatArr.Add(NameStr)
                End If
                Return False
            End If
        Else
            Return False
        End If
    End Function
#End Region
#Region "下载文件"
    Friend ScanWebClient As New System.Net.WebClient
    Friend RecommandWebClient As New System.Net.WebClient
    Friend ListWebClient As New System.Net.WebClient
    Friend AlbumWebClient As New System.Net.WebClient
    Public Sub DownloadFiles(ByVal UrlStr As String, ByVal TargetPath_T As String, ByVal ScanDownloadType As Boolean)
        ToolStripProgressBar_Update.Value = 0
        ToolStripStatusLabel_UpdatePer.Text = "0%[ / ]"
        If ScanDownloadType Then
            ScanIndex += 1
            ScanWebClient.DownloadFileAsync(New Uri(UrlStr), TargetPath_T)
        Else
            RecommandWebClient.DownloadFileAsync(New Uri(UrlStr), TargetPath_T)
        End If
    End Sub '读取更新配置url
    Private Sub ShowDownProgress(ByVal sender As Object, ByVal e As System.Net.DownloadProgressChangedEventArgs)
        ToolStripProgressBar_Update.Value = e.ProgressPercentage
        ToolStripStatusLabel_UpdatePer.Text = e.ProgressPercentage & "%[" & FileSize(e.BytesReceived) & "/" & FileSize(e.TotalBytesToReceive) & "]"
    End Sub 'ProgressShow
    Public Sub ScanDownloadCompleted(ByVal sender As Object, ByVal e As System.ComponentModel.AsyncCompletedEventArgs)
        ToolStripProgressBar_Update.Value = 100
        LogText("ID=" & ScanID & ">_歌曲[" & FileNameStr & "] - 下载完成!")
        NextID(True)
    End Sub '下载完成->启动子
    Public Sub RecommandDownloadCompleted(ByVal sender As Object, ByVal e As System.ComponentModel.AsyncCompletedEventArgs)
        ToolStripProgressBar_Update.Value = 100
        DailyDownloadSuccessSum += 1
        LogText("(" & DailyDownloadSuccessSum & "/" & GetSingleDaily_Count & ").歌曲[" & FileNameStr & "](ID=" & NowDownloadRecommandId & ") - 下载完成!")
        SuccessDownloadKey = NowDownloadKey
        NextID(False)
    End Sub '下载完成->启动子
    Public Sub ListDownloadCompleted(ByVal sender As Object, ByVal e As System.ComponentModel.AsyncCompletedEventArgs)
        ToolStripProgressBar_Update.Value = 100
        ListIDIndex += 1
        LogText("歌单(ID=" & ListId & ")>_(" & ListIDIndex & "/" & SingleListArr.Count & ").歌曲[" & ListFileNameStr & "](ID=" & NowDownloadListId & ") - 下载完成!")
        DownloadListTimer.Enabled = True
    End Sub '下载完成->启动子
    Public Sub AlbumDownloadCompleted(ByVal sender As Object, ByVal e As System.ComponentModel.AsyncCompletedEventArgs)
        ToolStripProgressBar_Update.Value = 100
        AlbumINdex += 1
        LogText(NowAlbum_AlbumNameShowText & ">> (" & AlbumINdex & "/" & AlbumArr.Count & ")." & NowAlbum_SongsNameShowText & " - 下载完成!")
        DownloadAlbumTimer.Enabled = True
    End Sub '下载完成->启动子
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
        Dim AlbumName As String
        Dim AlbumID As String
    End Structure
    Dim FileNameStr, ListFileNameStr, AlbumFileNameStr As String
    Dim ScanFlag As Boolean = False
    Dim DailyFlag As Boolean = False
    Dim ScanIndex As Integer = 0
    Dim ScanMax As String
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
                LogText("无ID=" & ID & "的歌曲信息.(空信息)")
                NextID(ScanGetType)
            Else
                If AutoAlbum Then
                    Try
                        ErrReTry = 0
                        GetAlbum(FileName.AlbumID, ID)
                    Catch ex As Exception
                    End Try
                End If
                LogText("正在下载: 歌曲[" & FileNameStr & "](ID=" & ID & ")", False)
                If System.IO.File.Exists(DownLoadPath & FileNameStr & ".Mp3") Then
                    LogText("已存在: 歌曲[" & FileNameStr & "](ID=" & ID & ")")
                    RepeatCheck(FileNameStr)
                    SuccessDownloadKey = NowDownloadKey
                    NextID(ScanGetType)
                Else
                    If ScanGetType = False Then
                        NowDownloadRecommandId = ID
                    End If
                    If RepeatCheck(FileNameStr) = False Then
                        DownloadFiles(FileUrl & ID, DownLoadPath & FileNameStr & ".Mp3", ScanGetType)
                    Else
                        LogText("已下载过: 歌曲[" & FileNameStr & "](ID=" & ID & ")")
                        NextID(ScanGetType)
                    End If
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
            ScanID += 1
            GroupBox_Log.Text = "当前扫描ID:" & ScanID
            If ScanFlag Then
                ScanDelayTimer.Enabled = True
            Else
                GoAlbum()
            End If
        Else
            Daily_ID_Index += 1
            DownloadDailyTimer.Enabled = True
        End If
    End Sub
    Function JsonRead(ByVal Id As String) As MInfo
        Dim jsonUrl As String = "http://music.163.com/api/song/detail/?id=" & Id & "&ids=%5B" & Id & "%5D"
        Dim UrlCode As String = GetWebCode(jsonUrl)
        Dim TempMInfo As New MInfo
        Dim JsonObj_Code As New With {.code = ""}
        Try
            JsonObj_Code = JsonConvert.DeserializeAnonymousType(UrlCode, JsonObj_Code)
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
                TempMInfo.AlbumID = JsonObjStr("album")("id").ToString
                TempMInfo.AlbumName = JsonObjStr("album")("name").ToString
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
        If ScanIndex >= Int(ScanMax) Then
            LogText("【歌曲扫描】.已达到单次扫描下载上限:" & ScanMax)
            ToolStripMenuItem_Scan_Click(Nothing, Nothing)
            GoAlbum()
        Else
            GetPageInfo(ScanID, True)
        End If
    End Sub
#End Region
#Region "每日歌曲"
    ReadOnly Api_Daily As String = "http://openapi.music.163.com/openapi/music/basic/recommend/songlist/get/v2?appId=a301010000000000aadb4e5a28b45a67&bizContent=%7B%22limit%22%3A<*>%7D&signType=RSA_SHA256&accessToken=x46c13d33a898ad1d257c5009a1daadfced5a1160176c2309y&device=%7B%22deviceType%22%3A%22andrwear%22%2C%22os%22%3A%22andrwear%22%2C%22appVer%22%3A%220.1%22%2C%22channel%22%3A%22hm%22%2C%22model%22%3A%22kys%22%2C%22deviceId%22%3A%22321%22%2C%22brand%22%3A%22hm%22%2C%22osVer%22%3A%228.1.0%22%7D&timestamp="
    Dim DailyNameArr As New ArrayList
    Dim DailyIDArr As New ArrayList
    Dim SingleListArr As New ArrayList
    Sub GetRecommandDaily()
        SuccessDownloadKey = ""
        NowDownloadKey = ""
        DailyNameArr.Clear()
        DailyIDArr.Clear()
        Dim Api_Daily_Now As String = Replace(Api_Daily, "<*>", DailyMax) & DateTimeOffset.UtcNow.ToUnixTimeSeconds.ToString & "000"
        Dim UrlCode As String = GetWebCode(Api_Daily_Now)
        Dim JsonObj_Code As New With {.code = ""}
        Try
            JsonObj_Code = JsonConvert.DeserializeAnonymousType(UrlCode, JsonObj_Code)
        Catch ex As Exception
            JsonObj_Code.code = "-1"
        End Try
        If JsonObj_Code.code = "200" Then
            Dim JsonObjStr As JObject = CType(JsonConvert.DeserializeObject(UrlCode), JObject)
            Dim MCount As Integer = CType(JsonObjStr("data"), JArray).Count
            If MCount > 0 Then
                For i = 0 To MCount - 1
                    UrlCode = CType(JsonObjStr("data"), JArray).Item(i).ToString
                    Dim JsonObj_Name As New With {.name = ""}
                    Dim RecommandSearchInfo As SearchInfo
                    RecommandSearchInfo.Key = JsonConvert.DeserializeAnonymousType(UrlCode, JsonObj_Name).name.ToString
                    RecommandSearchInfo.ID = i
                    DailyNameArr.Add(RecommandSearchInfo)
                Next
                LogText(vbCrLf & "【每日歌单】.预计解析" & DailyNameArr.Count & "首.")
                GetDaily_ID_Index = 0
                GetDaily_ID_IndexCount = DailyNameArr.Count
                GetSingleDaily_Count = 0
                Daily_ID_Index = 0
                DailyDownloadSuccessSum = 0
                GetDailyIDTimer.Interval = 1000
                RecommandStep = 2
                StopFlag_GetDailyIDTimer = GetDaily_ID_IndexCount
                GetDailyIDTimer.Enabled = True
            Else
                ToolStripSplitButton_Daily.Text = "每日歌单."
                DailyFlag = False
                RecommandStep = 0
                DailyIDArr.Clear()
                ToolStripSplitButton_Daily.Enabled = True
                ToolStripMenuItem_ScanButton.Enabled = True
                更改IDToolStripMenuItem.Enabled = True
                ToolStripSplitButton_List.Enabled = True
                ToolStripSplitButton_ContinueList.Enabled = True
                If OnContinueScan Then
                    OnContinueScan = False
                    ToolStripMenuItem_ScanButton.Text = "停止扫描"
                    ScanDelayTimer.Enabled = True
                    ScanFlag = True
                End If
                LogText("【每日歌单】:未获取到歌单数据.")
            End If
        Else
            ToolStripSplitButton_Daily.Text = "每日歌单."
            DailyFlag = False
            RecommandStep = 0
            DailyIDArr.Clear()
            ToolStripSplitButton_Daily.Enabled = True
            ToolStripMenuItem_ScanButton.Enabled = True
            更改IDToolStripMenuItem.Enabled = True
            ToolStripSplitButton_List.Enabled = True
            ToolStripSplitButton_ContinueList.Enabled = True
            If OnContinueScan Then
                OnContinueScan = False
                ToolStripMenuItem_ScanButton.Text = "停止扫描"
                ScanDelayTimer.Enabled = True
                ScanFlag = True
            End If
            LogText("【每日歌单】:获取失败." & JsonObj_Code.code)
        End If
    End Sub
    Private WithEvents GetDailyIDTimer As New System.Windows.Forms.Timer
    Private Sub GetDailyIDTimer_Tick(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles GetDailyIDTimer.Tick
        GetDailyIDTimer.Enabled = False
        If GetDaily_ID_Index < Math.Min(GetDaily_ID_IndexCount, StopFlag_GetDailyIDTimer) Then
            If Delay_Flag Then
                LogText("(" & GetDaily_ID_Index + 1 & "/" & GetDaily_ID_IndexCount & ").正在解析:" & "[" & CType(DailyNameArr(GetDaily_ID_Index), SearchInfo).Key & "]" & "延迟等待:" & Int(GetDailyIDTimer.Interval / 1000) & "s.", Not Delay_Flag)
            Else
                LogText("(" & GetDaily_ID_Index + 1 & "/" & GetDaily_ID_IndexCount & ").正在解析:" & "[" & CType(DailyNameArr(GetDaily_ID_Index), SearchInfo).Key & "]")
            End If
            GetDaily_MusicID(CType(DailyNameArr(GetDaily_ID_Index), SearchInfo).Key)
        Else
            Delay_Plus = 0
            LogText(vbCrLf & " -- 解析【每日歌单】:" & GetSingleDaily_Count & "首.(" & DailyIDArr.Count & "条歌曲数据)")
            RecommandStep = 3
            If StopFlag_DownloadDailyTimer = -1 Then
                RecommandStep = 0
                DailyIDArr.Clear()
                LogText(vbCrLf & " -- 【每日歌单】解析已中止!.  --  " & Format(Now, "yyyy-MM-dd HH:mm"))
                ToolStripSplitButton_Daily.Enabled = True
                ToolStripMenuItem_ScanButton.Enabled = True
                更改IDToolStripMenuItem.Enabled = True
                ToolStripSplitButton_List.Enabled = True
                ToolStripSplitButton_ContinueList.Enabled = True
                If OnContinueScan Then
                    OnContinueScan = False
                    ToolStripMenuItem_ScanButton.Text = "停止扫描"
                    ScanDelayTimer.Enabled = True
                    ScanFlag = True
                End If
            Else
                DownloadDaily()
            End If
        End If
    End Sub
    Public Structure SearchInfo
        Dim Key As String
        Dim ID As String
    End Structure
    Dim GetDaily_ID_Index As Integer '//NameArr 当前第几条
    Dim GetDaily_ID_IndexCount As Integer '//NameArr 词条总数(需要下载的歌曲数量)
    Dim GetSingleDaily_Count As Integer '//实际下载歌曲数(与GetDaily_ID_IndexCount差在有Key但没有搜索到)
    Dim Daily_ID_Index As Integer '//获取链接条目数
    Dim DailyDownloadSuccessSum As Integer '//成功下载数
    Dim ListIDIndex As Integer '//
    Dim ListDownloadSuccessSum As Integer '//
    Dim Delay_Plus, Delay_Plus_List, Delay_Plus_ContinueList As Integer
    Dim Delay_Flag As Boolean
    Sub GetDaily_MusicID(ByVal SearchKey As String)
        Dim SearchUrl As String = "https://music.163.com/api/search/get/web?csrf_token=hlpretag=&hlposttag=&s=" & SearchKey & "&type=1&offset=0&total=true&limit=5"
        Dim UrlCode As String = GetWebCode(SearchUrl)
        Dim JsonObj_Code As New With {.code = ""}
        Try
            JsonObj_Code = JsonConvert.DeserializeAnonymousType(UrlCode, JsonObj_Code)
        Catch ex As Exception
            JsonObj_Code.code = "-1"
        End Try
        Delay_Flag = False
        If JsonObj_Code.code = "200" Then
            Dim JsonObj_res As JObject = CType(JsonConvert.DeserializeObject(UrlCode), JObject)
            Dim JsonObj_songs As JObject = CType(JsonConvert.DeserializeObject(JsonObj_res("result").ToString), JObject)
            Dim MCount As Integer = CType(JsonObj_songs("songs"), JArray).Count
            If MCount > 0 Then
                GetSingleDaily_Count += 1
                For i = 0 To MCount - 1
                    Dim JsonObj_ID As JObject = CType(JsonConvert.DeserializeObject(JsonObj_songs("songs")(i).ToString), JObject)
                    Dim TempSearchInfo As SearchInfo
                    TempSearchInfo.ID = JsonObj_ID("id")
                    TempSearchInfo.Key = SearchKey
                    If TempSearchInfo.ID.Length > 0 Then
                        DailyIDArr.Add(TempSearchInfo)
                    End If
                Next
            End If
            GetDaily_ID_Index += 1
            GetDailyIDTimer.Interval = 1000 + Math.Round(Rnd(), 1) * 2000
        ElseIf JsonObj_Code.code = "406" OrElse JsonObj_Code.code = "405" OrElse JsonObj_Code.code = "-447" Then
            Delay_Flag = True
            Delay_Plus += 15000
            If Delay_Plus >= 3600000 Then
                GetDaily_ID_Index += 1
                Delay_Plus = 1800000
            End If
            Delay_Plus = Math.Min(1800000, Delay_Plus)
            GetDailyIDTimer.Interval = 1000 + Math.Round(Rnd(), 1) * 2000 + Delay_Plus
        Else
            GetDaily_ID_Index += 1
            GetDailyIDTimer.Interval = 1000 + Math.Round(Rnd(), 1) * 2000
        End If
        GetDailyIDTimer.Enabled = True
    End Sub
    Sub DownloadDaily()
        LogText(vbCrLf & "开始下载每日歌单:" & GetSingleDaily_Count & "首.(" & DailyIDArr.Count & "条歌曲数据)" & vbCrLf)
        StopFlag_DownloadDailyTimer = DailyIDArr.Count
        DownloadDailyTimer.Interval = 1000
        DownloadDailyTimer.Enabled = True
    End Sub
    Private WithEvents DownloadDailyTimer As New System.Windows.Forms.Timer
    Dim SuccessDownloadKey, NowDownloadKey As String
    Dim NowDownloadRecommandId, NowDownloadListId As String
    Private Sub DownloadDailyTimer_Tick(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles DownloadDailyTimer.Tick
        DownloadDailyTimer.Enabled = False
        DownloadDailyTimer.Interval = 1000 + Math.Round(Rnd(), 1) * 2000
        If Daily_ID_Index < Math.Min(DailyIDArr.Count, StopFlag_DownloadDailyTimer) Then
            Dim TempSearchInfo As SearchInfo = DailyIDArr(Daily_ID_Index)
            If SuccessDownloadKey <> TempSearchInfo.Key OrElse (SuccessDownloadKey = "" AndAlso TempSearchInfo.Key = "") Then
                NowDownloadKey = TempSearchInfo.Key
                GetPageInfo(TempSearchInfo.ID, False)
            Else
                LogText("已跳过类似[ " & TempSearchInfo.Key & " ]歌曲:ID=" & TempSearchInfo.ID, False)
                DownloadDailyTimer.Interval = 100
                NextID(False)
            End If
        Else
            RecommandStep = 0
            DailyIDArr.Clear()
            LogText(vbCrLf & " -- 【每日歌单】下载结束!总计下载:" & DailyDownloadSuccessSum & "首.  --  " & Format(Now, "yyyy-MM-dd HH:mm"))
            ToolStripSplitButton_Daily.Text = "每日歌单."
            DailyFlag = False
            GoAlbum()
        End If
    End Sub
#End Region
#Region "定时"
    Dim FreshDate, FreshListDate As Date
    Private WithEvents FreshTimer As New System.Windows.Forms.Timer
    Sub CheckAutoRecommand()
        Dim EnableFresh As Boolean = False
        If AutoDailyClock > -1 AndAlso AutoDailyClock < 24 Then
            EnableFresh = True
            LogText("*已开启自动下载【每日歌曲】(" & DailyMax & "首).@" & AutoDailyClock & ":00")
        Else
            AutoDailyClock = -1
            LogText("*未开启自动下载【每日歌曲】.")
        End If
        If AutoListClock > -1 AndAlso AutoListClock < 24 Then
            EnableFresh = True
            LogText("*已开启自动下载【随机歌单】(" & ListMax & "个).@" & AutoListClock & ":00")
        Else
            AutoListClock = -1
            LogText("*未开启自动下载【随机歌单】.")
        End If
        If EnableFresh Then
            FreshTimer.Enabled = True
        End If
    End Sub
    Private Sub FreshTimer_Tick(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles FreshTimer.Tick
        If AutoDailyClock > -1 AndAlso Now.Minute = 0 AndAlso Now.Hour = AutoDailyClock AndAlso Now.Date > FreshDate.Date Then
            FreshDate = Now.Date '//防止不断触发
            If DailyFlag = False Then
                ToolStripSplitButton_Recommand_ButtonClick(Nothing, Nothing)
            End If
        End If
        If AutoListClock > -1 AndAlso Now.Minute = 0 AndAlso Now.Hour = AutoListClock AndAlso Now.Date > FreshListDate Then
            FreshListDate = Now.Date '//防止不断触发
            If DailyFlag Then
                AutoListClock += 1
                If AutoListClock > 23 Then
                    AutoListClock = 0
                End If
            Else
                If RecommandListFlag = False Then
                    ToolStripSplitButton2_ButtonClick(Nothing, Nothing)
                End If
            End If
        End If
        If Now.Minute = 0 AndAlso Now.Hour = 0 Then
            AutoSave()
            TextBox_Log.Text = ""
            ShowSetting()
        End If
    End Sub
#End Region
#Region "随机歌单"
    ReadOnly RecommendListUrl As String = "https://openapi.music.163.com/openapi/music/basic/recommend/playlist/get?appId=a301010000000000aadb4e5a28b45a67&bizContent=%7B%22limit%22%3A<*>%7D&signType=RSA_SHA256&accessToken=x46c13d33a898ad1d257c5009a1daadfced5a1160176c2309y&device=%7B%22deviceType%22%3A%22andrwear%22%2C%22os%22%3A%22andrwear%22%2C%22appVer%22%3A%220.1%22%2C%22channel%22%3A%22hm%22%2C%22model%22%3A%22kys%22%2C%22deviceId%22%3A%22321%22%2C%22brand%22%3A%22hm%22%2C%22osVer%22%3A%228.1.0%22%7D&timestamp="
    ReadOnly RecommendListSearchUrl As String = "https://music.163.com/api/search/get/web?csrf_token=hlpretag=&hlposttag=&s={<*>}&type=1000&offset=0&total=true&limit=20"
    Dim RecommandSingleListArr As New ArrayList
    Dim RecommandDownloadIdArr As New ArrayList
    Dim RecommandListFlag As Boolean = False
    Private Sub ToolStripSplitButton2_ButtonClick(sender As Object, e As EventArgs) Handles ToolStripSplitButton_ContinueList.ButtonClick
        RecommandListFlag = Not RecommandListFlag
        If RecommandListFlag Then
            ToolStripSplitButton_ContinueList.Text = "停止解析[随机歌单]"
            ToolStripSplitButton_List.Enabled = False
            ToolStripMenuItem_ScanButton.Enabled = False
            更改IDToolStripMenuItem.Enabled = False
            ToolStripSplitButton_Daily.Enabled = False
            GetRecommandList()
        Else
            ToolStripSplitButton_ContinueList.Text = "随机歌单"
            StopFlag_ContinueList = -1
        End If
    End Sub
    Sub GetRecommandList()
        StopFlag_ContinueList = 0
        Delay_Plus_ContinueList = 0
        DownloadRecommandSongSum = 0
        DownloadRecommandListSum = 0
        RecommandSingleListArr.Clear()
        Delay_Plus_List = 0
        Dim Api_Daily_Now As String = Replace(RecommendListUrl, "<*>", ListMax) & DateTimeOffset.UtcNow.ToUnixTimeSeconds.ToString & "000"
        Dim UrlCode As String = GetWebCode(Api_Daily_Now)
        Dim JsonObj_Code As New With {.code = ""}
        Try
            JsonObj_Code = JsonConvert.DeserializeAnonymousType(UrlCode, JsonObj_Code)
        Catch ex As Exception
            JsonObj_Code.code = "-1"
        End Try
        If JsonObj_Code.code = "200" Then
            Dim JsonObjStr As JObject = CType(JsonConvert.DeserializeObject(UrlCode), JObject)
            Dim Json_Records As New With {.name = ""}
            'Json_Records = JsonConvert.DeserializeAnonymousType(JsonObjStr("data")("records"), Json_Records) 
            Dim MCount As Integer = CType(JsonObjStr("data")("records"), JArray).Count
            If MCount > 0 Then
                For i = 0 To MCount - 1
                    UrlCode = CType(JsonObjStr("data")("records"), JArray).Item(i).ToString
                    Dim JsonObj_Name As New With {.name = ""}
                    RecommandSingleListArr.Add(JsonConvert.DeserializeAnonymousType(UrlCode, JsonObj_Name).name.ToString)
                Next
                LogText(vbCrLf & "【随机歌单】.预计解析" & RecommandSingleListArr.Count & "个.")
                SearchListIDInsex = 0
                RecommandDownloadIdArr.Clear()
                If StopFlag_ContinueList <> -1 Then
                    StopFlag_ContinueList = RecommandSingleListArr.Count
                End If
                GetRecommandListIDTimer.Interval = 1000
                GetRecommandListIDTimer.Enabled = True
            End If
        End If
    End Sub
    Private WithEvents GetRecommandListIDTimer As New System.Windows.Forms.Timer
    Dim SearchListIDInsex As Integer
    Private Sub GetRecommandListIDTimer_Tick(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles GetRecommandListIDTimer.Tick
        GetRecommandListIDTimer.Enabled = False
        If SearchListIDInsex < Math.Min(RecommandSingleListArr.Count, StopFlag_ContinueList) Then
            Dim UrlCode As String = GetWebCode(Replace(RecommendListSearchUrl, "<*>", RecommandSingleListArr(SearchListIDInsex)))
            Dim JsonObj_Code As New With {.code = ""}
            Try
                JsonObj_Code = JsonConvert.DeserializeAnonymousType(UrlCode, JsonObj_Code)
            Catch ex As Exception
                JsonObj_Code.code = "-1"
            End Try
            If JsonObj_Code.code = "200" Then
                Dim JsonObjStr As JObject = CType(JsonConvert.DeserializeObject(UrlCode), JObject)
                Dim MCount As Integer
                Try
                    MCount = CType(JsonObjStr("result")("playlists"), JArray).Count
                Catch ex As Exception
                End Try
                If MCount > 0 Then
                    For i = 0 To MCount - 1
                        UrlCode = CType(JsonObjStr("result")("playlists"), JArray).Item(i).ToString
                        Dim SInfo As SearchInfo
                        Dim JsonObj_ID As New With {.id = ""}
                        SInfo.ID = JsonConvert.DeserializeAnonymousType(UrlCode, JsonObj_ID).id.ToString
                        Dim JsonObj_Name As New With {.Name = ""}
                        SInfo.Key = JsonConvert.DeserializeAnonymousType(UrlCode, JsonObj_Name).Name.ToString
                        RecommandDownloadIdArr.Add(SInfo)
                        LogText("(" & SearchListIDInsex + 1 & "/" & RecommandSingleListArr.Count & ").找到歌单:[ " & SInfo.Key & " ](ID=" & SInfo.ID & ")")
                        Exit For
                    Next
                End If
                GetRecommandListIDTimer.Interval = 1000
            Else
                If JsonObj_Code.code = "406" OrElse JsonObj_Code.code = "405" OrElse JsonObj_Code.code = "-447" Then
                    Delay_Plus_List += 1
                    GetRecommandListIDTimer.Interval = 1000 + Delay_Plus_List * 5000
                    LogText("(" & SearchListIDInsex & "/" & RecommandSingleListArr.Count & ").歌单搜索延迟(" & Int(GetRecommandListIDTimer.Interval / 1000) & "秒)", False)
                End If
                Dim JsonObj_err As New With {.Msg = ""}
                Try
                    JsonObj_err = JsonConvert.DeserializeAnonymousType(UrlCode, JsonObj_err)
                Catch ex As Exception
                End Try
                LogText("(" & SearchListIDInsex + 1 & "/" & RecommandSingleListArr.Count & ").歌单搜索失败.(" & JsonObj_Code.code & ":" & JsonObj_err.Msg & ")")
            End If
            SearchListIDInsex += 1
            GetRecommandListIDTimer.Enabled = True
        Else
            '//解析完毕
            If StopFlag_ContinueList <> -1 Then
                LogText(vbCrLf & "开始自动下载【随机歌单】:" & RecommandDownloadIdArr.Count & "个.")
                StopFlag_ContinueList = RecommandDownloadIdArr.Count
            End If
            DownloadRecommandListIndex = 0
            DownloadRecommandListTimer.Interval = 1000
            DownloadRecommandListTimer.Enabled = True
        End If
    End Sub
    Private WithEvents DownloadRecommandListTimer As New System.Windows.Forms.Timer
    Dim DownloadRecommandListIndex As Integer
    Dim DownloadRecommandSongSum, DownloadRecommandListSum As Integer
    Dim ListContinue As Boolean = False
    Private Sub DownloadRecommandListTimer_Tick(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles DownloadRecommandListTimer.Tick
        DownloadRecommandListTimer.Enabled = False
        If DownloadRecommandListIndex < Math.Min(RecommandDownloadIdArr.Count, StopFlag_ContinueList) Then
            ListContinue = True
            ListFlag = False
            ToolStripTextBox_ListId.Text = CType(RecommandDownloadIdArr(DownloadRecommandListIndex), SearchInfo).ID
            ToolStripSplitButton1_ButtonClick(Nothing, Nothing)
        Else
            If StopFlag_ContinueList = -1 Then
                LogText(vbCrLf & " -- 【随机歌单】中止!共" & DownloadRecommandListSum & "个歌单." & DownloadRecommandSongSum & "首.  --  " & Format(Now, "yyyy-MM-dd HH:mm"))
            Else
                LogText(vbCrLf & " -- 【随机歌单】下载完成!共" & DownloadRecommandListSum & "个歌单." & DownloadRecommandSongSum & "首.  --  " & Format(Now, "yyyy-MM-dd HH:mm"))
            End If
            RecommandListFlag = False
            ToolStripSplitButton_ContinueList.Text = "随机歌单"
            ToolStripTextBox_ListId.Text = ""
            StopFlag_DownloadListTimer = 0
            StopFlag_ContinueList = 0
            GoAlbum()
        End If
    End Sub
#End Region
#Region "单个歌单"
    Dim ListId As String
    Dim ListFlag As Boolean = False
    Dim NextListID As String
    Private Sub ToolStripSplitButton1_ButtonClick(sender As Object, e As EventArgs) Handles ToolStripSplitButton_List.ButtonClick
        ListFlag = Not ListFlag
        If ListFlag Then
            DownloadDirCheck()
            ListId = ToolStripTextBox_ListId.Text
            StopFlag_DownloadListTimer = 0
            If ListId.Length > 0 AndAlso IsNumeric(ListId) Then
                更改IDToolStripMenuItem.Enabled = False
                ToolStripMenuItem_ScanButton.Enabled = False
                ToolStripSplitButton_Daily.Enabled = False
                ToolStripSplitButton_List.Enabled = False
                If RecommandListFlag = False Then
                    ToolStripSplitButton_ContinueList.Enabled = False
                End If
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
    Dim ListContinueSuccesFlag As Boolean = False
    Sub DownloadList()
        ListContinueSuccesFlag = False
        ListIDIndex = 0
        SingleListArr.Clear()
        Dim ListUrlStr As String = ListUrl & ListId
        Dim UrlCode As String = GetWebCode(ListUrlStr)
        Dim JsonObj_Code As New With {.code = ""}
        Try
            JsonObj_Code = JsonConvert.DeserializeAnonymousType(UrlCode, JsonObj_Code)
        Catch ex As Exception
            JsonObj_Code.code = "-1"
        End Try
        If JsonObj_Code.code = "200" Then
            ListContinueSuccesFlag = True
            Dim JsonObjStr As JObject = CType(JsonConvert.DeserializeObject(UrlCode), JObject)
            Dim Jarr As JArray = JsonObjStr("result")("tracks")
            If Jarr.Count > 0 Then
                For i = 0 To Jarr.Count - 1
                    Dim TempMinfo As New MInfo With {
                        .code = "200",
                        .ID = Jarr(i)("id").ToString,
                        .Name = Jarr(i)("name").ToString,
                        .AlbumID = Jarr(i)("album")("id").ToString,
                        .AlbumName = Jarr(i)("album")("name").ToString
                    }
                    Dim ArtNum As Integer = Jarr(i)("artists").Count
                    If ArtNum > 0 Then
                        For ii = 0 To ArtNum - 1
                            TempMinfo.Artist &= Jarr(i)("artists")(ii)("name").ToString
                            If ii < ArtNum - 1 Then
                                TempMinfo.Artist &= "&"
                            End If
                        Next
                    End If
                    SingleListArr.Add(TempMinfo)
                Next
                If ListContinue Then
                    DownloadRecommandListSum += 1
                End If
                LogText("歌单(ID=" & ListId & ")解析:" & SingleListArr.Count & "首歌曲.")
                StopFlag_DownloadListTimer = SingleListArr.Count
                DownloadListTimer.Interval = 1000
                DownloadListTimer.Enabled = True
            Else
                LogText("歌单(ID=" & ListId & ")未解析到歌曲.")
                StopFlag_DownloadListTimer = -1
                DownloadListTimer.Interval = 100
                DownloadListTimer.Enabled = True
            End If
        Else
            Delay_Plus_ContinueList += 1
            Dim JsonObj_err As New With {.Msg = ""}
            Try
                JsonObj_err = JsonConvert.DeserializeAnonymousType(UrlCode, JsonObj_err)
            Catch ex As Exception
            End Try
            LogText("歌单(ID=" & ListId & ")抓取错误.(" & JsonObj_Code.code & ":" & JsonObj_err.Msg & ")")
            StopFlag_DownloadListTimer = -1
            DownloadListTimer.Interval = 100
            DownloadListTimer.Enabled = True
        End If
    End Sub
    Private WithEvents DownloadListTimer As New System.Windows.Forms.Timer
    Private Sub DownloadListTimer_Tick(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles DownloadListTimer.Tick
        DownloadListTimer.Enabled = False
        If ListIDIndex < Math.Min(SingleListArr.Count, StopFlag_DownloadListTimer) Then
            Randomize()
            DownloadListTimer.Interval = 1000 + Math.Round(Rnd(), 1) * 2000
            Dim TempMInfo As MInfo = SingleListArr(ListIDIndex)
            NowDownloadListId = TempMInfo.ID
            ListFileNameStr = ReturnFileNameStr(TempMInfo)
            LogText("正在下载歌单(ID=" & ListId & ")歌曲[" & ListFileNameStr & "](" & ListIDIndex + 1 & "/" & SingleListArr.Count & ")", False)
            If AutoAlbum Then
                Try
                    ErrReTry = 0
                    If GetAlbum(TempMInfo.AlbumID, TempMInfo.ID) Then
                        DownloadListTimer.Interval = 20000 + Math.Round(Rnd(), 1) * 2000
                    End If
                Catch ex As Exception
                End Try
            End If
            If RepeatCheck(ListFileNameStr) = False Then
                ListWebClient.DownloadFileAsync(New Uri(FileUrl & TempMInfo.ID), DownLoadPath & ListFileNameStr & ".Mp3")
                If ListContinue Then
                    DownloadRecommandSongSum += 1
                End If
            Else
                LogText("已下载过歌曲[" & FileNameStr & "](ID=" & NowDownloadListId & ").")
                ListIDIndex += 1
                DownloadListTimer.Enabled = True
            End If
        ElseIf ListContinue Then
            DownloadListTimer.Interval = 1000
            If ListIDIndex > 0 Then
                LogText("歌单(ID=" & ListId & ")[" & CType(RecommandDownloadIdArr(DownloadRecommandListIndex), SearchInfo).Key & "]歌曲(" & ListIDIndex & "首)下载完成!")
            End If
            DownloadRecommandListIndex += 1
            If ListContinueSuccesFlag = False Then
                DownloadRecommandListTimer.Interval = 1000 + 60000 * Delay_Plus_ContinueList
            End If
            DownloadRecommandListTimer.Enabled = True
        Else
            DownloadListTimer.Interval = 1000
            ToolStripTextBox_ListId.Text = ""
            StopFlag_DownloadListTimer = 0
            LogText(" -- 歌单(ID=" & ListId & ")下载结束!总计下载:" & ListIDIndex & "首.")
            GoAlbum()
        End If
    End Sub
#End Region
#Region "XmlSetting"
    ReadOnly InitialXmlSettingStr As String = "<?xml version=" & Chr(34) & "1.0" & Chr(34) & "?>" & vbCrLf &
"<Music163_analyzeDownload_Setting> " & vbCrLf &
"<DownloadDir>" & TargetPath & "</DownloadDir>" & vbCrLf &
"<ScanId>0</ScanId>" & vbCrLf &
"<ScanMax>500</ScanMax>" & vbCrLf &
"<AutoDailyClock>-1</AutoDailyClock>" & vbCrLf &
"<DailyMax>50</DailyMax>" & vbCrLf &
"<AutoListClock>-1</AutoListClock>" & vbCrLf &
"<ListMax>10</ListMax>" & vbCrLf &
"<NoRepeat>0</NoRepeat>" & vbCrLf &
"<AutoAlbum>0</AutoAlbum>" & vbCrLf &
"<AlbumLimit>5</AlbumLimit>" & vbCrLf &
"<CleanSmallKb>150</CleanSmallKb>" & vbCrLf &
"</Music163_analyzeDownload_Setting>"
    Sub ReadXmlSetting()
        If System.IO.File.Exists(XmlSettingPath) = False Then
            System.IO.File.WriteAllText(XmlSettingPath, InitialXmlSettingStr)
        End If
        ScanID = ReadXmlKeyValue("ScanId", 0)
        DownLoadPath = ReadXmlKeyValue("DownloadDir", TargetPath)
        AutoDailyClock = ReadXmlKeyValue("AutoDailyClock", -1)
        DailyMax = Math.Max(1, Convert.ToInt32(ReadXmlKeyValue("DailyMax", 50)))
        AutoListClock = ReadXmlKeyValue("AutoListClock", -1)
        ListMax = Math.Max(1, Convert.ToInt32(ReadXmlKeyValue("ListMax", 10)))
        ScanMax = ReadXmlKeyValue("ScanMax", "1000")
        NoRepeat = ReadXmlKeyValue("NoRepeat", 0)
        AutoAlbum = ReadXmlKeyValue("AutoAlbum", 0)
        AlbumLimitNum = ReadXmlKeyValue("AlbumLimit", 5)
        SmallKb = ReadXmlKeyValue("CleanSmallKb", 150)
        清理无效音频ToolStripMenuItem.Text = "清理小音频<" & SmallKb & "Kb"
    End Sub
    Dim AutoAlbum As Boolean
    Dim DailyMax, ListMax As Integer
    Function ReadXmlKeyValue(ByVal QurStr As String, ByVal DefaultValue As String) As String
        Dim Res As String = DefaultValue
        Try
            Dim xmlDoc As New XmlDocument()
            xmlDoc.Load(XmlSettingPath)
            Res = CType(xmlDoc.SelectSingleNode("Music163_analyzeDownload_Setting").SelectSingleNode(QurStr), XmlElement).InnerText
        Catch ex As Exception
        End Try
        Return Res
    End Function
    Sub WriteXml(ByVal NameStr As String, ByVal Value As String)
        Dim SuccesFlag As Boolean = False
        Dim xmlDoc As New XmlDocument()
        xmlDoc.Load(XmlSettingPath)
        Dim RootNode As XmlElement = xmlDoc.DocumentElement
        Dim isExit As Boolean = False
        If IsNothing(RootNode) = False Then
            Try
                Dim mBound, i As Integer
                '循环体遍历子节点
                mBound = RootNode.ChildNodes.Count - 1
                For i = 0 To mBound
                    If RootNode.ChildNodes(i).Name = NameStr Then
                        RootNode.ChildNodes(i).InnerText = Value
                        SuccesFlag = True
                        isExit = True
                        Exit For
                    End If
                Next
            Catch ex As Exception
            End Try
        End If
        Try
            '如果修改失败，则创建节点
            If isExit = False Then
                Dim xn As XmlNode = xmlDoc.CreateNode(XmlNodeType.Element, NameStr, "")
                RootNode.AppendChild(xn)
                xn.InnerText = Value
            End If
        Catch ex As Exception
            SuccesFlag = False    '如果XML文件遭到破坏，则返回False
        End Try
        If SuccesFlag Then
            xmlDoc.Save(XmlSettingPath)
            LogText("已更改下载文件夹:" & Value)
        Else
            LogText("下载文件夹:" & Value & "修改失败,请在配置文件中手动修改.")
        End If
    End Sub
#End Region
#Region "Album"
    Dim AlbumArr As New ArrayList
    Dim ErrReTry As Integer
    Dim AlbumLimitNum As Integer
    ReadOnly AlbumPath As String = "http://music.163.com/api/album/<*>?ext=true&id={<*>}&offset=0&total=true"
    Function GetAlbum(ByVal AlbumID As Integer, ByVal FromID As Integer) As Boolean
        Dim Res As Boolean = False
        Dim AlbumUrl As String = Replace(AlbumPath, "<*>", AlbumID)
        Dim UrlCode As String = GetWebCode(AlbumUrl)
        Dim TempMInfo As New MInfo
        Dim JsonObj_Code As New With {.code = ""}
        LogText("【关联专辑】.解析专辑(ID=" & AlbumID & ")", False)
        Try
            JsonObj_Code = JsonConvert.DeserializeAnonymousType(UrlCode, JsonObj_Code)
        Catch ex As Exception
            JsonObj_Code.code = "-1"
        End Try
        TempMInfo.code = JsonObj_Code.code
        If JsonObj_Code.code = "200" Then
            ErrReTry = 0
            Dim JsonObjStr As JObject = CType(JsonConvert.DeserializeObject(UrlCode), JObject)
            Dim Jarray_Songs As JArray = JsonObjStr("album")("songs")
            Dim AlbumNameStr, AlbumIDStr As String
            AlbumNameStr = JsonObjStr("album")("name")
            AlbumIDStr = JsonObjStr("album")("id")
            If Jarray_Songs.Count > 0 Then
                Dim LimitIndex As Integer = 0
                For i = 0 To Jarray_Songs.Count - 1
                    TempMInfo.ID = Jarray_Songs.Item(i)("id").ToString
                    TempMInfo.Name = Jarray_Songs.Item(i)("name").ToString
                    TempMInfo.Artist = ""
                    Dim ArtistsNum As Integer = Jarray_Songs.Item(i)("artists").Count
                    If ArtistsNum > 0 Then
                        For ii = 0 To ArtistsNum - 1
                            TempMInfo.Artist &= Jarray_Songs.Item(i)("artists").Item(ii)("name").ToString
                            If ii < ArtistsNum - 1 Then
                                TempMInfo.Artist &= "&"
                            End If
                        Next
                    Else
                        TempMInfo.Artist = "Unknown"
                    End If
                    TempMInfo.AlbumID = AlbumIDStr
                    TempMInfo.AlbumName = AlbumNameStr
                    If RepeatCheck(ReturnFileNameStr(TempMInfo), True) = False AndAlso TempMInfo.ID <> FromID Then
                        AlbumArr.Add(TempMInfo)
                        LimitIndex += 1
                    End If
                    If LimitIndex >= AlbumLimitNum AndAlso AlbumLimitNum <> -1 Then
                        Exit For
                    End If
                Next
                Res = True
                If LimitIndex > 0 Then
                    LogText("【关联专辑】.解析专辑<" & AlbumNameStr & ">(ID=" & AlbumIDStr & ") +" & LimitIndex & "首.")
                End If
            Else
                TempMInfo.code = "-1"
            End If
        ElseIf JsonObj_Code.code = "-462" AndAlso ErrReTry <= 4 Then
            ErrReTry += 1
            Res = GetAlbum(AlbumID, FromID)
        Else
            ErrReTry = 0
            Dim JsonObj_err As New With {.Message = ""}
            Try
                JsonObj_err = JsonConvert.DeserializeAnonymousType(UrlCode, JsonObj_err)
            Catch ex As Exception
            End Try
            LogText("【关联专辑】.解析歌曲失败.(" & JsonObj_Code.code & ":" & JsonObj_err.Message & ")", False)
        End If
        ToolStripLabel_AlbumNum.Text = "【关联专辑】.已解析歌曲:" & AlbumArr.Count
        Return Res
    End Function

    Private Sub 清理无效音频ToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles 清理无效音频ToolStripMenuItem.Click
        Clean()
    End Sub
    Dim AlbumSeccessNum As Integer
    Sub GoAlbum()
        If AlbumArr.Count > 0 AndAlso AutoAlbum Then
            LogText(vbCrLf & "【关联专辑】.开始下载歌曲.预计:" & AlbumArr.Count & "首.")
            AlbumINdex = 0
            AlbumSeccessNum = 0
            DownloadAlbumTimer.Enabled = True
        Else
            If AutoAlbum = False Then
                LogText("*未设置[关联专辑]")
            Else
                'LogText("*未搜寻到专辑内关联歌曲")
            End If
            AlbumArr.Clear()
            Toinitial()
        End If
    End Sub
    Dim AlbumINdex As Integer
    Dim NowAlbum_AlbumNameShowText, LastAlbum_AlbumNameShowText As String
    Dim NowAlbum_SongsNameShowText As String
    Private WithEvents DownloadAlbumTimer As New System.Windows.Forms.Timer
    Private Sub DownloadAlbumTimer_Tick(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles DownloadAlbumTimer.Tick
        DownloadAlbumTimer.Enabled = False
        If AlbumINdex < AlbumArr.Count Then
            Randomize()
            DownloadAlbumTimer.Interval = 1000 + Math.Round(Rnd(), 1) * 2000
            Dim TempMInfo As MInfo = AlbumArr(AlbumINdex)
            AlbumFileNameStr = ReturnFileNameStr(TempMInfo)
            NowAlbum_AlbumNameShowText = "专辑<" & TempMInfo.AlbumName & ">(ID=" & TempMInfo.AlbumID & ")"
            NowAlbum_SongsNameShowText = "歌曲[" & AlbumFileNameStr & "](ID=" & TempMInfo.ID & ")"
            If NowAlbum_AlbumNameShowText <> LastAlbum_AlbumNameShowText Then
                LogText("【关联专辑】下载:" & NowAlbum_AlbumNameShowText & ">>")
                LastAlbum_AlbumNameShowText = NowAlbum_AlbumNameShowText
            End If
            LogText("正在下载:" & NowAlbum_AlbumNameShowText & ">> (" & AlbumINdex + 1 & "/" & AlbumArr.Count & ")." & NowAlbum_SongsNameShowText, False)
            If RepeatCheck(AlbumFileNameStr) = False Then
                AlbumSeccessNum += 1
                AlbumWebClient.DownloadFileAsync(New Uri(FileUrl & TempMInfo.ID), DownLoadPath & AlbumFileNameStr & ".Mp3")
            Else
                LogText("已下载过:" & NowAlbum_SongsNameShowText & "")
                AlbumINdex += 1
                    DownloadAlbumTimer.Enabled = True
                End If
            Else
                AlbumArr.Clear()
            ToolStripLabel_AlbumNum.Text = "【关联专辑】.已解析歌曲:"
            ToolStripTextBox_ListId.Text = ""
            LogText(vbCrLf & " -- 【关联专辑】下载结束!总计下载:" & AlbumSeccessNum & "首.")
            Toinitial()
        End If
    End Sub
    Private Sub ToolStripLabel_Album_Click(sender As Object, e As EventArgs) Handles ToolStripLabel_Album.Click
        AutoAlbum = Not AutoAlbum
        If AutoAlbum Then
            ToolStripLabel_Album.Text = "尝试下载专辑✔️"
        Else
            ToolStripLabel_Album.Text = "尝试下载专辑❌"
        End If
        ToolStripLabel_AlbumNum.Visible = AutoAlbum
    End Sub
#End Region
#Region "Clean"
    Dim SmallKb As Int64
    Sub Clean()
        Dim DelectSmallSum As Integer = 0
        On Error Resume Next
        Dim DirectoryInfos As New DirectoryInfo(DownLoadPath)
        Dim GetFiles As FileInfo() = DirectoryInfos.GetFiles("*")
        For Each FileInfos As FileInfo In GetFiles
            If FileInfos.Length < SmallKb * 8192 Then
                IO.File.Delete(FileInfos.FullName)
                DelectSmallSum += 1
            End If
        Next
        LogText(vbCrLf & "【清理小音频】(<" & SmallKb & "kb)共:" & DelectSmallSum & vbCrLf)
    End Sub
#End Region
End Class