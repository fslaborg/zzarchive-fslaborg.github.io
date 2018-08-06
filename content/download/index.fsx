(**
FsLab _Use a template or get a package_
============================================

Getting FsLab is easy! You can download a template or use Paket or NuGet package:

- **Use a template.** Get a template from GitHub with all the setup you need
- **Get a package.** FsLab is available on NuGet and you can use Paket for best experience

---

<div class="row">
<div class="col-md-6">
*)
(*** hide ***)
#I "../../"
(**

Use a template
-------------------

First [install the .NET SDK](https://www.microsoft.com/net/download) and, if on Linux, [install Mono](https://www.mono-project.com/docs/getting-started/install/).
Then use:

<pre>
dotnet new -i FsLab.Templates
dotnet new fslab-journal -lang F# -n Experiment1
cd Experiment1
</pre>

This creates a new journal. The templates work on Linux, OSX and Windows, and you can
edit with Emacs, VSCode, Visual Studio, Vim and other F# editors.

You can now generate HTML and show it:

<pre>
build html
</pre>

Use `build run` to both show and watch for changes:

<pre>
build run
</pre>

You can now edit `Experiment1.fsx` or open `Experiment1.fsproj`.

The templates are designed to be used from F# Interactive. Select a couple of lines
of code and use "Alt+Enter" (in Visual Studio), "Ctrl+Enter" (in VSCode) or
other command to send the code to F# Interactive.

The template comes with build scripts `build.sh` and `build.cmd` that can be used
as follows:

 - `build run` generates HTML outputs, opens them in a browser and automatically
    updates them when the source files change
 - `build show` generates HTML and shows it
 - `build run` generates HTML, shows it and watches for changes
 - `build latex` generates LaTeX output
 - `build pdf` generates LaTeX and also invokes `pdflatex`

</div>
<div class="col-md-6">

### Going Deeper

The templates use [Paket](http://fsprojects.github.io/Paket) for dependency management.
When you use the templates, you'll need to run Paket to get the dependencies.
This happens automatically when you build the project, but you can also do it by hand.

 - In the basic template, run [`paket install`](http://fsprojects.github.io/Paket/paket-install.html).
   For example, when using mono on Mac, run:

        mono .paket/paket.exe install

 - In the journal template you can use build scripts `build.sh` and `build.cmd`.
   The following will process all journals and open them in a web browser:

        chmod +x build.sh
        ./build.sh run

### Referencing FsLab using NuGet

FsLab is [available on NuGet](https://www.nuget.org/packages/FsLab/) as `FsLab`.

To use it directly, create a new project (F# Tutorial is a good start in Visual
Studio and VSCode). Then add reference to the FsLab NuGet package and
wait until all components are downloaded.

Finally add a new script file, say `Tutorial.fsx` and load `FsLab.fsx` using the
following command (the version will differ, so you'll have to check the created
`packages` directory; when the project is in a sub-folder you'll also need
to use `../packages`).
*)
#load "packages/FsLab/FsLab.fsx"
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
