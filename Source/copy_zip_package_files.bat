rem delete existing
rmdir "ZipPackage" /Q /S

rem Create required folders
mkdir "ZipPackage"
mkdir "ZipPackage\FNA"
mkdir "ZipPackage\x64"
mkdir "ZipPackage\x86"

set "CONFIGURATION=Release"

rem Copy output files
copy "Myra\bin\%CONFIGURATION%\Myra.dll" ZipPackage /Y
copy "Myra\bin\%CONFIGURATION%\Myra.pdb" ZipPackage /Y
copy "Myra\bin\FNA\%CONFIGURATION%\Myra.dll" "ZipPackage\FNA\" /Y
copy "Myra\bin\FNA\%CONFIGURATION%\Myra.pdb" "ZipPackage\FNA\" /Y
copy "Myra\bin\%CONFIGURATION%\Newtonsoft.Json.dll" ZipPackage /Y
copy "Myra\bin\%CONFIGURATION%\Newtonsoft.Json.xml" ZipPackage /Y
copy "Myra.UIEditor\bin\%CONFIGURATION%\NLog.dll" ZipPackage /Y
copy "Myra.UIEditor\bin\%CONFIGURATION%\NLog.xml" ZipPackage /Y
copy "Myra.UIEditor\bin\%CONFIGURATION%\NLog.config" ZipPackage /Y
copy "Samples\Myra.Samples.AllWidgetsSample\bin\%CONFIGURATION%\Myra.Samples.AllWidgetsSample.exe" ZipPackage /Y
copy "Samples\Myra.Samples.CustomUIStylesheetSample\bin\%CONFIGURATION%\Myra.Samples.CustomUIStylesheetSample.exe" ZipPackage /Y
copy "Samples\Myra.Samples.FormattedTextSample\bin\%CONFIGURATION%\Myra.Samples.FormattedTextSample.exe" ZipPackage /Y
copy "Samples\Myra.Samples.GridSample\bin\%CONFIGURATION%\Myra.Samples.GridSample.exe" ZipPackage /Y
copy "Samples\Myra.Samples.NotepadSample\bin\%CONFIGURATION%\Myra.Samples.NotepadSample.exe" ZipPackage /Y
copy "Samples\Myra.Samples.SplitPaneSample\bin\%CONFIGURATION%\Myra.Samples.SplitPaneSample.exe" ZipPackage /Y
copy "Samples\Myra.Samples.TabControlSample\bin\%CONFIGURATION%\Myra.Samples.TabControlSample.exe" ZipPackage /Y
copy "Samples\Myra.Samples.TextBlocksSample\bin\%CONFIGURATION%\Myra.Samples.TextBlocksSample.exe" ZipPackage /Y
copy "Samples\Myra.UIEditor.Plugin.LibGDX\bin\%CONFIGURATION%\Myra.UIEditor.Plugin.LibGDX.dll" ZipPackage /Y
copy "Samples\Myra.UIEditor.Plugin.YellowMenuButtons\bin\%CONFIGURATION%\Myra.UIEditor.Plugin.YellowMenuButtons.dll" ZipPackage /Y
copy "Myra.Editor\bin\%CONFIGURATION%\Myra.Editor.dll" ZipPackage /Y
copy "Myra.Editor\bin\%CONFIGURATION%\Myra.Editor.pdb" ZipPackage /Y
copy "Myra.Editor\bin\FNA\%CONFIGURATION%\Myra.Editor.dll" "ZipPackage\FNA\" /Y
copy "Myra.Editor\bin\FNA\%CONFIGURATION%\Myra.Editor.pdb" "ZipPackage\FNA\" /Y
copy "Myra.UIEditor\bin\%CONFIGURATION%\Myra.UIEditor.exe" ZipPackage /Y
copy "Myra.UIEditor\bin\%CONFIGURATION%\Myra.UIEditor.exe.config" ZipPackage /Y
copy "Myra.UIEditor\bin\%CONFIGURATION%\Myra.UIEditor.pdb" ZipPackage /Y

copy "Samples\Myra.Samples.CustomUIStylesheetSample\Content\bin\DesktopGL\*.*" "ZipPackage\Content" /Y

copy "Myra-Dependencies\MonoGame.Framework.DesktopGL\MonoGame.Framework.dll" "ZipPackage" /Y
copy "Myra-Dependencies\MonoGame.Framework.DesktopGL\MonoGame.Framework.dll.config" "ZipPackage" /Y
copy "Myra-Dependencies\MonoGame.Framework.DesktopGL\x64\libSDL2-2.0.so.0" "ZipPackage\x64" /Y
copy "Myra-Dependencies\MonoGame.Framework.DesktopGL\x64\libopenal.so.1" "ZipPackage\x64" /Y
copy "Myra-Dependencies\MonoGame.Framework.DesktopGL\x64\SDL2.dll" "ZipPackage\x64" /Y
copy "Myra-Dependencies\MonoGame.Framework.DesktopGL\x64\soft_oal.dll" "ZipPackage\x64" /Y
copy "Myra-Dependencies\MonoGame.Framework.DesktopGL\x86\libSDL2-2.0.so.0" "ZipPackage\x86" /Y
copy "Myra-Dependencies\MonoGame.Framework.DesktopGL\x86\libopenal.so.1" "ZipPackage\x86" /Y
copy "Myra-Dependencies\MonoGame.Framework.DesktopGL\x86\SDL2.dll" "ZipPackage\x86" /Y
copy "Myra-Dependencies\MonoGame.Framework.DesktopGL\x86\soft_oal.dll" "ZipPackage\x86" /Y
copy "Myra-Dependencies\MonoGame.Framework.DesktopGL\libSDL2-2.0.0.dylib" "ZipPackage" /Y
copy "Myra-Dependencies\MonoGame.Framework.DesktopGL\libopenal.1.dylib" "ZipPackage" /Y