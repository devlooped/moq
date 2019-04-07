---
uid: Moq.It.IsNotIn``1(``0[])
example:
  - *content
---
The following example shows how to expect a method call with an integer argument of any value except 1, 2, or 3.

```csharp
mock.Setup(x => x.HasInventory(It.IsAny<string>(),
                               It.IsNotIn(1, 2, 3)))
    .Returns(false);
```
