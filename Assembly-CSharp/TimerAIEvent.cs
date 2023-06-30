using System;
using ProtoBuf;
using UnityEngine;

// Token: 0x02000388 RID: 904
public class TimerAIEvent : BaseAIEvent
{
	// Token: 0x170002A8 RID: 680
	// (get) Token: 0x0600205D RID: 8285 RVA: 0x000D7E59 File Offset: 0x000D6059
	// (set) Token: 0x0600205E RID: 8286 RVA: 0x000D7E61 File Offset: 0x000D6061
	public float DurationMin { get; set; }

	// Token: 0x170002A9 RID: 681
	// (get) Token: 0x0600205F RID: 8287 RVA: 0x000D7E6A File Offset: 0x000D606A
	// (set) Token: 0x06002060 RID: 8288 RVA: 0x000D7E72 File Offset: 0x000D6072
	public float DurationMax { get; set; }

	// Token: 0x06002061 RID: 8289 RVA: 0x000D7E7B File Offset: 0x000D607B
	public TimerAIEvent()
		: base(AIEventType.Timer)
	{
		base.Rate = BaseAIEvent.ExecuteRate.Fast;
	}

	// Token: 0x06002062 RID: 8290 RVA: 0x000D7E8C File Offset: 0x000D608C
	public override void Init(AIEventData data, global::BaseEntity owner)
	{
		base.Init(data, owner);
		TimerAIEventData timerData = data.timerData;
		this.DurationMin = timerData.duration;
		this.DurationMax = timerData.durationMax;
	}

	// Token: 0x06002063 RID: 8291 RVA: 0x000D7EC0 File Offset: 0x000D60C0
	public override AIEventData ToProto()
	{
		AIEventData aieventData = base.ToProto();
		aieventData.timerData = new TimerAIEventData();
		aieventData.timerData.duration = this.DurationMin;
		aieventData.timerData.durationMax = this.DurationMax;
		return aieventData;
	}

	// Token: 0x06002064 RID: 8292 RVA: 0x000D7EF5 File Offset: 0x000D60F5
	public override void Reset()
	{
		base.Reset();
		this.currentDuration = UnityEngine.Random.Range(this.DurationMin, this.DurationMax);
		this.elapsedDuration = 0f;
	}

	// Token: 0x06002065 RID: 8293 RVA: 0x000D7F1F File Offset: 0x000D611F
	public override void Execute(AIMemory memory, AIBrainSenses senses, StateStatus stateStatus)
	{
		base.Result = base.Inverted;
		this.elapsedDuration += this.deltaTime;
		if (this.elapsedDuration >= this.currentDuration)
		{
			base.Result = !base.Inverted;
		}
	}

	// Token: 0x04001966 RID: 6502
	protected float currentDuration;

	// Token: 0x04001967 RID: 6503
	protected float elapsedDuration;
}
