<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class MainForm
    Inherits System.Windows.Forms.Form

    'Form 重写 Dispose，以清理组件列表。
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Windows 窗体设计器所必需的
    Private components As System.ComponentModel.IContainer

    '注意: 以下过程是 Windows 窗体设计器所必需的
    '可以使用 Windows 窗体设计器修改它。  
    '不要使用代码编辑器修改它。
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Me.components = New System.ComponentModel.Container()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(MainForm))
        Me.StatusStrip_St = New System.Windows.Forms.StatusStrip()
        Me.ToolStripStatusLabel_St = New System.Windows.Forms.ToolStripStatusLabel()
        Me.MenuStrip_Menu = New System.Windows.Forms.MenuStrip()
        Me.打开目录ToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.打开软件目录ToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.打卡下载目录ToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.设置开机启动ToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ToolStripButton1 = New System.Windows.Forms.ToolStripButton()
        Me.设置文件配置ToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.选择下载文件夹ToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.退出XToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.TextBox_Log = New System.Windows.Forms.TextBox()
        Me.GroupBox_Log = New System.Windows.Forms.GroupBox()
        Me.ToolStrip2 = New System.Windows.Forms.ToolStrip()
        Me.ToolStripMenuItem1 = New System.Windows.Forms.ToolStripLabel()
        Me.ToolStripSeparator5 = New System.Windows.Forms.ToolStripSeparator()
        Me.StatusStrip1 = New System.Windows.Forms.StatusStrip()
        Me.ToolStripProgressBar_Update = New System.Windows.Forms.ToolStripProgressBar()
        Me.ToolStripStatusLabel_UpdatePer = New System.Windows.Forms.ToolStripStatusLabel()
        Me.ToolStripStatusLabel1 = New System.Windows.Forms.ToolStripStatusLabel()
        Me.NotifyIcon1 = New System.Windows.Forms.NotifyIcon(Me.components)
        Me.ToolStrip1 = New System.Windows.Forms.ToolStrip()
        Me.ToolStripMenuItem_ScanButton = New System.Windows.Forms.ToolStripMenuItem()
        Me.ToolStripSeparator3 = New System.Windows.Forms.ToolStripSeparator()
        Me.更改IDToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ToolStripTextBox_Changeid = New System.Windows.Forms.ToolStripTextBox()
        Me.ToolStripSeparator2 = New System.Windows.Forms.ToolStripSeparator()
        Me.ToolStripSplitButton_Daily = New System.Windows.Forms.ToolStripSplitButton()
        Me.ToolStripSeparator4 = New System.Windows.Forms.ToolStripSeparator()
        Me.ToolStripSplitButton_ContinueList = New System.Windows.Forms.ToolStripSplitButton()
        Me.ToolStripSeparator1 = New System.Windows.Forms.ToolStripSeparator()
        Me.ToolStripTextBox_ListId = New System.Windows.Forms.ToolStripTextBox()
        Me.ToolStripSplitButton_List = New System.Windows.Forms.ToolStripSplitButton()
        Me.Panel_Do = New System.Windows.Forms.Panel()
        Me.FolderBrowserDialog1 = New System.Windows.Forms.FolderBrowserDialog()
        Me.StatusStrip_St.SuspendLayout()
        Me.MenuStrip_Menu.SuspendLayout()
        Me.GroupBox_Log.SuspendLayout()
        Me.ToolStrip2.SuspendLayout()
        Me.StatusStrip1.SuspendLayout()
        Me.ToolStrip1.SuspendLayout()
        Me.Panel_Do.SuspendLayout()
        Me.SuspendLayout()
        '
        'StatusStrip_St
        '
        Me.StatusStrip_St.ImageScalingSize = New System.Drawing.Size(24, 24)
        Me.StatusStrip_St.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.ToolStripStatusLabel_St})
        Me.StatusStrip_St.Location = New System.Drawing.Point(0, 519)
        Me.StatusStrip_St.Name = "StatusStrip_St"
        Me.StatusStrip_St.Size = New System.Drawing.Size(1166, 30)
        Me.StatusStrip_St.TabIndex = 2
        Me.StatusStrip_St.Text = "StatusStrip1"
        '
        'ToolStripStatusLabel_St
        '
        Me.ToolStripStatusLabel_St.Name = "ToolStripStatusLabel_St"
        Me.ToolStripStatusLabel_St.Size = New System.Drawing.Size(27, 23)
        Me.ToolStripStatusLabel_St.Text = "St"
        '
        'MenuStrip_Menu
        '
        Me.MenuStrip_Menu.BackColor = System.Drawing.SystemColors.Control
        Me.MenuStrip_Menu.GripMargin = New System.Windows.Forms.Padding(2, 2, 0, 2)
        Me.MenuStrip_Menu.ImageScalingSize = New System.Drawing.Size(24, 24)
        Me.MenuStrip_Menu.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.打开目录ToolStripMenuItem, Me.设置开机启动ToolStripMenuItem, Me.ToolStripButton1, Me.设置文件配置ToolStripMenuItem, Me.选择下载文件夹ToolStripMenuItem, Me.退出XToolStripMenuItem})
        Me.MenuStrip_Menu.Location = New System.Drawing.Point(0, 0)
        Me.MenuStrip_Menu.Name = "MenuStrip_Menu"
        Me.MenuStrip_Menu.Size = New System.Drawing.Size(1166, 36)
        Me.MenuStrip_Menu.TabIndex = 4
        Me.MenuStrip_Menu.Text = "MenuStrip1"
        '
        '打开目录ToolStripMenuItem
        '
        Me.打开目录ToolStripMenuItem.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.打开软件目录ToolStripMenuItem, Me.打卡下载目录ToolStripMenuItem})
        Me.打开目录ToolStripMenuItem.Name = "打开目录ToolStripMenuItem"
        Me.打开目录ToolStripMenuItem.Size = New System.Drawing.Size(89, 32)
        Me.打开目录ToolStripMenuItem.Text = "目录(&O)"
        '
        '打开软件目录ToolStripMenuItem
        '
        Me.打开软件目录ToolStripMenuItem.Name = "打开软件目录ToolStripMenuItem"
        Me.打开软件目录ToolStripMenuItem.Size = New System.Drawing.Size(218, 34)
        Me.打开软件目录ToolStripMenuItem.Text = "打开软件目录"
        '
        '打卡下载目录ToolStripMenuItem
        '
        Me.打卡下载目录ToolStripMenuItem.Name = "打卡下载目录ToolStripMenuItem"
        Me.打卡下载目录ToolStripMenuItem.Size = New System.Drawing.Size(218, 34)
        Me.打卡下载目录ToolStripMenuItem.Text = "打开下载目录"
        '
        '设置开机启动ToolStripMenuItem
        '
        Me.设置开机启动ToolStripMenuItem.Name = "设置开机启动ToolStripMenuItem"
        Me.设置开机启动ToolStripMenuItem.Size = New System.Drawing.Size(134, 32)
        Me.设置开机启动ToolStripMenuItem.Text = "设置开机启动"
        '
        'ToolStripButton1
        '
        Me.ToolStripButton1.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right
        Me.ToolStripButton1.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.ToolStripButton1.Name = "ToolStripButton1"
        Me.ToolStripButton1.Size = New System.Drawing.Size(170, 27)
        Me.ToolStripButton1.Text = "🎧[Music.163.com]"
        '
        '设置文件配置ToolStripMenuItem
        '
        Me.设置文件配置ToolStripMenuItem.Name = "设置文件配置ToolStripMenuItem"
        Me.设置文件配置ToolStripMenuItem.Size = New System.Drawing.Size(134, 32)
        Me.设置文件配置ToolStripMenuItem.Text = "设置文件配置"
        '
        '选择下载文件夹ToolStripMenuItem
        '
        Me.选择下载文件夹ToolStripMenuItem.Name = "选择下载文件夹ToolStripMenuItem"
        Me.选择下载文件夹ToolStripMenuItem.Size = New System.Drawing.Size(152, 32)
        Me.选择下载文件夹ToolStripMenuItem.Text = "选择下载文件夹"
        '
        '退出XToolStripMenuItem
        '
        Me.退出XToolStripMenuItem.Name = "退出XToolStripMenuItem"
        Me.退出XToolStripMenuItem.Size = New System.Drawing.Size(85, 32)
        Me.退出XToolStripMenuItem.Text = "退出(&X)"
        '
        'TextBox_Log
        '
        Me.TextBox_Log.BackColor = System.Drawing.SystemColors.Control
        Me.TextBox_Log.Dock = System.Windows.Forms.DockStyle.Fill
        Me.TextBox_Log.Font = New System.Drawing.Font("宋体", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(134, Byte))
        Me.TextBox_Log.Location = New System.Drawing.Point(3, 52)
        Me.TextBox_Log.Multiline = True
        Me.TextBox_Log.Name = "TextBox_Log"
        Me.TextBox_Log.ReadOnly = True
        Me.TextBox_Log.ScrollBars = System.Windows.Forms.ScrollBars.Vertical
        Me.TextBox_Log.Size = New System.Drawing.Size(1160, 366)
        Me.TextBox_Log.TabIndex = 7
        '
        'GroupBox_Log
        '
        Me.GroupBox_Log.Controls.Add(Me.TextBox_Log)
        Me.GroupBox_Log.Controls.Add(Me.ToolStrip2)
        Me.GroupBox_Log.Controls.Add(Me.StatusStrip1)
        Me.GroupBox_Log.Dock = System.Windows.Forms.DockStyle.Fill
        Me.GroupBox_Log.Font = New System.Drawing.Font("宋体", 9.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(134, Byte))
        Me.GroupBox_Log.Location = New System.Drawing.Point(0, 68)
        Me.GroupBox_Log.Name = "GroupBox_Log"
        Me.GroupBox_Log.Size = New System.Drawing.Size(1166, 451)
        Me.GroupBox_Log.TabIndex = 8
        Me.GroupBox_Log.TabStop = False
        '
        'ToolStrip2
        '
        Me.ToolStrip2.BackColor = System.Drawing.SystemColors.Control
        Me.ToolStrip2.ImageScalingSize = New System.Drawing.Size(24, 24)
        Me.ToolStrip2.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.ToolStripMenuItem1, Me.ToolStripSeparator5})
        Me.ToolStrip2.Location = New System.Drawing.Point(3, 24)
        Me.ToolStrip2.Margin = New System.Windows.Forms.Padding(0, 5, 0, 5)
        Me.ToolStrip2.Name = "ToolStrip2"
        Me.ToolStrip2.Size = New System.Drawing.Size(1160, 28)
        Me.ToolStrip2.TabIndex = 11
        Me.ToolStrip2.Text = "ToolStrip2"
        '
        'ToolStripMenuItem1
        '
        Me.ToolStripMenuItem1.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right
        Me.ToolStripMenuItem1.Name = "ToolStripMenuItem1"
        Me.ToolStripMenuItem1.Size = New System.Drawing.Size(122, 23)
        Me.ToolStripMenuItem1.Text = "Downloaded:"
        '
        'ToolStripSeparator5
        '
        Me.ToolStripSeparator5.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right
        Me.ToolStripSeparator5.Name = "ToolStripSeparator5"
        Me.ToolStripSeparator5.Size = New System.Drawing.Size(6, 28)
        '
        'StatusStrip1
        '
        Me.StatusStrip1.ImageScalingSize = New System.Drawing.Size(24, 24)
        Me.StatusStrip1.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.ToolStripProgressBar_Update, Me.ToolStripStatusLabel_UpdatePer, Me.ToolStripStatusLabel1})
        Me.StatusStrip1.Location = New System.Drawing.Point(3, 418)
        Me.StatusStrip1.Name = "StatusStrip1"
        Me.StatusStrip1.Size = New System.Drawing.Size(1160, 30)
        Me.StatusStrip1.TabIndex = 10
        Me.StatusStrip1.Text = "StatusStrip1"
        '
        'ToolStripProgressBar_Update
        '
        Me.ToolStripProgressBar_Update.Name = "ToolStripProgressBar_Update"
        Me.ToolStripProgressBar_Update.Size = New System.Drawing.Size(100, 22)
        '
        'ToolStripStatusLabel_UpdatePer
        '
        Me.ToolStripStatusLabel_UpdatePer.Name = "ToolStripStatusLabel_UpdatePer"
        Me.ToolStripStatusLabel_UpdatePer.Size = New System.Drawing.Size(26, 23)
        Me.ToolStripStatusLabel_UpdatePer.Text = "%"
        '
        'ToolStripStatusLabel1
        '
        Me.ToolStripStatusLabel1.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
        Me.ToolStripStatusLabel1.Name = "ToolStripStatusLabel1"
        Me.ToolStripStatusLabel1.Size = New System.Drawing.Size(0, 23)
        Me.ToolStripStatusLabel1.Text = "ToolStripStatusLabel1"
        '
        'NotifyIcon1
        '
        Me.NotifyIcon1.Icon = CType(resources.GetObject("NotifyIcon1.Icon"), System.Drawing.Icon)
        Me.NotifyIcon1.Text = "Music163_analyzeDownload"
        '
        'ToolStrip1
        '
        Me.ToolStrip1.BackColor = System.Drawing.SystemColors.Control
        Me.ToolStrip1.ImageScalingSize = New System.Drawing.Size(24, 24)
        Me.ToolStrip1.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.ToolStripMenuItem_ScanButton, Me.ToolStripSeparator3, Me.更改IDToolStripMenuItem, Me.ToolStripTextBox_Changeid, Me.ToolStripSeparator2, Me.ToolStripSplitButton_Daily, Me.ToolStripSeparator4, Me.ToolStripSplitButton_ContinueList, Me.ToolStripSeparator1, Me.ToolStripTextBox_ListId, Me.ToolStripSplitButton_List})
        Me.ToolStrip1.Location = New System.Drawing.Point(0, 0)
        Me.ToolStrip1.Margin = New System.Windows.Forms.Padding(0, 5, 0, 5)
        Me.ToolStrip1.Name = "ToolStrip1"
        Me.ToolStrip1.Size = New System.Drawing.Size(1166, 32)
        Me.ToolStrip1.TabIndex = 9
        Me.ToolStrip1.Text = "ToolStrip1"
        '
        'ToolStripMenuItem_ScanButton
        '
        Me.ToolStripMenuItem_ScanButton.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right
        Me.ToolStripMenuItem_ScanButton.Name = "ToolStripMenuItem_ScanButton"
        Me.ToolStripMenuItem_ScanButton.Size = New System.Drawing.Size(62, 32)
        Me.ToolStripMenuItem_ScanButton.Text = "扫描"
        '
        'ToolStripSeparator3
        '
        Me.ToolStripSeparator3.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right
        Me.ToolStripSeparator3.Name = "ToolStripSeparator3"
        Me.ToolStripSeparator3.Size = New System.Drawing.Size(6, 32)
        '
        '更改IDToolStripMenuItem
        '
        Me.更改IDToolStripMenuItem.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right
        Me.更改IDToolStripMenuItem.Name = "更改IDToolStripMenuItem"
        Me.更改IDToolStripMenuItem.Size = New System.Drawing.Size(81, 32)
        Me.更改IDToolStripMenuItem.Text = "更改ID"
        '
        'ToolStripTextBox_Changeid
        '
        Me.ToolStripTextBox_Changeid.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right
        Me.ToolStripTextBox_Changeid.Font = New System.Drawing.Font("Microsoft JhengHei UI", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.ToolStripTextBox_Changeid.Name = "ToolStripTextBox_Changeid"
        Me.ToolStripTextBox_Changeid.Size = New System.Drawing.Size(200, 32)
        '
        'ToolStripSeparator2
        '
        Me.ToolStripSeparator2.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right
        Me.ToolStripSeparator2.Name = "ToolStripSeparator2"
        Me.ToolStripSeparator2.Size = New System.Drawing.Size(6, 32)
        '
        'ToolStripSplitButton_Daily
        '
        Me.ToolStripSplitButton_Daily.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text
        Me.ToolStripSplitButton_Daily.Image = CType(resources.GetObject("ToolStripSplitButton_Daily.Image"), System.Drawing.Image)
        Me.ToolStripSplitButton_Daily.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.ToolStripSplitButton_Daily.Name = "ToolStripSplitButton_Daily"
        Me.ToolStripSplitButton_Daily.Size = New System.Drawing.Size(103, 27)
        Me.ToolStripSplitButton_Daily.Text = "每日歌单"
        '
        'ToolStripSeparator4
        '
        Me.ToolStripSeparator4.Name = "ToolStripSeparator4"
        Me.ToolStripSeparator4.Size = New System.Drawing.Size(6, 32)
        '
        'ToolStripSplitButton_ContinueList
        '
        Me.ToolStripSplitButton_ContinueList.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text
        Me.ToolStripSplitButton_ContinueList.Image = CType(resources.GetObject("ToolStripSplitButton_ContinueList.Image"), System.Drawing.Image)
        Me.ToolStripSplitButton_ContinueList.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.ToolStripSplitButton_ContinueList.Name = "ToolStripSplitButton_ContinueList"
        Me.ToolStripSplitButton_ContinueList.Size = New System.Drawing.Size(103, 27)
        Me.ToolStripSplitButton_ContinueList.Text = "随机歌单"
        '
        'ToolStripSeparator1
        '
        Me.ToolStripSeparator1.Name = "ToolStripSeparator1"
        Me.ToolStripSeparator1.Size = New System.Drawing.Size(6, 32)
        '
        'ToolStripTextBox_ListId
        '
        Me.ToolStripTextBox_ListId.Font = New System.Drawing.Font("Microsoft JhengHei UI", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.ToolStripTextBox_ListId.Name = "ToolStripTextBox_ListId"
        Me.ToolStripTextBox_ListId.Size = New System.Drawing.Size(200, 32)
        '
        'ToolStripSplitButton_List
        '
        Me.ToolStripSplitButton_List.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text
        Me.ToolStripSplitButton_List.Image = CType(resources.GetObject("ToolStripSplitButton_List.Image"), System.Drawing.Image)
        Me.ToolStripSplitButton_List.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.ToolStripSplitButton_List.Name = "ToolStripSplitButton_List"
        Me.ToolStripSplitButton_List.Size = New System.Drawing.Size(103, 27)
        Me.ToolStripSplitButton_List.Text = "下载歌单"
        '
        'Panel_Do
        '
        Me.Panel_Do.AutoSize = True
        Me.Panel_Do.Controls.Add(Me.ToolStrip1)
        Me.Panel_Do.Dock = System.Windows.Forms.DockStyle.Top
        Me.Panel_Do.Location = New System.Drawing.Point(0, 36)
        Me.Panel_Do.MaximumSize = New System.Drawing.Size(0, 40)
        Me.Panel_Do.MinimumSize = New System.Drawing.Size(0, 30)
        Me.Panel_Do.Name = "Panel_Do"
        Me.Panel_Do.Size = New System.Drawing.Size(1166, 32)
        Me.Panel_Do.TabIndex = 10
        '
        'MainForm
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(9.0!, 18.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(1166, 549)
        Me.Controls.Add(Me.GroupBox_Log)
        Me.Controls.Add(Me.Panel_Do)
        Me.Controls.Add(Me.StatusStrip_St)
        Me.Controls.Add(Me.MenuStrip_Menu)
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.MainMenuStrip = Me.MenuStrip_Menu
        Me.MinimumSize = New System.Drawing.Size(750, 600)
        Me.Name = "MainForm"
        Me.StatusStrip_St.ResumeLayout(False)
        Me.StatusStrip_St.PerformLayout()
        Me.MenuStrip_Menu.ResumeLayout(False)
        Me.MenuStrip_Menu.PerformLayout()
        Me.GroupBox_Log.ResumeLayout(False)
        Me.GroupBox_Log.PerformLayout()
        Me.ToolStrip2.ResumeLayout(False)
        Me.ToolStrip2.PerformLayout()
        Me.StatusStrip1.ResumeLayout(False)
        Me.StatusStrip1.PerformLayout()
        Me.ToolStrip1.ResumeLayout(False)
        Me.ToolStrip1.PerformLayout()
        Me.Panel_Do.ResumeLayout(False)
        Me.Panel_Do.PerformLayout()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents StatusStrip_St As StatusStrip
    Friend WithEvents ToolStripStatusLabel_St As ToolStripStatusLabel
    Friend WithEvents MenuStrip_Menu As MenuStrip
    Friend WithEvents 打开目录ToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents TextBox_Log As TextBox
    Friend WithEvents GroupBox_Log As GroupBox
    Friend WithEvents StatusStrip1 As StatusStrip
    Friend WithEvents ToolStripProgressBar_Update As ToolStripProgressBar
    Friend WithEvents ToolStripStatusLabel_UpdatePer As ToolStripStatusLabel
    Friend WithEvents NotifyIcon1 As NotifyIcon
    Friend WithEvents 打开软件目录ToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents 打卡下载目录ToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents 退出XToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents 设置开机启动ToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents ToolStrip1 As ToolStrip
    Friend WithEvents ToolStripTextBox_Changeid As ToolStripTextBox
    Friend WithEvents 更改IDToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents ToolStripSeparator3 As ToolStripSeparator
    Friend WithEvents ToolStripSeparator1 As ToolStripSeparator
    Friend WithEvents ToolStripSeparator2 As ToolStripSeparator
    Friend WithEvents ToolStripMenuItem_ScanButton As ToolStripMenuItem
    Friend WithEvents ToolStripSplitButton_Daily As ToolStripSplitButton
    Friend WithEvents Panel_Do As Panel
    Friend WithEvents ToolStripTextBox_ListId As ToolStripTextBox
    Friend WithEvents ToolStripSplitButton_List As ToolStripSplitButton
    Friend WithEvents FolderBrowserDialog1 As FolderBrowserDialog
    Friend WithEvents ToolStripButton1 As ToolStripButton
    Friend WithEvents ToolStripStatusLabel1 As ToolStripStatusLabel
    Friend WithEvents 设置文件配置ToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents 选择下载文件夹ToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents ToolStripSplitButton_ContinueList As ToolStripSplitButton
    Friend WithEvents ToolStripSeparator4 As ToolStripSeparator
    Friend WithEvents ToolStrip2 As ToolStrip
    Friend WithEvents ToolStripSeparator5 As ToolStripSeparator
    Friend WithEvents ToolStripMenuItem1 As ToolStripLabel
End Class
