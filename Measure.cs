using System;
namespace Taiki
{
    public class Measure : IMeasure
    {
        #region Name Attribute
        private string _name;
        public string Name => _name;
        #endregion
        public bool IsCalculated => false;
        public string Calculation => null;
        public bool IsUsedForBatch  => false;
        public int Ordinal {get; set;}
        public Measure(string name)
        {
            this._name = name;
        }
        public bool Equals(IMeasure other)
        {
            if (other.Name == _name)
                return true;
            return false;
        }
        public object Clone() => this.MemberwiseClone();
        public string ColumnName => String.Format("[Measures].[{0}]", _name);
    }
}