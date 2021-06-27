using System;
using System.Collections.Generic;

namespace Taiki
{
    public class AttributeHierarchy : List<AttributeHierarchyMember>, IEquatable<AttributeHierarchy>, IEquatable<string>, ICloneable
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
        public AttributeHierarchy AsExclusionFilter()
        {
            _excludeMembers = true;
            return this;
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
        public AttributeHierarchy NotInRows()
        {
            _includeInRows = false;
            return this;
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
        public AttributeHierarchy AsBatch()
        {
            _useForBatch = true;
            return this;
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
            if (_name == other._name)
                return true;
            return false;
        }
         public bool Equals(string attributeHierarchyName)
        {
            if (_name == attributeHierarchyName)
                return true;
            return false;
        }
        public object Clone()
        {
            AttributeHierarchy clone = new AttributeHierarchy(_dimensionName, _name);
            
            clone._excludeMembers = _excludeMembers;
            clone._includeInRows = _includeInRows;
            clone._useForBatch = _useForBatch;

            foreach (AttributeHierarchyMember member in this)
            {
                clone.Add((AttributeHierarchyMember)member.Clone());
            }

            return (object)clone;
        }

        #region Checks for Upserting filters
        public bool Contains(string caption)
        {
            AttributeHierarchyMember attributeHierarchyMember = new AttributeHierarchyMember(_dimensionName, _name, caption);
            if (Contains(attributeHierarchyMember))
                return true;
            return false;
        }
        public bool Contains(string from, string to)
        {
            AttributeHierarchyMember attributeHierarchyMemberRange = new AttributeHierarchyMember(_dimensionName, _name, from, to);
            if (Contains(attributeHierarchyMemberRange))
                return true;   
            return false;
        }
        #endregion
        #region Add Functions
        public AttributeHierarchyMember Add(string caption)
        {
            AttributeHierarchyMember attributeHierarchyMember;

            if (!Contains(caption))
            {
                attributeHierarchyMember = new AttributeHierarchyMember(dimensionName: _dimensionName, attributeHierarchyName: _name, caption);
                Add(attributeHierarchyMember);
            }
            else
                attributeHierarchyMember = Find(a => a.From == caption && a.MemberType == AttributeHierarchyMemberType.Single);

            return attributeHierarchyMember;
        }
        public AttributeHierarchyMember Add(string from, string to)
        {
            AttributeHierarchyMember attributeHierarchyMember;

            if (!Contains(from, to))
            {
                attributeHierarchyMember = new AttributeHierarchyMember(dimensionName: _dimensionName, attributeHierarchyName: _name, from: from, to: to);
                Add(attributeHierarchyMember);
            }
            else
                attributeHierarchyMember = Find(a => a.From == from && a.To == to && a.MemberType == AttributeHierarchyMemberType.Range);

            return attributeHierarchyMember;
        }
        #endregion
        public string ColumnName => String.Format("[{0}].[{1}].[{1}].[MEMBER_CAPTION]", _dimensionName, _name);
        public override string ToString()
        {
            return _name;
        }

    }
}