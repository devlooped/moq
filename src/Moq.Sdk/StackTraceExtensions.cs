using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using Stunts;

namespace Moq.Sdk
{
    /// <summary>
    /// Extensions to get clean stack traces containing user code only.
    /// </summary>
    public static class StackTraceExtensions
    {
        /// <summary>
        /// Gets the stack trace scoped to the given invocation.
        /// </summary>
        public static string GetStackTrace(this IMethodInvocation invocation)
        {
            var allFrames = EnhancedStackTrace.Current();
            var invocationFrame = allFrames.FirstOrDefault(x => x.GetMethod() == invocation.MethodBase);
            //if (invocationFrame == null)

            // We know that the generated proxies live in the same assembly as the tests, so we use that 
            // to scope the stack trace from the current invocation method up to the top call (test method)
            var testFrame = allFrames.LastOrDefault(x => x.GetMethod().DeclaringType?.Assembly == invocation.MethodBase.DeclaringType.Assembly);

            var sb = new StringBuilder();
            // Always append if we didn't find the tip invocation frame
            var appendLine = invocationFrame == null;
            foreach (var frame in allFrames)
            {
                if (!appendLine && frame == invocationFrame)
                    appendLine = true;

                if (appendLine)
                {
                    sb.Append("\t");
                    var filePath = frame.GetFileName();
                    if (!string.IsNullOrEmpty(filePath))
                    {
                        sb.Append(EnhancedStackTrace.TryGetFullPath(filePath).Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar));
                        var lineNo = frame.GetFileLineNumber();
                        var colNo = frame.GetFileColumnNumber();
                        if (lineNo != 0 && colNo != 0)
                        {
                            sb.Append("(").Append(lineNo).Append(",").Append(colNo).Append("): ");
                        }
                    }

                    sb.Append("at ");
                    sb.AppendLine(frame.ToString());

                    if (frame == testFrame)
                        break;
                }
            }

            return sb.ToString();
        }
    }
}