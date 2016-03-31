using System;
using System.Collections.Generic;
using System.Text;

namespace Base
{
    class CEntry
    {
    }
//' 登录控制类
//Option Explicit

//'##ModelId=3858918000E9
//Public Type EntryType
//    '##ModelId=385891800139
//    OpID As Long
//    '##ModelId=38589180014D
//    Name As String
//    '##ModelId=385891800161
//    Computer As String
//    '##ModelId=385891800176
//    Application As String
//    '##ModelId=38589180018A
//    LogonTime As Date
//    '##ModelId=3858918001A8
//    UpdateTime As Date
//    '##ModelId=3858918001BC
//    LogonMode As LogonModeConstants
//End Type

//'##ModelId=38583317038A
//Public Tag              As String

//'##ModelId=3858331703BC
//Private Const msTAB_S_ENTRY = "S_Entry"
//'##ModelId=3858331703C6
//Private Const msFLD_ENTRY_ID = "EntryID"
//'##ModelId=3858331703DA
//Private Const msFLD_OPERATOR_ID = "OperatorID"
//'##ModelId=385833180006
//Private Const msFLD_APP_CODE = "AppCode"
//'##ModelId=38583318001A
//Private Const msFLD_MACHINE_CODE = "MachineCode"
//'##ModelId=385833180024
//Private Const msFLD_LOGON_TIME = "LogonTime"
//'##ModelId=385833180038
//Private Const msFLD_KEEP_ALIVE = "KeepAlive"
//'##ModelId=38583318004C
//Private Const msFLD_LOGON_MODE = "LogonMode"

//'##ModelId=385833180060
//Private Const msTAB_P_OPERATOR = "P_Operator"
//'##ModelId=38583318006A
//Private Const msPFLD_ID = "ID"
//'##ModelId=38583318007E
//Private Const msPFLD_NAME = "Name"

//'##ModelId=385833180092
//Private Const msTAB_S_LOCK = "S_Lock"
//'##ModelId=3858331800A6
//Private Const msLFLD_CLERK_ID = "P_ClerkID"

//'##ModelId=3858331800DA
//Private mSvr            As CBaseServer
//'##ModelId=385833180117
//Private mrsFind         As Recordset

//' 返回第一个满足搜索条件的人员ID号。找不到则gnNULLID
//'##ModelId=38583318013D
//Public Function FindFirst(Optional ByVal LogonMode As LogonModeConstants = lmNormal) As Long
//    If Not mrsFind Is Nothing Then
//        If mrsFind.State = adStateOpen Then
//            mrsFind.Close
//        End If
//    End If

//    Set mrsFind = mSvr.cntMain.Execute("Select * From S_Entry Where LogonMode=" & LogonMode & " order By OperatorID")
//    If mrsFind.EOF And mrsFind.BOF Then
//        ' No match record.
//        FindFirst = gnNULLID
//        mrsFind.Close
//        Set mrsFind = Nothing
//        Exit Function
//    End If
        
//    FindFirst = mrsFind!OperatorID
//    Tag = mrsFind!MachineID
//    mrsFind.MoveNext
//    ' Don't close mrsFind, for it will be used in FindNext.
//End Function

//' 返回下一个满足搜索条件的人员ID号。找不到则gnNULLID
//'##ModelId=38583318016F
//Public Function FindNext() As Long
//    If Not mrsFind Is Nothing Then
//        If mrsFind.State = adStateOpen Then
//            If Not mrsFind.EOF Then
//                FindNext = mrsFind!OperatorID
//                Tag = mrsFind!MachineID
//                mrsFind.MoveNext
//                Exit Function
//            Else
//                mrsFind.Close
//            End If
//        End If
//    End If
    
//    FindNext = gnNULLID
//End Function

//' Check if specified operator logged on.
//' return:
//'   0   No record match the OperatorID
//'   1   Exists, and machine and application is same.
//'   2   Exists, and machine or application is different.
//'##ModelId=385833180183
//Public Function FindOperatorID(ByVal OpID As Long, ByVal AppCode As String, ByVal MachineCode As String) As Long
//    Dim rs As Recordset
    
//    Set rs = mSvr.cntMain.Execute("Select * From S_Entry where OperatorID=" & CStr(OpID))
//    If rs.BOF And rs.EOF Then
//        FindOperatorID = 0
//        rs.Close
//        Exit Function
//    End If
        
//    If (StrComp(rs!MachineID, MachineCode, vbTextCompare) = 0) And _
//       (StrComp(rs!AppID, AppCode, vbTextCompare) = 0) Then
//        FindOperatorID = 1
//        rs.Close
//        Exit Function
//    End If
    
//    FindOperatorID = 2
//    Tag = rs!MachineID & "(" & rs!AppID & ")"
//    rs.Close
//End Function

//'##ModelId=385833180197
//Public Function Insert(ByVal OpID As Long, Optional ByVal LogonMode As LogonModeConstants = lmNormal) As Long
//On Error GoTo ErrorAttach
//    Dim lrtn            As Long
//    Dim sErrSource      As String
//    sErrSource = "CEntry.Insert()"

//    Dim sql             As String
//    Dim Rst             As Recordset
//    Dim lID             As Long
//    Dim bInserted       As Boolean
//    Dim sOp             As String
//    Dim sMachineCode    As String
//    Dim sAppCode        As String
    
//    sql = sprintf("SELECT * FROM %s WHERE (%s=%d) AND (%s='%s') AND (%s='%s')", _
//                  msTAB_S_ENTRY, msFLD_OPERATOR_ID, OpID, _
//                  msFLD_APP_CODE, mSvr.AppCode, _
//                  msFLD_MACHINE_CODE, mSvr.MachineCode)
//    Set Rst = mSvr.cntMain.Execute(sql, , adCmdText)
    
//    ' 如果有相同操作员、应用程序、工作站的登录信息，则认为上次是非正常退出
//    ' 删除旧记录
//    If Not (Rst.BOF And Rst.EOF) Then
//        Rst.Close
//        lrtn = DELETE(OpID)
//        If lrtn <> gnSUCCESS Then GoTo BackOut
//    Else
//        Rst.Close
//    End If
//    ' 先尝试
//    lID = mSvr.Method.sp_AutoNumber(msTAB_S_ENTRY)
//    sql = sprintf("INSERT INTO %s(%s, %s, %s, %s, %s, %s, %s) " & _
//                  "VALUES(%d, %d, '%s', '%s', '%s', '%s', %d)", _
//                  msTAB_S_ENTRY, msFLD_ENTRY_ID, msFLD_OPERATOR_ID, msFLD_APP_CODE, _
//                  msFLD_MACHINE_CODE, msFLD_LOGON_TIME, msFLD_KEEP_ALIVE, msFLD_LOGON_MODE, _
//                  lID, OpID, mSvr.AppCode, mSvr.MachineCode, Date2Str(Now), Date2Str(Now), LogonMode)
//    On Error Resume Next
//    mSvr.cntMain.Execute sql, , adCmdText
//    If Err.Number <> 0 Then
//        mSvr.cntMain.Errors.Clear
//    Else
//        bInserted = True
//    End If
//    On Error GoTo ErrorAttach
//    ' 再检测
//    sql = sprintf("SELECT * FROM %s WHERE (%s=%d) OR (%s=%d)", _
//                  msTAB_S_ENTRY, msFLD_OPERATOR_ID, OpID, msFLD_LOGON_MODE, lmExclusive)
//    Set Rst = mSvr.cntMain.Execute(sql, , adCmdText)
//    While Not Rst.EOF
//        If Rst(msFLD_OPERATOR_ID) = OpID Then
//'            If (Rst(msFLD_APP_CODE) = mSvr.AppCode) And _
//'                (Rst(msFLD_MACHINE_CODE) <> mSvr.MachineCode) Then
//'                lrtn = STR_OP_USER_ALREADY_LOGON
//'                GoTo MyLogicError
//'            End If
//        Else
//            lrtn = STR_OP_EXCLUSIVE
//            GoTo MyLogicError
//        End If
//        Rst.MoveNext
//    Wend
//    Rst.Close
    
//    lrtn = gnSUCCESS
//ExitEntry:
//    Set Rst = Nothing
//    Insert = lrtn
//    Exit Function
//ErrorAttach:
//    lrtn = STR_UNEXPECTED_ERROR
//    mSvr.Err.Push Err.Number, sprintf("%s.%s%nLine:%d", Err.Source, sErrSource, Erl), Err.Description
//    Resume BackOut
//MyLogicError:
//    On Error Resume Next
//    'Add by yc 2001/04/15
//    Tag = Rst(msFLD_MACHINE_CODE)
    
//    sOp = Rst(msFLD_OPERATOR_ID)
//    sMachineCode = Rst(msFLD_MACHINE_CODE)
//    sAppCode = Rst(msFLD_APP_CODE)
//    Rst.Close
//    sql = sprintf("SELECT %s FROM %s WHERE %s=%d", msPFLD_NAME, msTAB_P_OPERATOR, msPFLD_ID, sOp)
//    Set Rst = mSvr.cntMain.Execute(sql, , adCmdText)
//    sOp = Rst(msPFLD_NAME)
//    Rst.Close
//    If bInserted Then
//        mSvr.cntMain.Execute sprintf("DELETE FROM %s WHERE %s=%d", msTAB_S_ENTRY, msFLD_ENTRY_ID, lID), , adCmdText
//    End If
//    mSvr.Err.Push lrtn, sprintf("操作员：%s%n工作站：%s%n程序：%s%n", sOp, sMachineCode, sAppCode), ""
//    GoTo BackOut
//BackOut:
//    GoTo ExitEntry:
//End Function

//'##ModelId=3858331801AB
//Public Function Update(ByVal OpID As Long, ByVal LogonMode As LogonModeConstants) As Long
//On Error GoTo ErrorAttach
//    Dim lrtn            As Long
//    Dim sErrSource      As String
//    sErrSource = "CEntry.Update()"

//    Dim sql             As String
//    Dim Rst             As Recordset
//    Dim lRecordAffected As Long
//    ' 是否独占冲突
//    If LogonMode = lmExclusive Then
//        'Modified By YC 2001/04/13 加上了AppCode
//        sql = sprintf("SELECT * FROM %s WHERE (%s<>%d) AND (%s=%d) And (%s='%s')", _
//                      msTAB_S_ENTRY, msFLD_OPERATOR_ID, OpID, msFLD_LOGON_MODE, lmExclusive, msFLD_APP_CODE, mSvr.AppCode)
//        Set Rst = mSvr.cntMain.Execute(sql, , adCmdText)
   
//        If Not (Rst.BOF And Rst.EOF) Then
//            sql = sprintf("SELECT %s FROM %s WHERE %s=%d", _
//                          msPFLD_NAME, msTAB_P_OPERATOR, msPFLD_ID, Rst(msFLD_OPERATOR_ID))
//            Rst.Close
//            Set Rst = mSvr.cntMain.Execute(sql, , adCmdText)
//            lrtn = STR_OP_EXCLUSIVE
//            mSvr.Err.Push lrtn, sErrSource, sprintf(Sys.Res.GetString(lrtn), Rst(msPFLD_NAME))
//            Rst.Close
//            GoTo BackOut
//        Else
//            Rst.Close
//        End If
//    End If
//    ' 更新登陆模式
//    sql = sprintf("Update %s SET %s=%d WHERE %s=%d and %s='%s' and %s='%s'", _
//                  msTAB_S_ENTRY, msFLD_LOGON_MODE, LogonMode, msFLD_OPERATOR_ID, OpID, msFLD_APP_CODE, mSvr.AppCode, msFLD_MACHINE_CODE, mSvr.MachineCode)
//    mSvr.cntMain.Execute sql, lRecordAffected, adCmdText
//    If lRecordAffected <> 1 Then
//        lrtn = STR_OP_EMPTY_CHANGE_LOGON
//        GoTo MyLogicError
//    End If
    
//    lrtn = gnSUCCESS
//ExitEntry:
//    Set Rst = Nothing
//    Update = lrtn
//    Exit Function
//ErrorAttach:
//    lrtn = STR_UNEXPECTED_ERROR
//    mSvr.Err.Push Err.Number, sprintf("%s.%s%nLine:%d", Err.Source, sErrSource, Erl), Err.Description
//    Resume BackOut
//MyLogicError:
//    mSvr.Err.Push lrtn, sErrSource, ""
//    GoTo BackOut
//BackOut:
//    GoTo ExitEntry:
//End Function

//'##ModelId=3858331801BF
//Public Function DELETE(ByVal OpID As Long) As Long
//On Error GoTo ErrorAttach
//    Dim lrtn            As Long
//    Dim sErrSource      As String
//    sErrSource = "CEntry.DELETE()"
    
    
//    mSvr.cntMain.Execute sprintf("DELETE FROM %s WHERE %s=%d", msTAB_S_LOCK, msLFLD_CLERK_ID, OpID)
    
//    '改为可让多程序登录
//    mSvr.cntMain.Execute sprintf("DELETE FROM %s WHERE %s=%d and %s='%s' and %s='%s'", _
//                                  msTAB_S_ENTRY, msFLD_OPERATOR_ID, OpID, _
//                                  msFLD_APP_CODE, mSvr.AppCode, _
//                                  msFLD_MACHINE_CODE, mSvr.MachineCode)

//    lrtn = gnSUCCESS
//ExitEntry:
//    DELETE = lrtn
//    Exit Function
//ErrorAttach:
//    lrtn = STR_UNEXPECTED_ERROR
//    mSvr.Err.Push Err.Number, sprintf("%s.%s", Err.Source, sErrSource), Err.Description
//    Resume BackOut
//BackOut:
//    GoTo ExitEntry:
//End Function

//'##ModelId=3858331801D3
//Private Sub Class_Terminate()
//    Terminate
//    If Not mrsFind Is Nothing Then
//        If mrsFind.State = adStateOpen Then
//            mrsFind.Close
//        End If
//        Set mrsFind = Nothing
//    End If
//End Sub

//'##ModelId=3858331801F1
//Public Function RetrieveList() As EntryType()
//    On Error GoTo E
    
//    Dim sql As String
//    Dim rs As Recordset
//    Dim UserList() As EntryType
//    Dim I As Integer

//    ' Modified by HL
//    sql = "SELECT S_Entry.* " & _
//        "FROM P_Operator,S_Entry Where " & _
//        "  P_Operator.ID = S_Entry.OperatorID " & _
//        "ORDER BY S_Entry.LogonTime DESC "
//    Set rs = mSvr.cntMain.Execute(sql, , adCmdText)
    
//    I = 0
//    While Not rs.EOF
//        ReDim Preserve UserList(I) As EntryType
//        With UserList(I)
//            .OpID = rs!OperatorID
//            .Name = rs!Name
//            .Computer = rs!MachineID
//            .Application = rs!AppID
//            .LogonTime = Str2Date(rs!DateTime)
//            .UpdateTime = Str2Date(rs!KeepAlive)
//            .LogonMode = rs!LogonMode
//        End With
//        rs.MoveNext
//        I = I + 1
//    Wend
//    rs.Close
//    RetrieveList = UserList
//    Exit Function
//E:
//    If mSvr.cntMain.Errors.Count > 0 Then
//        mSvr.Err.PushAdoErrors
//    Else
//        mSvr.Err.PushErrObject
//    End If
//End Function

//'=============================================================================='-
//'
//'【友元方法】
//'=============================================================================='
//'##ModelId=38583318020F
//Public Sub Initialize(ByRef Svr As CBaseServer)
//    Set mSvr = Svr
//End Sub

//'##ModelId=385833180223
//Public Sub Terminate()
//    Set mSvr = Nothing
//End Sub


}
