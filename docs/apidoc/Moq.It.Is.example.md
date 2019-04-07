---
uid: Moq.It.Is``1(System.Linq.Expressions.Expression{System.Func{``0,System.Boolean}})
example:
  - *content
---
This example shows how to return the value `1` whenever the argument to the `Do` method is an even number.

```csharp
mock.Setup(x => x.Do(It.Is<int>(i => i % 2 == 0)))
    .Returns(1);
```

This example shows how to throw an exception if the argument to the method is a negative number:

```csharp
mock.Setup(x =>; x.GetUser(It.Is<int>(i => i < 0)))
    .Throws(new ArgumentException());
```
