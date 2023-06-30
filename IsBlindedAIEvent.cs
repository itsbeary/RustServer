using System;

// Token: 0x0200037B RID: 891
public class IsBlindedAIEvent : BaseAIEvent
{
	// Token: 0x06002030 RID: 8240 RVA: 0x000D75BC File Offset: 0x000D57BC
	public IsBlindedAIEvent()
		: base(AIEventType.IsBlinded)
	{
		base.Rate = BaseAIEvent.ExecuteRate.Fast;
	}

	// Token: 0x06002031 RID: 8241 RVA: 0x000D75D0 File Offset: 0x000D57D0
	public override void Execute(AIMemory memory, AIBrainSenses senses, StateStatus stateStatus)
	{
		bool flag = senses.brain.Blinded();
		if (base.Inverted)
		{
			base.Result = !flag;
			return;
		}
		base.Result = flag;
	}
}
