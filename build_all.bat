dotnet --version
dotnet build build\Monogame\Myra.sln /p:Configuration=Release --no-incremental
dotnet build build\FNA\Myra.sln /p:Configuration=Release --no-incremental
rem dotnet pack src\Myra\Myra.csproj -Version %APPVEYOR_BUILD_VERSION% --no-incremental
rem dotnet pack Myra\Myra.FNA.nuspec -Version %APPVEYOR_BUILD_VERSION% --no-incremental
rem copy_zip_package_files.bat
rem rename "ZipPackage" "Myra.%APPVEYOR_BUILD_VERSION%"
rem 7z a Myra.%APPVEYOR_BUILD_VERSION%.zip Myra.%APPVEYOR_BUILD_VERSION%
