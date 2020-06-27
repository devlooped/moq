// Copyright (c) 2007, Clarius Consulting, Manas Technology Solutions, InSTEDD, and Contributors.
// All rights reserved. Licensed under the BSD 3-Clause License; see License.txt.

namespace Moq.Tests.FSharpTypes

open System;

type HasIndexer() =
    let mutable item = new obj()
    abstract Item: int -> obj with get, set
    default this.Item with get(index: int) = item and set (index: int) (value: obj) = item <- value
