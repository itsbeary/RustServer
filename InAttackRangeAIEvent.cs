using System;

// Token: 0x02000378 RID: 888
public class InAttackRangeAIEvent : BaseAIEvent
{
	// Token: 0x06002022 RID: 8226 RVA: 0x000D73A7 File Offset: 0x000D55A7
	public InAttackRangeAIEvent()
		: base(AIEventType.InAttackRange)
	{
		base.Rate = BaseAIEvent.ExecuteRate.Fast;
	}

	// Token: 0x06002023 RID: 8227 RVA: 0x000D73B8 File Offset: 0x000D55B8
	public override void Execute(AIMemory memory, AIBrainSenses senses, StateStatus stateStatus)
	{
		BaseEntity baseEntity = memory.Entity.Get(base.InputEntityMemorySlot);
		base.Result = false;
		if (baseEntity == null)
		{
			return;
		}
		IAIAttack iaiattack = base.Owner as IAIAttack;
		if (iaiattack == null)
		{
			return;
		}
		float num;
		bool flag = iaiattack.IsTargetInRange(baseEntity, out num);
		base.Result = (base.Inverted ? (!flag) : flag);
	}
}
