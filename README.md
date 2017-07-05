moq
===

The most popular and friendly mocking framework for .NET

[![Version](https://img.shields.io/nuget/v/Moq.svg)](https://www.nuget.org/packages/Moq)
[![Downloads](https://img.shields.io/nuget/dt/Moq.svg)](https://www.nuget.org/packages/Moq)
[![Join the chat at https://gitter.im/Moq](https://badges.gitter.im/Join%20Chat.svg)](https://gitter.im/Moq?utm_source=badge&utm_medium=badge&utm_campaign=pr-badge&utm_content=badge)


```csharp
  var mock = new Mock<ILoveThisFramework>();

  // WOW! No record/replay weirdness?! :)
  mock.Setup(framework => framework.DownloadExists("2.0.0.0"))
      .Returns(true);

  // Hand mock.Object as a collaborator and exercise it, 
  // like calling methods on it...
  ILoveThisFramework lovable = mock.Object;
  bool download = lovable.DownloadExists("2.0.0.0");

  // Verify that the given method was indeed called with the expected value at most once
  mock.Verify(framework => framework.DownloadExists("2.0.0.0"), Times.AtMostOnce());
```

Moq also is the first and only framework so far to provide Linq to Mocks, so that the same behavior above can be achieved much more succinctly:

```csharp
  ILoveThisFramework lovable = Mock.Of<ILoveThisFramework>(l =>
    l.DownloadExists("2.0.0.0") == true);

  // Hand the instance as a collaborator and exercise it, 
  // like calling methods on it...
  bool download = lovable.DownloadExists("2.0.0.0");

  // Simply assert the returned state:
  Assert.True(download);
  
  // If you really want to go beyond state testing and want to 
  // verify the mock interaction instead...
  Mock.Get(lovable).Verify(framework => framework.DownloadExists("2.0.0.0"));
```

You can think of Linq to Mocks as "from the universe of mocks, give me one whose behavior matches this expression".

Checkout the [Quickstart](https://github.com/Moq/moq4/wiki/Quickstart) for more examples!

## What?

Moq (pronounced "Mock-you" or just "Mock") is the only mocking library for .NET developed from scratch to take full advantage of .NET Linq expression trees and lambda expressions, which makes it the most productive, type-safe and refactoring-friendly mocking library available. And it supports mocking interfaces as well as classes. Its API is extremely simple and straightforward, and doesn't require any prior knowledge or experience with mocking concepts.

## Why?

The library was created mainly for developers who aren't currently using any mocking library (or are displeased with the complexities of some other implementation), and who are typically [manually writing their own mocks](http://blogs.clariusconsulting.net/kzu/mocks-stubs-and-fakes-its-a-continuum/) (with more or less "fanciness"). Most developers in this situation also happen to be [quite pragmatic and adhere to state](http://blogs.clariusconsulting.net/kzu/state-testing-vs-interaction-testing/) (or classic) TDD. It's the result of feeling that the barrier of entry from other mocking libraries is a bit high, and a simpler, more lightweight and elegant approach is possible. Moq achieves all this by taking full advantage of the elegant and compact C# and VB language features collectively known as LINQ (they are not just for queries, as the acronym implies). 

Moq is designed to be a very practical, unobtrusive and straight-forward way to quickly setup dependencies for your tests. Its API design helps even novice users to fall in the "pit of success" and avoid most common misuses/abuses of mocking. 

When it was conceived, it was the only mocking library that went against the generalized and somewhat unintuitive (especially for novices) Record/Replay approach from all other frameworks (and [that might have been a good thing](http://blogs.clariusconsulting.net/kzu/whats-wrong-with-the-recordreplyverify-model-for-mocking-frameworks/) ;)). 

Not using Record/Replay also means that it's straightforward to move common expectations to a fixture setup method and even override those expectations when needed in a specific unit test.

You can read more about the "why" and see some nice screenshots at [kzu's blog](http://blogs.clariusconsulting.net/kzu/why-do-we-need-yet-another-net-mocking-framework/).

## Where?

See our [Quickstart](https://github.com/Moq/moq4/wiki/Quickstart) examples to get a feeling of the extremely simple API and install from [nuget](http://nuget.org/packages/moq). Check out the API documentation at [NuDoq](http://www.nudoq.org/#!/Projects/Moq).

Read about the announcement at [kzu's blog](http://blogs.clariusconsulting.net/kzu/linq-to-mock-moq-is-born/). Get some background on [the state of mock libraries from Scott Hanselman](http://www.hanselman.com/blog/MoqLinqLambdasAndPredicatesAppliedToMockObjects.aspx). 


## Who?

Moq was originally developed by [Clarius](http://www.clariusconsulting.net), [Manas](http://www.manas.com.ar) and [InSTEDD](http://www.instedd.org).

Moq uses [Castle DynamicProxy](http://www.castleproject.org/projects/dynamicproxy/) internally as the interception mechanism to enable mocking. It's merged into Moq binaries, so you don't need to do anything other than referencing Moq.dll, though.

## Features at a glance
Moq offers the following features:
  * Strong-typed: no strings for expectations, no object-typed return values or constraints
  * Unsurpassed VS intellisense integration: everything supports full VS intellisense, from setting expectations, to specifying method call arguments, return values, etc.
  * No Record/Replay idioms to learn. Just construct your mock, set it up, use it and optionally verify calls to it (you may not verify mocks when they act as stubs only, or when you are doing more classic state-based testing by checking returned values from the object under test)
  * VERY low learning curve as a consequence of the previous three points. For the most part, you don't even need to ever read the documentation.
  * Granular control over mock behavior with a simple  [MockBehavior](http://www.nudoq.org/#!/Packages/Moq/Moq/MockBehavior)  enumeration (no need to learn what's the theoretical difference between a mock, a stub, a fake, a dynamic mock, etc.)
  * Mock both interfaces and classes
  * Override expectations: can set default expectations in a fixture setup, and override as needed on tests
  * Pass constructor arguments for mocked classes
  * Intercept and raise events on mocks
  * Intuitive support for out/ref arguments

We appreciate deeply any [feedback](http://moq.uservoice.com/) that you may have!

![OhLoh](http://www.ohloh.net/p/moq/widgets/project_thin_badge.gif)

![ClariusLabs](http://download.codeplex.com/Project/Download/FileDownload.aspx?ProjectName=clarius&DownloadId=17830&Build=14806&boo.png)
