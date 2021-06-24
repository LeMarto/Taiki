using System;
using System.Collections.ObjectModel;
using System.Text;

namespace Taiki
{
    public class CurrentMembersUniqueNameCalculatedMeasure : IMeasure
    {
        #region Name Attribute
        private string _name;
        public string Name => _name;
        #endregion
        public bool IsUsedForBatch => true;
        #region Calculation Property
        private string _calculation;
        public string Calculation => _calculation;
        #endregion
        public bool IsCalculated => true;
        public int Ordinal {get; set;}
        public CurrentMembersUniqueNameCalculatedMeasure(string name, ReadOnlyCollection<IMeasure> otherMeasures, Dimension dim, AttributeHierarchy attr)
        {
                _name = name;
                CommaHelper orHelper = new CommaHelper("OR ");
                orHelper.Reset();
                StringBuilder sb = new StringBuilder();
                sb.Append("IIF(");
                foreach (Measure measure in otherMeasures)
                {
                    sb.Append(orHelper.Get());
                    sb.AppendFormat("[Measures].[{0}] <> 0", measure.Name);
                }
                sb.AppendFormat(",[{0}].[{1}].CURRENTMEMBER.UNIQUENAME, NULL)", dim.Name, attr.Name);
                _calculation =  sb.ToString();
        }
        public object Clone() => this.MemberwiseClone();
        public bool Equals(IMeasure other)
        {
            if (other.Name == _name)
                return true;
            return false;
        }
        public string ColumnName => String.Format("[Measures].[{0}]", _name);
    }
}