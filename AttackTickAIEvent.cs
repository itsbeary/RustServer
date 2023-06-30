using System;

// Token: 0x0200036F RID: 879
public class AttackTickAIEvent : BaseAIEvent
{
	// Token: 0x06001FE5 RID: 8165 RVA: 0x000D6B3C File Offset: 0x000D4D3C
	public AttackTickAIEvent()
		: base(AIEventType.AttackTick)
	{
		base.Rate = BaseAIEvent.ExecuteRate.VeryFast;
	}

	// Token: 0x06001FE6 RID: 8166 RVA: 0x000D6B50 File Offset: 0x000D4D50
	public override void Execute(AIMemory memory, AIBrainSenses senses, StateStatus stateStatus)
	{
		base.Result = base.Inverted;
		IAIAttack iaiattack = base.Owner as IAIAttack;
		if (iaiattack == null)
		{
			return;
		}
		BaseEntity baseEntity = memory.Entity.Get(base.InputEntityMemorySlot);
		iaiattack.AttackTick(this.deltaTime, baseEntity, senses.Memory.IsLOS(baseEntity));
		base.Result = !base.Inverted;
	}
}
