using System;

// Token: 0x02000382 RID: 898
public class StateErrorAIEvent : BaseAIEvent
{
	// Token: 0x06002043 RID: 8259 RVA: 0x000D7A35 File Offset: 0x000D5C35
	public StateErrorAIEvent()
		: base(AIEventType.StateError)
	{
		base.Rate = BaseAIEvent.ExecuteRate.Fast;
	}

	// Token: 0x06002044 RID: 8260 RVA: 0x000D7A45 File Offset: 0x000D5C45
	public override void Execute(AIMemory memory, AIBrainSenses senses, StateStatus stateStatus)
	{
		base.Result = base.Inverted;
		if (stateStatus == StateStatus.Error)
		{
			base.Result = !base.Inverted;
			return;
		}
		if (stateStatus == StateStatus.Running)
		{
			base.Result = base.Inverted;
		}
	}
}
