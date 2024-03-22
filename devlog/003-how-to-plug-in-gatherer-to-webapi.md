Problem: How can I easily plug in Udup to standard WebAPI? I would like to use it like a swashbuckle. Ideally, if all could be configurable
from IServiceCollection and IApplicationBuilder.

### Approach 1. Add Udup to the IServiceCollection and IApplicationBuilder (DECLINED)

It worked when I was using reflection to gather, but reflection is limited.
It failed because I didn't find a way to use Roslyn at runtime. I assume that there is no way to analyze source code in runtime. it feels weird to pack source code as a embedded resource and then analyze it.

### Approach 2. Use source generator (NOT LIKELY)

It could work but there are massive problems:
1. It's hard to use PackageReferences and ProjectReferences. https://github.com/dotnet/roslyn/discussions/47517#discussioncomment-64145
2. It's hard to adjust configuration in code. 2 ways came up to my mind and both feels bad. 
   1. Use only attributes to point out types that represents events and eventhandlers.
   2. Add json configuration file
3. netstandard 2.1

### Approach 3. Use console application and provide it to `dotnet tool` (GOOD AT START)

This tool could be triggered at build (adding it to .csproj build step), at docker image creation or in pipeline. Also can be generated in test and verify it with snapshot testing (awareness testing). 
It can be configured by parameters or pointing to a configuration file.

### Approach 4. Create separate project to extend and configure basic udup functionality (NUKE way)

It will allow to easily configure Udup and extend it. This is similar to the approach 3.




## Summary

It's hard to decide on the best approach. I would love use it like Swashbuckle, but I need roslyn and source code. I also want to configure it with fluent methods.

I will postpone this decision. I don't know yet what exactly I want to configure. For now I think about some simple selectors for event and event handlers (mediatr, custom events types, some other libs)

I will focus on gathering more useful data from source code, later will introduce some conventions to obtain even more from solution structure.
