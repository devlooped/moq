---
uid: Moq.Mock`1.Verify``1(System.Linq.Expressions.Expression{System.Func{`0,``0}},System.String)
example:
  - *content
---
This example assumes that the mock has been used, and later we want to verify that a given invocation with specific parameters was performed:

```csharp
var mock = new Mock<IWarehouse>();
// exercise mock
//...
// Will throw if the test code didn't call HasInventory.
mock.Verify(warehouse => warehouse.HasInventory(TALISKER, 50), "When filling orders, inventory has to be checked");
```
