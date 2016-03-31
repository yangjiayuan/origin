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
            _Title = "�쳣Ʒ��ʶ����ѯ";
            _SQL = " select A.BillDate,C.Name MaterialName,D.Name MachiningStandardName,B.Code,B.WorkCode,B.Reson,B.Forward,E.Name ProcessName,OperatorName,B.Notes from D_OtherProduct A,D_OtherProductBill B,P_Material C,P_MachiningStandard D,P_Process E " +
                    " where A.ID=B.MainID and C.ID=B.MaterialID and D.ID=B.MachiningStandardID and E.ID=B.ProcessID";

            base.AddCondition("startDate", "��ʼʱ��", "Date", "A.BillDate>='{0:yyyy-MM-dd}'");
            base.AddCondition("endDate", "����ʱ��", "Date", "A.BillDate<='{0:yyyy-MM-dd}'");

            _IsWhere = false;
            _IsAppendSQL = true;

            base.AddColumn("BillDate", "����");
            base.AddColumn("MaterialName", "��Ʒ");
            base.AddColumn("MachiningStandardName", "�ӹ�ͼ��");
            base.AddColumn("Code", "������");
            base.AddColumn("WorkCode", "��Ʊ��");
            base.AddColumn("Reson", "�쳣ԭ��");
            base.AddColumn("Forward", "����");
            base.AddColumn("ProcessName", "����");
            base.AddColumn("OperatorName", "��ҵ��Ա");
            base.AddColumn("Notes", "��ע");

            return base.Initialize();
        }
    }
}
