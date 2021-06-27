using System;

namespace Taiki
{
    public enum AttributeHierarchyMemberType
    {
        Single,
        Range
    }

    public class AttributeHierarchyMember : IEquatable<AttributeHierarchyMember>
    {
        #region DimensionName Property
        private string _dimensionName;
        public string DimensionName => _dimensionName;
        #endregion
        #region AttributeHierarchyName Property
        private string _attributeHierarchyName;
        public string AttributeHierarchyName => _attributeHierarchyName;
        #endregion
        #region From Property
        private string _from;
        public string From => _from;
        #endregion
        #region To Property
        private string _to;
        public string To => _to;
        #endregion
        #region MemberType Property
        private AttributeHierarchyMemberType _memberType;
        public AttributeHierarchyMemberType MemberType => _memberType;
        #endregion
        public string UniqueName 
        {
            get
            {
                string uniqueName;
                if (_memberType == AttributeHierarchyMemberType.Single)
                    uniqueName = string.Format("[{0}].[{1}].{3}[{2}]", _dimensionName, _attributeHierarchyName, _from, _valuesAsCaptions ? "" : "&");
                else
                    uniqueName = string.Format("[{0}].[{1}].{4}[{2}]:[{0}].[{1}].{4}[{3}]", _dimensionName, _attributeHierarchyName, _from == null ? "NULL" : _from, _to == null ? "NULL" : _to, _valuesAsCaptions ? "" : "&");
                return uniqueName;
            }
        }
        #region ValuesAsCaptions Property
        private bool _valuesAsCaptions = false;
        public bool ValuesAsCaptions => _valuesAsCaptions;
        public AttributeHierarchyMember AsCaptions()
        {
            _valuesAsCaptions = true;
            return this;
        }
        #endregion
        public AttributeHierarchyMember(string dimensionName, string attributeHierarchyName, string from)
        {
            _dimensionName = dimensionName;
            _attributeHierarchyName = attributeHierarchyName;
            _from = from;
            _memberType = AttributeHierarchyMemberType.Single;
        }  
        public AttributeHierarchyMember(string dimensionName, string attributeHierarchyName, string from, string to)
        {
            if (from == null && to == null)
                throw new Exception("You cant have a range that goes from null to null!");
            _dimensionName = dimensionName;
            _attributeHierarchyName = attributeHierarchyName;
            _from = from;
            _to = to;
            _memberType = AttributeHierarchyMemberType.Range;
        }
        public AttributeHierarchyMember(string uniqueName, AttributeHierarchyMemberType membertype)
        {
            if (membertype == AttributeHierarchyMemberType.Single)
            {
                _dimensionName = uniqueName.Split("].[")[0];
                _dimensionName = _dimensionName.Substring(1, _dimensionName.Length - 1);
                _attributeHierarchyName = uniqueName.Split("].[")[1].Split("].&[")[0];
                _from = uniqueName.Split("].[")[1].Split("].&[")[1];
                _from = _from.Substring(0, _from.Length - 1);
                _memberType = AttributeHierarchyMemberType.Single;
            }
            else
                throw new NotImplementedException();
        }
        public object Clone() => this.MemberwiseClone();

        public bool Equals(AttributeHierarchyMember other)
        {
            if (other.UniqueName == this.UniqueName)
                return true;
            return false;
        }
    }
}