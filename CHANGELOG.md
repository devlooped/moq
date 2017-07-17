# Moq Changelog

All notable changes to this project will be documented in this file.

The format is loosely based on [Keep a Changelog](http://keepachangelog.com/en/1.0.0/).


## 4.7.99 (2017-07-17)

#### Added

* Add `[NeutralResourcesLanguage]` to assembly info for portable library use (@benbillbob, #394)
* Add portable, [SourceLink](https://github.com/ctaggart/SourceLink)-ed debugging symbols (PDB) to NuGet package, enabling end users to step into Moq's source code (@stakx, #417)

#### Changed

* Move all hardcoded message strings to `Resources.resx` (@stakx, #403)
* Update package `Castle.Core` (DynamicProxy) from version 4.1.0 to 4.1.1 (@stakx, #416)
* Clean up and simplify the build process by merging separate .NET Framework and .NET Standard projects (@stakx, #417)
* Replace outdated `ReleaseNotes.md` with new `CHANGELOG.md` (@stakx, #423)

#### Fixed

* Fix member name typo in reflection code (@JohanLarsson, #389)
* Make `Interceptor` more thread-safe during `mock.Setup` (@stakx, #392)
* Make abstract events defined in classes work even when `CallBase` is true by suppressing `InvokeBase()` (@stakx, #395)
* Allow setting up null return values using `Mock.Of` (@stakx, #396)
* Allow `Mock<T>.Raise` to raise events on child mocks instead of raising no or the wrong event (@stakx, #397)
* Improve specificity of `Setup` / `Verify` exception messages for static members and extension methods (@stakx, #400)
* Prevent internal interception on a mock from changing its `DefaultValue` property (@vladonemo, #411)
* Prevent stack overflow in conditional setups (@stakx, #412)
* Fix `NullReferenceException` caused by internally relying on a mock's `IEnumerable` implementation (@stakx, #413)
* Improve method match accuracy in `ExtractProxyCall` so that the order of setting up methods in an hierarchy of interfaces does not matter (@stakx, #415) 
* Improve mockability of C++/CLI interfaces having custom modifiers (`modopt`, `modreq`) in their method signatures (@stakx, #416)
* Make types implementing the same generic type more than two times mockable (@stakx, #416)
* Fix misreported `Times` in verification error messages (@stakx, #417)


## 4.7.63 (2017-06-21)

#### Changed

* Ensure that `null` never matches an `It.IsRegex(…)` (@stakx, #385)

#### Fixed

* Fix mocking of non-virtual methods via `mock.As<TInterface>()` which was broken by #381 (@stakx, #387) 


## 4.7.58 (2017-06-21)

#### Fixed

* Fix formatting inconsistencies for array values in `MockException.Message` (@stakx, #380)
* Fix major "class method vs. interface method" bug introduced by #119 / commit 162a543 (@stakx, #381)
* Fix mocking for redeclared interface properties (get-set but get-only in base type) (@stakx, #382)


## 4.7.49 (2017-06-18)

#### Fixed

* Fix incorrect Castle.Core package reference in NuGet package specification (@stakx, #379)


## 4.7.46 (2017-06-18)

#### Changed

* Extend `EmptyDefaultValueProvider` so it understands multidimensional arrays (return empty multidimensional arrays instead of `null`) (@stakx, #360)
* Update package `Castle.Core` (DynamicProxy) from version 4.0.0 to 4.1.0, update `System` and `Microsoft` packages from versions 4.0.1 to 4.3.0 (@stakx, #369)
* Clean up and reduce usage of conditional compilation (`#if`) (@stakx, #378)

#### Fixed

* Ensure default mock names are (more) unique (@stakx, #359)
* Make `It.IsAny`, `It.IsNotNull` work for COM types (@stakx, #361)
* Fix equality check bug in `ExpressionKey` which meant Moq would sometimes ignore exact argument order in setups (@kostadinmarinov, #135; @stakx, #363)
* Ensure incorrect implementations of `ISerializable` are caught properly (@stakx, #370)
* Make event accessor recognition logic more correct (don't just rely on `add_` or `remove_` name prefixes) (@stakx, #376)


## 4.7.25 (2017-06-03)

#### Added

* Add new `setup.ReturnsAsync` and `setup.ThrowsAsync` overloads allowing you to specify a delay (@jochenz, #289)

#### Changed

* Migrate .NET Core project to the new `.csproj` format (@jeremymeng, #336)


## 4.7.12 (2017-05-30)

#### Fixed

* Add overload in `mock.Protected()` setups to enforce the old behavior of exact parameter matching (new behavior fails for specific overloads) (@80er, #347)


## 4.7.11 (2017-05-30)

Minor change only (fix typo in documentation)


## 4.7.10 (2017-05-10)

Minor change only (update project URL in NuGet package metadata)


## 4.7.9 (2017-04-29)

Minor change only (add download badge to `README.md`)


## 4.7.8 (2017-03-26)

Minor change only (update license URL in NuGet package metadata)


## 4.7.7 (2017-03-25)

#### Fixed

* Allow setting up of protected methods with nullable parameters (@RobSiklos, #200)
* Fix incorrect code example in XML documentation comment for `Mock.Verify` (@jcockhren, #333)
* Fix bug in `HasMatchingParameterTypes` which caused `Protected().Verify` to reject valid mocks (@jeremymeng, #335)


## 4.7.1 (2017-02-28)

Minor change only (fix typo in documentation)


## 4.7.0 (2017-02-22)

Minor change only (correcting Moq version after semver violation)
This version is the successor of 4.6.62-alpha.


## 4.6.62-alpha (2017-02-21)

This version is the predecessor of 4.7.0 and essentially a cleaned up merge of both 4.5.30 and 4.6.39-alpha.


----

**Note:** The release notes for some Moq versions 4.6.*-alpha are still missing. They need to be
reconstructed from the commit history and the GitHub issue archive. If you would like to help make
this changelog more complete, any pull requests towards that goal are appreciated!

----


## 4.5.30 (2017-01-09)

#### Fixed

* Fix issue in `ExpressionKey` that caused identical setups to no longer be considered the same (@MatKubicki, #313)


## 4.5.29 (2016-12-10)

#### Fixed

* Make the recently added `setup.ReturnsAsync(Func<TResult> result)` evaluate its argument lazily, like `setup.Returns` would (@SaroTasciyan, #309)


## 4.5.28 (2016-11-11)

#### Added

* New method `setup.ReturnsAsync(Func<TResult> result)` (@joeenzminger, @tlycken, #297)


## 4.5.23 (2016-10-11)

#### Fixed

* Improves comparison of call arguments in `ExpressionKey` so that setups for the same method but with differing arguments don't override each other (@MatKubicki, #291)


## 4.5.22 (2016-09-20)

#### Fixed

* Properly raise events when mocked type implements multiple interfaces (@bradreimer, #288)


## 4.5.21 (2016-08-12)

#### Fixed

* Fix `mock.Reset()` which so far forgot to also clear all existing calls in the interceptor (@anilkamath87, #277)


## 4.5.20 (2016-08-12)

#### Fixed

* Ensure that mocking of `System.Object` methods always works, even in strict mode (again) (@kolomanschaft, #280)


## 4.5.19 (2016-08-10)

#### Fixed

* Make `SetupAllProperties` work with `DefaultValue.Mock` in the presence of object graph loops (i.e. when a type contains a property having the same type) (@vladonemo, #245)
* Prevent invalid implementations of `ISerializable` from being mocked (as DynamicProxy cannot handle these) via the new `SerializableTypesValueProvider` decorator (@vladonemo, #245)


## 4.5.18 (2016-08-10)

#### Fixed

* Ensure that mocking of `System.Object` methods always works, even in strict mode (@kolomanschaft, #279)


## 4.5.16 (2016-07-18)

#### Added

* Allow mocking of `System.Object` methods (`ToString`, `Equals`, and `GetHashCode`) (@kolomanschaft, #250)


## 4.5.13 (2016-07-11)

#### Changed

* `Verify` exception messages should include actual array values for specific calls instead of just `T[]` or variable names (@hahn-kev, #264)


## 4.5.10 (2016-06-21)

#### Added

* Add helper classes `Capture`, `CaptureMatch<T>` to simplify parameter capture (@ocoanet, #251)


## 4.5.9 (2016-06-09)

Minor change only (remove download badge from `README.md`)


## 4.5.8 (2016-05-26)

Minor change only (add `.editorconfig` file)


## 4.5.7 (2016-05-26)

#### Changed

* Reference Castle.Core as a NuGet package dependency instead of ILMerge-ing it into Moq's own assembly. (@kzu)
* Update package GitInfo from version 1.1.14 to 1.1.15 to fix versioning issue with cached Git info (@kzu)

#### Removed

* Remove debugging symbols (PDB) from package since GitLink doesn't seem to be working (@kzu)


## 4.5.3 (2016-05-26)

#### Added

* New helper methods `Mock.Verify(params Mock[] mocks)` and `Mock.VerifyAll(...)` (@RehanSaeed, #238)


## 4.5.0 (2016-05-24)

Minor change only (update release notes in NuGet package metadata)
This version is the successor of 4.5.9-alpha.

## 4.5.9-alpha (2016-05-22)

This version is the predecessor of 4.5.0.

#### Fixed

* Fix broken hashing in `ExpressionKey` that causes Moq to not be able to distinguish method setups differing only in the exact argument order (@LeonidLevin, #262)


## 4.5.7-alpha (2016-05-22)

#### Removed

* Remove (for the time being) official statement in release notes that Moq supports .NET Core (@kzu)


## 4.5.6-alpha (2016-05-22)

#### Added

* Add `ReturnsAsync` and `ThrowsAsync` methods for sequential values setups (@abatishchev, #261)

#### Changed

* Migrate to .NET 4.5 and NuGet 3 (@kzu)
* Update package GitInfo from version 1.1.13 to 1.1.14 (@kzu)

### Removed

* Remove COM unit tests (@kzu)
* Remove legacy Silverlight unit test project (@kzu)
* Remove `[NeutralResourcesLanguage]` attribute from main assembly (@kzu)

#### Fixed

* Avoid race conditions / improve thread safety of `MockDefaultValueProvider` (@mizbrodin, #207)
* Fix a code typo in `ExpressionComparer` (@AppChecker, #242)
* When mocking delegates, copy parameter attributes so `out` parameters work correctly (@urasandesu, #255)


## 4.2.1510.2205 (2015-10-22)

#### Added

* Add Gitter badge to `README.md` (@kzu)

#### Changed

* Upgrade to Castle.Core version 3.3.3 (@MSK61, @kzu, #204)

#### Fixed

* Make Task Parallel Library (TPL) code Silverlight 5 compatible (@iskiselev, #148)
* Fox license hyperlink so it points to correct version of the BSD license (@chkpnt, #198)


## 4.2.1507.118 (2015-06-29)

#### Fixed

* Move Silverlight 5 assemblies in proper `lib\sl5` folder (instead of `lib\sl4`) in the NuGet package (@kzu)


## 4.2.1506.2515 (2015-06-25)

#### Added

* Add new `mock.Reset()` extension method (@ashmind, #138)

#### Fixed

* Allow `null` to match nullable value types (@pkpjpm, #185)


## 4.2.1506.2016 (2015-06-20)

#### Added

* Add NuGet badges to `README.md` (@kzu)
* Add generic type argument information in exception messages for easier debugging (@hasaki, #177)

#### Changed

* Migrate Silverlight support from version 4 to 5 (@kzu) 
* Upgrade source code to Visual Studio 2013 (@kzu)

#### Fixed

* Fix `NullReferenceException` when passing `null` to a nullable argument but trying to match it with a non-nullable `IsAny` (@benjamin-hodgson, #160)
* Fix infinite loop when calling `SetupAllProperties` for entities with a hierarchy (@Moq, #180)


## 4.2.1502.911 (2015-02-09)

#### Fixed

* `SetupAllProperties` doesn't setup properties that don't have a setter (@NGloreous, #137)
* Fix DynamicProxy-related `NotImplementedException` when mocking interfaces with `CallBase = true` (@rjasica, #145)


## 4.2.1409.1722 (2014-09-17) / 4.2.1408.717 (2014-08-07)

Minor change only (update NuGet metadata)
This version was released twice under different version numbers.


## 4.2.1408.619 (2014-08-06)


#### Changed

* Migrate to MSBuild version 12 (@kzu)
* Enable `mock.As<TInterface>()` syntax for all implemented interfaces (except COM interfaces) even after `mock.Object` has been called (@scott-xu, #119, #123)

#### Deprecated

* Change `README.md` to show the use of `AtMostOnce` in `Verify` phase instead of `Setup` phase (@kzu)

#### Fixed

* Ordered call issue with invocations with same arguments in `MockSequence` (@drieseng, #97)
* Update various hyperlinks in `README.md` (@kzu)
* Improve Moq's support for curried delegates and fix failing unit tests when compiling Moq with Roslyn (@theoy, #125)


## 4.2.1402.2112 (2014-02-21)

#### Added

* Allow mocks to have names (@pimterry, #76, #83)

#### Changed

* Make `InterfaceProxy` class publicly visible to reenable mocking of interfaces (@pimterry, #99)

#### Removed

* Remove commercial licenses offer in `README.md` (@kzu)

#### Fixed

* Improve thread safety by splitting the interceptor's context into global one and one for the current proxy invocation only (@MatKubicki, #80)


## 4.2.1312.1622 (2013-12-16)

#### Fixed

* Improve thread safety in `Interceptor` and fix multithreading tests (@MatKubicki, #68)


## 4.2.1312.1621 (2013-12-16)

#### Fixed

* Fix `NullReferenceException` when trying to get a default value for `Task<T>` when `T` is a reference type (@alextercete, #73)


## 4.2.1312.1615 (2013-12-16)

Minor change only (update release notes)


## 4.2.1312.1416 (2013-12-14)

#### Changed

* `InSequence` setups can now be used on the same mock instance (@halllo, #72)


## 4.2.1312.1323 (2013-12-13)

#### Changed

* Move extension methods in class `Moq.Language.Flow.IReturnsExtensions` to `Moq.ReturnsExtensions` for better discoverability (@jdom, #71)


## 4.2.1312.1319 (2013-12-13)

#### Added

* Add `ReturnsAsync` and `ThrowsAsync` setup methods for better async support (@Blewzman, #60) 
* Migrate `README.md` content from Google Code (@kzu)

#### Changed

* Change `EmptyDefaultValueProvider` so it does not return `null` for `Task` and `Task<T>` types, but completed, awaitable tasks (@alextercete, #66)


## 4.1.1311.615 (2013-11-06)

#### Added

* Add `Throws` method to sequential value setups for testing retry logic in web services (@kellyselden, #59)

#### Fixed

* Matchers should not ignore implicit type conversions (@svick, #56)


## 4.1.1309.1617 (2013-09-16) / 4.1.1309.919 (2013-09-09)

This version was released twice under different version numbers.

#### Fixed

* Fix regression bug surrounding `Mock.As<TInterface>()`: Prevent early initialization of mock from checking for delegate mocks. (@kzu, #54)


## 4.1.1309.801 (2013-09-08)

#### Added

* Allow resetting of all call counts (those that will be compared with `Times`) (@salfab, #55)


## 4.1.1309.800 (2013-09-08) / 4.1.1308.2321 (2013-08-23)

This version was released twice under different version numbers.

#### Added

* New `It.IsNotNull<T>` matcher (@Pjanssen, #40)
* Add covariant `IMock<out T>` interface to `Mock<T>` (@tkellogg, #44)

#### Changed

* Rename `Changelog.txt` to `ReleaseNotes.md` and inject the latter into the NuGet metadata (@Moq, #52)

#### Fixed

* Fix "collection modified" exception thrown as result of more methods being called on a mock while verifying method calls (@yonahw, #36)
* Fix `NullReferenceException` when subscribing to an event (@bittailor, #39)
* Fix thread safety issue (`IndexOutOfRangeException`) on setup (@stoo101, #51)


## 4.1.1308.2316

Minor change only (change title in NuGet package metadata)


## 4.1.1308.2120

#### Added

* Add capability for mocking delegates (event handlers) (@quetzalcoatl, #4)
* Allow `CallBase` for specific method / property (@srudin, #8)
* New matchers `It.IsIn` and `It.IsNotIn` (@rdingwall, #27)
* Add `.gitignore` file (@yorah, #10 / @FellicePollano, #30)
* Add new `Verify` method overload group that accepts a `Times` instance (@ChrisMissal, #34)

#### Changed

* Update Castle.Core assemblies from version 1.2.0.0 to 3.2.0, fetch Castle.Core via NuGet (@yorah, #11 / @kzu)
* Corrected Verify method behavior for generic methods calls (@Suremaker, #25)
* Split up `Interceptor.Intercept` into a set of 8 strategies, introduce `InterceptionAction`(@FellicePollano, #31)

#### Fixed

* Fix `SetupSequentialContext` to increment counter also after `Throws` (@lukas-ais, #7)
* Make `Mock.Of` work on properties with non-public setters (@yorah, #9, #19)
* Adding (and removing) handlers for events declared on interfaces when `CallBase = true` (@IharBury, #13)
* Distinguish between verification exception and mock crash (@quetzalcoatl, #16)
* Improve thread safety of `Interceptor` class (@FelicePollano, #29)


----

**Note:** Release notes in the above format are not available for ealier versions of Moq. The above
changelog entries have been reconstructed from the Git commit history. What follows below are the
original release notes, for which maintenance stopped around Moq version 4.5. They are nevertheless
included below as they go back further in time.

----


## 4.5

* Updated to .NET 4.5
* Dropped support for .NET < 4.5 and Silverlight
* Remove ILMerge. Depend on Castle NuGet package instead.

## 4.3

* Added support for Roslyn
* Automatically add implemented interfaces to mock

## 4.2

* Improved support for async APIs by making default value a completed task
* Added support for async Returns and Throws
* Improved mock invocation sequence testing
* Improved support for multi-threaded tests
* Added support for named mocks

## 4.1

* Added covariant `IMock<out T>` interface to `Mock<T>`
* Added `It.IsNotNull<T>`
* Fix: 'NullReferenceException when subscribing to an event'
* Added overloads to `Verify` to accept `Times` as a Method Group
* Feature request: `It.IsIn(..)`, `It.IsNotIn(...)`
* Corrected Verify method behavior for generic methods calls
* Differentiate verification error from mock crash
* Fix: Adding (and removing) handlers for events declared on interfaces works when `CallBase = true`.
* Update to latest Castle
* Fix: `Mock.Of` (Functional Syntax) doesn't work on properties with non-public setters
* Fix: Allow to use `CallBase` instead of `Returns`
* Fix: Solved Multi-threading issue - `IndexOutOfRangeException`
* Capability of mocking delegates (event handlers)

## 4.0

* Linq to Mocks: `Mock.Of<T>(x => x.Id == 23 && x.Title == "Rocks!")`
* Fixed issues:
  *  87	`BadImageFormatException` when using a mock with a Visual Studio generated Accessor object
  *  166	Unable to use a delegate to mock a function that takes 5 or more parameters.
  *  168	Call count failure message never says which is the actual invocation count
  *  175	`theMock.Object` failing on VS2010 Beta 1
  *  177	Generic constraint on interface method causes `BadImageFormatException` when getting `Object`.
  *  183	Display what invocations were recieved when the expected one hasn't been met
  *  186	Methods that are not virtual gives non-sense-exception message
  *  188	More `Callback` Overloads
  *  199	Simplify `SetupAllProperties` implementation to simply iterate and call `SetupProperty`
  *  200	Fluent mock does not honor parent mock `CallBase` setting.
  *  202	`Mock.Protected().Expect()` deprecated with no work-around
  *  204	Allow default return values to be specified (per-mock)
  *  205	Error calling `SetupAllProperties` for `Mock<IDataErrorInfo>`
  *  206	Linq-to-Mocks Never Returns on Implicit Boolean Property
  *  207	`NullReferenceException` thrown when using `Mocks.CreateQuery` with implicit boolean expression
  *  208	Can't setup a mock for method that accept lambda expression as argument.
  *  211	`SetupAllProperties` should return the `Mock<T>` instead of `void`. 
  *  223	When a method is defined to make the setup an asserts mock fails
  *  226	Can't raise events on mocked Interop interfaces
  *  229	`CallBase` is not working for virtual events
  *  238	Moq fails to mock events defined in F# 
  *  239	Use `Func` instead of `Predicate`
  *  250	4.0 Beta 2 regression - cannot mock `MethodInfo` when targetting .NET 4
  *  251	When a generic interface also implements a non-generic version, `Verify` does not work in some cases
  *  254	Unable to create mock of `EnvDTE.DTE`
  *  261	Can not use protected setter in public property
  *  267	Generic argument as dependency for method `Setup` overrides all previous method setups for a given method
  *  273	Attempting to create a mock thrown a Type Load exception. The message refers to an inaccessible interface.
  *  276	.Net 3.5 no more supported

## 3.0

* Silverlight support! Finally integrated Jason's Silverlight contribution! Issue #73
* Brand-new simplified event raising syntax (#130): `mock.Raise(foo => foo.MyEvent += null, new MyArgs(...));`
* Support for custom event signatures (not compatible with `EventHandler`): `mock.Raise(foo => foo.MyEvent += null, arg1, arg2, arg3);`
* Substantially improved property setter behavior: `mock.VerifySet(foo => foo.Value = "foo");` (also available for `SetupSet`
* Renamed `Expect*` with `Setup*`
* Vastly simplified custom argument matchers: `public int IsOdd() { return Match<int>.Create(v => i % 2 == 0); }`
* Added support for verifying how many times a member was invoked: `mock.Verify(foo => foo.Do(), Times.Never());`
* Added simple sample app named StoreSample
* Moved Stub functionality to the core API (`SetupProperty` and `SetupAllProperties`)
* Fixed sample ASP.NET MVC app to work with latest version
* Allow custom matchers to be created with a substantially simpler API
* Fixed issue #145 which prevented discrimination of setups by generic method argument types
* Fixed issue #141 which prevented ref arguments matching value types (i.e. a Guid)
* Implemented improvement #131: Add support for `It.IsAny` and custom argument matchers for `SetupSet`/`VerifySet`
* Implemented improvement #124 to render better error messages
* Applied patch from David Kirkland for improvement #125 to improve matching of enumerable parameters
* Implemented improvement #122 to provide custom errors for `Verify`
* Implemented improvement #121 to provide `null` as default value for `Nullable<T>`
* Fixed issue #112 which fixes passing a null argument to a mock constructor
* Implemented improvement #111 to better support params arguments
* Fixed bug #105 about improperly overwriting setups for property getter and setter
* Applied patch from Ihar.Bury for issue #99 related to protected expectations 
* Fixed issue #97 on not being able to use `SetupSet`/`VerifySet` if property did not have a getter
* Better integration with Pex (http://research.microsoft.com/en-us/projects/Pex/)
* Various other minor fixes (#134, #135, #137, #138, #140, etc.)

## 2.6

* Implemented Issue #55: We now provide a `mock.DefaultValue` = [`DefaultValue.Empty` | `DefaultValue.Mock`] which will provide the current behavior (default) or mocks for mockeable return types for loose mock invocations without expectations.
* Added support for stubbing properties from moq-contrib: now you can do `mock.Stub(m => m.Value)` and add stub behavior to the property. `mock.StubAll()` is also provided. This integrates with the `DefaultValue` behavior too, so you can stub entire hierarchies :).
* Added support for mocking methods with `out` and `ref` parameters (Issue #50)
* Applied patch contributed by slava for Issue #72: add support to limit numbor of calls on mocked method (we now have `mock.Expect(...).AtMost(5)`)
* Implemented Issue #94: Easier setter verification: Now we support `ExpectSet(m = m.Value, "foo")` and `VerifySet(m = m.Value, 5)` (Thanks ASP.NET MVC Team!)
* Implemented issue #96: Automatically chain mocks when setting expectations. It's now possible to specify expectations for an entire hierarchy of objects just starting from the root mock. THIS IS REALLY COOL!!!
* Fixed Issue #89: `Expects()` does not always return last expectation
* Implemented Issue 91: Expect a method/property to never be called (added `Never()` method to an expectation. Can be used on methods, property getters and setters)
* Fixed Issue 86: `IsAny<T>` should check if the value is actually of type `T`
* Fixed Issue 88: Cannot mock protected internal virtual methods using `Moq.Protected`
* Fixed Issue 90: Removing event handlers from mocked objects
* Updated demo and added one more test for the dynamic addition of interfaces

## 2.5

* Added support for mocking protected members
* Added new way of extending argument matchers which is now very straightforward
* Added support for mocking events
* Added support for firing events from expectations
* Removed usage of MBROs which caused inconsistencies in mocking features
* Added `ExpectGet` and `ExpectSet` to better support properties, and provide better intellisense.
* Added verification with expressions, which better supports Arrange-Act-Assert testing model (can do `Verify(m => m.Do(...)))`
* Added `Throws<TException>`
* Added `mock.CallBase` property to specify whether the virtual members base implementation should be called
* Added support for implementing and setting expectations and verifying additional interfaces in the mock, via the new `mock.As<TInterface>()` method (thanks Fernando Simonazzi!)
* Improved argument type matching for `Is`/`IsAny` (thanks Jeremy.Skinner!)

## 2.0

* Refactored fluent API on mocks. This may cause some existing tests to fail, but the fix is trivial (just reorder the calls to `Callback`, `Returns` and `Verifiable`)
* Added support for retrieving a `Mock<T>` from a `T` instance created by a mock.
* Added support for retrieving the invocation arguments from a `Callback` or `Returns`.
* Implemented `AtMostOnce()` constraint 
* Added support for creating MBROs with protected constructors
* Loose mocks now return default empty arrays and `IEnumerables` instead of `null`s

## 1.5.1

* Refactored `MockFactory` to make it simpler and more explicit to use with regards to verification. Thanks Garry Shutler for the feedback! 

## 1.5

* Added `MockFactory` to allow easy construction of multiple mocks with the same behavior and verification 

## 1.4

* Added support for passing constructor arguments for mocked classes.
* Improved code documentation 

## 1.3

 * Added support for overriding expectations set previously on a Mock. Now adding a second expectation for the same method/property call will override the existing one. This facilitates setting up default expectations in a fixture setup and overriding when necessary in a specific test.
 * Added support for mock verification. Both `Verify` and `VerifyAll` are provided for more flexibility (the former only verifies methods marked `Verifiable`) 

## 1.2

* Added support for `MockBehavior` mock constructor argument to affect the way the mocks expect or throw on calls. 

## 1.1

* Merged branch for dynamic types. Now Moq is based on Castle DynamicProxy2 to support a wider range of mock targets.
* Added ILMerge so that Castle libraries are merged into Moq assembly (no need for external references and avoid conflicts)

## 1.0

* Initial release, initial documentation process in place, etc.
