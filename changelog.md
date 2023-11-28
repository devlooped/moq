# Changelog

## [v4.20.70](https://github.com/devlooped/moq/tree/v4.20.70) (2023-11-28)

[Full Changelog](https://github.com/devlooped/moq/compare/v4.20.69...v4.20.70)

:hammer: Other:

- a minor changes to improve the readability [\#1419](https://github.com/devlooped/moq/issues/1419)
- Poll on SponsorLink [\#1415](https://github.com/devlooped/moq/issues/1415)
- Change log is not updated since version 4.18.4 [\#1406](https://github.com/devlooped/moq/issues/1406)
- Stop using Moq as a guinea pig to get feedback on and develop SponsorLink [\#1396](https://github.com/devlooped/moq/issues/1396)
- Permanently delete all data from SponsorLink's database that has been collected during builds that included Moq \(notably any version 4.20.\*\) [\#1395](https://github.com/devlooped/moq/issues/1395)
- SponsorLink is now OSS too and no longer bundled [\#1384](https://github.com/devlooped/moq/issues/1384)
- SponsorLink and supporting OSS more broadly [\#1374](https://github.com/devlooped/moq/issues/1374)
- Performance issue with large interfaces [\#1350](https://github.com/devlooped/moq/issues/1350)

:twisted_rightwards_arrows: Merged:

- A minor negation in GetDelay to make it more readable \#1419 [\#1422](https://github.com/devlooped/moq/pull/1422) (@iPazooki)
- Manually update CHANGELOG.md for now [\#1407](https://github.com/devlooped/moq/pull/1407) (@kzu)
- Restore GDPR compliance and privacy [\#1402](https://github.com/devlooped/moq/pull/1402) (@DanWillman)
- Improve performance for mocking interfaces: Cache GetInterfaceMap [\#1351](https://github.com/devlooped/moq/pull/1351) (@rauhs)

## [v4.20.69](https://github.com/devlooped/moq/tree/v4.20.69) (2023-08-11)

[Full Changelog](https://github.com/devlooped/moq/compare/v4.20.2...v4.20.69)

:hammer: Other:

- Trouble to unit test a lambda expression with Moq [\#1387](https://github.com/devlooped/moq/issues/1387)
- Strange System.UnauthorizedAccessException during build using latest version [\#1377](https://github.com/devlooped/moq/issues/1377)
- Privacy issues with SponsorLink, starting from version 4.20 [\#1372](https://github.com/devlooped/moq/issues/1372)
- Upgrading to version 4.20.1 breaks the build [\#1371](https://github.com/devlooped/moq/issues/1371)
- Warnings with latest version from SponsorLink [\#1370](https://github.com/devlooped/moq/issues/1370)
- Missing License Information In Nuget metadata [\#1348](https://github.com/devlooped/moq/issues/1348)

:twisted_rightwards_arrows: Merged:

- Remove AWS sponsorship from readme [\#1383](https://github.com/devlooped/moq/pull/1383) (@kzu)

## [v4.20.2](https://github.com/devlooped/moq/tree/v4.20.2) (2023-08-09)

[Full Changelog](https://github.com/devlooped/moq/compare/v4.20.1...v4.20.2)

:bug: Fixed bugs:

- Remove SponsorLink since it breaks MacOS restore [\#1375](https://github.com/devlooped/moq/pull/1375) (@kzu)

:hammer: Other:

- Running unit tests through Rider causes AD0001 : Analyzer 'Moq.SponsorLinker' threw an exception of type 'System.UnauthorizedAccessException' [\#1369](https://github.com/devlooped/moq/issues/1369)

## [v4.20.1](https://github.com/devlooped/moq/tree/v4.20.1) (2023-08-08)

[Full Changelog](https://github.com/devlooped/moq/compare/v4.20.0...v4.20.1)

## [v4.20.0](https://github.com/devlooped/moq/tree/v4.20.0) (2023-08-08)

[Full Changelog](https://github.com/devlooped/moq/compare/v4.20.0-rc...v4.20.0)

:hammer: Other:

- Documentation link in readme is broken [\#1349](https://github.com/devlooped/moq/issues/1349)

:twisted_rightwards_arrows: Merged:

- fix website url [\#1364](https://github.com/devlooped/moq/pull/1364) (@tibel)

## [v4.20.0-rc](https://github.com/devlooped/moq/tree/v4.20.0-rc) (2023-08-04)

[Full Changelog](https://github.com/devlooped/moq/compare/v4.20.0-beta...v4.20.0-rc)

:hammer: Other:

- Async event mocking hides exception  [\#1352](https://github.com/devlooped/moq/issues/1352)

:twisted_rightwards_arrows: Merged:

- Add 💜 SponsorLink support [\#1363](https://github.com/devlooped/moq/pull/1363) (@kzu)

## [v4.20.0-beta](https://github.com/devlooped/moq/tree/v4.20.0-beta) (2023-08-03)

[Full Changelog](https://github.com/devlooped/moq/compare/v4.20.0-alpha...v4.20.0-beta)

## [v4.20.0-alpha](https://github.com/devlooped/moq/tree/v4.20.0-alpha) (2023-08-03)

[Full Changelog](https://github.com/devlooped/moq/compare/v4.18.4...v4.20.0-alpha)

:sparkles: Implemented enhancements:

- Is there a `RaiseAsync` method for raising `Func<..., Task>` events? [\#1310](https://github.com/devlooped/moq/issues/1310)
- Provide a way to opt out of VerifyAll\(\) [\#937](https://github.com/devlooped/moq/issues/937)
- Set Times expectation on Setup [\#373](https://github.com/devlooped/moq/issues/373)
- Add `setup.Verifiable(Times times, [string failMessage])` method [\#1319](https://github.com/devlooped/moq/pull/1319) (@stakx)

:bug: Fixed bugs:

- Verifying a protected generic method that returns a value is broken [\#1314](https://github.com/devlooped/moq/issues/1314)

:hammer: Other:

- invalid path on linux [\#1353](https://github.com/devlooped/moq/issues/1353)
- Thread Safety issue in Moq reported by Infer\# report [\#1347](https://github.com/devlooped/moq/issues/1347)
- Moq should use CallBase to call default method from interfaces [\#1345](https://github.com/devlooped/moq/issues/1345)
- Functionality Request. Verify Mock using the Mock Setup as base. [\#1344](https://github.com/devlooped/moq/issues/1344)
- System.ArgumentException : Object of type 'Microsoft.DurableTask.TaskName' cannot be converted to type 'System.String' [\#1343](https://github.com/devlooped/moq/issues/1343)
- Appveyor \(CI\): Strong name signature verification error fails builds & blocks next version release [\#1340](https://github.com/devlooped/moq/issues/1340)
- Mocking derived classes of Exception results in .Object being null [\#1337](https://github.com/devlooped/moq/issues/1337)
- base class can not be mocked  [\#1332](https://github.com/devlooped/moq/issues/1332)
- Verify\(It.Is\<T\>\(t =\> t.SomeProperty.Equals\("SomeValue"\)\), Times.Exactly\(x\)\) isn't working as expected [\#1331](https://github.com/devlooped/moq/issues/1331)
- Add/support Target Framework net7.0 [\#1327](https://github.com/devlooped/moq/issues/1327)
- Mock method with Expression as parameter. [\#1324](https://github.com/devlooped/moq/issues/1324)
- `System.TypeLoadException : Could not load type 'Castle.Core.Internal.CollectionExtensions' from assembly 'Castle.Core, Version=5.0.0.0` after updating to Moq 4.18.4 [\#1320](https://github.com/devlooped/moq/issues/1320)
- Weird exception when mocking interface with SignalRHubAttribute applied [\#1308](https://github.com/devlooped/moq/issues/1308)

:twisted_rightwards_arrows: Merged:

- Revamp structure, apply oss template, cleanup projects/imports [\#1358](https://github.com/devlooped/moq/pull/1358) (@kzu)
- \#1340 updated appveyor.yml with workaround to make builds work again [\#1346](https://github.com/devlooped/moq/pull/1346) (@david-kalbermatten)
- Don't throw away generic type arguments in one `mock.Protected().Verify<T>()` method overload [\#1325](https://github.com/devlooped/moq/pull/1325) (@stakx)
- Use PackageLicenseExpression instead of PackageLicenseUrl [\#1322](https://github.com/devlooped/moq/pull/1322) (@wismann)
- Add `Mock<T>.RaiseAsync` [\#1313](https://github.com/devlooped/moq/pull/1313) (@stakx)
- Add `ThrowsAsync` for non-generic `ValueTask` [\#1235](https://github.com/devlooped/moq/pull/1235) (@johnthcall)

## [v4.18.4](https://github.com/devlooped/moq/tree/v4.18.4) (2022-12-30)

[Full Changelog](https://github.com/devlooped/moq/compare/v4.18.3...v4.18.4)

:sparkles: Implemented enhancements:

- Improve a exception message for extension methods. [\#1305](https://github.com/devlooped/moq/issues/1305)
- Mocking a method that takes an in parameter [\#1301](https://github.com/devlooped/moq/issues/1301)

:bug: Fixed bugs:

- .NET Hot Reload breaks mocked interfaces and throws `IndexOutOfRangeException` [\#1252](https://github.com/devlooped/moq/issues/1252)

:hammer: Other:

- Ignoring latest setup [\#1311](https://github.com/devlooped/moq/issues/1311)
- Invocation count incorrect. Argument object is stored as reference and is changed incorrectly [\#1309](https://github.com/devlooped/moq/issues/1309)
- Moq 4.18.3 Release still pending [\#1302](https://github.com/devlooped/moq/issues/1302)
- Moq fails when a method with input parameter of type Expression\<Func\<T,bool\>\> is called [\#1288](https://github.com/devlooped/moq/issues/1288)
- Use Moq as a proxy [\#1287](https://github.com/devlooped/moq/issues/1287)
- TargetParameterCountException when using event Raise with Moq [\#1285](https://github.com/devlooped/moq/issues/1285)
- Add CITATION.cff file [\#1266](https://github.com/devlooped/moq/issues/1266)
- `MissingMethodException` when using new C\# language features \(`in` parameters, `init`-only setters, etc.\) with generic methods, or with members in generic types [\#1148](https://github.com/devlooped/moq/issues/1148)

:twisted_rightwards_arrows: Merged:

- Update version to 4.18.4 [\#1318](https://github.com/devlooped/moq/pull/1318) (@stakx)
- Update DynamicProxy to version 5.1.1 [\#1317](https://github.com/devlooped/moq/pull/1317) (@stakx)

## [v4.18.3](https://github.com/devlooped/moq/tree/v4.18.3) (2022-12-05)

[Full Changelog](https://github.com/devlooped/moq/compare/v4.18.2...v4.18.3)

:bug: Fixed bugs:

- `SetupAllProperties` crashes when invoked on a `Mock<T>` subclass [\#1278](https://github.com/devlooped/moq/issues/1278)

:hammer: Other:

- Testing non virtual public methods [\#1303](https://github.com/devlooped/moq/issues/1303)
- Mocking dependency with `in` keyword parameter does not match [\#1292](https://github.com/devlooped/moq/issues/1292)
- System.InvalidProgramException : Cannot create boxed ByRef-like values. [\#1286](https://github.com/devlooped/moq/issues/1286)
- Does ReturnsAsnyc not available for protected setup? [\#1264](https://github.com/devlooped/moq/issues/1264)
- Mocking Static Abstract Interface Methods? [\#1238](https://github.com/devlooped/moq/issues/1238)

:twisted_rightwards_arrows: Merged:

- Update version to 4.18.3 [\#1306](https://github.com/devlooped/moq/pull/1306) (@stakx)
- Update moved artifacts in upstream repos [\#1298](https://github.com/devlooped/moq/pull/1298) (@kzu)
- Fix typo in the documentation of Mock`1.cs \#1 [\#1294](https://github.com/devlooped/moq/pull/1294) (@valentin-p)
- Let `StubbedPropertiesSetup` figure out type parameter in a less fragile way [\#1281](https://github.com/devlooped/moq/pull/1281) (@stakx)
- +M▼ includes [\#1280](https://github.com/devlooped/moq/pull/1280) (@github-actions[bot])
- Update and maintain list of sponsors automatically [\#1279](https://github.com/devlooped/moq/pull/1279) (@kzu)

## [v4.18.2](https://github.com/devlooped/moq/tree/v4.18.2) (2022-08-02)

[Full Changelog](https://github.com/devlooped/moq/compare/v4.18.1...v4.18.2)

:bug: Fixed bugs:

- "Expression is not an event add" when using .Raises\(\) [\#1175](https://github.com/devlooped/moq/issues/1175)

:hammer: Other:

- Improperly throws System.TypeLoadException on mock when a record has a base record on dotnet 6 [\#1273](https://github.com/devlooped/moq/issues/1273)
- Performance Issues After Version 4.12.0 [\#1269](https://github.com/devlooped/moq/issues/1269)
- Mixing "property behavior" and "Setup for properties" broken \(or at least changed\) in 4.17.1 [\#1265](https://github.com/devlooped/moq/issues/1265)
- MissingMethodException when mocking interface with sealed default implementation [\#1209](https://github.com/devlooped/moq/issues/1209)
- Can't raise non-virtual async event [\#977](https://github.com/devlooped/moq/issues/977)

:twisted_rightwards_arrows: Merged:

- Add regression test for interface with partial default implementation [\#1277](https://github.com/devlooped/moq/pull/1277) (@stakx)
- Add regression tests for subscribing to & raising redeclared event [\#1276](https://github.com/devlooped/moq/pull/1276) (@stakx)
- Upgrade DynamicProxy to version 5.1.0 for better `record` type support [\#1275](https://github.com/devlooped/moq/pull/1275) (@stakx)
- add STTE for older target frameworks only [\#1274](https://github.com/devlooped/moq/pull/1274) (@tibel)
- Update README.md [\#1271](https://github.com/devlooped/moq/pull/1271) (@harveer07)

## [v4.18.1](https://github.com/devlooped/moq/tree/v4.18.1) (2022-05-16)

[Full Changelog](https://github.com/devlooped/moq/compare/v4.18.0...v4.18.1)

:sparkles: Implemented enhancements:

- \[Feature Request\] Allow to change MockBehavior of mocks  [\#1230](https://github.com/devlooped/moq/issues/1230)
- Make Capture.In\<\>\(\) support System.Collections.Generic.Queue\<\>\(\) [\#1198](https://github.com/devlooped/moq/issues/1198)

:bug: Fixed bugs:

- Difference in behavior when mocking async method using .Result vs without [\#1253](https://github.com/devlooped/moq/issues/1253)
- Setup property doesn't work as expected [\#1248](https://github.com/devlooped/moq/issues/1248)
- Update from 4.13.1 to 4.16.1 lazy evaluation setups fail [\#1217](https://github.com/devlooped/moq/issues/1217)

:hammer: Other:

- Absence of a clause in the documentation about the lack of support for sealed methods [\#1256](https://github.com/devlooped/moq/issues/1256)
- moqthis.com is down [\#1244](https://github.com/devlooped/moq/issues/1244)
- Setup return value fails in a constellation with nullable ints [\#1223](https://github.com/devlooped/moq/issues/1223)

:twisted_rightwards_arrows: Merged:

- Leave quoted \(nested\) expressions unchanged when evaluating captured variables [\#1262](https://github.com/devlooped/moq/pull/1262) (@stakx)
- Update link to documentation website [\#1261](https://github.com/devlooped/moq/pull/1261) (@SeanKilleen)
- Make `StubbedPropertySetup.IsMatch` less picky [\#1260](https://github.com/devlooped/moq/pull/1260) (@stakx)
- Allow async `.Result` setups to return `null` [\#1259](https://github.com/devlooped/moq/pull/1259) (@stakx)

## [v4.18.0](https://github.com/devlooped/moq/tree/v4.18.0) (2022-05-11)

[Full Changelog](https://github.com/devlooped/moq/compare/v4.17.2...v4.18.0)

:bug: Fixed bugs:

- Failure when invoking a method with by-ref parameter & mockable return type on a mock with `CallBase` and `DefaultValue.Mock` configured [\#1249](https://github.com/devlooped/moq/issues/1249)
- Placeholder in exception is not filled out [\#1246](https://github.com/devlooped/moq/issues/1246)

:hammer: Other:

- sealed method mock calls actual method instead of mocked [\#1255](https://github.com/devlooped/moq/issues/1255)
- Unable to call base interface getter with cast without explicit setup. [\#1254](https://github.com/devlooped/moq/issues/1254)
- Moq is using Castle.Core which has an old version of System.Net.Http which is vulnerable to "DoS", "Spoofing", "Privilege Escalation", "Authentication Bypass" and "Information Exposure" [\#1219](https://github.com/devlooped/moq/issues/1219)
- Can't set up "private protected" properties [\#1170](https://github.com/devlooped/moq/issues/1170)

:twisted_rightwards_arrows: Merged:

- Upgrade DynamicProxy to version 5.0.0 [\#1257](https://github.com/devlooped/moq/pull/1257) (@stakx)
- Account for by-ref params in `MethodExpectation.CreateFrom(Invocation)` [\#1251](https://github.com/devlooped/moq/pull/1251) (@stakx)
- Fixed missing placeholder in exception message Resources.TypeNotMockable [\#1247](https://github.com/devlooped/moq/pull/1247) (@abatishchev)

## [v4.17.2](https://github.com/devlooped/moq/tree/v4.17.2) (2022-03-06)

[Full Changelog](https://github.com/devlooped/moq/compare/v4.17.1...v4.17.2)

:bug: Fixed bugs:

- Property stubs not working on sub mock [\#1240](https://github.com/devlooped/moq/issues/1240)

:hammer: Other:

- Add support for `success` condition parameter in `When`. [\#1237](https://github.com/devlooped/moq/issues/1237)

:twisted_rightwards_arrows: Merged:

- Make `SetupProperty` work with recursive expressions again [\#1241](https://github.com/devlooped/moq/pull/1241) (@stakx)

## [v4.17.1](https://github.com/devlooped/moq/tree/v4.17.1) (2022-02-26)

[Full Changelog](https://github.com/devlooped/moq/compare/v4.17.0...v4.17.1)

:bug: Fixed bugs:

- SetupAllProperties causes mocks to become race-prone [\#1231](https://github.com/devlooped/moq/issues/1231)
- Property setups are ignored on mocks instantiated using `Mock.Of` [\#1066](https://github.com/devlooped/moq/issues/1066)

:hammer: Other:

- It.IsSubtype\<T\> doesn't work when T has constraints  [\#1215](https://github.com/devlooped/moq/issues/1215)

:twisted_rightwards_arrows: Merged:

- Create dedicated setup type for `SetupAllProperties` [\#1234](https://github.com/devlooped/moq/pull/1234) (@stakx)

## [v4.17.0](https://github.com/devlooped/moq/tree/v4.17.0) (2022-02-12)

[Full Changelog](https://github.com/devlooped/moq/compare/v4.16.1...v4.17.0)

:sparkles: Implemented enhancements:

- Feature - stubbing indexers [\#1178](https://github.com/devlooped/moq/issues/1178)
- Access invocation arguments when using Throws/ThrowsAsync during Setup [\#1048](https://github.com/devlooped/moq/issues/1048)
- Enable multi-method setups [\#1203](https://github.com/devlooped/moq/pull/1203) (@stakx)

:bug: Fixed bugs:

- Moq.Verify cant detect Invocation when a method is passed [\#1225](https://github.com/devlooped/moq/issues/1225)
- implicit guard too strict; prevents valid user wildcard `_` implementations [\#1199](https://github.com/devlooped/moq/issues/1199)
- Parameter is invalid in Protected\(\).SetupSet\(\) method [\#1184](https://github.com/devlooped/moq/issues/1184)
- Issue mocking Vb.Net class with overloaded property in base class [\#1153](https://github.com/devlooped/moq/issues/1153)

:hammer: Other:

- Poor error message mocking a non-virtual Set property [\#1228](https://github.com/devlooped/moq/issues/1228)
- Calling .Raise raises the event on a new instance of the mocked object [\#1221](https://github.com/devlooped/moq/issues/1221)
- Null reference exception when using the "new" keyword with properties [\#1216](https://github.com/devlooped/moq/issues/1216)
- Specifying the expected number of occurrences in IsIn\(\) [\#1214](https://github.com/devlooped/moq/issues/1214)
- Moq 4.16.1 is not compatible with netcoreapp3.1 [\#1212](https://github.com/devlooped/moq/issues/1212)
- Using type matchers with `.Callback` and `.Returns` should be forbidden [\#1205](https://github.com/devlooped/moq/issues/1205)
- Add README to NuGet package [\#1194](https://github.com/devlooped/moq/issues/1194)
- `mock.Protected()` setup methods fail when argument is of type `Expression` [\#1188](https://github.com/devlooped/moq/issues/1188)
- Mock.Verify susceptible to reference type alterations [\#1187](https://github.com/devlooped/moq/issues/1187)
- System.CommandLine CLI parsing library won't call a method on a mocked object [\#1182](https://github.com/devlooped/moq/issues/1182)
- When testing a method which overrides a method in the base class, the unit test always passes. [\#1180](https://github.com/devlooped/moq/issues/1180)
- The type initializer for 'Moq.Async.AwaitableFactory' threw an exception:  Could not load file or assembly 'System.Threading.Tasks.Extensions, Version=4.2.0.1 [\#1179](https://github.com/devlooped/moq/issues/1179)
- Need a better way to manage parent class child class relationships [\#1176](https://github.com/devlooped/moq/issues/1176)
- Setup a method with argument of interface type doesn't work when a not null instance is passed to that method [\#1172](https://github.com/devlooped/moq/issues/1172)
- Extension methods \(here: ServiceProviderServiceExtensions.GetRequiredService\) may not be used in setup / verification expressions. [\#1171](https://github.com/devlooped/moq/issues/1171)
- High severity vulnerabilities identified with the dependency package used in Moq 4.16.1 [\#1169](https://github.com/devlooped/moq/issues/1169)
- Expected invocation on the mock should never have been performed, but was 1 times. when upgrading to the latest moq version 4.16.1 from 4.10.0 [\#1168](https://github.com/devlooped/moq/issues/1168)
- Missing setter functionality from ProtectedAsMock [\#1164](https://github.com/devlooped/moq/issues/1164)
- ProtectedAsMock issues [\#1162](https://github.com/devlooped/moq/issues/1162)
- Mock an auto-derived interface with all getter-only properties having settings, too [\#1158](https://github.com/devlooped/moq/issues/1158)
- Moq setup mismatches generic methods with derived generic arguments [\#1157](https://github.com/devlooped/moq/issues/1157)
- GetHashCode\(\) issues when called from inside constructor of mocked class [\#1156](https://github.com/devlooped/moq/issues/1156)
- MissingMethodException when Verify Method with Array of ValueTuple [\#1154](https://github.com/devlooped/moq/issues/1154)
- DefaultValue.Mock lazy initialization [\#1149](https://github.com/devlooped/moq/issues/1149)
- Test method throwing exception with C\# 9.0 init keyword [\#1147](https://github.com/devlooped/moq/issues/1147)
- Intermittent System.TypeLoadExceptions and System.Security.VerificationExceptions [\#1145](https://github.com/devlooped/moq/issues/1145)
- Using object parameter in callback function in place of It.IsAnyType throws exception [\#1137](https://github.com/devlooped/moq/issues/1137)
- Mocking a method with "in" parameter of generic type [\#1136](https://github.com/devlooped/moq/issues/1136)
- Get DocFX documentation online [\#1090](https://github.com/devlooped/moq/issues/1090)
- Issue with selecting a constructor with null value [\#969](https://github.com/devlooped/moq/issues/969)

:twisted_rightwards_arrows: Merged:

- Update `Castle.Core` \(DynamicProxy\) to version 4.4.1 [\#1233](https://github.com/devlooped/moq/pull/1233) (@stakx)
- Use same comparison logic in `LazyEvalMatcher` as in `ConstantMatcher` [\#1232](https://github.com/devlooped/moq/pull/1232) (@stakx)
- Create basic setup for Jekyll-based documentation website [\#1208](https://github.com/devlooped/moq/pull/1208) (@stakx)
- Disallow use of type matchers in `.Callback` and `.Returns` [\#1206](https://github.com/devlooped/moq/pull/1206) (@stakx)
- Combine `StubbedProperty{G|S}etterSetup` [\#1204](https://github.com/devlooped/moq/pull/1204) (@stakx)
- Make guard against unmatchable matchers less strict to enable user-based wildcard matching [\#1202](https://github.com/devlooped/moq/pull/1202) (@adamfk)
- Refactor `SetupCollection` towards a smaller & more consistent set of methods [\#1201](https://github.com/devlooped/moq/pull/1201) (@stakx)
- Fix IReturns xml doc [\#1192](https://github.com/devlooped/moq/pull/1192) (@adam-knights)
- Add new Throws overloads that allow arguments to be passed to it [\#1191](https://github.com/devlooped/moq/pull/1191) (@adam-knights)
- fix ProtectedMock fails when arguments are of type Expression \#1188 [\#1189](https://github.com/devlooped/moq/pull/1189) (@tonyhallett)
- Use `value` argument in `mock.Protected().SetupSet` and `...VerifySet` [\#1186](https://github.com/devlooped/moq/pull/1186) (@tonyhallett)
- Fix virtual properties and automocking for `mock.Protected().As<>()` [\#1185](https://github.com/devlooped/moq/pull/1185) (@tonyhallett)
- Add "set" methods to ProtectedAsMock [\#1165](https://github.com/devlooped/moq/pull/1165) (@tonyhallett)
- Fix `InvalidOperationException` when mocking class with overloaded property/indexer in base class [\#1155](https://github.com/devlooped/moq/pull/1155) (@stakx)

## [v4.16.1](https://github.com/devlooped/moq/tree/v4.16.1) (2021-02-23)

[Full Changelog](https://github.com/devlooped/moq/compare/v4.16.0...v4.16.1)

:sparkles: Implemented enhancements:

- Improve error message when verification fails [\#1138](https://github.com/devlooped/moq/issues/1138)
- Add .NET Standard 2.1 [\#1041](https://github.com/devlooped/moq/issues/1041)
- Support for Mock.Of\<T\> with not default constructor  [\#963](https://github.com/devlooped/moq/issues/963)
- Setup does not throw NotSupportedException when setting up sealed method seen through an interface [\#453](https://github.com/devlooped/moq/issues/453)

:bug: Fixed bugs:

- Property indexers raises ' System.Reflection.AmbiguousMatchException: Ambiguous match found.' exception. [\#1129](https://github.com/devlooped/moq/issues/1129)

:hammer: Other:

- Callback validation too strict when setting up a task's `.Result` property [\#1132](https://github.com/devlooped/moq/issues/1132)
- Performance with large interfaces [\#1128](https://github.com/devlooped/moq/issues/1128)
- Interface Default methods are ignored [\#972](https://github.com/devlooped/moq/issues/972)

:twisted_rightwards_arrows: Merged:

- Ensure `Returns(InvocationFunc)` doesn't throw `TargetInvocationException` [\#1141](https://github.com/devlooped/moq/pull/1141) (@stakx)
- Improve error message when verification fails [\#1140](https://github.com/devlooped/moq/pull/1140) (@bfriesen)
- Fix type mismatch during callback validation for task `.Result` setups [\#1133](https://github.com/devlooped/moq/pull/1133) (@stakx)
- Avoid `AmbiguousMatchException` when interface has indexer besides property [\#1131](https://github.com/devlooped/moq/pull/1131) (@mujdatdinc)
- `.CallBase` for default interface implementations [\#1130](https://github.com/devlooped/moq/pull/1130) (@stakx)
- Multitarget .NET Standard 2.1 [\#1042](https://github.com/devlooped/moq/pull/1042) (@twsl)

## [v4.16.0](https://github.com/devlooped/moq/tree/v4.16.0) (2021-01-16)

[Full Changelog](https://github.com/devlooped/moq/compare/v4.15.2...v4.16.0)

:sparkles: Implemented enhancements:

- Return matched invocation from `Verify` [\#1117](https://github.com/devlooped/moq/issues/1117)
- TypeNotMockable Exception [\#1112](https://github.com/devlooped/moq/issues/1112)
- Easier async setups through a new `Await(...)` operator [\#1007](https://github.com/devlooped/moq/issues/1007)
- Add ability to set up the `.Result` of \(value\) tasks [\#1126](https://github.com/devlooped/moq/pull/1126) (@stakx)

:bug: Fixed bugs:

- System.InvalidOperationException because of call to Verifiable in MockSequence after 4.14.0 [\#1114](https://github.com/devlooped/moq/issues/1114)

:hammer: Other:

- Please document generic argument matching support for versions prior to 4.13 [\#1119](https://github.com/devlooped/moq/issues/1119)
- Verify is not capturing a classes state at the time of invocation. [\#1118](https://github.com/devlooped/moq/issues/1118)
- Adding a setup looks like it became an O\(n\*n\) operation since \#984 was merged [\#1110](https://github.com/devlooped/moq/issues/1110)
- DbSet mocking is broken [\#1091](https://github.com/devlooped/moq/issues/1091)

:twisted_rightwards_arrows: Merged:

- Create and deconstruct awaitables using dedicated factories \(`IAwaitableFactory`\) [\#1125](https://github.com/devlooped/moq/pull/1125) (@stakx)
- Abandon 'Try' pattern in verification methods [\#1122](https://github.com/devlooped/moq/pull/1122) (@stakx)
- Allow marking conditional setups as `.Verifiable()` once again [\#1121](https://github.com/devlooped/moq/pull/1121) (@stakx)
- Changed TypeNotMockable exception message [\#1116](https://github.com/devlooped/moq/pull/1116) (@jacker92)
- Fix for slow down with many setups on single mock [\#1111](https://github.com/devlooped/moq/pull/1111) (@CeesKaas)

## [v4.15.2](https://github.com/devlooped/moq/tree/v4.15.2) (2020-11-26)

[Full Changelog](https://github.com/devlooped/moq/compare/v4.15.0...v4.15.2)

:hammer: Other:

- moq is not compatible with System.Threading.Tasks.Extensions 4.5.3 or 4.5.4 [\#1107](https://github.com/devlooped/moq/issues/1107)
- Hi There, I just updated Moq package from version 4.14.7 to version 4.15.1 and noticed that it broke Unit Tests for my projects. Kindly look into this matter. Thanks in advance, Regards, Asheesh Agrawal [\#1103](https://github.com/devlooped/moq/issues/1103)
- VerifyNoOtherCalls is failing a lot for existing tests due to unverified event registrations [\#1102](https://github.com/devlooped/moq/issues/1102)

:twisted_rightwards_arrows: Merged:

- Updating System.Threading.Tasks.Extensions to 4.5.4 [\#1108](https://github.com/devlooped/moq/pull/1108) (@JeffAshton)

## [v4.15.0](https://github.com/devlooped/moq/tree/v4.15.0) (2020-11-10)

[Full Changelog](https://github.com/devlooped/moq/compare/v4.14.7...v4.15.0)

:sparkles: Implemented enhancements:

- `IInvocation` should expose `Exception` along with `ReturnValue` [\#1070](https://github.com/devlooped/moq/issues/1070)
- Capture with ProtectedSetup [\#934](https://github.com/devlooped/moq/issues/934)
- Add support for nested type matchers [\#919](https://github.com/devlooped/moq/issues/919)

:bug: Fixed bugs:

- `SetupProperty` fails if property getter and setter are not both defined in mocked type [\#1017](https://github.com/devlooped/moq/issues/1017)
- Failure when parameterized `Mock.Of<>` is used in query comprehension `from` clause [\#982](https://github.com/devlooped/moq/issues/982)

:hammer: Other:

- Slower performance after upgrading from 4.2.1510 to 4.14.7 [\#1083](https://github.com/devlooped/moq/issues/1083)
- Remove mandatory SetupAdd & SetupRemove for eventhandler subscription verification. [\#1058](https://github.com/devlooped/moq/issues/1058)
- Issue comparing Expressions when one value is a constant [\#1054](https://github.com/devlooped/moq/issues/1054)
- Mock.As behavior change [\#1051](https://github.com/devlooped/moq/issues/1051)
- Tag Mock.Object as NotNull \(JetBrains.Annotations\) [\#1043](https://github.com/devlooped/moq/issues/1043)
- MissingMethodException on virtual method with in parameter [\#988](https://github.com/devlooped/moq/issues/988)

:twisted_rightwards_arrows: Merged:

- Add a link to sources used for build to description [\#1101](https://github.com/devlooped/moq/pull/1101) (@kzu)
- Update version to 4.15.0 [\#1100](https://github.com/devlooped/moq/pull/1100) (@stakx)
- Minor simplifications [\#1098](https://github.com/devlooped/moq/pull/1098) (@kzu)
- Add support for nested type matchers [\#1092](https://github.com/devlooped/moq/pull/1092) (@stakx)
- Add Discord channel link to improve the experience over Gitter [\#1089](https://github.com/devlooped/moq/pull/1089) (@kzu)
- Enable parameterized `Mock.Of<>` in query comprehension `from` clause  [\#1085](https://github.com/devlooped/moq/pull/1085) (@stakx)
- Simplify event subscription and remove a remaining inconsistency [\#1084](https://github.com/devlooped/moq/pull/1084) (@stakx)
- Always record calls to += and -= event accessors [\#1082](https://github.com/devlooped/moq/pull/1082) (@stakx)
-  Evaluate all captured variables when comparing LINQ expressions  [\#1081](https://github.com/devlooped/moq/pull/1081) (@stakx)
- Fix `SetupProperty` for split properties \(where mocked type overrides only the getter\) [\#1079](https://github.com/devlooped/moq/pull/1079) (@stakx)
- Add exception property to IInvocation [\#1077](https://github.com/devlooped/moq/pull/1077) (@MaStr11)
- Make `Mock.Of<>` work with COM interop types that are annotated with `[CompilerGenerated]` [\#1076](https://github.com/devlooped/moq/pull/1076) (@stakx)
- Don't require inner mock setups to be matched [\#1075](https://github.com/devlooped/moq/pull/1075) (@stakx)
- Make `DefaultValue.Mock` inherit property stubbing [\#1074](https://github.com/devlooped/moq/pull/1074) (@stakx)
- Remove superfluous `Convert` nodes added by VB.NET in cases involving constrained generic type parameters [\#1068](https://github.com/devlooped/moq/pull/1068) (@stakx)
- Implement It.Is, It.IsIn, It.IsNotIn with a comparer overload [\#1064](https://github.com/devlooped/moq/pull/1064) (@weitzhandler)
- Add .NET Framework reference assembly packages [\#1063](https://github.com/devlooped/moq/pull/1063) (@stakx)
- Add ReturnValue to IInvocation  [\#921](https://github.com/devlooped/moq/pull/921) (@MaStr11)

## [v4.14.7](https://github.com/devlooped/moq/tree/v4.14.7) (2020-10-14)

[Full Changelog](https://github.com/devlooped/moq/compare/v4.14.6...v4.14.7)

:sparkles: Implemented enhancements:

- New It.Is overload for matching values using custom IEqualityComparer [\#1059](https://github.com/devlooped/moq/issues/1059)
- Feature request: Promote Invocation.ReturnValue to IInvocation [\#920](https://github.com/devlooped/moq/issues/920)

:bug: Fixed bugs:

- Verify\(\) fails on items not explicitely marked a Verifiable\(\) [\#1073](https://github.com/devlooped/moq/issues/1073)
- Mock.Of of Excel interop classes fails to initialize [\#1072](https://github.com/devlooped/moq/issues/1072)
- Setters on deep mocks no longer work after updating to 4.14.6 from old version  [\#1071](https://github.com/devlooped/moq/issues/1071)
- Callback not raised in mocked subclass [\#1067](https://github.com/devlooped/moq/issues/1067)

:hammer: Other:

- Unable to chain ReturnsAsync with Verifiable [\#1057](https://github.com/devlooped/moq/issues/1057)
- `mocked.Equals(mocked)` returns false by default if mocked class overrides `Equals` [\#802](https://github.com/devlooped/moq/issues/802)

## [v4.14.6](https://github.com/devlooped/moq/tree/v4.14.6) (2020-09-30)

[Full Changelog](https://github.com/devlooped/moq/compare/v4.14.5...v4.14.6)

:bug: Fixed bugs:

- Mock.Of\<T\>\(\(m\) =\> ...\) doesn't play well with Microsoft IOptions\<T\> [\#1039](https://github.com/devlooped/moq/issues/1039)

:hammer: Other:

- Unable to build Moq: "The `Microsoft.Build.Tasks.Git.LocateRepository` task failed unexpectedly." [\#1060](https://github.com/devlooped/moq/issues/1060)
- \[Feature request\] Loose mode for commands and strict for queries. [\#1056](https://github.com/devlooped/moq/issues/1056)
- ThrowsAnyAsync\<\> not validating type [\#1050](https://github.com/devlooped/moq/issues/1050)
- Mock methods that take or return a Span\<T\> [\#1049](https://github.com/devlooped/moq/issues/1049)
- Feature request: enable verification of extension-methods calls [\#1045](https://github.com/devlooped/moq/issues/1045)

:twisted_rightwards_arrows: Merged:

- Update SourceLink build package to 1.0.0 [\#1062](https://github.com/devlooped/moq/pull/1062) (@stakx)
- Fix setting of nested non-overridable properties via `Mock.Of` [\#1061](https://github.com/devlooped/moq/pull/1061) (@stakx)

## [v4.14.5](https://github.com/devlooped/moq/tree/v4.14.5) (2020-07-01)

[Full Changelog](https://github.com/devlooped/moq/compare/v4.14.4...v4.14.5)

:bug: Fixed bugs:

- VerifySet breaks with System.NullReferenceException for WriteOnly-Indexed-Properties [\#1036](https://github.com/devlooped/moq/issues/1036)

:twisted_rightwards_arrows: Merged:

- Fix `NullReferenceException` when using write-only indexer with `VerifySet` [\#1037](https://github.com/devlooped/moq/pull/1037) (@stakx)
- Convert `SequenceSetup` to use `Behavior`s extracted from `MethodCall` [\#1035](https://github.com/devlooped/moq/pull/1035) (@stakx)
- Simplify `Invocation` protocol [\#1034](https://github.com/devlooped/moq/pull/1034) (@stakx)
- Simplify `MethodCall`'s response types [\#1033](https://github.com/devlooped/moq/pull/1033) (@stakx)

## [v4.14.4](https://github.com/devlooped/moq/tree/v4.14.4) (2020-06-24)

[Full Changelog](https://github.com/devlooped/moq/compare/v4.14.3...v4.14.4)

:bug: Fixed bugs:

- NullReferenceException on subsequent setup if expression contains null reference [\#1031](https://github.com/devlooped/moq/issues/1031)

:hammer: Other:

- Do we still need to use DynamicProxy's `AttributesToAvoidReplicating`? [\#1026](https://github.com/devlooped/moq/issues/1026)

:twisted_rightwards_arrows: Merged:

- Expression execution during override detection must not stop setup from being added [\#1032](https://github.com/devlooped/moq/pull/1032) (@stakx)
- Remove unneeded conditional compilation [\#1030](https://github.com/devlooped/moq/pull/1030) (@stakx)
- No longer need to use `AttributesToAvoidReplicating` [\#1029](https://github.com/devlooped/moq/pull/1029) (@stakx)
- Upgrade test projects to `netcoreapp3.1` [\#1028](https://github.com/devlooped/moq/pull/1028) (@stakx)

## [v4.14.3](https://github.com/devlooped/moq/tree/v4.14.3) (2020-06-18)

[Full Changelog](https://github.com/devlooped/moq/compare/v4.14.2...v4.14.3)

:bug: Fixed bugs:

- Verify behavior change using DefaultValue.Mock 4.12.0 vs 4.13.0 [\#1024](https://github.com/devlooped/moq/issues/1024)

:twisted_rightwards_arrows: Merged:

- Mark setups resulting from `DefaultValue.Mock` as matched [\#1027](https://github.com/devlooped/moq/pull/1027) (@stakx)

## [v4.14.2](https://github.com/devlooped/moq/tree/v4.14.2) (2020-06-16)

[Full Changelog](https://github.com/devlooped/moq/compare/v4.14.1...v4.14.2)

:hammer: Other:

- API Documentation page broken. [\#1023](https://github.com/devlooped/moq/issues/1023)
- System.IO.FileNotFoundException thrown when referencing same DLL name with 2 different assembly versions within a single application [\#1019](https://github.com/devlooped/moq/issues/1019)

:twisted_rightwards_arrows: Merged:

- `DefaultValue.Mock` should not add redundant setups for already-matched invocations [\#1025](https://github.com/devlooped/moq/pull/1025) (@stakx)

## [v4.14.1](https://github.com/devlooped/moq/tree/v4.14.1) (2020-04-28)

[Full Changelog](https://github.com/devlooped/moq/compare/v4.14.0...v4.14.1)

:sparkles: Implemented enhancements:

- ISetupSequentialResult doesn't support Pass for async void methods. [\#993](https://github.com/devlooped/moq/issues/993)

:bug: Fixed bugs:

- StackOverflowException on VerifyAll when mocked method returns mocked object [\#1012](https://github.com/devlooped/moq/issues/1012)

:hammer: Other:

- Function match should catch exception? [\#1010](https://github.com/devlooped/moq/issues/1010)
- NullReferenceException in IMatcher.Matches [\#1005](https://github.com/devlooped/moq/issues/1005)
- Async Method Support Clean-up [\#384](https://github.com/devlooped/moq/issues/384)

:twisted_rightwards_arrows: Merged:

- Prevent stack overflow when verifying cyclic mock object graph [\#1014](https://github.com/devlooped/moq/pull/1014) (@stakx)
- Clean up setup execution method [\#1013](https://github.com/devlooped/moq/pull/1013) (@stakx)
- Update Moq.csproj [\#1011](https://github.com/devlooped/moq/pull/1011) (@Saibamen)
- Added method overloads to ISetupSequentialResult async void methods [\#1006](https://github.com/devlooped/moq/pull/1006) (@fuzzybair)

## [v4.14.0](https://github.com/devlooped/moq/tree/v4.14.0) (2020-04-24)

[Full Changelog](https://github.com/devlooped/moq/compare/v4.13.1...v4.14.0)

:sparkles: Implemented enhancements:

- Expose Moq.Times' Verify and Kind [\#970](https://github.com/devlooped/moq/issues/970)
- After updating moq from 4.10.1 to 4.11, mocking nhibernate session throws a System.NullReferenceException. [\#955](https://github.com/devlooped/moq/issues/955)
- `Callback` and `Returns`: Allow callback parameters to have type `object` where a type matcher was used [\#953](https://github.com/devlooped/moq/issues/953)
- Expose `Mock.Setups`, part 4: Mock, fluent setup & inner mock discovery [\#989](https://github.com/devlooped/moq/pull/989) (@stakx)
- add support for mocking protected generic methods [\#967](https://github.com/devlooped/moq/pull/967) (@JmlSaul)

:bug: Fixed bugs:

- Capture.In during Verify stopped working when upgrading Moq from 4.10.1 to 4.13.1  [\#968](https://github.com/devlooped/moq/issues/968)

:hammer: Other:

- Build fails with error CS8751: Internal error in the C\# compiler [\#994](https://github.com/devlooped/moq/issues/994)
- Call times verification change from 4.11.0 [\#990](https://github.com/devlooped/moq/issues/990)
- Mocked method call with multiple specific parameter call returns null [\#973](https://github.com/devlooped/moq/issues/973)
- IDE0008 Use explicit type instead of 'var' [\#971](https://github.com/devlooped/moq/issues/971)
- Maintainer on vacation until mid-March 2020 [\#966](https://github.com/devlooped/moq/issues/966)
- System.NullReferenceException : Object reference not set to an instance of an object. [\#965](https://github.com/devlooped/moq/issues/965)
- How do you mock an IAsyncEnumerable\<T\> ? [\#962](https://github.com/devlooped/moq/issues/962)
- Update from 4.9.0 to upper version breaks Setup out-param method [\#960](https://github.com/devlooped/moq/issues/960)
- If When\(Func\<bool\> condition\) is used to setup a mock, the setup verification is skipped. [\#959](https://github.com/devlooped/moq/issues/959)

:twisted_rightwards_arrows: Merged:

- Stop searching for setup when first match found [\#1004](https://github.com/devlooped/moq/pull/1004) (@stakx)
- Evaluate captured variables during expression tree comparison [\#1000](https://github.com/devlooped/moq/pull/1000) (@stakx)
- Expose `Mock.Setups`, part 6: Replace `OriginalSetup` with simpler `OriginalExpression` [\#999](https://github.com/devlooped/moq/pull/999) (@stakx)
- Fix silent bugs in `UpgradePropertyAccessorMethods` [\#998](https://github.com/devlooped/moq/pull/998) (@stakx)
- Make `conditionalSetup.Verifiable()` an error [\#997](https://github.com/devlooped/moq/pull/997) (@stakx)
- Expose `Mock.Setups`, part 5: Simplify fluent setup & inner mock discovery [\#995](https://github.com/devlooped/moq/pull/995) (@stakx)
- Allow `Func<IInvocation,>` callback with `Returns` [\#992](https://github.com/devlooped/moq/pull/992) (@stakx)
- Expose `Mock.Setups`, part 3: Individual setup verification [\#987](https://github.com/devlooped/moq/pull/987) (@stakx)
- Fix regression when verifying `SetupAllProperties`' setups [\#986](https://github.com/devlooped/moq/pull/986) (@stakx)
- Expose `Mock.Setups`, part 2: Match status of setups & invocations [\#985](https://github.com/devlooped/moq/pull/985) (@stakx)
- Expose `Mock.Setups`, part 1: Basics [\#984](https://github.com/devlooped/moq/pull/984) (@stakx)
- Make internal verification API more generic than `Verify()`, `VerifyAll()` [\#983](https://github.com/devlooped/moq/pull/983) (@stakx)
- `Mock.Of<Class>` shouldn't delete invocations made from `Class`' ctor [\#980](https://github.com/devlooped/moq/pull/980) (@stakx)
- Retire `FluentMockVisitor` [\#978](https://github.com/devlooped/moq/pull/978) (@stakx)
- Add `Times.Validate(count)` & `Times.ToString()` [\#975](https://github.com/devlooped/moq/pull/975) (@stakx)
- Get `Capture.In` working again in `Verify` expressions [\#974](https://github.com/devlooped/moq/pull/974) (@stakx)

## [v4.13.1](https://github.com/devlooped/moq/tree/v4.13.1) (2019-10-19)

[Full Changelog](https://github.com/devlooped/moq/compare/v4.13.0...v4.13.1)

:bug: Fixed bugs:

- `mock.SetupProperty` throws `NullReferenceException` when called for partially overridden property [\#951](https://github.com/devlooped/moq/issues/951)
- Recursive mocks with different paths [\#946](https://github.com/devlooped/moq/issues/946)
- Original mocked type not included after downcasting and Get  [\#943](https://github.com/devlooped/moq/issues/943)
- System.ArgumentException: Interface not found in v.4.13.0 but not before. [\#942](https://github.com/devlooped/moq/issues/942)
- Regression in 4.13.0 [\#932](https://github.com/devlooped/moq/issues/932)
- `SetupAllProperties` does not recognize property as read-write if only setter is overridden [\#886](https://github.com/devlooped/moq/issues/886)
- AmbiguousMatchException when setting up the property, that hides another one [\#939](https://github.com/devlooped/moq/pull/939) (@ishatalkin)

:hammer: Other:

- Test method throw exeption Castle.DynamicProxy.ProxyGenerationException [\#941](https://github.com/devlooped/moq/issues/941)
- Controller Mock return null for ActionResult other return types like int working fine [\#936](https://github.com/devlooped/moq/issues/936)
- Method not found: 'Void Castle.DynamicProxy.ProxyGenerationOptions.AddDelegateTypeMixin\(System.Type\)'.. [\#935](https://github.com/devlooped/moq/issues/935)
- Ability to evaluate "verify" after a certain period of time [\#931](https://github.com/devlooped/moq/issues/931)
- Problem when mocking an interface with hidden property of different type [\#930](https://github.com/devlooped/moq/issues/930)
- VerifyAll\(\) setup for mock that no method was called [\#929](https://github.com/devlooped/moq/issues/929)
- Issue while calling Setup \(mocked\) method which is called from a method that is in Task Factory c\# [\#927](https://github.com/devlooped/moq/issues/927)
- Quickstart guide add callback which sets mock setup property [\#926](https://github.com/devlooped/moq/issues/926)
- Can not instantiate proxy of class [\#924](https://github.com/devlooped/moq/issues/924)
- Getting value of property through read-only interface returns default [\#923](https://github.com/devlooped/moq/issues/923)
- 4.13 callback with returns and out parameter ~ bug? [\#917](https://github.com/devlooped/moq/issues/917)

:twisted_rightwards_arrows: Merged:

- Fix `SetupProperty` for partially overridden properties [\#952](https://github.com/devlooped/moq/pull/952) (@stakx)
- Ensure `SetupAllProperties` stubs all accessors of partially overridden properties [\#950](https://github.com/devlooped/moq/pull/950) (@stakx)
- Let `InnerMockSetup` \(i.e. internal setups for cached return values\) match generic args exactly instead of by assignment compatibility. [\#949](https://github.com/devlooped/moq/pull/949) (@stakx)
- Match `params` arrays in setup/verification expressions using structural equality [\#948](https://github.com/devlooped/moq/pull/948) (@stakx)
- Enable `mock.As<>` round-tripping [\#945](https://github.com/devlooped/moq/pull/945) (@stakx)
- Use correct proxy type in `InterfaceProxy` [\#944](https://github.com/devlooped/moq/pull/944) (@stakx)

## [v4.13.0](https://github.com/devlooped/moq/tree/v4.13.0) (2019-08-31)

[Full Changelog](https://github.com/devlooped/moq/compare/v4.12.0...v4.13.0)

:sparkles: Implemented enhancements:

- Method without setup not return null [\#897](https://github.com/devlooped/moq/issues/897)
- Support expression-based mock creation for constructor calls that takes parameters [\#884](https://github.com/devlooped/moq/issues/884)
- Provide fluent interface [\#833](https://github.com/devlooped/moq/issues/833)
- Need a way to verify that code added an event handler to an event [\#825](https://github.com/devlooped/moq/issues/825)
- Invocations.Clear\(\) does not cause Verify to fail. [\#733](https://github.com/devlooped/moq/issues/733)
- Open Generic Method Callback [\#343](https://github.com/devlooped/moq/issues/343)
- \#825 event handlers setup and verify [\#857](https://github.com/devlooped/moq/pull/857) (@lepijohnny)

:bug: Fixed bugs:

- Parameter types are ignored when matching an invoked generic method against setups [\#903](https://github.com/devlooped/moq/issues/903)
- Regression in 4.12.0: `SetupAllProperties` resets indexer setups [\#901](https://github.com/devlooped/moq/issues/901)
- Moq does not distinguish between distinct events if they have the same name [\#893](https://github.com/devlooped/moq/issues/893)
- Verify throws TargetInvocationException instead of MockException when SetupSequence includes returning a Task.FromException and the mocked method is not called enough times. [\#883](https://github.com/devlooped/moq/issues/883)
- MockDefaultValueProvider will try to set 'Callbase' on auto-generated mocks for delegates. [\#874](https://github.com/devlooped/moq/issues/874)
- SetupAllProperties\(\) and Mock.Of\<T\> regression in Moq 4.12.0 [\#870](https://github.com/devlooped/moq/issues/870)
- Moq does not mock 'Explicit Interface Implementation' and 'protected virtual' correctly [\#657](https://github.com/devlooped/moq/issues/657)

:hammer: Other:

- Strange behavior with dynamic + indexed property [\#913](https://github.com/devlooped/moq/issues/913)
- Permission to wrap & distribute as a COM library under GPLv3? [\#910](https://github.com/devlooped/moq/issues/910)
- Check ReturnsAsync method for null arguments. [\#909](https://github.com/devlooped/moq/issues/909)
- New method matching algorithm introduces behavioral change wrt return types [\#906](https://github.com/devlooped/moq/issues/906)
- VerifyNoOtherCalls fails due to calls on another Mock instance [\#892](https://github.com/devlooped/moq/issues/892)
- Feature to reset setups and event handlers? [\#889](https://github.com/devlooped/moq/issues/889)
- Behavioural change between 4.11 -\> 4.12 with CallBase=True and params args [\#877](https://github.com/devlooped/moq/issues/877)
- Project with Moq and explicit dependency on higher System.Threading.Tasks.Extension throw exception!! [\#873](https://github.com/devlooped/moq/issues/873)
- Callback with specific object type fails when not after a Return [\#872](https://github.com/devlooped/moq/issues/872)
- Regression for event subscription between 4.10 and 4.11 [\#867](https://github.com/devlooped/moq/issues/867)
- Linux compilation fails using Mono on Moq 4.8+ [\#864](https://github.com/devlooped/moq/issues/864)
- Intermittent AccessViolationException in Mock\<\>.Object [\#860](https://github.com/devlooped/moq/issues/860)
- Allow specifying action if setup was not met in strict mode [\#856](https://github.com/devlooped/moq/issues/856)
- System.Threading.Tasks.Dataflow FileLoadException  [\#855](https://github.com/devlooped/moq/issues/855)
- Usage of Nunit SameAsConstraint \(Is.SameAs\(...\)\) in mock verification leads to incorrect result [\#853](https://github.com/devlooped/moq/issues/853)
- Improve "Member x does not exist" failure message [\#852](https://github.com/devlooped/moq/issues/852)
- SetupAllProperties and SetupProperty behave differently with respect to verification [\#850](https://github.com/devlooped/moq/issues/850)

:twisted_rightwards_arrows: Merged:

- Retire `.xdoc` XML documentation files [\#916](https://github.com/devlooped/moq/pull/916) (@stakx)
- Add special handling for `.ReturnsAsync(null)` [\#915](https://github.com/devlooped/moq/pull/915) (@stakx)
- Improve precision of event accessor method resolution [\#914](https://github.com/devlooped/moq/pull/914) (@stakx)
- Fix type cast bug in `ParamArrayMatcher` [\#912](https://github.com/devlooped/moq/pull/912) (@stakx)
- Simplify code in `Match.Matches` and `It.IsAny<T>` [\#911](https://github.com/devlooped/moq/pull/911) (@stakx)
- Add support for generic type argument matchers \(It.IsAnyType and custom matchers\) [\#908](https://github.com/devlooped/moq/pull/908) (@stakx)
- Add test from \#343 to regression test suite [\#907](https://github.com/devlooped/moq/pull/907) (@stakx)
- New algorithm for matching invoked methods against expected methods [\#904](https://github.com/devlooped/moq/pull/904) (@stakx)
- Fix `SetupAllProperties` indexer regression & refactor accessor check methods for clarity [\#902](https://github.com/devlooped/moq/pull/902) (@stakx)
- Fail when implicit conversion operator renders argument matcher unmatchable [\#900](https://github.com/devlooped/moq/pull/900) (@stakx)
- Uninvoke `InnerMockSetup` on `mock.Invocations.Clear()` [\#899](https://github.com/devlooped/moq/pull/899) (@stakx)
- Uninvoke `SequenceSetup` on `mock.Invocations.Clear()` [\#896](https://github.com/devlooped/moq/pull/896) (@stakx)
- Remove or replace redundant conditional compilation symbols [\#895](https://github.com/devlooped/moq/pull/895) (@stakx)
- Fix `.editorconfig` for auto-generated files [\#894](https://github.com/devlooped/moq/pull/894) (@stakx)
- Make mock event subscription equally strict as regular event subscription [\#891](https://github.com/devlooped/moq/pull/891) (@stakx)
- Added support for lambda expressions while creating a mock [\#888](https://github.com/devlooped/moq/pull/888) (@frblondin)
- Fix too-optimistic Task unwrapping logic in `Unwrap` [\#885](https://github.com/devlooped/moq/pull/885) (@stakx)
- Merge cleaned-up `ExpressionStringBuilder` into `StringBuilderExtensions` [\#882](https://github.com/devlooped/moq/pull/882) (@stakx)
- Fix `ExpressionStringBuilder` formatting of ternary conditional operator [\#881](https://github.com/devlooped/moq/pull/881) (@stakx)
- Fix `ExpressionStringBuilder` formatting of indexer assignment [\#880](https://github.com/devlooped/moq/pull/880) (@stakx)
- Format method parameter type list more accurately [\#879](https://github.com/devlooped/moq/pull/879) (@stakx)
- Fix `SetupAllProperties` for properties named `Item` [\#878](https://github.com/devlooped/moq/pull/878) (@stakx)
- Make post-`Returns` callback delegate validation more strict [\#876](https://github.com/devlooped/moq/pull/876) (@stakx)
- fix: Don't attempt to set CallBase on auto-generated mocks for delegates [\#875](https://github.com/devlooped/moq/pull/875) (@dammejed)
- Fix `SetupAllProperties` for properties whose name starts with `Item` [\#871](https://github.com/devlooped/moq/pull/871) (@stakx)
- Minor grammar correction in Mock.xdoc [\#863](https://github.com/devlooped/moq/pull/863) (@OskarNS)
- Improve Error message given if protected method is setup with bad arguments [\#862](https://github.com/devlooped/moq/pull/862) (@jessfdm-codes)
- Let `mock.Invocations.Clear()` remove traces of earlier invocations more thoroughly [\#854](https://github.com/devlooped/moq/pull/854) (@stakx)

## [v4.12.0](https://github.com/devlooped/moq/tree/v4.12.0) (2019-06-20)

[Full Changelog](https://github.com/devlooped/moq/compare/v4.11.0...v4.12.0)

:sparkles: Implemented enhancements:

- Setup sequence doesn't support ReturnsAsync with Func\<\> [\#795](https://github.com/devlooped/moq/issues/795)

:bug: Fixed bugs:

- VerifyNoOtherCalls causes stack overflow when mock setup returns the mocked object \(since 4.11\) [\#846](https://github.com/devlooped/moq/issues/846)
- Breaking Change Moq 4.11 - SetupAllProperties\(\) \(includes Mock.Of\<T\>\) fails in cases it didn't before [\#845](https://github.com/devlooped/moq/issues/845)
- `SetupAllProperties` no longer overrides pre-existing property setups [\#837](https://github.com/devlooped/moq/issues/837)
- SetupAllProperties does not setup write-only properties [\#835](https://github.com/devlooped/moq/issues/835)

:hammer: Other:

- Can not mock the method with several overloads. [\#839](https://github.com/devlooped/moq/issues/839)
- Moq counts multiple invocations although there is one when using capture [\#834](https://github.com/devlooped/moq/issues/834)
- Moq pre-release version 4.11.0-rc1 [\#815](https://github.com/devlooped/moq/issues/815)
- Clean up XML documentation comments [\#772](https://github.com/devlooped/moq/issues/772)
- Adding `Callback` to a mock breaks async tests [\#702](https://github.com/devlooped/moq/issues/702)

:twisted_rightwards_arrows: Merged:

- Make return values of loose mock setups having no `.Returns` nor `.CallBase` more consistent [\#849](https://github.com/devlooped/moq/pull/849) (@stakx)
- `VerifyNoOtherCalls`: Prevent stack overflow by remembering already verified mocks [\#848](https://github.com/devlooped/moq/pull/848) (@stakx)
- Prevent auto-stubbing failure due to inaccessible property accessors [\#847](https://github.com/devlooped/moq/pull/847) (@stakx)
- Proposal: Prevent capturing arguments on match failure [\#844](https://github.com/devlooped/moq/pull/844) (@ocoanet)
- Clean up XML documentation [\#843](https://github.com/devlooped/moq/pull/843) (@stakx)
- Add LINQ to Mocks support for strict mocks [\#842](https://github.com/devlooped/moq/pull/842) (@stakx)
- Add `sequenceSetup.ReturnsAsync(Func<T>)` [\#841](https://github.com/devlooped/moq/pull/841) (@stakx)
- Fix SetupAllProperties to override pre-existing property setups \(\#837\) [\#840](https://github.com/devlooped/moq/pull/840) (@ishimko)
- Unskip unit tests regarding indexers & matchers [\#838](https://github.com/devlooped/moq/pull/838) (@stakx)
- Handle write-only properties in SetupAllProperties for strict mocks \(fix \#835\) [\#836](https://github.com/devlooped/moq/pull/836) (@ishimko)
- Delete obsolete `.nuspec` file [\#832](https://github.com/devlooped/moq/pull/832) (@stakx)
- On-demand SetupAllProperties [\#826](https://github.com/devlooped/moq/pull/826) (@ishimko)

## [v4.11.0](https://github.com/devlooped/moq/tree/v4.11.0) (2019-05-27)

[Full Changelog](https://github.com/devlooped/moq/compare/v4.11.0-rc2...v4.11.0)

## [v4.11.0-rc2](https://github.com/devlooped/moq/tree/v4.11.0-rc2) (2019-05-27)

[Full Changelog](https://github.com/devlooped/moq/compare/v4.11.0-rc1...v4.11.0-rc2)

:bug: Fixed bugs:

- Regression in 4.11.0-rc1: Unhelpful exception message with SetupProperty [\#823](https://github.com/devlooped/moq/issues/823)

:hammer: Other:

- Question about order of Setup [\#831](https://github.com/devlooped/moq/issues/831)
- Mock.Object is slow for some types [\#830](https://github.com/devlooped/moq/issues/830)
- System.NotSupportedException : Cannot create boxed ByRef-like values. [\#829](https://github.com/devlooped/moq/issues/829)
- Repos\moq4\tests\Moq.Tests\MatchExpressionFixture.cs:row 21 [\#828](https://github.com/devlooped/moq/issues/828)
- Targeting Framework Missing / Empty Visual Studio 2017 [\#827](https://github.com/devlooped/moq/issues/827)
- Setup with interpolated strings [\#821](https://github.com/devlooped/moq/issues/821)
- Consider switching to portable PDB symbol files [\#447](https://github.com/devlooped/moq/issues/447)

:twisted_rightwards_arrows: Merged:

- Correct error message when attempting to set up an indexer with `SetupProperty` [\#824](https://github.com/devlooped/moq/pull/824) (@stakx)
- Adjust `default(Times)` to `Times.AtLeastOnce()` [\#820](https://github.com/devlooped/moq/pull/820) (@stakx)
- Optimize `SetupCollection` \(esp. `ToArrayLive`\) for better performance [\#819](https://github.com/devlooped/moq/pull/819) (@stakx)
- Simplify interception pipeline [\#818](https://github.com/devlooped/moq/pull/818) (@stakx)
- Use portable PDB in separate NuGet symbol package [\#789](https://github.com/devlooped/moq/pull/789) (@stakx)

## [v4.11.0-rc1](https://github.com/devlooped/moq/tree/v4.11.0-rc1) (2019-04-19)

[Full Changelog](https://github.com/devlooped/moq/compare/v4.10.1...v4.11.0-rc1)

:sparkles: Implemented enhancements:

- ToString\(\) on SetupPhrase does not return expression [\#810](https://github.com/devlooped/moq/issues/810)
- `mock.Protected().Verify` method group has no overloads with `bool exactParameterMatch` [\#752](https://github.com/devlooped/moq/issues/752)
- Incomplete stack trace when exceptions when raising events [\#738](https://github.com/devlooped/moq/issues/738)
- Moq.Protected.Setup\(\) Breaks On Abstract Class with Overloaded Abstract Methods [\#735](https://github.com/devlooped/moq/issues/735)
- Trouble building on OSX [\#726](https://github.com/devlooped/moq/issues/726)
- Mock\<T\>.Raise only raises events on root object [\#166](https://github.com/devlooped/moq/issues/166)

:bug: Fixed bugs:

- `Times.Equals` is implemented incorrectly [\#805](https://github.com/devlooped/moq/issues/805)
- Verify gets confused between the same generic and non-generic signature [\#749](https://github.com/devlooped/moq/issues/749)
- ArgumentNullException when verifying method call on EF Core mocked DbContext when the method has not been actually invoked but some DbSet setter has [\#741](https://github.com/devlooped/moq/issues/741)
- InvalidOperationException when specifiying setup on mock with mock containing property of type Nullable\<T\>  [\#725](https://github.com/devlooped/moq/issues/725)
- `VerifyAll` throws System.InvalidOperationException: 'Nullable object must have a value.' [\#711](https://github.com/devlooped/moq/issues/711)
- Setup gets included in `Verify` despite being "unreachable" [\#703](https://github.com/devlooped/moq/issues/703)
- `Verify` can create setups that causes a subsequent `VerifyAll` to fail [\#699](https://github.com/devlooped/moq/issues/699)
- VerifySet fails on non-trivial property setter [\#430](https://github.com/devlooped/moq/issues/430)
- Recursive property setup overrides previous setups [\#110](https://github.com/devlooped/moq/issues/110)

:hammer: Other:

- Castle.DynamicProxy.InvalidProxyConstructorArgumentsException when attempting to Setup or Verify based on a value returned from a mocked property [\#809](https://github.com/devlooped/moq/issues/809)
- `in` string parameter verifying doesn't work in some cases [\#808](https://github.com/devlooped/moq/issues/808)
- InSequence only works with MockBehavior.Strict and exception thrown is misleading [\#804](https://github.com/devlooped/moq/issues/804)
- Mocking classes doesn`t work [\#773](https://github.com/devlooped/moq/issues/773)
- Re-deprecate `[Matcher]` attribute? [\#770](https://github.com/devlooped/moq/issues/770)
- Modify object captured by invocation impact the `Verify` [\#756](https://github.com/devlooped/moq/issues/756)
- Verify calls are made in the correct order [\#748](https://github.com/devlooped/moq/issues/748)
- Add support for detecting unused setups [\#747](https://github.com/devlooped/moq/issues/747)
- Raise event when stubbed property is changed [\#745](https://github.com/devlooped/moq/issues/745)
- How can I mock [\#743](https://github.com/devlooped/moq/issues/743)
- Add .Net Standard 2.0 target [\#742](https://github.com/devlooped/moq/issues/742)
- Prevent proxy types from being generated for an interface [\#739](https://github.com/devlooped/moq/issues/739)
- Unable to resolve IFluentInterface [\#734](https://github.com/devlooped/moq/issues/734)
- CallBase cannot be used with Delegate mocks [\#731](https://github.com/devlooped/moq/issues/731)
- ArgumentOutOfRangeException when setup expression contains indexer access [\#714](https://github.com/devlooped/moq/issues/714)
- Adding a generic ThrowsAsync method [\#692](https://github.com/devlooped/moq/issues/692)
- Use latest version of 'System.Threading.Tasks.Extensions' [\#690](https://github.com/devlooped/moq/issues/690)
- Improve processing of multi-dot \(also called "fluent" or "recursive"\) setup expressions [\#643](https://github.com/devlooped/moq/issues/643)
- Should Moq stop targeting .NET Standard in favor of .NET Core? [\#630](https://github.com/devlooped/moq/issues/630)

:twisted_rightwards_arrows: Merged:

- Add test from issue to regression test suite [\#814](https://github.com/devlooped/moq/pull/814) (@stakx)
- Revert binary backward-incompatible changes [\#813](https://github.com/devlooped/moq/pull/813) (@stakx)
- Add ToString\(\) for fluent setup API objects [\#812](https://github.com/devlooped/moq/pull/812) (@jacob-ewald)
- It's a library [\#811](https://github.com/devlooped/moq/pull/811) (@stakx)
- Fix implementation of `Times.Equals` [\#806](https://github.com/devlooped/moq/pull/806) (@stakx)
- Consolidate public extension method classes [\#801](https://github.com/devlooped/moq/pull/801) (@stakx)
- Rectify `MockFactory` and `MockRepository` APIs [\#800](https://github.com/devlooped/moq/pull/800) (@stakx)
- Simplify delegate proxy generation using DynamicProxy [\#798](https://github.com/devlooped/moq/pull/798) (@stakx)
- Update Castle.Core to version 4.4.0 [\#797](https://github.com/devlooped/moq/pull/797) (@stakx)
- Match indexers used as arguments in setup expression eagerly [\#796](https://github.com/devlooped/moq/pull/796) (@stakx)
- Keep `InvocationShape` argument matchers & expressions in sync [\#793](https://github.com/devlooped/moq/pull/793) (@stakx)
- Make `ConstantMatcher` more \(but not fully\) `StackOverflowException`-proof [\#792](https://github.com/devlooped/moq/pull/792) (@stakx)
- Re-deprecate `MatcherAttribute` [\#788](https://github.com/devlooped/moq/pull/788) (@stakx)
- Remove Pex interop code [\#787](https://github.com/devlooped/moq/pull/787) (@stakx)
- Make `stringBuilder.AppendValueOf` safer [\#786](https://github.com/devlooped/moq/pull/786) (@stakx)
- Retire the .NET Standard 1.x target [\#785](https://github.com/devlooped/moq/pull/785) (@stakx)
- Add  `netstandard2.0` target [\#784](https://github.com/devlooped/moq/pull/784) (@stakx)
- Manually set up inner mocks should be discoverable by Moq [\#783](https://github.com/devlooped/moq/pull/783) (@stakx)
- `ActionObserver` improved support for parameterized base class ctors & additional virtual calls [\#782](https://github.com/devlooped/moq/pull/782) (@stakx)
- Add fixed test case to regression test suite [\#781](https://github.com/devlooped/moq/pull/781) (@stakx)
- Arguments in recursive expressions should not be treated like `It.IsAny` by default [\#780](https://github.com/devlooped/moq/pull/780) (@stakx)
-  Don't list setups in `Verify(...)` error message [\#779](https://github.com/devlooped/moq/pull/779) (@stakx)
- Fix regression with incomplete `Verify` expression validation [\#778](https://github.com/devlooped/moq/pull/778) (@stakx)
- `InvocationShape` should be equality-comparable [\#777](https://github.com/devlooped/moq/pull/777) (@stakx)
- Replace `LambdaExpressionPart` with `InvocationShape` [\#776](https://github.com/devlooped/moq/pull/776) (@stakx)
- Turn `AmbientObserver` into `MatcherObserver` [\#775](https://github.com/devlooped/moq/pull/775) (@stakx)
- Allow raising events on inner mocks [\#774](https://github.com/devlooped/moq/pull/774) (@stakx)
- Remove catch & \(re-\) throw in `Raise` [\#771](https://github.com/devlooped/moq/pull/771) (@stakx)
- Make `AmbientObserver` reentrant [\#769](https://github.com/devlooped/moq/pull/769) (@stakx)
- Make handling of `MatchExpression` more robust [\#768](https://github.com/devlooped/moq/pull/768) (@stakx)
- Reconstruct `SetupSet` and `VerifySet` expressions from delegates [\#767](https://github.com/devlooped/moq/pull/767) (@stakx)
- Fix `IProxy` interface visibility [\#766](https://github.com/devlooped/moq/pull/766) (@stakx)
- Remove expression execution from `Setup` and `Verify` & make them recursive [\#765](https://github.com/devlooped/moq/pull/765) (@stakx)
- Make `System.Object` members interceptable by \*any\* interceptor [\#764](https://github.com/devlooped/moq/pull/764) (@stakx)
- Thorough aggregation of verification errors & more complete error messages [\#762](https://github.com/devlooped/moq/pull/762) (@stakx)
- Document that "reachability" bug has been fixed [\#761](https://github.com/devlooped/moq/pull/761) (@stakx)
- Replace `Mock.InnerMocks` with `InnerMockSetup` [\#760](https://github.com/devlooped/moq/pull/760) (@stakx)
- Import Cleanup and Grouping for the test project [\#755](https://github.com/devlooped/moq/pull/755) (@Shereef)
- Create a `CONTRIBUTING.md` [\#754](https://github.com/devlooped/moq/pull/754) (@stakx)
- Add `mock.Protected().Verify` overloads with additional `exactParameterMatch` parameter [\#753](https://github.com/devlooped/moq/pull/753) (@Shereef)
- Add new `mock.Protected().Setup()` method overload [\#751](https://github.com/devlooped/moq/pull/751) (@stakx)
- Verify gets confused between the same generic and non-generic signature [\#750](https://github.com/devlooped/moq/pull/750) (@lepijohnny)
- De-deprecate `[Matcher]` attribute [\#732](https://github.com/devlooped/moq/pull/732) (@stakx)

## [v4.10.1](https://github.com/devlooped/moq/tree/v4.10.1) (2018-12-03)

[Full Changelog](https://github.com/devlooped/moq/compare/v4.10.0...v4.10.1)

:sparkles: Implemented enhancements:

- error messages don't display the parent class name of an inner class object [\#727](https://github.com/devlooped/moq/issues/727)
- ArgumentNullException on trying to mock indexer setter with more than single index argument [\#695](https://github.com/devlooped/moq/issues/695)

:bug: Fixed bugs:

- `CallBase` should not be allowed for delegate mocks [\#706](https://github.com/devlooped/moq/issues/706)

:hammer: Other:

- Rename License.txt to LICENSE.txt [\#720](https://github.com/devlooped/moq/issues/720)
- `net45` target causes major build issues in .NET Framework 4.7 projects [\#719](https://github.com/devlooped/moq/issues/719)
- "Verify" on method called with Parallel.ForEach has the wrong number of calls [\#715](https://github.com/devlooped/moq/issues/715)
- FileNotFoundException: Microsoft.AspNetCore.Razor.Runtime [\#710](https://github.com/devlooped/moq/issues/710)
- NuDoq API documentation is down [\#701](https://github.com/devlooped/moq/issues/701)
- Mocking class with no parameterless ctor [\#700](https://github.com/devlooped/moq/issues/700)
- Cannot set callback on function call that uses default parameter [\#689](https://github.com/devlooped/moq/issues/689)

:twisted_rightwards_arrows: Merged:

- Increase version to 4.10.1 [\#730](https://github.com/devlooped/moq/pull/730) (@stakx)
- Upgrade NuGet dependencies to current versions [\#729](https://github.com/devlooped/moq/pull/729) (@stakx)
- address \#727 [\#728](https://github.com/devlooped/moq/pull/728) (@powerdude)
- Merge `HandleTracking` interception aspect into `RecordInvocation` [\#723](https://github.com/devlooped/moq/pull/723) (@stakx)
- Enable `FluentMockContext` to record \>1 matcher per invocation [\#722](https://github.com/devlooped/moq/pull/722) (@stakx)
- Drop `System.ValueTuple` NuGet dependency [\#721](https://github.com/devlooped/moq/pull/721) (@stakx)
- Check `IsSpecialName` for properties & indexers [\#718](https://github.com/devlooped/moq/pull/718) (@stakx)
- Merge `MethodCallReturn` into `MethodCall` [\#717](https://github.com/devlooped/moq/pull/717) (@stakx)
- Make internal `Mock` methods non-generic [\#716](https://github.com/devlooped/moq/pull/716) (@stakx)
- Add tests for Moq compatibility with COM interop type events [\#713](https://github.com/devlooped/moq/pull/713) (@stakx)
- Add tests for Moq compatibility with F\# events [\#712](https://github.com/devlooped/moq/pull/712) (@stakx)
- `CallBase` should not be allowed for delegate mocks [\#708](https://github.com/devlooped/moq/pull/708) (@m-wild)
- Remove `Mock.DelegateInterfaceMethod` [\#705](https://github.com/devlooped/moq/pull/705) (@stakx)
- Merge duplicate `FluentMockVisitor` classes [\#698](https://github.com/devlooped/moq/pull/698) (@stakx)
- Support indexer with multiple arguments [\#694](https://github.com/devlooped/moq/pull/694) (@idigra)

## [v4.10.0](https://github.com/devlooped/moq/tree/v4.10.0) (2018-09-08)

[Full Changelog](https://github.com/devlooped/moq/compare/v4.9.0...v4.10.0)

:sparkles: Implemented enhancements:

- License update to project [\#669](https://github.com/devlooped/moq/issues/669)
- Verify\(\) with custom failure message [\#650](https://github.com/devlooped/moq/issues/650)
- CallBase\(\) is missing on ISetup\<T\> [\#615](https://github.com/devlooped/moq/issues/615)
- WherePasses and WhereThrows to Linq to Mocks [\#613](https://github.com/devlooped/moq/issues/613)
- Verifiable vs VerifyNoOtherCalls [\#607](https://github.com/devlooped/moq/issues/607)
- Update existing mock using Expression [\#293](https://github.com/devlooped/moq/issues/293)
- Implement verification of event handler attaches and detaches \(something like `mock.Verify(m => m.Event += callback)`\) [\#49](https://github.com/devlooped/moq/issues/49)
- MockSequence [\#21](https://github.com/devlooped/moq/issues/21)
- Add VerifyNoOtherCalls to MockFactory [\#682](https://github.com/devlooped/moq/pull/682) (@BlythMeister)
- Add extensibility point for LINQ expression tree compilation [\#647](https://github.com/devlooped/moq/pull/647) (@stakx)

:bug: Fixed bugs:

- Does `SetupAllProperties` actually create any setups, or does it not? [\#681](https://github.com/devlooped/moq/issues/681)
- Verification exception message produces incomplete call expression for delegate mocks  [\#678](https://github.com/devlooped/moq/issues/678)
- Verification exception message does include configured setups for delegate mocks [\#677](https://github.com/devlooped/moq/issues/677)
- `.Callback(…)` after `.Returns(…)` / `.CallBase()` causes a variety of issues [\#668](https://github.com/devlooped/moq/issues/668)
- Fluent API allows setting up a second callback that won't get involved if `.Throws()` is specified too [\#667](https://github.com/devlooped/moq/issues/667)
- Fluent API allows setting the return value twice [\#666](https://github.com/devlooped/moq/issues/666)
- Fluent API allows setting the return value and throwing an exception at the same time [\#665](https://github.com/devlooped/moq/issues/665)
- "Different number of parameters" error when passing to Returns a dynamically compiled Expression [\#652](https://github.com/devlooped/moq/issues/652)
- Recursive setup expression creates ghost setups that make `VerifyAll` fail [\#556](https://github.com/devlooped/moq/issues/556)
- Heisenberg's virtual property set in ctor with MockBehavior.Strict and VerifyAll [\#456](https://github.com/devlooped/moq/issues/456)
- Use of SetupSet 'forgets' Method Setup [\#432](https://github.com/devlooped/moq/issues/432)
- The use of `Action<...>` instead of `Expression<Action<...>>` in the public API causes a variety of problems [\#414](https://github.com/devlooped/moq/issues/414)

:hammer: Other:

- SetReturnsDefault not working for Task\<\> [\#673](https://github.com/devlooped/moq/issues/673)
- Verify does not distinguish between a type and its implemented type in generic methods [\#660](https://github.com/devlooped/moq/issues/660)
- Occational null ref in CreateProxy [\#653](https://github.com/devlooped/moq/issues/653)
- Callback may not be threadsafe \(ID10T error\) [\#651](https://github.com/devlooped/moq/issues/651)
- Close all stale and dormant issues as "unresolved" [\#642](https://github.com/devlooped/moq/issues/642)
- Have you considered writing Roslyn analyzers & quick fixes for Moq4? [\#522](https://github.com/devlooped/moq/issues/522)
- mock.Setup\(m =\> m.A.B.X\) should setup only what's minimally required. It shouldn't setup all properties, nor override preexisting setups in A or B. [\#426](https://github.com/devlooped/moq/issues/426)
- Setting multiple indexed objects' property directly via Linq fails [\#314](https://github.com/devlooped/moq/issues/314)
- SetupSet problems with It.IsAny\<\> and indexer properties [\#218](https://github.com/devlooped/moq/issues/218)
- Moq 64bit bad performance [\#188](https://github.com/devlooped/moq/issues/188)
- Bug: Mocks of factories with parameters behave as if you use "It.IsAny\(x\)" if you mock a function call in the returned object. [\#147](https://github.com/devlooped/moq/issues/147)
- Recursive mocks don't work with argument matching [\#142](https://github.com/devlooped/moq/issues/142)

:twisted_rightwards_arrows: Merged:

-  Make `MockFactory.VerifyMocks` do what it says it does [\#691](https://github.com/devlooped/moq/pull/691) (@stakx)
- Make multi-line comparisons in tests ignorant of CRLF/LF [\#688](https://github.com/devlooped/moq/pull/688) (@stakx)
- Reformat `License.txt` so GitHub recognizes it  [\#687](https://github.com/devlooped/moq/pull/687) (@stakx)
- Make `VerifyAll` ignore setups from `SetupAllProperties` [\#684](https://github.com/devlooped/moq/pull/684) (@stakx)
- `Verify` exception should include complete call expression for delegate mocks [\#680](https://github.com/devlooped/moq/pull/680) (@stakx)
- `Verify` exception should report configured setups for delegate mocks [\#679](https://github.com/devlooped/moq/pull/679) (@stakx)
- Replace parameter list comparison extension methods [\#676](https://github.com/devlooped/moq/pull/676) (@stakx)
- Add nDoc remark for SetReturnsDefault behaviour [\#674](https://github.com/devlooped/moq/pull/674) (@Albert221)
- Fix `VerifyAll` + `VerifyNoOtherCalls` for sequence setups [\#672](https://github.com/devlooped/moq/pull/672) (@stakx)
- Introduce new abstract base class `Setup` [\#671](https://github.com/devlooped/moq/pull/671) (@stakx)
- Add `setup.CallBase()` for void methods [\#664](https://github.com/devlooped/moq/pull/664) (@stakx)
- Make `MethodCall` classes non-generic [\#663](https://github.com/devlooped/moq/pull/663) (@stakx)
- Extract invocation matching logic into `InvocationShape` [\#662](https://github.com/devlooped/moq/pull/662) (@stakx)
- Move matcher selection logic into `MatcherFactory` [\#661](https://github.com/devlooped/moq/pull/661) (@stakx)
- Make `VerifyNoOtherCalls` take into account previous calls to parameterless `Verify()` and `VerifyAll()` [\#659](https://github.com/devlooped/moq/pull/659) (@stakx)
- Prevent false parameter count mismatch when using compiled methods with `Returns` [\#654](https://github.com/devlooped/moq/pull/654) (@stakx)
- Clean up project directory structure [\#649](https://github.com/devlooped/moq/pull/649) (@stakx)
- Simplify the build process & scripts [\#648](https://github.com/devlooped/moq/pull/648) (@stakx)
- The condition determining whether a parameter is 'out' parameter is changed [\#645](https://github.com/devlooped/moq/pull/645) (@koutinho)

## [v4.9.0](https://github.com/devlooped/moq/tree/v4.9.0) (2018-07-13)

[Full Changelog](https://github.com/devlooped/moq/compare/v4.8.3...v4.9.0)

:sparkles: Implemented enhancements:

- Deep verification of call arguments [\#560](https://github.com/devlooped/moq/issues/560)

:bug: Fixed bugs:

- Mock\<T\>.CallBase seems to be broken [\#640](https://github.com/devlooped/moq/issues/640)

:hammer: Other:

- Write exact values into exception messages [\#636](https://github.com/devlooped/moq/issues/636)
- Upgrade to Castle.Core 4.3.1 once it's available [\#632](https://github.com/devlooped/moq/issues/632)

:twisted_rightwards_arrows: Merged:

- Upgrade development dependencies to latest versions [\#639](https://github.com/devlooped/moq/pull/639) (@stakx)
- Add missing dependency to `System.Reflection.Emit` [\#638](https://github.com/devlooped/moq/pull/638) (@stakx)
- Clean up `MockException` \(and deprecate `IError` along the way\) [\#634](https://github.com/devlooped/moq/pull/634) (@stakx)
- `IInvocation` and `IInvocationList` [\#633](https://github.com/devlooped/moq/pull/633) (@stakx)
- Remove superfluous `.StripConversion` helper method [\#631](https://github.com/devlooped/moq/pull/631) (@stakx)
- Replace `.ToLambda()` conversions with static typing [\#629](https://github.com/devlooped/moq/pull/629) (@stakx)
- Invocations collection improvements [\#628](https://github.com/devlooped/moq/pull/628) (@Code-Grump)
- Don't ignore non-public interfaces unconditionally [\#641](https://github.com/devlooped/moq/pull/641) (@stakx)
- Round-trip floats in diagnostic messages [\#637](https://github.com/devlooped/moq/pull/637) (@stakx)
- Update Castle.Core to version 4.3.1 [\#635](https://github.com/devlooped/moq/pull/635) (@stakx)
- Expose invocations [\#627](https://github.com/devlooped/moq/pull/627) (@Code-Grump)

## [v4.8.3](https://github.com/devlooped/moq/tree/v4.8.3) (2018-06-09)

[Full Changelog](https://github.com/devlooped/moq/compare/v4.8.2...v4.8.3)

:sparkles: Implemented enhancements:

- C\# 7.2 `in ValueType` parameters are not supported with .NET Standard / Core [\#605](https://github.com/devlooped/moq/issues/605)
- Add missing sequence setup methods `ReturnsAsync` and `ThrowsAsync` for `ValueTask<TResult>` [\#599](https://github.com/devlooped/moq/issues/599)
- Recursive/Deep Mock setup [\#597](https://github.com/devlooped/moq/issues/597)

:bug: Fixed bugs:

- Returns\(null\) requires explicit typecast in Strict mocks [\#600](https://github.com/devlooped/moq/issues/600)
- ReturnsAsync and ThrowsAsync with delay parameter starts timer at setup [\#593](https://github.com/devlooped/moq/issues/593)
- Usage of ReturnsExtensions.ThrowsAsync\(\) can cause UnobservedTaskException [\#592](https://github.com/devlooped/moq/issues/592)

:hammer: Other:

- Upgrade to Castle Core 4.3.0 [\#623](https://github.com/devlooped/moq/issues/623)
- Invalid Callback error when Returns argument has return type object [\#622](https://github.com/devlooped/moq/issues/622)
- It.Is reference types [\#621](https://github.com/devlooped/moq/issues/621)
- Regression in .NET Core? [\#619](https://github.com/devlooped/moq/issues/619)
- Multiple calls to getter \(that returns an IEnumerable\) returns null 2nd time [\#618](https://github.com/devlooped/moq/issues/618)
- Mock.Of\(\) and MockRepository.VerifyAll\(\) [\#617](https://github.com/devlooped/moq/issues/617)
- Mock Object Won't Return an int \(i.e. Value Type\) [\#616](https://github.com/devlooped/moq/issues/616)
- Cannot mock protected Dispose\(It-IsAny-Bool\) on System.ComponentModel.Component [\#614](https://github.com/devlooped/moq/issues/614)
- Moq 4.2 documentation [\#612](https://github.com/devlooped/moq/issues/612)
- Build fails \(moq 4.8.2\) [\#611](https://github.com/devlooped/moq/issues/611)
-  System.TypeInitializationException [\#610](https://github.com/devlooped/moq/issues/610)
- Return default value for not implemented methods for As\<\> method [\#609](https://github.com/devlooped/moq/issues/609)
- WindsorContainer UsingFactoryMethod broken on upgrade to 4.8.0+ [\#608](https://github.com/devlooped/moq/issues/608)
- Intellisense puts red squiggly under wrong method [\#604](https://github.com/devlooped/moq/issues/604)
- Verify an overloaded method with params string\[\]  [\#603](https://github.com/devlooped/moq/issues/603)
- Q: Verify with It.IsAny for structs? [\#601](https://github.com/devlooped/moq/issues/601)
- Raise does not trigger event handler when CallBase is true [\#586](https://github.com/devlooped/moq/issues/586)
- `Mock.Of<T>()` is much slower than `new Mock<T>` \(up to several orders of magnitude\) [\#547](https://github.com/devlooped/moq/issues/547)
- Create a `CONTRIBUTING.md` [\#471](https://github.com/devlooped/moq/issues/471)
- Add benchmarks [\#388](https://github.com/devlooped/moq/issues/388)

:twisted_rightwards_arrows: Merged:

- Add missing async methods for sequential setups on ValueTask\<\> [\#626](https://github.com/devlooped/moq/pull/626) (@stakx)
- Verify that `in` parameter modifier is supported [\#625](https://github.com/devlooped/moq/pull/625) (@stakx)
- Update Castle.Core to version 4.3.0 [\#624](https://github.com/devlooped/moq/pull/624) (@stakx)
- 'Returns' regression with null function callback [\#602](https://github.com/devlooped/moq/pull/602) (@Caraul)
- Replace Mock.Of code with 10x faster equivalent [\#598](https://github.com/devlooped/moq/pull/598) (@informatorius)
-  Fixed UnobservedTaskException after using ThrowsAsync. Fixed ReturnsAsync and ThrowsAsync with delay [\#595](https://github.com/devlooped/moq/pull/595) (@snrnats)
- Added `ISetupSequentialResult<TResult>.Returns` method overload that support delegate for deferred results [\#594](https://github.com/devlooped/moq/pull/594) (@snrnats)

## [v4.8.2](https://github.com/devlooped/moq/tree/v4.8.2) (2018-02-23)

[Full Changelog](https://github.com/devlooped/moq/compare/v4.8.1...v4.8.2)

:bug: Fixed bugs:

- NotImplementedException DynamicProxy2 error when CallBase = true while mocking interfaces again [\#582](https://github.com/devlooped/moq/issues/582)
- Setup cannot invoke callback with different number of parameters in Moq 4.8.1 [\#572](https://github.com/devlooped/moq/issues/572)

:hammer: Other:

- Invalid setup on non-virtual member. [\#590](https://github.com/devlooped/moq/issues/590)
- Mock.Object throwing "Castle.DynamicProxy.InvalidProxyConstructorArgumentsException" [\#589](https://github.com/devlooped/moq/issues/589)
- Not able to moq setup 2 different methods with same input parameters  [\#587](https://github.com/devlooped/moq/issues/587)
- Reference parameter in virtual method doesn't update correctly when class is mocked [\#585](https://github.com/devlooped/moq/issues/585)
- Fix multiple Mock.Setup\(\) with Expression\<Func\<T\>\> invocation [\#584](https://github.com/devlooped/moq/issues/584)
- Need a way to setup a property / method by name [\#580](https://github.com/devlooped/moq/issues/580)
- System.ArgumentException : Invalid callback. Setup on method with 0 parameter\(s\) cannot invoke callback with different number of parameters \(1\) [\#579](https://github.com/devlooped/moq/issues/579)
- ValueTuple Package not necessary for .Net 4.7 [\#578](https://github.com/devlooped/moq/issues/578)
- Help required in mocking generic repository [\#576](https://github.com/devlooped/moq/issues/576)
- MissingMethodException / "Method not found" after update to Moq 4.8.0 [\#566](https://github.com/devlooped/moq/issues/566)
- Moq's documentation on NuDoq is stuck at an outdated package version [\#354](https://github.com/devlooped/moq/issues/354)

:twisted_rightwards_arrows: Merged:

- Upgrade dependency on System.ValueTuple to version 4.4.0 [\#591](https://github.com/devlooped/moq/pull/591) (@stakx)
- Ignore CallBase for members of additional interfaces [\#583](https://github.com/devlooped/moq/pull/583) (@stakx)
- Issue \#572 Setup cannot invoke callback with different number of parameters in Moq 4.8.1 [\#575](https://github.com/devlooped/moq/pull/575) (@Caraul)

## [v4.8.1](https://github.com/devlooped/moq/tree/v4.8.1) (2018-01-08)

[Full Changelog](https://github.com/devlooped/moq/compare/v4.8.0...v4.8.1)

:sparkles: Implemented enhancements:

- DefaultValue.Empty and DefaultValue.Mock should support C\# 7 tuple return types [\#562](https://github.com/devlooped/moq/issues/562)

:bug: Fixed bugs:

- Callbase not working on latest 4.8 version [\#557](https://github.com/devlooped/moq/issues/557)

:hammer: Other:

- Support ref returns \(c\# 7\) [\#568](https://github.com/devlooped/moq/issues/568)
- After v4.8.0 upgrade and tests failing with missing netstandard, version=2.0.0.0 [\#567](https://github.com/devlooped/moq/issues/567)

:twisted_rightwards_arrows: Merged:

- Downgrade System.ValueTuple & System.Threading.Tasks.Extensions [\#571](https://github.com/devlooped/moq/pull/571) (@stakx)
- Upgrade NuGet packages to latest available versions [\#565](https://github.com/devlooped/moq/pull/565) (@stakx)
- Replace some helper data types with tuples [\#564](https://github.com/devlooped/moq/pull/564) (@stakx)
- Add C\# 7 tuple support for `DefaultValue.Empty` and `DefaultValue.Mock` [\#563](https://github.com/devlooped/moq/pull/563) (@stakx)
- Fix `CallBase` regression with explicitly implemented interface methods [\#558](https://github.com/devlooped/moq/pull/558) (@stakx)

## [v4.8.0](https://github.com/devlooped/moq/tree/v4.8.0) (2017-12-24)

[Full Changelog](https://github.com/devlooped/moq/compare/v4.8.0-rc1...v4.8.0)

:sparkles: Implemented enhancements:

- Verify that no unverified methods were called \(alternative to Strict\) [\#527](https://github.com/devlooped/moq/issues/527)
- Allow the equivalent of It.IsAny with ref parameters in `Setup` [\#479](https://github.com/devlooped/moq/issues/479)
- SetupSequence is missing for void methods [\#451](https://github.com/devlooped/moq/issues/451)
- Detect method signature mismatch in `Setup` / `Returns` [\#445](https://github.com/devlooped/moq/issues/445)
- Can ValueTask IReturnsResult\<TMock\> extensions be added? [\#437](https://github.com/devlooped/moq/issues/437)
- Unable to mock protected generic method [\#249](https://github.com/devlooped/moq/issues/249)
- SetupSequence is missing for protected members [\#243](https://github.com/devlooped/moq/issues/243)
- Stub protected method with out/ref params [\#223](https://github.com/devlooped/moq/issues/223)
- Support mocking the return value when the type is Task/Task\<T\> [\#171](https://github.com/devlooped/moq/issues/171)
- How can I mock a function with ref parameter? [\#105](https://github.com/devlooped/moq/issues/105)
- Add support for custom default value generators \(i.e. make default value providers part of the public API\) [\#533](https://github.com/devlooped/moq/pull/533) (@stakx)
- Add support for `ValueTask<TResult>` to `DefaultValue.Empty` [\#529](https://github.com/devlooped/moq/pull/529) (@stakx)

:bug: Fixed bugs:

- Custom matcher properties not printed correctly in error messages [\#516](https://github.com/devlooped/moq/issues/516)
- `mock.Verify` and `mock.VerifyAll` leak internal `MockVerificationException` type [\#507](https://github.com/devlooped/moq/issues/507)
- `SetupAllProperties` tries to set up inaccessible properties [\#498](https://github.com/devlooped/moq/issues/498)
- `Setup` does not recognize all virtual sealed methods as uninterceptable [\#496](https://github.com/devlooped/moq/issues/496)
- Invocations of methods named `add_X` or `remove_X` are not recorded [\#487](https://github.com/devlooped/moq/issues/487)
- Moq should not use a mock's internal implementation of IEnumerable [\#478](https://github.com/devlooped/moq/issues/478)
- Call count doesn't get updated properly when method is configured to throw exception [\#472](https://github.com/devlooped/moq/issues/472)
- SetupSequence\(\) is not thread-safe [\#467](https://github.com/devlooped/moq/issues/467)
- Mock delegate with functional style causes infinite loop [\#224](https://github.com/devlooped/moq/issues/224)

:hammer: Other:

- Objects of anonymous types are not matched [\#552](https://github.com/devlooped/moq/issues/552)
- Build script no longer runs xUnit tests for .NET Core 2.0 target [\#545](https://github.com/devlooped/moq/issues/545)
- Add diagnostics for mock method resolutions [\#505](https://github.com/devlooped/moq/issues/505)
- Find out why Moq's performance has become worse since version approx. 4.5 [\#504](https://github.com/devlooped/moq/issues/504)
- Check why there's both `MethodCall.ToString` and `MethodCall.Format`, and whether we can drop one of them \(ideally including `MethodCall.SetFileInfo`\) [\#503](https://github.com/devlooped/moq/issues/503)
- Consider changing `[AssemblyVersion]` to major version only \(4.0.0.0\) to save users from having to configure assembly version redirects [\#424](https://github.com/devlooped/moq/issues/424)
- Loose mock should also return empty for IReadOnlyList\<T\> [\#173](https://github.com/devlooped/moq/issues/173)

:twisted_rightwards_arrows: Merged:

- Adopt assembly versioning scheme 'major.minor.0.0' [\#554](https://github.com/devlooped/moq/pull/554) (@stakx)
- Build script: Fix xUnit test run for .NET Core [\#553](https://github.com/devlooped/moq/pull/553) (@stakx)
- Make interface proxy creation twice as fast [\#551](https://github.com/devlooped/moq/pull/551) (@stakx)
- `SetupAllProperties`: Defer property initialization [\#550](https://github.com/devlooped/moq/pull/550) (@stakx)
- Reduce use of reflection in `SetupAllProperties` [\#549](https://github.com/devlooped/moq/pull/549) (@stakx)
- Optimize a few aspects of the interception process [\#546](https://github.com/devlooped/moq/pull/546) (@stakx)
- `VerifyNoOtherCalls`: fix bugs with multi-dot expressions [\#544](https://github.com/devlooped/moq/pull/544) (@stakx)
- `VerifyNoOtherCalls`: Fix bugs with `Mock.Of<T>` [\#543](https://github.com/devlooped/moq/pull/543) (@stakx)
- Restore default value provider correctly in `FluentMockContext` [\#542](https://github.com/devlooped/moq/pull/542) (@stakx)
- Refactor `ImplementedInterfaces` contraption [\#541](https://github.com/devlooped/moq/pull/541) (@stakx)
- Move state \(fields\) from `Mock` into `Mock<T>` [\#540](https://github.com/devlooped/moq/pull/540) (@stakx)
- Add `Mock<T>.VerifyNoOtherCalls()` method [\#539](https://github.com/devlooped/moq/pull/539) (@stakx)
- Improve consistency of type names related to method interception [\#538](https://github.com/devlooped/moq/pull/538) (@stakx)
- Match any values for `ref` parameters using `It.Ref<TValue>.IsAny` / `ItExpr.Ref<TValue>.IsAny` [\#537](https://github.com/devlooped/moq/pull/537) (@stakx)
- New `LookupOrFallbackDefaultValueProvider` base class for custom default value providers [\#536](https://github.com/devlooped/moq/pull/536) (@stakx)
- Add `Empty`, `Mock` properties to `DefaultValueProvider` [\#535](https://github.com/devlooped/moq/pull/535) (@stakx)
- Prevent infinite loop when invoking delegate in `Mock.Of` setup expression [\#528](https://github.com/devlooped/moq/pull/528) (@stakx)
- Update Times.xdoc [\#525](https://github.com/devlooped/moq/pull/525) (@jason-roberts)
- Rewrite `EmptyDefaultValueProvider` to use dictionary lookup for special type handling [\#524](https://github.com/devlooped/moq/pull/524) (@stakx)
- Simplify and optimize interception [\#523](https://github.com/devlooped/moq/pull/523) (@stakx)
- Validate delegates passed to `Returns` more strictly [\#520](https://github.com/devlooped/moq/pull/520) (@stakx)
- Ensure custom matcher properties print correctly in verification error messages [\#517](https://github.com/devlooped/moq/pull/517) (@stakx)
- Remove redundant code from matcher infrastructure [\#514](https://github.com/devlooped/moq/pull/514) (@stakx)
- Apply a few optimizations [\#513](https://github.com/devlooped/moq/pull/513) (@stakx)
- Minor optimizations in ExpressionStringBuilder [\#512](https://github.com/devlooped/moq/pull/512) (@stakx)
- Remove leaked `MockVerificationException` and catch-throw-recatch cycle during verification [\#511](https://github.com/devlooped/moq/pull/511) (@stakx)
- Prevent Moq from relying on a mock's implementation of IEnumerable\<T\> [\#510](https://github.com/devlooped/moq/pull/510) (@stakx)
- Remove `Interceptor.calls`, `ExpressionKey`, and `SetupKind` [\#509](https://github.com/devlooped/moq/pull/509) (@stakx)
- Get `PexProtector` out of the debugger's way [\#508](https://github.com/devlooped/moq/pull/508) (@stakx)
- Add ValueTask extensions [\#506](https://github.com/devlooped/moq/pull/506) (@AdamDotNet)
- Allow `DefaultValue.Mock` to mock `Task<TMockable>` and `ValueTask<TMockable>` [\#502](https://github.com/devlooped/moq/pull/502) (@stakx)
- `Protected().As<T>()`: Fix generic method support [\#501](https://github.com/devlooped/moq/pull/501) (@stakx)
- Let `SetupAllProperties` skip inaccessible methods [\#499](https://github.com/devlooped/moq/pull/499) (@stakx)
- Improve `Setup` recognition logic for sealed methods [\#497](https://github.com/devlooped/moq/pull/497) (@stakx)
- Introduce `.Protected().As<TDuck>()` to perform duck-typed setup and verification of protected members [\#495](https://github.com/devlooped/moq/pull/495) (@stakx)
- Various refactorings and optimizations [\#494](https://github.com/devlooped/moq/pull/494) (@stakx)
- Add `SetupSequence` method group for protected members [\#493](https://github.com/devlooped/moq/pull/493) (@stakx)
- Increase clarity and minor cleanup in readme [\#489](https://github.com/devlooped/moq/pull/489) (@AndrewLane)
-  Record calls to methods with event accessor-like names [\#488](https://github.com/devlooped/moq/pull/488) (@stakx)
- Clean up solution and build script regarding unit test projects [\#485](https://github.com/devlooped/moq/pull/485) (@stakx)
- Reimplement `SetupSequence` to make it thread-safe [\#476](https://github.com/devlooped/moq/pull/476) (@stakx)
- Added test to show regression described in \#469 [\#474](https://github.com/devlooped/moq/pull/474) (@ADThomsen)
- Update invocation count correctly, even when setup throws exception [\#473](https://github.com/devlooped/moq/pull/473) (@stakx)
- Add new `Callback` and `Returns` overloads for setting up methods with `ref` parameters [\#468](https://github.com/devlooped/moq/pull/468) (@stakx)
- Add support for void sequences \(2nd iteration\) [\#463](https://github.com/devlooped/moq/pull/463) (@alexbestul)
- Push to NuGet on tag, not on commit to master branch [\#462](https://github.com/devlooped/moq/pull/462) (@stakx)

## [v4.8.0-rc1](https://github.com/devlooped/moq/tree/v4.8.0-rc1) (2017-12-08)

[Full Changelog](https://github.com/devlooped/moq/compare/v4.7.145...v4.8.0-rc1)

:sparkles: Implemented enhancements:

- Setup/Verifyable With Times [\#530](https://github.com/devlooped/moq/issues/530)
- Implement raising of non-virtual events via `Mock<T>.Raise` [\#457](https://github.com/devlooped/moq/issues/457)
- OutOfMemoryException thrown when mock is called many times - Option for verification opt-out needed [\#227](https://github.com/devlooped/moq/issues/227)
- Any reason there is no Mock.Of\<\>\(\) overload with MockBehavior? [\#154](https://github.com/devlooped/moq/issues/154)
- Provide better error reporting in Verify\(\) [\#84](https://github.com/devlooped/moq/issues/84)

:hammer: Other:

- Proposal Verifiable return an IVerifier [\#534](https://github.com/devlooped/moq/issues/534)
- It.Is Overload taking a T not a Func [\#532](https://github.com/devlooped/moq/issues/532)
- Calling the same method with the same object does not differentiate the delta at call time [\#531](https://github.com/devlooped/moq/issues/531)
- Moq.SetupGet doesn't work with Mock.Of\<\> [\#526](https://github.com/devlooped/moq/issues/526)
- SetupSequence and MockBehavior.Strict fails with "All invocations on the mock must have a corresponding setup." [\#521](https://github.com/devlooped/moq/issues/521)
- Regression in .net 4.7.1 [\#500](https://github.com/devlooped/moq/issues/500)
- Moq4 compiled in debug mode? [\#483](https://github.com/devlooped/moq/issues/483)
- MissingManifestResourceException when using 4.6.25-alpha in UAP [\#274](https://github.com/devlooped/moq/issues/274)
- Async callbacks do not respect async/await [\#256](https://github.com/devlooped/moq/issues/256)

## [v4.7.145](https://github.com/devlooped/moq/tree/v4.7.145) (2017-11-06)

[Full Changelog](https://github.com/devlooped/moq/compare/v4.7.142...v4.7.145)

:sparkles: Implemented enhancements:

- Mock.HasSetup\(mock, x =\> x.Foo\) [\#391](https://github.com/devlooped/moq/issues/391)
- Mock.Of or Mock.From function to generate mock from anonymous object [\#152](https://github.com/devlooped/moq/issues/152)

:hammer: Other:

- Sporatic TypeLoadException or MissingMethodException during unit test execution [\#246](https://github.com/devlooped/moq/issues/246)

:twisted_rightwards_arrows: Merged:

- Turn collection of source file information per setup into an opt-in feature [\#515](https://github.com/devlooped/moq/pull/515) (@stakx)

## [v4.7.142](https://github.com/devlooped/moq/tree/v4.7.142) (2017-10-10)

[Full Changelog](https://github.com/devlooped/moq/compare/v4.7.137...v4.7.142)

:bug: Fixed bugs:

- 4.7.137 regression [\#469](https://github.com/devlooped/moq/issues/469)

:hammer: Other:

- Upgrade Castle Core package dependency to next released version [\#470](https://github.com/devlooped/moq/issues/470)
- NullReferenceException when calling Verify\(\) on a mock of DbContext with a mocked Set\<\> property [\#464](https://github.com/devlooped/moq/issues/464)
- I just upgraded to Castle Core v4.2.0 whilst using Moq v4.7.127 and I am getting an assembly binding error [\#461](https://github.com/devlooped/moq/issues/461)
- Crash: when accessing 2nd mocked class object \(see example\)  [\#449](https://github.com/devlooped/moq/issues/449)

:twisted_rightwards_arrows: Merged:

- Update Castle.Core to version 4.2.1 [\#482](https://github.com/devlooped/moq/pull/482) (@stakx)

## [v4.7.137](https://github.com/devlooped/moq/tree/v4.7.137) (2017-09-30)

[Full Changelog](https://github.com/devlooped/moq/compare/v4.7.127...v4.7.137)

:bug: Fixed bugs:

- As\<TInterface\> strange behavior [\#458](https://github.com/devlooped/moq/issues/458)

:hammer: Other:

- Upgrade to Castle.Core version 4.2.0 [\#425](https://github.com/devlooped/moq/issues/425)
- Worthwhile to add in Quickstart? [\#230](https://github.com/devlooped/moq/issues/230)

:twisted_rightwards_arrows: Merged:

- Let `.As<T>` mocks generate same proxy as the uncast mock [\#460](https://github.com/devlooped/moq/pull/460) (@stakx)
- Update Castle.Core to version 4.2.0 [\#459](https://github.com/devlooped/moq/pull/459) (@stakx)

## [v4.7.127](https://github.com/devlooped/moq/tree/v4.7.127) (2017-09-25)

[Full Changelog](https://github.com/devlooped/moq/compare/v4.7.99...v4.7.127)

:sparkles: Implemented enhancements:

- Support Mock.Get\(mock, exp\) syntax [\#229](https://github.com/devlooped/moq/issues/229)
- Mock name should be a part of method verification error message [\#140](https://github.com/devlooped/moq/issues/140)

:bug: Fixed bugs:

- Given `mock.SetupGet(o => o.Property).CallBase()`, strict mocks throw, demanding that the setup provide a return value [\#448](https://github.com/devlooped/moq/issues/448)
- SetupAllProperties 2nd property not mocked [\#438](https://github.com/devlooped/moq/issues/438)

:hammer: Other:

- Mocking concrete class that is declared as interface don't use mock any longer [\#452](https://github.com/devlooped/moq/issues/452)
- IMemoryCache mock error [\#446](https://github.com/devlooped/moq/issues/446)
- Issue mocking Entity Framework since 4.7.58 [\#441](https://github.com/devlooped/moq/issues/441)
- Returns not working on UdpClient.Receive\(\) method. [\#439](https://github.com/devlooped/moq/issues/439)
- Setup with ReturnsAsync, need to return passed in value but getting null [\#435](https://github.com/devlooped/moq/issues/435)
- Moq access method failed for internal constructors [\#434](https://github.com/devlooped/moq/issues/434)
- Regression in 4.7.46: overwritten Setup still runs match conditions. [\#433](https://github.com/devlooped/moq/issues/433)
- Cannot use SetupGet on protected virtual property in other project [\#431](https://github.com/devlooped/moq/issues/431)
- Issue with Moq.pdb in version 4.7.99: "Symbol indexes could not be retrieved" [\#428](https://github.com/devlooped/moq/issues/428)
- Can I Use it with dotnet core 1.1 [\#427](https://github.com/devlooped/moq/issues/427)
- Upgrade from 4.2.1409.1722 to 4.2.1507.0118 changed VerifyAll behavior [\#191](https://github.com/devlooped/moq/issues/191)
- Implicit CallBase on internal methods [\#178](https://github.com/devlooped/moq/issues/178)
- Allow hidden methods to be mocked [\#22](https://github.com/devlooped/moq/issues/22)

:twisted_rightwards_arrows: Merged:

- Make setups for inaccessible internal members fail fast by throwing an exception [\#455](https://github.com/devlooped/moq/pull/455) (@stakx)
- Make strict mocks recognize that `.CallBase()` can set up a return value, too [\#450](https://github.com/devlooped/moq/pull/450) (@stakx)
- Switch back from portable PDBs to classic PDBs [\#443](https://github.com/devlooped/moq/pull/443) (@stakx)
- Make `SetupAllProperties` work correctly for same-typed sibling properties [\#442](https://github.com/devlooped/moq/pull/442) (@stakx)

## [v4.7.99](https://github.com/devlooped/moq/tree/v4.7.99) (2017-07-17)

[Full Changelog](https://github.com/devlooped/moq/compare/v4.7.63...v4.7.99)

:sparkles: Implemented enhancements:

- Error message when attempting to mock an extension method [\#317](https://github.com/devlooped/moq/issues/317)

:bug: Fixed bugs:

- Abstract event handlers are not mocked with CallBase=true - System.NotImplementedException is raised [\#296](https://github.com/devlooped/moq/issues/296)
- Times.Never is incorrectly reported in a setup with Times.Exactly [\#211](https://github.com/devlooped/moq/issues/211)
- NullReferenceException when argument implements IEnumerable [\#169](https://github.com/devlooped/moq/issues/169)
- ProxyGenerationException using 3+ observer [\#167](https://github.com/devlooped/moq/issues/167)

:hammer: Other:

- I get an error after updating Castle.Core to 4.1.1 [\#418](https://github.com/devlooped/moq/issues/418)
- Internal interception on a mock changes the DefaultValue property [\#409](https://github.com/devlooped/moq/issues/409)
- Clean up & simplify the build process [\#408](https://github.com/devlooped/moq/issues/408)
- Upgrade Castle.Core from 4.1.0 to vNext [\#402](https://github.com/devlooped/moq/issues/402)
- Consistency regarding where exception messages are put: in code vs. in resources file [\#399](https://github.com/devlooped/moq/issues/399)
- Moq in UWP needs NeutralResourcesLanguageAttribute  [\#393](https://github.com/devlooped/moq/issues/393)
- Possible regression between 4.7.49 and 4.7.58? [\#383](https://github.com/devlooped/moq/issues/383)
- System.NotSupportedException:“Unsupported expression: n =\> n” [\#367](https://github.com/devlooped/moq/issues/367)
- Throwing an exception nulls any out parameters [\#342](https://github.com/devlooped/moq/issues/342)
- cannot mock a method to return null using LINQ syntax [\#337](https://github.com/devlooped/moq/issues/337)
- Thread safety issue on setup   [\#326](https://github.com/devlooped/moq/issues/326)
- Provide better changelog/release notes [\#320](https://github.com/devlooped/moq/issues/320)
- Setup of method with an IEquatable argument gets lost with newest moq version [\#319](https://github.com/devlooped/moq/issues/319)
- Invoking public method on mocked instance invalidates mock setup on protected method [\#312](https://github.com/devlooped/moq/issues/312)
- Nested asynchronous functions are not executed [\#300](https://github.com/devlooped/moq/issues/300)
- Infinite while for ConditionalContext with InnerMock Property get access [\#292](https://github.com/devlooped/moq/issues/292)
- Moq verify uses objects modified in Returns, rather than what was actually passed in [\#276](https://github.com/devlooped/moq/issues/276)
- Odd behavior in regex matcher with nulls [\#266](https://github.com/devlooped/moq/issues/266)
- InternalsVisibleTo [\#258](https://github.com/devlooped/moq/issues/258)
- TypeLoadException while mocking mixed code interface that contains a method with 'const char' parameter. [\#244](https://github.com/devlooped/moq/issues/244)
- Fix issue \#163 -  Use the "Protected" extensions to test non-protected members [\#240](https://github.com/devlooped/moq/issues/240)
- Mock\<T\>.Raise doesn't work although InternalsVisibleTo is set [\#231](https://github.com/devlooped/moq/issues/231)
- When mocking a class, why are non-virtual methods hidden in the generated proxy? [\#212](https://github.com/devlooped/moq/issues/212)
- \[NuGET\] Some packages are not compatible with UAP,Version=v10.0 \(win10-x64-aot\) [\#195](https://github.com/devlooped/moq/issues/195)
- Can I attach an attribute to mocked properties? [\#190](https://github.com/devlooped/moq/issues/190)
- Allow Strict behavior with CallBase = true [\#181](https://github.com/devlooped/moq/issues/181)
- Can we package the PDB in the NuGet Drop? [\#170](https://github.com/devlooped/moq/issues/170)
- Verify fails to recognize subclass as type argument for generic method when mock invoked in supporting method [\#151](https://github.com/devlooped/moq/issues/151)
- Is there a reason why It.Is only accepts an expression, thus breaking type inference? [\#150](https://github.com/devlooped/moq/issues/150)
- Mock.Of async expression [\#139](https://github.com/devlooped/moq/issues/139)
- The result of setting up methods in an hierarchy of interfaces depends on the order by which they're set up [\#131](https://github.com/devlooped/moq/issues/131)
- SetupSet on hierarchy [\#124](https://github.com/devlooped/moq/issues/124)
- NullReferenceException thrown by Microsoft.CSharp when using Generic Interfaces and Dynamic [\#114](https://github.com/devlooped/moq/issues/114)
- Provide a quickstart that actually compiles [\#112](https://github.com/devlooped/moq/issues/112)
- Issue with System.Reflection.Missing object passed as an argument to mocked Method  [\#107](https://github.com/devlooped/moq/issues/107)
- Support for Xamarin.iOS and Xamarin.Android [\#102](https://github.com/devlooped/moq/issues/102)
- Moq still doesn't support multithreading [\#91](https://github.com/devlooped/moq/issues/91)
- ref keyword in interface method declaration causes exception [\#42](https://github.com/devlooped/moq/issues/42)

:twisted_rightwards_arrows: Merged:

- 	Replace outdated release notes with new changelog [\#423](https://github.com/devlooped/moq/pull/423) (@stakx)
- Add regression test for issue \#421 [\#422](https://github.com/devlooped/moq/pull/422) (@kgybels)
- Fix misreported setup `Times` in `Verify` messages [\#420](https://github.com/devlooped/moq/pull/420) (@stakx)
- Clean up build process [\#417](https://github.com/devlooped/moq/pull/417) (@stakx)
- Update Castle.Core from version 4.1.0 to 4.1.1 [\#416](https://github.com/devlooped/moq/pull/416) (@stakx)
- Improve method match accuracy in `ExtractProxyCall` [\#415](https://github.com/devlooped/moq/pull/415) (@stakx)
- IEnumerable mocks shouldn't cause NullReferenceException [\#413](https://github.com/devlooped/moq/pull/413) (@stakx)
- Prevent stack overflow in conditional setups [\#412](https://github.com/devlooped/moq/pull/412) (@stakx)
- Fix for the \#409 [\#411](https://github.com/devlooped/moq/pull/411) (@vladonemo)
- Unskip 3 unit tests [\#404](https://github.com/devlooped/moq/pull/404) (@stakx)
- Move hardcoded message strings to `Resources.resx` [\#403](https://github.com/devlooped/moq/pull/403) (@stakx)
- Improve Setup / Verify exception messages for static members and extension methods [\#400](https://github.com/devlooped/moq/pull/400) (@stakx)
- Allow `Mock<T>.Raise` to raise events on child mocks [\#397](https://github.com/devlooped/moq/pull/397) (@stakx)
- Allow setting up null return values using `Mock.Of` [\#396](https://github.com/devlooped/moq/pull/396) (@stakx)
- Make abstract events defined in classes work even when `CallBase` is true [\#395](https://github.com/devlooped/moq/pull/395) (@stakx)
- Add NeutralResourcesLanguage to Assembly info for portable library use [\#394](https://github.com/devlooped/moq/pull/394) (@benbillbob)
- Make `Interceptor` more thread-safe [\#392](https://github.com/devlooped/moq/pull/392) (@stakx)
- Fix typo. [\#389](https://github.com/devlooped/moq/pull/389) (@JohanLarsson)

## [v4.7.63](https://github.com/devlooped/moq/tree/v4.7.63) (2017-06-21)

[Full Changelog](https://github.com/devlooped/moq/compare/v4.7.58...v4.7.63)

:sparkles: Implemented enhancements:

- Null array elements are not printed to test output [\#321](https://github.com/devlooped/moq/issues/321)

:bug: Fixed bugs:

- Moq functionality broke after update to 4.7.1 [\#331](https://github.com/devlooped/moq/issues/331)
- Possibly undocumented breaking change in 4.2.1408.0511 [\#315](https://github.com/devlooped/moq/issues/315)
- Interface inheritance chains with read-only and read-write properties return defaults instead of mocked value. [\#275](https://github.com/devlooped/moq/issues/275)
- Casting Mock To Interface Loses Value [\#175](https://github.com/devlooped/moq/issues/175)
- Changes between versions 4.2.1402.2112 and 4.2.1409.1722 broken setup property [\#164](https://github.com/devlooped/moq/issues/164)
- Mock\<Class : IInterface\>: Inconsistency between mocked-behavior and verification [\#157](https://github.com/devlooped/moq/issues/157)
- Invocation on a non-virtual method when object casted to interface is wrong [\#156](https://github.com/devlooped/moq/issues/156)
- Mock's members incorrectly marked as non-final [\#141](https://github.com/devlooped/moq/issues/141)
- Changed behavior with explicit interface implementations [\#133](https://github.com/devlooped/moq/issues/133)

:hammer: Other:

- AddRangeAsync xunit .net core error [\#327](https://github.com/devlooped/moq/issues/327)
- bug\(assembly-loading\): creating a moq of an interface in a third-party assembly can sometimes throw a FileNotFoundException [\#299](https://github.com/devlooped/moq/issues/299)
- Setups using struct with nullable member overwrite previous setups [\#263](https://github.com/devlooped/moq/issues/263)
- Quickstart should include SetupSequence [\#165](https://github.com/devlooped/moq/issues/165)
- Latest version breaks interface mocks when property overridden in derived interface [\#162](https://github.com/devlooped/moq/issues/162)
- SetupAllProperties with DefaultValue.Mock throws on concrete property type without parameterless constructor [\#149](https://github.com/devlooped/moq/issues/149)
- Moq 1409 changes the behaviour of internals in a mocked class [\#132](https://github.com/devlooped/moq/issues/132)
- Raise doesnt work when the event handler signature has one parameter  [\#127](https://github.com/devlooped/moq/issues/127)

:twisted_rightwards_arrows: Merged:

- Fix mocking of non-virtual methods via interface [\#387](https://github.com/devlooped/moq/pull/387) (@stakx)
- Ensure that `null` never matches `It.IsRegex` [\#385](https://github.com/devlooped/moq/pull/385) (@stakx)

## [v4.7.58](https://github.com/devlooped/moq/tree/v4.7.58) (2017-06-20)

[Full Changelog](https://github.com/devlooped/moq/compare/v4.7.49...v4.7.58)

:sparkles: Implemented enhancements:

- Trying to mock a multidimensional array fails [\#311](https://github.com/devlooped/moq/issues/311)
- Missing .ThrowsAsync\(\) extension for Task-returning methods [\#294](https://github.com/devlooped/moq/issues/294)
- Parameter Array modifier lost in mock [\#235](https://github.com/devlooped/moq/issues/235)

:bug: Fixed bugs:

- Mock.Of\<T\> fails with exception 'Unhandled Exception: System.ArgumentException: Interface not found' [\#340](https://github.com/devlooped/moq/issues/340)
- Issue with setting up mock Objects, i.e. calling Mock\<T\>.Setup, when arguments have the same hashcodes [\#328](https://github.com/devlooped/moq/issues/328)

:hammer: Other:

- Exception after upgrade of Catle.Core to 4.1.0 [\#377](https://github.com/devlooped/moq/issues/377)
- Upgrade to Castle.Core 4.1.0 [\#375](https://github.com/devlooped/moq/issues/375)
- Extracting Func\<T, bool\> when using It.Is\<T\> can result in missing method setups [\#374](https://github.com/devlooped/moq/issues/374)
- Interface inheritance introduced a possible breaking change in 4.2.1408.619 [\#371](https://github.com/devlooped/moq/issues/371)
- SetUpAllProperties does not provide stubbing behavior for virtual properties. [\#283](https://github.com/devlooped/moq/issues/283)
- IsAssignableFrom breaks \_\_ComObject support [\#269](https://github.com/devlooped/moq/issues/269)
- Compared with null after type conversion using 'as' keyword  [\#237](https://github.com/devlooped/moq/issues/237)
- Setups with Nullable types on strict mocks no longer match null [\#184](https://github.com/devlooped/moq/issues/184)
- Wrong equality check in Interceptor.ExpressionKey [\#135](https://github.com/devlooped/moq/issues/135)
- System.InvalidCastException when invoking method that starts with "add" [\#82](https://github.com/devlooped/moq/issues/82)

:twisted_rightwards_arrows: Merged:

- Fix mocking of redeclared interface methods [\#382](https://github.com/devlooped/moq/pull/382) (@stakx)
- Fix "class method vs. interface method" bug introduced by 162a543 [\#381](https://github.com/devlooped/moq/pull/381) (@stakx)
- Fix formatting inconsistencies for array values in MockException.Message [\#380](https://github.com/devlooped/moq/pull/380) (@stakx)

## [v4.7.49](https://github.com/devlooped/moq/tree/v4.7.49) (2017-06-18)

[Full Changelog](https://github.com/devlooped/moq/compare/v4.7.46...v4.7.49)

:twisted_rightwards_arrows: Merged:

- Fix `Moq.nuspec` and package references [\#379](https://github.com/devlooped/moq/pull/379) (@stakx)

## [v4.7.46](https://github.com/devlooped/moq/tree/v4.7.46) (2017-06-18)

[Full Changelog](https://github.com/devlooped/moq/compare/v4.7.25...v4.7.46)

:hammer: Other:

- Clean up the mess of mixed tabs & spaces in code files [\#364](https://github.com/devlooped/moq/issues/364)
- Copyright notices still point to outdated Google Code project site [\#356](https://github.com/devlooped/moq/issues/356)
- new Mock\<T\> can generate not unique names [\#348](https://github.com/devlooped/moq/issues/348)
- Does moq have dependency on moq.resources? [\#344](https://github.com/devlooped/moq/issues/344)
- Protected\<T\>.Setup fails on specific overloads [\#341](https://github.com/devlooped/moq/issues/341)
- Moq cannot mock a method that contains a default value of null w/out explicitly setting that value in the mock's expression tree [\#316](https://github.com/devlooped/moq/issues/316)
- Nuget Package Moq 4.5.28 could not be installed on MonoAndroid=v7.0 [\#308](https://github.com/devlooped/moq/issues/308)
- A verify method that returns a boolean. [\#287](https://github.com/devlooped/moq/issues/287)
- Invalid type for a default value exception \(regression\) [\#286](https://github.com/devlooped/moq/issues/286)
- Verify passes on strict mock for parameter without proper setup. [\#285](https://github.com/devlooped/moq/issues/285)
- Missing release tags [\#247](https://github.com/devlooped/moq/issues/247)
- Mock any instance [\#214](https://github.com/devlooped/moq/issues/214)
- Misleading exception when type parameter of mocked class is internal [\#192](https://github.com/devlooped/moq/issues/192)
- MOQ Test for Hardcoded values [\#172](https://github.com/devlooped/moq/issues/172)
- Stack overflow when setting up mock with default value set to mock. [\#163](https://github.com/devlooped/moq/issues/163)
- Stack overflow when setting up a property on a mock that references another instance of the same type [\#161](https://github.com/devlooped/moq/issues/161)
- Quickstart has broken link to Microsoft Connect [\#159](https://github.com/devlooped/moq/issues/159)
- Update wiki from MockFactory to MockRepository [\#153](https://github.com/devlooped/moq/issues/153)
- Documentation [\#111](https://github.com/devlooped/moq/issues/111)
- Mocks.Of in the quickstart [\#93](https://github.com/devlooped/moq/issues/93)
- Can't setup protected methods with nullable parameters [\#92](https://github.com/devlooped/moq/issues/92)
- Counting number of call method before callback raise [\#65](https://github.com/devlooped/moq/issues/65)
- Allow setting up async methods easily [\#64](https://github.com/devlooped/moq/issues/64)
- 4.1 does not support multi-threading [\#62](https://github.com/devlooped/moq/issues/62)
- Add documentation to NuGet package and/or have updated easy to find online documentation [\#57](https://github.com/devlooped/moq/issues/57)
- Deadlock in Interceptor [\#47](https://github.com/devlooped/moq/issues/47)
- www.moqthis.com produces 404 [\#41](https://github.com/devlooped/moq/issues/41)
- "Error binding to target method" [\#26](https://github.com/devlooped/moq/issues/26)

:twisted_rightwards_arrows: Merged:

- Fix compilation warnings, clean up conditional compilation [\#378](https://github.com/devlooped/moq/pull/378) (@stakx)
- Make event accessor recognition logic more correct [\#376](https://github.com/devlooped/moq/pull/376) (@stakx)
- Ensure incorrect implementations of ISerializable are caught properly [\#370](https://github.com/devlooped/moq/pull/370) (@stakx)
- Update Castle Core from version 4.0.0 to 4.1.0 [\#369](https://github.com/devlooped/moq/pull/369) (@stakx)
- Renormalize indentation whitespace to tabs [\#368](https://github.com/devlooped/moq/pull/368) (@stakx)
- Fix equality check bug in ExpressionKey [\#363](https://github.com/devlooped/moq/pull/363) (@stakx)
- Replace old Google Code URL with GitHub URL [\#362](https://github.com/devlooped/moq/pull/362) (@stakx)
- Make `It.IsAny`, `It.IsNotNull` work for COM types [\#361](https://github.com/devlooped/moq/pull/361) (@stakx)
- Default values for multidimensional arrays [\#360](https://github.com/devlooped/moq/pull/360) (@stakx)
- Ensure default mock names are \(more\) unique [\#359](https://github.com/devlooped/moq/pull/359) (@stakx)
- Simplify logic in `ExpressionMatcher.Matches` [\#358](https://github.com/devlooped/moq/pull/358) (@stakx)
- Add `ThrowsAsync` overload for non-generic `Task` [\#357](https://github.com/devlooped/moq/pull/357) (@stakx)

## [v4.7.25](https://github.com/devlooped/moq/tree/v4.7.25) (2017-06-02)

[Full Changelog](https://github.com/devlooped/moq/compare/v4.7.12...v4.7.25)

:hammer: Other:

- Documentation Correction around using Verify\(\) [\#332](https://github.com/devlooped/moq/issues/332)
- Moq Docs [\#290](https://github.com/devlooped/moq/issues/290)
- Link is Broken to Doc [\#108](https://github.com/devlooped/moq/issues/108)

:twisted_rightwards_arrows: Merged:

- Fix failing AppVeyor build [\#352](https://github.com/devlooped/moq/pull/352) (@stakx)
- Migrate .NET Core projects to vs2017 [\#336](https://github.com/devlooped/moq/pull/336) (@jeremymeng)
- Added option to delay async returns and async throws [\#289](https://github.com/devlooped/moq/pull/289) (@jochenz)

## [v4.7.12](https://github.com/devlooped/moq/tree/v4.7.12) (2017-05-30)

[Full Changelog](https://github.com/devlooped/moq/compare/v4.7.11...v4.7.12)

:hammer: Other:

- Small grammar correction in documentation [\#260](https://github.com/devlooped/moq/issues/260)

:twisted_rightwards_arrows: Merged:

- Added overload to enforce old behavior of exact parameter matching [\#347](https://github.com/devlooped/moq/pull/347) (@80er)

## [v4.7.11](https://github.com/devlooped/moq/tree/v4.7.11) (2017-05-30)

[Full Changelog](https://github.com/devlooped/moq/compare/v4.7.10...v4.7.11)

:twisted_rightwards_arrows: Merged:

- Docs: Fix 4 typos \(fixes \#260\) [\#349](https://github.com/devlooped/moq/pull/349) (@stakx)

## [v4.7.10](https://github.com/devlooped/moq/tree/v4.7.10) (2017-05-06)

[Full Changelog](https://github.com/devlooped/moq/compare/v4.7.9...v4.7.10)

:hammer: Other:

- Passing a mock into another mock does not work for me [\#174](https://github.com/devlooped/moq/issues/174)

## [v4.7.9](https://github.com/devlooped/moq/tree/v4.7.9) (2017-04-29)

[Full Changelog](https://github.com/devlooped/moq/compare/v4.7.8...v4.7.9)

:hammer: Other:

- The dependency Moq \>= 4.7.8 could not be resolved [\#339](https://github.com/devlooped/moq/issues/339)
- Symbols for Moq 4.7.8 [\#338](https://github.com/devlooped/moq/issues/338)

## [v4.7.8](https://github.com/devlooped/moq/tree/v4.7.8) (2017-03-26)

[Full Changelog](https://github.com/devlooped/moq/compare/v4.7.7...v4.7.8)

:hammer: Other:

- Protected Verify doesn't accept mocks of interfaces as valid for those interfaces [\#334](https://github.com/devlooped/moq/issues/334)

## [v4.7.7](https://github.com/devlooped/moq/tree/v4.7.7) (2017-03-25)

[Full Changelog](https://github.com/devlooped/moq/compare/v4.7.1...v4.7.7)

:twisted_rightwards_arrows: Merged:

- Fix bug in HasMatchingParameterTypes [\#335](https://github.com/devlooped/moq/pull/335) (@jeremymeng)
- Fixing documentation typo on Verifiable\(\) usage. [\#333](https://github.com/devlooped/moq/pull/333) (@jcockhren)
- Fix for issue \#92: Can't setup protected methods with nullable parameters [\#200](https://github.com/devlooped/moq/pull/200) (@RobSiklos)

## [v4.7.1](https://github.com/devlooped/moq/tree/v4.7.1) (2017-02-28)

[Full Changelog](https://github.com/devlooped/moq/compare/v4.7.0...v4.7.1)

:twisted_rightwards_arrows: Merged:

- Fix typo in summary of Mock\<T\>.Setup docs [\#325](https://github.com/devlooped/moq/pull/325) (@xavierdecoster)

## [v4.7.0](https://github.com/devlooped/moq/tree/v4.7.0) (2017-02-22)

[Full Changelog](https://github.com/devlooped/moq/compare/v4.6.62-alpha...v4.7.0)

:hammer: Other:

- Identical setups are now not considered the same \(regression\) [\#295](https://github.com/devlooped/moq/issues/295)
- moq breaks ASP.NET Core xUnit.net project for Core 1.0 Web API application [\#284](https://github.com/devlooped/moq/issues/284)
- It.IsAny\<decimal\> throws "System.decimal is not a supported constant type" [\#265](https://github.com/devlooped/moq/issues/265)
- Make Moq portable to .NET Core [\#168](https://github.com/devlooped/moq/issues/168)
- Upgrade to Castle.Core 3.2.1 because of Mono [\#113](https://github.com/devlooped/moq/issues/113)

## [v4.6.62-alpha](https://github.com/devlooped/moq/tree/v4.6.62-alpha) (2017-02-21)

[Full Changelog](https://github.com/devlooped/moq/compare/v4.6.39-alpha...v4.6.62-alpha)

:twisted_rightwards_arrows: Merged:

- Fix build errors after merging master to netcore [\#324](https://github.com/devlooped/moq/pull/324) (@jeremymeng)

## [v4.6.39-alpha](https://github.com/devlooped/moq/tree/v4.6.39-alpha) (2017-02-18)

[Full Changelog](https://github.com/devlooped/moq/compare/v4.5.30...v4.6.39-alpha)

:twisted_rightwards_arrows: Merged:

- Upgrade Castle.Core dependency to 4.0.0 [\#323](https://github.com/devlooped/moq/pull/323) (@jeremymeng)

## [v4.5.30](https://github.com/devlooped/moq/tree/v4.5.30) (2017-01-09)

[Full Changelog](https://github.com/devlooped/moq/compare/v4.5.29...v4.5.30)

:twisted_rightwards_arrows: Merged:

- Fixing \#295 and changing fix for \#278 [\#313](https://github.com/devlooped/moq/pull/313) (@MatKubicki)

## [v4.5.29](https://github.com/devlooped/moq/tree/v4.5.29) (2016-12-10)

[Full Changelog](https://github.com/devlooped/moq/compare/v4.5.28...v4.5.29)

:hammer: Other:

- ReturnsAsync\(\) with factory method does not behave as expected [\#303](https://github.com/devlooped/moq/issues/303)
- Is it possible to mock local variable in a method [\#302](https://github.com/devlooped/moq/issues/302)
- Mocking "FindBy" with Generic Repository C\# .Net [\#301](https://github.com/devlooped/moq/issues/301)
- Using `It.IsAny<object>()` with a lambda from `dynamic` fails when binding properties [\#298](https://github.com/devlooped/moq/issues/298)

:twisted_rightwards_arrows: Merged:

- ReturnsAsync\(\) lazy evaluation issue fix \#303 [\#309](https://github.com/devlooped/moq/pull/309) (@SaroTasciyan)

## [v4.5.28](https://github.com/devlooped/moq/tree/v4.5.28) (2016-11-10)

[Full Changelog](https://github.com/devlooped/moq/compare/v4.5.23...v4.5.28)

:twisted_rightwards_arrows: Merged:

- adds ReturnAsync extention method that accepts a Func\<TResult\> [\#297](https://github.com/devlooped/moq/pull/297) (@joeenzminger)

## [v4.5.23](https://github.com/devlooped/moq/tree/v4.5.23) (2016-10-11)

[Full Changelog](https://github.com/devlooped/moq/compare/v4.5.22...v4.5.23)

:hammer: Other:

- Setups for properties with multiple arguments override each other even with differing values [\#278](https://github.com/devlooped/moq/issues/278)

:twisted_rightwards_arrows: Merged:

- Fixes an issue in comparing arguments of calls [\#291](https://github.com/devlooped/moq/pull/291) (@MatKubicki)

## [v4.5.22](https://github.com/devlooped/moq/tree/v4.5.22) (2016-09-20)

[Full Changelog](https://github.com/devlooped/moq/compare/v4.6.38-alpha...v4.5.22)

:hammer: Other:

- Add a .SetupThrowsDefault\<T\>\(\) to configure a mock to throw w/o specifying a method [\#281](https://github.com/devlooped/moq/issues/281)
- Events not working with CallBase = true depending on order interfaces are inherited.  C : IA, IB behaves different than C : IB, IA [\#228](https://github.com/devlooped/moq/issues/228)

:twisted_rightwards_arrows: Merged:

- Fix for issue \#228: Events not raised on mocked type with multiple interfaces [\#288](https://github.com/devlooped/moq/pull/288) (@bradreimer)

## [v4.6.38-alpha](https://github.com/devlooped/moq/tree/v4.6.38-alpha) (2016-08-20)

[Full Changelog](https://github.com/devlooped/moq/compare/v4.5.21...v4.6.38-alpha)

:twisted_rightwards_arrows: Merged:

- Fix Castle.Core dependency version [\#282](https://github.com/devlooped/moq/pull/282) (@jeremymeng)

## [v4.5.21](https://github.com/devlooped/moq/tree/v4.5.21) (2016-08-12)

[Full Changelog](https://github.com/devlooped/moq/compare/v4.5.20...v4.5.21)

:hammer: Other:

- Unexpected behaviour with MockExtensions.Reset [\#220](https://github.com/devlooped/moq/issues/220)

:twisted_rightwards_arrows: Merged:

- Moq Reset should clear all existing calls in the interceptor [\#277](https://github.com/devlooped/moq/pull/277) (@anilkamath87)

## [v4.5.20](https://github.com/devlooped/moq/tree/v4.5.20) (2016-08-12)

[Full Changelog](https://github.com/devlooped/moq/compare/v4.5.19...v4.5.20)

:twisted_rightwards_arrows: Merged:

- Fixed `InterceptObjectMethodsMixin` to properly check for `System.Object` methods [\#280](https://github.com/devlooped/moq/pull/280) (@kolomanschaft)

## [v4.5.19](https://github.com/devlooped/moq/tree/v4.5.19) (2016-08-10)

[Full Changelog](https://github.com/devlooped/moq/compare/v4.5.18...v4.5.19)

:twisted_rightwards_arrows: Merged:

- Fixing issues \#161 and \#163 [\#245](https://github.com/devlooped/moq/pull/245) (@vladonemo)

## [v4.5.18](https://github.com/devlooped/moq/tree/v4.5.18) (2016-08-10)

[Full Changelog](https://github.com/devlooped/moq/compare/v4.6.36-alpha...v4.5.18)

:hammer: Other:

- Somethings up with strict mock with recent changes [\#273](https://github.com/devlooped/moq/issues/273)
- Allow setting up "overrides" for `object` members when mocking interfaces [\#248](https://github.com/devlooped/moq/issues/248)

:twisted_rightwards_arrows: Merged:

- `System.Object` methods should always work. [\#279](https://github.com/devlooped/moq/pull/279) (@kolomanschaft)

## [v4.6.36-alpha](https://github.com/devlooped/moq/tree/v4.6.36-alpha) (2016-07-20)

[Full Changelog](https://github.com/devlooped/moq/compare/v4.5.16...v4.6.36-alpha)

:twisted_rightwards_arrows: Merged:

- Upgrade Castle.Core dependency to v4.0.0-beta1. [\#272](https://github.com/devlooped/moq/pull/272) (@jeremymeng)

## [v4.5.16](https://github.com/devlooped/moq/tree/v4.5.16) (2016-07-18)

[Full Changelog](https://github.com/devlooped/moq/compare/v4.5.13...v4.5.16)

:hammer: Other:

- Verifiable throw NullReferenceException for async Task methods [\#101](https://github.com/devlooped/moq/issues/101)

:twisted_rightwards_arrows: Merged:

- Allow for class mocks and interface mocks to mock System.Object methods [\#250](https://github.com/devlooped/moq/pull/250) (@kolomanschaft)

## [v4.5.13](https://github.com/devlooped/moq/tree/v4.5.13) (2016-07-11)

[Full Changelog](https://github.com/devlooped/moq/compare/v4.5.10...v4.5.13)

:hammer: Other:

- Moq not verifying Expressions as expected [\#271](https://github.com/devlooped/moq/issues/271)

:twisted_rightwards_arrows: Merged:

- Exceptions using Verify for specific calls should include actual values [\#264](https://github.com/devlooped/moq/pull/264) (@hahn-kev)

## [v4.5.10](https://github.com/devlooped/moq/tree/v4.5.10) (2016-06-21)

[Full Changelog](https://github.com/devlooped/moq/compare/v4.5.9...v4.5.10)

:twisted_rightwards_arrows: Merged:

- Add an helper class to simplify parameter capture [\#251](https://github.com/devlooped/moq/pull/251) (@ocoanet)

## [v4.5.9](https://github.com/devlooped/moq/tree/v4.5.9) (2016-06-09)

[Full Changelog](https://github.com/devlooped/moq/compare/v4.6.25-alpha...v4.5.9)

:hammer: Other:

- Gitter badge points to non-existing room [\#268](https://github.com/devlooped/moq/issues/268)

## [v4.6.25-alpha](https://github.com/devlooped/moq/tree/v4.6.25-alpha) (2016-06-06)

[Full Changelog](https://github.com/devlooped/moq/compare/v4.5.8...v4.6.25-alpha)

:hammer: Other:

- Stop supporting Silverlight in next version? [\#257](https://github.com/devlooped/moq/issues/257)

:twisted_rightwards_arrows: Merged:

- Add .NET Core Support  [\#267](https://github.com/devlooped/moq/pull/267) (@kzu)

## [v4.5.8](https://github.com/devlooped/moq/tree/v4.5.8) (2016-05-26)

[Full Changelog](https://github.com/devlooped/moq/compare/v4.5.7...v4.5.8)

## [v4.5.7](https://github.com/devlooped/moq/tree/v4.5.7) (2016-05-26)

[Full Changelog](https://github.com/devlooped/moq/compare/v4.5.3...v4.5.7)

## [v4.5.3](https://github.com/devlooped/moq/tree/v4.5.3) (2016-05-25)

[Full Changelog](https://github.com/devlooped/moq/compare/v4.5.0...v4.5.3)

:twisted_rightwards_arrows: Merged:

- VerityMocks and VerifyAllMocks Static Helper Methods [\#238](https://github.com/devlooped/moq/pull/238) (@RehanSaeed)

## [v4.5.0](https://github.com/devlooped/moq/tree/v4.5.0) (2016-05-24)

[Full Changelog](https://github.com/devlooped/moq/compare/v4.5.9-alpha...v4.5.0)

:hammer: Other:

- Method setup broken when specifying the same parameters in a different order in setup [\#252](https://github.com/devlooped/moq/issues/252)
- NotImplementedException DynamicProxy2 error when CallBase = true while mocking interfaces [\#128](https://github.com/devlooped/moq/issues/128)
- Swapped method arguments will overwrite previous method setup [\#50](https://github.com/devlooped/moq/issues/50)

## [v4.5.9-alpha](https://github.com/devlooped/moq/tree/v4.5.9-alpha) (2016-05-22)

[Full Changelog](https://github.com/devlooped/moq/compare/v4.5.7-alpha...v4.5.9-alpha)

:hammer: Other:

- Typo in condition [\#236](https://github.com/devlooped/moq/issues/236)

:twisted_rightwards_arrows: Merged:

- Fixed setup issues when specifying the same parameters in a different order \(issue \#252\) [\#262](https://github.com/devlooped/moq/pull/262) (@LeonidLevin)

## [v4.5.7-alpha](https://github.com/devlooped/moq/tree/v4.5.7-alpha) (2016-05-22)

[Full Changelog](https://github.com/devlooped/moq/compare/v4.5.6-alpha...v4.5.7-alpha)

## [v4.5.6-alpha](https://github.com/devlooped/moq/tree/v4.5.6-alpha) (2016-05-22)

[Full Changelog](https://github.com/devlooped/moq/compare/v4.2.1510.2205...v4.5.6-alpha)

:hammer: Other:

- SetupSequence doesn't support ReturnsAsync [\#259](https://github.com/devlooped/moq/issues/259)
- Nuget package with PCL is not available [\#253](https://github.com/devlooped/moq/issues/253)
- TypeLoad Exception [\#241](https://github.com/devlooped/moq/issues/241)
- Assert.Fail not available [\#234](https://github.com/devlooped/moq/issues/234)
- Specify a mock return regardless of parameters passed into the method call [\#226](https://github.com/devlooped/moq/issues/226)
- Does your project need more contributors/maintainers? [\#222](https://github.com/devlooped/moq/issues/222)
- Why is building moq so difficult? [\#221](https://github.com/devlooped/moq/issues/221)
- Cannot setup a callback on a protected method [\#217](https://github.com/devlooped/moq/issues/217)
- Support for dnxcore5.0? [\#216](https://github.com/devlooped/moq/issues/216)
- A COM type can't make its mock by same procedure as usual. [\#215](https://github.com/devlooped/moq/issues/215)
- nonComVisibleBaseClass Error [\#213](https://github.com/devlooped/moq/issues/213)
- Thread safety issue: race condition in MockDefaultValueProvider [\#205](https://github.com/devlooped/moq/issues/205)

:twisted_rightwards_arrows: Merged:

- Adding exension methods ReturnsAsync and ThrowsAsync for ISetupSequentialResult [\#261](https://github.com/devlooped/moq/pull/261) (@abatishchev)
- Fix the out parameter issue of delegate mock [\#255](https://github.com/devlooped/moq/pull/255) (@urasandesu)
- Fix typo [\#242](https://github.com/devlooped/moq/pull/242) (@AppChecker)
- Fix for issue \#205 \(Thread safety issue: race condition in MockDefaultValueProvider\) [\#207](https://github.com/devlooped/moq/pull/207) (@mizbrodin)

## [v4.2.1510.2205](https://github.com/devlooped/moq/tree/v4.2.1510.2205) (2015-10-22)

[Full Changelog](https://github.com/devlooped/moq/compare/v4.2.1507.118...v4.2.1510.2205)

:hammer: Other:

- All Clarius Consulting links broken in README.md [\#210](https://github.com/devlooped/moq/issues/210)
- How to ascertain an application is running in 3 tier mode. [\#209](https://github.com/devlooped/moq/issues/209)
- Upgrade to Castle.Core 3.3.3 to get correct exception messages [\#203](https://github.com/devlooped/moq/issues/203)
- Revision mismatch in nuspec file [\#201](https://github.com/devlooped/moq/issues/201)
- how to use Moq for ViewModel unit testing [\#197](https://github.com/devlooped/moq/issues/197)
- Mocking Abstract Class; Implements Interface; Generic Method Fails [\#193](https://github.com/devlooped/moq/issues/193)
- Mocking extension methods [\#189](https://github.com/devlooped/moq/issues/189)

:twisted_rightwards_arrows: Merged:

- upgrading to Castle.Core 3.3.3 [\#204](https://github.com/devlooped/moq/pull/204) (@MSK61)
- Saturday morning cartoon version of "conjunction" [\#202](https://github.com/devlooped/moq/pull/202) (@breyed)
- License is BSD-3, not BSD-2 [\#198](https://github.com/devlooped/moq/pull/198) (@chkpnt)
- Make TPL code Silverlight 5 compatible [\#148](https://github.com/devlooped/moq/pull/148) (@iskiselev)

## [v4.2.1507.118](https://github.com/devlooped/moq/tree/v4.2.1507.118) (2015-06-29)

[Full Changelog](https://github.com/devlooped/moq/compare/v4.2.1506.2515...v4.2.1507.118)

:hammer: Other:

-   Callback not beeing called in current version 4.2.1506.2016   [\#183](https://github.com/devlooped/moq/issues/183)

## [v4.2.1506.2515](https://github.com/devlooped/moq/tree/v4.2.1506.2515) (2015-06-25)

[Full Changelog](https://github.com/devlooped/moq/compare/v4.2.1506.2016...v4.2.1506.2515)

:twisted_rightwards_arrows: Merged:

- Issue 184: allow null to match nullable value types [\#185](https://github.com/devlooped/moq/pull/185) (@pkpjpm)
- Added MockExtensions.Reset\(\) to clear mock state \(expectations+invocations\) [\#138](https://github.com/devlooped/moq/pull/138) (@ashmind)

## [v4.2.1506.2016](https://github.com/devlooped/moq/tree/v4.2.1506.2016) (2015-06-20)

[Full Changelog](https://github.com/devlooped/moq/compare/v4.2.1502.911...v4.2.1506.2016)

:hammer: Other:

- Invalid setup on non-virtual member, but the member is virtual [\#182](https://github.com/devlooped/moq/issues/182)
- When throwing a MockException under Strict mode it would be beneficial to indicate any generic parameters used [\#176](https://github.com/devlooped/moq/issues/176)
- NullReferenceException when mocked method has a nullable value type argument, but It.IsAny constraint is not nullable [\#90](https://github.com/devlooped/moq/issues/90)
- Allow to use CallBase instead of Returns [\#20](https://github.com/devlooped/moq/issues/20)

:twisted_rightwards_arrows: Merged:

- Dev \> Master [\#180](https://github.com/devlooped/moq/pull/180) (@kzu)
- Add generic parameter type informat to exception text. [\#177](https://github.com/devlooped/moq/pull/177) (@hasaki)
- Fix NullReferenceException when passing null to a nullable argument but trying to match it with a non-nullable IsAny. [\#160](https://github.com/devlooped/moq/pull/160) (@benjamin-hodgson)

## [v4.2.1502.911](https://github.com/devlooped/moq/tree/v4.2.1502.911) (2015-02-09)

[Full Changelog](https://github.com/devlooped/moq/compare/v4.2.1408.717...v4.2.1502.911)

:hammer: Other:

- The second invocation of a method with a Dictionary argument overwrites the dictionary passed in the previous invocation. [\#146](https://github.com/devlooped/moq/issues/146)
- Multiple return values of type task [\#144](https://github.com/devlooped/moq/issues/144)
- Issue on Dispose when using a Mock and Strict behavior \(After 4.2.1402.2112\) [\#143](https://github.com/devlooped/moq/issues/143)
- SetupAllProperties doesn't setup properties that don't have a setter [\#136](https://github.com/devlooped/moq/issues/136)
- Issue on Dispose when using a Mock on WebClient and Strict behavior [\#129](https://github.com/devlooped/moq/issues/129)
- Links to blog posts are broken in README.md [\#121](https://github.com/devlooped/moq/issues/121)
- NullReferenceException thrown by Match\<T\> [\#115](https://github.com/devlooped/moq/issues/115)

:twisted_rightwards_arrows: Merged:

- Fix: \#128 NotImplementedException DynamicProxy2 error when CallBase = true  [\#145](https://github.com/devlooped/moq/pull/145) (@rjasica)
- Issue 136 - SetupAllProperties doesn't setup properties that don't have a setter [\#137](https://github.com/devlooped/moq/pull/137) (@NGloreous)

## [v4.2.1408.717](https://github.com/devlooped/moq/tree/v4.2.1408.717) (2014-08-07)

[Full Changelog](https://github.com/devlooped/moq/compare/v4.2.1409.1722...v4.2.1408.717)

## [v4.2.1409.1722](https://github.com/devlooped/moq/tree/v4.2.1409.1722) (2014-08-07)

[Full Changelog](https://github.com/devlooped/moq/compare/v4.2.1408.619...v4.2.1409.1722)

:hammer: Other:

- Breaking Change [\#126](https://github.com/devlooped/moq/issues/126)

## [v4.2.1408.619](https://github.com/devlooped/moq/tree/v4.2.1408.619) (2014-08-06)

[Full Changelog](https://github.com/devlooped/moq/compare/v4.2.1402.2112...v4.2.1408.619)

:hammer: Other:

- Error mocking Generic Repository [\#120](https://github.com/devlooped/moq/issues/120)
- Missing ReturnsExtensions for Methods which returns Task [\#117](https://github.com/devlooped/moq/issues/117)
- Windows Phone 8 version [\#106](https://github.com/devlooped/moq/issues/106)
- Doesn't support .NET Core 4.5.1 \(can't install\) [\#100](https://github.com/devlooped/moq/issues/100)
- Mocking Interfaces [\#98](https://github.com/devlooped/moq/issues/98)
- Ordered call issue with invocations with same arguments in MockSequence [\#96](https://github.com/devlooped/moq/issues/96)

:twisted_rightwards_arrows: Merged:

- Delegate currying refactorings [\#125](https://github.com/devlooped/moq/pull/125) (@theoy)
- Should only add public interfaces [\#123](https://github.com/devlooped/moq/pull/123) (@scott-xu)
- Add exiting implemented interfaces to ImplementedInterfaces [\#119](https://github.com/devlooped/moq/pull/119) (@scott-xu)
- Only mark a condition as evaluated successfully on the method call that ... [\#97](https://github.com/devlooped/moq/pull/97) (@drieseng)
- Basic code contracts for public facing API [\#95](https://github.com/devlooped/moq/pull/95) (@alexsimply)

## [v4.2.1402.2112](https://github.com/devlooped/moq/tree/v4.2.1402.2112) (2014-02-21)

[Full Changelog](https://github.com/devlooped/moq/compare/v4.2.1312.1622...v4.2.1402.2112)

:hammer: Other:

- Override previous setup [\#89](https://github.com/devlooped/moq/issues/89)
- Mock class's method with multiple parameter only take last paramter [\#88](https://github.com/devlooped/moq/issues/88)
- unable to mock in unittesting on windows app? [\#79](https://github.com/devlooped/moq/issues/79)
- Problems with Task in 4.2 version [\#78](https://github.com/devlooped/moq/issues/78)
- Allow naming of mocks [\#74](https://github.com/devlooped/moq/issues/74)
- Can't Mock class inherited from DBContext [\#67](https://github.com/devlooped/moq/issues/67)
- Bug using Mock.Of on objects with virtual properties. [\#14](https://github.com/devlooped/moq/issues/14)

:twisted_rightwards_arrows: Merged:

- Make InterfaceProxy public [\#99](https://github.com/devlooped/moq/pull/99) (@pimterry)
- Fix Proxy -\> Poxy typo in the Silverlight project [\#85](https://github.com/devlooped/moq/pull/85) (@pimterry)
- Fix mock naming code & tests in Silverlight [\#83](https://github.com/devlooped/moq/pull/83) (@pimterry)
- Issue \#78 [\#80](https://github.com/devlooped/moq/pull/80) (@MatKubicki)
- Use default proxy generation options, rather than an explicitly empty one [\#77](https://github.com/devlooped/moq/pull/77) (@pimterry)
- Give mocks names [\#76](https://github.com/devlooped/moq/pull/76) (@pimterry)

## [v4.2.1312.1622](https://github.com/devlooped/moq/tree/v4.2.1312.1622) (2013-12-16)

[Full Changelog](https://github.com/devlooped/moq/compare/v4.2.1312.1621...v4.2.1312.1622)

:twisted_rightwards_arrows: Merged:

- Fixing \#62 - Multithreaded test not working [\#68](https://github.com/devlooped/moq/pull/68) (@MatKubicki)

## [v4.2.1312.1621](https://github.com/devlooped/moq/tree/v4.2.1312.1621) (2013-12-16)

[Full Changelog](https://github.com/devlooped/moq/compare/v4.2.1312.1615...v4.2.1312.1621)

:twisted_rightwards_arrows: Merged:

- Fix error when returning Task of a reference type [\#73](https://github.com/devlooped/moq/pull/73) (@alextercete)

## [v4.2.1312.1615](https://github.com/devlooped/moq/tree/v4.2.1312.1615) (2013-12-16)

[Full Changelog](https://github.com/devlooped/moq/compare/v4.2.1312.1416...v4.2.1312.1615)

## [v4.2.1312.1416](https://github.com/devlooped/moq/tree/v4.2.1312.1416) (2013-12-14)

[Full Changelog](https://github.com/devlooped/moq/compare/v4.2.1312.1323...v4.2.1312.1416)

:twisted_rightwards_arrows: Merged:

- InSequence-Setups can now be used on the same mock instance. [\#72](https://github.com/devlooped/moq/pull/72) (@halllo)

## [v4.2.1312.1323](https://github.com/devlooped/moq/tree/v4.2.1312.1323) (2013-12-13)

[Full Changelog](https://github.com/devlooped/moq/compare/v4.2.1312...v4.2.1312.1323)

:twisted_rightwards_arrows: Merged:

- Make Async methods on IReturns more discoverable [\#71](https://github.com/devlooped/moq/pull/71) (@jdom)

## [v4.2.1312](https://github.com/devlooped/moq/tree/v4.2.1312) (2013-12-13)

[Full Changelog](https://github.com/devlooped/moq/compare/v4.2.1312.1319...v4.2.1312)

## [v4.2.1312.1319](https://github.com/devlooped/moq/tree/v4.2.1312.1319) (2013-12-13)

[Full Changelog](https://github.com/devlooped/moq/compare/v4.1.1311.615...v4.2.1312.1319)

:hammer: Other:

- Cannot use Mock\<T\>.Raise\(\) on WPF Routed Events [\#61](https://github.com/devlooped/moq/issues/61)

:twisted_rightwards_arrows: Merged:

- Make TPL code Silverlight compatible [\#70](https://github.com/devlooped/moq/pull/70) (@alextercete)
- Re-normalize repository [\#69](https://github.com/devlooped/moq/pull/69) (@alextercete)
- Avoid NullReferenceException with async methods [\#66](https://github.com/devlooped/moq/pull/66) (@alextercete)
- Add extension methods for IReturns to allow specifying return values exceptions on async methods in a synchronous way [\#60](https://github.com/devlooped/moq/pull/60) (@danielcweber)

## [v4.1.1311.615](https://github.com/devlooped/moq/tree/v4.1.1311.615) (2013-11-06)

[Full Changelog](https://github.com/devlooped/moq/compare/v4.1.1309.919...v4.1.1311.615)

:hammer: Other:

- Mock.As appears to be broken in Moq 4.1.1308.2321 [\#54](https://github.com/devlooped/moq/issues/54)

:twisted_rightwards_arrows: Merged:

- adding sequential capability to throws for testing retry logic in web se... [\#59](https://github.com/devlooped/moq/pull/59) (@kellyselden)
- Matcher should always work with the whole expression, including Convert [\#56](https://github.com/devlooped/moq/pull/56) (@svick)

## [v4.1.1309.919](https://github.com/devlooped/moq/tree/v4.1.1309.919) (2013-09-09)

[Full Changelog](https://github.com/devlooped/moq/compare/v4.1.1309.1617...v4.1.1309.919)

## [v4.1.1309.1617](https://github.com/devlooped/moq/tree/v4.1.1309.1617) (2013-09-09)

[Full Changelog](https://github.com/devlooped/moq/compare/v4.1.1309.801...v4.1.1309.1617)

## [v4.1.1309.801](https://github.com/devlooped/moq/tree/v4.1.1309.801) (2013-09-08)

[Full Changelog](https://github.com/devlooped/moq/compare/v4.1.1308.2321...v4.1.1309.801)

:twisted_rightwards_arrows: Merged:

- support for reset of the call counts on all members. [\#55](https://github.com/devlooped/moq/pull/55) (@salfab)

## [v4.1.1308.2321](https://github.com/devlooped/moq/tree/v4.1.1308.2321) (2013-08-23)

[Full Changelog](https://github.com/devlooped/moq/compare/v4.1.1309.800...v4.1.1308.2321)

## [v4.1.1309.800](https://github.com/devlooped/moq/tree/v4.1.1309.800) (2013-08-23)

[Full Changelog](https://github.com/devlooped/moq/compare/v4.1.1308.2316...v4.1.1309.800)

:twisted_rightwards_arrows: Merged:

- Merge dev into master [\#52](https://github.com/devlooped/moq/pull/52) (@kzu)
- \#326 [\#51](https://github.com/devlooped/moq/pull/51) (@stoo101)
- Remove leftover git markup and code erroneously included in bad merge. Fixes \#43 [\#45](https://github.com/devlooped/moq/pull/45) (@yonahw)
- Added covariant IMock\<out T\> interface to Mock\<T\> [\#44](https://github.com/devlooped/moq/pull/44) (@tkellogg)
- Added It.IsAnyValue [\#40](https://github.com/devlooped/moq/pull/40) (@Pjanssen)
- Fix for issue 361 [\#39](https://github.com/devlooped/moq/pull/39) (@bittailor)
- Fixed collection modified exception thrown as result of more methods bei... [\#36](https://github.com/devlooped/moq/pull/36) (@yonahw)
- added overload to Verify to accept Times as a Method Group [\#34](https://github.com/devlooped/moq/pull/34) (@ChrisMissal)
- refactored Interceptor.Intercept to use a set of strategies. . [\#31](https://github.com/devlooped/moq/pull/31) (@FelicePollano)
- added gitignore [\#30](https://github.com/devlooped/moq/pull/30) (@FelicePollano)
- Fixed space/tab mismatch in solving Issue \#249  [\#29](https://github.com/devlooped/moq/pull/29) (@FelicePollano)
- Feature request: It.IsIn\(..\), It.IsNotIn\(...\) [\#27](https://github.com/devlooped/moq/pull/27) (@rdingwall)
- Corrected Verify method behavior for generic methods calls [\#25](https://github.com/devlooped/moq/pull/25) (@Suremaker)

## [v4.1.1308.2316](https://github.com/devlooped/moq/tree/v4.1.1308.2316) (2013-08-23)

[Full Changelog](https://github.com/devlooped/moq/compare/v4.1.1308.2120...v4.1.1308.2316)

## [v4.1.1308.2120](https://github.com/devlooped/moq/tree/v4.1.1308.2120) (2013-08-21)

[Full Changelog](https://github.com/devlooped/moq/compare/v4.0.10827...v4.1.1308.2120)

:hammer: Other:

- Leftover git markup in Interceptor.cs [\#43](https://github.com/devlooped/moq/issues/43)
- feature: differentiate crashes from verification errors [\#15](https://github.com/devlooped/moq/issues/15)

:twisted_rightwards_arrows: Merged:

- ILMerge compatible .NET 4.5 [\#48](https://github.com/devlooped/moq/pull/48) (@lukas-ais)
- Fix using Mock.Of on objects having properties with private setters [\#19](https://github.com/devlooped/moq/pull/19) (@yorah)
- added some type parameters [\#18](https://github.com/devlooped/moq/pull/18) (@quetzalcoatl)
- differentiate verification error from mock crash [\#16](https://github.com/devlooped/moq/pull/16) (@quetzalcoatl)
- Fix issue \#325 [\#13](https://github.com/devlooped/moq/pull/13) (@IharBury)
- Fix castle references in .csproj [\#11](https://github.com/devlooped/moq/pull/11) (@yorah)
- Add .gitignore file [\#10](https://github.com/devlooped/moq/pull/10) (@yorah)
- Fix Issue \#328 [\#9](https://github.com/devlooped/moq/pull/9) (@yorah)
- CallBase\(\) Solution fixes [\#8](https://github.com/devlooped/moq/pull/8) (@srudin)
- Solving issue 319: SetupSequentialContext with Throws [\#7](https://github.com/devlooped/moq/pull/7) (@lukas-ais)
- Capability of mocking delegates \(event handlers\) [\#4](https://github.com/devlooped/moq/pull/4) (@quetzalcoatl)

## [v4.0.10827](https://github.com/devlooped/moq/tree/v4.0.10827) (2010-08-19)

[Full Changelog](https://github.com/devlooped/moq/compare/bbadca0853506904f48aed419f8be6a6359e5fe0...v4.0.10827)



\* *This Changelog was automatically generated by [github_changelog_generator](https://github.com/github-changelog-generator/github-changelog-generator)*
