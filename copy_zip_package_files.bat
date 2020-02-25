rem delete existing
rmdir "ZipPackage" /Q /S

rem Create required folders
mkdir "ZipPackage"
mkdir "ZipPackage\x64"
mkdir "ZipPackage\x86"
mkdir "ZipPackage\Stylesheets"
mkdir "ZipPackage\Assets"

set "CONFIGURATION=Release\net45"
set "CONFIGURATION_XENKO=Release\netstandard2.0"

rem Copy output files
copy "src\Myra\bin\MonoGame\%CONFIGURATION%\Myra.dll" ZipPackage /Y
copy "src\Myra\bin\MonoGame\%CONFIGURATION%\Myra.pdb" ZipPackage /Y
copy "samples\Myra.Samples.AllWidgets\bin\MonoGame\%CONFIGURATION%\Myra.Samples.AllWidgets.exe" ZipPackage /Y
copy "samples\Myra.Samples.CustomUIStylesheet\bin\MonoGame\%CONFIGURATION%\Myra.Samples.CustomUIStylesheet.exe" ZipPackage /Y
copy "samples\Myra.Samples.CustomWidgets\bin\MonoGame\%CONFIGURATION%\Myra.Samples.CustomWidgets.exe" ZipPackage /Y
copy "samples\Myra.Samples.GridContainer\bin\MonoGame\%CONFIGURATION%\Myra.Samples.GridContainer.exe" ZipPackage /Y
copy "samples\Myra.Samples.SplitPaneContainer\bin\MonoGame\%CONFIGURATION%\Myra.Samples.SplitPaneContainer.exe" ZipPackage /Y
copy "samples\Myra.Samples.Notepad\bin\MonoGame\%CONFIGURATION%\Myra.Samples.Notepad.exe" ZipPackage /Y
copy "samples\Myra.Samples.NonModalWindows\bin\MonoGame\%CONFIGURATION%\Myra.Samples.NonModalWindows.exe" ZipPackage /Y
copy "samples\Myra.Samples.DebugConsole\bin\MonoGame\%CONFIGURATION%\Myra.Samples.DebugConsole.exe" ZipPackage /Y
copy "samples\Myra.Samples.AssetManagement\bin\MonoGame\%CONFIGURATION%\Myra.Samples.AssetManagement.exe" ZipPackage /Y
copy "samples\Myra.Samples.ObjectEditor\bin\MonoGame\%CONFIGURATION%\Myra.Samples.ObjectEditor.exe" ZipPackage /Y
copy "src\MyraPad\bin\%CONFIGURATION%\MyraPad.exe" ZipPackage /Y
copy "src\MyraPad\bin\%CONFIGURATION%\XNAssets.dll" "ZipPackage" /Y
copy "src\MyraPad\bin\%CONFIGURATION%\info.lundin.math.dll" "ZipPackage" /Y
copy "src\MyraPad\bin\%CONFIGURATION%\MonoGame.Framework.dll" "ZipPackage" /Y
copy "src\MyraPad\bin\%CONFIGURATION%\MonoGame.Framework.dll.config" "ZipPackage" /Y
copy "src\MyraPad\bin\%CONFIGURATION%\x64\libSDL2-2.0.so.0" "ZipPackage\x64" /Y
copy "src\MyraPad\bin\%CONFIGURATION%\x64\libopenal.so.1" "ZipPackage\x64" /Y
copy "src\MyraPad\bin\%CONFIGURATION%\x64\SDL2.dll" "ZipPackage\x64" /Y
copy "src\MyraPad\bin\%CONFIGURATION%\x64\soft_oal.dll" "ZipPackage\x64" /Y
copy "src\MyraPad\bin\%CONFIGURATION%\x86\libSDL2-2.0.so.0" "ZipPackage\x86" /Y
copy "src\MyraPad\bin\%CONFIGURATION%\x86\libopenal.so.1" "ZipPackage\x86" /Y
copy "src\MyraPad\bin\%CONFIGURATION%\x86\SDL2.dll" "ZipPackage\x86" /Y
copy "src\MyraPad\bin\%CONFIGURATION%\x86\soft_oal.dll" "ZipPackage\x86" /Y
copy "src\MyraPad\bin\%CONFIGURATION%\libSDL2-2.0.0.dylib" "ZipPackage" /Y
copy "src\MyraPad\bin\%CONFIGURATION%\libopenal.1.dylib" "ZipPackage" /Y
xcopy "samples\Stylesheets\*.*" "ZipPackage\Stylesheets\*.*" /s
xcopy "samples\Myra.Samples.AssetManagement\Assets\fonts\*.*" "ZipPackage\Assets\fonts\*.*" /s
xcopy "samples\Myra.Samples.AssetManagement\Assets\images\*.*" "ZipPackage\Assets\images\*.*" /s
copy "samples\Myra.Samples.ObjectEditor\image.png" ZipPackage /Y
