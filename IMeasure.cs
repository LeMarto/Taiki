using System;

namespace Taiki
{
    public interface IMeasure : IEquatable<IMeasure>, ICloneable
    {
        string Name {get;}
        bool IsCalculated {get;}
        bool IsUsedForBatch {get;}
        string Calculation {get;}
        string ColumnName {get;}
        int Ordinal {get;set;}
    }
}