using System;

// Token: 0x02000383 RID: 899
public class StateFinishedAIEvent : BaseAIEvent
{
	// Token: 0x06002045 RID: 8261 RVA: 0x000D7A76 File Offset: 0x000D5C76
	public StateFinishedAIEvent()
		: base(AIEventType.StateFinished)
	{
		base.Rate = BaseAIEvent.ExecuteRate.Fast;
	}

	// Token: 0x06002046 RID: 8262 RVA: 0x000D7A86 File Offset: 0x000D5C86
	public override void Execute(AIMemory memory, AIBrainSenses senses, StateStatus stateStatus)
	{
		base.Result = base.Inverted;
		if (stateStatus == StateStatus.Finished)
		{
			base.Result = !base.Inverted;
		}
	}
}
