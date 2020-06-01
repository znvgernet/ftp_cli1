Imports System.Xml


Module public_parameter
    Public Fsu_Ftp_Login_User As String = "root"
    Public Fsu_Ftp_login_Pass As String = "lwdhcp"
    Public Fsu_Ftp_Conn_Port As Integer = 21

    Public Fsu_Telnet_Conn_Port As Integer = 23
    Public Fsu_Telnet_Conn_User As String = "root"
    Public Fsu_Telnet_Conn_Pass As String = "netviewu"

    Public Fsu_Ftp_login_Pass_new As String = "Zxm10*lwdhcp"
    Public Fsu_Telnet_Conn_Pass_new As String = "Zxm10*netviewu"

    Public parameter_xml_file As String = Application.StartupPath & "\parameter_fsu.xml"
    Public key_str As String = "Q3Es5Z02"  'DES加解密密钥 key "Q3Es5Z02"
    Public iv_str As String = "pUy8G6M2"   'DES加解密初始化向量 IV "pUy8G6M2"



    Public Sub get_parameter_from_xml(ByVal xml_file As String)
        Try
            Dim doc As New XmlDocument
            doc.Load(xml_file)
            Dim node_1 As XmlNode = doc.SelectSingleNode("root")
            Dim node_2 As XmlNode = node_1.SelectSingleNode("fsu")
            Dim node_3 As XmlNode = node_2.SelectSingleNode("fsu_ftp_parameter")
            Dim node_4 As XmlNode = node_2.SelectSingleNode("fsu_telnet_parameter")

            Fsu_Ftp_Login_User = node_3.SelectSingleNode("ftp_login_user").InnerXml
            Fsu_Ftp_login_Pass = DecryptDes(node_3.SelectSingleNode("ftp_login_pass").InnerXml, key_str, iv_str)
            Fsu_Ftp_Conn_Port = node_3.SelectSingleNode("ftp_port").InnerXml


            Fsu_Telnet_Conn_Port = node_4.SelectSingleNode("telnet_port").InnerXml
            Fsu_Telnet_Conn_User = node_4.SelectSingleNode("telnet_login_user").InnerXml
            Fsu_Telnet_Conn_Pass = DecryptDes(node_4.SelectSingleNode("telnet_login_pass").InnerXml, key_str, iv_str)
        Catch ex As Exception
            Create_xml_fsu_parameter(xml_file)
        End Try
    End Sub

    Public Sub Update_parameter_to_xml(ByVal xml_file As String)
        Dim doc As New XmlDocument
        doc.Load(xml_file)
        Dim node_1 As XmlNode = doc.SelectSingleNode("root")
        Dim node_2 As XmlNode = node_1.SelectSingleNode("fsu")
        Dim node_3 As XmlNode = node_2.SelectSingleNode("fsu_ftp_parameter")
        Dim node_4 As XmlNode = node_2.SelectSingleNode("fsu_telnet_parameter")

        node_3.SelectSingleNode("ftp_login_user").InnerXml = Fsu_Ftp_Login_User
        node_3.SelectSingleNode("ftp_login_pass").InnerXml = EncryptDes(Fsu_Ftp_login_Pass, key_str, iv_str)
        node_3.SelectSingleNode("ftp_port").InnerXml = Fsu_Ftp_Conn_Port

        node_4.SelectSingleNode("telnet_port").InnerXml = Fsu_Telnet_Conn_Port
        node_4.SelectSingleNode("telnet_login_user").InnerXml = Fsu_Telnet_Conn_User
        node_4.SelectSingleNode("telnet_login_pass").InnerXml = EncryptDes(Fsu_Telnet_Conn_Pass, key_str, iv_str)

        doc.Save(xml_file)
    End Sub

    Public Sub Create_xml_fsu_parameter(ByVal xml_file_path As String)
        Dim xmlreport As XmlDocument
        xmlreport = New XmlDocument
        xmlreport.LoadXml("<?xml version=""1.0"" encoding=""UTF-8"" ?><root/>")
        Dim knode As XmlNode
        knode = xmlreport.SelectSingleNode("root")
        Dim xnode As XmlNode
        Dim xnode_1 As XmlNode
        Dim xnode_2 As XmlNode

        xnode = xmlreport.CreateNode(XmlNodeType.Element, "fsu", "")

        xnode_1 = xmlreport.CreateNode(XmlNodeType.Element, "fsu_ftp_parameter", "")
        xnode_2 = xmlreport.CreateElement("ftp_port")
        xnode_2.InnerXml = Fsu_Ftp_Conn_Port
        xnode_1.AppendChild(xnode_2)

        xnode_2 = xmlreport.CreateElement("ftp_login_user")
        xnode_2.InnerXml = Fsu_Ftp_Login_User
        xnode_1.AppendChild(xnode_2)

        xnode_2 = xmlreport.CreateElement("ftp_login_pass")
        xnode_2.InnerXml = EncryptDes(Fsu_Ftp_login_Pass, key_str, iv_str)
        xnode_1.AppendChild(xnode_2)

        xnode.AppendChild(xnode_1)
        knode.AppendChild(xnode)

        xnode_1 = xmlreport.CreateNode(XmlNodeType.Element, "fsu_telnet_parameter", "")
        xnode_2 = xmlreport.CreateElement("telnet_port")
        xnode_2.InnerXml = Fsu_Telnet_Conn_Port
        xnode_1.AppendChild(xnode_2)

        xnode_2 = xmlreport.CreateElement("telnet_login_user")
        xnode_2.InnerXml = Fsu_Telnet_Conn_User
        xnode_1.AppendChild(xnode_2)

        xnode_2 = xmlreport.CreateElement("telnet_login_pass")
        xnode_2.InnerXml = EncryptDes(Fsu_Telnet_Conn_Pass, key_str, iv_str)
        xnode_1.AppendChild(xnode_2)

        xnode.AppendChild(xnode_1)
        knode.AppendChild(xnode)

        xmlreport.Save(xml_file_path)
    End Sub

    Public Function Check_file_isexist(ByVal filepath As String) As Boolean
        If My.Computer.FileSystem.FileExists(filepath) Then
            Return True
        Else
            Return False
        End If
    End Function

    '密码解密函数 使用DES对称解密
    Public Function DecryptDes(ByVal SourceStr As String, ByVal myKey As String, ByVal myIV As String) As String    '使用标准DES对称解密
        Dim des As New System.Security.Cryptography.DESCryptoServiceProvider 'DES算法
        'Dim des As New System.Security.Cryptography.TripleDESCryptoServiceProvider 'TripleDES算法
        des.Key = System.Text.Encoding.UTF8.GetBytes(myKey) 'myKey DES用8个字符，TripleDES要24个字符
        des.IV = System.Text.Encoding.UTF8.GetBytes(myIV) 'myIV DES用8个字符，TripleDES要8个字符
        Dim buffer As Byte() = Convert.FromBase64String(SourceStr)
        Dim ms As New System.IO.MemoryStream(buffer)
        Dim cs As New System.Security.Cryptography.CryptoStream(ms, des.CreateDecryptor(), System.Security.Cryptography.CryptoStreamMode.Read)
        Dim sr As New System.IO.StreamReader(cs)
        DecryptDes = sr.ReadToEnd()
    End Function

    '密码加密函数 使用DES对称加密
    Public Function EncryptDes(ByVal SourceStr As String, ByVal myKey As String, ByVal myIV As String) As String '使用的DES对称加密
        Dim des As New System.Security.Cryptography.DESCryptoServiceProvider 'DES算法
        'Dim des As New System.Security.Cryptography.TripleDESCryptoServiceProvider 'TripleDES算法
        Dim inputByteArray As Byte()
        inputByteArray = System.Text.Encoding.Default.GetBytes(SourceStr)
        des.Key = System.Text.Encoding.UTF8.GetBytes(myKey) 'myKey DES用8个字符，TripleDES要24个字符
        des.IV = System.Text.Encoding.UTF8.GetBytes(myIV) 'myIV DES用8个字符，TripleDES要8个字符
        Dim ms As New System.IO.MemoryStream
        Dim cs As New System.Security.Cryptography.CryptoStream(ms, des.CreateEncryptor(), System.Security.Cryptography.CryptoStreamMode.Write)
        Dim sw As New System.IO.StreamWriter(cs)
        sw.Write(SourceStr)
        sw.Flush()
        cs.FlushFinalBlock()
        ms.Flush()
        EncryptDes = Convert.ToBase64String(ms.GetBuffer(), 0, ms.Length)

    End Function

End Module
