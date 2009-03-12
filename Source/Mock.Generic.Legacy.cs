using System;
using System.ComponentModel;
using System.Linq.Expressions;
using Moq.Language.Flow;

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
			return Mock.SetupSet(this, expression);
		}

		/// <summary>
		/// Obsolete.
		/// </summary>
		[Obsolete("ExpectSet has been renamed to SetupSet, and the new syntax allows you to pass the value in the expression itself, like f => f.Value = 25.", true)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public ISetupSetter<T, TProperty> ExpectSet<TProperty>(Expression<Func<T, TProperty>> expression, TProperty value)
		{
			return Mock.SetupSet(this, expression, value);
		}
	}

	/// <summary>
	/// Provides legacy API members as extensions so that 
	/// existing code continues to compile, but new code 
	/// doesn't see then.
	/// </summary>
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
			return Mock.SetupSet(mock, expression, value);
		}

		/// <summary>
		/// Obsolete.
		/// </summary>
		[EditorBrowsable(EditorBrowsableState.Never)]
		[Obsolete("Use the new syntax, which allows you to pass the value in the expression itself, mock.VerifySet(m => m.Value = 25);", true)]
		public static void VerifySet<T, TProperty>(this Mock<T> mock, Expression<Func<T, TProperty>> expression, TProperty value)
			where T : class
		{
			Mock.VerifySet(mock, expression, value, Times.AtLeastOnce(), null);
		}

		/// <summary>
		/// Obsolete.
		/// </summary>
		[EditorBrowsable(EditorBrowsableState.Never)]
		[Obsolete("Use the new syntax, which allows you to pass the value in the expression itself, mock.VerifySet(m => m.Value = 25, failMessage);", true)]
		public static void VerifySet<T, TProperty>(this Mock<T> mock, Expression<Func<T, TProperty>> expression, TProperty value, string failMessage)
			where T : class
		{
			Mock.VerifySet(mock, expression, value, Times.AtLeastOnce(), failMessage);
		}
	}
}