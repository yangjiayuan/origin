using System;
using System.Collections.Generic;
using System.Text;
using System.Data;

namespace Base
{
    public class COMField
    {
        public enum Enum_Visible:int { VisibleInBrower = 1, VisibleInDetail = 2, VisibleAll = 3, NotVisible = 0 };
        public enum Enum_OrderType  : int {ASC = 0, DESC = 1}
        public string FieldID;
        public string TableName;
        public string FieldName;
        public string FieldTitle;
        public int ColOrder;
        public bool Mandatory;
        public int FieldType;  //0是本表字段 1是外表字段
        public string ValueType;
        public string Format;
        public string DefaultValue;
        public Enum_Visible Visible;
        public bool IsPrimaryKey;
        public Enum_OrderType OrderType;
        public int OrderIndex;
        public bool Enable;
        public string RelationPath;
        public string TableRelation;
        public string RFieldName;
        public string LeftTableName;
        public string RightTableName;
        public string LeftFieldName;
        public string RightFieldName;
        public string JoinType;
        public bool ShowSummary;
        public bool NewLine;
        public int Width;
        public int Height = 1;
        public string GroupName;
        //存放字典数据表
        public DataSet SourceDS;
        public COMField() 
        {
            Visible = Enum_Visible.NotVisible;
            Enable = true;
        }
        public override string ToString()
        {
            return FieldTitle;
        }
        public string FullFieldName
        {
            get
            {
                if (FieldType == 0)
                    return string.Format("{0}.{1}", TableName, FieldName);
                else
                    return string.Format("{0}.{1}", TableRelation, RFieldName);
            }
        }
        public COMField Clone
        {
            get 
            {
                COMField FieldCopy = new COMField();
                FieldCopy.FieldID = FieldID;
                FieldCopy.TableName=TableName;
                FieldCopy.FieldName = FieldName;
                FieldCopy.FieldTitle = FieldTitle;
                FieldCopy.ColOrder=ColOrder;
                FieldCopy.Mandatory=Mandatory;
                FieldCopy.FieldType = FieldType;
                FieldCopy.ValueType = ValueType;
                FieldCopy.Format = Format;
                FieldCopy.DefaultValue = DefaultValue;
                FieldCopy.Visible=Visible;
                FieldCopy.IsPrimaryKey=IsPrimaryKey;
                FieldCopy.OrderType=OrderType;
                FieldCopy.OrderIndex=OrderIndex;
                FieldCopy.Enable=Enable;
                FieldCopy.TableRelation=TableRelation;
                FieldCopy.RFieldName=RFieldName;
                FieldCopy.LeftTableName=LeftTableName;
                FieldCopy.RightTableName=RightTableName;
                FieldCopy.LeftFieldName=LeftFieldName;
                FieldCopy.RightFieldName=RightFieldName;
                FieldCopy.JoinType=JoinType;
                FieldCopy.RelationPath = RelationPath;
                FieldCopy.ShowSummary = ShowSummary;
                FieldCopy.NewLine = NewLine;
                FieldCopy.Width = Width;
                FieldCopy.Height = Height;
                FieldCopy.GroupName = GroupName;
                FieldCopy.SourceDS = SourceDS;
                return FieldCopy;
            }
        }


    }
}
