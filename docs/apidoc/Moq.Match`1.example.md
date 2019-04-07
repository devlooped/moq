---
uid: Moq.Match`1
example:
  - *content
---
Creating a custom matcher is straightforward. You just need to create a method that returns a value from a call to <xref:Moq.Match.Create``1(System.Predicate{``0})> with your matching condition and an optional friendly render expression:

```csharp
public Order IsBigOrder()
{
	return Match.Create<Order>(
		o => o.GrandTotal &gt;= 5000, 
		/* a friendly expression to render on failures */
		() => IsBigOrder());
}
```

This method can be used in any mock setup invocation:

```csharp
mock.Setup(m => m.Submit(IsBigOrder()).Throws<UnauthorizedAccessException>();
```

At runtime, Moq knows that the return value was a matcher and
evaluates your predicate with the actual value passed into your predicate.

Another example might be a case where you want to match a lists of orders that contains a particular one. You might create matcher like the following:

```csharp
public static class Orders
{
	public static IEnumerable<Order> Contains(Order order)
	{
		return Match.Create<IEnumerable<Order>>(orders => orders.Contains(order));
	}
}
```

Now we can invoke this static method instead of an argument in an
invocation:

```csharp
var order = new Order { ... };
var mock = new Mock<IRepository<Order>>();

mock.Setup(x => x.Save(Orders.Contains(order)))
    .Throws<ArgumentException>();
```
