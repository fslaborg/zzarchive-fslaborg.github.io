(*** hide ***)
#I "../../"
(**
Getting started with FsLab
==========================

In this page, you'll learn some basic information about getting started
with FsLab. This page will not give you a lot of information about the 
FsLab libraries. For that, refer to the individual libraries (see the 
"Documentation" tab in the navigation bar). Instead, you'll learn:

 * How to create FsLab project 
 * Where to get FsLab templates
 * How to write your first script

Using FsLab templates
---------------------

The easiest way to get started is to copy one of the templates that are 
available from the [FsLab templates](http://github.com/fslaborg/FsLab.Templates)
repository. The templates work with Xamarin Studio, Visual Studio and other
F# editors. 

The templates use [Paket](http://fsprojects.github.io/Paket) for dependency 
management. When you copy one of the templates, you'll need to run `paket install`
to get the dependencies. This should happen automatically when you build the 
project (e.g. using "Ctrl+Shift+B" in Visual Studio). However, you can also
run the following from command line (drop `mono` and use backslash on Windows):

    [lang=text]
    mono .paket/paket-bootstrap.exe
    mono .paket/paket.exe install

This will download all the packages and also create `paket.lock` file with the 
version information. Once the packages are downloaded, you can open the `Tutorial.fsx`
script, which starts with something like this:
*)
#load "packages/FsLab/FsLab.fsx"
open FsLab
open Deedle
open FSharp.Data
(**
The tempalates are designed to be used from F# Interactive. Select a couple of lines
of code and use "Alt+Enter" (in Visual Studio), "Ctrl+Enter" (in Xamarin Studio) or
other command to send the code to F# Interactive. 

### FsLab Basic template

The basic FsLab template is a minimal sample containing a tutorial and Paket references
as described above. The template works with Xamarin Studio, Visual Studio or via command line.

 - Download [FsLab basic template](https://github.com/fslaborg/FsLab.Templates/archive/basic.zip)

### FsLab Journal template (Visual Studio)

The FsLab Journal template contains everything as described above, but also adds the
ability to generate HTML and LaTeX reports from your literate scripts. Currently, this
is only available for Visual Studio. Cross-platform version is coming soon!

 - Download [FsLab Journal template](https://github.com/fslaborg/FsLab.Templates/archive/journal-vs.zip)
 - Install [Visual Studio template](https://visualstudiogallery.msdn.microsoft.com/45373b36-2a4c-4b6a-b427-93c7a8effddb)

Referencing FsLab via Paket or NuGet
------------------------------------

It is equally easy to install FsLab using NuGet or using Paket. We recommend
using [Paket](http://fsprojects.github.io/Paket). This is a bit more work at
the beginning, but it will save you time when updating to new versions later!

### Referencing FsLab using NuGet

FsLab is [available on NuGet](https://www.nuget.org/packages/FsLab/) as `FsLab`.
To use it directly, create a new project (F# Tutorial is a good start in Visual
Studio and Xamarin Studio). Then add refernece to the FsLab NuGet package and
wait until all components are downloaded.

Finally add a new script file, say `Tutorial.fsx` and load `FsLab.fsx` using the
following command (the version will differ, so you'll have to check the created
`packages` directory; sometimes you also need `../packages` instead).
*)
#load "packages/FsLab.0.1.4/FsLab.fsx"
open FsLab
(**

### Referencing FsLab using Paket

When referencing FsLab using Paket, you'll need to [download the latest version of
Paket](https://github.com/fsprojects/Paket/releases/latest) and create a file called
`paket.dependencies` with the following contents:

    [lang=text]
    source https://nuget.org/api/v2

    nuget FsLab

This specifies that Paket should install the `FsLab` package for you. Then
run the following command (drop `mono` on Windows):

    [lang=text]
    mono paket.exe install

The typical usage is to put `paket.exe` into a sub-folder called `.paket`, but this
is completely up to you. Paket will drop the version numbers from folder names, so
you can reference FsLab as follows in your scripts:
*)
#load "packages/FsLab/FsLab.fsx"
open FsLab
(**

Writing FsLab scripts
---------------------

<img src="/img/carousel/cc.png" style="width:150px;float:right;margin:0px 20px 20px 20px" />

This guide is not trying to give a comprehensive overview of all the libraries you'll
find in the FsLab package, but let's have a look at least at one simple example that 
shows some of the interesting features. 

The demo shows all steps of the typical data science cycle and you'll see how FsLab
helps with all of the three steps. We'll get some data from the WorldBank using type
providers, do a simple analysis and create a little visualization.

### Accessing data with type providers

First, let's reference the libraries that we'll be using. Here, we use `FSharp.Data` for 
data access, `Deedle` for interactive data exploration and `Foogle` for visualization:
*)
open Foogle
open Deedle
open FSharp.Data
(**
Next, we'll connect to the WorldBank and access indicators for the European Union and
Czech Republic. When doing this yourself, change the names to your country and a region
or country nearby!
*)
let wb = WorldBankData.GetDataContext()
let cz = wb.Countries.``Czech Republic``.Indicators
let eu = wb.Countries.``European Union``.Indicators
(**
When using advanced F# editor (Xamarin, Visual Studio, Emacs with F# mode, etc.), you'll
get auto-completion after typing `wb.Countries.` - this is the type provider magic that 
makes it easy to access external data sources.

### Interactive data exploration

You can similarly easily find interesting indicators. For example, let's say we want to
compare university enrollment in CZ and EU. To do that, we just pick the relevant indicator
and use the `series` function to create a Deedle time-series:
*)
let czschool = series cz.``School enrollment, tertiary (% gross)``
let euschool = series eu.``School enrollment, tertiary (% gross)``
(**
When using Deedle, you can apply numerical operations to entire time-series. Here, we 
calculate the difference between CZ and EU data. Deedle automatically aligns the time-series
and matches corresponding years. We then pick the 5 years with largest differences:
*)
abs (czschool - euschool)
|> Series.sort
|> Series.rev
|> Series.take 5
(**
### Visualizing results

As a final step, we're going to create a chart that shows the two time series
side-by-side. The following example uses the Foogle chart library, which is a 
lightweight wrapper over Google chart. When used in F# Interactive, this opens
a web browser with the chart, but we can also embed it into this page:

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