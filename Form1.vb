'https://zhidao.baidu.com/question/191858141.html
Public Class Form1
    Dim Watcher As New Watcher_like
    Private t As System.Threading.Thread '创建一个多线程，防止函数影响系统处理U盘
    Private Delegate Sub VoidDelegate() '创建一个 委托 来在多线程 执行 主线程 内容
    Public Const WM_DEVICECHANGE = &H219
    Public Const DBT_DEVICEARRIVAL = &H8000
    Public Const DBT_DEVICEREMOVECOMPLETE = &H8004
    'Dim DriveLetter As String

    Protected Overrides Sub WndProc(ByRef m As System.Windows.Forms.Message)
        If m.Msg = WM_DEVICECHANGE Then
            Select Case m.WParam
                Case DBT_DEVICEARRIVAL
                    'Dim s() As DriveInfo = DriveInfo.GetDrives'.GetDrives 检索计算机上的所有逻辑驱动器的驱动器名称。
                    'For Each drive As DriveInfo In s
                    '    If drive.DriveType = DriveType.Removable Then
                    '        DriveLetter = drive.Name.ToString()
                    '        Debug.WriteLine("U盘 :" & DriveLetter & "已插入.")
                    '    End If
                    'Next
                    Watcher.Start()
                    InitiateThreadRefleshListBox()'RefleshListBox 
                Case DBT_DEVICEREMOVECOMPLETE
                    'Dim s() As DriveInfo = DriveInfo.GetDrives
                    'For Each drive As DriveInfo In s
                    '    If drive.ToString = DriveLetter Then Exit Sub
                    'Next
                    'Debug.WriteLine("U盘:" & DriveLetter & "已卸载！")
                    Watcher.Eject()
                    Watcher.Start()
                    InitiateThreadRefleshListBox() 'BUG 这一步不执行
            End Select
        End If
        MyBase.WndProc(m)
    End Sub

    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Watcher.Start() '为了检测启动软件前 是否已经有已有的驱动器
        InitiateThreadRefleshListBox()
    End Sub

    Private Sub RefleshListBox() '在lamda函数里面不能用 ByRef
        Do '等待Watcher._list生成
            If Watcher._IsListLoaded Then
                Me.Invoke(New VoidDelegate(Sub() 'BUG 这个会执行两次
                                               ListBox1.Items.Clear() '先把之前的内容删掉
                                               'Stop
                                               For Each item In Watcher._listDriveNames
                                                   ListBox1.Items.Add(item)
                                               Next item
                                           End Sub))
                Exit Do
            End If
        Loop
        t.Abort()
    End Sub

    Private Sub InitiateThreadRefleshListBox()
        t = New Threading.Thread(Sub() '使用一个多线程，防止函数影响系统处理U盘
                                     RefleshListBox()
                                 End Sub)
        t.Start()
    End Sub

    Private Sub Form1_Closing(ByVal sender As Object, ByVal e As System.ComponentModel.CancelEventArgs) Handles MyBase.Closing
        Watcher.FormExit()
    End Sub
End Class
