---
uid: Moq.Mock`1.Setup(System.Linq.Expressions.Expression{System.Action{`0}})
example:
  - *content
---
```csharp
var mock = new Mock<IProcessor>();
mock.Setup(x => x.Execute("ping"));
```
