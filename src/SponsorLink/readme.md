# SponsorLink .NET Analyzer

This is one opinionated implementation of [SponsorLink](https://devlooped.com/SponsorLink) 
for .NET projects leveraging Roslyn analyzers.

It is intended for use by [devlooped](https://github.com/devlooped) projects, but can be 
used as a template for other sponsorables as well. Supporting arbitrary sponsoring scenarios 
is out of scope though, since we just use GitHub sponsors for now.

## Usage

A project initializing from this template repo via [dotnet-file](https://github.com/devlooped/dotnet-file) 
will have all the sources cloned under `src\SponsorLink`. 

Including the analyzer and targets in a project involves two steps. 

1. Create an analyzer project and add the following property:

```xml
  <PropertyGroup>
    ...
    <CustomAfterMicrosoftCSharpTargets>$(MSBuildThisFileDirectory)..\SponsorLink\SponsorLink.targets</CustomAfterMicrosoftCSharpTargets>
  </PropertyGroup>
```

2. Add a `buildTransitive\[PackageId].targets` file with the following import:

```xml
<Project xmlns="http://schemas.microsoft.com/developer/msbuild/2003">
  <Import Project="Devlooped.Sponsors.targets"/>
</Project>
```

As long as NuGetizer is used, the right packaging will be done automatically.