using System;

namespace Taiki
{
    public class AttributeHierarchyMemberRange : IEquatable<AttributeHierarchyMemberRange>, IAttributeHierarchyMember
    {
        #region DimensionName Property
        private string _dimensionName;
        public string DimensionName => _dimensionName;
        #endregion
        #region AttributeHierarchyName Property
        private string _attributeHierarchyName;
        public string AttributeHierarchyName => _attributeHierarchyName;
        #endregion
        public string UniqueName => string.Format("[{0}].[{1}].&[{2}]:[{0}].[{1}].&[{3}]", _dimensionName, _attributeHierarchyName, _captionFrom == null ? "NULL" : _captionFrom, _captionTo == null ? "NULL" : _captionTo);
        #region CaptionFrom Property
        private string _captionFrom;
        public string CaptionFrom => _captionFrom;
        #endregion
        #region CaptionTo Property
        private string _captionTo;
        public string CaptionTo => _captionTo;
        #endregion
        public AttributeHierarchyMemberRange(string dimensionName, string attributeHierarchyName, string captionFrom, string captionTo)
        {
            if (captionFrom == null && captionTo == null)
                throw new Exception("You cant have a range that goes from null to null!");

            this._dimensionName = dimensionName;
            this._attributeHierarchyName = attributeHierarchyName;
            this._captionFrom = captionFrom;
            this._captionTo = captionTo;
        }

        public object Clone() => this.MemberwiseClone();

        public bool Equals(AttributeHierarchyMemberRange other)
        {
            if (other.UniqueName == UniqueName)
                return true;
            return false;
        }

        public bool Equals(IAttributeHierarchyMember other)
        {
            if (other.UniqueName == UniqueName)
                return true;
            return false;
        }
    }
}