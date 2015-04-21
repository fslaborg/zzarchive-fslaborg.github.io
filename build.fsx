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
open FSharp.Markdown
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
  [ "project-name", "N/A"; "project-author", "N/A"; "project-summary", "N/A"; 
    "project-github", githubLink; "project-nuget", "N/A" ]

// --------------------------------------------------------------------------------------
// Building documentation using F# Formatting
// --------------------------------------------------------------------------------------

// Create FSI evaluator for handling FsLab docs
#load "FsiMock.fs"
#load "packages/FsLab/FsLab.fsx"
#load "Formatters.fs"
let fsiEvaluator1 = FsiEvaluator() 
let fsiEvaluator = FsLab.Formatters.wrapFsiEvaluator fsiEvaluator1 "." (System.IO.Path.Combine(__SOURCE_DIRECTORY__,"output")) "G4"
fsiEvaluator1.EvaluationFailed.Add(fun e ->
  traceImportant <| sprintf "Evaluation failed: %s" e.StdErr
)

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

let customizeDocument (ctx:ProcessingContext) (doc:LiterateDocument) =
  // Find the first <hr /> element and treat the part before as a banner
  let pars = doc.Paragraphs |> Array.ofSeq
  let optBreak = pars |> Array.tryFindIndex (function HorizontalRule _ -> true | _ -> false)
  let banner, rest = 
    match optBreak with 
    | None -> [||], pars
    | Some idx -> pars.[.. idx-1], pars.[idx+1 ..]
  let isSpecialPage = match rest.[0] with InlineBlock _ -> true | _ -> false
  let isIndexPage = doc.SourceFile = Path.GetFullPath(content @@ "index.md")

  let newPars = 
    [ // On index page, we keep everything unchanged
      if isIndexPage then yield! pars else

      // If the page has banner, then emit banner inside .banner-wrapper
      if banner <> [||] then
        yield InlineBlock """<div class="first-wrapper banner-spaced banner-wrapper"><div class="container">"""
        yield! banner
        yield InlineBlock "</div></div>"
        yield InlineBlock """<div class="content-wrapper"><div class="container">"""
      else
        yield InlineBlock """<div class="first-wrapper content-wrapper"><div class="container">"""
      
      if isSpecialPage then
        // If the page starts with HTML, it needs to define its own rows/columns
        yield! rest
      else 
        // By default, we create content blokc and add right panel in another column
        yield InlineBlock """<div class="row"><div class="col-md-9">"""
        yield! rest
        yield InlineBlock """</div><div class="col-md-3">"""
        yield InlineBlock (File.ReadAllText(templates @@ "rightpanel.html"))
        yield InlineBlock "</div></div>"

      yield InlineBlock "</div></div>" ]
  doc.With(newPars)

// Build documentation from `fsx` and `md` files in `content`
let buildDocumentation () =
  Literate.ProcessDirectory
    ( content, docTemplate, output, replacements = ("root", "/")::info,
      layoutRoots = layoutRoots, ?assemblyReferences = references,
      generateAnchors = true, processRecursive = true, 
      fsiEvaluator = fsiEvaluator, customizeDocument = customizeDocument )

let retryCall f = 
  let rec loop n = 
    try f()
    with e ->
      if n = 0 then printfn "%A" e else 
        printfn "Error: %s" e.Message
        printfn "Waiting 1s before retrying: %d" n 
        System.Threading.Thread.Sleep(1000)
        loop (n-1)
  loop 10

// --------------------------------------------------------------------------------------
// FAKE targets for generating and releasing documentation
// --------------------------------------------------------------------------------------

Target "Clean" (fun _ ->
    CleanDirs [__SOURCE_DIRECTORY__ @@ "output"]
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
