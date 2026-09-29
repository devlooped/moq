# Moq SDK

As explained in the [Overview](Overview.md), the goal of this SDK is to provide a common low-level API that allows rich mocking APIs to be built on top. Moq itself is of course the primary example and driver of the SDK features, but they are not tied to any particular setup or verification API. In particular, it provides method invocation matching/setup and introspection, invocation recording and instrospection, and a general-purpose state bag associated with every mock object for use in custom behaviors.

The SDK builds on top of [Stunts](Stunts.md) and its behavior pipeline. The Moq SDK provides sub-pipelines of (mock) behaviors that are executed in order whenever the sub-pipeline setup matches the current invocation, such as:

```
MyProxy
    - stunt behavior 1 (i.e. record all calls)
    - stunt behavior 2 (i.e. logging)
    - mock (stunt) behavior 3 for setup X (i.e. a property getter)
        - stunt behavior a (i.e. invoke a callback)
        - stunt behavior b (i.e. return a value)
    - mock (stunt) behavior 4 for setup Y (i.e. a method invocation with specific argument values)
        - stunt behavior c (i.e. invoke a callback)
        - stunt behavior d (i.e. throw an exception)
    - stunt behavior 5 (i.e. return default values)
```

The SDK extends the *Stunts* [AppliesTo](../src/Stunts/Stunts/IStuntBehavior.cs#L14) concept (a simple boolean given an [IMethodInvocation](../src/Stunts/Stunts/IMethodInvocation.cs)) and turns into a flexible argument matching strategy configured via the mock's setup operations. The responsibility for matching an invocation to a given setup is in [MockSetup.AppliesTo](https://github.com/moq/moq/blob/master/src/Moq/Moq.Sdk/MockSetup.cs#L32) in Moq.

The SDK also extends the *Stunts* code generation, automatically implementing [IMocked](../src/Moq.Sdk/IMocked.cs) on all generated classes. Its `Runtime` property exposes the [IMockRuntime](../src/Moq.Sdk/IMockRuntime.cs), which gives the mocking library author access to the key features of a mock described in [Mock Introspection](#mock-introspection).

The first step in getting your mocking API going is to provide a static factory method for your mocks. *Moq* itself provides this via the [Mock.Of](https://github.com/moq/moq/blob/master/src/Moq/Moq/contentFiles/cs/netstandard2.0/Mock.Overloads.cs) static method overloads, which follows the same approach as the [Stunt.Of](https://github.com/moq/moq/blob/master/docs/Stunts.md#stunt-factory) factory, and leverages the code generation provided by the Stunts and Moq SDKs to emit the "proxy" classes (a.k.a. *stunts*) into the test/app assembly itself.

You could create the following static class in your test project that references the `Moq.Sdk`:

```csharp
using System;
using System.Reflection;
using Moq.Sdk;

public static class Mocker
{
    [MockGenerator]
    public static T For<T>() 
        => (T)MockFactory.Default.CreateMock(typeof(Mocker).Assembly, typeof(T), new Type[0], new object[0]);
                                //CreateMock(mocksAssembly, baseType, implementedInterfaces, construtorArguments)
}
```

Marking a method with `[MockGenerator]` is all that's needed to trigger the code generation in the SDK whenever it's used. In the case above, we're using the default [MockFactory](https://github.com/moq/moq/blob/master/src/Moq/Moq.Sdk/MockFactory.cs) which looks up a matching type in the current assembly based on a default naming convention for mocks (i.e. for a `Mocker.For<IServiceProvider>`, it would attemt to locate a type named `Mocks.IServiceProviderMock`). 

In the case above, the resulting mock won't be very useful, since it doesn't have any built-in behaviors. The following are some of the commonly used behaviors you can easily add to a mock before returning it from the factory method:

1. [PropertyBehavior](https://github.com/moq/moq/blob/master/src/Moq/Moq.Sdk/PropertyBehavior.cs): provides an automatic "backing field" in the mock' `State` to implement property getter/setters. 

```csharp
    [MockGenerator]
    public static T For<T>() 
        => (T)MockFactory.Default.CreateMock(typeof(Mocker).Assembly, typeof(T), new Type[0], new object[0])
                         .AddBehavior(new PropertyBehavior());
```

2.  [EventBehavior](https://github.com/moq/moq/blob/master/src/Moq/Moq.Sdk/EventBehavior.cs): implements events so that they support adding/removing event handlers, as well as raising events using an [EventRaiser](https://github.com/moq/moq/blob/master/src/Moq/Moq.Sdk/EventRaiser.cs), like [Moq.Raise](https://github.com/moq/moq/blob/master/src/Moq/Moq/Raise.cs) class does:

```csharp
    [MockGenerator]
    public static T For<T>() 
        => (T)MockFactory.Default.CreateMock(typeof(Mocker).Assembly, typeof(T), new Type[0], new object[0])
                         .AddBehavior(new EventBehavior());

    ...
    using static Moq.Syntax;
    ...
    var changed = Mocker.For<INotifyPropertyChanged>();
    var property = "";
    mock.PropertyChanged += (sender, args) => property = args.PropertyName;

    // Raise the event
    mock.PropertyChanged += Raise<PropertyChangedEventHandler>(new PropertyChangedEventArgs("Mode"));

    Assert.Equal("Mode", property);
```

## Mock Introspection

All generated mocks implement `IMocked`, from which you can retrieve the mock introspection information via the `IMockRuntime Runtime { get; }` property. `MockRuntime.Get(object)` does the same for any mocked instance, and Moq exposes it as the `Sdk` extension property on its (higher-level) `IMock<T>`, as in `mock.Sdk`. The [IMockRuntime](../src/Moq.Sdk/IMockRuntime.cs) exposes:

* `Invocations`: a list of all invocations performed on the mock, which can be modified at will.
* `Setups`: an enumeration of the configured mock behaviors (sub-pipelines) within the overall behaviors of the stunt.
* `State`: a general purpose, concurrent-safe state bag associated with the mock.
* `Behaviors`: the overall stunt behavior pipeline (from `IStunt`).

Libraries can surface their own settings as extension properties over `IMockRuntime` backed by `State`, the way Moq provides `Name`, `Behavior`, `DefaultValue` and `CallBase` (i.e. `mock.Sdk.Name = "calculator"`).

Moq typed setups and the `Setup(() => ...)` syntax both return an `ISetup` whose `Sdk` property is the underlying `IMockSetup`, so extenders can add their own setup verbs on top of the same pipeline.

The introspection API has been heavily annotated with debugger hints so that when inspecting a mock in the debugger, the rendering is useful and easy to explore:

By default, the received invocations are shown:

![Mock Debugger](https://raw.githubusercontent.com/moq/moq/master/docs/img/MockSdk_Debugger.png)

Expanding the Invocations property shows a friendly rendering of what they were:

![Mock Debugging Invocations](https://raw.githubusercontent.com/moq/moq/master/docs/img/MockSdk_DebuggerInvocations.png)

Setups shows the configured mock behaviors for a given setup (or invocation matcher):

![Mock Debugging Setups](https://raw.githubusercontent.com/moq/moq/master/docs/img/MockSdk_DebuggerSetups.png)

State renders the state bag associated with the mock, which in this case holds the backing field for the assigned Mode property, the backing delegate for the subscribed event, among other things:

![Mock Debugging State](https://raw.githubusercontent.com/moq/moq/master/docs/img/MockSdk_DebuggerState.png)

Finally, the overall collection of stunt behaviors is also available:

![Mock Debugging Behaviors](https://raw.githubusercontent.com/moq/moq/master/docs/img/MockSdk_DebuggerBehaviors.png)