using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Threading;
using Microsoft.AnalysisServices.AdomdClient;

namespace Taiki
{
    public class MDXGenerator
    {
        public string ServerAddress;
        public string CubeName;
        public string CatalogName;
        public readonly FieldList Fields = new FieldList();
        public readonly MeasureList Measures = new MeasureList();
        private void ApplyBatchFilter(FieldList fields, AttributeHierarchyMember member)
        {
            //Find the dimension
            Dimension dimension = fields.Find(dim => dim.Name == member.DimensionName);
            //Find the attribute
            AttributeHierarchy attribute = dimension.Find(attr => attr.Name == member.AttributeHierarchyName);
            //Empty the attribute filters
            attribute.Clear();

            //Apply the filter
            attribute.Add(member);
        }
        
        public ReadOnlyCollection<MDXBatchData> Generate()
        {
            return GetBatchesMDX(0);
        }
        private ReadOnlyCollection<MDXBatchData> GetBatchesMDX(int retryAttempt)
        {
            //If the fields contain batch fields, set up everything
            if (!Fields.HasDimensionsWithBatchAttributes)
                return GetSingleMDX();

            //Clone contents so we can play with them
            MeasureList batchMeasures = (MeasureList)Measures.Clone();
            FieldList batchFields = (FieldList)Fields.Clone();

            //get batchfields and generate required calculated measures
            foreach (Dimension dim in batchFields.DimensionsWithBatchAttributes)
            {
                foreach (AttributeHierarchy attr in dim.AttributeHierarchiesUsedForBatch)
                {
                    string measureName = string.Format("{0}__{1}__Key", dim.Name.Replace(" ", "_"), attr.Name.Replace(" ", "_"));
                    CurrentMembersUniqueNameCalculatedMeasure bfm = new CurrentMembersUniqueNameCalculatedMeasure(measureName, batchMeasures.NonBatchFieldMeasures, dim, attr);
                    batchMeasures.Add(bfm);
                }
            }

            //prepare the fields for the batch query
            batchFields.HideAllButBatchFields();

            //create batches mdx
            string mdx = GenerateMDX(batchFields, batchMeasures);

            List<MDXBatchData> batches = new List<MDXBatchData>();

            try
            {
                //Extract the batches
                using (AdomdConnection con = new AdomdConnection(String.Format("Data Source={0};Catalog={1}", ServerAddress, CatalogName)))
                {
                    con.Open();

                    AdomdCommand cmd = con.CreateCommand();
                    cmd.CommandText = mdx;

                    AdomdDataReader reader = cmd.ExecuteReader();
                    
                    //Get the ordinals for the measures once
                    foreach(IMeasure measure in batchMeasures)
                    {
                        measure.Ordinal = reader.GetOrdinal(measure.ColumnName);
                    }
                    int id = 1;
                    while (reader.Read())
                    {
                        bool allMeasuresNull = true;
                        //Ignore records that contains all its measures as null...
                        foreach(IMeasure measure in batchMeasures)
                        {
                            if (!reader.IsDBNull(measure.Ordinal))
                            {
                                allMeasuresNull = false;
                                break;
                            }
                        }

                        if (allMeasuresNull)
                            continue;

                        //List containing Attribute Members Values used for batch
                        List<AttributeHierarchyMember> attributeMemberValuesCurrentBatch = new List<AttributeHierarchyMember>();

                        //clone measures and fields so we can create each of the mdx for the batches
                        MeasureList measures = (MeasureList)Measures.Clone();
                        FieldList fields = (FieldList)Fields.Clone();
                    
                        //Get the calculated measure values to be used for the batches
                        foreach(IMeasure calculatedMeasure in batchMeasures.BatchFieldMeasures)
                        {
                            string batchMemberUniqueName = reader.IsDBNull(calculatedMeasure.Ordinal) ? null : reader.GetString(calculatedMeasure.Ordinal);
                            AttributeHierarchyMember attributeHierarchyMemberBatch = new AttributeHierarchyMember(batchMemberUniqueName, AttributeHierarchyMemberType.Single);
                            attributeMemberValuesCurrentBatch.Add(attributeHierarchyMemberBatch);
                            ApplyBatchFilter(fields, attributeHierarchyMemberBatch);
                        }

                        mdx = GenerateMDX(fields, measures);
                        MDXBatchData batch = new MDXBatchData(id, mdx, attributeMemberValuesCurrentBatch.AsReadOnly());
                        batches.Add(batch);
                        id++;
                    }
                    reader.Close();
                    con.Close();
                }
            }
            catch (Exception e)
            {
                if (retryAttempt > 10)
                    throw new Exception(String.Format("Could not generate MDX. Exception \"{1}\" thrown. Message: \"{0}\"", e.GetType().ToString(), e.Message));
                
                Thread.Sleep(10000);
                return GetBatchesMDX(++retryAttempt);
            }
            return batches.AsReadOnly();
        }
        private ReadOnlyCollection<MDXBatchData> GetSingleMDX()
        {
            string mdx = GenerateMDX(Fields, Measures);
            MDXBatchData batch = new MDXBatchData(1, mdx, null);
            List<MDXBatchData> batches = new List<MDXBatchData>();
            batches.Add(batch);
            return batches.AsReadOnly();
        }
        private string GenerateMDX(FieldList fields, MeasureList measures)
        {
            StringBuilder sb = new StringBuilder();
            TabHelper tabulator =  new TabHelper();
            CommaHelper comma = new CommaHelper();
            CommaHelper asterisk = new CommaHelper("*");
            CommaHelper innerComma = new CommaHelper();  

            if (measures.HasCalculatedMeasures)
            {
                sb.AppendLine("WITH");
                tabulator.Inc();
                foreach(IMeasure calculatedMeasure in measures.CalculatedMeasures)
                {
                    sb.Append(tabulator.Get()); sb.AppendFormat("MEMBER [Measures].[{0}] AS {1}\n", calculatedMeasure.Name, calculatedMeasure.Calculation);
                }
            }

            sb.AppendLine("SELECT");
            sb.AppendLine("NON EMPTY");
            sb.AppendLine("{");
            
            tabulator.Inc(); comma.Reset();

            foreach(IMeasure measure in measures)
            {
                sb.Append(tabulator.Get()); sb.Append(comma.Get()); sb.AppendFormat("[Measures].[{0}]\n", measure.Name);
            }

            tabulator.Dec();
            
            sb.AppendLine("} ON COLUMNS");

            if (fields.HasDimensionsToShow)
            {
                sb.AppendLine(",NON EMPTY");
                sb.AppendLine("{");
                
                tabulator.Inc(); asterisk.Reset();
                
                foreach(Dimension dimension in fields.DimensionsToShow)
                {
                    foreach(AttributeHierarchy attribute in dimension.AttributeHierarchiesToIncludeInRows)
                    {
                        sb.Append(tabulator.Get()); sb.Append(asterisk.Get()); sb.AppendFormat("[{0}].[{1}].[{1}].ALLMEMBERS\n", dimension.Name, attribute.Name);              
                    }
                }
                
                tabulator.Dec();
                
                sb.AppendLine("} ON ROWS");
            }
            
            if (fields.HasDimensionsWithNestedFilters)
            {
                sb.AppendLine("FROM");
                sb.AppendLine("(");
                tabulator.Inc(); comma.Reset();

                sb.Append(tabulator.Get()); sb.AppendLine("SELECT");
                sb.Append(tabulator.Get()); sb.AppendLine("(");

                tabulator.Inc();

                foreach(Dimension dim in fields.DimensionsWithNestedFilters)
                {
                    foreach(AttributeHierarchy attr in dim.AttributeHierarchiesWithMembersToIncludeInNestedFromClause)
                    {
                        sb.Append(tabulator.Get()); sb.Append(comma.Get()); sb.AppendLine("{");
                        tabulator.Inc(); innerComma.Reset();
                        
                        foreach (AttributeHierarchyMember filterValue in attr)
                        {
                            sb.Append(tabulator.Get());
                            sb.Append(innerComma.Get());
                            if (attr.ExcludeMembers)
                                sb.Append("-");
                            sb.AppendLine(filterValue.UniqueName);
                        }
                        
                        tabulator.Dec();
                        sb.Append(tabulator.Get()); sb.AppendLine("}");
                    }
                }
                sb.Append(tabulator.Get()); sb.AppendLine(") ON COLUMNS");
                sb.Append(tabulator.Get()); sb.AppendFormat("FROM [{0}]\n", CubeName);
                tabulator.Dec();
                sb.AppendLine(")");           
            }
            else
            {
                sb.AppendFormat("FROM [{0}]\n", CubeName);
            }

            if (fields.HasDimensionsWithWhereFilters)
            {
                sb.AppendLine("WHERE");
                sb.AppendLine("(");
                tabulator.Inc(); comma.Reset();

                foreach(Dimension dim in fields.DimensionWithWhereFilters)
                {
                    foreach(AttributeHierarchy attr in dim.AttributeHierarchiesWithMembersToIncludeInWhereClause)
                    {
                        sb.Append(tabulator.Get()); sb.Append(comma.Get()); 
                        if (attr.ExcludeMembers)
                            sb.Append("-");
                        sb.AppendLine("{");
                        tabulator.Inc(); innerComma.Reset();
                        
                        foreach (AttributeHierarchyMember filterValue in attr)
                        {
                            sb.Append(tabulator.Get());
                            sb.Append(innerComma.Get());
                            sb.AppendLine(filterValue.UniqueName);
                        }
                        
                        tabulator.Dec();
                        sb.Append(tabulator.Get()); sb.AppendLine("}");
                    }
                }
                tabulator.Dec();
                sb.AppendLine(")");
            }

            return sb.ToString();
        }
        
    }
}