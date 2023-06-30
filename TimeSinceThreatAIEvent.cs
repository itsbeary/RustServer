using System;
using ProtoBuf;

// Token: 0x02000387 RID: 903
public class TimeSinceThreatAIEvent : BaseAIEvent
{
	// Token: 0x170002A7 RID: 679
	// (get) Token: 0x06002057 RID: 8279 RVA: 0x000D7D9E File Offset: 0x000D5F9E
	// (set) Token: 0x06002058 RID: 8280 RVA: 0x000D7DA6 File Offset: 0x000D5FA6
	public float Value { get; private set; }

	// Token: 0x06002059 RID: 8281 RVA: 0x000D7DAF File Offset: 0x000D5FAF
	public TimeSinceThreatAIEvent()
		: base(AIEventType.TimeSinceThreat)
	{
		base.Rate = BaseAIEvent.ExecuteRate.Fast;
	}

	// Token: 0x0600205A RID: 8282 RVA: 0x000D7DC0 File Offset: 0x000D5FC0
	public override void Init(AIEventData data, global::BaseEntity owner)
	{
		base.Init(data, owner);
		TimeSinceThreatAIEventData timeSinceThreatData = data.timeSinceThreatData;
		this.Value = timeSinceThreatData.value;
	}

	// Token: 0x0600205B RID: 8283 RVA: 0x000D7DE8 File Offset: 0x000D5FE8
	public override AIEventData ToProto()
	{
		AIEventData aieventData = base.ToProto();
		aieventData.timeSinceThreatData = new TimeSinceThreatAIEventData();
		aieventData.timeSinceThreatData.value = this.Value;
		return aieventData;
	}

	// Token: 0x0600205C RID: 8284 RVA: 0x000D7E0C File Offset: 0x000D600C
	public override void Execute(AIMemory memory, AIBrainSenses senses, StateStatus stateStatus)
	{
		base.Result = base.Inverted;
		if (base.Inverted)
		{
			base.Result = senses.TimeSinceThreat < this.Value;
			return;
		}
		base.Result = senses.TimeSinceThreat >= this.Value;
	}
}
