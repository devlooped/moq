// Copyright (c) 2007, Clarius Consulting, Manas Technology Solutions, InSTEDD.
// All rights reserved. Licensed under the BSD 3-Clause License; see License.txt.

using System.Text;

using Xunit;

namespace Moq.Tests
{
	public class StringBuilderExtensionsFixture
	{
		[Theory]
		[InlineData(nameof(IMethods.Empty), "")]
		[InlineData(nameof(IMethods.Int), "int")]
		[InlineData(nameof(IMethods.IntAndString), "int, string")]
		[InlineData(nameof(IMethods.InInt), "in int")]
		[InlineData(nameof(IMethods.RefInt), "ref int")]
		[InlineData(nameof(IMethods.OutInt), "out int")]
		[InlineData(nameof(IMethods.BoolAndParamsString), "bool, params string[]")]
		public void AppendParameterList_formats_parameter_lists_correctly(string methodName, string expected)
		{
			var actual = GetFormattedParameterListOf(methodName);
			Assert.Equal(expected, actual);
		}

		private string GetFormattedParameterListOf(string methodName)
		{
			var stringBuilder = new StringBuilder();
			var method = typeof(IMethods).GetMethod(methodName);
			stringBuilder.AppendParameterTypeList(method.GetParameters());
			return stringBuilder.ToString();
		}

		public interface IMethods
		{
			void Empty();
			void Int(int arg1);
			void IntAndString(int arg1, string arg2);
			void InInt(in int arg1);
			void RefInt(ref int arg1);
			void OutInt(out int arg1);
			void BoolAndParamsString(bool arg1, params string[] arg2);
		}
	}
}
