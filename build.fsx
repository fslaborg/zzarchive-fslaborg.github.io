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
let fsiEvaluator1 = FsiEvaluator(fsiObj = FsiEvaluatorConfig.CreateNoOpFsiObject())
let fsiEvaluator = 
  FsLab.Formatters.wrapFsiEvaluator fsiEvaluator1 "." (__SOURCE_DIRECTORY__ @@ "output") "G4"
fsiEvaluator1.EvaluationFailed.Add(fun e ->
  traceImportant <| sprintf "Evaluation failed: %s" e.StdErr )

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

let layoutRoots =
  [ templates; formatting @@ "templates"
    formatting @@ "templates/reference" ]

// --------------------------------------------------------------------------------------
// Document pre-processing
// --------------------------------------------------------------------------------------

/// Format MarkdownSpans as plain text dropping basic formatting
let rec (|AsPlainText|) items =
  items |> List.map (function
    | Literal text 
    | DirectLink(AsPlainText text, _) 
    | Emphasis(AsPlainText text)
    | Strong(AsPlainText text) -> text.Replace('\n', ' ').Replace('\r', ' ')
    | _ -> "")
  |> String.concat " "
  

/// Matches a list with twitter meta tags 
let (|TwitterMetaList|_|) = function
  | ListBlock(_, items) ->
      let parsedItems = items |> List.map (function 
        | [Span(AsPlainText text)] when text.StartsWith("twitter:") -> 
            let space = text.IndexOf(" ")
            if space > 0 then Some(text.[0 .. space-1], text.[space+1 .. ])
            else None
        | _ -> None ) 
      if parsedItems |> List.forall (function Some _ -> true | _ -> false) then
        Some(List.map Option.get parsedItems)
      else None
  | _ -> None


/// Transforms the document by doing the following:
///
///  - drop twitter meta tags 
///  - if it is the index page, do nothing else
///  - if it has a banner, wrap banner with DIVs
///  - if it is not a special page, insert right panel
///    (special pages start with HTML to define columns)
///
let customizeDocument (ctx:ProcessingContext) (doc:LiterateDocument) =
  let pars = doc.Paragraphs |> Array.ofSeq
  let pars = pars |> Array.filter (function TwitterMetaList _ -> false | _ -> true)

  // Find the first <hr /> element and treat the part before as a banner
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


// --------------------------------------------------------------------------------------
// Building documentation
// --------------------------------------------------------------------------------------

// Build documentation from `fsx` and `md` files in `content`
let buildDocumentation () =

  // Extract twitter meta tags from the document and insert it as
  // a special 'twitter-meta' key in the page properties
  let getMetaReplacement paragraphs =
    let metaOpt = paragraphs |> Seq.tryPick (function 
      TwitterMetaList kvps -> Some kvps | _ -> None)
    match metaOpt with
    | Some kvps ->
        let html = 
          kvps 
          |> List.map (fun (k, v) -> sprintf "<meta name=\"%s\" content=\"%s\" />" (k.Trim()) (v.Trim()))
          |> String.concat "\n"
        ["twitter-meta", html]
    | _ -> []

  // Call one or the other process function with all the arguments
  let processScriptFile file output = 
    let doc = Literate.ParseScriptFile(file)
    let meta = getMetaReplacement doc.Paragraphs
    Literate.ProcessScriptFile
      ( file, docTemplate, output, fsiEvaluator = fsiEvaluator, 
        replacements = ("root", "/")::(meta @ info), layoutRoots = layoutRoots, 
        generateAnchors = true, customizeDocument = customizeDocument )

  let processMarkdown file output = 
    let doc = Literate.ParseMarkdownFile(file)
    let meta = getMetaReplacement doc.Paragraphs
    Literate.ProcessMarkdown
      ( file, docTemplate, output, replacements = ("root", "/")::(meta @ info),
        layoutRoots = layoutRoots, generateAnchors = true, customizeDocument = customizeDocument )
    
  /// Recursively process all files in the directory tree
  let rec processDirectory indir outdir = 
    // Create output directory if it does not exist
    if Directory.Exists(outdir) |> not then
      try Directory.CreateDirectory(outdir) |> ignore 
      with _ -> failwithf "Cannot create directory '%s'" outdir

    let fsx = [ for f in Directory.GetFiles(indir, "*.fsx") -> processScriptFile, f ]
    let mds = [ for f in Directory.GetFiles(indir, "*.md") -> processMarkdown, f ]
    for func, file in fsx @ mds do
      let dir = Path.GetDirectoryName(file)
      let name = Path.GetFileNameWithoutExtension(file)
      let output = Path.Combine(outdir, sprintf "%s.html" name)

      // Update only when needed
      let changeTime = File.GetLastWriteTime(file)
      let generateTime = File.GetLastWriteTime(output)
      if changeTime > generateTime then
        printfn "Generating '%s/%s.html'" dir name
        func file output
    for d in Directory.EnumerateDirectories(indir) do
      let name = Path.GetFileName(d)
      processDirectory (Path.Combine(indir, name)) (Path.Combine(outdir, name))

  processDirectory content output


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
  System.Console.ReadLine() |> ignore
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
