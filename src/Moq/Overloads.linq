<Query Kind="Statements" />

// Generates ReturnsExtension.Overloads.cs and CallbackExtension.Overloads.cs
var dir = Path.GetDirectoryName(Util.CurrentQueryPath);

string TypeArgs(int count) => string.Join(", ", Enumerable.Range(1, count).Select(i => "T" + i));
string Args(int count) => string.Join(", ", Enumerable.Range(0, count).Select(i => $"(T{i + 1})args.GetValue({i})!"));

var returns = new StringBuilder();
foreach (var task in new[] { "Task", "ValueTask" })
{
	for (var i = 1; i <= 16; i++)
	{
		var result = task == "Task" ? $"Task.FromResult(handler({Args(i)}))" : $"new ValueTask<TResult>(handler({Args(i)}))";
		returns.Append($$"""
        /// <summary>
        /// Sets the result value for an async method to be calculated on every call 
        /// by the given <paramref name="handler"/>, which receives the invocation arguments.
        /// </summary>
        [OverloadResolutionPriority(2)]
        public static ISetup<Func<{{TypeArgs(i)}}, {{task}}<TResult>>, {{task}}<TResult>> Returns<{{TypeArgs(i)}}, TResult>(this ISetup<Func<{{TypeArgs(i)}}, {{task}}<TResult>>, {{task}}<TResult>> setup, Func<{{TypeArgs(i)}}, TResult> handler)
        {
            setup.SetReturnValue(args => {{result}});
            return setup;
        }


""");
	}
}

File.WriteAllText(Path.Combine(dir, "ReturnsExtension.Overloads.cs"), $$"""
using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Moq
{
    partial class ReturnsExtension
    {
{{returns.ToString().TrimEnd()}}
    }
}

""");

var callbacks = new StringBuilder();
for (var i = 1; i <= 16; i++)
{
	callbacks.Append($$"""
        /// <summary>
        /// Specifies a callback to invoke when the member is called, which receives the invocation arguments.
        /// </summary>
        public static ISetup<Func<{{TypeArgs(i)}}, TResult>, TResult> Callback<{{TypeArgs(i)}}, TResult>(this ISetup<Func<{{TypeArgs(i)}}, TResult>, TResult> setup, Action<{{TypeArgs(i)}}> callback)
        {
            setup.AddCallback(args => callback({{Args(i)}}));
            return setup;
        }


""");
}

File.WriteAllText(Path.Combine(dir, "CallbackExtension.Overloads.cs"), $$"""
using System;

namespace Moq
{
    partial class CallbackExtension
    {
{{callbacks.ToString().TrimEnd()}}
    }
}

""");
