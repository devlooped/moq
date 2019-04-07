---
uid: Moq.Mock`1
example:
  - *content
---
The following example shows establishing setups with specific values
for method invocations:

```csharp
// Arrange
var order = new Order(TALISKER, 50);
var mock = new Mock<IWarehouse>();

mock.Setup(x => x.HasInventory(TALISKER, 50))
    .Returns(true);

// Act
order.Fill(mock.Object);

// Assert
Assert.True(order.IsFilled);
```

The following example shows how to use the @It class
to specify conditions for arguments instead of specific values:

```csharp
// Arrange
var order = new Order(TALISKER, 50);
var mock = new Mock<IWarehouse>();

// shows how to expect a value within a range
mock.Setup(x => x.HasInventory(It.IsAny<string>(), It.IsInRange(0, 100, Range.Inclusive)))
    .Returns(false);

// shows how to throw for unexpected calls.
mock.Setup(x => x.Remove(It.IsAny<string>(), It.IsAny<int>()))
    .Throws(new InvalidOperationException());

// Act
order.Fill(mock.Object);

// Assert
Assert.False(order.IsFilled);
```
