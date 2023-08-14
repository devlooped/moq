# CleanMoq

![CleanMoq](https://raw.githubusercontent.com/hassanhabib/CleanMoq/e52332d5f3a991a7827f8133bb8a95c057753a68/assets/img/cleanmoq_git_logo.png)

[![Nuget](https://img.shields.io/nuget/v/CleanMoq?logo=nuget&style=default)](https://www.nuget.org/packages/CleanMoq)
![Nuget](https://img.shields.io/nuget/dt/CleanMoq?logo=nuget&style=default&color=blue&label=Downloads)
[![The Standard Community](https://img.shields.io/discord/934130100008538142?style=default&color=%237289da&label=The%20Standard%20Community&logo=Discord)](https://discord.gg/vdPZ7hS52X)


## Introduction
This is CleanMoq a branch of the popular Moq library.

All copyrights of use can be found in the  license text [here](https://github.com/hassanhabib/CleanMoq/blob/main/License.txt)

CleanMoq is a mocking library for .NET developers to take full advantage of .NET Linq expression trees and lambda expressions, which makes it the most productive, type-safe and refactoring-friendly mocking library available. And it supports mocking interfaces as well as classes. Its API is extremely simple and straightforward, and doesn't require any prior knowledge or experience with mocking concepts.

## Purpose
For use in engineering standard compliant systems.
We take the utmost opinion that no bots or obtrusive hidden features without consent are intruduced into the background of any of our dependent libraries with the highest regard.  
This forked branch of the original Moq library was born to do just that, to protect all our systems from future potential unwarranted harms.
Please use with confidence in building your Standard Compliant systems.

## Roadmap
On August 4, 2023 SponserLink was introduced in the 4.20.0 version of the Moq library by its creators, and on August 9, 2023 it was removed via the proj file only after the Apple systems failed to build.

Moq Version 4.20.69 released on August 11, 2023 states SponsorLink removal via pull request #1375.  Again only with the removal of the project reference. SponsorLink is still a viable potential threat of being reimplemented at any givin point in time as the coding for it still resides within this cuurent version v4.20.69.

CleanMoq was forked from Moq version 4.20.0 where all reference coding to SponsorLink has been removed not just the project reference within the Moq.CodeAnalysis project file. 

It has been completely removed in CleanMoq.

Version 1.0.0 is the beginning of CleanMoq, it is the version 4.20.0 without the spywhere.

CleanMoq will be enhanced to standard principles as required and versioning forward will commence from version 1.0.0. 

CleanMoq may or may not be updated with future enhancements of the original Moq library as it progresses towards the future. 

CleanMoq will stand as its own library into the future, we hope we can build everyones confidence in using it without worry or concerns. 

The motto remains the same in The Standard Community (Coalition of Good-Hearted Engineers)

## How to install
Install from [NuGet](http://nuget.org/packages/CleanMoq).

## How to use
### Standard compliant sample setup for service dependencies
```csharp
   public partial class AIFileServiceTests
    {
        private readonly Mock<IOpenAIBroker> openAIBrokerMock;
        private readonly Mock<IDateTimeBroker> dateTimeBrokerMock;
        private readonly ICompareLogic compareLogic;
        private readonly IAIFileService aiFileService;

        public AIFileServiceTests()
        {
            this.openAIBrokerMock = new Mock<IOpenAIBroker>();
            this.dateTimeBrokerMock = new Mock<IDateTimeBroker>();
            this.compareLogic = new CompareLogic();

            this.aiFileService = new AIFileService(
                openAIBroker: this.openAIBrokerMock.Object,
                dateTimeBroker: this.dateTimeBrokerMock.Object);
        }
```

### Sample test employing verify functions 

```csharp
    [Fact]
        private async Task ShouldThrowDependencyExceptionOnUploadIfUrlNotFoundAsync()
        {
            // given
            AIFile someAIFile = CreateRandomAIFile();

            var httpResponseUrlNotFoundException =
                new HttpResponseUrlNotFoundException();

            var invalidConfigurationFileException =
                new InvalidConfigurationAIFileException(
                    message: "Invalid AI file configuration error occurred, contact support.",
                        httpResponseUrlNotFoundException);

            var expectedFileDependencyException =
                new AIFileDependencyException(
                    message: "AI file dependency error occurred, contact support.",
                        invalidConfigurationFileException);

            this.openAIBrokerMock.Setup(broker =>
                broker.PostFileFormAsync(It.IsAny<ExternalAIFileRequest>()))
                    .ThrowsAsync(httpResponseUrlNotFoundException);

            // when
            ValueTask<AIFile> uploadFileTask =
                this.aiFileService.UploadFileAsync(someAIFile);

            AIFileDependencyException actualFileDependencyException =
                await Assert.ThrowsAsync<AIFileDependencyException>(
                    uploadFileTask.AsTask);

            // then
            actualFileDependencyException.Should().BeEquivalentTo(
                expectedFileDependencyException);

            this.openAIBrokerMock.Verify(broker =>
               broker.PostFileFormAsync(It.IsAny<ExternalAIFileRequest>()),
                   Times.Once);

            this.openAIBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }

```

## Why?

The library was created mainly for developers who aren't currently using any mocking library or are displeased with the complexities of some other implementation.

CleanMoq is designed to be a very practical, unobtrusive and straight-forward way to quickly setup dependencies for your tests. Its API design helps even novice users to fall in the "pit of success" and avoid most common misuses/abuses of mocking.

## Who?

Moq was originally developed by [Clarius](http://www.clariusconsulting.net), [Manas](http://www.manas.com.ar) and [InSTEDD](http://www.instedd.org).

Moq uses [Castle DynamicProxy](http://www.castleproject.org/projects/dynamicproxy/) internally as the interception mechanism to enable mocking.

## Features at a glance
CleanMoq offers all the same original features as the Moq library, features like below:
* Strong-typed: no strings for expectations, no object-typed return values or constraints
* Unsurpassed VS IntelliSense integration: everything supports full VS IntelliSense, from setting expectations, to specifying method call arguments, return values, etc.
* No Record/Replay idioms to learn. Just construct your mock, set it up, use it and optionally verify calls to it (you may not verify mocks when they act as stubs only, or when you are doing more classic state-based testing by checking returned values from the object under test)
* VERY low learning curve as a consequence of the previous three points. For the most part, you don't even need to ever read the documentation.
* Granular control over mock behavior with a simple MockBehavior.
* Mock both interfaces and classes
* Override expectations: can set default expectations in a fixture setup, and override as needed on tests
* Pass constructor arguments for mocked classes
* Intercept and raise events on mocks
* Intuitive support for ```out/ref``` arguments

## Thank You
Special thanks to the original maintainers and supporters of Moq - find the list on the front-page for Moq [here](https://github.com/moq/moq)
