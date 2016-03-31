using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Transactions;
using Base;

namespace UI
{
    public partial class frmRole : Form
    {
        private bool _IsNew = false;
        private bool _IsEdit = false;
        private bool _HasUp = false;
        private bool _HasDown = false;

        private string RootIDKey = "99";
        private TreeNode RootNode;
         
        private DataSet _MainDataSet;
        private COMFields _MainTableDefine;
        private COMFields _DetailTableDefine;

        private Guid _MainID;
        private SortedList<string,Right> moRoleRights;

        public event DataTableEventHandler Changed;
        public event DataTableEventHandler PageUp;
        public event DataTableEventHandler PageDown;


        public string MainTable
        {
            get { return _MainTableDefine.OrinalTableName; }
        }

        public DataSet MainDataSet
        {
            get { return _MainDataSet; }
        }

        public frmRole()
        {
            InitializeComponent();
            moRoleRights = new SortedList<string, Right>();
        }

        public bool ShowData(COMFields mainTableDefine, DataSetEventArgs data, bool IsEdit, Form MDIParent)
        {
            _HasDown = data.HasNext;
            _HasUp = data.HasPrevious;
            _MainDataSet = data.Data;
            _MainTableDefine = mainTableDefine;
            _IsEdit = IsEdit;


            if (!InitData(data.Data))
            {
                return false;
            }
                
            this.MdiParent = MDIParent;
            //

            //Data Binding
            TxtName.DataBindings.Add("Text", _MainDataSet.Tables[MainTable],"Code", false, DataSourceUpdateMode.OnPropertyChanged);
            TxtDescription.DataBindings.Add("Text", _MainDataSet.Tables[MainTable], "Name", false, DataSourceUpdateMode.OnPropertyChanged);
            ChkBoxDisable.DataBindings.Add("Checked", _MainDataSet.Tables[MainTable], "Disable", false, DataSourceUpdateMode.OnPropertyChanged);
            if (_MainDataSet.Tables[MainTable].Rows[0]["Disable"] == DBNull.Value)
                _MainDataSet.Tables[MainTable].Rows[0]["Disable"] = 0;
            if (MDIParent == null)
                this.ShowDialog();
            else
            {
                this.Show();
            }
            return true;
        }


        private void frmRole_Load(object sender, EventArgs e)
        {
            CRightItem RootRight = new CRightItem(CSystem.Sys.Svr);

            RootRight = CSystem.Sys.Svr.Right[RootIDKey];

            RootNode = new TreeNode(RootRight.Name);
            RootNode.Tag = RootIDKey;
            tvRights.Nodes.Add(RootNode);
            LoadRights(RootIDKey, RootNode);
            RootNode.Expand();


        }

        private void LoadRights(string ParentRightKey,TreeNode ParentNode)
        {
           
            string PID; 
  
            foreach (CRightItem SubRight in CSystem.Sys.Svr.Right.Rights)
            {
               PID =(string) SubRight.PID;

               if (PID == ParentRightKey)
               {   
                   TreeNode ChildNode=new TreeNode();
                   ChildNode.Tag = SubRight.ID;
                   ChildNode.Text = SubRight.Name;
                   ParentNode.Nodes.Add(ChildNode);
                   //加载增改删核,以及扩展的权限,其中第4位为复核,需要明确定义且Key为Check,否则没有
                   if (SubRight.NeedRightControl)
                   {
                       addRightNote(1, "新增", ChildNode);
                       addRightNote(1<<1, "修改", ChildNode);
                       addRightNote(1<<2, "删除", ChildNode);
                       string exp = SubRight.RightExpression;
                       if (exp != null && exp.Length > 0)
                       {
                           string[] s = exp.Split(',');
                           int j = 0;//用于存放偏移,如果第一个不是复核,将从1开始
                           for (int i = 0; s.Length > i; i++)
                           {
                               string[] sb = s[i].Split('=');
                               if (sb.Length == 2)
                               {
                                   string key = sb[0].Trim().ToLower();
                                   string value = sb[1].Trim();
                                   //这个逻辑实际上是有问题的，会空出一个位，但是只能将错就错
                                   if (key != "check" && j==0)
                                       j = 1;
                                   addRightNote(1<<(3+i+j), value, ChildNode);
                               }
                           }
                       }
                   }
                   

                   if (moRoleRights.ContainsKey(SubRight.ID.ToString().Trim()))
                   {
                       int rightValue = moRoleRights[SubRight.ID.Trim()].RightValue;
                       ChildNode.Checked = true;
                       foreach (TreeNode node in ChildNode.Nodes)
                       {
                           if (node.Tag.GetType() == typeof(int))
                           {
                               int v = (int)node.Tag;
                               if ((v & rightValue) == v)
                                   node.Checked = true;
                           }
                           else
                               break;
                       }
                   }
               }
            }
        }

        private void addRightNote(int index, string name, TreeNode node)
        {
            TreeNode tnRight = new TreeNode();
            tnRight.Tag = index;
            tnRight.Text = name;
            node.Nodes.Add(tnRight);
        }

        private void tvRights_BeforeExpand(object sender, TreeViewCancelEventArgs e)
        {
           TreeNode PNode=e.Node;

           if (PNode.Nodes.Count > 0)
           {
               foreach (TreeNode CNode in PNode.Nodes)
                   if (CNode.Tag.GetType() == typeof(string))
                       LoadRights((string)CNode.Tag, CNode);
                   else
                       break;
           }
        }

        private void tvRights_AfterCheck(object sender, TreeViewEventArgs e)
        {
            TreeNode PNode = e.Node;

            if (PNode.Tag.GetType() == typeof(int))
                if (!PNode.Parent.Checked)
                    PNode.Parent.Checked = true;
            //if (PNode.Nodes.Count > 0)
            //{
            //    foreach (TreeNode CNode in PNode.Nodes)
            //        CNode.Checked = PNode.Checked;
            //}
        }

        private void toolClose_Click(object sender, EventArgs e)
        {
            if (_IsEdit)
            {
                switch (Msg.Question("正在修改数据，是否保存？", MessageBoxButtons.YesNoCancel))
                {
                    case DialogResult.Yes:
                        if (Save())
                        {
                            if (Changed != null)
                            {
                                Changed(this, new DataTableEventArgs(_MainDataSet.Tables[_MainTableDefine.OrinalTableName]));
                            }
                        }
                        else
                            return;
                        break;
                    case DialogResult.Cancel:
                        return;
                }
                _IsEdit = false;
            }
            this.Close();
        }

        private bool Save()
        {

            this.BindingContext[MainDataSet, _MainTableDefine.OrinalTableName].EndCurrentEdit();

            using (SqlConnection conn = new SqlConnection(CSystem.Sys.Svr.cntMain.ConnectionString))
            {
                SqlTransaction sqlTran = null;
                try
                {
                    conn.Open();
                    sqlTran = conn.BeginTransaction();

                    string ErrText = null;
 
                    DataSet d = CSystem.Sys.Svr.cntMain.Select(string.Format("Select count(*) from {2} where Code='{0}' and ID<>'{1}'", TxtName.Text, _MainID, MainTable), conn, sqlTran);
                    if ((int)d.Tables[0].Rows[0][0] > 0)
                        ErrText = ErrText + "代码已被使用，请修改代码后再保存！";

                    CSystem.Sys.Svr.cntMain.Update(_MainDataSet.Tables[_MainTableDefine.OrinalTableName], conn, sqlTran);
                    
                    //if (_IsNew==false)
                    CSystem.Sys.Svr.cntMain.Excute(String.Format("Delete from P_RoleRight where MainID ='{0}'",_MainID),conn,sqlTran);

                    DataSet DS = CSystem.Sys.Svr.cntMain.Select(string.Format("Select * From {0} where 1=0","P_RoleRight"),"P_RoleRight");
                    DataTable DT = DS.Tables["P_RoleRight"];

                    SavingRoleRight(DT, _MainID, RootNode);
                    CSystem.Sys.Svr.cntMain.Update(DT,conn,sqlTran);

                    sqlTran.Commit();
                    _IsNew = false;
                    _IsEdit = false;
                    return true;
                }
                catch (Exception ex)
                {
                    if (sqlTran != null)
                        sqlTran.Rollback();
                    Msg.Error(ex.ToString());
                }
                finally
                {
                    conn.Close();
                }
            }
            return false;
        }

        private int SavingRoleRight(DataTable RoleRightTable, Guid RoleID, TreeNode RightNode)
        {
            DataRow RoleRightItem = null;
            TreeNode CNode;
            if (RightNode.Checked == true && RightNode.Tag != null)
            {
                if (RightNode.Tag.GetType() == typeof(int))
                    return (int)RightNode.Tag;
                else
                {
                    RoleRightItem = RoleRightTable.NewRow();
                    RoleRightItem["ID"] = System.Guid.NewGuid();
                    RoleRightItem["MainID"] = RoleID;
                    RoleRightItem["RightID"] = (string)RightNode.Tag;
                    RoleRightItem["RightValue"] = 0;
                    RoleRightTable.Rows.Add(RoleRightItem);
                }
            }
            if (RightNode.Nodes.Count > 0)
            {
                int rightValue = 0;
                for (int i = 0; i < RightNode.Nodes.Count; i++)
                {
                    CNode = RightNode.Nodes[i];
                    rightValue += SavingRoleRight(RoleRightTable, RoleID, CNode);
                }
                if (RoleRightItem!=null)
                    RoleRightItem["RightValue"] = rightValue;
            }
            return 0;
                 
        }
        protected virtual bool InitData(DataSet ds)
        {
            if (ds.Tables[_MainTableDefine.OrinalTableName].Rows.Count == 0)
            {
                _IsNew = true;
                DataRow dr = ds.Tables[_MainTableDefine.OrinalTableName].NewRow();
                _MainID = System.Guid.NewGuid();
                dr["ID"] = _MainID;
                ds.Tables[_MainTableDefine.OrinalTableName].Rows.Add(dr);
            }
            else
            {
                _IsNew = false;
                _MainID = (Guid)ds.Tables[_MainTableDefine.OrinalTableName].Rows[0]["ID"];
                DataSet RoleRightsDS = CSystem.Sys.Svr.cntMain.Select(string.Format("Select ID,RightID,RightValue from P_RoleRight where MainID='{0}'", _MainID), "P_RoleRight");

                if (RoleRightsDS.Tables["P_RoleRight"].Rows.Count > 0)
                {
                    foreach (DataRow DR in RoleRightsDS.Tables["P_RoleRight"].Rows)
                        if (!moRoleRights.ContainsKey(DR["RightID"].ToString().Trim()))
                            moRoleRights.Add(DR["RightID"].ToString().Trim(), new Right((Guid)DR["ID"], DR["RightID"].ToString().Trim(), (int)DR["RightValue"]));
                }
            }
            return true;
        }

        private void toolSave_Click(object sender, EventArgs e)
        {
            if (Save())
            {
                DataTableEventArgs data = new DataTableEventArgs(_MainDataSet.Tables[_MainTableDefine.OrinalTableName]);
                Changed(this, data);
                this.Close();
            }
                
        }

        private class Right
        {
            public Guid ID;
            public string RightID;
            public int RightValue;
            public Right(Guid id, string rightID, int rightValue)
            {
                ID = id;
                RightID = rightID;
                RightValue = rightValue;
            }
        }

    }
}