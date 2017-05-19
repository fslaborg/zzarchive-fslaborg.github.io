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
`Deedle` for interactive data exploration and `XPlot` for visualization:
*)
#load "packages/FsLab/FsLab.fsx"

(**
Next, we connect to the World Bank and access the indicators for the European Union and
Czech Republic. When doing this yourself, change the names to your country and a region
or country nearby!
*)
open FSharp.Data

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
open Deedle

let czschool = series cz.``Gross enrolment ratio, tertiary, both sexes (%)``
let euschool = series eu.``Gross enrolment ratio, tertiary, both sexes (%)``
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
side-by-side. The following example uses the XPlot chart library which is a wrapper over GoogleCharts.
When used in F# Interactive, this opens a web page with the chart, 
but we can also embed it into this page, just like the table above:

*)
(*** define-output:chart ***)
open XPlot.GoogleCharts

Frame(["CZ"; "EU"], [czschool ; euschool])
|> Frame.filterRows (fun year s -> year >= 1985)
|> Chart.Line
|> Chart.WithLegend true
|> Chart.WithTitle "University enrollment in Czech Republic and European Union"

(*** include-it:chart ***)
(**

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
