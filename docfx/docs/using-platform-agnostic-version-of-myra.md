Myra.PlatformAgnostic is a version of Myra that isn't tied to any game engine.

It is available on NuGet: https://www.nuget.org/packages/Myra.PlatformAgnostic/

To use it, you must implement the interfaces [IMyraPlatform](https://github.com/rds1983/Myra/blob/master/src/Myra/Platform/IMyraPlatform.cs) and [IMyraRenderer](https://github.com/rds1983/Myra/blob/master/src/Myra/Platform/IMyraRenderer.cs). 

For the latter, among the methods DrawSprite and DrawQuad, only one should be implemented. The other can throw NotImplementedException. The property RendererType should indicate which method is implemented.

Samples that work through DrawSprite:
https://github.com/rds1983/Myra/tree/master/samples/Myra.Samples.PlatformAgnostic
https://github.com/rds1983/Myra/tree/master/samples/Myra.Samples.Silk.NET.TrippyGL


Sample that works through DrawQuad:
https://github.com/rds1983/Myra/tree/master/samples/Myra.Samples.Silk.NET