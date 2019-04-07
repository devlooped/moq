---
uid: Moq.It.IsIn``1(System.Collections.Generic.IEnumerable{``0})
example:
  - *content
---
The following example shows how to expect a method call with an integer argument with value from a list.

```csharp
var values = new List<int> { 1, 2, 3 };

mock.Setup(x => x.HasInventory(It.IsAny<string>(), It.IsIn(values)))
    .Returns(false);
```
