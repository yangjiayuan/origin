using System;
using System.Collections.Generic;
using System.Text;

namespace Base
{
//##############################################################################'-
//
//〖名称〗CBaseServer
//〖说明〗全局对象 Sys(CSystem) 用 OpenServer()方法创建，用 CloseServer() 方法关闭。
//##############################################################################'
    public class BaseServer
    {
        public Guid NullID = new Guid(0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0);
        public  Guid User;
        public  string UserName;
        public Guid DepartmentID;
        public string DepartmentName;
        public DateTime SystemTime;

        string msTAB_BASEINFO = "S_Table";
        string msTAB_FIELDS = "S_TableField";
        string msTAB_RELATION = "S_TableRelation";
        Boolean mbInited;
        string mAppCode;
        string mMachineCode;
        CMethod mMethod;
        COMFieldManager mLDI;
        CRelations mRelations;
        CErrors mErr;
        DBConnection mCntMain;
        COperator mOperator;
        CProperties mProperties;
        CRight mRight;
        CBaseInfo mBaseInfo;
        CCodeRules mCodeRules;
        CPreferences mPreferences;

//'=============================================================================='
//'【公共属性】
//'=============================================================================='
//'------------------------------------------------------------------------------'
//'〖名称〗CBaseServer.AppCode()
//'〖说明〗使用该服务的应用程序编码
//'------------------------------------------------------------------------------'
        public string AppCode
        {
            get { return mAppCode; }
        }

//'------------------------------------------------------------------------------'
//'〖名称〗CBaseServer.cntMain()
//'〖说明〗数据库连接
//'------------------------------------------------------------------------------'
        public DBConnection cntMain
        {
            get { return mCntMain; }
        }

        //'------------------------------------------------------------------------------'
        //'〖名称〗CBaseServer.Err()
        //'〖说明〗错误处理对象
        //'------------------------------------------------------------------------------'
        //Public Property Get Err() As CErrors
        //    Set Err = mErr
        //End Property


//'------------------------------------------------------------------------------'
//'〖名称〗CBaseServer.Inited()
//'〖说明〗服务对象是否被成功地初始化
//'------------------------------------------------------------------------------'
        public Boolean Inited
        {
            get { return mbInited; }
        }

        //'------------------------------------------------------------------------------'
        //'〖名称〗CBaseServer.Operator()
        //'〖说明〗使用服务的应用程序所在工作站号
        //'------------------------------------------------------------------------------'
        //Public Property Get MachineCode() As String
        //    Let MachineCode = mMachineCode
        //End Property

        //'------------------------------------------------------------------------------'
        //'〖名称〗CBaseServer.Operator()
        //'〖说明〗当前操作员
        //'------------------------------------------------------------------------------'
       public COperator Operator
       {
           get { return mOperator; }
       }
             
     

//'------------------------------------------------------------------------------'
//'〖名称〗CBaseServer.Properties() 
//'〖说明〗基本属性——缺省
//'------------------------------------------------------------------------------'
        public CProperties Properties
        {
            get {return mProperties;}
            
        }

        public CRelations Relations
        {
            get { return mRelations; }
        }
        public CBaseInfo BaseInfo
        {
            get { return mBaseInfo; }
        }
        public CCodeRules CodeRules
        {
            get { return mCodeRules; }
        }
        public CPreferences Preferences
        {
            get { return mPreferences; }
        }
        //'------------------------------------------------------------------------------'
        //'〖属性〗CBaseServer.Right()
        //'〖说明〗服务权限
        //'------------------------------------------------------------------------------'
        public CRight Right
        {
            get {return mRight;}
        }

        //'------------------------------------------------------------------------------'
        //'〖名称〗BaseServer.Method()
        //'〖说明〗SP 对象
        //'------------------------------------------------------------------------------'
        //Public Property Get Method() As CMethod
        //    Set Method = mMethod
        //End Property


//'------------------------------------------------------------------------------'
//'〖名称〗BaseServer.LDI()
//'〖说明〗逻辑数据库接口
//'------------------------------------------------------------------------------'
        public COMFieldManager LDI
        {
            get { return mLDI; }
        }

        //'=============================================================================='
        //'【公共方法】
        //'=============================================================================='

        //Public Sub GYErrorBox(Optional ErrorID As Long = gnLOGIC_ERROR, Optional SaveToLog As Boolean = True)
        //' 本函数根据gErr中的错误定义，将错误显示出来


        //    Screen.MousePointer = vbDefault
        //    With frmErrorBox
        //        .txtDetail = mErr.GetAllErrorsDescription(False, SaveToLog)
        //        Select Case ErrorID

        //            Case gnLOGIC_ERROR
        //                ' 没有指明错误标识
        //                .txtDescription = "未知错误"
        //                .SetState 1   ' Set state to full.
        //            Case gnUSER_CANCEL
        //                .txtDescription = "用户取消操作"
        //                .SetState 1
        //                mErr.Clear False
        //            Case Else
        //                .txtDescription = Sys.Res.GetString(ErrorID)
        //                If InStr(1, .txtDescription, NotFindString, vbTextCompare) Then
        //                    .txtDescription = mErr.Pop.Description
        //                End If
        //                .SetState 0
        //        End Select
        //        mErr.Clear False
        //        .Show vbModal
        //    End With
        //End Sub


        //'〖名称〗CBaseServer.Initialize()
        //'〖说明〗初始化服务，由 CSystem.OpenServer() 方法调用
        //'〖参数〗[I]SvrInfo     : 打开数据库服务的必要信息
        //'　　　　[I]AppCode     : 打开数据库服务的应用程序
        //'　　　　[i]MachineCode : 应用程序所在工作站
        //'------------------------------------------------------------------------------'
        public Boolean Initialize(string SvrInfo, string AppCode, string MachineCode)
        {
            
            long lrtn;
            string sLogicErr;

            mAppCode = AppCode;
            mMachineCode = MachineCode;

            mPreferences = new CPreferences();
            mCntMain = new DBConnection(SvrInfo);

            mBaseInfo = new CBaseInfo(this);
            mCodeRules = new CCodeRules(this);
            
            //If mCntMain Is Nothing Then
            //    MsgBox "无法打开数据库", vbCritical
            //    GoTo BackOut
            //End If

            sLogicErr = "无法从数据库读取基本信息";
            mProperties = new CProperties();
            mProperties.Init(this,mCntMain, msTAB_BASEINFO);
            mProperties.ReadFromDB();

            sLogicErr = "无法装载关系表";
            mRelations = new CRelations();
            mRelations.Init(this, mCntMain, msTAB_RELATION);
            mRelations.ReadFromDB();

            sLogicErr = "无法逻辑字段接口对象";
            mLDI = new COMFieldManager() ;
            mLDI.Initialization(this,mCntMain,msTAB_FIELDS);

            sLogicErr = "无法取得系统权限";
            mRight = new CRight();
            mRight.Initialization(this, mCntMain);



            //Set mMethod = New CMethod
            //mMethod.Initialize Me, mCntMain


            //判断当前的应用程序的版本号和数据库中的版本号是否一致。
            //s = mProperties(gsBASEINFO_GROUPWARE_MARK)
            //If (VBA.Err.Number <> 0) Or (s <> gsGROUPWARE_MARK_DB) Then
            //    MsgBox Sys.Res.GetString(STR_INIT_ERR_INVALID_GROUPWARE_MARK), vbCritical
            //    GoTo BackOut
            //End If

            //s = 0
            //s = mProperties(gsBASEINFO_COMPATIBILITY_GROUP)
            //If (VBA.Err.Number <> 0) Or (Val(s) < gnCOMPATIBILITY_GROUP_DB) Then
            //    MsgBox Sys.Res.GetString(STR_INIT_ERR_DBVER_TOO_LOW), vbCritical
            //    GoTo BackOut
            //ElseIf Val(s) > gnCOMPATIBILITY_GROUP_DB Then
            //    MsgBox Sys.Res.GetString(STR_INIT_ERR_DBVER_TOO_HIGH), vbCritical
            //    GoTo BackOut
            //End If

            //sLogicErr = "无法创建错误处理对象"
            //Set mErr = New CErrors
            //mErr.Initialize Me

            sLogicErr = "无法创建操作员对象";
            mOperator = new  COperator();
            mOperator.Initialization(this,mCntMain);



            mbInited = true;
            return true;
        }

    }
}
