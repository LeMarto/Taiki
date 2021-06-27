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

        public bool Contains(string dimensionName) 
        {
            Dimension dimension = Find(d => d.Equals(dimensionName));
            return dimension != null;
        }
        #endregion
        public Dimension Add(string dimensionName)
        {
            Dimension dimension;

            if (!Contains(dimensionName))
            {
                dimension = new Dimension(dimensionName);
                Add(dimension);
            }
            else
                dimension = Find(d=> d.Equals(dimensionName));

            return dimension;
        }
    }
}