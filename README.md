# Taiki
A .Net 5.0 library to simplify the generation of MDX for batch or single extraction from a MS Analysis Services cube.

## Motivation
The main use case that motivated the creation of this library was needing to extract information from an Microsoft
SSAS multidimensional cube from a server that had **very small** timeout windows set up. As I needed to extract lots
of data, I realized that instead of extracting one big chunk of data, I would require to extract multiple smaller 
batches. The creation of these batches became very tedious and error prone, hence I created this library to assist me
in creating the MDX query for each of the batches automatically. The library also helps in creating "single batch" mdx as well.

## Example
If we wanted to extract from adventureworks the internet sales amount by year by quarter (one batch per year) 
we would need to create 2 separate mdxs:

One for 2005:
```mdx
SELECT
NON EMPTY
{
	[Measures].[Order Count]
} ON COLUMNS

,NON EMPTY
(
	[Delivery Date].[Calendar Year].[Calendar Year].ALLMEMBERS *
	[Delivery Date].[Calendar Quarter of Year].[Calendar Quarter of Year].ALLMEMBERS
) ON ROWS

FROM 
(
	SELECT
	(

		{
		[Delivery Date].[Calendar Year].&[2005]
		}

	) ON COLUMNS
	FROM [Adventure Works]
)
WHERE 
(
	{[Sales Channel].[Sales Channel].&[Internet]}
)
```
One for 2006
```mdx
SELECT
NON EMPTY
{
	[Measures].[Order Count]
} ON COLUMNS

,NON EMPTY
(
	[Delivery Date].[Calendar Year].[Calendar Year].ALLMEMBERS *
	[Delivery Date].[Calendar Quarter of Year].[Calendar Quarter of Year].ALLMEMBERS
) ON ROWS

FROM 
(
	SELECT
	(

		{
		[Delivery Date].[Calendar Year].&[2006]
		}

	) ON COLUMNS
	FROM [Adventure Works]
)
WHERE 
(
	{[Sales Channel].[Sales Channel].&[Internet]}
)
```
Now, take into consideration this is a very simple example. These mdx could be programatically generated like so:
```C#
MDXGenerator ex = new MDXGenerator();

//We want to extract from Adventure Works
ex.CubeName = "Adventure Works";
ex.CatalogName = "Adventure Works";
ex.ServerAddress = "localhost";

//We want to extract the sales amount in the columns
ex.Measures.Add("Sales Amount");

/*
From the delivery date dimension we want to include the calendar year (but ONLY 2005 and 2006) in the rows
(Notice the sort of "functional" notation to simplify the addition of dimension->attribute hierarchy-> attribute hierarchy member)
Notice also, we are setting the calendar year as a batch field. That means that whatever data comes from the mdx, it will be segmented at least
one file per value of that attribute.
*/
ex.Fields.Add("Delivery Date").Add("Calendar Year").AsBatch().Add("2005");
ex.Fields.Add("Delivery Date").Add("Calendar Year").Add("2006");
/*
We also want to include the Calendar Quarter of year as well 
*/
ex.Fields.Add("Delivery Date").Add("Calendar Quarter of Year");

/*
Then, we want only the internet channel sales, so we add them with the 
NotInRows() method, as even though we want to filter by it, we dont want it
in the columns...
*/
ex.Fields.Add("Sales Channel").Add("Sales Channel").NotInRows().Add("Internet");

/*Finally, we generate the mdx*/
ReadOnlyCollection<MDXBatchData> batches = ex.Generate();

/*and print the MDX for each of the batches*/
foreach(MDXBatchData batch in batches)
{
    Console.WriteLine(batch.MDX);
}
```
