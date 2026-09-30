# moq

[![Version](https://img.shields.io/endpoint?url=https://shields.kzu.io/vpre/Moq/main&label=nuget.ci&color=brightgreen)](https://pkg.kzu.io/index.json)
[![Status](https://github.com/moq/moq/workflows/build/badge.svg?branch=main)](https://github.com/moq/moq/actions?query=branch%3Amain+workflow%3Abuild+)
[![License](https://img.shields.io/github/license/moq/moq.svg)](https://github.com/moq/moq/blob/master/LICENSE)
[![Discord Chat](https://img.shields.io/badge/chat-on%20discord-7289DA.svg)](https://discord.gg/8PtpGdu)
[![GitHub](https://img.shields.io/badge/-source-181717.svg?logo=GitHub)](https://github.com/moq/moq)


> [!IMPORTANT]
> This branch now contains the *upcoming* version of Moq (v5).
> Source and license for the current stable Moq v4.x are at the [v4 branch](https://github.com/devlooped/moq/tree/v4)

CI package feed: https://pkg.kzu.app/index.json

<!-- #content -->
The most popular and friendly mocking framework for .NET

## Usage

```csharp
using Moq;
using static Moq.Syntax;

var calc = new Mock<ICalculator>();       // or Mock.Get(Mock.Of<ICalculator>())

// Typed setups are generated for every mocked type's members (C# 14+).
calc.Add(2, 3).Returns(5);
calc.Add(Any<int>(), Any<int>()).Returns((x, y) => x + y);
calc.Mode.Returns(CalculatorMode.Scientific);
calc.TurnOn().Throws(new InvalidOperationException());
calc.RaiseTurnedOn();

// Any language version, and members that collide with the mock's own (i.e. Object, CallBase), 
// or recursive setups: invoke the mocked object within a setup lambda.
Setup(() => calc.Object.Memory.Recall()).Returns(42);

// ref/out parameters: typed setups receive them in the handler...
int x = Any<int>(), y = Any<int>();
calc.TryAdd(ref x, ref y, out _).Returns((ref a, ref b, out sum) => { sum = a + b; return true; });
// ...or use a custom delegate, which a code fix generates for you from SetupRef(parser.Object.TryParse).
SetupRef<TryParse>(parser.Object.TryParse).Returns((string input, out DateTimeOffset date) => DateTimeOffset.TryParse(input, out date));

// Verification
Verify.Called(calc).Add(2, 3).Once();
Verify.NotCalled(calc).TurnOn();
Verify.Called(() => calc.Object.Add(2, 3), times: 1);

// Additional interfaces
var disposable = new Mock<ICalculator, IDisposable>().As<IDisposable>();
```

`Mock<T>` and `Mock.Of<T>` are compiled into your test project as source, so you can customize 
how every mock is created with a partial class:

```csharp
namespace Moq;

partial class Mock<T>
{
    partial void OnCreated() => this.Sdk.Name = typeof(T).Name;
}
```

The lower-level runtime (invocations, setups, state and behaviors) is available from any mock via 
`mock.Sdk`. See [Moq SDK](docs/MoqSdk.md).
<!-- #content -->

## Testing built packages locally

You can either build from command line or explicitly Pack (from the context menu) the *Moq.Package* project.

Packages are generated in the `bin` folder in the repository root. To test these packages you can just add a package source 
pointing to it. You can also just place a `NuGet.Config` like the following anywhere above the directory with the 
test solution(s):

```xml
<configuration>
	<packageSources>
		<add key="moq" value="[cloned repo dir]\bin" />
  </packageSources>
</configuration>
```

You can also do use project properties (or a *Directory.Build.props* to affect an entire folder hierarchy) with:

```xml
<Project>
  <PropertyGroup>
    <RestoreSources>https://api.nuget.org/v3/index.json;$(RestoreSources)</RestoreSources>
    <RestoreSources Condition="Exists('[cloned repo dir]\bin')">[cloned repo dir]\bin;$(RestoreSources)</RestoreSources>
  </PropertyGroup>
<Project>
```

Every time the packages are produced, the local nuget cache is cleared, so that a subsequent restore in VS will 
automatically cause the updated version to be unpacked again. 
The locally built version will always have the version [42.42.42](https://en.wikipedia.org/wiki/42_(number)#The_Hitchhiker's_Guide_to_the_Galaxy).

<!-- include https://github.com/devlooped/sponsors/raw/main/footer.md -->
# Sponsors 

<!-- sponsors.md -->
[![Clarius Org](https://avatars.githubusercontent.com/u/71888636?v=4&s=39 "Clarius Org")](https://github.com/clarius)
[![MFB Technologies, Inc.](https://avatars.githubusercontent.com/u/87181630?v=4&s=39 "MFB Technologies, Inc.")](https://github.com/MFB-Technologies-Inc)
[![SandRock](https://avatars.githubusercontent.com/u/321868?u=99e50a714276c43ae820632f1da88cb71632ec97&v=4&s=39 "SandRock")](https://github.com/sandrock)
[![DRIVE.NET, Inc.](https://avatars.githubusercontent.com/u/15047123?v=4&s=39 "DRIVE.NET, Inc.")](https://github.com/drivenet)
[![Keith Pickford](https://avatars.githubusercontent.com/u/16598898?u=64416b80caf7092a885f60bb31612270bffc9598&v=4&s=39 "Keith Pickford")](https://github.com/Keflon)
[![Thomas Bolon](https://avatars.githubusercontent.com/u/127185?u=7f50babfc888675e37feb80851a4e9708f573386&v=4&s=39 "Thomas Bolon")](https://github.com/tbolon)
[![Reuben Swartz](https://avatars.githubusercontent.com/u/724704?u=2076fe336f9f6ad678009f1595cbea434b0c5a41&v=4&s=39 "Reuben Swartz")](https://github.com/rbnswartz)
[![Jacob Foshee](https://avatars.githubusercontent.com/u/480334?v=4&s=39 "Jacob Foshee")](https://github.com/jfoshee)
[![](https://avatars.githubusercontent.com/u/33566379?u=bf62e2b46435a267fa246a64537870fd2449410f&v=4&s=39 "")](https://github.com/Mrxx99)
[![Eric Johnson](https://avatars.githubusercontent.com/u/26369281?u=41b560c2bc493149b32d384b960e0948c78767ab&v=4&s=39 "Eric Johnson")](https://github.com/eajhnsn1)
[![Jonathan ](https://avatars.githubusercontent.com/u/5510103?u=98dcfbef3f32de629d30f1f418a095bf09e14891&v=4&s=39 "Jonathan ")](https://github.com/Jonathan-Hickey)
[![Ken Bonny](https://avatars.githubusercontent.com/u/6417376?u=569af445b6f387917029ffb5129e9cf9f6f68421&v=4&s=39 "Ken Bonny")](https://github.com/KenBonny)
[![Simon Cropp](https://avatars.githubusercontent.com/u/122666?v=4&s=39 "Simon Cropp")](https://github.com/SimonCropp)
[![agileworks-eu](https://avatars.githubusercontent.com/u/5989304?v=4&s=39 "agileworks-eu")](https://github.com/agileworks-eu)
[![Zheyu Shen](https://avatars.githubusercontent.com/u/4067473?v=4&s=39 "Zheyu Shen")](https://github.com/arsdragonfly)
[![Vezel](https://avatars.githubusercontent.com/u/87844133?v=4&s=39 "Vezel")](https://github.com/vezel-dev)
[![ChilliCream](https://avatars.githubusercontent.com/u/16239022?v=4&s=39 "ChilliCream")](https://github.com/ChilliCream)
[![4OTC](https://avatars.githubusercontent.com/u/68428092?v=4&s=39 "4OTC")](https://github.com/4OTC)
[![domischell](https://avatars.githubusercontent.com/u/66068846?u=0a5c5e2e7d90f15ea657bc660f175605935c5bea&v=4&s=39 "domischell")](https://github.com/DominicSchell)
[![Adrian Alonso](https://avatars.githubusercontent.com/u/2027083?u=129cf516d99f5cb2fd0f4a0787a069f3446b7522&v=4&s=39 "Adrian Alonso")](https://github.com/adalon)
[![torutek](https://avatars.githubusercontent.com/u/33917059?v=4&s=39 "torutek")](https://github.com/torutek)
[![Ryan McCaffery](https://avatars.githubusercontent.com/u/16667079?u=c0daa64bb5c1b572130e05ae2b6f609ecc912d4d&v=4&s=39 "Ryan McCaffery")](https://github.com/mccaffers)
[![Seika Logiciel](https://avatars.githubusercontent.com/u/2564602?v=4&s=39 "Seika Logiciel")](https://github.com/SeikaLogiciel)
[![Andrew Grant](https://avatars.githubusercontent.com/devlooped-user?s=39 "Andrew Grant")](https://github.com/wizardness)
[![eska-gmbh](https://avatars.githubusercontent.com/devlooped-team?s=39 "eska-gmbh")](https://github.com/eska-gmbh)
[![Geodata AS](https://avatars.githubusercontent.com/u/5946299?v=4&s=39 "Geodata AS")](https://github.com/geodata-no)


<!-- sponsors.md -->
[![Sponsor this project](https://avatars.githubusercontent.com/devlooped-sponsor?s=118 "Sponsor this project")](https://github.com/sponsors/devlooped)

[Learn more about GitHub Sponsors](https://github.com/sponsors)

<!-- https://github.com/devlooped/sponsors/raw/main/footer.md -->
