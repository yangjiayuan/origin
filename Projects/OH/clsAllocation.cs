using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using Base;
using UI;

namespace OH
{
    public class clsAllocation:ToolDetailForm
    {
        public override bool AllowDeleteRowInToolBar
        {
            get
            {
                return true;
            }
        }

        public override bool AllowUpdateInGrid(string TableName)
        {
            return false;
        }
        public override bool AllowInsertRowInGrid(string TableName)
        {
            return false;
        }
        public override bool AutoCode
        {
            get
            {
                return true;
            }
        }
        public override bool NewData(System.Data.DataSet ds, Base.COMFields mainTableDefine, List<Base.COMFields> detailTableDefine)
        {
            frmAllocation frm = new frmAllocation();
            DataSet dsAllocation = frm.GetData();
            if (dsAllocation == null)
                return false;
            else
            {
                ds.Merge(dsAllocation);
                ds.Tables[mainTableDefine.OrinalTableName].Rows[0]["WarehouseID"] = frm.WarehouseID;
                ds.Tables[mainTableDefine.OrinalTableName].Rows[0]["WarehouseName"] = frm.WarehouseName;
                ds.Tables[mainTableDefine.OrinalTableName].Rows[0]["PackagePlanID"] = frm.PackagePlanID;
                ds.Tables[mainTableDefine.OrinalTableName].Rows[0]["PackageCode"] = frm.PackagePlanCode;
                ds.Tables[mainTableDefine.OrinalTableName].Rows[0]["ConsignDate"] = frm.ConsignDate;
                ds.Tables[mainTableDefine.OrinalTableName].Rows[0]["CustomerName"] = frm.CustomerName;
            }
            return true;
        }
        public override bool Check(Guid ID, System.Data.SqlClient.SqlConnection conn, System.Data.SqlClient.SqlTransaction sqlTran, DataRow mainRow)
        {
            DBConnection cnt = CSystem.Sys.Svr.cntMain;
            //查出调配单
            DataSet dsAlloction = cnt.Select("Select * from D_Allocation where ID='" + ID + "'", "D_Allocation", conn, sqlTran);
            cnt.Select("Select * from D_AllocationBill where MainID='" + ID + "'", "D_AllocationBill", dsAlloction, conn, sqlTran);
            //生成两个仓库的出入库
            //生成成品仓库的出库
            DataSet dsWarehoueOut = cnt.Select("Select * from D_WarehouseOut where 1=0", "D_WarehouseOut", conn, sqlTran);
            cnt.Select("Select * from D_WarehouseOutBill where 1=0","D_WarehouseOutBill", dsWarehoueOut, conn, sqlTran);
            DataRow drMain = dsWarehoueOut.Tables["D_WarehouseOut"].NewRow();
            drMain["ID"] = Guid.NewGuid();
            drMain["WarehouseID"] = dsAlloction.Tables["D_Allocation"].Rows[0]["WarehouseID"];
            drMain["AllocationID"] = dsAlloction.Tables["D_Allocation"].Rows[0]["ID"];
            drMain["Code"] = dsAlloction.Tables["D_Allocation"].Rows[0]["Code"];
            //drMain["WorkCode"] = dsAlloction.Tables["D_Allocation"].Rows[0]["WorkCode"];
            drMain["Status"] = 0;
            drMain["Type"] = 0;//需要改
            drMain["BillDate"] = dsAlloction.Tables["D_Allocation"].Rows[0]["BillDate"];
            drMain["CreatedBy"] = CSystem.Sys.Svr.User;
            drMain["CreateDate"] = CSystem.Sys.Svr.SystemTime;
            drMain["CheckStatus"] = 1;
            drMain["CheckedBy"] = CSystem.Sys.Svr.User;
            drMain["CheckDate"] = CSystem.Sys.Svr.SystemTime;
            drMain["MoveStatus"] = 0;
            dsWarehoueOut.Tables["D_WarehouseOut"].Rows.Add(drMain);

            foreach (DataRow dr in dsAlloction.Tables["D_AllocationBill"].Rows)
            {
                DataRow drDetail = dsWarehoueOut.Tables["D_WarehouseOutBill"].NewRow();
                drDetail["ID"] = Guid.NewGuid();
                drDetail["MainID"] = drMain["ID"];
                drDetail["LineNumber"] = dsWarehoueOut.Tables["D_WarehouseOutBill"].Rows.Count;
                drDetail["PositionID"] = dr["PositionID"];
                //drDetail["DesignCodeID"] = dr["DesignCodeID"];
                drDetail["MaterialID"] = dr["MaterialID"];
                drDetail["IngredientID"] = dr["IngredientID"];
                drDetail["MachiningStandardID"] = dr["MachineStandardID"];
                //drDetail["ProductStandardID"] = dr["ProductStandardID"];
                drDetail["Length"] = dr["Length"];
                drDetail["Quantity1"] = dr["Quantity1"];
                drDetail["Quantity2"] = dr["Quantity2"];
                drDetail["SourceID"] = dr["ID"];
                dsWarehoueOut.Tables["D_WarehouseOutBill"].Rows.Add(drDetail);
            }
            CSystem.Sys.Svr.cntMain.Update(dsWarehoueOut.Tables["D_WarehouseOut"], conn, sqlTran);
            CSystem.Sys.Svr.cntMain.Update(dsWarehoueOut.Tables["D_WarehouseOutBill"], conn, sqlTran);

            //生成包装仓库的入库
            DataSet dsWarehoueIn = cnt.Select("Select * from D_WarehouseIn where 1=0", "D_WarehouseIn", conn, sqlTran);
            cnt.Select("Select * from D_WarehouseInBill where 1=0", "D_WarehouseInBill", dsWarehoueIn, conn, sqlTran);
            Guid ConsignWarehouseID;
            Guid DefaultPositionID;
            bool CheckIngredient;
            bool CheckMachiningStandard;
            bool CheckDesignCode;
            bool CheckProductStandard;
            DataSet ds = CSystem.Sys.Svr.cntMain.Select("Select * from P_Warehouse Where WarehouseType=3", conn, sqlTran);
            if (ds.Tables[0].Rows.Count == 0)
                return false;
            else
            {
                ConsignWarehouseID = (Guid)ds.Tables[0].Rows[0]["ID"];
                DefaultPositionID = (Guid)ds.Tables[0].Rows[0]["DefaultPositionID"];
                CheckIngredient = (int)ds.Tables[0].Rows[0]["CheckIngredient"]==1;
                CheckMachiningStandard =(int)ds.Tables[0].Rows[0]["CheckMachiningStandard"]==1;
                CheckDesignCode =(int)ds.Tables[0].Rows[0]["CheckDesignCode"]==1;
                CheckProductStandard =(int)ds.Tables[0].Rows[0]["CheckProductStandard"]==1;
            }
            drMain = dsWarehoueIn.Tables["D_WarehouseIn"].NewRow();
            drMain["ID"] = Guid.NewGuid();
            drMain["WarehouseID"] = ConsignWarehouseID;
            drMain["AllocationID"] = dsAlloction.Tables["D_Allocation"].Rows[0]["ID"];
            drMain["Code"] = dsAlloction.Tables["D_Allocation"].Rows[0]["Code"];
            //drMain["WorkCode"] = dsAlloction.Tables["D_Allocation"].Rows[0]["WorkCode"];
            drMain["Status"] = 0;
            drMain["Type"] = 0;//需要改
            drMain["BillDate"] = dsAlloction.Tables["D_Allocation"].Rows[0]["BillDate"];
            drMain["CreatedBy"] = CSystem.Sys.Svr.User;
            drMain["CreateDate"] = CSystem.Sys.Svr.SystemTime;
            drMain["CheckStatus"] = 1;
            drMain["CheckedBy"] = CSystem.Sys.Svr.User;
            drMain["CheckDate"] = CSystem.Sys.Svr.SystemTime;
            //drMain["MoveStatus"] = 0;
            dsWarehoueIn.Tables["D_WarehouseIn"].Rows.Add(drMain);

            foreach (DataRow dr in dsAlloction.Tables["D_AllocationBill"].Rows)
            {
                DataRow drDetail = dsWarehoueIn.Tables["D_WarehouseInBill"].NewRow();
                drDetail["ID"] = Guid.NewGuid();
                drDetail["MainID"] = drMain["ID"];
                drDetail["LineNumber"] = dsWarehoueIn.Tables["D_WarehouseInBill"].Rows.Count;
                drDetail["PositionID"] = DefaultPositionID;
                //drDetail["DesignCodeID"] = dr["DesignCodeID"];
                drDetail["MaterialID"] = dr["MaterialID"];
                if (CheckIngredient)
                    drDetail["IngredientID"] = dr["IngredientID"];
                if (CheckMachiningStandard)
                    drDetail["MachiningStandardID"] = dr["MachiningStandardID"];
                //if (CheckProductStandard)
                    //drDetail["ProductStandardID"] = dr["ProductStandardID"];
                drDetail["Length"] = dr["Length"];
                drDetail["Quantity1"] = dr["Quantity1"];
                drDetail["Quantity2"] = dr["Quantity2"];
                drDetail["SourceID"] = dr["ID"];
                dsWarehoueIn.Tables["D_WarehouseInBill"].Rows.Add(drDetail);
            }
            CSystem.Sys.Svr.cntMain.Update(dsWarehoueIn.Tables["D_WarehouseIn"], conn, sqlTran);
            CSystem.Sys.Svr.cntMain.Update(dsWarehoueIn.Tables["D_WarehouseInBill"], conn, sqlTran);

            //更新包装计划
            DataSet dsPackagePlan = cnt.Select("Select * from D_PackagePlan where ID='" + dsAlloction.Tables["D_Allocation"].Rows[0]["PackagePlanID"] + "'", "D_PackagePlan", conn, sqlTran);
            cnt.Select("Select * from D_PackagePlanBill where MainID='" + dsAlloction.Tables["D_Allocation"].Rows[0]["PackagePlanID"] + "'", "D_PackagePlanBill", dsPackagePlan, conn, sqlTran);
            int PackageStatus = 0;
            foreach (DataRow dr in dsPackagePlan.Tables["D_PackagePlanBill"].Rows)
            {
                DataRow[] allocation =dsAlloction.Tables["D_AllocationBill"].Select("SourceID='"+dr["ID"]+"'");
                if (allocation.Length > 0)
                {
                    dr["FinishQuantity1"] = (decimal)dr["FinishQuantity1"] + (decimal)allocation[0]["Quantity1"];
                    dr["FinishQuantity2"] = (decimal)dr["FinishQuantity2"] + (decimal)allocation[0]["Quantity2"];
                }
                if ((decimal)dr["Quantity2"] == (decimal)dr["FinishQuantity2"] && (decimal)dr["Quantity1"] == (decimal)dr["FinishQuantity1"])
                {
                    if (PackageStatus != 1)
                        PackageStatus = 2;
                }
                else
                {
                    if (PackageStatus == 2)
                        PackageStatus = 1;
                }
            }
            int finished = 0;
            if (PackageStatus == 2)
                finished = 1;
            dsPackagePlan.Tables["D_PackagePlan"].Rows[0]["PackageStatus"] = PackageStatus;
            dsPackagePlan.Tables["D_PackagePlan"].Rows[0]["Finished"] = finished;
            CSystem.Sys.Svr.cntMain.Update(dsPackagePlan.Tables["D_PackagePlan"], conn, sqlTran);
            CSystem.Sys.Svr.cntMain.Update(dsPackagePlan.Tables["D_PackagePlanBill"], conn, sqlTran);
            return true;
        }
        public override bool UnCheck(Guid ID, System.Data.SqlClient.SqlConnection conn, System.Data.SqlClient.SqlTransaction sqlTran, DataRow mainRow)
        {
            DBConnection cnt = CSystem.Sys.Svr.cntMain;
            //查出调配单
            DataSet dsAlloction = cnt.Select("Select * from D_Allocation where ID='" + ID + "'", "D_Allocation", conn, sqlTran);
            cnt.Select("Select * from D_AllocationBill where MainID='" + ID + "'", "D_AllocationBill", dsAlloction, conn, sqlTran);

            //删除出入库
            cnt.Excute(string.Format("Delete from D_WarehouseOutBill where MainID in (Select ID from D_WarehouseOut where AllocationID='{0}');Delete from D_WarehouseOut where AllocationID='{0}';Delete from D_WarehouseInBill where MainID in (Select ID from D_WarehouseIn where AllocationID='{0}');Delete from D_WarehouseIn where AllocationID='{0}';", ID), conn, sqlTran);
            
            //更新包装计划
            DataSet dsPackagePlan = cnt.Select("Select * from D_PackagePlan where ID='" + dsAlloction.Tables["D_Allocation"].Rows[0]["PackagePlanID"] + "'", "D_PackagePlan", conn, sqlTran);
            cnt.Select("Select * from D_PackagePlanBill where MainID='" + dsAlloction.Tables["D_Allocation"].Rows[0]["PackagePlanID"] + "'", "D_PackagePlanBill", dsPackagePlan, conn, sqlTran);
            int PackageStatus = (int)dsPackagePlan.Tables["D_PackagePlan"].Rows[0]["PackageStatus"];
            int finished = (int)dsPackagePlan.Tables["D_PackagePlan"].Rows[0]["Finished"];
            foreach (DataRow dr in dsPackagePlan.Tables["D_PackagePlanBill"].Rows)
            {
                DataRow[] allocation = dsAlloction.Tables["D_AllocationBill"].Select("SourceID='" + dr["ID"] + "'");
                if (allocation.Length > 0)
                {
                    dr["FinishQuantity1"] = (decimal)dr["FinishQuantity1"] - (decimal)allocation[0]["Quantity1"];
                    dr["FinishQuantity2"] = (decimal)dr["FinishQuantity2"] - (decimal)allocation[0]["Quantity2"];
                    if (0 == (decimal)dr["FinishQuantity2"] && 0 == (decimal)dr["FinishQuantity1"])
                    {
                        if (PackageStatus == 2)
                            PackageStatus = 0;
                    }
                    else
                    {
                        PackageStatus = 1;
                    }
                }
            }
            
            if (PackageStatus != 2)
                finished = 0;
            dsPackagePlan.Tables["D_PackagePlan"].Rows[0]["PackageStatus"] = PackageStatus;
            dsPackagePlan.Tables["D_PackagePlan"].Rows[0]["Finished"] = finished;
            CSystem.Sys.Svr.cntMain.Update(dsPackagePlan.Tables["D_PackagePlan"], conn, sqlTran);
            CSystem.Sys.Svr.cntMain.Update(dsPackagePlan.Tables["D_PackagePlanBill"], conn, sqlTran);
            return true;
        }
        public override System.Windows.Forms.Form GetForm(CRightItem right, System.Windows.Forms.Form mdiForm)
        {
            frmBrowser frm = (frmBrowser)base.GetForm(right, mdiForm);
            SortedList<string, SortedList<string, object>> defaultValue = new SortedList<string, SortedList<string, object>>();
            SortedList<string, object> Value = new SortedList<string, object>();
            Value.Add("BillDate", CSystem.Sys.Svr.SystemTime.Date);
            defaultValue.Add("D_Allocation", Value);
            frm.DefaultValue = defaultValue;
            return frm;
        }
    }
}
