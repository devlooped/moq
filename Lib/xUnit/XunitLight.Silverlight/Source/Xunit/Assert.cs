using System;
using System.Collections;
using System.Collections.Generic;
using Xunit.Sdk;

namespace Xunit
{
	/// <summary>
	/// Contains various static methods that are used to verify that conditions are met during the
	/// process of running tests.
	/// </summary>
	public class Assert
	{
		/// <summary>
		/// Used by the Throws and DoesNotThrow methods.
		/// </summary>
		public delegate void ThrowsDelegate();

		/// <summary>
		/// Initializes a new instance of the <see cref="Assert"/> class.
		/// </summary>
		protected Assert() { }

		/// <summary>
		/// Verifies that a collection contains a given object.
		/// </summary>
		/// <typeparam name="T">The type of the object to be verified</typeparam>
		/// <param name="expected">The object expected to be in the collection</param>
		/// <param name="collection">The collection to be inspected</param>
		/// <exception cref="ContainsException">Thrown when the object is not present in the collection</exception>
		public static void Contains<T>(T expected,
									   IEnumerable<T> collection)
		{
			Contains(expected, collection, GetComparer<T>());
		}

		/// <summary>
		/// Verifies that a collection contains a given object, using a comparer.
		/// </summary>
		/// <typeparam name="T">The type of the object to be verified</typeparam>
		/// <param name="expected">The object expected to be in the collection</param>
		/// <param name="collection">The collection to be inspected</param>
		/// <param name="comparer">The comparer used to equate objects in the collection with the expected object</param>
		/// <exception cref="ContainsException">Thrown when the object is not present in the collection</exception>
		public static void Contains<T>(T expected,
									   IEnumerable<T> collection,
									   IComparer<T> comparer)
		{
			foreach (T item in collection)
				if (comparer.Compare(expected, item) == 0)
					return;

			throw new ContainsException(expected);
		}

		/// <summary>
		/// Verifies that a string contains a given sub-string, using the current culture.
		/// </summary>
		/// <param name="expectedSubString">The sub-string expected to be in the string</param>
		/// <param name="actualString">The string to be inspected</param>
		/// <exception cref="ContainsException">Thrown when the sub-string is not present inside the string</exception>
		public static void Contains(string expectedSubString,
									string actualString)
		{
			Contains(expectedSubString, actualString, StringComparison.CurrentCulture);
		}

		/// <summary>
		/// Verifies that a string contains a given sub-string, using the given comparison type.
		/// </summary>
		/// <param name="expectedSubString">The sub-string expected to be in the string</param>
		/// <param name="actualString">The string to be inspected</param>
		/// <param name="comparisonType">The type of string comparison to perform</param>
		/// <exception cref="ContainsException">Thrown when the sub-string is not present inside the string</exception>
		public static void Contains(string expectedSubString,
									string actualString,
									StringComparison comparisonType)
		{
			if (actualString.IndexOf(expectedSubString, comparisonType) < 0)
				throw new ContainsException(expectedSubString);
		}

		/// <summary>
		/// Verifies that a collection does not contain a given object.
		/// </summary>
		/// <typeparam name="T">The type of the object to be compared</typeparam>
		/// <param name="expected">The object that is expected not to be in the collection</param>
		/// <param name="collection">The collection to be inspected</param>
		/// <exception cref="DoesNotContainException">Thrown when the object is present inside the container</exception>
		public static void DoesNotContain<T>(T expected,
											 IEnumerable<T> collection)
		{
			DoesNotContain(expected, collection, GetComparer<T>());
		}

		/// <summary>
		/// Verifies that a collection does not contain a given object, using a comparer.
		/// </summary>
		/// <typeparam name="T">The type of the object to be compared</typeparam>
		/// <param name="expected">The object that is expected not to be in the collection</param>
		/// <param name="collection">The collection to be inspected</param>
		/// <param name="comparer">The comparer used to equate objects in the collection with the expected object</param>
		/// <exception cref="DoesNotContainException">Thrown when the object is present inside the container</exception>
		public static void DoesNotContain<T>(T expected,
											 IEnumerable<T> collection,
											 IComparer<T> comparer)
		{
			foreach (T item in collection)
				if (comparer.Compare(expected, item) == 0)
					throw new DoesNotContainException(expected);
		}

		/// <summary>
		/// Verifies that a string does not contain a given sub-string, using the current culture.
		/// </summary>
		/// <param name="expectedSubString">The sub-string which is expected not to be in the string</param>
		/// <param name="actualString">The string to be inspected</param>
		/// <exception cref="DoesNotContainException">Thrown when the sub-string is present inside the string</exception>
		public static void DoesNotContain(string expectedSubString,
										  string actualString)
		{
			DoesNotContain(expectedSubString, actualString, StringComparison.CurrentCulture);
		}

		/// <summary>
		/// Verifies that a string does not contain a given sub-string, using the current culture.
		/// </summary>
		/// <param name="expectedSubString">The sub-string which is expected not to be in the string</param>
		/// <param name="actualString">The string to be inspected</param>
		/// <param name="comparisonType">The type of string comparison to perform</param>
		/// <exception cref="DoesNotContainException">Thrown when the sub-string is present inside the given string</exception>
		public static void DoesNotContain(string expectedSubString,
										  string actualString,
										  StringComparison comparisonType)
		{
			if (actualString.IndexOf(expectedSubString, comparisonType) >= 0)
				throw new DoesNotContainException(expectedSubString);
		}

		/// <summary>
		/// Verifies that a block of code does not throw any exceptions.
		/// </summary>
		/// <param name="testCode">A delegate to the code to be tested</param>
		public static void DoesNotThrow(ThrowsDelegate testCode)
		{
			Exception ex = Record.Exception(testCode);

			if (ex != null)
				throw new DoesNotThrowException(ex);
		}

		/// <summary>
		/// Verifies that a collection is empty.
		/// </summary>
		/// <param name="collection">The collection to be inspected</param>
		/// <exception cref="ArgumentNullException">Thrown when the collection is null</exception>
		/// <exception cref="EmptyException">Thrown when the collection is not empty</exception>
		public static void Empty(IEnumerable collection)
		{
			if (collection == null) throw new ArgumentNullException("collection", "cannot be null");

#pragma warning disable 168
			foreach (object @object in collection)
				throw new EmptyException();
#pragma warning restore 168
		}

		/// <summary>
		/// Verifies that two objects are equal, using a default comparer.
		/// </summary>
		/// <typeparam name="T">The type of the objects to be compared</typeparam>
		/// <param name="expected">The expected value</param>
		/// <param name="actual">The value to be compared against</param>
		/// <exception cref="EqualException">Thrown when the objects are not equal</exception>
		public static void Equal<T>(T expected,
									T actual)
		{
			Equal(expected, actual, GetComparer<T>());
		}

		/// <summary>
		/// Verifies that two objects are equal, using a custom comparer.
		/// </summary>
		/// <typeparam name="T">The type of the objects to be compared</typeparam>
		/// <param name="expected">The expected value</param>
		/// <param name="actual">The value to be compared against</param>
		/// <param name="comparer">The comparer used to compare the two objects</param>
		/// <exception cref="EqualException">Thrown when the objects are not equal</exception>
		public static void Equal<T>(T expected,
									T actual,
									IComparer<T> comparer)
		{
			if (comparer.Compare(expected, actual) != 0)
				throw new EqualException(expected, actual);
		}

		/// <summary>Do not call this method.</summary>
		[Obsolete("This is an override of Object.Equals(). Call Assert.Equal() instead.", true)]
		public new static bool Equals(object a,
									  object b)
		{
			throw new InvalidOperationException("Assert.Equals should not be used");
		}

		/// <summary>
		/// Verifies that the condition is false.
		/// </summary>
		/// <param name="condition">The condition to be tested</param>
		/// <exception cref="FalseException">Thrown if the condition is not false</exception>
		public static void False(bool condition)
		{
			False(condition, null);
		}

		/// <summary>
		/// Verifies that the condition is false.
		/// </summary>
		/// <param name="condition">The condition to be tested</param>
		/// <param name="userMessage">The message to show when the condition is not false</param>
		/// <exception cref="FalseException">Thrown if the condition is not false</exception>
		public static void False(bool condition,
								 string userMessage)
		{
			if (condition)
				throw new FalseException(userMessage);
		}

		static IComparer<T> GetComparer<T>()
		{
			return new AssertComparer<T>();
		}

		/// <summary>
		/// Verifies that a value is within a given range.
		/// </summary>
		/// <typeparam name="T">The type of the value to be compared</typeparam>
		/// <param name="actual">The actual value to be evaluated</param>
		/// <param name="low">The (inclusive) low value of the range</param>
		/// <param name="high">The (inclusive) high value of the range</param>
		/// <exception cref="InRangeException">Thrown when the value is not in the given range</exception>
		public static void InRange<T>(T actual,
									  T low,
									  T high)
		{
			InRange(actual, low, high, GetComparer<T>());
		}

		/// <summary>
		/// Verifies that a value is within a given range, using a comparer.
		/// </summary>
		/// <typeparam name="T">The type of the value to be compared</typeparam>
		/// <param name="actual">The actual value to be evaluated</param>
		/// <param name="low">The (inclusive) low value of the range</param>
		/// <param name="high">The (inclusive) high value of the range</param>
		/// <param name="comparer">The comparer used to evaluate the value's range</param>
		/// <exception cref="InRangeException">Thrown when the value is not in the given range</exception>
		public static void InRange<T>(T actual,
									  T low,
									  T high,
									  IComparer<T> comparer)
		{
			if (comparer.Compare(low, actual) > 0 || comparer.Compare(actual, high) > 0)
				throw new InRangeException(actual, low, high);
		}

		/// <summary>
		/// Verifies that an object is of the given type or a derived type.
		/// </summary>
		/// <typeparam name="T">The type the object should be</typeparam>
		/// <param name="object">The object to be evaluated</param>
		/// <returns>The object, casted to type T when successful</returns>
		/// <exception cref="IsAssignableFromException">Thrown when the object is not the given type</exception>
		public static T IsAssignableFrom<T>(object @object)
		{
			IsAssignableFrom(typeof(T), @object);
			return (T)@object;
		}

		/// <summary>
		/// Verifies that an object is of the given type or a derived type.
		/// </summary>
		/// <param name="expectedType">The type the object should be</param>
		/// <param name="object">The object to be evaluated</param>
		/// <exception cref="IsAssignableFromException">Thrown when the object is not the given type</exception>
		public static void IsAssignableFrom(Type expectedType,
											object @object)
		{
			if (@object == null || !expectedType.IsAssignableFrom(@object.GetType()))
				throw new IsAssignableFromException(expectedType, @object);
		}

		/// <summary>
		/// Verifies that an object is not exactly the given type.
		/// </summary>
		/// <typeparam name="T">The type the object should not be</typeparam>
		/// <param name="object">The object to be evaluated</param>
		/// <exception cref="IsNotTypeException">Thrown when the object is the given type</exception>
		public static void IsNotType<T>(object @object)
		{
			IsNotType(typeof(T), @object);
		}

		/// <summary>
		/// Verifies that an object is not exactly the given type.
		/// </summary>
		/// <param name="expectedType">The type the object should not be</param>
		/// <param name="object">The object to be evaluated</param>
		/// <exception cref="IsNotTypeException">Thrown when the object is the given type</exception>
		public static void IsNotType(Type expectedType,
									 object @object)
		{
			if (expectedType.Equals(@object.GetType()))
				throw new IsNotTypeException(expectedType, @object);
		}

		/// <summary>
		/// Verifies that an object is exactly the given type (and not a derived type).
		/// </summary>
		/// <typeparam name="T">The type the object should be</typeparam>
		/// <param name="object">The object to be evaluated</param>
		/// <returns>The object, casted to type T when successful</returns>
		/// <exception cref="IsTypeException">Thrown when the object is not the given type</exception>
		public static T IsType<T>(object @object)
		{
			IsType(typeof(T), @object);
			return (T)@object;
		}

		/// <summary>
		/// Verifies that an object is exactly the given type (and not a derived type).
		/// </summary>
		/// <param name="expectedType">The type the object should be</param>
		/// <param name="object">The object to be evaluated</param>
		/// <exception cref="IsTypeException">Thrown when the object is not the given type</exception>
		public static void IsType(Type expectedType,
								  object @object)
		{
			if (@object == null || !expectedType.Equals(@object.GetType()))
				throw new IsTypeException(expectedType, @object);
		}

		/// <summary>
		/// Verifies that a collection is not empty.
		/// </summary>
		/// <param name="collection">The collection to be inspected</param>
		/// <exception cref="ArgumentNullException">Thrown when a null collection is passed</exception>
		/// <exception cref="NotEmptyException">Thrown when the collection is empty</exception>
		public static void NotEmpty(IEnumerable collection)
		{
			if (collection == null) throw new ArgumentNullException("collection", "cannot be null");

#pragma warning disable 168
			foreach (object @object in collection)
				return;
#pragma warning restore 168

			throw new NotEmptyException();
		}

		/// <summary>
		/// Verifies that two objects are not equal, using a default comparer.
		/// </summary>
		/// <typeparam name="T">The type of the objects to be compared</typeparam>
		/// <param name="expected">The expected object</param>
		/// <param name="actual">The actual object</param>
		/// <exception cref="NotEqualException">Thrown when the objects are equal</exception>
		public static void NotEqual<T>(T expected,
									   T actual)
		{
			NotEqual(expected, actual, GetComparer<T>());
		}

		/// <summary>
		/// Verifies that two objects are not equal, using a custom comparer.
		/// </summary>
		/// <typeparam name="T">The type of the objects to be compared</typeparam>
		/// <param name="expected">The expected object</param>
		/// <param name="actual">The actual object</param>
		/// <param name="comparer">The comparer used to examine the objects</param>
		/// <exception cref="NotEqualException">Thrown when the objects are equal</exception>
		public static void NotEqual<T>(T expected,
									   T actual,
									   IComparer<T> comparer)
		{
			if (comparer.Compare(expected, actual) == 0)
				throw new NotEqualException();
		}

		/// <summary>
		/// Verifies that a value is not within a given range, using the default comparer.
		/// </summary>
		/// <typeparam name="T">The type of the value to be compared</typeparam>
		/// <param name="actual">The actual value to be evaluated</param>
		/// <param name="low">The (inclusive) low value of the range</param>
		/// <param name="high">The (inclusive) high value of the range</param>
		/// <exception cref="NotInRangeException">Thrown when the value is in the given range</exception>
		public static void NotInRange<T>(T actual,
										 T low,
										 T high)
		{
			NotInRange(actual, low, high, GetComparer<T>());
		}

		/// <summary>
		/// Verifies that a value is not within a given range, using a comparer.
		/// </summary>
		/// <typeparam name="T">The type of the value to be compared</typeparam>
		/// <param name="actual">The actual value to be evaluated</param>
		/// <param name="low">The (inclusive) low value of the range</param>
		/// <param name="high">The (inclusive) high value of the range</param>
		/// <param name="comparer">The comparer used to evaluate the value's range</param>
		/// <exception cref="NotInRangeException">Thrown when the value is in the given range</exception>
		public static void NotInRange<T>(T actual,
										 T low,
										 T high,
										 IComparer<T> comparer)
		{
			if (comparer.Compare(low, actual) <= 0 && comparer.Compare(actual, high) <= 0)
				throw new NotInRangeException(actual, low, high);
		}

		/// <summary>
		/// Verifies that an object reference is not null.
		/// </summary>
		/// <param name="object">The object to be validated</param>
		/// <exception cref="NotNullException">Thrown when the object is not null</exception>
		public static void NotNull(object @object)
		{
			if (@object == null)
				throw new NotNullException();
		}

		/// <summary>
		/// Verifies that two objects are not the same instance.
		/// </summary>
		/// <param name="expected">The expected object instance</param>
		/// <param name="actual">The actual object instance</param>
		/// <exception cref="NotSameException">Thrown when the objects are the same instance</exception>
		public static void NotSame(object expected,
								   object actual)
		{
			if (object.ReferenceEquals(expected, actual))
				throw new NotSameException();
		}

		/// <summary>
		/// Verifies that an object reference is null.
		/// </summary>
		/// <param name="object">The object to be inspected</param>
		/// <exception cref="NullException">Thrown when the object reference is not null</exception>
		public static void Null(object @object)
		{
			if (@object != null)
				throw new NullException(@object);
		}

		/// <summary>
		/// Verifies that two objects are the same instance.
		/// </summary>
		/// <param name="expected">The expected object instance</param>
		/// <param name="actual">The actual object instance</param>
		/// <exception cref="SameException">Thrown when the objects are not the same instance</exception>
		public static void Same(object expected,
								object actual)
		{
			if (!object.ReferenceEquals(expected, actual))
				throw new SameException(expected, actual);
		}

		/// <summary>
		/// Verifies that the exact exception is thrown (and not a derived exception type).
		/// </summary>
		/// <typeparam name="T">The type of the exception expected to be thrown</typeparam>
		/// <param name="testCode">A delegate to the code to be tested</param>
		/// <returns>The exception that was thrown, when successful</returns>
		/// <exception cref="ThrowsException">Thrown when an exception was not thrown, or when an exception of the incorrect type is thrown</exception>
		public static T Throws<T>(ThrowsDelegate testCode)
			where T : Exception
		{
			return (T)Throws(typeof(T), testCode);
		}

		/// <summary>
		/// Verifies that the exact exception is thrown (and not a derived exception type).
		/// </summary>
		/// <typeparam name="T">The type of the exception expected to be thrown</typeparam>
		/// <param name="userMessage">The message to be shown if the test fails</param>
		/// <param name="testCode">A delegate to the code to be tested</param>
		/// <returns>The exception that was thrown, when successful</returns>
		/// <exception cref="ThrowsException">Thrown when an exception was not thrown, or when an exception of the incorrect type is thrown</exception>
		public static T Throws<T>(string userMessage,
								  ThrowsDelegate testCode)
			where T : Exception
		{
			return (T)Throws(typeof(T), testCode);
		}

		/// <summary>
		/// Verifies that the exact exception is thrown (and not a derived exception type).
		/// </summary>
		/// <param name="exceptionType">The type of the exception expected to be thrown</param>
		/// <param name="testCode">A delegate to the code to be tested</param>
		/// <returns>The exception that was thrown, when successful</returns>
		/// <exception cref="ThrowsException">Thrown when an exception was not thrown, or when an exception of the incorrect type is thrown</exception>
		public static Exception Throws(Type exceptionType,
									   ThrowsDelegate testCode)
		{
			Exception exception = Record.Exception(testCode);

			if (exception == null)
				throw new ThrowsException(exceptionType);

			if (!exceptionType.Equals(exception.GetType()))
				throw new ThrowsException(exceptionType, exception);

			return exception;
		}

		/// <summary>
		/// Verifies that an expression is true.
		/// </summary>
		/// <param name="condition">The condition to be inspected</param>
		/// <exception cref="TrueException">Thrown when the condition is false</exception>
		public static void True(bool condition)
		{
			True(condition, null);
		}

		/// <summary>
		/// Verifies that an expression is true.
		/// </summary>
		/// <param name="condition">The condition to be inspected</param>
		/// <param name="userMessage">The message to be shown when the condition is false</param>
		/// <exception cref="TrueException">Thrown when the condition is false</exception>
		public static void True(bool condition,
								string userMessage)
		{
			if (!condition)
				throw new TrueException(userMessage);
		}

		class AssertComparer<T> : IComparer<T>
		{
			public int Compare(T x,
							   T y)
			{
				Type type = typeof(T);

				// Null?

				if (!type.IsValueType || (type.IsGenericType && type.GetGenericTypeDefinition().IsAssignableFrom(typeof(Nullable<>))))
				{
					if (Equals(x, default(T)))
					{
						if (Equals(y, default(T)))
							return 0;
						return -1;
					}

					if (Equals(y, default(T)))
						return -1;
				}

				// Same type?

				if (x.GetType() != y.GetType())
					return -1;

				// Arrays?

				if (x.GetType().IsArray)
				{
					Array xArray = x as Array;
					Array yArray = y as Array;

					if (xArray != null && yArray != null)
					{
						if (xArray.Rank != 1)
							throw new ArgumentException("Multi-dimension array comparison is not supported");

						if (xArray.Length != yArray.Length)
							return -1;

						for (int index = 0; index < xArray.Length; index++)
							if (!Equals(xArray.GetValue(index), yArray.GetValue(index)))
								return -1;

						return 0;
					}
				}

				// Implements IComparable<T>?

				IComparable<T> comparable1 = x as IComparable<T>;

				if (comparable1 != null)
					return comparable1.CompareTo(y);

				// Implements IComparable?

				IComparable comparable2 = x as IComparable;

				if (comparable2 != null)
					return comparable2.CompareTo(y);

				// Implements IEquatable<T>?

				IEquatable<T> equatable = x as IEquatable<T>;

				if (equatable != null)
					return equatable.Equals(y) ? 0 : -1;

				// Last case, rely on Object.Equals

				return Equals(x, y) ? 0 : -1;
			}
		}
	}
}
