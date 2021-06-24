using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Taiki
{
    public class FieldList : List<Dimension>, ICloneable
    {
        public bool HasDimensionsToShow
        {
            get
            {
                bool result=false;
                foreach(Dimension dimension in this)
                {
                    if (dimension.AttributeHierarchiesToIncludeInRows.Count > 0)
                    {
                        result = true;
                        break;
                    }
                }
                return result;
            }
        }

        public ReadOnlyCollection<Dimension> DimensionsToShow
        {
            get => this.FindAll(dimension => dimension.AttributeHierarchiesToIncludeInRows.Count > 0).AsReadOnly();
        }
        public ReadOnlyCollection<Dimension> DimensionsWithBatchAttributes
        {
            get => this.FindAll(dimension => dimension.AttributeHierarchiesUsedForBatch.Count > 0).AsReadOnly();
        }
        public ReadOnlyCollection<Dimension> DimensionsWithNestedFilters
        {
            get => this.FindAll(dimension => dimension.AttributeHierarchiesWithMembersToIncludeInNestedFromClause.Count > 0).AsReadOnly();
        }
        public ReadOnlyCollection<Dimension> DimensionWithWhereFilters
        {
            get => this.FindAll(dimension => dimension.AttributeHierarchiesWithMembersToIncludeInWhereClause.Count > 0).AsReadOnly();
        }

        public bool HasDimensionsWithNestedFilters
        {
            get
            {
                bool result=false;
                foreach(Dimension dimension in this)
                {
                    if (dimension.AttributeHierarchiesWithMembersToIncludeInNestedFromClause.Count > 0)
                    {
                        result = true;
                        break;
                    }
                }
                return result;
            }
        }
        public bool HasDimensionsWithBatchAttributes
        {
            get
            {
                bool result=false;
                foreach(Dimension dimension in this)
                {
                    if (dimension.AttributeHierarchiesUsedForBatch.Count > 0)
                    {
                        result = true;
                        break;
                    }
                }
                return result;
            }
        }
        public bool HasDimensionsWithWhereFilters
        {
            get
            {
                bool result=false;
                foreach(Dimension dimension in this)
                {
                    if (dimension.AttributeHierarchiesWithMembersToIncludeInWhereClause.Count > 0)
                    {
                        result = true;
                        break;
                    }
                }
                return result;
            }
        }

        public object Clone()
        {
            FieldList clone = new FieldList();
            foreach(Dimension dimension in this)
            {
                clone.Add((Dimension)dimension.Clone());
            }

            return (object)clone;
        }

        public void HideAllButBatchFields()
        {
            this.ForEach(dim => dim.HideAllButBatchFields());
        }

        #region Upsert helper functions
        public bool ContainsDimension(string dimensionName)
        {
            Dimension dim = new Dimension(dimensionName);

            if (this.Contains(dim))
                return true;

            return false;
        }
        #endregion
        public void Add(string dimensionName, string attributeHierarchyName)
        {
            if (!ContainsDimension(dimensionName))
                this.Add(new Dimension(dimensionName));

            Dimension dim = this.Find(dim => dim.Name == dimensionName);

            if (!dim.ContainsAttributeHierarchy(attributeHierarchyName))
                dim.Add(new AttributeHierarchy(dimensionName, attributeHierarchyName));
        }
        public void Add(string dimensionName, string attributeHierarchyName, string memberCaption)
        {
            Add(dimensionName, attributeHierarchyName);
            AttributeHierarchy attr = this.Find(dim=>dim.Name == dimensionName).Find(attr=>attr.Name == attributeHierarchyName);

            if (!attr.ContainsMemberCaption(memberCaption))
                attr.Add(new AttributeHierarchyMember(dimensionName, attributeHierarchyName, memberCaption));
        }

        public void Add(string dimensionName, string attributeHierarchyName, string memberCaptionFrom, string memberCaptionTo)
        {
            Add(dimensionName, attributeHierarchyName);
            AttributeHierarchy attr = this.Find(dim=>dim.Name == dimensionName).Find(attr=>attr.Name == attributeHierarchyName);

            if (!attr.ContainsMemberCaptionRange(memberCaptionFrom, memberCaptionTo))
                attr.Add(new AttributeHierarchyMemberRange(dimensionName, attributeHierarchyName, memberCaptionFrom, memberCaptionTo));
        }
    }
}