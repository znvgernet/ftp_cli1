Public Class Form3
    Private Sub Form3_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        TextBox1.Text = Fsu_Ftp_Conn_Port
        TextBox2.Text = Fsu_Ftp_Login_User
        TextBox3.Text = Fsu_Ftp_login_Pass

        TextBox4.Text = Fsu_Telnet_Conn_Port
        TextBox5.Text = Fsu_Telnet_Conn_User
        TextBox6.Text = Fsu_Telnet_Conn_Pass

    End Sub

    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        Me.Close()
        Me.Dispose()
    End Sub

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        Fsu_Ftp_Conn_Port = TextBox1.Text
        Fsu_Ftp_Login_User = TextBox2.Text
        Fsu_Ftp_login_Pass = TextBox3.Text

        Fsu_Telnet_Conn_Port = TextBox4.Text
        Fsu_Telnet_Conn_User = TextBox5.Text
        Fsu_Telnet_Conn_Pass = TextBox6.Text
        Update_parameter_to_xml(parameter_xml_file)
        Me.Close()
        Me.Dispose()
    End Sub

    Private Sub TextBox3_TextChanged(sender As Object, e As EventArgs) Handles TextBox3.TextChanged

    End Sub

    Private Sub TextBox3_DoubleClick(sender As Object, e As EventArgs) Handles TextBox3.DoubleClick
        If TextBox3.PasswordChar = "*" Then
            TextBox3.PasswordChar = ""
        Else
            TextBox3.PasswordChar = "*"
        End If
    End Sub

    Private Sub TextBox6_DoubleClick(sender As Object, e As EventArgs) Handles TextBox6.DoubleClick
        If TextBox6.PasswordChar = "*" Then
            TextBox6.PasswordChar = ""
        Else
            TextBox6.PasswordChar = "*"
        End If
    End Sub

    Private Sub Button4_Click(sender As Object, e As EventArgs) Handles Button4.Click
        TextBox6.Text = Fsu_Telnet_Conn_Pass_new
        TextBox3.Text = Fsu_Ftp_login_Pass_new
    End Sub

    Private Sub Button3_Click(sender As Object, e As EventArgs) Handles Button3.Click
        TextBox6.Text = "netviewu"
        TextBox3.Text = "lwdhcp"
    End Sub
End Class