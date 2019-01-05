dotnet --version
dotnet build build\Monogame\Myra.sln /p:Configuration=Release --no-incremental
dotnet build build\FNA\Myra.sln /p:Configuration=Release --no-incremental
call copy_zip_package_files.bat
rename "ZipPackage" "Myra.%APPVEYOR_BUILD_VERSION%"
7z a Myra.%APPVEYOR_BUILD_VERSION%.zip Myra.%APPVEYOR_BUILD_VERSION%
