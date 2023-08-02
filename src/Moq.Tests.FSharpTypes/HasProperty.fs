// Copyright (c) 2007, Clarius Consulting, Manas Technology Solutions, InSTEDD, and Contributors.
// All rights reserved. Licensed under the BSD 3-Clause License; see License.txt.

namespace Moq.Tests.FSharpTypes

open System;

type HasProperty() =
    let mutable property = new obj()
    abstract Property: obj with get, set
    default this.Property with get() = property and set(value: obj) = property <- value
