---
uid: Moq.It.IsRegex(System.String)
example:
  - *content
---
The following example shows how to expect a call to a method where the string argument matches the given regular expression:

```csharp
mock.Setup(x => x.Check(It.IsRegex("[a-z]+"))).Returns(1);
```
