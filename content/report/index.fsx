(*** hide ***)
#I "../../"
(**

 - twitter:card summary
 - twitter:creator @fslaborg
 - twitter:title O'Reilly: Analyzing and Visualizing Data with F#
 - twitter:description This O'Reilly report explains the key features of the F# language 
    that make it a great tool for data science and machine learning and it guides you
    through the entire data science workflow with FsLab.
 - twitter:image https://fslab.org/img/report/cover-square.png

O'Reilly _Analyzing and Visualizing Data with F#_
==================================================

<img src="/img/report/cover.png" style="width:150px;float:right;margin:0px 5% 20px 3%" />

This report explains many of the key features of the F# language that make it a great tool 
for data science and machine learning. Real world examples take you through the entire data 
science workflow with F#:

 -	How F# and type providers ease the chore of data access
 -	The process of data analysis and visualization, using the Deedle library, R type provider and the XPlot charting library
 -	Implementing a clustering algorithm and how the F# type inference helps you understand your code
 
The report also includes a list of resources to help you learn more about using F# for data science. 

***************************************************************************************************

Report details
--------------

The report is freely available from O'Reilly. To get it, follow the link below. This page provides
more information, errata and also the latest version of the source code.

<div style="text-align:center;padding:10px 0px 10px 0px">
<a href="http://www.oreilly.com/programming/free/analyzing-visualizing-data-f-sharp.csp" class="btn btn-primary" role="button">
  <strong>Get it from O'Reilly</strong></a>
<a href="https://github.com/fslaborg/OReilly.Report/archive/master.zip" class="btn btn-primary" role="button">
  <strong>Download source code</strong></a>
</div>

What's in the report?
---------------------

### Chapter 1: Accessing Data with Type Providers

The examples in this chapter focus on the _access_ part of the data science
workflow. In most languages, this is typically the most frustrating part of the
_access_, _analyze_, _visualize_ loop. In F#, type providers come to the rescue!

### Chapter 2: Analyzing Data using F# and Deedle

In this chapter, we look at a more realistic case study of doing data science
with F#. We use World Bank as our data source, but this time we call
it directly using the XML provider. This demonstrates a general approach that 
works with any REST-based service.

### Chapter 3: Implementing Machine Learning Algorithms

This chapter completes our brief tour by using the F# language to 
implement the k-means clustering algorithm. This illustrates two aspects
of F# that make it nice for writing algorithms: type inference and 
interactive development style.

### Chapter 4: Conclusions and Next Steps

If you want to learn more about using F# for data science and machine learning,
there are a number of excellent resources that are worth checking out now that
you have finished the quick overview from this report. This chapter gives
you a good list!

Visualization gallery
---------------------

<div class="row">
  <div class="col-md-6">
    <p>Change in CO<sub>2</sub> emissions between 2000 and 2010 across the world (Chapter 2).</p>
    <img src="/img/report/co2-change.png" class="img-responsive" />
  </div>
  <div class="col-md-6">
    <p>Is life expectancy correlated with (a logarithm of the) GDP? (Chapter 2).</p>
    <img src="/img/report/gdp-life.png" class="img-responsive" />
  </div>
</div>
<div class="row">
  <div class="col-md-6">
    <p>Looking at correlations between indicators about the world (Chapter 2).</p>
    <img src="/img/report/rplot.png" class="img-responsive" />
  </div>
  <div class="col-md-6">
    <p>Automatically clustering countries in the world based on growth indicators (Chapter 3).</p>
    <img src="/img/report/world.png" class="img-responsive" />
  </div>
</div>

*)
