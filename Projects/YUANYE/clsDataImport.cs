using System;
using System.Collections.Generic;
using System.Text;
using UI;
using Base;
using System.Data;
using System.Data.SqlClient;
using System.Data.OleDb;
using System.Collections;
using System.Windows.Forms;
//using Excel=Microsoft.Office.Interop.Excel;
//using Microsoft.Office.Interop.Excel;

namespace YUANYE
{
    public class clsDataImport : IDataImport
    {
        #region IDataImport Members

        Hashtable hsQuote=new Hashtable();
        private const double tax = 0.17;
        public bool Import(string fileName, Guid customerID)
        {
            DBConnection cnt = CSystem.Sys.Svr.cntMain;
            //先检查数据，订单号是否已存在，物料有没有，参数是否全。
            //Excel.Application excel = new Microsoft.Office.Interop.Excel.Application();
            //Excel.Workbook wb = excel.Workbooks.Open(fileName, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing);
            //Excel.Worksheet workSheet = (Worksheet)wb.Worksheets[1];
             OleDbConnection excelConn = new OleDbConnection("provider=microsoft.jet.oledb.4.0;extended properties=excel 8.0;data source=" + fileName);
             DataTable dtData = new DataTable();
            try
            {               
                excelConn.Open();
                DataTable dtTables = excelConn.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, new object[] { null, null, null, "TABLE" });
                string tName = dtTables.Rows[0]["TABLE_NAME"] as string;
                //tName = tName.Substring(1, tName.Length - 2);
                string sql = string.Format("select * from [{0}]", tName);               
                OleDbDataAdapter da = new OleDbDataAdapter(sql, excelConn);
                da.Fill(dtData);    // 填充dataset
            }
            catch { }
            finally
            {
                excelConn.Close();
            }

            SortedList<string, Guid> materialIDs = new SortedList<string, Guid>();
            SortedList<string, int> paraCounts = new SortedList<string, int>();
            SortedList<string, decimal> salesPrices = new SortedList<string, decimal>();
            SortedList<string, decimal> costPrices = new SortedList<string, decimal>();
            StringBuilder sb = new StringBuilder();
            hsQuote.Clear();
            foreach (DataRow dr in dtData.Rows)
            {
                //取订单号
                string s = dr["订单号"] as string;
                DataSet ds = cnt.Select(string.Format("Select code from D_SalesOrder where Code='{0}' ", s));
                if (ds.Tables[0].Rows.Count > 0)
                {
                    sb.AppendLine(string.Format("订单号（{0}）的订单已存在！", s));
                }
                s = dr["报价图号"] as string;
                ds = cnt.Select(string.Format("Select * from P_Quote where Code='{0}' ", s));
                if (ds.Tables[0].Rows.Count == 0)
                {
                    sb.AppendLine(string.Format("报价图号（{0}）不存在！", s));
                }
                else
                    hsQuote[s] = ds.Tables[0].Rows[0]["ID"];
            }

            if (sb.Length > 0)
            {
                Msg.Error(sb.ToString());
                return false;
            }

            //导数据
            using (SqlConnection conn = new SqlConnection(cnt.ConnectionString))
            {
                conn.Open();
                SqlTransaction tran = conn.BeginTransaction();
                try
                {
                    DataSet ds = cnt.Select("Select * from D_SalesOrder where 1=0", "D_SalesOrder", conn, tran);
                    cnt.Select("Select * from D_SalesOrderBill where 1=0", "D_SalesOrderBill", ds, conn, tran);
                    DataTable dtSalesOrder = ds.Tables["D_SalesOrder"];
                    DataTable dtSalesOrderBill = ds.Tables["D_SalesOrderBill"];
                    DataSet customerDs = cnt.Select("select * from P_Customer where id='"+customerID+"'", conn, tran);
                    decimal _strFreightPrice = VarConverTo.ConvertToDecimal(customerDs.Tables[0].Rows[0]["WeightRate"]);
                    decimal _strWeightRate = VarConverTo.ConvertToDecimal(customerDs.Tables[0].Rows[0]["FreightPrice"]);
                    string _strFactoryID = VarConverTo.ConvertToString(customerDs.Tables[0].Rows[0]["FactoryID"]);
                    string _strBusinessUserId = VarConverTo.ConvertToString(customerDs.Tables[0].Rows[0]["BusinessUserId"]);
                    DataSet MaterialCustomerDs = new DataSet();
                    string sql = "select b.mainid,b.AlisaCode,b.CustomerID from P_MaterialBill b where b.CustomerID='" + customerID + "'";
                    MaterialCustomerDs = CSystem.Sys.Svr.cntMain.Select(sql);

                    Guid mainID = Guid.NewGuid();
                    int lineNumber = 0;
                    decimal money = 0;
                    decimal tax = 0;
                    decimal amount = 0;
                    decimal quantity = 0;
                    decimal length = 0;
                    decimal weight = 0;
                    DataRow lastMainRow = null;
                    foreach (DataRow dr in dtData.Rows)
                    {
                        lineNumber = 0;
                        money = 0;
                        tax = 0;
                        amount = 0;
                        quantity = 0;
                        length = 0;
                        weight = 0;
                        //取订单号
                        string s = dr["订单号"] as string;
                        DataRow drMain = dtSalesOrder.NewRow();
                        mainID = Guid.NewGuid();
                        drMain["ID"] = mainID;
                        drMain["CustomerID"] = customerID;
                        drMain["QuoteId"]=hsQuote[dr["报价图号"]];
                        drMain["Code"] = s;
                        drMain["BusinessUserId"] = _strBusinessUserId;
                        drMain["FactoryID"] = _strFactoryID;
                        drMain["ElevatorCode"] = VarConverTo.ConvertToString(dr["电梯编号"]);
                        drMain["ProjectName"] = VarConverTo.ConvertToString(dr["项目名称"]);
                        drMain["ContractCode"] = VarConverTo.ConvertToString(dr["合同编号"]);
                        drMain["ConsignDate"] = (dr["交货日期"]);
                        drMain["InvoiceStatus"] = 0;
                        drMain["ConsignStatus"] = 0;
                        drMain["ConsignType"] = VarConverTo.ConvertToInt(dr["发货方式"]);
                        drMain["Notes"] = VarConverTo.ConvertToString(dr["备注"]);
                        drMain["AllocationCode"] = "";
                        drMain["ProjectCode"] = "";
                        drMain["packagesize"] = 0;
                        drMain["packagequantity"] = 0;
                        drMain["BoxCount"] = 0;
                        drMain["AdditionalCost"] = 0;
                        drMain["IsTop"] = 0;
                        drMain["ConsignCodes"] = "";
                        drMain["InvoiceCodes"] = "";


                        drMain["BillDate"] = CSystem.Sys.Svr.SystemTime;
                        drMain["CreateDate"] = CSystem.Sys.Svr.SystemTime;
                        drMain["CreatedBy"] = CSystem.Sys.Svr.User;
                        drMain["CheckStatus"] = 0;
                        drMain["deleted"] = 0;
                        quantity = VarConverTo.ConvertToDecimal(dr["数量"]);
                        length = VarConverTo.ConvertToDecimal(dr["长度"]);
                        dtSalesOrder.Rows.Add(drMain);
                        //明细的处理
                        s = dr["报价图号"] as string;
                        DataSet QuoteBillDs = cnt.Select("select a.*,b.StartDate,b.EndDate,c.Code as MaterialCode,c.Name as MaterialName,d.Name as MachiningStandard,c.NeedLength,c.ConvertWeight,c.Weight from P_QuoteBill a inner join P_Material c on a.MaterialID=c.id left join P_MachiningStandard d on a.MachiningStandardID=d.id inner join P_Quote b on a.MainId=b.Id where a.MainID='" + hsQuote[s] + "'", conn, tran);
                        if (QuoteBillDs.Tables[0].Rows.Count == 0)
                        {
                            Msg.Error(string.Format("报价图号是([0]),的没有定义明细数据!", s));
                            return false;
                        }
                        else
                        {
                            foreach (DataRow drdtl in QuoteBillDs.Tables[0].Rows)
                            {
                                int isHaveTax = VarConverTo.ConvertToInt(drdtl["HaveTax"]);
                                decimal _Length = VarConverTo.ConvertToDecimal(drdtl["Length"]);
                                decimal _Quantity = VarConverTo.ConvertToDecimal(drdtl["Quantity"]);
                                decimal _Price = VarConverTo.ConvertToDecimal(drdtl["Price"]);
                                decimal _PriceType = VarConverTo.ConvertToInt(drdtl["PriceType"]);


                                DataRow drBill = dtSalesOrderBill.NewRow();
                                drBill["ID"] = Guid.NewGuid();
                                drBill["MainID"] = mainID;
                                drBill["LineNumber"] = lineNumber++;
                                drBill["MaterialID"] = drdtl["MaterialID"];
                                drBill["MachiningStandardID"] = drdtl["MachiningStandardID"];
                                drBill["Price"] = drdtl["Price"];

                                drBill["ConsignQuantity"] = System.Convert.ToDecimal(drdtl["Quantity"]) * quantity;
                                drBill["MaterialAlisaName"] = getCustomerAlsia(MaterialCustomerDs.Tables[0], drdtl["MaterialID"].ToString(), drdtl["MaterialName"].ToString());
                                //drBill["NeedLength"] = drdtl["NeedLength"];
                                drBill["Length"] = (length > 0 ? length : _Length);
                                drBill["TotalLength"] = _Quantity * quantity * VarConverTo.ConvertToDecimal(drBill["Length"]);
                                if (VarConverTo.ConvertToDecimal(drdtl["Weight"]) > 0)
                                {
                                    drBill["TotalWeight"] = VarConverTo.ConvertToDecimal(drdtl["Weight"]) * Convert.ToDecimal(drdtl["Quantity"]) * quantity;
                                }
                                else
                                {
                                    drBill["TotalWeight"] = Convert.ToDecimal(drdtl["Quantity"]) * quantity * length * VarConverTo.ConvertToDecimal(drdtl["ConvertWeight"]);
                                }
                                drBill["PriceType"] = _PriceType;
                                calculate(drBill, isHaveTax);
                                amount += VarConverTo.ConvertToDecimal(drBill["SalesAmount"]);
                                money += VarConverTo.ConvertToDecimal(drBill["SalesMoney"]);
                                tax += VarConverTo.ConvertToDecimal(drBill["SalesTax"]);
                                weight += VarConverTo.ConvertToDecimal(drBill["TotalWeight"]);
                                dtSalesOrderBill.Rows.Add(drBill);
                            }
                            drMain["Money"] = money;
                            drMain["Amount"] = amount;
                            drMain["Weight"] = weight;
                            drMain["Tax"] = tax;
                        }
                    }
                    if (lastMainRow != null)
                    {
                        lastMainRow["Money"] = money;
                        lastMainRow["Amount"] = amount;
                        lastMainRow["Tax"] = tax;
                    }
                    cnt.Update(dtSalesOrder);
                    cnt.Update(dtSalesOrderBill);

                    tran.Commit();
                }
                catch
                {
                    tran.Rollback();
                }
            }
            return true;
        }
        private string getCustomerAlsia(DataTable customerDT, string MaterialID, string MaterialName)
        {
            DataRow[] rows = customerDT.Select("mainid='" + MaterialID + "'");
            if (rows.Length == 1)
            {
                return rows[0]["AlisaCode"].ToString();
            }
            else
                return MaterialName;
        }

        private void calculate(DataRow drSalesOrderBill, int isHaveTax)
        {
            if (isHaveTax == 0)
            {
                if ((int)drSalesOrderBill["PriceType"] == 0)
                {
                    drSalesOrderBill["SalesAmount"] = (decimal)drSalesOrderBill["Price"] * System.Convert.ToDecimal(drSalesOrderBill["ConsignQuantity"]);
                    drSalesOrderBill["SalesMoney"] = (decimal)drSalesOrderBill["Price"] * System.Convert.ToDecimal(drSalesOrderBill["ConsignQuantity"]) * (decimal)(1 - tax);
                    drSalesOrderBill["SalesTax"] = (decimal)drSalesOrderBill["Price"] * System.Convert.ToDecimal(drSalesOrderBill["ConsignQuantity"]) * (decimal)tax;
                }
                else if ((int)drSalesOrderBill["PriceType"] == 1)
                {
                    drSalesOrderBill["SalesAmount"] = (decimal)drSalesOrderBill["Price"] * System.Convert.ToDecimal(drSalesOrderBill["TotalLength"]);
                    drSalesOrderBill["SalesMoney"] = (decimal)drSalesOrderBill["Price"] * System.Convert.ToDecimal(drSalesOrderBill["TotalLength"]) * (decimal)(1 - tax);
                    drSalesOrderBill["SalesTax"] = (decimal)drSalesOrderBill["Price"] * System.Convert.ToDecimal(drSalesOrderBill["TotalLength"]) * (decimal)tax;
                }
            }
            else
            {
                if ((int)drSalesOrderBill["PriceType"] == 0)
                {
                    drSalesOrderBill["SalesAmount"] = (decimal)drSalesOrderBill["Price"] * System.Convert.ToDecimal(drSalesOrderBill["ConsignQuantity"]) / (decimal)(1 - tax);
                    drSalesOrderBill["SalesMoney"] = (decimal)drSalesOrderBill["Price"] * System.Convert.ToDecimal(drSalesOrderBill["ConsignQuantity"]);
                    drSalesOrderBill["SalesTax"] = (decimal)drSalesOrderBill["Price"] * System.Convert.ToDecimal(drSalesOrderBill["ConsignQuantity"]) * (decimal)tax;
                }
                else if ((int)drSalesOrderBill["PriceType"] == 1)
                {
                    drSalesOrderBill["SalesAmount"] = (decimal)drSalesOrderBill["Price"] * System.Convert.ToDecimal(drSalesOrderBill["TotalLength"]) / (decimal)(1 - tax);
                    drSalesOrderBill["SalesMoney"] = (decimal)drSalesOrderBill["Price"] * System.Convert.ToDecimal(drSalesOrderBill["TotalLength"]);
                    drSalesOrderBill["SalesTax"] = (decimal)drSalesOrderBill["Price"] * System.Convert.ToDecimal(drSalesOrderBill["TotalLength"]) * (decimal)tax;
                }
            }
        }

        #endregion
    }
}
