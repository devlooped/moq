---
uid: Moq.Mock.Get``1(``0)
example:
  - *content
---
The following example shows how to add a new setup to an object
instance which is not the original @Moq.Mock`1 but rather
the object associated with it:

```csharp
// Typed instance, not the mock, is retrieved from some test API.
HttpContextBase context = GetMockContext();

// context.Request is the typed object from the "real" API
// so in order to add a setup to it, we need to get
// the mock that "owns" it
Mock<HttpRequestBase> request = Mock.Get(context.Request);
mock.Setup(req => req.AppRelativeCurrentExecutionFilePath)
    .Returns(tempUrl);
```
