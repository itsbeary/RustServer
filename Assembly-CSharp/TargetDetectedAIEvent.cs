using System;
using ProtoBuf;

// Token: 0x02000384 RID: 900
public class TargetDetectedAIEvent : BaseAIEvent
{
	// Token: 0x170002A4 RID: 676
	// (get) Token: 0x06002047 RID: 8263 RVA: 0x000D7AA7 File Offset: 0x000D5CA7
	// (set) Token: 0x06002048 RID: 8264 RVA: 0x000D7AAF File Offset: 0x000D5CAF
	public float Range { get; set; }

	// Token: 0x06002049 RID: 8265 RVA: 0x000D7AB8 File Offset: 0x000D5CB8
	public TargetDetectedAIEvent()
		: base(AIEventType.TargetDetected)
	{
		base.Rate = BaseAIEvent.ExecuteRate.Slow;
	}

	// Token: 0x0600204A RID: 8266 RVA: 0x000D7ACC File Offset: 0x000D5CCC
	public override void Init(AIEventData data, global::BaseEntity owner)
	{
		base.Init(data, owner);
		TargetDetectedAIEventData targetDetectedData = data.targetDetectedData;
		this.Range = targetDetectedData.range;
	}

	// Token: 0x0600204B RID: 8267 RVA: 0x000D7AF4 File Offset: 0x000D5CF4
	public override AIEventData ToProto()
	{
		AIEventData aieventData = base.ToProto();
		aieventData.targetDetectedData = new TargetDetectedAIEventData();
		aieventData.targetDetectedData.range = this.Range;
		return aieventData;
	}

	// Token: 0x0600204C RID: 8268 RVA: 0x000D7B18 File Offset: 0x000D5D18
	public override void Execute(AIMemory memory, AIBrainSenses senses, StateStatus stateStatus)
	{
		base.Result = base.Inverted;
		global::BaseEntity nearestTarget = senses.GetNearestTarget(this.Range);
		if (base.Inverted)
		{
			if (nearestTarget == null && base.ShouldSetOutputEntityMemory)
			{
				memory.Entity.Remove(base.OutputEntityMemorySlot);
			}
			base.Result = nearestTarget == null;
			return;
		}
		if (nearestTarget != null && base.ShouldSetOutputEntityMemory)
		{
			memory.Entity.Set(nearestTarget, base.OutputEntityMemorySlot);
		}
		base.Result = nearestTarget != null;
	}
}
