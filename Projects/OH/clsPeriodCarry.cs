using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using Base;
using UI;

namespace OH
{
    public class clsPeriodCarry
    {
        public bool CloseWarehouse(DateTime currDate,DateTime nextDate, SqlConnection conn, SqlTransaction tran)
        {
            try
            {
                string sql = "";
                sql = "Select 1 InOut,A.WarehouseID,B.Name WarehouseName,A.Type,Count(*) C from D_WarehouseIn A,P_Warehouse B where A.WarehouseID=B.ID and A.CheckStatus=0 and A.BillDate<'" + nextDate + "'  group by A.WarehouseID,A.Type,B.Name " +
                      "Union all select 2 InOut,A.WarehouseID,B.Name WarehouseName,A.Type,Count(*) C from D_WarehouseOut A,P_Warehouse B where A.WarehouseID=B.ID and A.CheckStatus=0 and A.BillDate<'" + nextDate + "'  group by A.WarehouseID,A.Type,B.Name";
                DataSet dsCheck = CSystem.Sys.Svr.cntMain.Select(sql, conn, tran);
                if (dsCheck.Tables[0].Rows.Count> 0)
                {
                    string msg = "";
                    foreach (DataRow dr in dsCheck.Tables[0].Rows)
                    {
                        string t = "";
                        //public enum enuWarehouseIn : int { In = 0, BuyIn = 1, MoveIn = 2, RepairIn = 3};
                        if ((int)dr["InOut"] == 1)
                        {
                            switch ((int)dr["Type"])
                            {
                                case 0:
                                    t = "普通入库单";
                                    break;
                                case 1:
                                    t = "采购入库单";
                                    break;
                                case 2:
                                    t = "调拨入库单";
                                    break;
                                case 3:
                                    t = "返修入库单";
                                    break;
                            }
                        }
                        else
                        {
                            //public enum enuWarehouseOut : int { Out = 0, ProduceOut = 1, MoveOut = 2, ScrapOut = 3, CheckOut = 4, ReturnOut = 5, RepairOut = 6 };
                            switch ((int)dr["Type"])
                            {
                                case 0:
                                    t = "普通出库单";
                                    break;
                                case 1:
                                    t = "生产领料单";
                                    break;
                                case 2:
                                    t = "调拨出库单";
                                    break;
                                case 3:
                                    t = "废品出库单";
                                    break;
                                case 4:
                                    t = "盘亏单";
                                    break;
                                case 5:
                                    t = "退货单";
                                    break;
                                case 6:
                                    t = "返修出库单";
                                    break;
                            }
                        }
                        msg += string.Format("仓库({0})的{1}存在{2}条单据没有复核\r\n", dr["WarehouseName"], t, dr["C"]);
                    }
                    Msg.Error(msg);
                    return false;
                }
                //检测工票有没有复核
                sql = string.Format("Select Count(*) from D_WorkOrder where Date<'{0}' and CheckStatus=0", nextDate);
                dsCheck = CSystem.Sys.Svr.cntMain.Select(sql, conn, tran);
                if ((int)dsCheck.Tables[0].Rows[0][0] > 0)
                {
                    Msg.Error("存在没有复核的工票!");
                    return false;
                }
                //检测移库是否全部完成
                sql = string.Format("select B.Name SourceName,C.Name TargetName,count(*) C from d_warehouseout A,P_Warehouse B,P_Warehouse C where A.WarehouseID=B.ID and A.TargetWarehouseID=C.ID and A.MoveStatus=0 and A.Type=2 and A.BillDate<'{0}' group by B.Name,C.Name", nextDate);
                dsCheck = CSystem.Sys.Svr.cntMain.Select(sql, conn, tran);
                if (dsCheck.Tables[0].Rows.Count > 0)
                {
                    string msg = "";
                    foreach (DataRow dr in dsCheck.Tables[0].Rows)
                    {
                        msg += string.Format("源仓库({0}),目标仓库({1})存在{2}条移库单没有完成\r\n", dr["SourceName"], dr["TargetName"], dr["C"]);
                    }
                    if (Msg.Question(msg+"请确认，是否继续月结？")!= System.Windows.Forms.DialogResult.Yes)
                        return false;
                    sql = string.Format("Update D_warehouseout set BillDate='{0}' WorkCode=WorkCode+'{1:MM}月未达' where MoveStatus=0 and Type=2 and BillDate<'{0}'", nextDate, currDate);
                    CSystem.Sys.Svr.cntMain.Excute(sql, conn, tran);
                }
                //根据入库单用SQL插入空记录
                sql = string.Format("insert into D_WarehouseBalance(ID,CheckDate,WarehouseID,PositionID,MaterialID,IngredientID,MachiningStandardID,DesignCodeID,ProductStandardID,Length) " +
                            "select newid() ID,'{0}' CheckDate,B.* from " +
                            "(select A.WarehouseID,B.PositionID,B.MaterialID,B.IngredientID,B.MachiningStandardID,B.DesignCodeID,B.ProductStandardID,B.Length " +
                            "from D_WarehouseIn A,D_WarehouseInBill B where A.ID=B.MainID and A.BillDate>='{0}' and A.BillDate<'{1}' " +
                            "group by A.WarehouseID,B.PositionID,B.MaterialID,B.IngredientID,B.MachiningStandardID,B.DesignCodeID,B.ProductStandardID,B.Length) B " +
                            " where not exists(Select C.ID from D_WarehouseBalance C where B.WarehouseID=C.WarehouseID and B.PositionID=C.PositionID and B.MaterialID=C.MaterialID and (B.IngredientID=C.IngredientID or (B.IngredientID is null and C.IngredientID is null)) and (B.MachiningStandardID = C.MachiningStandardID or (B.MachiningStandardID is null and C.MachiningStandardID is null)) and (B.DesignCodeID =C.DesignCodeID or (B.DesignCodeID is null and C.DesignCodeID is null)) and (B.ProductStandardID=C.ProductStandardID or (B.ProductStandardID is null and C.ProductStandardID is null)) and isnull(B.Length,0)=isnull(C.Length,0)) ",
                        currDate, nextDate);
                CSystem.Sys.Svr.cntMain.Excute(sql, conn, tran);
                //根据出库单用SQL插入空记录
                sql = string.Format("insert into D_WarehouseBalance(ID,CheckDate,WarehouseID,PositionID,MaterialID,IngredientID,MachiningStandardID,DesignCodeID,ProductStandardID,Length) " +
                            "select newid() ID,'{0}' CheckDate,B.* from " +
                            "(select A.WarehouseID,B.PositionID,B.MaterialID,B.IngredientID,B.MachiningStandardID,B.DesignCodeID,B.ProductStandardID,B.Length " +
                            "from D_WarehouseOut A,D_WarehouseOutBill B where A.ID=B.MainID and A.BillDate>='{0}' and A.BillDate<'{1}' " +
                            "group by A.WarehouseID,B.PositionID,B.MaterialID,B.IngredientID,B.MachiningStandardID,B.DesignCodeID,B.ProductStandardID,B.Length) B " +
                            " where not exists(Select C.ID from D_WarehouseBalance C where B.WarehouseID=C.WarehouseID and B.PositionID=C.PositionID and B.MaterialID=C.MaterialID and (B.IngredientID=C.IngredientID or (B.IngredientID is null and C.IngredientID is null)) and (B.MachiningStandardID = C.MachiningStandardID or (B.MachiningStandardID is null and C.MachiningStandardID is null)) and (B.DesignCodeID =C.DesignCodeID or (B.DesignCodeID is null and C.DesignCodeID is null)) and (B.ProductStandardID=C.ProductStandardID or (B.ProductStandardID is null and C.ProductStandardID is null)) and isnull(B.Length,0)=isnull(C.Length,0)) ",
                        currDate, nextDate);
                CSystem.Sys.Svr.cntMain.Excute(sql, conn, tran);
                //根据入库单更新Balance
                for (int i = 0; i < 4; i++)
                {
                    sql = string.Format("update C set C.InQuantity1_{0}=B.Quantity1,C.InQuantity2_{0}=B.Quantity2,C.InQuantity1=C.InQuantity1+B.Quantity1,C.InQuantity2=C.InQuantity2+B.Quantity2,C.InMoney_{0}=B.Money,C.InMoney=C.InMoney+B.Money from D_WarehouseBalance C," +
                            "(select A.WarehouseID,B.PositionID,B.MaterialID,B.IngredientID,B.MachiningStandardID,B.DesignCodeID,B.ProductStandardID,B.Length,sum(B.Quantity1) Quantity1,sum(B.Quantity2) Quantity2,sum(B.Money) Money " +
                            "from D_WarehouseIn A,D_WarehouseInBill B where A.ID=B.MainID and A.Status=0 and A.Type={0} and A.BillDate>='{1}' and A.BillDate<'{2}' " +
                            "group by A.WarehouseID,B.PositionID,B.MaterialID,B.IngredientID,B.MachiningStandardID,B.DesignCodeID,B.ProductStandardID,B.Length) B " +
                            "where C.CheckDate='{1}' and B.WarehouseID=C.WarehouseID and B.PositionID=C.PositionID and B.MaterialID=C.MaterialID and (B.IngredientID=C.IngredientID or (B.IngredientID is null and C.IngredientID is null)) and (B.MachiningStandardID = C.MachiningStandardID or (B.MachiningStandardID is null and C.MachiningStandardID is null)) and (B.DesignCodeID =C.DesignCodeID or (B.DesignCodeID is null and C.DesignCodeID is null)) and (B.ProductStandardID=C.ProductStandardID or (B.ProductStandardID is null and C.ProductStandardID is null)) and isnull(B.Length,0)=isnull(C.Length,0)",
                        i, currDate, nextDate);
                    CSystem.Sys.Svr.cntMain.Excute(sql, conn, tran);
                }                //更新入库单的计算标志
                sql = string.Format("Update D_WarehouseIn set Status=1 where BillDate>='{0}' and BillDate<'{1}' and Status=0", currDate, nextDate);
                CSystem.Sys.Svr.cntMain.Excute(sql, conn, tran);
                //根据出库单更新Balance
                for (int i = 0; i < 7; i++)
                {
                    sql = string.Format("update C set C.OutQuantity1_{0}=B.Quantity1,C.OutQuantity2_{0}=B.Quantity2,C.OutQuantity1=C.OutQuantity1+B.Quantity1,C.OutQuantity2=C.OutQuantity2+B.Quantity2,C.OutMoney_{0}=B.Money,C.OutMoney=C.OutMoney+B.Money from D_WarehouseBalance C," +
                        "(select A.WarehouseID,B.PositionID,B.MaterialID,B.IngredientID,B.MachiningStandardID,B.DesignCodeID,B.ProductStandardID,B.Length,sum(B.Quantity1) Quantity1,sum(B.Quantity2) Quantity2,sum(B.Money) Money " +
                        "from D_WarehouseOut A,D_WarehouseOutBill B where A.ID=B.MainID and A.Status=0 and A.Type={0} and A.BillDate>='{1}' and A.BillDate<'{2}' " +
                        "group by A.WarehouseID,B.PositionID,B.MaterialID,B.IngredientID,B.MachiningStandardID,B.DesignCodeID,B.ProductStandardID,B.Length) B " +
                        "where C.CheckDate='{1}' and B.WarehouseID=C.WarehouseID and B.PositionID=C.PositionID and B.MaterialID=C.MaterialID and (B.IngredientID=C.IngredientID or (B.IngredientID is null and C.IngredientID is null)) and (B.MachiningStandardID = C.MachiningStandardID or (B.MachiningStandardID is null and C.MachiningStandardID is null)) and (B.DesignCodeID =C.DesignCodeID or (B.DesignCodeID is null and C.DesignCodeID is null)) and (B.ProductStandardID=C.ProductStandardID or (B.ProductStandardID is null and C.ProductStandardID is null)) and isnull(B.Length,0)=isnull(C.Length,0)",
                    i, currDate, nextDate);
                    CSystem.Sys.Svr.cntMain.Excute(sql, conn, tran);
                }                //更新入库单的计算标志
                sql = string.Format("Update D_WarehouseOut set Status=1 where BillDate>='{0}' and BillDate<'{1}' and Status=0", currDate, nextDate);
                CSystem.Sys.Svr.cntMain.Excute(sql, conn, tran);
                //更新余额字段
                sql = string.Format("Update D_WarehouseBalance set EndQuantity1=BeginQuantity1+InQuantity1-OutQuantity1,EndQuantity2=BeginQuantity2+InQuantity2-OutQuantity2,EndMoney=BeginMoney+InMoney-OutMoney where CheckDate='{0}'", currDate);
                CSystem.Sys.Svr.cntMain.Excute(sql, conn, tran);
                //生成次月的余额表
                sql = string.Format("insert into D_WarehouseBalance(ID,CheckDate,WarehouseID,PositionID,MaterialID,IngredientID,MachiningStandardID,DesignCodeID,ProductStandardID,Length,BeginQuantity1,BeginQuantity2,BeginMoney) " +
                            "select newid() ID,'{0}' CheckDate,WarehouseID,PositionID,MaterialID,IngredientID,MachiningStandardID,DesignCodeID,ProductStandardID,Length,EndQuantity1 BeginQuantity1,EndQuantity2 BeginQuantity2,EndMoney BeginMoney " +
                            "from D_WarehouseBalance where CheckDate='{1}'",
                        nextDate, currDate);
                CSystem.Sys.Svr.cntMain.Excute(sql, conn, tran);
                return true;
            }
            catch (Exception ex)
            {
                Msg.Error(ex.ToString());
            }

            return false;
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
                                "select newid() ID,'{0}' Period,ID SourceID,MaterialCheckTypeID,MainID SourceMainID,LineNumber,ProcessID,MachineID,MaterialCheckObjectID,Quantity,CheckObjectMeasureID,Output,OutMeasureID,Reward,Punish " +
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
                    sql = string.Format("select C.MaterialCheckObjectID,C.MachineID,(case when C.SaveQty>0 then C.SaveQty*C.Reward else 0 end) Reward,(case when C.SaveQty<0 then C.SaveQty*C.Punish else 0 end) Punish from " +
                            "(select A.Qty FactQty,B.Qty FactOutput,(C.Quantity*B.Qty)/C.Output-A.Qty SaveQty,C.* from " +
                            "(select B.MachineID,C.MaterialCheckObjectID,D.IsCheckAmount,(case D.IsCheckAmount when 0 then Sum(B.Quantity1) else Sum(B.Money) end) Qty from D_WarehouseOut A,D_WarehouseOutBill B,P_Material C,P_MaterialCheckObject D where A.ID=B.MainID and B.MaterialID=C.ID and C.MaterialType=4 and C.MaterialCheckObjectID=D.ID " +
                                "and A.BillDate>='{0}' and A.BillDate<'{1}' " +
                                "group by B.MachineID,C.MaterialCheckObjectID,D.IsCheckAmount) A," +
                            "(select A.MachineID,Sum(B.Quantity) Qty from D_WorkOrder A,D_WorkOrderProduct B where A.ID=B.MainID " +
                                "and A.Date>='{0}' and A.Date<'{1}' " +
                                "group by A.MachineID) B," +
                            "(Select * from C_CheckStandardBill where MainID='{2}')C where A.MachineID=C.MachineID " +
                            "and A.MaterialCheckObjectID=C.MaterialCheckObjectID " +
                            "and B.MachineID=C.MachineID) C ", currDate, nextDate, id);
                    ds = CSystem.Sys.Svr.cntMain.Select(sql, "Data", conn, tran);
                    CSystem.Sys.Svr.cntMain.Select("Select * from C_MachineOperator", "C_MachineOperator", ds, conn, tran);
                    CSystem.Sys.Svr.cntMain.Select("Select * from D_OperatorRewardPunish where 1=0", "D_OperatorRewardPunish", ds, conn, tran);
                    foreach (DataRow dr in ds.Tables["Data"].Rows)
                    {
                        DataRow[] drs = ds.Tables["C_MachineOperator"].Select("MachineID='" + dr["MachineID"] + "'");
                        if (drs.Length == 0)
                            continue;
                        decimal Reward = 0;
                        decimal Punish = 0;
                        decimal rates = 0;
                        for (int i = 0; i < drs.Length; i++)
                        {
                            rates += (decimal)drs[i]["Rate"];
                        }
                        if (rates == 0)
                            continue;
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
                    CSystem.Sys.Svr.cntMain.Update(ds.Tables["D_OperatorRewardPunish"]);
                    return true;
                }
                catch (Exception e)
                {
                    Msg.Error(e.ToString());
                    return false;
                }
            }
        }

        internal bool ReturnWarehouse(DateTime currDate, DateTime lastDate, SqlConnection conn, SqlTransaction tran)
        {
            try
            {
                //删除当前会计期的余额表数据
                string sql = string.Format("Delete from D_WarehouseBalance where CheckDate='{0}'", currDate);
                CSystem.Sys.Svr.cntMain.Excute(sql, conn, tran);
                //清除上月的余额表除期初外的数据
                sql = string.Format("update D_WarehouseBalance set InQuantity1_0=0,InQuantity1_1=0,InQuantity1_2=0,InQuantity1_3=0," +
                                                                     "InQuantity2_0=0,InQuantity2_1=0,InQuantity2_2=0,InQuantity2_3=0," +
                                                                     "InMoney_0=0,InMoney_1=0,InMoney_2=0,InMoney_3=0," +
                                                                     "InQuantity1=0,InQuantity2=0,InMoney=0, " +
                                                                     "OutQuantity1_0=0,OutQuantity1_1=0,OutQuantity1_2=0,OutQuantity1_3=0,OutQuantity1_4=0,OutQuantity1_5=0," +
                                                                     "OutQuantity2_0=0,OutQuantity2_1=0,OutQuantity2_2=0,OutQuantity2_3=0,OutQuantity2_4=0,OutQuantity2_5=0," +
                                                                     "OutMoney_0=0,OutMoney_1=0,OutMoney_2=0,OutMoney_3=0,OutMoney_4=0,OutMoney_5=0," +
                                                                     "OutQuantity1=0,OutQuantity2=0,OutMoney=0, " +
                                                                     "EndQuantity1=0,EndQuantity2=0,EndMoney=0 " +
                                                    "where CheckDate='{0}'", lastDate);
                CSystem.Sys.Svr.cntMain.Excute(sql, conn, tran);
                //更新入库单的计算标志
                sql = string.Format("Update D_WarehouseIn set Status=0 where BillDate>='{0}' and BillDate<'{1}' and Status=1", lastDate, currDate);
                CSystem.Sys.Svr.cntMain.Excute(sql, conn, tran);
                //更新出库单的计算标志
                sql = string.Format("Update D_WarehouseOut set Status=0 where BillDate>='{0}' and BillDate<'{1}' and Status=1", lastDate, currDate);
                CSystem.Sys.Svr.cntMain.Excute(sql, conn, tran);
                return true;
            }
            catch(Exception e)
            {
                return false;
            }
        }
    }
}
