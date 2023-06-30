using System;

// Token: 0x0200037C RID: 892
public class IsVisibleAIEvent : BaseAIEvent
{
	// Token: 0x06002032 RID: 8242 RVA: 0x000D7603 File Offset: 0x000D5803
	public IsVisibleAIEvent()
		: base(AIEventType.IsVisible)
	{
		base.Rate = BaseAIEvent.ExecuteRate.Fast;
	}

	// Token: 0x06002033 RID: 8243 RVA: 0x000D7614 File Offset: 0x000D5814
	public override void Execute(AIMemory memory, AIBrainSenses senses, StateStatus stateStatus)
	{
		base.Result = false;
		BaseEntity baseEntity = memory.Entity.Get(base.InputEntityMemorySlot);
		if (baseEntity == null)
		{
			return;
		}
		if (!(base.Owner is IAIAttack))
		{
			return;
		}
		bool flag = senses.Memory.IsLOS(baseEntity);
		base.Result = (base.Inverted ? (!flag) : flag);
	}
}
