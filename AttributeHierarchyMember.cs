using System;

namespace Taiki
{
    public class AttributeHierarchyMember : IEquatable<AttributeHierarchyMember>, IAttributeHierarchyMember
    {
        #region DimensionName Property
        private string _dimensionName;
        public string DimensionName => _dimensionName;
        #endregion
        #region AttributeHierarchyName Property
        private string _attributeHierarchyName;
        public string AttributeHierarchyName => _attributeHierarchyName;
        #endregion
        #region Caption Property
        private string _caption;
        public string Caption => _caption;
        #endregion
        public string UniqueName => string.Format("[{0}].[{1}].&[{2}]", _dimensionName, _attributeHierarchyName, _caption);
        public AttributeHierarchyMember(string dimensionName, string attributeHierarchyName, string caption)
        {
            this._dimensionName = dimensionName;
            this._attributeHierarchyName = attributeHierarchyName;
            this._caption = caption;
        }
        public AttributeHierarchyMember(string uniqueName)
        {
            _dimensionName = uniqueName.Split("].[")[0];
            _dimensionName = _dimensionName.Substring(1, _dimensionName.Length - 1);
            _attributeHierarchyName = uniqueName.Split("].[")[1].Split("].&[")[0];
            _caption = uniqueName.Split("].[")[1].Split("].&[")[1];
            _caption = _caption.Substring(0, _caption.Length - 1);
        }
        public object Clone() => this.MemberwiseClone();

        public bool Equals(AttributeHierarchyMember other)
        {
            if (other.UniqueName == this.UniqueName)
                return true;
            return false;
        }
        public bool Equals(IAttributeHierarchyMember other)
        {
            if (other.UniqueName == this.UniqueName)
                return true;
            return false;
        }
    }
}