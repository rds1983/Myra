rem delete existing
rmdir "ZipPackage" /Q /S

rem Create required folders
mkdir "ZipPackage"
mkdir "ZipPackage\Assets"
mkdir "ZipPackage\DesktopGL"
mkdir "ZipPackage\DesktopGL\x64"
mkdir "ZipPackage\DesktopGL\x86"

set "CONFIGURATION=Release"

rem Copy output files
copy "Myra\bin\%CONFIGURATION%\Myra.dll" ZipPackage /Y
copy "Myra\bin\%CONFIGURATION%\Myra.pdb" ZipPackage /Y
copy "Myra\bin\%CONFIGURATION%\Newtonsoft.Json.dll" ZipPackage /Y
copy "Myra\bin\%CONFIGURATION%\Newtonsoft.Json.xml" ZipPackage /Y
copy "Myra.UIEditor\bin\%CONFIGURATION%\NLog.dll" ZipPackage /Y
copy "Myra.UIEditor\bin\%CONFIGURATION%\NLog.xml" ZipPackage /Y
copy "Myra.UIEditor\bin\%CONFIGURATION%\NLog.config" ZipPackage /Y
copy "Myra\bin\%CONFIGURATION%\StbSharp.dll" ZipPackage /Y
copy "Myra\bin\%CONFIGURATION%\Sichem.Framework.dll" ZipPackage /Y
copy "Myra.Samples\bin\%CONFIGURATION%\Myra.Samples.dll" ZipPackage /Y
copy "Myra.Samples\bin\%CONFIGURATION%\Myra.Samples.pdb" ZipPackage /Y
copy "Myra.Samples.WinForms\bin\%CONFIGURATION%\Myra.Samples.WinForms.dll" ZipPackage /Y
copy "Myra.Samples.WinForms\bin\%CONFIGURATION%\Myra.Samples.Winforms.pdb" ZipPackage /Y
copy "Myra.Editor\bin\%CONFIGURATION%\Myra.Editor.dll" ZipPackage /Y
copy "Myra.Editor\bin\%CONFIGURATION%\Myra.Editor.pdb" ZipPackage /Y
copy "Myra.UIEditor.Plugin.LibGDX\bin\%CONFIGURATION%\Myra.UIEditor.Plugin.LibGDX.dll" ZipPackage /Y
copy "Myra.UIEditor.Plugin.LibGDX\bin\%CONFIGURATION%\Myra.UIEditor.Plugin.LibGDX.pdb" ZipPackage /Y
copy "Myra.UIEditor.Plugin.YellowMenuButtons\bin\%CONFIGURATION%\Myra.UIEditor.Plugin.YellowMenuButtons.dll" ZipPackage /Y
copy "Myra.UIEditor.Plugin.YellowMenuButtons\bin\%CONFIGURATION%\Myra.UIEditor.Plugin.YellowMenuButtons.pdb" ZipPackage /Y
copy "Myra.Samples.DesktopGLLauncher\bin\%CONFIGURATION%\Myra.Samples.DesktopGLLauncher.exe" ZipPackage /Y
copy "Myra.Samples.DesktopGLLauncher\bin\%CONFIGURATION%\Myra.Samples.DesktopGLLauncher.exe.config" ZipPackage /Y
copy "Myra.Samples.DesktopGLLauncher\bin\%CONFIGURATION%\Myra.Samples.DesktopGLLauncher.pdb" ZipPackage /Y
copy "Myra.UIEditor\bin\%CONFIGURATION%\Myra.UIEditor.exe" ZipPackage /Y
copy "Myra.UIEditor\bin\%CONFIGURATION%\Myra.UIEditor.exe.config" ZipPackage /Y
copy "Myra.UIEditor\bin\%CONFIGURATION%\Myra.UIEditor.pdb" ZipPackage /Y

copy "Myra.Samples\Assets\awesomeGame.ui" "ZipPackage\Assets" /Y
copy "Myra.Samples\Assets\mistral.fnt" "ZipPackage\Assets" /Y
copy "Myra.Samples\Assets\mistral_0.png" "ZipPackage\Assets" /Y

copy "Myra-Dependencies\MonoGame.Framework.DesktopGL\MonoGame.Framework.dll" "ZipPackage\DesktopGL" /Y
copy "Myra-Dependencies\MonoGame.Framework.DesktopGL\MonoGame.Framework.dll.config" "ZipPackage\DesktopGL" /Y
copy "Myra-Dependencies\MonoGame.Framework.DesktopGL\x64\libSDL2-2.0.so.0" "ZipPackage\DesktopGL\x64" /Y
copy "Myra-Dependencies\MonoGame.Framework.DesktopGL\x64\libopenal.so.1" "ZipPackage\DesktopGL\x64" /Y
copy "Myra-Dependencies\MonoGame.Framework.DesktopGL\x64\SDL2.dll" "ZipPackage\DesktopGL\x64" /Y
copy "Myra-Dependencies\MonoGame.Framework.DesktopGL\x64\soft_oal.dll" "ZipPackage\DesktopGL\x64" /Y
copy "Myra-Dependencies\MonoGame.Framework.DesktopGL\x86\libSDL2-2.0.so.0" "ZipPackage\DesktopGL\x86" /Y
copy "Myra-Dependencies\MonoGame.Framework.DesktopGL\x86\libopenal.so.1" "ZipPackage\DesktopGL\x86" /Y
copy "Myra-Dependencies\MonoGame.Framework.DesktopGL\x86\SDL2.dll" "ZipPackage\DesktopGL\x86" /Y
copy "Myra-Dependencies\MonoGame.Framework.DesktopGL\x86\soft_oal.dll" "ZipPackage\DesktopGL\x86" /Y
copy "Myra-Dependencies\MonoGame.Framework.DesktopGL\libSDL2-2.0.0.dylib" "ZipPackage\DesktopGL" /Y
copy "Myra-Dependencies\MonoGame.Framework.DesktopGL\libopenal.1.dylib" "ZipPackage\DesktopGL" /Y
