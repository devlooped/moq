# Moq.Types.ComTypes.dll

This directory contains an assembly `Moq.Types.ComTypes.dll` containing
Component Object Model (COM) interop types, used to test various aspects
of Moq compatibility with COM.

This assembly can be built from the `ComTypes.idl` definition file also
contained in this directory.


## Building


### Requirements

In order to build the DLL, you will need the following tools:

 * **`midl`** (Microsoft's MIDL Compiler) for producing a Type Library
   from an `.idl` source file. This tool is part of the Windows SDK.
   See ["Using the MIDL Compiler" on Microsoft Docs](https://docs.microsoft.com/en-us/windows/desktop/Midl/using-the-midl-compiler-2)
   for more information.

 * **`tlbimp`** (Microsoft's .NET Framework Type Library to Assembly
   Converter) for producing a COM interop assembly from a Type Library.
   This tool is part of the .NET Framework SDK, and it is automatically
   installed with Visual Studio. See ["Tlbimp.exe (Type Library Importer)" on Microsoft Docs](https://docs.microsoft.com/en-us/dotnet/framework/tools/tlbimp-exe-type-library-importer)
   for more information.

 * Optionally, **`peverify`** (Microsoft's .NET Framework PE Verifier)
   which is also part of the .NET Framework SDK and installed together
   with Visual Studio. See ["Peverify.exe (PEVerify Tool)" on Microsoft Docs](https://docs.microsoft.com/en-us/dotnet/framework/tools/peverify-exe-peverify-tool)


### Steps

Open a command prompt and ensure that the tools mentioned above can be
found in the `PATH`. For instance, start Visual Studio's developer
command prompt.

Next, you can either run the `BuildDLLFromIDL.cmd` batch file found in
this directory, or run the following equivalent steps manually:

1. Create a COM Type Library (a `.tlb` file) from the `.idl` definition:

   ```
   midl.exe /mktyplib203 ^
            /env win32 ^
            /tlb ComTypes.tlb ^
            ComTypes.idl
   ```

2. Convert the generated type library to a .NET interop types assembly:

   ```
   tlbimp ComTypes.tlb ^
          /out:Moq.Tests.ComTypes.dll ^
          /namespace:Moq.Tests.ComTypes ^
          /keyfile:..\..\Moq.snk ^
          /asmversion:4.0.0.0
   ```

3. Optionally verify the generated assembly:

   ```
   peverify Moq.Tests.ComTypes.dll
   ```
