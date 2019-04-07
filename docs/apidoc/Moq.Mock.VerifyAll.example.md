---
uid: Moq.Mock.VerifyAll
example:
  - *content
---
This example sets up an expectation without marking it as verifiable. After
the mock is used, a `"VerifyAll()` call is issued on the mock
to ensure that all expectations are met:

```csharp
var mock = new Mock<IWarehouse>();
this.Setup(x => x.HasInventory(TALISKER, 50)).Returns(true);
...
// other test code
...
// Will throw if the test code has didn't call HasInventory, even
// that expectation was not marked as verifiable.
this.VerifyAll();
```
