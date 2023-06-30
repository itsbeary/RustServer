using System;
using ProtoBuf;

// Token: 0x02000372 RID: 882
public class BestTargetDetectedAIEvent : BaseAIEvent
{
	// Token: 0x0600200B RID: 8203 RVA: 0x000D70A9 File Offset: 0x000D52A9
	public BestTargetDetectedAIEvent()
		: base(AIEventType.BestTargetDetected)
	{
		base.Rate = BaseAIEvent.ExecuteRate.Normal;
	}

	// Token: 0x0600200C RID: 8204 RVA: 0x000D70BA File Offset: 0x000D52BA
	public override void Init(AIEventData data, global::BaseEntity owner)
	{
		base.Init(data, owner);
	}

	// Token: 0x0600200D RID: 8205 RVA: 0x000D70C4 File Offset: 0x000D52C4
	public override void Execute(AIMemory memory, AIBrainSenses senses, StateStatus stateStatus)
	{
		base.Result = base.Inverted;
		IAIAttack iaiattack = base.Owner as IAIAttack;
		if (iaiattack == null)
		{
			return;
		}
		global::BaseEntity bestTarget = iaiattack.GetBestTarget();
		if (base.Inverted)
		{
			if (bestTarget == null && base.ShouldSetOutputEntityMemory)
			{
				memory.Entity.Remove(base.OutputEntityMemorySlot);
			}
			base.Result = bestTarget == null;
			return;
		}
		if (bestTarget != null && base.ShouldSetOutputEntityMemory)
		{
			memory.Entity.Set(bestTarget, base.OutputEntityMemorySlot);
		}
		base.Result = bestTarget != null;
	}
}
