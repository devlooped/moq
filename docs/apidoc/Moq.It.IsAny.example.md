---
uid: Moq.It.IsAny``1
example:
  - *content
---
```csharp
// Throws an exception for a call to Remove with any string value.
mock.Setup(x => x.Remove(It.IsAny<string>()))
    .Throws(new InvalidOperationException());
```
