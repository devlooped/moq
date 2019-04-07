---
uid: Moq.Mock`1.VerifyGet``1(System.Linq.Expressions.Expression{System.Func{`0,``0}})
example:
  - *content
---
This example assumes that the mock has been used, and later we want to verify that a given property was retrieved from it:

```csharp
var mock = new Mock<IWarehouse>();
// exercise mock
//...
// Will throw if the test code didn't retrieve the IsClosed property.
mock.VerifyGet(warehouse => warehouse.IsClosed);
```
