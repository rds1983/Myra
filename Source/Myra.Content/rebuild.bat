echo Building DirectX version
mgcb /outputDir:bin/Windows /intermediateDir:obj/Windows /platform:Windows /reference:D:\Projects\Hebron\Myra\Myra-Dependencies\MonoGame.MultiCompileEffects\MonoGame.MultiCompileEffects.Content.Pipeline.dll /importer:EffectImporter /processor:MultiCompileEffectProcessor /build:Shaders\DefaultEffect.fx
copy /Y bin\Windows\Shaders\DefaultEffect.xnb bin\DefaultEffect.DirectX.xnb
mgcb /outputDir:bin/Windows /intermediateDir:obj/Windows /platform:Windows /reference:D:\Projects\Hebron\Myra\Myra-Dependencies\MonoGame.MultiCompileEffects\MonoGame.MultiCompileEffects.Content.Pipeline.dll /importer:EffectImporter /processor:MultiCompileEffectProcessor /build:Shaders\TerrainEffect.fx
copy /Y bin\Windows\Shaders\TerrainEffect.xnb bin\TerrainEffect.DirectX.xnb

echo Building OpenGL version
mgcb /outputDir:bin/Linux /intermediateDir:obj/Linux /platform:Linux /reference:D:\Projects\Hebron\Myra\Myra-Dependencies\MonoGame.MultiCompileEffects\MonoGame.MultiCompileEffects.Content.Pipeline.dll /importer:EffectImporter /processor:MultiCompileEffectProcessor /build:Shaders\DefaultEffect.fx
copy /Y bin\Linux\Shaders\DefaultEffect.xnb bin\DefaultEffect.OpenGL.xnb
mgcb /outputDir:bin/Linux /intermediateDir:obj/Linux /platform:Linux /reference:D:\Projects\Hebron\Myra\Myra-Dependencies\MonoGame.MultiCompileEffects\MonoGame.MultiCompileEffects.Content.Pipeline.dll /importer:EffectImporter /processor:MultiCompileEffectProcessor /build:Shaders\TerrainEffect.fx
copy /Y bin\Linux\Shaders\TerrainEffect.xnb bin\TerrainEffect.OpenGL.xnb
