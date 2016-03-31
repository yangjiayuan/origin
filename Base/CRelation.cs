using System;
using System.Collections.Generic;
using System.Text;

namespace Base
{
    public class CRelation
    {
        public string RelationName;
        public string LeftTable;
        public string LeftField;
        public string RightTable;
        public string RightField;
        public string JoinType;
        private string _Alia;
        private List<CRelation> _Children;
        public string Alia
        {
            get
            {
                if (_Alia == null)
                    return RelationName;
                else
                    return _Alia;
            }
            set { _Alia = value; }
        }
        public List<CRelation> Children
        {
            get
            {
                if (_Children == null)
                    _Children = new List<CRelation>();
                return _Children;
            }
        }
        public int ChildrenCount
        {
            get
            {
                if (_Children == null)
                    return 0;
                else
                    return _Children.Count;
            }
        }

        public CRelation Clone()
        {
            CRelation rel = new CRelation();
            rel.RelationName=this.RelationName;
            rel.LeftField=this.LeftField;
            rel.LeftTable=this.LeftTable;
            rel.RightField=this.RightField;
            rel.RightTable=this.RightTable;
            rel.JoinType=this.JoinType;
            rel._Alia = this._Alia;
            //由于复制一般针对原始的关系,不涉及到子关系,所以暂不处理子关系
            return rel;
        }
    }
}
