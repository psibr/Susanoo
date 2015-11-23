// include Fake lib
#r "packages/FAKE/tools/FakeLib.dll"
open Fake
open Fake.Paket

// Properties
let deployDir = "./nupkgs/"
let testOutputDir = "./test-results"

// Targets

Target "Rebuild" (fun _ ->
    build (fun p ->
        {p with
            Verbosity = Some MSBuildVerbosity.Minimal
            Targets = ["Rebuild"]
            Properties =
              [
                "Configuration", "Release"
                "Optimize", "true"
              ]}) "src/Susanoo.sln"
      |> DoNothing
)

Target "RebuildMono" (fun _ ->
    build (fun p ->
        {p with
            Verbosity = Some MSBuildVerbosity.Minimal
            Targets = ["Rebuild"]
            Properties =
              [
                "Configuration", "Mono"
                "Optimize", "true"
              ]}) "src/Susanoo.sln"
      |> DoNothing
)

Target "Build" (fun _ ->
    build (fun p ->
        {p with
            Verbosity = Some MSBuildVerbosity.Minimal
            Targets = ["Build"]
            Properties = ["Configuration", "Debug"]}) "src/Susanoo.sln"
      |> DoNothing
)

// define test dlls
let MSpecDllPath = "src/**/bin/**/*MSpec*.dll"
let MSpecDlls = !! MSpecDllPath
let testDlls =
  !! "src/**/bin/**/*Tests*.dll"
    -- MSpecDllPath

Target "MSpecTest" (fun _ ->
    CleanDir testOutputDir

    MSpecDlls
        |> MSpec (fun p ->
            {p with
                ExcludeTags = ["LongRunning"]
                HtmlOutputDir = testOutputDir})
)

Target "Pack" (fun _ ->
    Pack (fun p ->
        {p with
            OutputPath = deployDir})
)

// Dependencies

"Rebuild"
  ==> "MSpecTest"
  ==> "Pack"

// start build
RunTargetOrDefault "Rebuild"
