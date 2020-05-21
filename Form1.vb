Imports System
Imports System.Net
Imports System.IO
Imports System.Text
Imports System.Net.Sockets
Imports System.Threading
Imports System.Threading.Thread

Public Class Form1
    Public Thread_Total_count As Integer = 5
    Public StopFlag As Boolean = False
    Public telnet_tc As TcpClient
    Public input_str As String = ""

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        '10.72.76.59  "BeiJing", "BeiJing@zxm10", 6521
        Dim ftpClient As New clsFTP(TextBox4.Text, TextBox2.Text, TextBox6.Text, TextBox7.Text, TextBox5.Text, TextBox3)
        'Dim ftpClient As New clsFTP("10.72.76.156", TextBox2.Text, "root", "lwdhcp", 21, TextBox3)
        'Dim ftpClient As New clsFTP("10.72.76.59", "/河北_栾城天网项目_资料备份", "HeBei", "HeBei@zxm10", 6521, TextBox3)
        Dim str_file_path As String = "D:\bk_e\old_d\软件备份\Sybase 15.0.3_Win64_Server_ESD4.zip" ' "C:\hello"               '要上传的文件名及路径
        Dim str_filename As String = Path.GetFileName(str_file_path) '要上传的文件名
        Dim MyFileSize As Long = FileLen(str_file_path)   ' Returns file length (bytes). 要上传得文件大小
        Try

            If (ftpClient.Login() = True) Then
                'ftpClient.ChangeDirectory("/北京_xxx项目_资料备份/01业务程序安装包备份_202005")

                ftpClient.GetFileList("")
                '创建一个新文件夹
                'ftpClient.CreateDirectory("FTPFOLDERNEW")

                '将新的文件夹设置为活动文件夹。
                'ftpClient.ChangeDirectory("FTPFOLDERNEW")

                '设置FTP模式
                'ftpClient.SetBinaryMode(True)

                '从你的硬盘上上载一个文件到FTP网页
                'ftpClient.UploadFile(str_file_path)

                '获得刚刚上传的文件的大小
                'Dim lng_filesize As Long
                'lng_filesize = ftpClient.GetFileSize(str_filename)

                'If lng_filesize = MyFileSize Then
                'MsgBox("成功上传文件" & str_filename & " 文件大小" & lng_filesize)
                'Else
                'MsgBox("上传失败")
                'End If

                '对上载文件重命名
                'ftpClient.RenameFile("SampleFile.xml", "SampleFile_new.xml")

                '删除一个文件
                'ftpClient.DeleteFile("/北京_xxx项目_资料备份/hello")

                '总是关闭链接，确保没有任何不在使用中的FTP链接
                '检查你是否登录到FTP服务器，并且接着关闭链接
                ftpClient.CloseConnection()
            End If
            'MsgBox("执行完毕！")

        Catch ex As Exception
            TextBox3.AppendText(ex.Message & vbCrLf)
        End Try
    End Sub

    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        '允许线程中访问界面控件
        Form.CheckForIllegalCrossThreadCalls = False
        '设置dock
        Panel1.Height = Me.ToolStrip1.Height + 5
        TabControl1.Dock = DockStyle.Fill

        SplitContainer3.Dock = DockStyle.Fill
        SplitContainer1.Dock = DockStyle.Fill
        SplitContainer2.Dock = DockStyle.Fill

        Panel1.Dock = DockStyle.Top
        'Panel3.Dock = DockStyle.Fill
        Panel2.Dock = DockStyle.Fill

        DataGridView1.Dock = DockStyle.Fill
        DataGridView2.Dock = DockStyle.Fill

        TextBox1.Dock = DockStyle.Fill
        TextBox8.Dock = DockStyle.Fill
        DataGridView5.Dock = DockStyle.Fill
        'DataGridView3.ColumnHeadersVisible = False

        Panel4.Dock = DockStyle.Top
        TextBox3.Dock = DockStyle.Fill

        '设置大小与位置
        Setcontrolsize()

        If Check_file_isexist(parameter_xml_file) Then
            get_parameter_from_xml(parameter_xml_file)
        Else
            Fsu_Ftp_Login_User = "root"
            Fsu_Ftp_login_Pass = "lwdhcp"
            Fsu_Ftp_Conn_Port = 21

            Fsu_Telnet_Conn_Port = 23
            Fsu_Telnet_Conn_User = "root"
            Fsu_Telnet_Conn_Pass = "netviewu"

            Create_xml_fsu_parameter(parameter_xml_file)
        End If

    End Sub

    Private Sub Form1_Resize(sender As Object, e As EventArgs) Handles Me.Resize
        Setcontrolsize()
    End Sub

    Private Sub Setcontrolsize()
        On Error Resume Next

        SplitContainer1.SplitterDistance = SplitContainer1.Width / 2 - 3 + 10
        SplitContainer2.SplitterDistance = SplitContainer2.Width / 2 - 3 + 10
        ToolStrip2.Left = SplitContainer1.SplitterDistance

        DataGridView1.Columns(0).Width = (SplitContainer1.SplitterDistance - 220) / 3 + 20
        DataGridView1.Columns(2).Width = (SplitContainer1.SplitterDistance - 220) / 3 - 20
        DataGridView1.Columns(3).Width = (SplitContainer1.SplitterDistance - 220) / 3

        DataGridView2.Columns(0).Width = (SplitContainer1.SplitterDistance - 70) / 2
        DataGridView2.Columns(1).Width = (SplitContainer1.SplitterDistance - 70) / 2

        DataGridView5.Columns(0).Width = DataGridView5.Width - DataGridView5.RowHeadersWidth - 20


    End Sub

    Private Sub ToolStripButton1_Click(sender As Object, e As EventArgs) Handles ToolStripButton1.Click
        Form2.ShowDialog(Me)
    End Sub

    Private Sub ToolStripButton2_Click(sender As Object, e As EventArgs) Handles ToolStripButton2.Click
        Clear_fsu_ip_list()
    End Sub

    Private Sub ToolStripButton3_Click(sender As Object, e As EventArgs) Handles ToolStripButton3.Click
        If MsgBox("是否确定删除主机列表中FSU的历史告警库文件？", vbOKCancel + vbQuestion, "确定") = vbOK Then
            Dim m_thread As New Thread(AddressOf Del_his_alarmdb_from_fsu)
            m_thread.Start()
            'Del_his_alarmdb_from_fsu()
        End If
    End Sub

    Private Sub ToolStripButton4_Click(sender As Object, e As EventArgs) Handles ToolStripButton4.Click
        Choose_update_file()
    End Sub

    Private Sub ToolStripButton5_Click(sender As Object, e As EventArgs) Handles ToolStripButton5.Click
        Clear_update_file_list()
    End Sub

    Private Sub ToolStripButton6_Click(sender As Object, e As EventArgs) Handles ToolStripButton6.Click
        If MsgBox("是否确定将列表中的所有文件更新到FSU列表中？", vbOKCancel + vbQuestion, "确定") = vbOK Then
            Dim m_thread As New Thread(AddressOf Update_file_to_fsu)
            m_thread.Start()
            'Update_file_to_fsu()
        End If
    End Sub

    Private Sub ToolStripButton7_Click(sender As Object, e As EventArgs) Handles ToolStripButton7.Click
        If MsgBox("是否确定将列表中的所有FSU都重新启动？", vbOKCancel + vbQuestion, "确定") = vbOK Then
            Dim m_thread As New Thread(AddressOf Reboot_fsu_1)
            m_thread.Start()
        End If
    End Sub

    Public Sub Clear_fsu_ip_list()
        If MsgBox("是否确定要清除已加载的FSU主机地址信息，是则确定，否则取消操作", MsgBoxStyle.Question + MsgBoxStyle.YesNo, "执行确认") = MsgBoxResult.No Then
            Exit Sub
        End If
        DataGridView1.Rows.Clear()
    End Sub

    Public Sub Clear_update_file_list()
        If MsgBox("是否确定要清除已添加的待更新文件列表，是则确定，否则取消操作", MsgBoxStyle.Question + MsgBoxStyle.YesNo, "执行确认") = MsgBoxResult.No Then
            Exit Sub
        End If
        DataGridView2.Rows.Clear()
    End Sub

    Public Sub Choose_update_file()
        OpenFileDialog1.FileName = ""
        If ToolStripComboBox1.Text = "" Then
            MsgBox("选择FSU更新文件前请先输入FSU待更新文件路径", vbOKOnly + vbExclamation, "参数不足")
            ToolStripComboBox1.Focus()
            Exit Sub
        Else
            Dim k As String = ToolStripComboBox1.Text
            If Microsoft.VisualBasic.Right(k, 1) <> "/" Then
                k = k & "/"
            End If
            If Microsoft.VisualBasic.Left(k, 1) <> "/" Then
                k = "/" & k
            End If
            ToolStripComboBox1.Text = k
            MsgBox("文件将更新到FSU的 [ " & k & " ] 路径下面，请知悉！", vbOKOnly + vbInformation, "提示")
        End If
        If ToolStripComboBox1.Items.Count > 0 Then
            Dim combtxt As String
            For p As Integer = 0 To ToolStripComboBox1.Items.Count - 1
                combtxt = ToolStripComboBox1.Items(p).ToString
                If combtxt = ToolStripComboBox1.Text.Trim Then
                    If p = 0 Then
                        OpenFileDialog1.Filter = "设备字典表文件|*.xml|所有文件|*.*"
                        GoTo 1
                    End If
                    If p = 1 Then
                        OpenFileDialog1.Filter = "IG2000公共模块文件|ext_app_ig2000|所有文件|*.*"
                        GoTo 1
                    End If
                End If
            Next
            OpenFileDialog1.Filter = "所有文件|*.*"
1:
        End If
        Me.OpenFileDialog1.ShowDialog()
        If OpenFileDialog1.FileName <> "" Then
            Dim i As Long
            Dim k As Integer
            For i = 0 To UBound(OpenFileDialog1.FileNames)
                If OpenFileDialog1.FileNames(i) <> "" Then
                    k = DataGridView2.Rows.Add()
                    DataGridView2.Rows(k).Cells(0).Value = Getfilename(OpenFileDialog1.FileNames(i))
                    DataGridView2.Rows(k).Cells(0).Tag = OpenFileDialog1.FileNames(i)
                    DataGridView2.Rows(k).Cells(1).Value = ToolStripComboBox1.Text
                    'DataGridView2.Rows(k).Cells(2).Value = ""
                End If
            Next
        End If
    End Sub

    Private Function Getfilename(ByVal fpath As String) As String
        Dim m = fpath.Split("\")
        Getfilename = m(UBound(m))
    End Function

    Public Sub Set_control_status(ByVal flg As Boolean)
        ToolStripButton1.Enabled = flg
        ToolStripButton2.Enabled = flg
        ToolStripButton3.Enabled = flg
        ToolStripButton4.Enabled = flg
        ToolStripButton5.Enabled = flg
        ToolStripButton6.Enabled = flg
        ToolStripButton7.Enabled = flg

        配置选项ToolStripMenuItem.Enabled = flg

        Select Case flg
            Case True
                DataGridView1.Columns(0).SortMode = DataGridViewColumnSortMode.Automatic

                DataGridView2.Columns(0).SortMode = DataGridViewColumnSortMode.Automatic
                DataGridView2.Columns(1).SortMode = DataGridViewColumnSortMode.Automatic
            Case False
                DataGridView1.Columns(0).SortMode = DataGridViewColumnSortMode.NotSortable

                DataGridView2.Columns(0).SortMode = DataGridViewColumnSortMode.NotSortable
                DataGridView2.Columns(1).SortMode = DataGridViewColumnSortMode.NotSortable
        End Select
    End Sub

    Public Sub Del_his_alarmdb_from_fsu()
        If DataGridView1.Rows.Count <= 0 Or DataGridView2.Rows.Count <= 0 Then
            MsgBox("FSU主机地址列表和待更新文件列表均不能为空", vbExclamation + vbOKOnly, "条件不足")
            Exit Sub
        End If
        Set_control_status(False)
        DataGridView5.Rows.Clear()
        TextBox1.Text = ""
        TextBox8.Text = ""
        For i As Integer = 0 To DataGridView1.Rows.Count - 1
            DataGridView1.Rows(i).Cells(4).Value = ""
        Next
        For i As Integer = 0 To DataGridView1.Rows.Count - 1
            Del_hisalarmdb_from_fsu(DataGridView1.Rows(i).Cells(0).Value, DataGridView1.Rows(i).Cells(1).Value, DataGridView1.Rows(i).Cells(2).Value, DataGridView1.Rows(i).Cells(3).Tag)
        Next
        Set_control_status(True)
    End Sub

    Public Sub Del_hisalarmdb_from_fsu(ByVal hostip As String, ByVal hostport As Integer, ByVal login_user As String, ByVal login_pass As String)
        Dim ftpClient As New clsFTP(hostip, "", login_user, login_pass, hostport, TextBox1)
        Try
            Set_status_info_for_fsu_list(hostip, hostport, login_user, login_pass, "执行中", 0)
            If (ftpClient.Login() = True) Then
                If ftpClient.DeleteFile("/v2storenand/data/it_store/it_database/hisalarm.db") Then
                    Set_status_info_for_fsu_list(hostip, hostport, login_user, login_pass, "历史告警库删除成功", 20)
                Else
                    Set_status_info_for_fsu_list(hostip, hostport, login_user, login_pass, "历史告警库删除 失败，失败原因：" & ftpClient.MessageString, 21)
                End If
                '总是关闭链接，确保没有任何不在使用中的FTP链接
                '检查你是否登录到FTP服务器，并且接着关闭链接
                ftpClient.CloseConnection()

            Else
                Set_status_info_for_fsu_list(hostip, hostport, login_user, login_pass, "登录 失败", 0)
            End If
            Set_status_info_for_fsu_list(hostip, hostport, login_user, login_pass, "删除完毕", 0)
        Catch ex As Exception
            TextBox1.AppendText(ex.Message)
            Set_status_info_for_fsu_list(hostip, hostport, login_user, login_pass, "发生异常，异常原因 【" & ex.Message & "】 ", 1)
            Set_status_info_for_fsu_list(hostip, hostport, login_user, login_pass, "发生异常，异常原因 【" & ex.Message & "】 ", 21)
        End Try
    End Sub

    Public Sub Update_file_to_fsu()
        If DataGridView1.Rows.Count <= 0 Or DataGridView2.Rows.Count <= 0 Then
            MsgBox("FSU主机地址列表和待更新文件列表均不能为空", vbExclamation + vbOKOnly, "条件不足")
            Exit Sub
        End If
        Set_control_status(False)
        DataGridView5.Rows.Clear()
        TextBox8.Text = ""
        TextBox1.Text = ""
        For i As Integer = 0 To DataGridView1.Rows.Count - 1
            DataGridView1.Rows(i).Cells(4).Value = ""
        Next
        For i As Integer = 0 To DataGridView1.Rows.Count - 1
            Upload_file_to_FSU_new(DataGridView1.Rows(i).Cells(0).Value, DataGridView1.Rows(i).Cells(1).Value, DataGridView1.Rows(i).Cells(2).Value, DataGridView1.Rows(i).Cells(3).Tag)
        Next
        Set_control_status(True)
    End Sub

    Public Sub Upload_file_to_FSU_new(ByVal hostip As String, ByVal hostport As Integer, ByVal longin_user As String, ByVal login_pass As String)
        Dim ftpClient As New clsFTP(hostip, "", longin_user, login_pass, hostport, TextBox1)
        Dim str_file_path As String             '要上传的文件名及路径
        Dim str_filename As String  '要上传的文件名
        Dim MyFileSize As Long  ' Returns file length (bytes). 要上传得文件大小
        Dim lng_filesize As Long
        Try
            Set_status_info_for_fsu_list(hostip, hostport, longin_user, login_pass, "执行中", 0)
            If (ftpClient.Login() = True) Then
                For i As Integer = 0 To DataGridView2.Rows.Count - 1
                    If ftpClient.ChangeDirectory(DataGridView2.Rows(i).Cells(1).Value) Then
                        str_file_path = DataGridView2.Rows(i).Cells(0).Tag
                        str_filename = Path.GetFileName(str_file_path)
                        MyFileSize = FileLen(str_file_path)
                        ftpClient.UploadFile(str_file_path)
                        lng_filesize = ftpClient.GetFileSize(str_filename)
                        If lng_filesize = MyFileSize Then
                            Set_status_info_for_fsu_list(hostip, hostport, longin_user, login_pass, "文件 【" & str_filename & "】 上传成功", 20)
                        Else
                            Set_status_info_for_fsu_list(hostip, hostport, longin_user, login_pass, "文件 【" & str_filename & "】 上传 失败", 21)
                        End If
                    Else
                        Set_status_info_for_fsu_list(hostip, hostport, longin_user, login_pass, ftpClient.MessageString, 21)
                    End If
                Next
                '总是关闭链接，确保没有任何不在使用中的FTP链接
                '检查你是否登录到FTP服务器，并且接着关闭链接
                ftpClient.CloseConnection()
                Set_status_info_for_fsu_list(hostip, hostport, longin_user, login_pass, "更新完毕", 0)
            Else
                Set_status_info_for_fsu_list(hostip, hostport, longin_user, login_pass, "登录 失败", 0)
            End If
        Catch ex As Exception
            TextBox1.AppendText(ex.Message)
            Set_status_info_for_fsu_list(hostip, hostport, longin_user, login_pass, "发生异常，异常原因 【" & ex.Message & "】 ", 1)
            Set_status_info_for_fsu_list(hostip, hostport, longin_user, login_pass, "发生异常，异常原因 【" & ex.Message & "】 ", 21)
        End Try
    End Sub

    Public Sub Set_status_info_for_fsu_list(ByVal hostip As String, ByVal hostport As Integer, ByVal log_user As String, ByVal log_pass As String, ByVal status_info As String, ByVal flg As Integer)
        If flg < 10 Then
            If DataGridView1.Rows.Count > 0 Then
                For i As Integer = 0 To DataGridView1.Rows.Count - 1
                    If DataGridView1.Rows(i).Cells(0).Value = hostip And DataGridView1.Rows(i).Cells(1).Value = hostport And DataGridView1.Rows(i).Cells(2).Value = log_user And DataGridView1.Rows(i).Cells(3).Tag = log_pass Then
                        DataGridView1.Rows(i).Cells(4).Value = status_info '& vbCrLf & DataGridView1.Rows(i).Cells(4).Value
                        'DataGridView1.Refresh()
                        Exit Sub
                    End If
                Next
            End If
        End If
        If flg >= 20 And flg < 30 Then
            'Dim k As Integer
            'k = DataGridView5.Rows.Add()
            'DataGridView5.Rows(k).Cells(0).Value = hostip & " , " & hostport & " , " & log_user & " , " & status_info
            'If flg = 21 Then
            '    DataGridView5.Rows(k).Cells(0).Style.BackColor = Color.Orange
            '    DataGridView5.Rows(k).Cells(0).Style.ForeColor = Color.FromArgb(12, 33, 60)
            'End If
            'DataGridView3.CurrentCell = DataGridView3.Rows(k).Cells(0)
            TextBox8.AppendText(hostip & " , " & hostport & " , " & log_user & " , " & status_info & vbCrLf)
        End If
    End Sub

    Private Sub 退出XToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles 退出XToolStripMenuItem.Click
        Me.Close()
    End Sub

    Private Sub Form1_FormClosing(sender As Object, e As FormClosingEventArgs) Handles Me.FormClosing
        Dim proc As Process = New Process()
        proc = Process.GetCurrentProcess
        proc.Kill()
    End Sub

    Private Sub SplitContainer2_SplitterMoved(sender As Object, e As SplitterEventArgs) Handles SplitContainer2.SplitterMoved
        On Error Resume Next
        DataGridView5.Columns(0).Width = DataGridView5.Width - DataGridView5.RowHeadersWidth - 20
    End Sub

    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        Dim k As Integer
        For i As Integer = 0 To 99
            k = DataGridView5.Rows.Add()
            DataGridView5.Rows(k).Cells(0).Value = Now
        Next
    End Sub

    Public Sub Reboot_fsu_1()
        If DataGridView1.Rows.Count <= 0 Then
            MsgBox("FSU主机地址列表不能为空", vbExclamation + vbOKOnly, "条件不足")
            Exit Sub
        End If
        Set_control_status(False)
        DataGridView5.Rows.Clear()
        TextBox8.Text = ""
        TextBox1.Text = ""
        For i As Integer = 0 To DataGridView1.Rows.Count - 1
            DataGridView1.Rows(i).Cells(4).Value = ""
        Next
        For i As Integer = 0 To DataGridView1.Rows.Count - 1
            Reboot_Fsu(DataGridView1.Rows(i).Cells(0).Value, DataGridView1.Rows(i).Cells(1).Tag, DataGridView1.Rows(i).Cells(2).Tag, DataGridView1.Rows(i).Cells(0).Tag)
        Next
        Set_control_status(True)
    End Sub

    Public Sub Reboot_Fsu(ByVal hostip As String, ByVal hostport As Integer, ByVal login_user As String, ByVal login_pass As String)
        Dim tc As New TcpClient
        Try
            tc.ReceiveBufferSize = 4095
            tc.Connect(IPAddress.Parse(hostip), hostport)
            If tc.Connected Then
                Dim t As New Thread(AddressOf DataListener)
                t.Start(tc)
                Sleep(200)
                tc.Client.Send(System.Text.Encoding.GetEncoding("gb2312").GetBytes(login_user & vbCrLf))
                Sleep(200)
                tc.Client.Send(System.Text.Encoding.GetEncoding("gb2312").GetBytes(login_pass & vbCrLf))
                Sleep(200)
                tc.Client.Send(System.Text.Encoding.GetEncoding("gb2312").GetBytes("reboot" & vbCrLf))
                Sleep(500)
                Set_status_info_for_fsu_list(hostip, hostport, login_user, login_pass, "重启成功", 20)
            End If
            tc.Client.Close()
            tc.Close()
        Catch ex As Exception
            If tc.Connected Then
                tc.Close()
            End If
            Set_status_info_for_fsu_list(hostip, hostport, login_user, login_pass, ex.Message, 20)
        End Try
    End Sub

    Private Sub DataListener(Client As TcpClient)
        Dim Buffer(4095) As Byte
        Dim RecLength As Integer = 0
        Dim str As String = ""
        Client.ReceiveBufferSize = 4095
        Try
            Do Until Not Client.Connected
                RecLength = Client.Client.Receive(Buffer)
                Select Case RecLength
                    Case 0
                        Exit Do
                    Case Else
                        str = Encoding.UTF8.GetString(Buffer, 0, RecLength)
                        TextBox1.AppendText(str)
                End Select
            Loop
            TextBox1.AppendText(vbCrLf & "Good Bye " & Now & vbCrLf)
        Catch ex As Exception

        End Try
    End Sub

    Private Sub DataListener_test_telnet(Client As TcpClient)
        Dim Buffer(4095) As Byte
        Dim RecLength As Integer = 0
        Dim str As String = ""
        Client.ReceiveBufferSize = 4095
        Try
            Do Until Not Client.Connected
                RecLength = Client.Client.Receive(Buffer)
                Select Case RecLength
                    Case 0

                        Exit Do
                    Case Else
                        str = Encoding.UTF8.GetString(Buffer, 0, RecLength)
                        TextBox10.AppendText(str)
                End Select
            Loop
            'Client.Client.Close()
            'Client.Close()
            'Client.Dispose()
            Client = New TcpClient
            TextBox10.AppendText(vbCrLf & "Good Bye " & Now & vbCrLf)
        Catch ex As Exception
            'Client.Dispose()
        End Try
    End Sub

    Private Sub TextBox9_TextChanged(sender As Object, e As EventArgs) Handles TextBox9.TextChanged
        If TextBox9.Text = "" Then
            TextBox9.Text = "> "
            TextBox9.SelectionStart = TextBox9.Text.Length
        ElseIf TextBox9.Text.Length < 2 Then
            TextBox9.Text = "> "
            TextBox9.SelectionStart = TextBox9.Text.Length
        End If
    End Sub

    Private Sub TextBox9_KeyUp(sender As Object, e As KeyEventArgs) Handles TextBox9.KeyUp
        TextBox9.SelectionStart = TextBox9.Text.Length
        If e.KeyCode = 13 Then
            Test_telnet(TextBox9.Text)
            TextBox9.Text = "> "
            TextBox9.SelectionStart = TextBox9.Text.Length
        Else
            TextBox9.SelectionStart = TextBox9.Text.Length
        End If

    End Sub

    Public Sub Test_telnet(ByVal commstr As String)
        Try
            If InStr(commstr, "conn") > 0 Then
                telnet_tc = New TcpClient
                Dim kl = commstr.Substring(1).Substring(6).Split(",")
                If UBound(kl) = 1 Then
                    telnet_tc.Connect(IPAddress.Parse(kl(0)), Int(kl(1)))
                Else
                    telnet_tc.Connect(IPAddress.Parse(kl(0)), Fsu_Telnet_Conn_Port)
                End If
                'telnet_tc.Connect(IPAddress.Parse(commstr.Substring(1).Substring(6)), Fsu_Telnet_Conn_Port)
                If telnet_tc.Connected Then
                    Dim t As New Thread(AddressOf DataListener_test_telnet)
                    t.Start(telnet_tc)
                End If
            Else
                If telnet_tc.Connected Then
                    telnet_tc.Client.Send(System.Text.Encoding.GetEncoding("gb2312").GetBytes(commstr.Substring(2) & vbCrLf))
                End If
            End If
        Catch ex As Exception
            TextBox10.AppendText(ex.Message)
        End Try

    End Sub

    Private Sub TabControl1_SelectedIndexChanged(sender As Object, e As EventArgs) Handles TabControl1.SelectedIndexChanged
        If TabControl1.SelectedIndex = 2 Then
            TextBox9.SelectionStart = TextBox9.Text.Length
            TextBox9.Focus()
        End If
    End Sub

    Private Sub TextBox10_DoubleClick(sender As Object, e As EventArgs) Handles TextBox10.DoubleClick
        TextBox9.SelectionStart = TextBox9.Text.Length
        TextBox9.Focus()
    End Sub

    Private Sub TextBox9_KeyDown(sender As Object, e As KeyEventArgs) Handles TextBox9.KeyDown
        TextBox9.SelectionStart = TextBox9.Text.Length
    End Sub

    Private Sub 配置选项ToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles 配置选项ToolStripMenuItem.Click
        Form3.ShowDialog(Me)
    End Sub
End Class
