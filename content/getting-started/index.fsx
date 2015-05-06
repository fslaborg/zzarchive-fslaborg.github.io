(*** hide ***)
#I "../../"
(**
Tutorial _Getting started with FsLab_
=====================================

<img src="/img/carousel/cc.png" style="width:180px;float:right;margin:0px 5% 20px 3%" />

In this tutorial, you'll learn how to get started with FsLab. This walkthrough will not give
you detailed information about the FsLab libraries. For that, refer to the individual libraries
(see the "Documentation" tab in the navigation bar), but it quickly demonstrates how FsLab
simplifies the three phases of a typical data science workflow:

 * **Access data** in a safe and easy way with type providers
 * **Analyze data** using powerful series and data frame libraries
 * **Visualize results** produce elegant charts and reports

***************************************************************************************************

Introduction
------------

The demo shows all the typical steps of a data science cycle and you'll see how FsLab
helps with each of them. The example compares university enrollment in the
European Union and the Czech Republic - we'll start by getting data about the countries
from the World Bank, then we'll do a simple exploratory data analysis and we'll
finish with a little visualization.

Accessing data with type providers
----------------------------------

First, you need to [download the FsLab template or package](/download). Then, we
reference the libraries that we need. Here, we use `FSharp.Data` for data access,
`Deedle` for interactive data exploration and `Foogle` for visualization:
*)
#load "packages/FsLab/FsLab.fsx"

open Foogle
open Deedle
open FSharp.Data
(**
Next, we connect to the World Bank and access the indicators for the European Union and
Czech Republic. When doing this yourself, change the names to your country and a region
or country nearby!
*)
let wb = WorldBankData.GetDataContext()
let cz = wb.Countries.``Czech Republic``.Indicators
let eu = wb.Countries.``European Union``.Indicators
(**
When using advanced F# editor (Xamarin, Visual Studio, Emacs with F# mode etc.), 
you'll get auto-completion after typing `wb.Countries.` - this is the type provider magic that
makes it easy to access external data sources.

Interactive data exploration
----------------------------

Just like we can easily find countries and regions, we can easily get interesting indicators
about them. To compare university enrollment in Czech Republic and European Union, we just pick
the relevant indicator and use the `series` function to create a Deedle time-series:
*)
let czschool = series cz.``School enrollment, tertiary (% gross)``
let euschool = series eu.``School enrollment, tertiary (% gross)``
(**
When using Deedle, you can apply numerical operations to an entire time-series. Here, we
calculate the difference between CZ and EU data. Deedle automatically aligns the time-series
and matches corresponding years, so you do not have to worry about aligning data from multiple
sources. We then pick the 5 years with largest differences:
*)
(*** define-output:diffs ***)
abs (czschool - euschool)
|> Series.sort
|> Series.rev
|> Series.take 5
(*** include-it:diffs ***)
(**

With the FsLab journal template, you can easily embed the results of a computation into a
report. In fact, this page has been generated using exactly that mechanism!

Visualizing results
-------------------

As a final step, we're going to create a chart that shows the two time series
side-by-side. The following example uses the Foogle chart library, which is a
lightweight wrapper over Google chart. When used in F# Interactive, this opens
a web browser with the chart, but we can also embed it into this page, just like
the table above:

*)
Chart.LineChart
 ([ for y in 1985 .. 2012 ->
     string y,
       [ cz.``School enrollment, tertiary (% gross)``.[y]
         eu.``School enrollment, tertiary (% gross)``.[y] ] ],
  Labels = ["CZ"; "EU"])
(**

<div id="chart_div" style="height:300px; margin:0px 60px 0px 20px;"></div>
<script type="text/javascript">
  function main() {
    var data = google.visualization.arrayToDataTable([["","CZ","EU"],["1985",16.4573,25.3308696746826],["1986",16.19277,25.7687797546387],["1987",16.39002,26.0407600402832],["1988",16.35163,26.5969200134277],["1989",16.16396,27.5715198516846],["1990",16.05048,28.6432704925537],["1991",16.03141,30.3707008361816],["1992",14.63799,32.1341018676758],["1993",14.51548,34.8727416992188],["1994",19.60312,38.0942115783691],["1995",20.78567,40.2029304504395],["1996",21.66652,42.3797988891602],["1997",23.44813,44.7898597717285],["1998",23.88066,46.1005706787109],["1999",25.56303,48.35546875],["2000",28.42699,50.0173797607422],["2001",30.07788,52.1987495422363],["2002",34.60981,54.0809707641602],["2003",37.30648,56.0009613037109],["2004",44.20706,57.5373191833496],["2005",48.90877,59.0155181884766],["2006",50.64701,60.1520118713379],["2007",54.48956,60.788818359375],["2008",58.05345,61.6428489685059],["2009",60.6826,62.8722610473633],["2010",63.21222,64.3464126586914],["2011",64.58364,65.3488388061523],["2012",64.17338,66.5549621582031]]);
    var options = {"curveType":"none","backgroundColor": { fill:'transparent' }};
    var chart = new google.visualization.LineChart(document.getElementById('chart_div'));
    chart.draw(data, options);
  }
  google.load('visualization', '1', { 'packages': ['corechart','geochart'] });
  google.setOnLoadCallback(main);
</script>

Summary
-------

This short article demonstrated how to get started with FsLab and we also looked
at a demo that shows how FsLab simplifies the three tasks of working with data.

 - Type providers make it easier to access data and help you avoid issues
   by integrating external data (like the World Bank) into the language and
   into your editor.

 - The Deedle library provides rich and easy-to-use tools for interactive data
   exploration using data frame, series and time-series (and it can also integrate
   with R).

 - FsLab comes with visualization libraries that you can use to produce elegant
   HTML or LaTeX output.
*)
