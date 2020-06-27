// Copyright (c) 2007, Clarius Consulting, Manas Technology Solutions, InSTEDD, and Contributors.
// All rights reserved. Licensed under the BSD 3-Clause License; see License.txt.

using System;
using System.ComponentModel;
using System.Linq.Expressions;

using Moq.Language.Flow;
using Moq.Protected;

namespace Moq
{
	// Keeps legacy members that are hidden and are provided 
	// for backwards compatibility (so that existing projects 
	// still compile, but people don't see them).
	// A bug in EditorBrowsable actually prevents us from moving these members 
	// completely to extension methods, as the attribute is not honored and 
	// therefore the members are always visible.
	public partial class Mock<T>
	{
		/// <summary>
		/// Obsolete.
		/// </summary>
		[Obsolete("Expect has been renamed to Setup.", false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public ISetup<T> Expect(Expression<Action<T>> expression)
		{
			return Setup(expression);
		}

		/// <summary>
		/// Obsolete.
		/// </summary>
		[Obsolete("Expect has been renamed to Setup.", false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public ISetup<T, TResult> Expect<TResult>(Expression<Func<T, TResult>> expression)
		{
			return Setup(expression);
		}

		/// <summary>
		/// Obsolete.
		/// </summary>
		[Obsolete("ExpectGet has been renamed to SetupGet.", false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public ISetupGetter<T, TProperty> ExpectGet<TProperty>(Expression<Func<T, TProperty>> expression)
		{
			return SetupGet(expression);
		}

		/// <summary>
		/// Obsolete.
		/// </summary>
		[Obsolete("ExpectSet has been renamed to SetupSet.", false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public ISetupSetter<T, TProperty> ExpectSet<TProperty>(Expression<Func<T, TProperty>> expression)
		{
			return this.SetupSet(expression);
		}

		/// <summary>
		/// Obsolete.
		/// </summary>
		[Obsolete("ExpectSet has been renamed to SetupSet, and the new syntax allows you to pass the value in the expression itself, like f => f.Value = 25.", true)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public ISetupSetter<T, TProperty> ExpectSet<TProperty>(Expression<Func<T, TProperty>> expression, TProperty value)
		{
			throw new NotSupportedException();
		}
	}

	/// <summary>
	/// Contains obsolete API members as extension methods so that existing code continues to compile,
	/// but new code doesn't see them.
	/// </summary>
	[EditorBrowsable(EditorBrowsableState.Never)]
	public static class MockLegacyExtensions
	{
		/// <summary>
		/// Obsolete.
		/// </summary>
		[EditorBrowsable(EditorBrowsableState.Never)]
		[Obsolete("The new syntax allows you to pass the value in the expression itself, like f => f.Value = 25.", true)]
		public static ISetupSetter<T, TProperty> SetupSet<T, TProperty>(this Mock<T> mock, Expression<Func<T, TProperty>> expression, TProperty value)
			where T : class
		{
			throw new NotSupportedException();
		}

		/// <summary>
		/// Obsolete.
		/// </summary>
		[EditorBrowsable(EditorBrowsableState.Never)]
		[Obsolete("Use the new syntax, which allows you to pass the value in the expression itself, mock.VerifySet(m => m.Value = 25);", true)]
		public static void VerifySet<T, TProperty>(this Mock<T> mock, Expression<Func<T, TProperty>> expression, TProperty value)
			where T : class
		{
			throw new NotSupportedException();
		}

		/// <summary>
		/// Obsolete.
		/// </summary>
		[EditorBrowsable(EditorBrowsableState.Never)]
		[Obsolete("Use the new syntax, which allows you to pass the value in the expression itself, mock.VerifySet(m => m.Value = 25, failMessage);", true)]
		public static void VerifySet<T, TProperty>(this Mock<T> mock, Expression<Func<T, TProperty>> expression, TProperty value, string failMessage)
			where T : class
		{
			throw new NotSupportedException();
		}
	}
}
