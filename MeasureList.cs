using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Taiki
{
    public class MeasureList : List<IMeasure>, ICloneable
    {
        public bool HasCalculatedMeasures
        {
            get => this.FindAll(measure => measure.IsCalculated).Count > 0;
        }
        public ReadOnlyCollection<IMeasure> CalculatedMeasures
        {
            get => this.FindAll(measure => measure.IsCalculated).AsReadOnly();
        }
        public bool HasBatchFieldMeasures
        {
            get => this.FindAll(measure => measure.IsUsedForBatch).Count > 0;
        }
        public ReadOnlyCollection<IMeasure> BatchFieldMeasures
        {
            get => this.FindAll(measure => measure.IsUsedForBatch).AsReadOnly();
        }
        public ReadOnlyCollection<IMeasure> NonBatchFieldMeasures
        {
            get => this.FindAll(measure => measure.IsUsedForBatch == false).AsReadOnly();
        }

        public void Add(string measureName)
        {
            Measure m = new Measure(measureName);
            Add(m);
        }
        public object Clone()
        {
            MeasureList clone = new MeasureList();
            foreach (IMeasure measure in this)
            {
                clone.Add((IMeasure)measure.Clone());
            }
            return (object)clone;
        }
    }
}