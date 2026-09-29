# Moq

Moq stands in for a type with a mock, and configures how that mock answers member invocations.

## Language

**Mock**:
An object standing in for a type.
_Avoid_: stub, fake, double

**Invocation**:
One call to a member of a mock, together with the arguments it received.

**Setup**:
One configured member invocation on a mock.
_Avoid_: expectation

**Typed setup**:
A setup addressed by calling the member on the mock, or by `SetupRef`. Its handlers receive that member's arguments.
_Avoid_: generated setup, untyped setup

**Syntax setup**:
A setup addressed by `Syntax.Setup`. Its handlers receive the invocation's argument collection. The lambda may walk through mocks returned from other members.
_Avoid_: untyped setup, recursive setup
