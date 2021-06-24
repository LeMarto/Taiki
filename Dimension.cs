using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Taiki
{
    public class Dimension : List<AttributeHierarchy>,  IEquatable<Dimension>, ICloneable
    {
        #region Name Property
        private string _name;
        public string Name  => _name;
        #endregion
        #region AttributeHierarchiesToIncludeInRows Property
        /*
        Show the attribute hierarchies that actually will
        be put in the ON COLUMNS section of the MDX, which in turn
        determines that if their members should be in the nested FROM
        or in the WHERE clause
        */
        public ReadOnlyCollection<AttributeHierarchy> AttributeHierarchiesToIncludeInRows
        {
            get => this.FindAll(attr => attr.IncludeInRows).AsReadOnly();
        }
        #endregion
        #region AttributeHierarchiesWithMembersToIncludeInNestedFromClause Property
        /*
        Show the attribute hierarchies with members that need to be included 
        in the columns section, therefore they cant be put in the WHERE clause but
        on the nested FROM clause.
        */
        public ReadOnlyCollection<AttributeHierarchy> AttributeHierarchiesWithMembersToIncludeInNestedFromClause
        {
            get => this.FindAll(attr => attr.IncludeInRows).FindAll(attr => attr.Count > 0).AsReadOnly();
        }
        #endregion
        #region AttributeHierarchiesWithMembersToIncludeInWhereClause Property
        /*
        Show the attribute hierarchies with filters that do not require to be put
        on the ON COLUMNS section, they are just filters. Hence, they can
        be put on the WHERE clause.
        */
        public ReadOnlyCollection<AttributeHierarchy> AttributeHierarchiesWithMembersToIncludeInWhereClause
        {
            get => this.FindAll(attr => attr.IncludeInRows == false).FindAll(attr => attr.Count > 0).AsReadOnly();
        }
        #endregion
        #region AttributeHierarchiesUsedForBatch Property
        /*
        Show the attribute hierarchies that are to be used as batch segmentators
        */
        public ReadOnlyCollection<AttributeHierarchy> AttributeHierarchiesUsedForBatch
        {
            get => this.FindAll(attr => attr.UseForBatch == true).AsReadOnly();
        }
        #endregion
        public Dimension(string name) : base()
        {
            this._name = name;
        }
        public bool Equals(Dimension other)
        {
            if (other._name == this._name)
                return true;

            return false;
        }
        public object Clone()
        {
            Dimension clone = new Dimension(_name);
            foreach(AttributeHierarchy attribute in this)
            {
                clone.Add((AttributeHierarchy)attribute.Clone());
            }

            return (object)clone;
        }
        #region Upsert helper functions
        public bool ContainsAttributeHierarchy(string attributeHierarchyName)
        {
            AttributeHierarchy attr = new AttributeHierarchy(attributeHierarchyName, _name);

            if (this.Contains(attr))
                return true;

            return false;
        }
        #endregion
        /*
        Used to Prepare for the batch getting query
        */
        public void HideAllButBatchFields()
        {
            this.FindAll(attr => attr.UseForBatch == false).ForEach(attr => attr.IncludeInRows = false);
            this.FindAll(attr => attr.UseForBatch).ForEach(attr => attr.IncludeInRows = true);
        }
    }
}