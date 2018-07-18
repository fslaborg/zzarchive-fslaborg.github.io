Resources _Books, talks and articles_
=====================================

<img src="/img/report/cover.png" style="width:150px;float:right;margin:0px 5% 20px 3%" />

This page collects books, videos, recorded conference talks, articles and blog posts
about FsLab and the various libraries that are a part of the FsLab package.

 * [Books and reports](#Books-and-reports) including free O'Reilly report
 * [Videos and conference talks](#Videos-and-conference-talks) including Strata, NDC and Øredev
 * [Blog posts and articles](#Blog-posts-and-articles) about FsLab, XPlot, Deedle and more

If you did a talk or wrote an article yourself, send us [a pull request and add it](https://github.com/fslaborg/fslab.org/blob/source/content/resources/index.md)
to the list! 

***************************************************************************************************

Books and reports
-----------------

### Analyzing and Visualizing Data with F#

_Tomas Petricek, O'Reilly, 2015_

<img src="/img/report/cover.png" style="float:right;width:100px;margin:0px 30px 0px 30px" />

This report explains many of the key features of the F# language that make it a great tool 
for data science and machine learning. Real world examples take you through the entire data 
science workflow with F#, from data access and analysis to presenting the results. You'll learn about
FSharp.Data and type providers, the process of data analysis with Deedle and R type provider and
the implementation of basic machine learning algorithm with F#.

### Machine Learning Projects for .NET Developers

_Mathias Brandewinder, Apress, 2015_

<img src="/img/other/mlprojects.jpg" style="float:right;width:100px;margin:0px 30px 0px 30px" />

The book shows you how to build smarter .NET applications that learn from data, using simple 
algorithms and techniques that can be applied to a wide range of real-world problems. You’ll 
code each project in the familiar setting of Visual Studio, while the machine learning logic 
uses F#, a language ideally suited to machine learning applications in .NET. If you’re new to 
F#, this book will give you everything you need to get started. If you’re already familiar 
with F#, this is your chance to put the language into action in an exciting new context.

Videos and conference talks
---------------------------

FsLab and using F# for data science and machine learning is a frequent topic at
large developer conferences including [Strata](http://conferences.oreilly.com/strata/strataeu2014/public/schedule/detail/37395).
Here are some of the best talks that have been recorded.

### Editor's picks

 - [**Putting fun into data analysis with F#**](https://vimeo.com/144816160) (Øredev 2015)<br />
  This talk shows how to analyze social network data from Twitter looking at community dynamics and sentiment analysis.
     
 - [**F# + Machine Learning conference**](https://channel9.msdn.com/Events/FSharp-Events/fsharp-ML-MVP-Summit-2015) (Channel 9)<br />
   Two talks showing how to use FsLab together with <a href="http://www.m-brace.net/">M-Brace</a> for scalable data 
   analysis, both locally and in the cloud.

 - [**Crunching through big data with MBrace, Azure and F#**](https://vimeo.com/131637364) (NDC)<br />
   This talk shows how to scale data analytics using the MBrace project, building sample applications for digit recognition and analysis of news.

### Other great talks

 - [**F# Type Providers and the R Programming Language**](https://vimeo.com/49045879) (NYC F# meetup) <br />
   A presentation about the R type provider by it's author Howard Mansell. The talk introduces type providers and show the R type provider in action.
   
 - [**How Machine Learning Helps Cancer Research**](https://vimeo.com/144989925) (Øredev)<br />
   Introduction to machine learning and how it is used by cancer researchers. The talk is more about machine learning itself, but
   includes some nice FsLab demos!   

 - [**The F# Path to Data Scripting Nirvana**](http://channel9.msdn.com/Events/dotnetConf/2015/The-F-Path-to-Data-Scripting-Nirvana) (Channel 9)<br />
   This dotNetConf talk shows how to use FsLab together with <a href="http://www.m-brace.net/">M-Brace</a> for scalable 
   data analysis, both locally and in the cloud.

 - [**Understanding the World with F#**](http://channel9.msdn.com/posts/Understanding-the-World-with-F) (Channel 9)<br />
   This lecture shows how to access JSON, CSV and World Bank data, combine and analyze the data and visualize the results.

 - [**F# and Machine Learning: a winning combination**](https://vimeo.com/97514517) (NDC)<br />
   Introduction to machine learning with F# and FsLab libraries such as FSharp.Data type providers for easy data access.
      
 - [**Mona Lisa, F# and Azure: simple solutions to hard problems**](https://vimeo.com/113597999) (NDC)<br />
   In this talk, we will explore one of the latter, the Mona Lisa Travelling Salesman Problem, and how we used modern tools to tackle it

 - [**How F# Learned to Stop Worrying and Love the Data**](http://channel9.msdn.com/posts/Tomas-Petricek-How-F-Learned-to-Stop-Worrying-and-Love-the-Data) (Channel 9)<br />
   This lecture shows the wide range of type providers for data access. It also shows early 
   work on building visualizations with [FunScript](http://funscript.info).

 - [**Doing data science with F#**](https://vimeo.com/111289053) (Øredev 2014)<br />
   A conference talk that shows additional FsLab examples, including the HTML and
   LaTeX report generation from your source code.


Blog posts and articles
-----------------------

There is an amazing number of great (and also fun) articles and blog posts that demonstrate the power of FsLab.
If you wrote an article yourself, send us [a pull request and add it](https://github.com/fslaborg/fslab.org/blob/source/content/resources/index.md)! 

### Editor's picks

 - [**F# tackles James Bond**](http://evelinag.com/blog/2015/11-18-f-tackles-james-bond/) (Evelina Gabasova)<br />
    The blog post analyzes budget, box office and ratings for James Bond movies and offers an interesting comparison with R.

 - [**Visualizing interesting world facts with FsLab**](http://tomasp.net/blog/2015/fslab-world-visualization) (Tomas Petricek)<br />
   This article uses the WorldBank type provider and XPlot visualization library to discover interesting things about the world.
   
 - [**JournalでReproducible Research(レポート)を簡単に作りたい**](http://d.hatena.ne.jp/teramonagi/20141217/1418766536)
   ([@teramonagi](https://twitter.com/teramonagi))<br />
   この記事ではF#でReproducible Researchを行うための環境としてFsLab Jounalというテンプレートを使うという方法を紹介しました。
   「新しい項目」を追加する際には「FsLab Tutorial(using R)」なんてのもあり、RTypeProviderを用いてF#とRを連携させる方法についても簡単に試すことができます。

### Other great articles

 - [**Christmas Carol and other eigenvectors**](http://evelinag.com/blog/2014/12-15-christmas-carol-and-other-eigenvectors/) (Evelina Gabasova) <br />
  Do you know who wrote the classic Christmas story, 'A Christmas Carol'? This F# analysis proves that it was Charles Dickens!

 - [**F# Neural Networks with FsLab**](https://sergeytihon.wordpress.com/2013/11/18/f-neural-networks-with-rprovider-deedle/) (Sergey Tihon)<br />
   Neural networks are very powerful tool and at the same time, it is not easy to use all its power. But with R provider and Deedle,
   you can now easily use neural networks from F#.

 - [**Who's the most central? F# network on Twitter**](http://evelinag.com/blog/2014/05-12-analyzing-f-network-on-twitter/) (Evelina Gabasova)<br />
  A great introduction to social network analysis using the FsLab tools, looking at the network around the F# community.

 - [**Data Visualization with Plotly**](http://fsharp-code.blogspot.co.uk/2015/06/data-visualization-with-plotly.html) (Taha Hachana) <br />
   A quick introduction showing how to create different charts using the Plot.ly wrapper in the XPlot visualization library.
  
 - [**XPlot ‘hello world’ – Using Google Charts**](http://blog.leifbattermann.de/2015/11/07/xplot-hello-world/) (Leif Battermann) <br />
   A demo showing the Google Charts API of XPlot together with a nice way of integrating charts in .NET web applications.
