(**
FsLab _Download a template or get a package_
============================================

Getting FsLab is easy! You can download a template or use Paket or NuGet package:

 - **Download a template.** Get a template from GitHub with all the setup you need
 - **NuGet package.** FsLab is available on NuGet and you can use Paket for best experience

---

<div class="row">
<div class="col-md-6">
*)
(*** hide ***)
#I "../../"
(**

Download a template
-------------------

The easiest way to get started is to copy one of the templates that are
from the [FsLab templates](http://github.com/fslaborg/FsLab.Templates)
repository. The templates work with Xamarin Studio, Visual Studio and other
F# editors.

<div style="margin:25px 0px 45px 15px">
<a href="https://github.com/fslaborg/FsLab.Templates/archive/basic.zip" class="btn btn-primary" role="button">
  <strong>FsLab basic template</strong></a>
<a href="https://github.com/fslaborg/FsLab.Templates/archive/journal-vs.zip" class="btn btn-primary" role="button">
  <strong>FsLab journal (VS 2013)</strong></a>
</div>

### Using the templates

The templates use [Paket](http://fsprojects.github.io/Paket) for dependency
management. When you copy one of the templates, you'll need to run `paket install`
to get the dependencies. This happens automatically when you build the
project (e.g. using "Ctrl+Shift+B" in Visual Studio). You can also use
the command line (drop `mono` and use backslash on Windows):

    [lang=text]
    mono .paket/paket.bootstrapper.exe
    mono .paket/paket.exe install

This downloads the packages and creates `paket.lock` file with the version information.
Once the packages are downloaded, you can open the `Tutorial.fsx` script, which starts
with something like this:
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

### FsLab Journal template (Visual Studio 2013)

The FsLab Journal template contains everything as described above, but also adds the
ability to generate HTML and LaTeX reports from your literate scripts. Currently, this
is only available for Visual Studio. Cross-platform version is coming soon!

 - Download [FsLab Journal template](https://github.com/fslaborg/FsLab.Templates/archive/journal-vs.zip)
 - Install [Visual Studio template](https://visualstudiogallery.msdn.microsoft.com/45373b36-2a4c-4b6a-b427-93c7a8effddb)

</div>
<div class="col-md-6">

Reference a package
-------------------

It is equally easy to install FsLab using NuGet or using Paket. We recommend
using [Paket](http://fsprojects.github.io/Paket). This is a bit more work at
the beginning, but it will save you time when updating to new versions later!

<div style="margin:25px 0px 45px 100px">
<a href="https://www.nuget.org/packages/FsLab/" class="btn btn-primary" role="button">
  <strong>View FsLab on NuGet</strong></a>
</div>


### Referencing FsLab using NuGet

FsLab is [available on NuGet](https://www.nuget.org/packages/FsLab/) as `FsLab`.
To use it directly, create a new project (F# Tutorial is a good start in Visual
Studio and Xamarin Studio). Then add refernece to the FsLab NuGet package and
wait until all components are downloaded.

Finally add a new script file, say `Tutorial.fsx` and load `FsLab.fsx` using the
following command (the version will differ, so you'll have to check the created
`packages` directory; when the project is in a sub-folder you'll also need
to use `../packages`).
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
This creates `paket.lock` file with the information about installed versions.
Paket makes it easy to update to the latest version of FsLab. Just run `paket update`.

</div>
</div>
*)
