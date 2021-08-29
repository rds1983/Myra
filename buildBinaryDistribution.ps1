$version = $args[0]
echo "Version: $version"

# Recreate "ZipPackage"
Remove-Item -Recurse -Force "ZipPackage" -ErrorAction Ignore
Remove-Item -Recurse -Force "Myra.$version" -ErrorAction Ignore

New-Item -ItemType directory -Path "ZipPackage"
New-Item -ItemType directory -Path "ZipPackage\x64"
New-Item -ItemType directory -Path "ZipPackage\x86"
New-Item -ItemType directory -Path "ZipPackage\Stylesheets"
New-Item -ItemType directory -Path "ZipPackage\Assets"
New-Item -ItemType directory -Path "ZipPackage\Assets\fonts"
New-Item -ItemType directory -Path "ZipPackage\Assets\images"

# Copy-Item -Path files
Copy-Item -Path "src\Myra\bin\MonoGame\Release\net45\Myra.dll" -Destination "ZipPackage"
Copy-Item -Path "src\Myra\bin\MonoGame\Release\net45\Myra.pdb" -Destination "ZipPackage"
Copy-Item -Path "samples\Myra.Samples.AllWidgets\bin\MonoGame\Release\net45\Myra.Samples.AllWidgets.exe" -Destination "ZipPackage"
Copy-Item -Path "samples\Myra.Samples.CustomUIStylesheet\bin\MonoGame\Release\net45\Myra.Samples.CustomUIStylesheet.exe" -Destination "ZipPackage"
Copy-Item -Path "samples\Myra.Samples.CustomWidgets\bin\MonoGame\Release\net45\Myra.Samples.CustomWidgets.exe" -Destination "ZipPackage"
Copy-Item -Path "samples\Myra.Samples.GridContainer\bin\MonoGame\Release\net45\Myra.Samples.GridContainer.exe" -Destination "ZipPackage"
Copy-Item -Path "samples\Myra.Samples.SplitPaneContainer\bin\MonoGame\Release\net45\Myra.Samples.SplitPaneContainer.exe" -Destination "ZipPackage"
Copy-Item -Path "samples\Myra.Samples.Notepad\bin\MonoGame\Release\net45\Myra.Samples.Notepad.exe" -Destination "ZipPackage"
Copy-Item -Path "samples\Myra.Samples.NonModalWindows\bin\MonoGame\Release\net45\Myra.Samples.NonModalWindows.exe" -Destination "ZipPackage"
Copy-Item -Path "samples\Myra.Samples.DebugConsole\bin\MonoGame\Release\net45\Myra.Samples.DebugConsole.exe" -Destination "ZipPackage"
Copy-Item -Path "samples\Myra.Samples.AssetManagement\bin\MonoGame\Release\net45\Myra.Samples.AssetManagement.exe" -Destination "ZipPackage"
Copy-Item -Path "samples\Myra.Samples.AssetManagement\Assets\fonts\*" -Destination "ZipPackage\Assets\fonts\" -Recurse
Copy-Item -Path "samples\Myra.Samples.AssetManagement\Assets\images\*" -Destination "ZipPackage\Assets\images\" -Recurse
Copy-Item -Path "samples\Myra.Samples.ObjectEditor\bin\MonoGame\Release\net45\Myra.Samples.ObjectEditor.exe" -Destination "ZipPackage"
Copy-Item -Path "samples\Myra.Samples.ObjectEditor\image.png" -Destination "ZipPackage"
Copy-Item -Path "samples\Myra.Samples.TextRendering\bin\MonoGame\Release\net45\Myra.Samples.TextRendering.exe" -Destination "ZipPackage"
Copy-Item -Path "samples\Stylesheets\*" -Destination "ZipPackage\Stylesheets\" -Recurse
Copy-Item -Path "src\MyraPad\bin\Release\net45\MyraPad.exe" -Destination "ZipPackage"
Copy-Item -Path "src\MyraPad\bin\Release\net45\FontStashSharp.MonoGame.dll" -Destination "ZipPackage"
Copy-Item -Path "src\MyraPad\bin\Release\net45\StbImageSharp.dll" -Destination "ZipPackage"
Copy-Item -Path "src\MyraPad\bin\Release\net45\StbTrueTypeSharp.dll" -Destination "ZipPackage"
Copy-Item -Path "src\MyraPad\bin\Release\net45\Cyotek.Drawing.BitmapFont.dll" -Destination "ZipPackage"
Copy-Item -Path "src\MyraPad\bin\Release\net45\info.lundin.math.dll" -Destination "ZipPackage"
Copy-Item -Path "src\MyraPad\bin\Release\net45\MonoGame.Framework.dll" -Destination "ZipPackage"
Copy-Item -Path "src\MyraPad\bin\Release\net45\MonoGame.Framework.dll.config" -Destination "ZipPackage"
Copy-Item -Path "src\MyraPad\bin\Release\net45\x64\libSDL2-2.0.so.0" -Destination "ZipPackage\x64"
Copy-Item -Path "src\MyraPad\bin\Release\net45\x64\libopenal.so.1" -Destination "ZipPackage\x64"
Copy-Item -Path "src\MyraPad\bin\Release\net45\x64\SDL2.dll" -Destination "ZipPackage\x64"
Copy-Item -Path "src\MyraPad\bin\Release\net45\x64\soft_oal.dll" -Destination "ZipPackage\x64"
Copy-Item -Path "src\MyraPad\bin\Release\net45\x86\libSDL2-2.0.so.0" -Destination "ZipPackage\x86"
Copy-Item -Path "src\MyraPad\bin\Release\net45\x86\libopenal.so.1" -Destination "ZipPackage\x86"
Copy-Item -Path "src\MyraPad\bin\Release\net45\x86\SDL2.dll" -Destination "ZipPackage\x86"
Copy-Item -Path "src\MyraPad\bin\Release\net45\x86\soft_oal.dll" -Destination "ZipPackage\x86"
Copy-Item -Path "src\MyraPad\bin\Release\net45\libSDL2-2.0.0.dylib" -Destination "ZipPackage"
Copy-Item -Path "src\MyraPad\bin\Release\net45\libopenal.1.dylib" -Destination "ZipPackage"
Copy-Item -Path "src\Myra.GdxTextureAtlasToMyra\bin\Release\net45\Myra.GdxTextureAtlasToMyra.exe" -Destination "ZipPackage"

# Compress
Rename-Item "ZipPackage" "Myra.$version"
Compress-Archive -Path "Myra.$version" -DestinationPath "Myra.$version.zip" -Force

# Delete the folder
Remove-Item -Recurse -Force "Myra.$version"