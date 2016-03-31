using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Base;
using System.Data.SqlClient;
using UI;

namespace OH
{
    public partial class frmCalculateMaterialCheck : Form,IMenuAction
    {
        public frmCalculateMaterialCheck()
        {
            InitializeComponent();
        }

        private void butCalc_Click(object sender, EventArgs e)
        {
            using (SqlConnection conn = new SqlConnection(CSystem.Sys.Svr.cntMain.ConnectionString))
            {
                txtResult.Text = "";
                butCalc.Enabled = false;
                butCancel.Enabled = false;
                SqlTransaction tran = null;
                try
                {
                    conn.Open();
                    tran = conn.BeginTransaction();
                    DataSet ds = CSystem.Sys.Svr.cntMain.Select(string.Format("Select * from S_BaseInfo where ID='{0}'", @"系统\系统状态"), conn, tran);
                    if (ds.Tables[0].Rows.Count != 1)
                    {
                        Msg.Error("基本信息表数据丢失!");
                        return;
                    } int systemStatus = int.Parse((string)ds.Tables[0].Rows[0]["Value"]);
                    DateTime currDate;
                    if (systemStatus == 0)
                    {
                        currDate = DateTime.Now;
                        currDate = new DateTime(currDate.Year, currDate.Month, 1);
                    }
                    else
                    {
                        ds = CSystem.Sys.Svr.cntMain.Select(string.Format("Select * from S_BaseInfo where ID='{0}'", @"系统\当前会计期"), conn, tran);
                        if (ds.Tables[0].Rows.Count != 1)
                        {
                            Msg.Error("基本信息表数据丢失!");
                            return;
                        }
                        currDate = DateTime.Parse((string)ds.Tables[0].Rows[0]["Value"]);
                    }
                    DateTime nextDate = currDate.AddMonths(1);

                    //先清数据
                    CSystem.Sys.Svr.cntMain.Excute(string.Format("Delete from C_CheckStandardBillOld where Period='{0}'", currDate), conn, tran);
                    CSystem.Sys.Svr.cntMain.Excute(string.Format("Delete from D_OperatorRewardPunish where Period='{0}'", currDate), conn, tran);


                    bool Rtn = CloseCheckStandard(currDate, nextDate, conn, tran);
                    //计算出各人员的奖金
                    if (Rtn)
                        Rtn = ComputeReward(currDate, nextDate, conn, tran);
                    if (Rtn)
                        tran.Commit();
                    else
                    {
                        tran.Rollback();
                        Msg.Warning("计算失败！");
                        return;
                    }
                }
                catch (Exception ex)
                {
                    tran.Rollback();
                    Msg.Error(ex.ToString());
                    return;
                }
                finally
                {
                    butCalc.Enabled = true;
                    butCancel.Enabled = true;
                }
            }
            lableNotes.Text = "物料考核计算完成！";
            Msg.Information("物料考核计算完成！");
            this.Close();
        }
        public bool CloseCheckStandard(DateTime currDate, DateTime nextDate, SqlConnection conn, SqlTransaction tran)
        {
            string sql;
            sql = @"Select * from S_BaseInfo where ID='物料考核\考核版本'";
            DataSet ds = CSystem.Sys.Svr.cntMain.Select(sql, "S_BaseInfo", conn, tran);
            if (ds.Tables[0].Rows.Count != 1)
            {
                Msg.Error("读取当前的考核版本失败！");
                return false;
            }
            else
            {
                try
                {
                    Guid id = new Guid(ds.Tables[0].Rows[0]["Value"] as string);
                    sql = string.Format("Insert into C_CheckStandardBillOld(ID,Period,MaterialCheckTypeID,SourceID,SourceMainID,LineNumber,ProcessID,MachineID,MaterialCheckObjectID,Quantity,CheckObjectMeasureID,Output,OutMeasureID,Reward,Punish) " +
                                "select newid() ID,'{0}' Period,MaterialCheckTypeID,ID SourceID,MainID SourceMainID,LineNumber,ProcessID,MachineID,MaterialCheckObjectID,Quantity,CheckObjectMeasureID,Output,OutMeasureID,Reward,Punish " +
                                "from C_CheckStandardBill where MainID='{1}'", currDate, id);
                    CSystem.Sys.Svr.cntMain.Excute(sql, conn, tran);
                    return true;
                }
                catch (Exception e)
                {
                    Msg.Error(e.ToString());
                    return false;
                }
            }
        }
        public bool ComputeReward(DateTime currDate, DateTime nextDate, SqlConnection conn, SqlTransaction tran)
        {
            string sql;
            sql = @"Select * from S_BaseInfo where ID='物料考核\考核版本'";
            DataSet ds = CSystem.Sys.Svr.cntMain.Select(sql, "S_BaseInfo", conn, tran);
            if (ds.Tables[0].Rows.Count != 1)
            {
                Msg.Error("读取当前的考核版本失败！");
                return false;
            }
            else
            {
                try
                {
                    Guid id = new Guid(ds.Tables[0].Rows[0]["Value"] as string);
                    sql = string.Format("select C.MaterialCheckObjectID,C.MachineID,P.Name MachineName,(case when C.SaveQty>0 then C.SaveQty*C.Reward else 0 end) Reward,(case when C.SaveQty<0 then C.SaveQty*C.Punish else 0 end) Punish from " +
                            "(select isnull(A.Qty,0) FactQty,isnull(sum(B.Qty),0) FactOutput,(A.Quantity*isnull(sum(B.Qty),0))/A.Output-isnull(A.Qty,0) SaveQty,A.MachineID,A.CheckObjectMeasureID,A.OutMeasureID,A.MaterialCheckObjectID,A.Output,A.Quantity,A.Reward,A.Punish from " +
                        "(select C.MachineID,C.CheckObjectMeasureID,C.OutMeasureID,C.MaterialCheckObjectID,C.Output,C.Quantity,C.Reward,C.Punish,D.IsCheckAmount,D.CheckCountType,(case D.IsCheckAmount when 0 then (case D.CheckType when 0 then Sum(WO.Quantity1) else  Sum(WO.Quantity2) end ) else Sum(WO.Money) end) Qty from " +
                                "(Select * from C_CheckStandardBill where MainID='{2}') C left join "+
                                "(Select distinct A.ID,A.CheckType,A.IsCheckAmount,1 CheckCountType from P_MaterialCheckObject A " +
                                        "where exists(select id from P_MaterialCheckObjectCraft B where A.ID=B.MainID) " +
                                    "union Select distinct A.ID,A.CheckType,A.IsCheckAmount,2 CheckCountType from P_MaterialCheckObject A  " +
                                        "where exists(select id from P_MaterialCheckObjectAddition B where A.ID=B.MainID) " +
                                            "and not exists(select id from P_MaterialCheckObjectCraft B where A.ID=B.MainID) " +
                                    "union Select distinct A.ID,A.CheckType,A.IsCheckAmount,0 CheckCountType from P_MaterialCheckObject A  " +
                                        "where not exists(select id from P_MaterialCheckObjectCraft B where A.ID=B.MainID) " +
                                            "and not exists(select id from P_MaterialCheckObjectAddition B where A.ID=B.MainID)) D on C.MaterialCheckObjectID=D.ID left join " +
                            "(Select B.*,C.MaterialCheckObjectID from D_WarehouseOut A,D_WarehouseOutBill B,P_Material C where A.ID=B.MainID and B.MaterialID=C.ID and C.MaterialType=4  " +
                                    "and A.BillDate>='{0}' and A.BillDate<'{1}') WO  on WO.MachineID=C.MachineID and WO.MaterialCheckObjectID=D.ID " +
                        "group by C.MachineID,C.CheckObjectMeasureID,C.OutMeasureID,C.MaterialCheckObjectID,C.Output,C.Quantity,C.Reward,C.Punish,D.IsCheckAmount,D.CheckType,D.CheckCountType) A " +
                        "left join " +
                                "(select A.MachineID,B.CraftID,B.AdditionID,Sum(B.Quantity) Qty from D_WorkOrder A,D_WorkOrderProduct B where A.ID=B.MainID " +
                                    "and A.Date>='{0}' and A.Date<'{1}' " +
                                    "group by A.MachineID,B.CraftID,B.AdditionID) B on B.MachineID=A.MachineID " +
                        " left join P_MaterialCheckObjectCraft E on A.MaterialCheckObjectID=E.MainID and B.CraftID=E.CraftID left join P_MaterialCheckObjectAddition F on A.MaterialCheckObjectID=F.MainID and F.AdditionID=B.AdditionID " +
                        "where (A.CheckCountType=0 or A.CheckCountType is null or (A.CheckCountType=1 and not E.ID is null) or (A.CheckCountType=2 and not F.ID is null)) " +
                        "Group by A.MachineID,A.CheckObjectMeasureID,A.OutMeasureID,A.MaterialCheckObjectID,A.Output,A.Quantity,A.Reward,A.Punish,isnull(A.Qty,0)" +
                            ") C inner join P_Machine P on C.MachineID=P.ID ", currDate, nextDate, id);
                    ds = CSystem.Sys.Svr.cntMain.Select(sql, "Data", conn, tran);
                    CSystem.Sys.Svr.cntMain.Select("Select * from C_MachineOperator", "C_MachineOperator", ds, conn, tran);
                    CSystem.Sys.Svr.cntMain.Select("Select * from D_OperatorRewardPunish where 1=0", "D_OperatorRewardPunish", ds, conn, tran);
                    foreach (DataRow dr in ds.Tables["Data"].Rows)
                    {
                        //if (dr["StationID"] != DBNull.Value)
                        //{
                            //查询相关工票中指定设备,指定岗位的人员情况
                            //DataSet dsOperator = CSystem.Sys.Svr.cntMain.Select(string.Format("Select OperatorID from D_WorkOrderOperator A,D_WorkOrder B where A.MainID=B.ID and A.MachineID='{0}' and B.StationID='{1}'", dr["MachineID"], dr["StationID"]), conn, tran);

                        //}
                        DataRow[] drs = ds.Tables["C_MachineOperator"].Select("MachineID='" + dr["MachineID"] + "'");
                        if (drs.Length == 0)
                        {
                            txtResult.AppendText(dr["MachineName"]+" 没有定义操作工分配比例！\r\n");
                            continue;
                        }
                        decimal Reward = 0;
                        decimal Punish = 0;
                        decimal rates = 0;
                        for (int i = 0; i < drs.Length; i++)
                        {
                            rates += (decimal)drs[i]["Rate"];
                        }
                        if (rates == 0)
                        {
                            txtResult.AppendText(dr["MachineName"] + " 定义的操作工分配比例总和为０！\r\n");
                            continue;
                        }
                        DataRow firstRow = null;
                        for (int i = 0; i < drs.Length; i++)
                        {
                            DataRow newRow = ds.Tables["D_OperatorRewardPunish"].NewRow();
                            ds.Tables["D_OperatorRewardPunish"].Rows.Add(newRow);
                            if (firstRow == null)
                                firstRow = newRow;
                            newRow["ID"] = Guid.NewGuid();
                            newRow["Period"] = currDate;
                            newRow["OperatorID"] = drs[i]["OperatorID"];
                            newRow["MachineID"] = drs[i]["MachineID"];
                            newRow["MaterialCheckObjectID"] = dr["MaterialCheckObjectID"];
                            newRow["Reward"] = (decimal)dr["Reward"] * (decimal)drs[i]["Rate"] / rates;
                            Reward += (decimal)newRow["Reward"];
                            newRow["Punish"] = (decimal)dr["Punish"] * (decimal)drs[i]["Rate"] / rates;
                            Punish += (decimal)newRow["Punish"];
                        }
                        if (Reward != (decimal)dr["Reward"])
                            firstRow["Reward"] = (decimal)firstRow["Reward"] + ((decimal)dr["Reward"] - Reward);
                        if (Punish != (decimal)dr["Punish"])
                            firstRow["Punish"] = (decimal)firstRow["Punish"] + ((decimal)dr["Punish"] - Punish);
                    }
                    CSystem.Sys.Svr.cntMain.Update(ds.Tables["D_OperatorRewardPunish"], conn, tran);
                    return true;
                }
                catch (Exception e)
                {
                    Msg.Error(e.ToString());
                    return false;
                }
            }
        }

        private void frmCalculateMaterialCheck_Load(object sender, EventArgs e)
        {
            using (SqlConnection conn = new SqlConnection(CSystem.Sys.Svr.cntMain.ConnectionString))
            {
                SqlTransaction tran = null;
                try
                {
                    conn.Open();
                    tran = conn.BeginTransaction();
                    DataSet ds = CSystem.Sys.Svr.cntMain.Select(string.Format("Select * from S_BaseInfo where ID='{0}'", @"系统\系统状态"), conn, tran);
                    if (ds.Tables[0].Rows.Count != 1)
                    {
                        lableNotes.Text = "基本信息表数据丢失!";
                        return;
                    } int systemStatus = int.Parse((string)ds.Tables[0].Rows[0]["Value"]);
                    DateTime currDate;
                    if (systemStatus == 0)
                    {
                        currDate = DateTime.Now;
                        currDate = new DateTime(currDate.Year, currDate.Month, 1);
                    }
                    else
                    {
                        ds = CSystem.Sys.Svr.cntMain.Select(string.Format("Select * from S_BaseInfo where ID='{0}'", @"系统\当前会计期"), conn, tran);
                        if (ds.Tables[0].Rows.Count != 1)
                        {
                            lableNotes.Text = "基本信息表数据丢失!";
                            return;
                        }
                        currDate = DateTime.Parse((string)ds.Tables[0].Rows[0]["Value"]);
                    }
                    DateTime nextDate = currDate.AddMonths(1);

                    ds = CSystem.Sys.Svr.cntMain.Select(string.Format("select 1 where exists( select ID from D_OperatorRewardPunish where Period='{0}')", currDate), conn, tran);
                    if (ds.Tables[0].Rows.Count == 1)
                    {
                        lableNotes.Text = "您已进行物料考核计算，但仍可以重新计算！";
                    }
                    else
                        lableNotes.Text = "您还没有进行物料考核计算！";
                }
                catch (Exception ex)
                {
                    Msg.Warning(ex.ToString());
                }
            }
         }

         #region IMenuAction Members

         public Form GetForm(CRightItem right, Form mdiForm)
         {
             this.ShowDialog();
             return null;
         }

         #endregion
     }
}