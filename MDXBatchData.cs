using System.Collections.ObjectModel;

namespace Taiki
{
    public class MDXBatchData
    {
        public readonly int Id;
        public readonly string MDX;
        #region Members Property
        public ReadOnlyCollection<AttributeHierarchyMember> Members => _members;
        private ReadOnlyCollection<AttributeHierarchyMember> _members;
        #endregion
        public MDXBatchData(int id, string mdx, ReadOnlyCollection<AttributeHierarchyMember> members)
        {
            Id = id;
            MDX = mdx;
            _members = members;
        }
    }
}