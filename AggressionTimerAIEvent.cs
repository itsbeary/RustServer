using System;
using ProtoBuf;

// Token: 0x0200038F RID: 911
public class AggressionTimerAIEvent : BaseAIEvent
{
	// Token: 0x170002AE RID: 686
	// (get) Token: 0x0600207C RID: 8316 RVA: 0x000D819F File Offset: 0x000D639F
	// (set) Token: 0x0600207D RID: 8317 RVA: 0x000D81A7 File Offset: 0x000D63A7
	public float Value { get; private set; }

	// Token: 0x0600207E RID: 8318 RVA: 0x000D81B0 File Offset: 0x000D63B0
	public AggressionTimerAIEvent()
		: base(AIEventType.AggressionTimer)
	{
		base.Rate = BaseAIEvent.ExecuteRate.Fast;
	}

	// Token: 0x0600207F RID: 8319 RVA: 0x000D81C4 File Offset: 0x000D63C4
	public override void Init(AIEventData data, global::BaseEntity owner)
	{
		base.Init(data, owner);
		AggressionTimerAIEventData aggressionTimerData = data.aggressionTimerData;
		this.Value = aggressionTimerData.value;
	}

	// Token: 0x06002080 RID: 8320 RVA: 0x000D81EC File Offset: 0x000D63EC
	public override AIEventData ToProto()
	{
		AIEventData aieventData = base.ToProto();
		aieventData.aggressionTimerData = new AggressionTimerAIEventData();
		aieventData.aggressionTimerData.value = this.Value;
		return aieventData;
	}

	// Token: 0x06002081 RID: 8321 RVA: 0x000D8210 File Offset: 0x000D6410
	public override void Execute(AIMemory memory, AIBrainSenses senses, StateStatus stateStatus)
	{
		base.Result = base.Inverted;
		if (base.Inverted)
		{
			base.Result = senses.TimeInAgressiveState < this.Value;
			return;
		}
		base.Result = senses.TimeInAgressiveState >= this.Value;
	}
}
