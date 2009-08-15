using System;
using System.Collections.Generic;
using System.Linq;
using Moq.Proxy;
using Moq.Visualizer.Properties;

namespace Moq.Visualizer
{
	[Serializable]
	public class MockContextViewModel
	{
		internal MockContextViewModel(Mock mock)
		{
			this.Behavior = mock.Behavior;
			this.DefaultValue = mock.DefaultValue;
			this.CallBase = mock.CallBase;
			this.Mocks = new[] { GetMock(mock) };
		}

		public MockBehavior Behavior { get; private set; }

		public bool CallBase { get; private set; }

		public DefaultValue DefaultValue { get; private set; }

		public IEnumerable<MockViewModel> Mocks { get; private set; }

		private static ContainerViewModel CreateExpandedContainer<T>(string name, IEnumerable<T> children)
		{
			return new ContainerViewModel<T>(name, children.ToArray()) { IsExpanded = true };
		}

		private static MockViewModel GetMock(Mock mock)
		{
			var actualCalls = mock.Interceptor.ActualCalls.ToDictionary(ac => ac, i => GetCall(i));

			var setups = mock.Interceptor.OrderedCalls.Select(s => GetSetup(s, actualCalls))
				.OrderBy(s => s.SetupExpression);
			var calls = actualCalls.Values.Where(c => !c.HasSetup);
			var innerMocks = mock.InnerMocks.Values.Select(m => GetMock(m));

			return new MockViewModel(
				mock.MockedType,
				CreateExpandedContainer<SetupViewModel>(Resources.SetupsContainerName, setups),
				CreateExpandedContainer<CallViewModel>(Resources.OtherCallsContainerName, calls),
				CreateExpandedContainer<MockViewModel>(Resources.MocksContainerName, innerMocks))
			{
				IsExpanded = true
			};
		}

		private static CallViewModel GetCall(ICallContext callContext)
		{
			return new CallViewModel(callContext.Method, callContext.Arguments, callContext.ReturnValue);
		}

		private static SetupViewModel GetSetup(
			IProxyCall proxyCall,
			IDictionary<ICallContext, CallViewModel> actualCalls)
		{
			if (proxyCall.Invoked)
			{
				var setupCalls = actualCalls.Keys.Where(ac => proxyCall.Matches(ac))
					.Select(ac => GetSetupCall(actualCalls[ac]));
				return new SetupViewModel(
					proxyCall.SetupExpression.ToStringFixed(),
					proxyCall.IsVerifiable,
					proxyCall.IsNever,
					CreateExpandedContainer<CallViewModel>("Invocations", setupCalls));
			}

			return new SetupViewModel(
					proxyCall.SetupExpression.ToStringFixed(),
					proxyCall.IsVerifiable,
					proxyCall.IsNever);
		}

		private static CallViewModel GetSetupCall(CallViewModel call)
		{
			call.HasSetup = true;
			return call;
		}
	}
}