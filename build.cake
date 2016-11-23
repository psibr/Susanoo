var target = Argument("target", "default");
var configuration = Argument("configuration", "Release");

Task("restore")
    .Does(() =>
{
   DotNetCoreRestore();
});

Task("build")
    .IsDependentOn("restore")
    .Does(() =>
{
   MSBuild("./Susanoo.sln", settings =>
   {
       settings.SetConfiguration(configuration);
   });
});

Task("test")
    .IsDependentOn("build")
    .Does(() =>
{
    var files = GetFiles("./test/**/project.json");
    foreach(var file in files)
    {
        DotNetCoreTest(file.ToString());
    }
});

Task("default")
    .IsDependentOn("test")
    .Does(() =>
{
});

RunTarget(target);