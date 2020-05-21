Public Class Form2
    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        Me.Dispose()
        Me.Close()
    End Sub

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        If TextBox2.Text <> "" Then
            Dim kl = TextBox2.Text.Trim.Split(vbCrLf)
            Dim kip As String = ""
            Dim k As Integer
            If Not CheckBox1.Checked = True Then
                Form1.DataGridView1.Rows.Clear()
            End If
            For i As Integer = 0 To UBound(kl)
                kip = Trim(kl(i).Trim.Replace(vbCr, "").Replace(vbLf, "") & "")
                If kip <> "" Then
                    k = Form1.DataGridView1.Rows.Add()
                    Form1.DataGridView1.Rows(k).Cells(0).Value = kip
                    Form1.DataGridView1.Rows(k).Cells(0).Tag = Fsu_Telnet_Conn_Pass

                    Form1.DataGridView1.Rows(k).Cells(1).Value = Fsu_Ftp_Conn_Port
                    Form1.DataGridView1.Rows(k).Cells(1).Tag = Fsu_Telnet_Conn_Port

                    Form1.DataGridView1.Rows(k).Cells(2).Value = Fsu_Ftp_Login_User
                    Form1.DataGridView1.Rows(k).Cells(2).Tag = Fsu_Telnet_Conn_User

                    Form1.DataGridView1.Rows(k).Cells(3).Value = "".PadLeft(8, "*")
                    Form1.DataGridView1.Rows(k).Cells(3).Tag = Fsu_Ftp_login_Pass
                End If
            Next
        End If
        Me.Dispose()
        Me.Close()
    End Sub
End Class