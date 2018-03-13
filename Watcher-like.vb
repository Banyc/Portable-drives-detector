Public Class Watcher_like
    Private _IsStarted As Boolean '检测是否执行 RefleshList 函数
    Private Const LogPath As String = "Log.txt" '输出Log的地址
    Private _t As System.Threading.Thread '线程初始器？
    Public _listDriveNames As New List(Of String) '放当前所有驱动器的名字 记得加NEW
    Public _IsListLoaded As Boolean  '判断 list是否已经 加载完毕
    'Public Delegate Sub VoidDelegate() '创建一个 委托 来在多线程 执行 主线程 内容

    Public Sub Start()
        _t = New Threading.Thread(AddressOf RefleshList) '以多线程的方式运行 Watcher
        _IsStarted = True
        WriteLog("Start thread!") '在Log里面表示进行了 开始 操作
        _t.Start()
    End Sub
    '强制停止
    Public Sub Abort()
        If _t.IsAlive Then
            _IsStarted = False
            WriteLog("Abort the thread!")
            _t.Abort()
        Else
            WriteLog("Cannot abort! Because the thread has been dead!")
        End If
    End Sub

    Public Sub Eject()
        WriteLog("Drive ejects!")
    End Sub

    Public Sub FormExit()
        WriteLog("Form exits!" & vbCrLf)
    End Sub

    Private Sub RefleshList()
        _IsListLoaded = False '初始化，表示list还没有被载入
        If _IsStarted Then
            Try
                'RefleshList() '把所有驱动器名添加到list里
                _listDriveNames.Clear() '把上次的记录清除掉
                'For Each drive In My.Computer.FileSystem.Drives '获得所有驱动器
                For Each drive In System.IO.DriveInfo.GetDrives  '获得所有驱动器 跟上面的是一样的
                    If drive.DriveType = System.IO.DriveType.Removable Then '如果驱动器类型是可移动的（U盘）
                        _listDriveNames.Add(drive.Name)
                        WriteLog(drive.Name) '顺便把名字写到Log里
                    End If
                Next drive
                _IsListLoaded = True '表示已经完成 list更新
            Catch ex As Exception
                WriteLog(ex.ToString)
            End Try
        End If
        Abort()
    End Sub

    'Public ReadOnly Property GetList As List(Of String)
    '    Get
    '        Return _listDriveNames
    '    End Get
    'End Property

    Private Sub WriteLog(ByRef writeInText As String)
        Dim f1 As New System.IO.StreamWriter(LogPath, True) 'true代表是追加（如果本身就不存在的话，创建新的 LogPath）
        f1.Write(System.DateTime.Now.ToString & " " & writeInText & vbCrLf)
        f1.Close()
    End Sub
End Class
