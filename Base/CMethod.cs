using System;
using System.Collections.Generic;
using System.Text;

namespace Base
{
    class CMethod
    {
    }
//Option Explicit

//'##ModelId=3858332802BE
//Private mSvr            As CBaseServer
//'##ModelId=3858332802D2
//Private mCnt            As Connection
//'##ModelId=3858332802DA
//Private mCorpID         As Long

//' 存储过程的封装函数

//' 功能：封装sp_Autonumber，返回某个表的下一个AutoNumber的ID号。
//'##ModelId=385833280316
//Public Function sp_AutoNumber(sTabName As String, Optional IncStep As Long = 1) As Long
//Attribute sp_AutoNumber.VB_Description = "Stored procedure sp_AutoNubmer in MainDB."
//     On Error GoTo E
    
//    Dim cmdAutoNumber As Command
    
//    Set cmdAutoNumber = New Command
//    mCnt.Errors.Clear ' 在调用SP前将错误集合清空, by WZQ
//    With cmdAutoNumber
//        If Sys.CurrentDBType = dbtSQLSERVER65 Or Sys.CurrentDBType = dbtSQLSERVER70 Then
//            .CommandText = "sp_AutoNumber"
//            .CommandType = adCmdStoredProc
//            .Parameters.Append .CreateParameter(, adInteger, adParamReturnValue, 4)
//            .Parameters.Append .CreateParameter(, adVarChar, adParamInput, 30, sTabName)
//            .Parameters.Append .CreateParameter(, adInteger, adParamInput, 4, IncStep)
//        ElseIf Sys.CurrentDBType = dbtIBMDB25x Then
//            .CommandText = "BKGYV2SP!sp_AutoNumber"
//            .CommandType = adCmdStoredProc
//            .Parameters.Append .CreateParameter(, adInteger, adParamOutput, 4)
//            .Parameters.Append .CreateParameter(, adVarChar, adParamInput, 30, sTabName)
//            .Parameters.Append .CreateParameter(, adInteger, adParamInput, 4, IncStep)
//        End If

//        Set .ActiveConnection = mCnt
//        .Execute
//        sp_AutoNumber = .Parameters(0).Value
//        mSvr.Err.PushAdoErrors mCnt  ' 我在sp_AutoNumber中使用了RaisError，如果有错误，可以在这里将它记录下来, by WZQ
//    End With
    
//    Set cmdAutoNumber = Nothing
//    Exit Function
//E:
//    ' 执行sp_AutoNumber出错，记录日志，将结果置为-1
//    mSvr.Err.PushErrObject
//    mSvr.Err.PushAdoErrors mCnt
//    Set cmdAutoNumber = Nothing
//    sp_AutoNumber = -1
//    'mSvr.Log.AddLog sys.Res.GetString(STR_SP_AUTONUMBER_FAIL), gyLogTypeError, "CSPWrapper.sp_AutoNumber"
//End Function

//'##ModelId=38583328033E
//Public Function SvrLock(ByVal TableName As String, ByVal RecordID As Long, ByVal LockType As Long) As Long
//    SvrLock = sp_Lock(mCorpID, mSvr.Operator.ID, TableName, RecordID, LockType)
//End Function
                        
//'##ModelId=385833280352
//Public Function UpdateAlive() As Long
//On Error GoTo ErrorAttach
//    Dim lrtn            As Long
//    Dim sErrSource      As String
//    sErrSource = "CMethod.UpdateAlive()"
    
//    Dim sql             As String
//    Dim Rst             As Recordset
    
//    sql = "Select KeepAlive From S_ENTRY Where OperatorID=" & CStr(mSvr.Operator.ID)
//    Set Rst = New Recordset
//    Rst.Open sql, mCnt, adOpenDynamic, adLockOptimistic, adCmdText
//    If Rst.BOF And Rst.EOF Then
//        Rst.Close
//        lrtn = STR_ENTRY_NO_SUCH_OP
//        GoTo MyLogicError
//    End If
    
//    Rst!KeepAlive = Now()
//    Rst.Update
//    Rst.Close
//    Exit Function
//    lrtn = gnSUCCESS
//ExitEntry:
//    Set Rst = Nothing
//    UpdateAlive = lrtn
//    Exit Function
//ErrorAttach:
//    lrtn = STR_ENTRY_ERROR_KEEP_ALIVE
//    mSvr.Err.PushAdoErrors mCnt
//    mSvr.Err.Push Err.Number, sprintf("%s.%s", Err.Source, sErrSource), Err.Description
//    Resume BackOut
//MyLogicError:
//    mSvr.Err.Push lrtn, sprintf(sErrSource), ""
//    GoTo BackOut
//BackOut:
//    GoTo ExitEntry:
//End Function

//'------------------------------------------------------------------------------'
//'〖方法〗封装sp_LogicLock
//'〖参数〗
//'　　　　CorpID         ：公司 ID
//'　　　　OperatorID     ：操作员 ID
//'　　　　TableName      ：逻辑表名称
//'　　　　RecordID       ：逻辑记录 ID
//'　　　　LockType       ：锁的操作类型
//'〖返回〗gnSUCCESS
//'　　　　STR_LOCK_WRN_*         #DEBUG_VERSION <> 1 时直接记入日志，返回 gnSUCCESS
//'　　　　STR_LOCK_ERR_*
//'　　　　STR_UNEXPECTED_ERROR
//'〖说明〗
//' -------------------------------------------
//'| 锁类型   | 可忽略（无意义）参数　　　     |
//'| LockType | CorpID | TableName  | RecordID |
//'|企业锁    | Y      | Y          | Y        |
//'|公司锁    |        | Y          | Y        |
//'|表锁　    |        |            | Y        |
//'|记录锁    |        |            |          |
//' -------------------------------------------
//'〖维护〗赵剑英
//'〖时间〗1999年02月05日
//'------------------------------------------------------------------------------'
//'##ModelId=385833280366
//Private Function sp_Lock(ByVal CorpID As Long, ByVal OperatorID As Long, ByVal TableName As String, ByVal RecordID As Long, ByVal LockType As Long) As Long
//    On Error GoTo ErrorAttach

//    Const sERR_SOURCE = "CMethod.sp_Lock(%d,%d,%s,%d,%d)"
    
//    ' 用于追踪错误
//    Dim lrtn            As Long
//    Dim lMyRtn          As Long
    
//    Dim cmdLock As Command

//    Set cmdLock = New Command
//    With cmdLock
//        If Sys.CurrentDBType = dbtSQLSERVER65 Or Sys.CurrentDBType = dbtSQLSERVER70 Then
//            .CommandText = "{?=call sp_LogicLock(?,?,?,?,?)}"
//            .Parameters.Append .CreateParameter(, adInteger, adParamReturnValue, 4)
//            .Parameters.Append .CreateParameter(, adInteger, adParamInput, 4, CorpID)
//            .Parameters.Append .CreateParameter(, adInteger, adParamInput, 4, OperatorID)
//            .Parameters.Append .CreateParameter(, adVarChar, adParamInput, 30, TableName)
//            .Parameters.Append .CreateParameter(, adInteger, adParamInput, 4, RecordID)
//            .Parameters.Append .CreateParameter(, adInteger, adParamInput, 4, LockType)
//        ElseIf Sys.CurrentDBType = dbtIBMDB25x Then
//            .CommandText = "BKGYV2SP!sp_LogicLock"
//            .CommandType = adCmdStoredProc
//            .Parameters.Append .CreateParameter(, adInteger, adParamOutput, 4)
//            .Parameters.Append .CreateParameter(, adInteger, adParamInput, 4, CorpID)
//            .Parameters.Append .CreateParameter(, adInteger, adParamInput, 4, OperatorID)
//            .Parameters.Append .CreateParameter(, adVarChar, adParamInput, 30, TableName)
//            .Parameters.Append .CreateParameter(, adInteger, adParamInput, 4, RecordID)
//            .Parameters.Append .CreateParameter(, adInteger, adParamInput, 4, LockType)
//        End If
//        Set .ActiveConnection = mCnt
//        .Execute
//        lrtn = .Parameters(0).Value
//    End With
//    Set cmdLock = Nothing

//    If lrtn > 0 Then
//        GoTo LogicWarning
//    ElseIf lrtn < 0 Then
//        GoTo LogicError
//    End If
    
//    sp_Lock = gnSUCCESS
//ExitEntry:
//    Set cmdLock = Nothing
//    On Error Resume Next
//    mCnt.Errors.Clear
//    Exit Function
//ErrorAttach:
//    sp_Lock = STR_UNEXPECTED_ERROR
//#If DEBUG_VERSION = 1 Then
//    mSvr.Err.PushAdoErrors mCnt
//#Else
//    mCnt.Errors.Clear
//#End If
//    mSvr.Err.Push Err.Number, Err.Source & "." & sprintf(sERR_SOURCE, CorpID, OperatorID, AddQM(TableName), RecordID, LockType), Err.Description
//    Resume ExitEntry
//LogicError:
//    Select Case lrtn
//        Case -1
//            lMyRtn = STR_LOCK_ERR_INVALID_CORPID
//        Case -2
//            lMyRtn = STR_LOCK_ERR_INVALID_OPERATORID
//        Case -3
//            lMyRtn = STR_LOCK_ERR_INVALID_TABLENAME
//        Case -4
//            lMyRtn = STR_LOCK_ERR_INVALID_RECORDID
//        Case -5
//            lMyRtn = STR_LOCK_ERR_INVALID_LOCKTYPE
//        Case -6
//            lMyRtn = STR_LOCK_ERR_HEIGHT_CONFLICT
//        Case -7
//            lMyRtn = STR_LOCK_ERR_NORMAL_CONFLICT
//        Case -8
//            lMyRtn = STR_LOCK_ERR_LOW_CONFLICT
//    End Select
//    mSvr.Err.Push lMyRtn, sprintf(sERR_SOURCE, CorpID, OperatorID, AddQM(TableName), RecordID, LockType), ""
//    sp_Lock = lMyRtn
//    GoTo ExitEntry
//LogicWarning:
//    Select Case lrtn
//        Case 1
//            lMyRtn = STR_LOCK_WRN_DOUBLE_LOCK
//        Case 2
//            lMyRtn = STR_LOCK_WRN_LOCK_NONENTITY
//    End Select

//End Function

//'##ModelId=385833280384
//Public Sub Initialize(ByRef Svr As CBaseServer, ByRef cntSYS As Connection)
//    Set mSvr = Svr
//    Set mCnt = cntSYS
//    mCorpID = mSvr.Properties(gsBASEINFO_DEF_CORP_ID)
//End Sub

//'##ModelId=385833280399
//Public Sub Terminate()
//    On Error Resume Next

//    If Not mCnt Is Nothing Then
//        Set mCnt = Nothing
//    End If
//    Set mSvr = Nothing
//End Sub

//'##ModelId=3858332803A3
//Private Sub Class_Terminate()
//    Terminate
//End Sub

}
