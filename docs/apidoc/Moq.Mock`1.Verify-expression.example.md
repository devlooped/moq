---
uid: Moq.Mock`1.Verify(System.Linq.Expressions.Expression{System.Action{`0}})
example:
  - *content
---
This example assumes that the mock has been used, and later we want to verify that a given invocation with specific parameters was performed:

```csharp
var mock = new Mock<IProcessor>();
// exercise mock
//...
// Will throw if the test code didn't call Execute with a "ping" string argument.
mock.Verify(proc => proc.Execute("ping"));
```
