using System;
using System.Collections.Generic;
using System.Text;

namespace Base
{
    class CErrors
    {
    }
//Option Explicit

//'##ModelId=38583F2D006A
//Private mSvr            As CBaseServer
//'##ModelId=38583F2D00CC
//Private mCol            As Collection

//' ���ص�ǰ�����ջ�е����д������������ѡ����ClearStack�����Ƿ��ڷ��������󽫶�ջ��ա�
//'##ModelId=38583F2D00FE
//Public Function GetAllErrorsDescription(Optional ClearStack As Boolean = True, Optional SaveToLog As Boolean = True) As String
//    Dim s As String, I As Integer
    
//    If mCol.Count = 0 Then
//        s = "<��ǰû�и���ϸ�Ĵ�����Ϣ��>"
//    Else
//        s = "����������" & mCol.Count
        
//        For I = mCol.Count To 1 Step -1
//            s = s & vbCrLf & _
//              String(15, "-") & I & String(15, "-") & vbCrLf & _
//              mCol(I).FullDescription
//        Next
//    End If
    
//    GetAllErrorsDescription = s
    
//    If ClearStack Then Call Clear(SaveToLog)
//End Function

//'��ն�ջ
//'##ModelId=38583F2D0158
//Public Sub Clear(Optional SaveToLog As Boolean = True)

//    ' ���������
//    Set mCol = New Collection
//End Sub

//'*******************************************
//'���ܣ�������Ϣѹջ
//'���� ��
//'   lngNumber As Long'�������
//'   strSource As String'����Դ
//'   strDescription As String   '����������Ϣ
//'   lngSQLRetCode As Long      'SQL���ش�����
//'****************************************
//'##ModelId=38583F2D0176
//Public Function Push(ByVal lngNumber As Long, strSource As String, strDescription As String, Optional ByVal sSQLState As String) As CError
//    Dim clsError As CError
//    Set clsError = New CError
    
//    With clsError
//        .Number = lngNumber
//        .Source = strSource
//        .Description = strDescription
//        .SQLState = sSQLState
//    End With
    
//    If Not CheckDuplicateError(clsError) Then mCol.Add clsError
    
//    Set Push = clsError
//End Function

//'*******************************************
//'���ܣ�������Ϣѹջ
//'���� ��
//'   oError As object '������
//'****************************************
//'##ModelId=38583F2D018A
//Public Function PushErrObject() As CError
//    If Err.Number = 0 Then Exit Function
    
//    Dim clsErr As CError
//    Set clsErr = New CError
    
//    With clsErr
//        .Number = Err.Number
//        .Source = Err.Source
//        .Description = Err.Description
//    End With
    
//    If Not CheckDuplicateError(clsErr) Then mCol.Add clsErr
//    Set PushErrObject = clsErr
//End Function

//'*******************************************
//'���ܣ�ADO������Ϣѹջ
//'���� ��
//'   cnt ��ѹջ�����ݿ�����
//'****************************************
//'##ModelId=38583F2D019E
//Public Function PushAdoErrors(Optional cnt As Connection = Nothing) As CError
//    Dim clsErr As CError
//    Dim clsAdoErr       As ADODB.Error
//    Dim cntErr          As Connection
    
//    Set cntErr = IIf(cnt Is Nothing, mSvr.cntMain, cnt)
    
//    ' �����ݿ��еĴ���
//    If Not cntErr Is Nothing Then
//        For Each clsAdoErr In cntErr.Errors
//            If clsAdoErr.Number <> 0 Then
//                Set clsErr = New CError
                
//                With clsErr
//                    .Number = clsAdoErr.Number
//                    .Source = clsAdoErr.Source
//                    .Description = clsAdoErr.Description
//                    .SQLState = clsAdoErr.SQLState
//                End With
                
//                If Not CheckDuplicateError(clsErr) Then mCol.Add clsErr
//            End If
//        Next
//        cntErr.Errors.Clear
//    End If
    
//    Set PushAdoErrors = clsErr
//End Function

//'*******************************************
//'���ܣ�������Ϣ��ջ
//'****************************************
//'##ModelId=38583F2D01B2
//Public Function Pop(Optional SaveToLog As Boolean = True) As CError
//    Dim clsErr As CError
    
//    If mCol.Count > 0 Then
//        Set Pop = mCol(mCol.Count)
//        mCol.Remove mCol.Count
//    End If
//End Function

//'*******************************************
//'���ܣ�������Ϣ��
//'****************************************
//'##ModelId=38589183038D
//Public Property Get Count() As Long
//Attribute Count.VB_Description = "*******************************************\r\n���ܣ�������Ϣ��\r\n****************************************"
//    Count = mCol.Count
//End Property

//'##ModelId=38583F2D01DA
//Public Sub Initialize(ByRef Svr As CBaseServer)
//    Set mSvr = Svr
//End Sub

//'##ModelId=38583F2D0202
//Public Sub Terminate()
//On Error Resume Next
//    If Not mSvr Is Nothing Then
//        PushAdoErrors
//        Clear
//        Set mSvr = Nothing
//    End If
//End Sub

//' Ϊ�˷�ֹ�ظ�����ͬ���Ĵ���
//'##ModelId=38583F2D0216
//Private Function CheckDuplicateError(E As CError) As Boolean
//    Dim e1 As CError
//    If mCol.Count > 0 Then
//        Set e1 = mCol(mCol.Count)
//        With E
//            If .Number = e1.Number And _
//                .Source = e1.Source And _
//                .SQLState = e1.SQLState And _
//                .Description = e1.Description Then
//                CheckDuplicateError = True
//                Exit Function
//            End If
//        End With
//    End If
//End Function

//'##ModelId=38583F2D0235
//Private Sub Class_Initialize()
//    Set mCol = New Collection
//End Sub

//'##ModelId=38583F2D023F
//Private Sub Class_Terminate()
//    Terminate
//    Set mCol = Nothing
//End Sub


}
