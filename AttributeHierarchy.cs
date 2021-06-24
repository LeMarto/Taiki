using System;
using System.Collections.Generic;

namespace Taiki
{
    public class AttributeHierarchy : List<IAttributeHierarchyMember>, IEquatable<AttributeHierarchy>, ICloneable
    {
        private string _dimensionName;
        #region Name Property
        private string _name;
        public string Name => _name;
        #endregion
        #region ExcludeMembers Property
        /*
        If this flag is set, the members will be used for
        excluding, instead of the default include.
        */
        private bool _excludeMembers = false;
        public bool ExcludeMembers
        {
            get => _excludeMembers;
            set => _excludeMembers = value;
        }
        #endregion
        #region IncludeInRows Property 
        /*
        If this flag is set (default), the attribute hierarchy
        will be included in the rows section of the MDX.
        */
        private bool _includeInRows = true;
        public bool IncludeInRows
        {
            get => _includeInRows;
            set => _includeInRows = value;
        }
        #endregion
        #region UseForBatch Property
        /*
        If this flag is set, the attribute hierarchy
        will be used to split the extraction in
        multiple batches
        */
        private bool _useForBatch = false;
        public bool UseForBatch
        {
            get => _useForBatch;
            set => _useForBatch = value;
        }
        #endregion
        #region HasMembers Property
        /*
        Is the attribute hierarchy filtered?
        */
        public bool HasMembers
        {
            get => Count > 0;
        }
        #endregion
        public AttributeHierarchy(string dimensionName, string attributeHierarchyName) : base()
        {
            this._name = attributeHierarchyName;
            this._dimensionName = dimensionName;
        }
        public bool Equals(AttributeHierarchy other)
        {
            if (other._name == this._name)
                return true;
            return false;
        }
        public object Clone()
        {
            AttributeHierarchy clone = new AttributeHierarchy(_dimensionName, _name);
            
            clone._excludeMembers = _excludeMembers;
            clone._includeInRows = _includeInRows;
            clone._useForBatch = _useForBatch;

            foreach (IAttributeHierarchyMember member in this)
            {
                clone.Add((IAttributeHierarchyMember)member.Clone());
            }

            return (object)clone;
        }
        #region Checks for Upserting filters
        public bool ContainsMemberCaption(string caption)
        {
            AttributeHierarchyMember memberSingle = new AttributeHierarchyMember(_dimensionName, _name, caption);
            if (this.Contains(memberSingle))
                return true;
            return false;
        }
        public bool ContainsMemberCaptionRange(string captionFrom, string captionTo)
        {
            AttributeHierarchyMemberRange memberRange = new AttributeHierarchyMemberRange(_dimensionName, _name, captionFrom, captionTo);
            if (this.Contains(memberRange))
                return true;   
            return false;
        }
        #endregion
        public string ColumnName => String.Format("[{0}].[{1}].[{1}].[MEMBER_CAPTION]", _dimensionName, _name);
        public override string ToString()
        {
            return _name;
        }

    }
}