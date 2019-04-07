---
uid: Moq.Mock`1.Raise(System.Action{`0},System.EventArgs)
example:
  - *content
---
The following example shows how to raise a @System.ComponentModel.INotifyPropertyChanged.PropertyChanged event:

```csharp
var mock = new Mock<IViewModel>();
mock.Raise(x => x.PropertyChanged -= null, new PropertyChangedEventArgs("Name"));
```

This example shows how to invoke an event with a custom event arguments
class in a view that will cause its corresponding presenter to
react by changing its state:

```csharp
var mockView = new Mock<IOrdersView>();
var presenter = new OrdersPresenter(mockView.Object);

// Check that the presenter has no selection by default
Assert.Null(presenter.SelectedOrder);

// Raise the event with a specific arguments data
mockView.Raise(v => v.SelectionChanged += null, new OrderEventArgs { Order = new Order("moq", 500) });

// Now the presenter reacted to the event, and we have a selected order
Assert.NotNull(presenter.SelectedOrder);
Assert.Equal("moq", presenter.SelectedOrder.ProductName);
```csharp
