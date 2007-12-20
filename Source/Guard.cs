using System;

internal static class Guard
{
	/// <summary>
	/// Checks an argument to ensure it isn't null.
	/// </summary>
	/// <param name="value">The argument value to check.</param>
	/// <param name="argumentName">The name of the argument.</param>
	public static void ArgumentNotNull(object value, string argumentName)
	{
		if (value == null)
			throw new ArgumentNullException(argumentName);
	}

	/// <summary>
	/// Checks a string argument to ensure it isn't null or empty.
	/// </summary>
	/// <param name="argumentValue">The argument value to check.</param>
	/// <param name="argumentName">The name of the argument.</param>
	public static void ArgumentNotNullOrEmptyString(string argumentValue, string argumentName)
	{
		ArgumentNotNull(argumentValue, argumentName);

		if (argumentValue.Length == 0)
			throw new ArgumentException(
				"Value cannot be null or an empty string.",
				argumentName);
	}

	public static void CanBeAssigned(Type typeToAssign, Type targetType)
	{
		if (!targetType.IsAssignableFrom(typeToAssign))
		{
			if (targetType.IsInterface)
				throw new ArgumentException(String.Format(
					"Type {0} does not implement required interface {1}",
					typeToAssign, targetType));
			else
				throw new ArgumentException(String.Format(
					"Type {0} does not from required type {1}",
					typeToAssign, targetType));
		}
	}

	public static void CanBeAssigned(Type typeToAssign, Type targetType, string argumentName)
	{
		if (!targetType.IsAssignableFrom(typeToAssign))
		{
			if (targetType.IsInterface)
				throw new ArgumentException(String.Format(
					"Type {0} does not implement required interface {1}",
					typeToAssign, targetType), argumentName);
			else
				throw new ArgumentException(String.Format(
					"Type {0} does not from required type {1}",
					typeToAssign, targetType), argumentName);
		}
	}
}
