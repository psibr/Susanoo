set PATH=C:\Program Files (x86)\MSBuild\14.0\Bin;%PATH%

dotnet restore


msbuild


dotnet test test/Susanoo.Core.Tests

dotnet test test/Susanoo.Transforms.Tests

dotnet test test/Susanoo.Transforms.SqlServer.Tests


dotnet pack src\Susanoo.Core --output artifacts

dotnet pack src\Susanoo.Transforms --output artifacts

dotnet pack src\Susanoo.Transforms.SqlServer --output artifacts

dotnet pack src\Susanoo.DependencyInjection.AutoFac --output artifacts

dotnet pack src\Susanoo.DependencyInjection.StructureMap --output artifacts