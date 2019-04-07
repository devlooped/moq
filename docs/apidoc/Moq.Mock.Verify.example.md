---
uid: Moq.Mock.Verify
example:
  - *content
---
This example sets up an expectation and marks it as verifiable. After
the mock is used, a `Verify()` call is issued on the mock
to ensure the method in the setup was invoked:

```csharp
var mock = new Mock<IWarehouse>();
this.Setup(x => x.HasInventory(TALISKER, 50)).Returns(true).Verifiable();
...
// other test code
...
// Will throw if the test code has didn't call HasInventory.
this.Verify();
```
