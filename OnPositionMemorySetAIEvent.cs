using System;

// Token: 0x0200037E RID: 894
public class OnPositionMemorySetAIEvent : BaseAIEvent
{
	// Token: 0x06002036 RID: 8246 RVA: 0x000D7717 File Offset: 0x000D5917
	public OnPositionMemorySetAIEvent()
		: base(AIEventType.OnPositionMemorySet)
	{
		base.Rate = BaseAIEvent.ExecuteRate.Fast;
	}

	// Token: 0x06002037 RID: 8247 RVA: 0x000D7728 File Offset: 0x000D5928
	public override void Execute(AIMemory memory, AIBrainSenses senses, StateStatus stateStatus)
	{
		base.Result = false;
		if (memory.Position.GetTimeSinceSet(5) <= 0.5f)
		{
			base.Result = !base.Inverted;
			return;
		}
		base.Result = base.Inverted;
	}
}
