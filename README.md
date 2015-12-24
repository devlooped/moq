moq
===

##Table of Contents
**[1. Intro](#intro)**
**[2. What](#what)**
**[3. Why](#why)**
**[4. Where](#where)**
**[5. Who](#who)**
**[6. Step By Step Beginner's Guide](#beginner)**
**[7. Feedback](#feedback)**

<div id="intro"></div>
The most popular and friendly mocking framework for .NET

[![NuGet downloads](https://img.shields.io/nuget/dt/Moq.svg)](https://www.nuget.org/packages/Moq)
[![Version](https://img.shields.io/nuget/v/Moq.svg)](https://www.nuget.org/packages/Moq)
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

Moq also is the first and only framework so far to provide Linq to Mocks, so that the same behavior above can be achieved much more succintly:

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

<div id="what"></div>

## What?

Moq (pronounced "Mock-you" or just "Mock") is the only mocking library for .NET developed from scratch to take full advantage of .NET Linq expression trees and lambda expressions, which makes it the most productive, type-safe and refactoring-friendly mocking library available. And it supports mocking interfaces as well as classes. Its API is extremely simple and straightforward, and doesn't require any prior knowledge or experience with mocking concepts.

<div id="why"></div>

## Why?

The library was created mainly for developers who aren't currently using any mocking library (or are displeased with the complexities of some other implementation), and who are typically [manually writing their own mocks](http://blogs.clariusconsulting.net/kzu/mocks-stubs-and-fakes-its-a-continuum/) (with more or less "fanciness"). Most developers in this situation also happen to be [quite pragmatic and adhere to state](http://blogs.clariusconsulting.net/kzu/state-testing-vs-interaction-testing/) (or classic) TDD. It's the result of feeling that the barrier of entry from other mocking libraries is a bit high, and a simpler, more lightweight and elegant approach is possible. Moq achieves all this by taking full advantage of the elegant and compact C# and VB language features collectively known as LINQ (they are not just for queries, as the acronym implies). 

Moq is designed to be a very practical, unobtrusive and straight-forward way to quickly setup dependencies for your tests. Its API design helps even novice users to fall in the "pit of success" and avoid most common misuses/abuses of mocking. 

When it was conceived, it was the only mocking library that went against the generalized and somewhat unintuitive (especially for novices) Record/Replay approach from all other frameworks (and [that might have been a good thing](http://blogs.clariusconsulting.net/kzu/whats-wrong-with-the-recordreplyverify-model-for-mocking-frameworks/) ;)). 

Not using Record/Replay also means that it's straightforward to move common expectations to a fixture setup method and even override those expectations when needed in a specific unit test.

You can read more about the "why" and see some nice screenshots at [kzu's blog](http://blogs.clariusconsulting.net/kzu/why-do-we-need-yet-another-net-mocking-framework/).

<div id="where"></div>

## Where?

See our [Quickstart](https://github.com/Moq/moq4/wiki/Quickstart) examples to get a feeling of the extremely simple API and install from [nuget](http://nuget.org/packages/moq). Check out the API documentation at [NuDoq](http://www.nudoq.org/#!/Projects/Moq).

Read about the announcement at [kzu's blog](http://blogs.clariusconsulting.net/kzu/linq-to-mock-moq-is-born/). Get some background on [the state of mock libraries from Scott Hanselman](http://www.hanselman.com/blog/MoqLinqLambdasAndPredicatesAppliedToMockObjects.aspx). 

<div id="who"></div>

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

<div id="beginner"></div>

## The beginner's guide to Mocking (using Moq)

Obivously there is always more than one solution for writing unit tests / mocking. 

In this guide I will show you how to Mock using a test class with properties that contain the Classes I want to mock and my Repository. 

This guide will show you how to write Setup and Teardown helper methods for your Test Class.. but more on that later.


#### Table of Contents
**[Assumptions](#assumptions)**
**[Why Mock](#why)**
**[Explore Full Example](#fullexample)**
**[1. Create Test Class](#first)**
**[2. Create Setup Helper Method](#second)**
**[3. Create Cleanup Helper Method](#third)**
**[4. Connect Mocks to Data Source](#fourth)**
**[5. The Test Method](#five)**
**[6. Closing](#closingguide)**


<div id="assumptions" /></div>

## Assumptions
In order to really understand what is happening, you probably should be familiar with the following concepts:

+ __Unit Testing__
-- [Unit Test Basics](https://msdn.microsoft.com/en-us/library/hh694602.aspx)

* __DbContext, DbSets__ 
--  [the DbContext Class](https://msdn.microsoft.com/en-us/data/jj729737.aspx) 
--    [Defining DbSets](https://msdn.microsoft.com/en-us/data/jj592675.aspx)

* __Repository Pattern__ 
While not everyone uses the repository pattern, this code sample uses the
repository pattern which creates a layer of abstraction / interaction between the 
DbContext and the Controller.
--[Lean More at ASP.net](http://www.asp.net/mvc/overview/older-versions/getting-started-with-ef-5-using-mvc-4/implementing-the-repository-and-unit-of-work-patterns-in-an-asp-net-mvc-application) 
---

<div id="why"></div>

## Why does one need to Mock?

Mocking is used when you need to test your application's interaction with the database. This will ensure that your test methods do not inject data into your database when the test methods are run. 

<div id="fullexample" /></div>
## Explore Full Example
** Let's explore the following code as a whole and then we can break it down bit by bit **

```
using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using My.Models;
using System.Data.Entity;
using Moq;
using System.Collections.Generic;
using System.Linq;

namespace My.Tests.Models
{
    [TestClass]
    public class RepositoryTests
    {
        private Mock<MyContext> _context;
        private MyRepository _repo;
        private Mock<DbSet<MySet>> _mySet;

        private void ConnectMocksToDataStore(IEnumerable<MySet> data_store)
        {
            var data_source = data_store.AsQueryable();
            _mySet.As<IQueryable<MySet>>().Setup(data => data.Provider).Returns(data_source.Provider);
            _mySet.As<IQueryable<MySet>>().Setup(data => data.Expression).Returns(data_source.Expression);
            _mySet.As<IQueryable<MySet>>().Setup(data => data.ElementType).Returns(data_source.ElementType);
            _mySet.As<IQueryable<MySet>>().Setup(data => data.GetEnumerator()).Returns(data_source.GetEnumerator());
            _context.Setup(a => a.SetName).Returns(_mySet.Object);
        }

        [TestInitialize]
        public void Initialize()
        {
            _context = new Mock<MyContext>();
            _mySet = new Mock<DbSet<MySet>>();
            _repo = new MyRepository(_context.Object);
        }

        [TestCleanup]
        public void Cleanup()
        {
            _context = null;
            _mySet = null;
            _repo = null;
        }
    
        [TestMethod]
        public void RepositoryTestsEnsureICanGetAllUsers()
        {
            List<User> users = new List<User>()
            {
                new User {UserID = 1, Handle = "Stiffy"},
                new User {UserID = 2, Handle = "Michael"}
            };

            _mySet.Object.AddRange(users);
            ConnectMocksToDataStore(users);

            var actual = _repo.GetAllUsers();  
            Assert.AreEqual(1, actual[0].UserID);
            CollectionAssert.AreEqual(users, actual);
        }
     }
}
```
<div id="first"></div>
## Step One 
### Create Test Class with Private Data Members Containing Your Mocks and Repo.

Let's take a look at the first part of the code you saw earlier:

```
    [TestClass]
    public class RepositoryTests
    {
        private Mock<MyContext> _context;
        private MyRepository _repo;
        private Mock<DbSet<MySet>> _mySet;

```

1. I've created a class with private properties that WILL contain:
- a Mock of my DbContext
- a Mock of my DbSet(s)
- my Repository

In the line of code 
`private Mock<MyContext> _context;`  
you can see I've created a private data member named `_context` of type `Mock<MyContext>` where *MyContext* is the name of my DbContext class I have already created.

As a reminder, I have not Mocked anything at this point. I've merely created private data members that will hold my Mocks. 

<div id="second"></div>
## Step Two
### Create Setup Helper Method

Let's take a look at this bit of code:

```
        [TestInitialize]
        public void Initialize()
        {
            _context = new Mock<MyContext>();
            _mySet = new Mock<DbSet<MySet>>();
            _repo = new MyRepository(_context.Object);
        }
```

#### What this does:

the `[TestInitialize]` Attribute Tag Identifies the method to run before the test to allocate and configure resources needed by all tests in the test class.

This seems simple enough right? This `Initialize()` Method is still just setting up my environment so that I can actually write my tests exercising my Mocks. You can see in the `Initialize()` method I've assigned each property an instance of a Mock Context, Mock Set and a Repository passing in my Mocked Context... 

The only thing to note about the line of code 
`_repo = new MyRepository(_context.Object);` is when I created MyRepository class, I created a constructor that accepts a class of MyContext as a parameter and assigns the passed in MyContext to a private property in the class.

**So why did you pass in `_context.Object`  and not simply `_context` when calling the MyRepository Constructor ?**

This is because I have MOCKED my context. 

MOQ API reads - `Object(Property) - Gets the Mocked Object Instance`

In other words using `.Object` actually EXPOSES the context (in this case the DbContext class I've made called MyContext) and is allowing me to use it as if it was not Mocked. 

<div id="third"></div>
## Step Three
### Create a CleanUp Method

This helper method is just going to free up resources after each test is run. In otherwords, each individual Test Method will interact with only data in our Mocked "Database" that I specify in each Test Method. Each Test Method works with Data independent of one another. 

```
        [TestCleanup]
        public void Cleanup()
        {
            _context = null;
            _mySet = null;
            _repo = null;
        }
```

Simple.. the `[TestCleanup]` tag specifies the Cleanup() Method is to be ran at the completion of each test. 


<div id="fourth"></div>
## Step Four
### Create a Method that connects your Mocks to the data source

Let's take a look at this bit of code:

``` 
        private void ConnectMocksToDataStore(IEnumerable<MySet> data_store)
        {
            var data_source = data_store.AsQueryable();
            _mySet.As<IQueryable<MySet>>().Setup(data => data.Provider).Returns(data_source.Provider);
            _mySet.As<IQueryable<MySet>>().Setup(data => data.Expression).Returns(data_source.Expression);
            _mySet.As<IQueryable<MySet>>().Setup(data => data.ElementType).Returns(data_source.ElementType);
            _mySet.As<IQueryable<MySet>>().Setup(data => data.GetEnumerator()).Returns(data_source.GetEnumerator());
            _context.Setup(a => a.SetName).Returns(_mySet.Object);
        }
```

Here I have created a Method named `ConnectMocksToDataStore` that accepts a type of `IEnumerable<MySet>`.

** Why did you set this Method up with an IEnumerable parameter? **

Good Question, and I think it's best answered if I explain what I am trying to accomplish with this method. When I am writing my tests, I will actually build out a "simulated database table" by using a List, and you will see that as you read further. I want to make that List Queryable, and that is what you see happening with this line:
`var data_source = data_store.AsQueryable();` 

The .AsQueryable() method converts an IEnumerable or IEnumerable<T> to an IQueryable or IQueryable<T>, so when I call this later passing in a list that I will build within a test method, that list becomes Queryable when I pass the list into the above method.

Similar thing is happening with my Mock Set, _mySet with the following line of code:

`_mySet.As<IQueryable<MySet>>().Setup(data => data.Provider).Returns(data_source.Provider);`

(and the same applies to the .Expression, .ElementType, and .GetEnumerator()  in the subsequent three lines of code.)

remember `_mySet` is my Mocked DbSet of the class I created called MySet... 

`mySet.As<IQueryable<MySet>>()` is actually part of MOQ.. I am now using the MOQ framework.

According to MOQ's API Documentation there is a `.As<TInterface>` Method available... 

** What is the `.As<TInterface>` Method call doing? ** 

Glad you asked. 

This Method call is allowing me to implement the IQueryable Interface to my mocked object... in the code above.. I am implementing the IQueryable Interface to my Mocked Set `_mySet`.

So that clears that little bit up.. let's examine what the .Setup() and .Returns() method calls are doing.

As you remember, an Interface requires us to implement the Signatures for Methods, Properties, Indexers, or Events it defines in the Interface, and that is why you see the following lines of code: 

```
.Setup(data => data.Provider).Returns(data_source.Provider);
.Setup(data => data.Expression).Returns(data_source.Expression);
.Setup(data => data.ElementType).Returns(data_source.ElementType);
.Setup(data => data.GetEnumerator()).Returns(data_source.GetEnumerator());
```

I am using the MOQ Framework `.Setup()` and `.Returns()` methods to specify when a Method like GetEnumerator() is called on my Mocked object, it will return the GetEnumerator() Method's return value from whatever I initially passed into the `ConnectMocksToDataStore(IEnumerable<MySet> data_source)` method call.

This last line of code from the above snippet:

`_context.Setup(a => a.SetName).Returns(_mySet.Object)`

I am specifying the behavior of my Mocked Context using the same MOQ framework Method Calls of `.Setup()` and `.Returns()`

If you are familiar with the setting up a DbContext, as you should be since I wrote that was an assumption about your knowledge level at the beginning of this guide, the lamda expression `(a => a.SetName)` , + SetName + is just the name of the Property in your DbContext that contains your DbSet. In this case, I am specifying that my Mocked Context, when calling the property SetName, will return the Mocked MySet Object.

<div id="five"></div>

## Step Five
### Writing Test Methods To Exercies Our Mocks

So now we have made it to the meat and potatoes of all that work we have done. We are ready to exercise and test our Mocks. 

Let's examine the following code:

```
        [TestMethod]
        public void RepositoryTestsEnsureICanGetAllUsers()
        {
            List<User> users = new List<User>()
            {
                new User {UserID = 1, Handle = "Stiffy"},
                new User {UserID = 2, Handle = "Michael"}
            };

            _mySet.Object.AddRange(users);
            ConnectMocksToDataStore(users);

            var actual = _repo.GetAllUsers();  
            Assert.AreEqual(1, actual[0].UserID);
            CollectionAssert.AreEqual(users, actual);
        }
```
I have setup the following TestMethod.

You can see that I am ARRANGING my test by creating a List<User> named users and adding new users to that list. 

The line of code `_mySet.Object.AddRange(users)` adds that list to my Mocked Set and I am calling the Method I wrote in Step Three and passing in the List. Review Step Three if you forgot what the `ConnectMocksToDataStore(users)` line of code is doing. 

The last lines of code is just my assertions for this test. 

The line `var actual = _repo.GetAllUsers()` is just calling the GetAllUsers() Method from my created Repository class that contains the actual LINQ statement for retrieving the data. 

---
<div id="closingguide"></div>
##In Closing

Again, as a reminder, this is only one way of using the MOQ framework to Mock and Exercise your "Database" using Memory versus an actual Database. Be cognizant that this can result in different behavior than using Entity Framework’s LINQ provider (LINQ to Entities) to translate queries into SQL that is run against your database.

---

<div id="feedback"></div>

##Feedback

We appreciate deeply any [feedback](http://moq.uservoice.com/) that you may have!

![OhLoh](http://www.ohloh.net/p/moq/widgets/project_thin_badge.gif)

![ClariusLabs](http://download.codeplex.com/Project/Download/FileDownload.aspx?ProjectName=clarius&DownloadId=17830&Build=14806&boo.png)
