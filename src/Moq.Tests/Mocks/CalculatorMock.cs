using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading;
using Moq.Sdk;
using Sample;
using Avatars;

namespace Mocks
{
    public partial class CalculatorMock : Calculator, IMocked, IAvatar
    {
        BehaviorPipeline pipeline = new BehaviorPipeline();

        public IList<IAvatarBehavior> Behaviors => pipeline.Behaviors;

        public override event EventHandler TurnedOn
        {
            add => pipeline.Execute(MethodInvocation.Create(this, MethodBase.GetCurrentMethod(), (m, n) => { base.TurnedOn += value; return m.CreateReturn(); }, value));
            remove => pipeline.Execute(MethodInvocation.Create(this, MethodBase.GetCurrentMethod(), (m, n) => { base.TurnedOn -= value; return m.CreateReturn(); }, value));
        }

        public override CalculatorMode Mode
        {
            get => pipeline.Execute<CalculatorMode>(MethodInvocation.Create(this, MethodBase.GetCurrentMethod(), (m, n) => m.CreateValueReturn(base.Mode)));
            set => pipeline.Execute(MethodInvocation.Create(this, MethodBase.GetCurrentMethod(), (m, n) => { base.Mode = value; return m.CreateReturn(); }, value));
        }

        public override int? this[string name]
        {
            get => pipeline.Execute<int?>(MethodInvocation.Create(this, MethodBase.GetCurrentMethod(), (m, n) => m.CreateValueReturn(base[name]), name));
            set => pipeline.Execute(MethodInvocation.Create(this, MethodBase.GetCurrentMethod(), (m, n) => { base[name] = value; return m.CreateReturn(); }, name, value));
        }

        public override bool IsOn => pipeline.Execute<bool>(MethodInvocation.Create(this, MethodBase.GetCurrentMethod(), (m, n) => m.CreateValueReturn(base.IsOn)));

        public override int Add(int x, int y) =>
            pipeline.Execute<int>(MethodInvocation.Create(this, MethodBase.GetCurrentMethod(), (m, n) => m.CreateValueReturn(base.Add(x, y)), x, y));

        public override int Add(int x, int y, int z) =>
            pipeline.Execute<int>(MethodInvocation.Create(this, MethodBase.GetCurrentMethod(), (m, n) => m.CreateValueReturn(base.Add(x, y, z)), x, y, z));

        public override bool TryAdd(ref int x, ref int y, out int? z)
        {
            z = default;
            var local_x = x;
            var local_y = y;
            var local_z = z;

            var result = pipeline.Invoke(MethodInvocation.Create(this, MethodBase.GetCurrentMethod(),
                (m, n) => m.CreateValueReturn(base.TryAdd(ref local_x, ref local_y, out local_z), local_x, local_y, local_z),
                x, y, z), true);

            x = result.Outputs.Get<int>("x");
            y = result.Outputs.Get<int>("y");
            z = result.Outputs.GetNullable<int>("z");

            return (bool)result.ReturnValue;
        }

        public override void TurnOn() =>
            pipeline.Execute(MethodInvocation.Create(this, MethodBase.GetCurrentMethod(), (m, n) => { base.TurnOn(); return m.CreateReturn(); }));

        public override void Store(string name, int value) =>
            pipeline.Execute(MethodInvocation.Create(this, MethodBase.GetCurrentMethod(), (m, n) => { base.Store(name, value); return m.CreateReturn(); }, name, value));

        public override int? Recall(string name) =>
            pipeline.Execute<int?>(MethodInvocation.Create(this, MethodBase.GetCurrentMethod(), (m, n) => m.CreateValueReturn(base.Recall(name)), name));

        public override void Clear(string name) =>
            pipeline.Execute(MethodInvocation.Create(this, MethodBase.GetCurrentMethod(), (m, n) => { base.Clear(name); return m.CreateReturn(); }, name));

        public override ICalculatorMemory Memory
        {
            get => pipeline.Execute<ICalculatorMemory>(MethodInvocation.Create(this, MethodBase.GetCurrentMethod(), (m, n) => m.CreateValueReturn(base.Memory)));
        }

        #region IMocked
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        IMock mock;

        [DebuggerDisplay("Invocations = {Invocations.Count}", Name = nameof(IMocked.Mock))]
        [DebuggerBrowsable(DebuggerBrowsableState.Collapsed)]
        [CompilerGenerated]
        IMock IMocked.Mock => LazyInitializer.EnsureInitialized(ref mock, () => new DefaultMock(this));
        #endregion
    }
}
