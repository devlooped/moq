---
uid: Moq.Mock.As``1
example:
  - *content
---
The following example creates a mock for the main interface
and later adds @System.IDisposable to it to verify
it's called by the consumer code:

```csharp
var mock = new Mock<IProcessor>();
mock.Setup(x => x.Execute("ping"));

// add IDisposable interface
var disposable = mock.As<IDisposable>();
disposable.Setup(d => d.Dispose()).Verifiable();
```
