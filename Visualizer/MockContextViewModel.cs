using System;
using System.Collections.Generic;
using System.Linq;
using Moq.Proxy;

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
			this.SetMocks(GetMock(mock));
		}

		public MockBehavior Behavior { get; private set; }

		public bool CallBase { get; private set; }

		public DefaultValue DefaultValue { get; private set; }

		public IEnumerable<MockViewModel> Mocks { get; private set; }

		private void SetMocks(params MockViewModel[] mocks)
		{
			this.Mocks = mocks;
		}

		private static MockViewModel GetMock(Mock mock)
		{
			var actualCalls = mock.Interceptor.ActualCalls.ToDictionary(ac => ac, i => GetCall(i));

			var setups = mock.Interceptor.OrderedCalls.Select(s => GetSetup(s, actualCalls))
				.OrderBy(s => s.SetupExpression).ToArray();
			var calls = actualCalls.Values.Where(c => !c.HasSetup).ToArray();
			var innerMocks = mock.InnerMocks.Values.Select(m => GetMock(m)).ToArray();

			var setup = new ContainerViewModel<SetupViewModel>("Setups", setups) { IsExpanded = true };
			var call = new ContainerViewModel<CallViewModel>("Invocations without setup", calls) { IsExpanded = true };
			var im = new ContainerViewModel<MockViewModel>("Inner Mocks", innerMocks) { IsExpanded = false };

			return new MockViewModel(mock.MockedType, setup, call, im) { IsExpanded = true };
		}

		private static CallViewModel GetCall(ICallContext callContext)
		{
			return new CallViewModel(callContext.Method, callContext.Arguments, callContext.ReturnValue);
		}

		private static SetupViewModel GetSetup(IProxyCall proxyCall, IDictionary<ICallContext, CallViewModel> actualCalls)
		{
			if (proxyCall.Invoked)
			{
				var setupCalls = actualCalls.Keys.Where(ac => proxyCall.Matches(ac))
					.Select(ac => GetSetupCall(actualCalls[ac]));
				return new SetupViewModel(
					proxyCall.SetupExpression.ToStringFixed(),
					proxyCall.IsVerifiable,
					proxyCall.IsNever,
					new ContainerViewModel<CallViewModel>("Invocations", setupCalls.ToArray()) { IsExpanded = true });
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