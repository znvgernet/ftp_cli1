Imports System
Imports System.Net
Imports System.IO
Imports System.Text
Imports System.Net.Sockets


Public Class clsFTP
#Region "Class Variable Declarations"
    Private m_sRemoteHost, m_sRemotePath, m_sRemoteUser As String
    Private m_sRemotePassword, m_sMess As String
    Private m_iRemotePort, m_iBytes As Int32
    Private m_objClientSocket As Socket
    Private m_iRetValue As Int32
    Private m_bLoggedIn As Boolean
    Private m_sMes, m_sReply As String

    '设置用户来对FTP服务器读取和写入数据的数据包的大小
    '对下列具体大小

    Public BLOCK_SIZE = 524288  '512K
    Private m_aBuffer(BLOCK_SIZE) As Byte
    Public info_show_textbox As TextBox
    Public ASCII As Encoding = Encoding.UTF8
    Public flag_bool As Boolean
    '普通变量定义
    Private m_sMessageString As String
#End Region

#Region "Class Constructors"

    'Main类的构造器
    Public Sub New()
        m_sRemoteHost = "localhost"
        m_sRemotePath = "."
        m_sRemoteUser = "anonymous"
        m_sRemotePassword = ""
        m_sMessageString = ""
        m_iRemotePort = 21
        m_bLoggedIn = False
    End Sub

    '参数化的构造器
    Public Sub New(ByVal sRemoteHost As String,
                   ByVal sRemotePath As String,
                   ByVal sRemoteUser As String,
                   ByVal sRemotePassword As String,
                   ByVal iRemotePort As Int32,
                   ByVal txtinfobox As TextBox)

        m_sRemoteHost = sRemoteHost
        m_sRemotePath = sRemotePath
        m_sRemoteUser = sRemoteUser
        m_sRemotePassword = sRemotePassword
        m_sMessageString = ""
        m_iRemotePort = iRemotePort
        m_bLoggedIn = False
        info_show_textbox = txtinfobox
    End Sub

#End Region

#Region "Public Properties"

    '设置或得到你想链接的FTP服务器的名称
    Public Property RemoteHostFTPServer() As String
        '得到FTP服务器的名称
        Get
            Return m_sRemoteHost
        End Get

        '设置FTP服务器的名称

        Set(ByVal Value As String)
            m_sRemoteHost = Value
        End Set
    End Property

    '设置或得到你想链接的FTP服务器的FTP端口
    Public Property RemotePort() As Int32

        '得到FTP端口号

        Get
            Return m_iRemotePort
        End Get

        '设置FTP端口数号

        Set(ByVal Value As Int32)
            m_iRemotePort = Value
        End Set
    End Property

    '设置或得到你想链接的FTP服务器的远程路径
    Public Property RemotePath() As String

        '得到远程路径
        Get
            Return m_sRemotePath
        End Get
        '设置远程路径
        Set(ByVal Value As String)
            m_sRemotePath = Value
        End Set
    End Property

    '设置你想链接的远程FTP服务器的密码
    Public Property RemotePassword() As String
        Get
            Return m_sRemotePassword
        End Get
        Set(ByVal Value As String)
            m_sRemotePassword = Value
        End Set
    End Property

    '设置或得到你想链接远程的FTP服务器的用户
    Public Property RemoteUser() As String
        Get
            Return m_sRemoteUser
        End Get

        Set(ByVal Value As String)
            m_sRemoteUser = Value
        End Set
    End Property

    '设置messagestring类
    Public Property MessageString() As String
        Get
            Return m_sMessageString
        End Get

        Set(ByVal Value As String)
            m_sMessageString = Value
        End Set
    End Property

#End Region
#Region "Public Subs and Functions"

    '从文件系统中返回一个文件列表。在string()函数中返回文件。

    Public Function GetFileList(ByVal sMask As String) As String()
        Dim cSocket As Socket
        Dim bytes As Int32
        Dim seperator As Char = ControlChars.Lf
        Dim mess() As String

        m_sMes = ""
        '检查你是否登录到FTP服务器上
        If (Not (m_bLoggedIn)) Then
            Login()
        End If
        '创建新的socket用于接收文件列表数据
        cSocket = CreateDataSocket()
        '发送FTP命令
        SendCommand("NLST " & sMask)
        If (Not (m_iRetValue = 150 Or m_iRetValue = 125)) Then
            MessageString = m_sReply
            Throw New IOException(m_sRemoteHost & ":" & m_iRemotePort & "：" & m_sReply.Substring(4))
        End If
        m_sMes = ""
        Do While (True)
            m_aBuffer.Clear(m_aBuffer, 0, m_aBuffer.Length)
            bytes = cSocket.Receive(m_aBuffer, m_aBuffer.Length, 0)
            m_sMes += ASCII.GetString(m_aBuffer, 0, bytes)
            info_show_textbox.AppendText(m_sMes & vbCrLf)
            If (bytes < m_aBuffer.Length) Then
                Exit Do
            End If
        Loop
        mess = m_sMes.Split(seperator)
        cSocket.Close()

        ReadReply()
        If (m_iRetValue <> 226) Then
            MessageString = m_sReply
            Throw New IOException(m_sRemoteHost & ":" & m_iRemotePort & "：" & m_sReply.Substring(4))
        End If
        Return mess
    End Function

    '得到FTP服务器上的文件大小
    Public Function GetFileSize(ByVal sFileName As String) As Long
        Dim size As Long

        If (Not (m_bLoggedIn)) Then
            Login()
        End If

        '发送一个FTP命令
        SendCommand("SIZE " & sFileName)
        size = 0

        If (m_iRetValue = 213) Then
            size = Int64.Parse(m_sReply.Substring(4))
        Else
            MessageString = m_sReply
            Throw New IOException(m_sRemoteHost & ":" & m_iRemotePort & "：" & m_sReply.Substring(4))
        End If

        Return size
    End Function

    '登录FTP服务器

    Public Function Login() As Boolean
        m_objClientSocket = New Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp)
        Dim ep As New IPEndPoint(Net.IPAddress.Parse(m_sRemoteHost), m_iRemotePort)

        Try
            m_objClientSocket.Connect(ep)
        Catch ex As Exception
            MessageString = m_sReply  '"Cannot connect to the remote server"
            Throw New IOException("Cannot connect to the remote server" & " (" & m_sRemoteHost & ":" & m_iRemotePort & ")")
        End Try

        ReadReply()
        If (m_iRetValue <> 220) Then
            CloseConnection()
            MessageString = m_sReply
            Throw New IOException(m_sReply & " (" & m_sRemoteHost & ":" & m_iRemotePort & ")")
        End If

        '为了发送一个对服务器的用户登录ID，发送一个FTP命令

        SendCommand("USER " & m_sRemoteUser)

        If (Not (m_iRetValue = 331 Or m_iRetValue = 230)) Then
            Cleanup()
            MessageString = m_sReply
            Throw New IOException(m_sReply & " (" & m_sRemoteHost & ":" & m_iRemotePort & ")")
        End If

        If (m_iRetValue <> 230) Then
            '为了发送一个对服务器的用户密码，发送一个FTP命令
            SendCommand("PASS " & m_sRemotePassword)
            If (Not (m_iRetValue = 230 Or m_iRetValue = 202)) Then
                Cleanup()
                MessageString = m_sReply
                Throw New IOException(m_sReply & " (" & m_sRemoteHost & ":" & m_iRemotePort & ")")
            End If
        End If

        m_bLoggedIn = True
        '为了改变映射的远程服务器的文件夹的目录，调用用户定义的ChangeDirectory函数
        ChangeDirectory(m_sRemotePath)

        '返回最终结果
        Return m_bLoggedIn
    End Function

    '如果模式值为真，对下载设置为二进制模式。否则，设置为ASCII模式

    Public Sub SetBinaryMode(ByVal bMode As Boolean)
        If (bMode) Then
            '发送FTP命令，设置为二进制模式
            '(TYPE是一种用作说明请求类型的FTP命令.)
            SendCommand("TYPE I")
        Else
            '发送FTP命令，设置ASCII模式。
            '(TYPE是一种用作说明请求类型的FTP命令。)
            SendCommand("TYPE A")
        End If
        If (m_iRetValue <> 200) Then
            MessageString = m_sReply
            Throw New IOException(m_sReply & " (" & m_sRemoteHost & ":" & m_iRemotePort & ")")
        End If
    End Sub

    '向配置好的本地目录下载一个文件。保持文件名一样。

    Public Sub DownloadFile(ByVal sFileName As String)
        DownloadFile(sFileName, "", False)
    End Sub

    '向一个配置好的本地文件夹下载一个远程文件。保持文件名一样。
    Public Sub DownloadFile(ByVal sFileName As String, ByVal bResume As Boolean)

        DownloadFile(sFileName, "", bResume)

    End Sub

    '对本地文件名下载一个远程文件。你必须包含一个路径。

    '本地文件名将会创建或者将会被重写，但是路径必须存在。

    Public Sub DownloadFile(ByVal sFileName As String,
     ByVal sLocalFileName As String)
        DownloadFile(sFileName, sLocalFileName, False)
    End Sub

    '对一个本地文件名下载一个远程文件。你必须包含一个路径。设置恢复标志。本地文件名将会被创建或被重写，但是本地路径必须存在。

    Public Sub DownloadFile(ByVal sFileName As String,
     ByVal sLocalFileName As String,
     ByVal bResume As Boolean)

        Dim st As Stream
        Dim output As FileStream
        Dim cSocket As Socket
        Dim offset, npos As Long

        If (Not (m_bLoggedIn)) Then
            Login()
        End If

        SetBinaryMode(True)
        If (sLocalFileName.Equals("")) Then
            sLocalFileName = sFileName
        End If
        If (Not (File.Exists(sLocalFileName))) Then
            st = File.Create(sLocalFileName)
            st.Close()
        End If

        output = New FileStream(sLocalFileName, FileMode.Open)
        cSocket = CreateDataSocket()
        offset = 0

        If (bResume) Then
            offset = output.Length

            If (offset > 0) Then
                '发送一个FTP命令重新启动
                SendCommand("REST " & offset)
                If (m_iRetValue <> 350) Then
                    offset = 0
                End If
            End If

            If (offset > 0) Then
                npos = output.Seek(offset, SeekOrigin.Begin)
            End If
        End If

        '发送一个FTP命令重新找到一个文件。
        SendCommand("RETR " & sFileName)

        If (Not (m_iRetValue = 150 Or m_iRetValue = 125)) Then
            MessageString = m_sReply
            Throw New IOException(m_sRemoteHost & ":" & m_iRemotePort & "：" & m_sReply.Substring(4))
        End If

        Do While (True)
            m_aBuffer.Clear(m_aBuffer, 0, m_aBuffer.Length)
            m_iBytes = cSocket.Receive(m_aBuffer, m_aBuffer.Length, 0)
            output.Write(m_aBuffer, 0, m_iBytes)
            If (m_iBytes <= 0) Then
                Exit Do
            End If
        Loop

        output.Close()
        If (cSocket.Connected) Then
            cSocket.Close()
        End If

        ReadReply()
        If (Not (m_iRetValue = 226 Or m_iRetValue = 250)) Then
            MessageString = m_sReply
            Throw New IOException(m_sRemoteHost & ":" & m_iRemotePort & "：" & m_sReply.Substring(4))
        End If

    End Sub

    '这是一个从你的本地硬盘上向你的FTP文件夹中上载文件的函数

    Public Sub UploadFile(ByVal sFileName As String)
        UploadFile(sFileName, False)
    End Sub

    '这是一个从你的本地硬盘上向你的FTP网页上上载的函数和设置恢复标志

    Public Sub UploadFile(ByVal sFileName As String,
     ByVal bResume As Boolean)
        Dim cSocket As Socket
        Dim offset As Long
        Dim input As FileStream
        Dim bFileNotFound As Boolean

        If (Not (m_bLoggedIn)) Then
            Login()
        End If

        cSocket = CreateDataSocket()
        offset = 0

        If (bResume) Then
            Try
                SetBinaryMode(True)
                offset = GetFileSize(sFileName)
            Catch ex As Exception
                offset = 0
            End Try
        End If

        If (offset > 0) Then
            SendCommand("REST " & offset)
            If (m_iRetValue <> 350) Then
                '远程服务器可能不支持恢复。
                offset = 0
            End If
        End If

        '发送一个FTP命令，存储一个文件。
        SendCommand("STOR " & Path.GetFileName(sFileName))

        If (Not (m_iRetValue = 125 Or m_iRetValue = 150)) Then
            MessageString = m_sReply
            Throw New IOException(m_sReply & " (" & m_sRemoteHost & ":" & m_iRemotePort & ")")
        End If

        '在上载之前，检查文件是否存在。
        bFileNotFound = False
        If (File.Exists(sFileName)) Then
            '打开输入流读取源文件
            input = New FileStream(sFileName, FileMode.Open)
            If (offset <> 0) Then
                input.Seek(offset, SeekOrigin.Begin)
            End If

            '上载这个文件
            'info_show_textbox.AppendText(vbCrLf & "开始时间：" & Now & vbCrLf)
            m_iBytes = input.Read(m_aBuffer, 0, m_aBuffer.Length)
            Do While (m_iBytes > 0)
                cSocket.Send(m_aBuffer, m_iBytes, 0)
                m_iBytes = input.Read(m_aBuffer, 0, m_aBuffer.Length)
            Loop
            'info_show_textbox.AppendText(vbCrLf & "结束时间：" & Now & vbCrLf)
            input.Close()
        Else
            bFileNotFound = True
        End If

        If (cSocket.Connected) Then
            cSocket.Close()
        End If

        '如果找不到文件，检查返回值

        If (bFileNotFound) Then
            MessageString = m_sReply
            Throw New IOException("The file: " & sFileName & " was not found. " &
           "Cannot upload the file to the FTP site")
        End If

        ReadReply()

        If (Not (m_iRetValue = 226 Or m_iRetValue = 250)) Then
            MessageString = m_sReply
            Throw New IOException(m_sReply & " (" & m_sRemoteHost & ":" & m_iRemotePort & ")")
        End If
    End Sub

    '从远程FTP服务器上删除一个文件。

    Public Function DeleteFile(ByVal sFileName As String) As Boolean
        Dim bResult As Boolean

        bResult = True
        If (Not (m_bLoggedIn)) Then
            Login()
        End If
        '发送一个FTP命令，删除一个文件。
        SendCommand("DELE " & sFileName)
        If (m_iRetValue <> 250) Then
            bResult = False
            MessageString = m_sReply
        End If

        '返回最终结果

        Return bResult
    End Function

    '在远程FTP服务器上重命名一个文件

    Public Function RenameFile(ByVal sOldFileName As String,
    ByVal sNewFileName As String) As Boolean

        Dim bResult As Boolean
        bResult = True
        If (Not (m_bLoggedIn)) Then
            Login()
        End If
        '发送一个FTP命令，对一个文件重命名
        SendCommand("RNFR " & sOldFileName)
        If (m_iRetValue <> 350) Then
            MessageString = m_sReply
            Throw New IOException(m_sReply.Substring(4))
        End If

        '发送一个FTP命令，对一个文件更改为新名称
        '如果新的文件名存在，会被覆盖。
        SendCommand("RNTO " & sNewFileName)
        If (m_iRetValue <> 250) Then
            MessageString = m_sReply
            Throw New IOException(m_sReply.Substring(4))
        End If
        '返回最终结果
        Return bResult
    End Function

    '这是一个在远程服务器上创建目录的函数
    Public Function CreateDirectory(ByVal sDirName As String) As Boolean
        Dim bResult As Boolean

        bResult = True
        If (Not (m_bLoggedIn)) Then
            Login()
        End If
        '发送一个FTP命令，在FTP服务器上制作一个目录
        SendCommand("MKD " & sDirName)
        If (m_iRetValue <> 257) Then
            bResult = False
            MessageString = m_sReply
        End If

        '返回最终结果
        Return bResult
    End Function

    '这是一个在远程FTP服务器上删除目录的函数
    Public Function RemoveDirectory(ByVal sDirName As String) As Boolean

        Dim bResult As Boolean
        bResult = True
        '检查是否已登录FTP服务器
        If (Not (m_bLoggedIn)) Then
            Login()
        End If
        '发送一个FTP命令，删除在FTP服务器上的目录
        SendCommand("RMD " & sDirName)
        If (m_iRetValue <> 250) Then
            bResult = False
            MessageString = m_sReply
        End If

        '返回最终结果
        Return bResult
    End Function

    '这是一个用来在远程FTP服务器上改变当前工作目录的函数。
    Public Function ChangeDirectory(ByVal sDirName As String) As Boolean
        Dim bResult As Boolean

        bResult = True
        '检查你是否在根目录
        If (sDirName.Equals(".")) Then
            Return bResult
            Exit Function
        End If
        '检查是否已登录FTP服务器
        If (Not (m_bLoggedIn)) Then
            Login()
        End If
        '发送FTP命令，改变在FTP服务器上的目录。
        SendCommand("CWD " & sDirName)
        If (m_iRetValue <> 250) Then
            bResult = False
            MessageString = m_sReply
        End If

        Me.m_sRemotePath = sDirName

        '返回最终结果
        Return bResult
    End Function

    '关闭远程服务器的FTP链接
    Public Sub CloseConnection()
        If (Not (m_objClientSocket Is Nothing)) Then
            '发送一个FTP命令，结束FTP服务系统。
            SendCommand("QUIT")
        End If

        Cleanup()
    End Sub

#End Region

#Region "Private Subs and Functions"

    '从FTP服务器得到回应。

    Private Sub ReadReply()
        m_sMes = ""
        m_sReply = ReadLine()
        m_iRetValue = Int32.Parse(m_sReply.Substring(0, 3))
        info_show_textbox.AppendText(m_sReply & vbCrLf)
    End Sub

    '清除一些变量

    Private Sub Cleanup()
        If Not (m_objClientSocket Is Nothing) Then
            m_objClientSocket.Close()
            m_objClientSocket = Nothing
        End If

        m_bLoggedIn = False
    End Sub

    '从FTP服务器读取一行。

    Private Function ReadLine(Optional ByVal bClearMes As Boolean = False) As String
        Dim seperator As Char = ControlChars.Lf
        Dim mess() As String

        If (bClearMes) Then
            m_sMes = ""
        End If
        Do While (True)
            m_aBuffer.Clear(m_aBuffer, 0, BLOCK_SIZE)
            m_iBytes = m_objClientSocket.Receive(m_aBuffer, m_aBuffer.Length, 0)
            m_sMes += ASCII.GetString(m_aBuffer, 0, m_iBytes)
            If (m_iBytes < m_aBuffer.Length) Then
                Exit Do
            End If
        Loop

        mess = m_sMes.Split(seperator)
        If (m_sMes.Length > 2) Then
            m_sMes = mess(mess.Length - 2)
        Else
            m_sMes = mess(0)
        End If

        If (Not (m_sMes.Substring(3, 1).Equals(" "))) Then
            Return ReadLine(True)
        End If

        Return m_sMes
    End Function

    '这是一个你想链接的FTP服务器用于发送命令的函数。

    Private Sub SendCommand(ByVal sCommand As String)
        sCommand = sCommand & ControlChars.CrLf
        Dim cmdbytes As Byte() = ASCII.GetBytes(sCommand)
        m_objClientSocket.Send(cmdbytes, cmdbytes.Length, 0)
        ReadReply()
    End Sub

    '创建一个数据包
    Private Function CreateDataSocket() As Socket

        Dim index1, index2, len As Int32
        Dim partCount, i, port As Int32
        Dim ipData, buf, ipAddress As String
        Dim parts(6) As Int32
        Dim ch As Char
        Dim s As Socket
        Dim ep As IPEndPoint
        '发送一个FTP命令，用于被动数据链接

        SendCommand("PASV")
        If (m_iRetValue <> 227) Then
            MessageString = m_sReply
            Throw New IOException(m_sReply.Substring(4))
        End If

        index1 = m_sReply.IndexOf("(")
        index2 = m_sReply.IndexOf(")")
        ipData = m_sReply.Substring(index1 + 1, index2 - index1 - 1)

        len = ipData.Length
        partCount = 0
        buf = ""

        For i = 0 To ((len - 1) And partCount <= 6)
            ch = Char.Parse(ipData.Substring(i, 1))
            If (Char.IsDigit(ch)) Then
                buf += ch
            ElseIf (ch <> ",") Then
                MessageString = m_sReply
                Throw New IOException("Malformed PASV reply: " & m_sReply)
            End If

            If ((ch = ",") Or (i + 1 = len)) Then
                Try
                    parts(partCount) = Int32.Parse(buf)
                    partCount += 1
                    buf = ""
                Catch ex As Exception
                    MessageString = m_sReply
                    Throw New IOException("Malformed PASV reply: " & m_sReply)
                End Try

            End If

        Next

        ipAddress = parts(0) & "." & parts(1) & "." & parts(2) & "." & parts(3)

        '在Visual Basic .Net 2002中进行调用。你想移动8位。在Visual Basic .NET 2002中，你必须将此数乘2的8次方。
        '端口=parts(4)*(2^8)
        '进行这个调用，并且用Visual Basic .NET 2003解释当前行。


        port = parts(4) << 8
        '确定数据端口数
        port = port + parts(5)
        s = New Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp)
        ep = New IPEndPoint(Net.IPAddress.Parse(ipAddress), port)  'Dns.Resolve(ipAddress).AddressList(0)
        Try
            s.Connect(ep)
        Catch ex As Exception
            MessageString = m_sReply
            Throw New IOException("Cannot connect to remote server.")
            '如果你不能链接到特定的FTP服务器，也就是说，将其布尔值设置为false。
            flag_bool = False
        End Try

        '如果你能够链接到特定的FTP服务器，将布尔值设置为true。

        flag_bool = True
        Return s
    End Function

#End Region
End Class
