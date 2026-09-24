using System;
using System.Collections.Generic;
using System.Linq;
using Avatars;

public class RecordingBehavior : IAvatarBehavior
{
    public List<IMethodInvocation> Invocations { get; } = new List<IMethodInvocation>();

    public bool AppliesTo(IMethodInvocation invocation) => true;

    public IMethodReturn Execute(IMethodInvocation invocation, ExecuteHandler next)
    {
        Invocations.Add(invocation);
        return next.Invoke(invocation, next);
    }

    public override string ToString() => string.Join(Environment.NewLine, Invocations.Select(i => i.ToString()));
}