---
uid: Moq.Mock`1.VerifySet(System.Action{`0})
example:
  - *content
---
This example assumes that the mock has been used, and later we want to verify that a given property was set on it:

```csharp
var mock = new Mock<IWarehouse>();
// exercise mock
//...
// Will throw if the test code didn't set the IsClosed property.
mock.VerifySet(warehouse => warehouse.IsClosed = true);
```csharp
