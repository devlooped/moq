---
uid: Moq.It.IsRegex(System.String,System.Text.RegularExpressions.RegexOptions)
example:
  - *content
---
The following example shows how to expect a call to a method where the string argument matches the given regular expression, in a case insensitive way:

```csharp
mock.Setup(x => x.Check(It.IsRegex("[a-z]+", RegexOptions.IgnoreCase))).Returns(1);
```
