using System;
using UnityEngine;

// Token: 0x02000370 RID: 880
public class AttackedAIEvent : BaseAIEvent
{
	// Token: 0x06001FE7 RID: 8167 RVA: 0x000D6BB3 File Offset: 0x000D4DB3
	public AttackedAIEvent()
		: base(AIEventType.Attacked)
	{
		base.Rate = BaseAIEvent.ExecuteRate.Fast;
	}

	// Token: 0x06001FE8 RID: 8168 RVA: 0x000D6BCE File Offset: 0x000D4DCE
	public override void Reset()
	{
		base.Reset();
		this.lastExecuteTime = Time.time;
	}

	// Token: 0x06001FE9 RID: 8169 RVA: 0x000D6BE4 File Offset: 0x000D4DE4
	public override void Execute(AIMemory memory, AIBrainSenses senses, StateStatus stateStatus)
	{
		base.Result = base.Inverted;
		this.combatEntity = memory.Entity.Get(base.InputEntityMemorySlot) as BaseCombatEntity;
		float num = this.lastExecuteTime;
		this.lastExecuteTime = Time.time;
		if (this.combatEntity == null)
		{
			return;
		}
		if (this.combatEntity.lastAttackedTime >= num)
		{
			if (this.combatEntity.lastAttacker == null)
			{
				return;
			}
			if (this.combatEntity.lastAttacker == this.combatEntity)
			{
				return;
			}
			BasePlayer basePlayer = this.combatEntity.lastAttacker as BasePlayer;
			if (basePlayer != null && basePlayer == memory.Entity.Get(5) && basePlayer.lastDealtDamageTo == base.Owner)
			{
				return;
			}
			if (base.ShouldSetOutputEntityMemory)
			{
				memory.Entity.Set(this.combatEntity.lastAttacker, base.OutputEntityMemorySlot);
			}
			base.Result = !base.Inverted;
		}
	}

	// Token: 0x04001949 RID: 6473
	protected float lastExecuteTime = float.NegativeInfinity;

	// Token: 0x0400194A RID: 6474
	private BaseCombatEntity combatEntity;
}
