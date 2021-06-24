using System.Collections.ObjectModel;

namespace Taiki
{
    public class ExtractionBatch
    {
        public readonly int Id;
        public readonly string MDX;
        public readonly string ServerAddress;
        public readonly string CubeName;
        public readonly string CatalogName;
        #region Members Property
        public ReadOnlyCollection<AttributeHierarchyMember> Members => _members;
        private ReadOnlyCollection<AttributeHierarchyMember> _members;
        #endregion
        public ExtractionBatch(int id, string mdx, string serverAddress, string cubeName, string catalogName, ReadOnlyCollection<AttributeHierarchyMember> members)
        {
            Id = id;
            MDX = mdx;
            _members = members;
            ServerAddress = serverAddress;
            CubeName = cubeName;
            CatalogName = catalogName;
        }
    }
}