using System;
using ProtoBuf;

// Token: 0x02000380 RID: 896
public class PlayerDetectedAIEvent : BaseAIEvent
{
	// Token: 0x170002A3 RID: 675
	// (get) Token: 0x0600203B RID: 8251 RVA: 0x000D78D8 File Offset: 0x000D5AD8
	// (set) Token: 0x0600203C RID: 8252 RVA: 0x000D78E0 File Offset: 0x000D5AE0
	public float Range { get; set; }

	// Token: 0x0600203D RID: 8253 RVA: 0x000D78E9 File Offset: 0x000D5AE9
	public PlayerDetectedAIEvent()
		: base(AIEventType.PlayerDetected)
	{
		base.Rate = BaseAIEvent.ExecuteRate.Slow;
	}

	// Token: 0x0600203E RID: 8254 RVA: 0x000D78FC File Offset: 0x000D5AFC
	public override void Init(AIEventData data, global::BaseEntity owner)
	{
		base.Init(data, owner);
		PlayerDetectedAIEventData playerDetectedData = data.playerDetectedData;
		this.Range = playerDetectedData.range;
	}

	// Token: 0x0600203F RID: 8255 RVA: 0x000D7924 File Offset: 0x000D5B24
	public override AIEventData ToProto()
	{
		AIEventData aieventData = base.ToProto();
		aieventData.playerDetectedData = new PlayerDetectedAIEventData();
		aieventData.playerDetectedData.range = this.Range;
		return aieventData;
	}

	// Token: 0x06002040 RID: 8256 RVA: 0x000D7948 File Offset: 0x000D5B48
	public override void Execute(AIMemory memory, AIBrainSenses senses, StateStatus stateStatus)
	{
		base.Result = false;
		global::BaseEntity nearestPlayer = senses.GetNearestPlayer(this.Range);
		if (base.Inverted)
		{
			if (nearestPlayer == null && base.ShouldSetOutputEntityMemory)
			{
				memory.Entity.Remove(base.OutputEntityMemorySlot);
			}
			base.Result = nearestPlayer == null;
			return;
		}
		if (nearestPlayer != null && base.ShouldSetOutputEntityMemory)
		{
			memory.Entity.Set(nearestPlayer, base.OutputEntityMemorySlot);
		}
		base.Result = nearestPlayer != null;
	}
}
