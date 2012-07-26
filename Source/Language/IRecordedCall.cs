using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using Moq.Proxy;

namespace Moq.Language
{
    /// <summary>
    /// This interface exposes information about one recorded method invocation.
    /// It can be used to perform assertions about the parameters or return value
    /// of a mocked method or property.
    /// </summary>
    public interface IRecordedCall
    {
        /// <summary>
        /// The Arguments that were passed to the method when it was invoked.
        /// </summary>
        ReadOnlyCollection<object> Arguments { get; }

        /// <summary>
        /// The MethodInfo of the method that was invoked
        /// </summary>
        MethodInfo Method { get; }

        /// <summary>
        /// The Returned value of the recorded method invocation
        /// </summary>
        object ReturnValue { get; }
    }
    
    internal class RecordedCall : IRecordedCall
    {
        private ReadOnlyCollection<object> mArguments;
        public ReadOnlyCollection<object> Arguments
        {
            get { return this.mArguments; }
        }

        private MethodInfo mMethod;
        public MethodInfo Method
        {
            get { return this.mMethod; }
        }

        private object mReturnValue;
        public object ReturnValue
        {
            get { return this.mReturnValue; }
        }

        internal RecordedCall(ICallContext call)
        {
            this.mArguments = new ReadOnlyCollection<object>(new List<object>(call.Arguments));
            this.mMethod = call.Method;
            this.mReturnValue = call.ReturnValue;
        }
    }
    
    /// <summary>
    /// Extends IRecordedCall allowing the call to be replayed to a delegate.
    /// </summary>
    public static class IRecordedCallExtensions
    {
        private static IEnumerable<IRecordedCall> VerifyAndInvoke(this IEnumerable<IRecordedCall> calls, Delegate callback, Times times)
        {
            int timesExecuted = 0;
            var callEnumerator = calls.GetEnumerator();
            bool enumHasNext = callEnumerator.MoveNext();

            Predicate<int> couldFinish;
            if (times == default(Times))
                if (!enumHasNext)
                    throw new ArgumentException("Expected at least one recorded method call remaining, but there were none.");
                else
                    couldFinish = (i) => !enumHasNext;

            else
                couldFinish = times.Verify;
            
            while (!couldFinish(timesExecuted))
            {
                if(!enumHasNext)
                    ThrowUnexpectedCallEnd(times, calls.Count(), timesExecuted);

                var call = callEnumerator.Current;

                call.VerifyAndInvoke(callback);

                timesExecuted++;
                enumHasNext = callEnumerator.MoveNext();
            }

            //support Between
            while (couldFinish(timesExecuted + 1))
            {
                if (!enumHasNext)
                    return Enumerable.Empty<IRecordedCall>();   //return empty list

                var call = callEnumerator.Current;
                try
                {
                    call.VerifyAndInvoke(callback);
                }
                catch
                {
                    //ignore if we failed an assert in this one, it's ok, just go to the next
                    break;
                }

                timesExecuted++;
                enumHasNext = callEnumerator.MoveNext();
            }

            if(enumHasNext)
            {
                return EnumerateRemaining(callEnumerator);
            }

            return Enumerable.Empty<IRecordedCall>();
        }

        private static IEnumerable<IRecordedCall> EnumerateRemaining(IEnumerator<IRecordedCall> enumerator)
        {
            do
            {
                yield return enumerator.Current;
            } while (enumerator.MoveNext());
        }

        private static IRecordedCall VerifyAndInvoke(this IRecordedCall call, Delegate callback)
        {
            var replayParams = callback.Method.GetParameters();
            var actualParams = call.Method.GetParameters();

            if (!CheckParams(actualParams, replayParams))
                ThrowParameterMismatch(actualParams, replayParams);

            //do verify
            callback.InvokePreserveStack(call.Arguments.ToArray());

            return call;
        }

        private static bool CheckParams(ParameterInfo[] actual, ParameterInfo[] replay)
        {
            if (actual.Length == replay.Length)
            {
                for (int i = 0; i < actual.Length; i++)
                {
                    Type replayType = replay[i].ParameterType;
                    Type actualType = actual[i].ParameterType;
                    if (actualType.IsByRef) actualType = actualType.GetElementType();

                    if (!replayType.IsAssignableFrom(actualType))
                        return false;
                }
                return true;
            }
            return false;
        }
        
        private static void ThrowUnexpectedCallEnd(Times times, int count, int timesExecuted)
        {
            throw new ArgumentException(times.GetExceptionMessage(
                        string.Format("The assertion chain ran out of recorded method calls, there were only {0} call(s) satisfying the verify requirements.", timesExecuted),
                        "", count));
        }

        private static void ThrowParameterMismatch(ParameterInfo[] expected, ParameterInfo[] actual)
        {
            throw new ArgumentException(string.Format(
                CultureInfo.CurrentCulture,
                "Invalid callback. Verify on method with parameters ({0}) cannot replay with parameters ({1}).",
                string.Join(",", expected.Select(p => p.ParameterType.Name).ToArray()),
                string.Join(",", actual.Select(p => p.ParameterType.Name).ToArray())
            ));
        }

        #region Enumerable Assertions

        /// <summary>
        /// Asserts on the actual values that were passed into a mocked method.  The callback receives
        /// these values and is expected to perform the assertions.<br/>
        /// By default each call to Assert performs an assertion on one recorded invocation of the
        /// mocked method, but this can be changed with the 'times' parameter.
        /// </summary>
        /// <param name="calls">The values that were passed into the mocked method</param>
        /// <param name="action">The callback which receives the recorded objects and performs the assertions.</param>
        /// <param name="times">The number of recorded invocations for which this callback should be invoked.</param>
        /// <typeparam name="T1">The type of the method's 1st parameter</typeparam>
        /// <typeparam name="T2">The type of the method's 2nd parameter</typeparam>
        /// <returns>This object to allow chaining</returns>
        public static IEnumerable<IRecordedCall> Replay<T1, T2>(this IEnumerable<IRecordedCall> calls, Action<T1, T2> action, Times times = default(Times))
        {
            return calls.VerifyAndInvoke(action, times);
        }

        /// <summary>
        /// Asserts on the actual values that were passed into a mocked method.  The callback receives
        /// these values and is expected to perform the assertions.<br/>
        /// By default each call to Assert performs an assertion on one recorded invocation of the
        /// mocked method, but this can be changed with the 'times' parameter.
        /// </summary>
        /// <param name="action">The callback which receives the recorded objects and performs the assertions.</param>
        /// <param name="times">The number of recorded invocations for which this callback should be invoked.</param>
        /// <param name="calls">The values that were passed into the mocked method</param>
        /// <typeparam name="T1">The type of the method's 1st parameter</typeparam>
        /// <typeparam name="T2">The type of the method's 2nd parameter</typeparam>
        /// <typeparam name="T3">The type of the method's 3rd parameter</typeparam>
        /// <returns>This object to allow chaining</returns>
        public static IEnumerable<IRecordedCall> Replay<T1, T2, T3>(this IEnumerable<IRecordedCall> calls, Action<T1, T2, T3> action, Times times = default(Times))
        {
            return calls.VerifyAndInvoke(action, times);
        }

        /// <summary>
        /// Asserts on the actual values that were passed into a mocked method.  The callback receives
        /// these values and is expected to perform the assertions.<br/>
        /// By default each call to Assert performs an assertion on one recorded invocation of the
        /// mocked method, but this can be changed with the 'times' parameter.
        /// </summary>
        /// <param name="calls">The values that were passed into the mocked method</param>
        /// <param name="action">The callback which receives the recorded objects and performs the assertions.</param>
        /// <param name="times">The number of recorded invocations for which this callback should be invoked.</param>
        /// <typeparam name="T1">The type of the method's 1st parameter</typeparam>
        /// <typeparam name="T2">The type of the method's 2nd parameter</typeparam>
        /// <typeparam name="T3">The type of the method's 3rd parameter</typeparam>
        /// <typeparam name="T4">The type of the method's 4th parameter</typeparam>
        /// <returns>This object to allow chaining</returns>
        public static IEnumerable<IRecordedCall> Replay<T1, T2, T3, T4>(this IEnumerable<IRecordedCall> calls, Action<T1, T2, T3, T4> action, Times times = default(Times))
        {
            return calls.VerifyAndInvoke(action, times);
        }

        /// <summary>
        /// Asserts on the actual values that were passed into a mocked method.  The callback receives
        /// these values and is expected to perform the assertions.<br/>
        /// By default each call to Assert performs an assertion on one recorded invocation of the
        /// mocked method, but this can be changed with the 'times' parameter.
        /// </summary>
        /// <param name="action">The callback which receives the recorded objects and performs the assertions.</param>
        /// <param name="times">The number of recorded invocations for which this callback should be invoked.</param>
        /// <param name="calls">The values that were passed into the mocked method</param>
        /// <typeparam name="T1">The type of the method's 1st parameter</typeparam>
        /// <typeparam name="T2">The type of the method's 2nd parameter</typeparam>
        /// <typeparam name="T3">The type of the method's 3rd parameter</typeparam>
        /// <typeparam name="T4">The type of the method's 4th parameter</typeparam>
        /// <typeparam name="T5">The type of the method's 5th parameter</typeparam>
        /// <returns>This object to allow chaining</returns>
        public static IEnumerable<IRecordedCall> Replay<T1, T2, T3, T4, T5>(this IEnumerable<IRecordedCall> calls, Action<T1, T2, T3, T4, T5> action, Times times = default(Times))
        {
            return calls.VerifyAndInvoke(action, times);
        }

        /// <summary>
        /// Asserts on the actual values that were passed into a mocked method.  The callback receives
        /// these values and is expected to perform the assertions.<br/>
        /// By default each call to Assert performs an assertion on one recorded invocation of the
        /// mocked method, but this can be changed with the 'times' parameter.
        /// </summary>
        /// <param name="action">The callback which receives the recorded objects and performs the assertions.</param>
        /// <param name="times">The number of recorded invocations for which this callback should be invoked.</param>
        /// <param name="calls">The values that were passed into the mocked method</param>
        /// <typeparam name="T1">The type of the method's 1st parameter</typeparam>
        /// <typeparam name="T2">The type of the method's 2nd parameter</typeparam>
        /// <typeparam name="T3">The type of the method's 3rd parameter</typeparam>
        /// <typeparam name="T4">The type of the method's 4th parameter</typeparam>
        /// <typeparam name="T5">The type of the method's 5th parameter</typeparam>
        /// <typeparam name="T6">The type of the method's 6th parameter</typeparam>
        /// <returns>This object to allow chaining</returns>
        public static IEnumerable<IRecordedCall> Replay<T1, T2, T3, T4, T5, T6>(this IEnumerable<IRecordedCall> calls, Action<T1, T2, T3, T4, T5, T6> action, Times times = default(Times))
        {
            return calls.VerifyAndInvoke(action, times);
        }

        /// <summary>
        /// Asserts on the actual values that were passed into a mocked method.  The callback receives
        /// these values and is expected to perform the assertions.<br/>
        /// By default each call to Assert performs an assertion on one recorded invocation of the
        /// mocked method, but this can be changed with the 'times' parameter.
        /// </summary>
        /// <param name="action">The callback which receives the recorded objects and performs the assertions.</param>
        /// <param name="times">The number of recorded invocations for which this callback should be invoked.</param>
        /// <param name="calls">The values that were passed into the mocked method</param>
        /// <typeparam name="T1">The type of the method's 1st parameter</typeparam>
        /// <typeparam name="T2">The type of the method's 2nd parameter</typeparam>
        /// <typeparam name="T3">The type of the method's 3rd parameter</typeparam>
        /// <typeparam name="T4">The type of the method's 4th parameter</typeparam>
        /// <typeparam name="T5">The type of the method's 5th parameter</typeparam>
        /// <typeparam name="T6">The type of the method's 6th parameter</typeparam>
        /// <typeparam name="T7">The type of the method's 7th parameter</typeparam>
        /// <returns>This object to allow chaining</returns>
        public static IEnumerable<IRecordedCall> Replay<T1, T2, T3, T4, T5, T6, T7>(this IEnumerable<IRecordedCall> calls, Action<T1, T2, T3, T4, T5, T6, T7> action, Times times = default(Times))
        {
            return calls.VerifyAndInvoke(action, times);
        }

        /// <summary>
        /// Asserts on the actual values that were passed into a mocked method.  The callback receives
        /// these values and is expected to perform the assertions.<br/>
        /// By default each call to Assert performs an assertion on one recorded invocation of the
        /// mocked method, but this can be changed with the 'times' parameter.
        /// </summary>
        /// <param name="action">The callback which receives the recorded objects and performs the assertions.</param>
        /// <param name="times">The number of recorded invocations for which this callback should be invoked.</param>
        /// <param name="calls">The values that were passed into the mocked method</param>
        /// <typeparam name="T1">The type of the method's 1st parameter</typeparam>
        /// <typeparam name="T2">The type of the method's 2nd parameter</typeparam>
        /// <typeparam name="T3">The type of the method's 3rd parameter</typeparam>
        /// <typeparam name="T4">The type of the method's 4th parameter</typeparam>
        /// <typeparam name="T5">The type of the method's 5th parameter</typeparam>
        /// <typeparam name="T6">The type of the method's 6th parameter</typeparam>
        /// <typeparam name="T7">The type of the method's 7th parameter</typeparam>
        /// <typeparam name="T8">The type of the method's 8th parameter</typeparam>
        /// <returns>This object to allow chaining</returns>
        public static IEnumerable<IRecordedCall> Replay<T1, T2, T3, T4, T5, T6, T7, T8>(this IEnumerable<IRecordedCall> calls, Action<T1, T2, T3, T4, T5, T6, T7, T8> action, Times times = default(Times))
        {
            return calls.VerifyAndInvoke(action, times);
        }

        /// <summary>
        /// Asserts on the actual values that were passed into a mocked method.  The callback receives
        /// these values and is expected to perform the assertions.<br/>
        /// By default each call to Assert performs an assertion on one recorded invocation of the
        /// mocked method, but this can be changed with the 'times' parameter.
        /// </summary>
        /// <param name="action">The callback which receives the recorded objects and performs the assertions.</param>
        /// <param name="times">The number of recorded invocations for which this callback should be invoked.</param>
        /// <param name="calls">The values that were passed into the mocked method</param>
        /// <typeparam name="T1">The type of the method's 1st parameter</typeparam>
        /// <typeparam name="T2">The type of the method's 2nd parameter</typeparam>
        /// <typeparam name="T3">The type of the method's 3rd parameter</typeparam>
        /// <typeparam name="T4">The type of the method's 4th parameter</typeparam>
        /// <typeparam name="T5">The type of the method's 5th parameter</typeparam>
        /// <typeparam name="T6">The type of the method's 6th parameter</typeparam>
        /// <typeparam name="T7">The type of the method's 7th parameter</typeparam>
        /// <typeparam name="T8">The type of the method's 8th parameter</typeparam>
        /// <typeparam name="T9">The type of the method's 9th parameter</typeparam>
        /// <returns>This object to allow chaining</returns>
        public static IEnumerable<IRecordedCall> Replay<T1, T2, T3, T4, T5, T6, T7, T8, T9>(this IEnumerable<IRecordedCall> calls, Action<T1, T2, T3, T4, T5, T6, T7, T8, T9> action, Times times = default(Times))
        {
            return calls.VerifyAndInvoke(action, times);
        }

        /// <summary>
        /// Asserts on the actual values that were passed into a mocked method.  The callback receives
        /// these values and is expected to perform the assertions.<br/>
        /// By default each call to Assert performs an assertion on one recorded invocation of the
        /// mocked method, but this can be changed with the 'times' parameter.
        /// </summary>
        /// <param name="action">The callback which receives the recorded objects and performs the assertions.</param>
        /// <param name="times">The number of recorded invocations for which this callback should be invoked.</param>
        /// <param name="calls">The values that were passed into the mocked method</param>
        /// <typeparam name="T1">The type of the method's 1st parameter</typeparam>
        /// <typeparam name="T2">The type of the method's 2nd parameter</typeparam>
        /// <typeparam name="T3">The type of the method's 3rd parameter</typeparam>
        /// <typeparam name="T4">The type of the method's 4th parameter</typeparam>
        /// <typeparam name="T5">The type of the method's 5th parameter</typeparam>
        /// <typeparam name="T6">The type of the method's 6th parameter</typeparam>
        /// <typeparam name="T7">The type of the method's 7th parameter</typeparam>
        /// <typeparam name="T8">The type of the method's 8th parameter</typeparam>
        /// <typeparam name="T9">The type of the method's 9th parameter</typeparam>
        /// <typeparam name="T10">The type of the method's 10th parameter</typeparam>
        /// <returns>This object to allow chaining</returns>
        public static IEnumerable<IRecordedCall> Replay<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(this IEnumerable<IRecordedCall> calls, Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10> action, Times times = default(Times))
        {
            return calls.VerifyAndInvoke(action, times);
        }

        /// <summary>
        /// Asserts on the actual values that were passed into a mocked method.  The callback receives
        /// these values and is expected to perform the assertions.<br/>
        /// By default each call to Assert performs an assertion on one recorded invocation of the
        /// mocked method, but this can be changed with the 'times' parameter.
        /// </summary>
        /// <param name="action">The callback which receives the recorded objects and performs the assertions.</param>
        /// <param name="times">The number of recorded invocations for which this callback should be invoked.</param>
        /// <param name="calls">The values that were passed into the mocked method</param>
        /// <typeparam name="T1">The type of the method's 1st parameter</typeparam>
        /// <typeparam name="T2">The type of the method's 2nd parameter</typeparam>
        /// <typeparam name="T3">The type of the method's 3rd parameter</typeparam>
        /// <typeparam name="T4">The type of the method's 4th parameter</typeparam>
        /// <typeparam name="T5">The type of the method's 5th parameter</typeparam>
        /// <typeparam name="T6">The type of the method's 6th parameter</typeparam>
        /// <typeparam name="T7">The type of the method's 7th parameter</typeparam>
        /// <typeparam name="T8">The type of the method's 8th parameter</typeparam>
        /// <typeparam name="T9">The type of the method's 9th parameter</typeparam>
        /// <typeparam name="T10">The type of the method's 10th parameter</typeparam>
        /// <typeparam name="T11">The type of the method's 11th parameter</typeparam>
        /// <returns>This object to allow chaining</returns>
        public static IEnumerable<IRecordedCall> Replay<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(this IEnumerable<IRecordedCall> calls, Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> action, Times times = default(Times))
        {
            return calls.VerifyAndInvoke(action, times);
        }

        /// <summary>
        /// Asserts on the actual values that were passed into a mocked method.  The callback receives
        /// these values and is expected to perform the assertions.<br/>
        /// By default each call to Assert performs an assertion on one recorded invocation of the
        /// mocked method, but this can be changed with the 'times' parameter.
        /// </summary>
        /// <param name="action">The callback which receives the recorded objects and performs the assertions.</param>
        /// <param name="times">The number of recorded invocations for which this callback should be invoked.</param>
        /// <param name="calls">The values that were passed into the mocked method</param>
        /// <typeparam name="T1">The type of the method's 1st parameter</typeparam>
        /// <typeparam name="T2">The type of the method's 2nd parameter</typeparam>
        /// <typeparam name="T3">The type of the method's 3rd parameter</typeparam>
        /// <typeparam name="T4">The type of the method's 4th parameter</typeparam>
        /// <typeparam name="T5">The type of the method's 5th parameter</typeparam>
        /// <typeparam name="T6">The type of the method's 6th parameter</typeparam>
        /// <typeparam name="T7">The type of the method's 7th parameter</typeparam>
        /// <typeparam name="T8">The type of the method's 8th parameter</typeparam>
        /// <typeparam name="T9">The type of the method's 9th parameter</typeparam>
        /// <typeparam name="T10">The type of the method's 10th parameter</typeparam>
        /// <typeparam name="T11">The type of the method's 11th parameter</typeparam>
        /// <typeparam name="T12">The type of the method's 12th parameter</typeparam>
        /// <returns>This object to allow chaining</returns>
        public static IEnumerable<IRecordedCall> Replay<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(this IEnumerable<IRecordedCall> calls, Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> action, Times times = default(Times))
        {
            return calls.VerifyAndInvoke(action, times);
        }

        /// <summary>
        /// Asserts on the actual values that were passed into a mocked method.  The callback receives
        /// these values and is expected to perform the assertions.<br/>
        /// By default each call to Assert performs an assertion on one recorded invocation of the
        /// mocked method, but this can be changed with the 'times' parameter.
        /// </summary>
        /// <param name="action">The callback which receives the recorded objects and performs the assertions.</param>
        /// <param name="times">The number of recorded invocations for which this callback should be invoked.</param>
        /// <param name="calls">The values that were passed into the mocked method</param>
        /// <typeparam name="T1">The type of the method's 1st parameter</typeparam>
        /// <typeparam name="T2">The type of the method's 2nd parameter</typeparam>
        /// <typeparam name="T3">The type of the method's 3rd parameter</typeparam>
        /// <typeparam name="T4">The type of the method's 4th parameter</typeparam>
        /// <typeparam name="T5">The type of the method's 5th parameter</typeparam>
        /// <typeparam name="T6">The type of the method's 6th parameter</typeparam>
        /// <typeparam name="T7">The type of the method's 7th parameter</typeparam>
        /// <typeparam name="T8">The type of the method's 8th parameter</typeparam>
        /// <typeparam name="T9">The type of the method's 9th parameter</typeparam>
        /// <typeparam name="T10">The type of the method's 10th parameter</typeparam>
        /// <typeparam name="T11">The type of the method's 11th parameter</typeparam>
        /// <typeparam name="T12">The type of the method's 12th parameter</typeparam>
        /// <typeparam name="T13">The type of the method's 13th parameter</typeparam>
        /// <returns>This object to allow chaining</returns>
        public static IEnumerable<IRecordedCall> Replay<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(this IEnumerable<IRecordedCall> calls, Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13> action, Times times = default(Times))
        {
            return calls.VerifyAndInvoke(action, times);
        }

        /// <summary>
        /// Asserts on the actual values that were passed into a mocked method.  The callback receives
        /// these values and is expected to perform the assertions.<br/>
        /// By default each call to Assert performs an assertion on one recorded invocation of the
        /// mocked method, but this can be changed with the 'times' parameter.
        /// </summary>
        /// <param name="action">The callback which receives the recorded objects and performs the assertions.</param>
        /// <param name="times">The number of recorded invocations for which this callback should be invoked.</param>
        /// <param name="calls">The values that were passed into the mocked method</param>
        /// <typeparam name="T1">The type of the method's 1st parameter</typeparam>
        /// <typeparam name="T2">The type of the method's 2nd parameter</typeparam>
        /// <typeparam name="T3">The type of the method's 3rd parameter</typeparam>
        /// <typeparam name="T4">The type of the method's 4th parameter</typeparam>
        /// <typeparam name="T5">The type of the method's 5th parameter</typeparam>
        /// <typeparam name="T6">The type of the method's 6th parameter</typeparam>
        /// <typeparam name="T7">The type of the method's 7th parameter</typeparam>
        /// <typeparam name="T8">The type of the method's 8th parameter</typeparam>
        /// <typeparam name="T9">The type of the method's 9th parameter</typeparam>
        /// <typeparam name="T10">The type of the method's 10th parameter</typeparam>
        /// <typeparam name="T11">The type of the method's 11th parameter</typeparam>
        /// <typeparam name="T12">The type of the method's 12th parameter</typeparam>
        /// <typeparam name="T13">The type of the method's 13th parameter</typeparam>
        /// <typeparam name="T14">The type of the method's 14th parameter</typeparam>
        /// <returns>This object to allow chaining</returns>
        public static IEnumerable<IRecordedCall> Replay<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(this IEnumerable<IRecordedCall> calls, Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14> action, Times times = default(Times))
        {
            return calls.VerifyAndInvoke(action, times);
        }

        /// <summary>
        /// Asserts on the actual values that were passed into a mocked method.  The callback receives
        /// these values and is expected to perform the assertions.<br/>
        /// By default each call to Assert performs an assertion on one recorded invocation of the
        /// mocked method, but this can be changed with the 'times' parameter.
        /// </summary>
        /// <param name="action">The callback which receives the recorded objects and performs the assertions.</param>
        /// <param name="times">The number of recorded invocations for which this callback should be invoked.</param>
        /// <param name="calls">The values that were passed into the mocked method</param>
        /// <typeparam name="T1">The type of the method's 1st parameter</typeparam>
        /// <typeparam name="T2">The type of the method's 2nd parameter</typeparam>
        /// <typeparam name="T3">The type of the method's 3rd parameter</typeparam>
        /// <typeparam name="T4">The type of the method's 4th parameter</typeparam>
        /// <typeparam name="T5">The type of the method's 5th parameter</typeparam>
        /// <typeparam name="T6">The type of the method's 6th parameter</typeparam>
        /// <typeparam name="T7">The type of the method's 7th parameter</typeparam>
        /// <typeparam name="T8">The type of the method's 8th parameter</typeparam>
        /// <typeparam name="T9">The type of the method's 9th parameter</typeparam>
        /// <typeparam name="T10">The type of the method's 10th parameter</typeparam>
        /// <typeparam name="T11">The type of the method's 11th parameter</typeparam>
        /// <typeparam name="T12">The type of the method's 12th parameter</typeparam>
        /// <typeparam name="T13">The type of the method's 13th parameter</typeparam>
        /// <typeparam name="T14">The type of the method's 14th parameter</typeparam>
        /// <typeparam name="T15">The type of the method's 15th parameter</typeparam>
        /// <returns>This object to allow chaining</returns>
        public static IEnumerable<IRecordedCall> Replay<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(this IEnumerable<IRecordedCall> calls, Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15> action, Times times = default(Times))
        {
            return calls.VerifyAndInvoke(action, times);
        }

        /// <summary>
        /// Asserts on the actual values that were passed into a mocked method.  The callback receives
        /// these values and is expected to perform the assertions.<br/>
        /// By default each call to Assert performs an assertion on one recorded invocation of the
        /// mocked method, but this can be changed with the 'times' parameter.
        /// </summary>
        /// <param name="action">The callback which receives the recorded objects and performs the assertions.</param>
        /// <param name="times">The number of recorded invocations for which this callback should be invoked.</param>
        /// <param name="calls">The values that were passed into the mocked method</param>
        /// <typeparam name="T1">The type of the method's 1st parameter</typeparam>
        /// <typeparam name="T2">The type of the method's 2nd parameter</typeparam>
        /// <typeparam name="T3">The type of the method's 3rd parameter</typeparam>
        /// <typeparam name="T4">The type of the method's 4th parameter</typeparam>
        /// <typeparam name="T5">The type of the method's 5th parameter</typeparam>
        /// <typeparam name="T6">The type of the method's 6th parameter</typeparam>
        /// <typeparam name="T7">The type of the method's 7th parameter</typeparam>
        /// <typeparam name="T8">The type of the method's 8th parameter</typeparam>
        /// <typeparam name="T9">The type of the method's 9th parameter</typeparam>
        /// <typeparam name="T10">The type of the method's 10th parameter</typeparam>
        /// <typeparam name="T11">The type of the method's 11th parameter</typeparam>
        /// <typeparam name="T12">The type of the method's 12th parameter</typeparam>
        /// <typeparam name="T13">The type of the method's 13th parameter</typeparam>
        /// <typeparam name="T14">The type of the method's 14th parameter</typeparam>
        /// <typeparam name="T15">The type of the method's 15th parameter</typeparam>
        /// <typeparam name="T16">The type of the method's 16th parameter</typeparam>
        /// <returns>This object to allow chaining</returns>
        public static IEnumerable<IRecordedCall> Replay<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16>(this IEnumerable<IRecordedCall> calls, Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16> action, Times times = default(Times))
        {
            return calls.VerifyAndInvoke(action, times);
        }

        /// <summary>
        /// Asserts on the actual values that were passed into a mocked method.  The callback receives
        /// these values and is expected to perform the assertions.<br/>
        /// By default each call to Assert performs an assertion on one recorded invocation of the
        /// mocked method, but this can be changed with the 'times' parameter.
        /// </summary>
        /// <param name="action">The callback which receives the recorded objects and performs the assertions.</param>
        /// <param name="times">The number of recorded invocations for which this callback should be invoked.</param>
        /// <param name="calls">The values that were passed into the mocked method</param>
        /// <returns>This object to allow chaining</returns>
        public static IEnumerable<IRecordedCall> Replay(this IEnumerable<IRecordedCall> calls, Action action, Times times = default(Times))
        {
            return calls.VerifyAndInvoke(action, times);
        }

        /// <summary>
        /// Asserts on the actual values that were passed into a mocked method.  The callback receives
        /// these values and is expected to perform the assertions.<br/>
        /// By default each call to Assert performs an assertion on one recorded invocation of the
        /// mocked method, but this can be changed with the 'times' parameter.
        /// </summary>
        /// <param name="action">The callback which receives the recorded objects and performs the assertions.</param>
        /// <param name="times">The number of recorded invocations for which this callback should be invoked.</param>
        /// <param name="calls">The values that were passed into the mocked method</param>
        /// <typeparam name="T">The type of the method's only parameter</typeparam>
        /// <returns>This object to allow chaining</returns>
        public static IEnumerable<IRecordedCall> Replay<T>(this IEnumerable<IRecordedCall> calls, Action<T> action, Times times = default(Times))
        {
            return calls.VerifyAndInvoke(action, times);
        }

        #endregion

        #region Individual Assertions


        /// <summary>
        /// Asserts on the actual values that were passed into a mocked method.  The callback receives
        /// these values and is expected to perform the assertions.
        /// </summary>
        /// <param name="action">The callback which receives the recorded objects and performs the assertions.</param>
        /// <param name="call">The values that were passed into the mocked method</param>
        /// <typeparam name="T1">The type of the method's 1st parameter</typeparam>
        /// <typeparam name="T2">The type of the method's 2nd parameter</typeparam>
        /// <returns>This object to allow chaining</returns>
        public static IRecordedCall Replay<T1, T2>(this IRecordedCall call, Action<T1, T2> action)
        {
            return call.VerifyAndInvoke(action);
        }

        /// <summary>
        /// Asserts on the actual values that were passed into a mocked method.  The callback receives
        /// these values and is expected to perform the assertions.
        /// </summary>
        /// <param name="action">The callback which receives the recorded objects and performs the assertions.</param>
        /// <param name="call">The values that were passed into the mocked method</param>
        /// <typeparam name="T1">The type of the method's 1st parameter</typeparam>
        /// <typeparam name="T2">The type of the method's 2nd parameter</typeparam>
        /// <typeparam name="T3">The type of the method's 3rd parameter</typeparam>
        /// <returns>This object to allow chaining</returns>
        public static IRecordedCall Replay<T1, T2, T3>(this IRecordedCall call, Action<T1, T2, T3> action)
        {
            return call.VerifyAndInvoke(action);
        }

        /// <summary>
        /// Asserts on the actual values that were passed into a mocked method.  The callback receives
        /// these values and is expected to perform the assertions.
        /// </summary>
        /// <param name="action">The callback which receives the recorded objects and performs the assertions.</param>
        /// <param name="call">The values that were passed into the mocked method</param>
        /// <typeparam name="T1">The type of the method's 1st parameter</typeparam>
        /// <typeparam name="T2">The type of the method's 2nd parameter</typeparam>
        /// <typeparam name="T3">The type of the method's 3rd parameter</typeparam>
        /// <typeparam name="T4">The type of the method's 4th parameter</typeparam>
        /// <returns>This object to allow chaining</returns>
        public static IRecordedCall Replay<T1, T2, T3, T4>(this IRecordedCall call, Action<T1, T2, T3, T4> action)
        {
            return call.VerifyAndInvoke(action);
        }

        /// <summary>
        /// Asserts on the actual values that were passed into a mocked method.  The callback receives
        /// these values and is expected to perform the assertions.
        /// </summary>
        /// <param name="action">The callback which receives the recorded objects and performs the assertions.</param>
        /// <param name="call">The values that were passed into the mocked method</param>
        /// <typeparam name="T1">The type of the method's 1st parameter</typeparam>
        /// <typeparam name="T2">The type of the method's 2nd parameter</typeparam>
        /// <typeparam name="T3">The type of the method's 3rd parameter</typeparam>
        /// <typeparam name="T4">The type of the method's 4th parameter</typeparam>
        /// <typeparam name="T5">The type of the method's 5th parameter</typeparam>
        /// <returns>This object to allow chaining</returns>
        public static IRecordedCall Replay<T1, T2, T3, T4, T5>(this IRecordedCall call, Action<T1, T2, T3, T4, T5> action)
        {
            return call.VerifyAndInvoke(action);
        }

        /// <summary>
        /// Asserts on the actual values that were passed into a mocked method.  The callback receives
        /// these values and is expected to perform the assertions.
        /// </summary>
        /// <param name="action">The callback which receives the recorded objects and performs the assertions.</param>
        /// <param name="call">The values that were passed into the mocked method</param>
        /// <typeparam name="T1">The type of the method's 1st parameter</typeparam>
        /// <typeparam name="T2">The type of the method's 2nd parameter</typeparam>
        /// <typeparam name="T3">The type of the method's 3rd parameter</typeparam>
        /// <typeparam name="T4">The type of the method's 4th parameter</typeparam>
        /// <typeparam name="T5">The type of the method's 5th parameter</typeparam>
        /// <typeparam name="T6">The type of the method's 6th parameter</typeparam>
        /// <returns>This object to allow chaining</returns>
        public static IRecordedCall Replay<T1, T2, T3, T4, T5, T6>(this IRecordedCall call, Action<T1, T2, T3, T4, T5, T6> action)
        {
            return call.VerifyAndInvoke(action);
        }

        /// <summary>
        /// Asserts on the actual values that were passed into a mocked method.  The callback receives
        /// these values and is expected to perform the assertions.
        /// </summary>
        /// <param name="action">The callback which receives the recorded objects and performs the assertions.</param>
        /// <param name="call">The values that were passed into the mocked method</param>
        /// <typeparam name="T1">The type of the method's 1st parameter</typeparam>
        /// <typeparam name="T2">The type of the method's 2nd parameter</typeparam>
        /// <typeparam name="T3">The type of the method's 3rd parameter</typeparam>
        /// <typeparam name="T4">The type of the method's 4th parameter</typeparam>
        /// <typeparam name="T5">The type of the method's 5th parameter</typeparam>
        /// <typeparam name="T6">The type of the method's 6th parameter</typeparam>
        /// <typeparam name="T7">The type of the method's 7th parameter</typeparam>
        /// <returns>This object to allow chaining</returns>
        public static IRecordedCall Replay<T1, T2, T3, T4, T5, T6, T7>(this IRecordedCall call, Action<T1, T2, T3, T4, T5, T6, T7> action)
        {
            return call.VerifyAndInvoke(action);
        }

        /// <summary>
        /// Asserts on the actual values that were passed into a mocked method.  The callback receives
        /// these values and is expected to perform the assertions.
        /// </summary>
        /// <param name="action">The callback which receives the recorded objects and performs the assertions.</param>
        /// <param name="call">The values that were passed into the mocked method</param>
        /// <typeparam name="T1">The type of the method's 1st parameter</typeparam>
        /// <typeparam name="T2">The type of the method's 2nd parameter</typeparam>
        /// <typeparam name="T3">The type of the method's 3rd parameter</typeparam>
        /// <typeparam name="T4">The type of the method's 4th parameter</typeparam>
        /// <typeparam name="T5">The type of the method's 5th parameter</typeparam>
        /// <typeparam name="T6">The type of the method's 6th parameter</typeparam>
        /// <typeparam name="T7">The type of the method's 7th parameter</typeparam>
        /// <typeparam name="T8">The type of the method's 8th parameter</typeparam>
        /// <returns>This object to allow chaining</returns>
        public static IRecordedCall Replay<T1, T2, T3, T4, T5, T6, T7, T8>(this IRecordedCall call, Action<T1, T2, T3, T4, T5, T6, T7, T8> action)
        {
            return call.VerifyAndInvoke(action);
        }

        /// <summary>
        /// Asserts on the actual values that were passed into a mocked method.  The callback receives
        /// these values and is expected to perform the assertions.
        /// </summary>
        /// <param name="action">The callback which receives the recorded objects and performs the assertions.</param>
        /// <param name="call">The values that were passed into the mocked method</param>
        /// <typeparam name="T1">The type of the method's 1st parameter</typeparam>
        /// <typeparam name="T2">The type of the method's 2nd parameter</typeparam>
        /// <typeparam name="T3">The type of the method's 3rd parameter</typeparam>
        /// <typeparam name="T4">The type of the method's 4th parameter</typeparam>
        /// <typeparam name="T5">The type of the method's 5th parameter</typeparam>
        /// <typeparam name="T6">The type of the method's 6th parameter</typeparam>
        /// <typeparam name="T7">The type of the method's 7th parameter</typeparam>
        /// <typeparam name="T8">The type of the method's 8th parameter</typeparam>
        /// <typeparam name="T9">The type of the method's 9th parameter</typeparam>
        /// <returns>This object to allow chaining</returns>
        public static IRecordedCall Replay<T1, T2, T3, T4, T5, T6, T7, T8, T9>(this IRecordedCall call, Action<T1, T2, T3, T4, T5, T6, T7, T8, T9> action)
        {
            return call.VerifyAndInvoke(action);
        }

        /// <summary>
        /// Asserts on the actual values that were passed into a mocked method.  The callback receives
        /// these values and is expected to perform the assertions.
        /// </summary>
        /// <param name="action">The callback which receives the recorded objects and performs the assertions.</param>
        /// <param name="call">The values that were passed into the mocked method</param>
        /// <typeparam name="T1">The type of the method's 1st parameter</typeparam>
        /// <typeparam name="T2">The type of the method's 2nd parameter</typeparam>
        /// <typeparam name="T3">The type of the method's 3rd parameter</typeparam>
        /// <typeparam name="T4">The type of the method's 4th parameter</typeparam>
        /// <typeparam name="T5">The type of the method's 5th parameter</typeparam>
        /// <typeparam name="T6">The type of the method's 6th parameter</typeparam>
        /// <typeparam name="T7">The type of the method's 7th parameter</typeparam>
        /// <typeparam name="T8">The type of the method's 8th parameter</typeparam>
        /// <typeparam name="T9">The type of the method's 9th parameter</typeparam>
        /// <typeparam name="T10">The type of the method's 10th parameter</typeparam>
        /// <returns>This object to allow chaining</returns>
        public static IRecordedCall Replay<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(this IRecordedCall call, Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10> action)
        {
            return call.VerifyAndInvoke(action);
        }

        /// <summary>
        /// Asserts on the actual values that were passed into a mocked method.  The callback receives
        /// these values and is expected to perform the assertions.
        /// </summary>
        /// <param name="action">The callback which receives the recorded objects and performs the assertions.</param>
        /// <param name="call">The values that were passed into the mocked method</param>
        /// <typeparam name="T1">The type of the method's 1st parameter</typeparam>
        /// <typeparam name="T2">The type of the method's 2nd parameter</typeparam>
        /// <typeparam name="T3">The type of the method's 3rd parameter</typeparam>
        /// <typeparam name="T4">The type of the method's 4th parameter</typeparam>
        /// <typeparam name="T5">The type of the method's 5th parameter</typeparam>
        /// <typeparam name="T6">The type of the method's 6th parameter</typeparam>
        /// <typeparam name="T7">The type of the method's 7th parameter</typeparam>
        /// <typeparam name="T8">The type of the method's 8th parameter</typeparam>
        /// <typeparam name="T9">The type of the method's 9th parameter</typeparam>
        /// <typeparam name="T10">The type of the method's 10th parameter</typeparam>
        /// <typeparam name="T11">The type of the method's 11th parameter</typeparam>
        /// <returns>This object to allow chaining</returns>
        public static IRecordedCall Replay<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(this IRecordedCall call, Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> action)
        {
            return call.VerifyAndInvoke(action);
        }

        /// <summary>
        /// Asserts on the actual values that were passed into a mocked method.  The callback receives
        /// these values and is expected to perform the assertions.
        /// </summary>
        /// <param name="action">The callback which receives the recorded objects and performs the assertions.</param>
        /// <param name="call">The values that were passed into the mocked method</param>
        /// <typeparam name="T1">The type of the method's 1st parameter</typeparam>
        /// <typeparam name="T2">The type of the method's 2nd parameter</typeparam>
        /// <typeparam name="T3">The type of the method's 3rd parameter</typeparam>
        /// <typeparam name="T4">The type of the method's 4th parameter</typeparam>
        /// <typeparam name="T5">The type of the method's 5th parameter</typeparam>
        /// <typeparam name="T6">The type of the method's 6th parameter</typeparam>
        /// <typeparam name="T7">The type of the method's 7th parameter</typeparam>
        /// <typeparam name="T8">The type of the method's 8th parameter</typeparam>
        /// <typeparam name="T9">The type of the method's 9th parameter</typeparam>
        /// <typeparam name="T10">The type of the method's 10th parameter</typeparam>
        /// <typeparam name="T11">The type of the method's 11th parameter</typeparam>
        /// <typeparam name="T12">The type of the method's 12th parameter</typeparam>
        /// <returns>This object to allow chaining</returns>
        public static IRecordedCall Replay<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(this IRecordedCall call, Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> action)
        {
            return call.VerifyAndInvoke(action);
        }

        /// <summary>
        /// Asserts on the actual values that were passed into a mocked method.  The callback receives
        /// these values and is expected to perform the assertions.
        /// </summary>
        /// <param name="action">The callback which receives the recorded objects and performs the assertions.</param>
        /// <param name="call">The values that were passed into the mocked method</param>
        /// <typeparam name="T1">The type of the method's 1st parameter</typeparam>
        /// <typeparam name="T2">The type of the method's 2nd parameter</typeparam>
        /// <typeparam name="T3">The type of the method's 3rd parameter</typeparam>
        /// <typeparam name="T4">The type of the method's 4th parameter</typeparam>
        /// <typeparam name="T5">The type of the method's 5th parameter</typeparam>
        /// <typeparam name="T6">The type of the method's 6th parameter</typeparam>
        /// <typeparam name="T7">The type of the method's 7th parameter</typeparam>
        /// <typeparam name="T8">The type of the method's 8th parameter</typeparam>
        /// <typeparam name="T9">The type of the method's 9th parameter</typeparam>
        /// <typeparam name="T10">The type of the method's 10th parameter</typeparam>
        /// <typeparam name="T11">The type of the method's 11th parameter</typeparam>
        /// <typeparam name="T12">The type of the method's 12th parameter</typeparam>
        /// <typeparam name="T13">The type of the method's 13th parameter</typeparam>
        /// <returns>This object to allow chaining</returns>
        public static IRecordedCall Replay<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(this IRecordedCall call, Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13> action)
        {
            return call.VerifyAndInvoke(action);
        }

        /// <summary>
        /// Asserts on the actual values that were passed into a mocked method.  The callback receives
        /// these values and is expected to perform the assertions.
        /// </summary>
        /// <param name="action">The callback which receives the recorded objects and performs the assertions.</param>
        /// <param name="call">The values that were passed into the mocked method</param>
        /// <typeparam name="T1">The type of the method's 1st parameter</typeparam>
        /// <typeparam name="T2">The type of the method's 2nd parameter</typeparam>
        /// <typeparam name="T3">The type of the method's 3rd parameter</typeparam>
        /// <typeparam name="T4">The type of the method's 4th parameter</typeparam>
        /// <typeparam name="T5">The type of the method's 5th parameter</typeparam>
        /// <typeparam name="T6">The type of the method's 6th parameter</typeparam>
        /// <typeparam name="T7">The type of the method's 7th parameter</typeparam>
        /// <typeparam name="T8">The type of the method's 8th parameter</typeparam>
        /// <typeparam name="T9">The type of the method's 9th parameter</typeparam>
        /// <typeparam name="T10">The type of the method's 10th parameter</typeparam>
        /// <typeparam name="T11">The type of the method's 11th parameter</typeparam>
        /// <typeparam name="T12">The type of the method's 12th parameter</typeparam>
        /// <typeparam name="T13">The type of the method's 13th parameter</typeparam>
        /// <typeparam name="T14">The type of the method's 14th parameter</typeparam>
        /// <returns>This object to allow chaining</returns>
        public static IRecordedCall Replay<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(this IRecordedCall call, Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14> action)
        {
            return call.VerifyAndInvoke(action);
        }

        /// <summary>
        /// Asserts on the actual values that were passed into a mocked method.  The callback receives
        /// these values and is expected to perform the assertions.
        /// </summary>
        /// <param name="action">The callback which receives the recorded objects and performs the assertions.</param>
        /// <param name="call">The values that were passed into the mocked method</param>
        /// <typeparam name="T1">The type of the method's 1st parameter</typeparam>
        /// <typeparam name="T2">The type of the method's 2nd parameter</typeparam>
        /// <typeparam name="T3">The type of the method's 3rd parameter</typeparam>
        /// <typeparam name="T4">The type of the method's 4th parameter</typeparam>
        /// <typeparam name="T5">The type of the method's 5th parameter</typeparam>
        /// <typeparam name="T6">The type of the method's 6th parameter</typeparam>
        /// <typeparam name="T7">The type of the method's 7th parameter</typeparam>
        /// <typeparam name="T8">The type of the method's 8th parameter</typeparam>
        /// <typeparam name="T9">The type of the method's 9th parameter</typeparam>
        /// <typeparam name="T10">The type of the method's 10th parameter</typeparam>
        /// <typeparam name="T11">The type of the method's 11th parameter</typeparam>
        /// <typeparam name="T12">The type of the method's 12th parameter</typeparam>
        /// <typeparam name="T13">The type of the method's 13th parameter</typeparam>
        /// <typeparam name="T14">The type of the method's 14th parameter</typeparam>
        /// <typeparam name="T15">The type of the method's 15th parameter</typeparam>
        /// <returns>This object to allow chaining</returns>
        public static IRecordedCall Replay<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(this IRecordedCall call, Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15> action)
        {
            return call.VerifyAndInvoke(action);
        }

        /// <summary>
        /// Asserts on the actual values that were passed into a mocked method.  The callback receives
        /// these values and is expected to perform the assertions.
        /// </summary>
        /// <param name="action">The callback which receives the recorded objects and performs the assertions.</param>
        /// <param name="call">The values that were passed into the mocked method</param>
        /// <typeparam name="T1">The type of the method's 1st parameter</typeparam>
        /// <typeparam name="T2">The type of the method's 2nd parameter</typeparam>
        /// <typeparam name="T3">The type of the method's 3rd parameter</typeparam>
        /// <typeparam name="T4">The type of the method's 4th parameter</typeparam>
        /// <typeparam name="T5">The type of the method's 5th parameter</typeparam>
        /// <typeparam name="T6">The type of the method's 6th parameter</typeparam>
        /// <typeparam name="T7">The type of the method's 7th parameter</typeparam>
        /// <typeparam name="T8">The type of the method's 8th parameter</typeparam>
        /// <typeparam name="T9">The type of the method's 9th parameter</typeparam>
        /// <typeparam name="T10">The type of the method's 10th parameter</typeparam>
        /// <typeparam name="T11">The type of the method's 11th parameter</typeparam>
        /// <typeparam name="T12">The type of the method's 12th parameter</typeparam>
        /// <typeparam name="T13">The type of the method's 13th parameter</typeparam>
        /// <typeparam name="T14">The type of the method's 14th parameter</typeparam>
        /// <typeparam name="T15">The type of the method's 15th parameter</typeparam>
        /// <typeparam name="T16">The type of the method's 16th parameter</typeparam>
        /// <returns>This object to allow chaining</returns>
        public static IRecordedCall Replay<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16>(this IRecordedCall call, Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16> action)
        {
            return call.VerifyAndInvoke(action);
        }

        /// <summary>
        /// Asserts on the actual values that were passed into a mocked method.  The callback receives
        /// these values and is expected to perform the assertions.
        /// </summary>
        /// <param name="action">The callback which receives the recorded objects and performs the assertions.</param>
        /// <param name="call">The values that were passed into the mocked method</param>
        /// <returns>This object to allow chaining</returns>
        public static IRecordedCall Replay(this IRecordedCall call, Action action)
        {
            return call.VerifyAndInvoke(action);
        }

        /// <summary>
        /// Asserts on the actual values that were passed into a mocked method.  The callback receives
        /// these values and is expected to perform the assertions.
        /// </summary>
        /// <param name="action">The callback which receives the recorded objects and performs the assertions.</param>
        /// <param name="call">The values that were passed into the mocked method</param>
        /// <typeparam name="T">The type of the method's only parameter</typeparam>
        /// <returns>This object to allow chaining</returns>
        public static IRecordedCall Replay<T>(this IRecordedCall call, Action<T> action)
        {
            return call.VerifyAndInvoke(action);
        }

        #endregion
    }

}
