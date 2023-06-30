using System;
using UnityEngine;

// Token: 0x0200037F RID: 895
public class PerformedAttackAIEvent : BaseAIEvent
{
	// Token: 0x06002038 RID: 8248 RVA: 0x000D7760 File Offset: 0x000D5960
	public PerformedAttackAIEvent()
		: base(AIEventType.PerformedAttack)
	{
		base.Rate = BaseAIEvent.ExecuteRate.Fast;
	}

	// Token: 0x06002039 RID: 8249 RVA: 0x000D777B File Offset: 0x000D597B
	public override void Reset()
	{
		base.Reset();
		this.lastExecuteTime = Time.time;
	}

	// Token: 0x0600203A RID: 8250 RVA: 0x000D7790 File Offset: 0x000D5990
	public override void Execute(AIMemory memory, AIBrainSenses senses, StateStatus stateStatus)
	{
		base.Result = false;
		this.combatEntity = memory.Entity.Get(base.InputEntityMemorySlot) as BaseCombatEntity;
		float num = this.lastExecuteTime;
		this.lastExecuteTime = Time.time;
		if (this.combatEntity == null)
		{
			return;
		}
		if (this.combatEntity.lastDealtDamageTime < num)
		{
			base.Result = base.Inverted;
			return;
		}
		if (this.combatEntity.lastDealtDamageTo == null)
		{
			return;
		}
		if (this.combatEntity.lastDealtDamageTo == this.combatEntity)
		{
			return;
		}
		BasePlayer basePlayer = this.combatEntity as BasePlayer;
		if (basePlayer != null)
		{
			if (basePlayer == memory.Entity.Get(5) && basePlayer.lastDealtDamageTo == base.Owner)
			{
				return;
			}
			if (basePlayer == memory.Entity.Get(5) && (basePlayer.lastDealtDamageTo.gameObject.layer == 21 || basePlayer.lastDealtDamageTo.gameObject.layer == 8))
			{
				return;
			}
		}
		if (base.ShouldSetOutputEntityMemory)
		{
			memory.Entity.Set(this.combatEntity.lastDealtDamageTo, base.OutputEntityMemorySlot);
		}
		base.Result = !base.Inverted;
	}

	// Token: 0x0400195D RID: 6493
	protected float lastExecuteTime = float.NegativeInfinity;

	// Token: 0x0400195E RID: 6494
	private BaseCombatEntity combatEntity;
}
