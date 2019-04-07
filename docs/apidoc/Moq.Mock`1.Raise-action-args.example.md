---
uid: Moq.Mock`1.Raise(System.Action{`0},System.Object[])
example:
  - *content
---
The following example shows how to raise a custom event that does not adhere to the standard @System.EventHandler:

```csharp
var mock = new Mock<IViewModel>();
mock.Raise(x => x.MyEvent -= null, "Name", bool, 25);
```
