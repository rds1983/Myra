$version = $args[0]
echo "Version: $version"

# Recreate "ZipPackage"
Remove-Item -Recurse -Force "ZipPackage" -ErrorAction Ignore
Remove-Item -Recurse -Force "Myra.$version" -ErrorAction Ignore

New-Item -ItemType directory -Path "ZipPackage"
New-Item -ItemType directory -Path "ZipPackage\Stylesheets"
New-Item -ItemType directory -Path "ZipPackage\Assets"
New-Item -ItemType directory -Path "ZipPackage\Assets\fonts"
New-Item -ItemType directory -Path "ZipPackage\Assets\images"

function Copy-Sample([string]$SampleName) {
	Copy-Item -Path "samples\Myra.Samples.$SampleName\bin\MonoGame\Release\net6.0\Myra.Samples.$SampleName.exe" -Destination "ZipPackage"
	Copy-Item -Path "samples\Myra.Samples.$SampleName\bin\MonoGame\Release\net6.0\Myra.Samples.$SampleName.dll" -Destination "ZipPackage"
	Copy-Item -Path "samples\Myra.Samples.$SampleName\bin\MonoGame\Release\net6.0\Myra.Samples.$SampleName.runtimeconfig.json" -Destination "ZipPackage"
}

# Copy-Item -Path files
Copy-Item -Path "src\Myra\bin\MonoGame\Release\netstandard2.0\Myra.*" -Destination "ZipPackage"
Copy-Sample "AllWidgets"
Copy-Sample "CustomUIStylesheet"
Copy-Sample "CustomWidgets"
Copy-Sample "GridContainer"
Copy-Sample "SplitPaneContainer"
Copy-Sample "Notepad"
Copy-Sample "NonModalWindows"
Copy-Sample "DebugConsole"
Copy-Sample "AssetManagement"
Copy-Item -Path "samples\Myra.Samples.AssetManagement\Assets\fonts\*" -Destination "ZipPackage\Assets\fonts\" -Recurse
Copy-Item -Path "samples\Myra.Samples.AssetManagement\Assets\images\*" -Destination "ZipPackage\Assets\images\" -Recurse
Copy-Sample "ObjectEditor"
Copy-Item -Path "samples\Myra.Samples.ObjectEditor\image.png" -Destination "ZipPackage"
Copy-Sample "TextRendering"
Copy-Item -Path "samples\Stylesheets\*" -Destination "ZipPackage\Stylesheets\" -Recurse
Copy-Item -Path "src\MyraPad\bin\Release\net6.0\System.CodeDom.dll" -Destination "ZipPackage"
Copy-Item -Path "src\MyraPad\bin\Release\net6.0\MyraPad.exe" -Destination "ZipPackage"
Copy-Item -Path "src\MyraPad\bin\Release\net6.0\MyraPad.dll" -Destination "ZipPackage"
Copy-Item -Path "src\MyraPad\bin\Release\net6.0\MyraPad.runtimeconfig.json" -Destination "ZipPackage"
Copy-Item -Path "src\MyraPad\bin\Release\net6.0\FontStashSharp.*" -Destination "ZipPackage"
Copy-Item -Path "src\MyraPad\bin\Release\net6.0\StbImageSharp.dll" -Destination "ZipPackage"
Copy-Item -Path "src\MyraPad\bin\Release\net6.0\StbTrueTypeSharp.dll" -Destination "ZipPackage"
Copy-Item -Path "src\MyraPad\bin\Release\net6.0\Cyotek.Drawing.BitmapFont.dll" -Destination "ZipPackage"
Copy-Item -Path "src\MyraPad\bin\Release\net6.0\AssetManagementBase.dll" -Destination "ZipPackage"
Copy-Item -Path "src\MyraPad\bin\Release\net6.0\DdsKtxXna.dll" -Destination "ZipPackage"
Copy-Item -Path "src\MyraPad\bin\Release\net6.0\XNAssets.MonoGame.dll" -Destination "ZipPackage"
Copy-Item -Path "src\MyraPad\bin\Release\net6.0\info.lundin.math.dll" -Destination "ZipPackage"
Copy-Item -Path "src\MyraPad\bin\Release\net6.0\MonoGame.Framework.dll" -Destination "ZipPackage"
Copy-Item -Path "src\MyraPad\bin\Release\net6.0\runtimes" -Destination "ZipPackage" -Recurse

# Compress
Rename-Item "ZipPackage" "Myra.$version"
Compress-Archive -Path "Myra.$version" -DestinationPath "Myra.$version.zip" -Force

# Delete the folder
Remove-Item -Recurse -Force "Myra.$version"