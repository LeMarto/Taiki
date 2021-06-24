using System;
namespace Taiki
{
    public interface IAttributeHierarchyMember : IEquatable<IAttributeHierarchyMember>, ICloneable
    {
        string UniqueName {get;}
    }
}