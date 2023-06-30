using System;
using ProtoBuf;

// Token: 0x02000386 RID: 902
public class ThreatDetectedAIEvent : BaseAIEvent
{
	// Token: 0x170002A6 RID: 678
	// (get) Token: 0x06002051 RID: 8273 RVA: 0x000D7CA2 File Offset: 0x000D5EA2
	// (set) Token: 0x06002052 RID: 8274 RVA: 0x000D7CAA File Offset: 0x000D5EAA
	public float Range { get; set; }

	// Token: 0x06002053 RID: 8275 RVA: 0x000D7CB3 File Offset: 0x000D5EB3
	public ThreatDetectedAIEvent()
		: base(AIEventType.ThreatDetected)
	{
		base.Rate = BaseAIEvent.ExecuteRate.Slow;
	}

	// Token: 0x06002054 RID: 8276 RVA: 0x000D7CC4 File Offset: 0x000D5EC4
	public override void Init(AIEventData data, global::BaseEntity owner)
	{
		base.Init(data, owner);
		ThreatDetectedAIEventData threatDetectedData = data.threatDetectedData;
		this.Range = threatDetectedData.range;
	}

	// Token: 0x06002055 RID: 8277 RVA: 0x000D7CEC File Offset: 0x000D5EEC
	public override AIEventData ToProto()
	{
		AIEventData aieventData = base.ToProto();
		aieventData.threatDetectedData = new ThreatDetectedAIEventData();
		aieventData.threatDetectedData.range = this.Range;
		return aieventData;
	}

	// Token: 0x06002056 RID: 8278 RVA: 0x000D7D10 File Offset: 0x000D5F10
	public override void Execute(AIMemory memory, AIBrainSenses senses, StateStatus stateStatus)
	{
		base.Result = base.Inverted;
		global::BaseEntity nearestThreat = senses.GetNearestThreat(this.Range);
		if (base.Inverted)
		{
			if (nearestThreat == null && base.ShouldSetOutputEntityMemory)
			{
				memory.Entity.Remove(base.OutputEntityMemorySlot);
			}
			base.Result = nearestThreat == null;
			return;
		}
		if (nearestThreat != null && base.ShouldSetOutputEntityMemory)
		{
			memory.Entity.Set(nearestThreat, base.OutputEntityMemorySlot);
		}
		base.Result = nearestThreat != null;
	}
}
