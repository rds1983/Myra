echo Building DirectX version
mgcb /outputDir:bin/Windows /intermediateDir:obj/Windows /platform:Windows /reference:D:\Projects\Hebron\Myra\Myra.Content.Pipeline\bin\Debug\Myra.Content.Pipeline.dll /importer:EffectImporter /processor:MultiCompileEffectProcessor /build:Shaders\DefaultEffect.fx
copy /Y bin\Windows\Shaders\DefaultEffect.xnb bin\DefaultEffect.DirectX.xnb

echo Building OpenGL version
mgcb /outputDir:bin/Linux /intermediateDir:obj/Linux /platform:Linux /reference:D:\Projects\Hebron\Myra\Myra.Content.Pipeline\bin\Debug\Myra.Content.Pipeline.dll /importer:EffectImporter /processor:MultiCompileEffectProcessor /build:Shaders\DefaultEffect.fx
copy /Y bin\Linux\Shaders\DefaultEffect.xnb bin\DefaultEffect.OpenGL.xnb
