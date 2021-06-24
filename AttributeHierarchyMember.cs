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
            string[] sections = uniqueName.Split('.');
            
            if (sections.Length < 3)
                throw new Exception("Malformed Attribute Hierarchy Unique Name!");
            
            _dimensionName = ParseDimensionOrAttributeHierarchyName(sections[0]);
            _attributeHierarchyName = ParseDimensionOrAttributeHierarchyName(sections[1]);
            _caption = ParseMemberCaption(sections[2]);
        }
        #region Unique Name Parsing helper functions
        private string ParseDimensionOrAttributeHierarchyName(string value)
        {
            int start_token_pos = 0, end_token_pos = value.Length-1;
            int start_pos = start_token_pos + 1;
            int length = end_token_pos - start_pos;
            
            if (value[start_token_pos] != '[' && value[end_token_pos] != ']')
                throw new Exception("malformed dimension or attribute");
            
            return value.Substring(start_pos,length);
        }
        private string ParseMemberCaption(string value)
        {
            int start_token_pos = 1, end_token_pos = value.Length-1;
            int start_pos = start_token_pos + 1;
            int length = end_token_pos - start_pos;
            
            if (value[start_token_pos] != '[' && value[end_token_pos] != ']')
                throw new Exception("malformed caption");
            
            return value.Substring(start_pos,length);
        }
        #endregion
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