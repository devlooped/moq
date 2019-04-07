---
uid: Moq.It.IsInRange``1(``0,``0,Moq.Range)
example:
  - *content
---
The following example shows how to expect a method call with an integer argument within the 0..100 range.

```csharp
mock.Setup(x => x.HasInventory(It.IsAny<string>(),
                               It.IsInRange(0, 100, Range.Inclusive)))
    .Returns(false);
```
