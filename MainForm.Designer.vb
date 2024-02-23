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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(MainForm))
        Me.StatusStrip_St = New System.Windows.Forms.StatusStrip()
        Me.ToolStripStatusLabel_St = New System.Windows.Forms.ToolStripStatusLabel()
        Me.MenuStrip_Menu = New System.Windows.Forms.MenuStrip()
        Me.打开目录ToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.打开下载目录ToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ToolStripTextBox_Changeid = New System.Windows.Forms.ToolStripTextBox()
        Me.更改IDToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.MenuStrip_Do = New System.Windows.Forms.MenuStrip()
        Me.ToolStripMenuItem_Scan = New System.Windows.Forms.ToolStripMenuItem()
        Me.TextBox_Log = New System.Windows.Forms.TextBox()
        Me.GroupBox_Log = New System.Windows.Forms.GroupBox()
        Me.StatusStrip1 = New System.Windows.Forms.StatusStrip()
        Me.ToolStripProgressBar_Update = New System.Windows.Forms.ToolStripProgressBar()
        Me.ToolStripStatusLabel_UpdatePer = New System.Windows.Forms.ToolStripStatusLabel()
        Me.StatusStrip_St.SuspendLayout()
        Me.MenuStrip_Menu.SuspendLayout()
        Me.MenuStrip_Do.SuspendLayout()
        Me.GroupBox_Log.SuspendLayout()
        Me.StatusStrip1.SuspendLayout()
        Me.SuspendLayout()
        '
        'StatusStrip_St
        '
        Me.StatusStrip_St.ImageScalingSize = New System.Drawing.Size(24, 24)
        Me.StatusStrip_St.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.ToolStripStatusLabel_St})
        Me.StatusStrip_St.Location = New System.Drawing.Point(0, 519)
        Me.StatusStrip_St.Name = "StatusStrip_St"
        Me.StatusStrip_St.Size = New System.Drawing.Size(728, 30)
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
        Me.MenuStrip_Menu.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.打开目录ToolStripMenuItem, Me.打开下载目录ToolStripMenuItem, Me.ToolStripTextBox_Changeid, Me.更改IDToolStripMenuItem})
        Me.MenuStrip_Menu.Location = New System.Drawing.Point(0, 0)
        Me.MenuStrip_Menu.Name = "MenuStrip_Menu"
        Me.MenuStrip_Menu.Size = New System.Drawing.Size(728, 34)
        Me.MenuStrip_Menu.TabIndex = 4
        Me.MenuStrip_Menu.Text = "MenuStrip1"
        '
        '打开目录ToolStripMenuItem
        '
        Me.打开目录ToolStripMenuItem.Name = "打开目录ToolStripMenuItem"
        Me.打开目录ToolStripMenuItem.Size = New System.Drawing.Size(98, 30)
        Me.打开目录ToolStripMenuItem.Text = "打开目录"
        '
        '打开下载目录ToolStripMenuItem
        '
        Me.打开下载目录ToolStripMenuItem.Name = "打开下载目录ToolStripMenuItem"
        Me.打开下载目录ToolStripMenuItem.Size = New System.Drawing.Size(134, 30)
        Me.打开下载目录ToolStripMenuItem.Text = "打开下载目录"
        '
        'ToolStripTextBox_Changeid
        '
        Me.ToolStripTextBox_Changeid.Font = New System.Drawing.Font("Microsoft JhengHei UI", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.ToolStripTextBox_Changeid.Name = "ToolStripTextBox_Changeid"
        Me.ToolStripTextBox_Changeid.Size = New System.Drawing.Size(300, 30)
        '
        '更改IDToolStripMenuItem
        '
        Me.更改IDToolStripMenuItem.Name = "更改IDToolStripMenuItem"
        Me.更改IDToolStripMenuItem.Size = New System.Drawing.Size(81, 30)
        Me.更改IDToolStripMenuItem.Text = "更改ID"
        '
        'MenuStrip_Do
        '
        Me.MenuStrip_Do.BackColor = System.Drawing.SystemColors.Control
        Me.MenuStrip_Do.GripMargin = New System.Windows.Forms.Padding(2, 2, 0, 2)
        Me.MenuStrip_Do.ImageScalingSize = New System.Drawing.Size(24, 24)
        Me.MenuStrip_Do.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.ToolStripMenuItem_Scan})
        Me.MenuStrip_Do.Location = New System.Drawing.Point(0, 34)
        Me.MenuStrip_Do.Name = "MenuStrip_Do"
        Me.MenuStrip_Do.RightToLeft = System.Windows.Forms.RightToLeft.Yes
        Me.MenuStrip_Do.Size = New System.Drawing.Size(728, 31)
        Me.MenuStrip_Do.TabIndex = 6
        Me.MenuStrip_Do.Text = "MenuStrip2"
        '
        'ToolStripMenuItem_Scan
        '
        Me.ToolStripMenuItem_Scan.Name = "ToolStripMenuItem_Scan"
        Me.ToolStripMenuItem_Scan.Size = New System.Drawing.Size(62, 27)
        Me.ToolStripMenuItem_Scan.Text = "扫描"
        '
        'TextBox_Log
        '
        Me.TextBox_Log.BackColor = System.Drawing.SystemColors.Control
        Me.TextBox_Log.Dock = System.Windows.Forms.DockStyle.Fill
        Me.TextBox_Log.Location = New System.Drawing.Point(3, 24)
        Me.TextBox_Log.Multiline = True
        Me.TextBox_Log.Name = "TextBox_Log"
        Me.TextBox_Log.ReadOnly = True
        Me.TextBox_Log.ScrollBars = System.Windows.Forms.ScrollBars.Vertical
        Me.TextBox_Log.Size = New System.Drawing.Size(722, 397)
        Me.TextBox_Log.TabIndex = 7
        '
        'GroupBox_Log
        '
        Me.GroupBox_Log.Controls.Add(Me.TextBox_Log)
        Me.GroupBox_Log.Controls.Add(Me.StatusStrip1)
        Me.GroupBox_Log.Dock = System.Windows.Forms.DockStyle.Fill
        Me.GroupBox_Log.Location = New System.Drawing.Point(0, 65)
        Me.GroupBox_Log.Name = "GroupBox_Log"
        Me.GroupBox_Log.Size = New System.Drawing.Size(728, 454)
        Me.GroupBox_Log.TabIndex = 8
        Me.GroupBox_Log.TabStop = False
        '
        'StatusStrip1
        '
        Me.StatusStrip1.ImageScalingSize = New System.Drawing.Size(24, 24)
        Me.StatusStrip1.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.ToolStripProgressBar_Update, Me.ToolStripStatusLabel_UpdatePer})
        Me.StatusStrip1.Location = New System.Drawing.Point(3, 421)
        Me.StatusStrip1.Name = "StatusStrip1"
        Me.StatusStrip1.Size = New System.Drawing.Size(722, 30)
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
        'MainForm
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(9.0!, 18.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(728, 549)
        Me.Controls.Add(Me.GroupBox_Log)
        Me.Controls.Add(Me.MenuStrip_Do)
        Me.Controls.Add(Me.StatusStrip_St)
        Me.Controls.Add(Me.MenuStrip_Menu)
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.MainMenuStrip = Me.MenuStrip_Menu
        Me.MaximizeBox = False
        Me.MinimumSize = New System.Drawing.Size(750, 600)
        Me.Name = "MainForm"
        Me.StatusStrip_St.ResumeLayout(False)
        Me.StatusStrip_St.PerformLayout()
        Me.MenuStrip_Menu.ResumeLayout(False)
        Me.MenuStrip_Menu.PerformLayout()
        Me.MenuStrip_Do.ResumeLayout(False)
        Me.MenuStrip_Do.PerformLayout()
        Me.GroupBox_Log.ResumeLayout(False)
        Me.GroupBox_Log.PerformLayout()
        Me.StatusStrip1.ResumeLayout(False)
        Me.StatusStrip1.PerformLayout()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents StatusStrip_St As StatusStrip
    Friend WithEvents ToolStripStatusLabel_St As ToolStripStatusLabel
    Friend WithEvents MenuStrip_Menu As MenuStrip
    Friend WithEvents 打开目录ToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents 打开下载目录ToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents MenuStrip_Do As MenuStrip
    Friend WithEvents ToolStripMenuItem_Scan As ToolStripMenuItem
    Friend WithEvents TextBox_Log As TextBox
    Friend WithEvents GroupBox_Log As GroupBox
    Friend WithEvents ToolStripTextBox_Changeid As ToolStripTextBox
    Friend WithEvents 更改IDToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents StatusStrip1 As StatusStrip
    Friend WithEvents ToolStripProgressBar_Update As ToolStripProgressBar
    Friend WithEvents ToolStripStatusLabel_UpdatePer As ToolStripStatusLabel
End Class
