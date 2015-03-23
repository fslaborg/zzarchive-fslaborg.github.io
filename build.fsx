// --------------------------------------------------------------------------------------
// FAKE build script
// --------------------------------------------------------------------------------------

#load "packages/FSharp.Formatting/FSharp.Formatting.fsx"
#I "packages/Suave/lib/net40"
#I "packages/FAKE/tools/"
#r "NuGet.Core.dll"
#r "FakeLib.dll"
#r "Suave.dll"
open Fake
open Fake.FileHelper
open Fake.Git
open Fake.AssemblyInfoFile
open Fake.ReleaseNotesHelper
open System
open System.IO
open FSharp.Literate
open Suave
open Suave.Web
open Suave.Http
open Suave.Http.Files

// --------------------------------------------------------------------------------------
// Global definitions
// --------------------------------------------------------------------------------------

let githubLink = "http://github.com/fslaborg/fslaborg.github.io"
let publishBranch = "master"
let info =
  [ "project-name", "##ProjectName##"
    "project-author", "##Author##"
    "project-summary", "##Summary##"
    "project-github", githubLink
    "project-nuget", "http://nuget.org/packages/##ProjectName##" ]

// --------------------------------------------------------------------------------------
// Building documentation using F# Formatting
// --------------------------------------------------------------------------------------

// Paths with template/source/output locations
let content    = __SOURCE_DIRECTORY__ @@ "content"
let output     = __SOURCE_DIRECTORY__ @@ "output"
let files      = __SOURCE_DIRECTORY__ @@ "files"
let templates  = __SOURCE_DIRECTORY__ @@ "templates"
let formatting = __SOURCE_DIRECTORY__ @@ "packages/FSharp.Formatting/"
let docTemplate = formatting @@ "templates/docpage.cshtml"

// Copy static files and CSS + JS from F# Formatting
let copyFiles () =
  CopyRecursive files output true |> Log "Copying file: "
  ensureDirectory (output @@ "content")
  CopyRecursive (formatting @@ "styles") (output @@ "content") true 
  |> Log "Copying styles and scripts: "

let references =
  if isMono then
    // Workaround compiler errors in Razor-ViewEngine
    let d = RazorEngine.Compilation.ReferenceResolver.UseCurrentAssembliesReferenceResolver()
    let loadedList = d.GetReferences () |> Seq.map (fun r -> r.GetFile()) |> Seq.cache
    // We replace the list and add required items manually as mcs doesn't like duplicates...
    let getItem name = loadedList |> Seq.find (fun l -> l.Contains name)
    [ (getItem "FSharp.Core").Replace("4.3.0.0", "4.3.1.0")
      Path.GetFullPath(__SOURCE_DIRECTORY__ @@ "packages/FSharp.Compiler.Service/lib/net40/FSharp.Compiler.Service.dll")
      Path.GetFullPath(__SOURCE_DIRECTORY__ @@ "packages/FSharp.Formatting/lib/net40/System.Web.Razor.dll")
      Path.GetFullPath(__SOURCE_DIRECTORY__ @@ "packages/FSharp.Formatting/lib/net40/RazorEngine.dll")
      Path.GetFullPath(__SOURCE_DIRECTORY__ @@ "packages/FSharp.Formatting/lib/net40/FSharp.Literate.dll")
      Path.GetFullPath(__SOURCE_DIRECTORY__ @@ "packages/FSharp.Formatting/lib/net40/FSharp.CodeFormat.dll")
      Path.GetFullPath(__SOURCE_DIRECTORY__ @@ "packages/FSharp.Formatting/lib/net40/FSharp.MetadataFormat.dll") ] |> Some
  else None

let layoutRoots =
  [ templates; formatting @@ "templates"
    formatting @@ "templates/reference" ]

// Build documentation from `fsx` and `md` files in `content`
let buildDocumentation () =
  let subdirs = Directory.EnumerateDirectories(content, "*", SearchOption.AllDirectories)
  for dir in Seq.append [content] subdirs do
    let sub = if dir.Length > content.Length then dir.Substring(content.Length + 1) else "."
    Literate.ProcessDirectory
      ( dir, docTemplate, output @@ sub, replacements = ("root", "/")::info,
        layoutRoots = layoutRoots,
        ?assemblyReferences = references,
        generateAnchors = true )

let retryCall f = 
  let rec loop n = 
    try f()
    with e ->
      if n = 0 then printfn "%A" e else 
        printfn "Waiting 1s before retrying: %d" n 
        System.Threading.Thread.Sleep(1000)
        loop (n-1)
  loop 10

// --------------------------------------------------------------------------------------
// FAKE targets for generating and releasing documentation
// --------------------------------------------------------------------------------------

Target "Clean" (fun _ ->
    CleanDirs ["output"]
)

Target "Generate" (fun _ ->
    copyFiles()
    buildDocumentation()
)

Target "Browse" (fun _ ->
  let dirActions = 
    [ content, buildDocumentation
      files, copyFiles
      templates, copyFiles >> buildDocumentation ]

  let killServer = ref None
  let startWebServer () =
    let serverConfig = { defaultConfig with homeFolder = Some output }
    let app =
        Writers.setHeader "Cache-Control" "no-cache, no-store, must-revalidate"
        >>= Writers.setHeader "Pragma" "no-cache"
        >>= Writers.setHeader "Expires" "0"
        >>= choose [ browseHome; file "index.html" ]
    let server = startWebServerAsync serverConfig app |> snd
    let tok = new System.Threading.CancellationTokenSource()
    Async.Start(server, tok.Token)
    killServer := Some(fun () -> tok.Cancel())

  let killWatchers = ref ignore
  let rec startWatching () =
    killWatchers := ignore
    for watchDir, f in dirActions do
      let watcher = new System.IO.FileSystemWatcher(watchDir, "*.*")
      watcher.EnableRaisingEvents <- true
      watcher.IncludeSubdirectories <- true
      watcher.Changed.Add(handleWatcherEvents f)
      watcher.Created.Add(handleWatcherEvents f)
      watcher.Renamed.Add(handleWatcherEvents f)
      let prevKill = killWatchers.Value
      killWatchers := fun () -> prevKill(); watcher.EnableRaisingEvents <- false; watcher.Dispose() 

  and handleWatcherEvents f (e:FileSystemEventArgs) =
    let fi = fileInfo e.FullPath 
    traceImportant <| sprintf "%s was changed." fi.Name
    killServer.Value |> Option.iter (fun kill -> kill())
    killWatchers.Value()
    retryCall f
    startWebServer()
    startWatching()

  startWebServer()
  startWatching()
  System.Diagnostics.Process.Start ("http://localhost:8083/index.html") |> ignore

  traceImportant "Waiting for site edits. Press any key to stop."
  System.Console.ReadKey() |> ignore
)

Target "Publish" (fun _ ->
  let tempDocsDir = __SOURCE_DIRECTORY__ @@ "temp/gh-pages"
  CleanDir tempDocsDir
  Repository.cloneSingleBranch "" (githubLink + ".git") publishBranch tempDocsDir

  CopyRecursive output tempDocsDir true |> tracefn "%A"
  StageAll tempDocsDir
  Git.Commit.Commit tempDocsDir (sprintf "Update site (%s)" (DateTime.Now.ToShortDateString()))
  Branches.push tempDocsDir
)

// --------------------------------------------------------------------------------------
// Regenerate all docs when publishing, by default just generate & browse
// --------------------------------------------------------------------------------------

"Clean"
  ==> "Generate"
  ==> "Publish"

"Generate" 
  ==> "Browse"

RunTargetOrDefault "Browse"
