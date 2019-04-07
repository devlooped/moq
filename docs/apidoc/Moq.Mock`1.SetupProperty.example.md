---
uid: Moq.Mock`1.SetupProperty``1(System.Linq.Expressions.Expression{System.Func{`0,``0}})
example:
  - *content
---
If you have an interface with an `int` property `Value`, you might stub it using the following straightforward call:

```csharp
var mock = new Mock<IHaveValue>();
mock.SetupProperty(v => v.Value);
```

After the `SetupProperty` call has been issued, setting and retrieving the object value will behave as expected:

```csharp
IHaveValue v = mock.Object;

v.Value = 5;
Assert.Equal(5, v.Value);
```
