using System;
using System.Collections.Generic;
using System.Text;
using UI;

namespace OH
{
    public class clsRptOtherProduct:BaseReport
    {
        public override bool Initialize()
        {
            _Title = "异常品标识卡查询";
            _SQL = " select A.BillDate,C.Name MaterialName,D.Name MachiningStandardName,B.Code,B.WorkCode,B.Reson,B.Forward,E.Name ProcessName,OperatorName,B.Notes from D_OtherProduct A,D_OtherProductBill B,P_Material C,P_MachiningStandard D,P_Process E " +
                    " where A.ID=B.MainID and C.ID=B.MaterialID and D.ID=B.MachiningStandardID and E.ID=B.ProcessID";

            base.AddCondition("startDate", "开始时间", "Date", "A.BillDate>='{0:yyyy-MM-dd}'");
            base.AddCondition("endDate", "结束时间", "Date", "A.BillDate<='{0:yyyy-MM-dd}'");

            _IsWhere = false;
            _IsAppendSQL = true;

            base.AddColumn("BillDate", "日期");
            base.AddColumn("MaterialName", "产品");
            base.AddColumn("MachiningStandardName", "加工图号");
            base.AddColumn("Code", "导轨编号");
            base.AddColumn("WorkCode", "工票号");
            base.AddColumn("Reson", "异常原因");
            base.AddColumn("Forward", "流向");
            base.AddColumn("ProcessName", "工序");
            base.AddColumn("OperatorName", "作业人员");
            base.AddColumn("Notes", "备注");

            return base.Initialize();
        }
    }
}
