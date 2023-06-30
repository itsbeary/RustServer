using System;

// Token: 0x0200036E RID: 878
public class AndAIEvent : BaseAIEvent
{
	// Token: 0x06001FE3 RID: 8163 RVA: 0x000D6B22 File Offset: 0x000D4D22
	public AndAIEvent()
		: base(AIEventType.And)
	{
		base.Rate = BaseAIEvent.ExecuteRate.Normal;
	}

	// Token: 0x06001FE4 RID: 8164 RVA: 0x000D6B33 File Offset: 0x000D4D33
	public override void Execute(AIMemory memory, AIBrainSenses senses, StateStatus stateStatus)
	{
		base.Result = false;
	}
}
