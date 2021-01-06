dotnet --version
dotnet build build\Myra.Monogame.sln /p:Configuration=Release --no-incremental
dotnet build build\Myra.Stride.sln /p:Configuration=Release --no-incremental
dotnet build build\Myra.PlatformAgnostic.sln /p:Configuration=Release --no-incremental