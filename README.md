# DetourUtility: A Lightweight C# Method Redirection Tool

**DetourUtility** is a lightweight utility in C# designed to dynamically redirect method calls at runtime using unsafe code for low-level memory manipulation. This technique is commonly known as "detouring" and is useful for modifying method behavior in scenarios where source code access is unavailable, such as in Unity or other compiled assemblies.

## Key Features
- Extracts `MethodInfo` from method call expressions, getter, and setter expressions.
- Compatible with both `32-bit` and `64-bit` systems.
- Enables method call detouring via unsafe code by modifying function pointers at runtime.

## Use Cases
Detouring is typically used in the following scenarios:
1. **Modding**: Especially useful in game modding where detouring allows the replacement or augmentation of game functions.
2. **Unity Editor Workarounds**: Unity has become more open to modification, but certain engine behaviors may still need fixing. Detouring enables custom editor bugfixes, tools, and workarounds.
3. **Runtime Engine Modifications**: While not ideal for production, detouring can resolve issues during development when other fixes are unavailable.
4. **Assembly Patching**: Useful for patching code from assemblies where the source code is unavailable.

## Caveats and Risks
Using detours comes with significant risks, including:
- **Crashes and Data Corruption**: Playing with function pointers can lead to crashes, freezes, or data corruption.
- **Platform Limitations**: Detouring works well with Mono and .NET but has limitations with IL2CPP (e.g., in Unity for consoles).
- **Recursion**: Improper detouring may lead to recursive calls, causing stack overflows.
- **Permanent Modifications**: Once applied, detours are permanent for the loaded assembly, with no way to call the original method unless it's manually reimplemented.

Make sure to fully understand the impact before using detours.


## Example Usage
```csharp
using System.Reflection;
using DetourUtility;

// Example of redirecting (detouring) method calls
MethodInfo originalMethod = typeof(SomeClass).GetMethod("OriginalMethod");
MethodInfo newMethod = typeof(SomeClass).GetMethod("NewMethod");

DetourUtility.TryDetourFromTo(originalMethod, newMethod);
```

## How It Works
`DetourUtility` rewrites function pointers in memory to redirect method calls. Depending on whether the system is 32-bit or 64-bit, the class adjusts its logic to handle different jump instructions at the assembly level.

This technique is based on tried-and-tested methods used in the modding and software patching communities, particularly in environments that require runtime code modification.

## Credits
[Detours: Redirecting C# Methods at Runtime](https://tryfinally.dev/detours-redirecting-csharp-methods-at-runtime?_sm_pdc=1&_sm_rid=FQ7SMMJPMvP2Pvkqt6MWlr21tP0sPSMMkRq4RHF)