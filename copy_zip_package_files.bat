rem delete existing
rmdir "ZipPackage" /Q /S

rem Create required folders
mkdir "ZipPackage"
mkdir "ZipPackage\FNA"
mkdir "ZipPackage\Xenko"
mkdir "ZipPackage\x64"
mkdir "ZipPackage\x86"
mkdir "ZipPackage\Stylesheets"

set "CONFIGURATION=Release\net45"

rem Copy output files
copy "src\Myra\bin\MonoGame\%CONFIGURATION%\Myra.dll" ZipPackage /Y
copy "src\Myra\bin\MonoGame\%CONFIGURATION%\Myra.pdb" ZipPackage /Y
copy "src\Myra\bin\FNA\%CONFIGURATION%\Myra.dll" "ZipPackage\FNA\" /Y
copy "src\Myra\bin\FNA\%CONFIGURATION%\Myra.pdb" "ZipPackage\FNA\" /Y
copy "src\Myra\bin\Xenko\%CONFIGURATION%\Myra.dll" "ZipPackage\Xenko\" /Y
copy "src\Myra\bin\Xenko\%CONFIGURATION%\Myra.pdb" "ZipPackage\Xenko\" /Y
copy "src\Myra\bin\MonoGame\%CONFIGURATION%\Newtonsoft.Json.dll" ZipPackage /Y
copy "samples\Myra.Samples.AllWidgets\bin\MonoGame\%CONFIGURATION%\Myra.Samples.AllWidgets.exe" ZipPackage /Y
copy "samples\Myra.Samples.CustomUIStylesheet\bin\%CONFIGURATION%\Myra.Samples.CustomUIStylesheet.exe" ZipPackage /Y
copy "samples\Myra.Samples.GridContainer\bin\%CONFIGURATION%\Myra.Samples.GridContainer.exe" ZipPackage /Y
copy "samples\Myra.Samples.SplitPaneContainer\bin\%CONFIGURATION%\Myra.Samples.SplitPaneContainer.exe" ZipPackage /Y
copy "src\Myra.UIEditor\bin\%CONFIGURATION%\Myra.UIEditor.exe" ZipPackage /Y
xcopy "samples\Stylesheets\*.*" "ZipPackage\Stylesheets\*.*" /s

copy "src\Myra.UIEditor\bin\%CONFIGURATION%\MonoGame.Framework.dll" "ZipPackage" /Y
copy "src\Myra.UIEditor\bin\%CONFIGURATION%\MonoGame.Framework.dll.config" "ZipPackage" /Y
copy "src\Myra.UIEditor\bin\%CONFIGURATION%\x64\libSDL2-2.0.so.0" "ZipPackage\x64" /Y
copy "src\Myra.UIEditor\bin\%CONFIGURATION%\x64\libopenal.so.1" "ZipPackage\x64" /Y
copy "src\Myra.UIEditor\bin\%CONFIGURATION%\x64\SDL2.dll" "ZipPackage\x64" /Y
copy "src\Myra.UIEditor\bin\%CONFIGURATION%\x64\soft_oal.dll" "ZipPackage\x64" /Y
copy "src\Myra.UIEditor\bin\%CONFIGURATION%\x86\libSDL2-2.0.so.0" "ZipPackage\x86" /Y
copy "src\Myra.UIEditor\bin\%CONFIGURATION%\x86\libopenal.so.1" "ZipPackage\x86" /Y
copy "src\Myra.UIEditor\bin\%CONFIGURATION%\x86\SDL2.dll" "ZipPackage\x86" /Y
copy "src\Myra.UIEditor\bin\%CONFIGURATION%\x86\soft_oal.dll" "ZipPackage\x86" /Y
copy "src\Myra.UIEditor\bin\%CONFIGURATION%\libSDL2-2.0.0.dylib" "ZipPackage" /Y
copy "src\Myra.UIEditor\bin\%CONFIGURATION%\libopenal.1.dylib" "ZipPackage" /Y