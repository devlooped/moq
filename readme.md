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