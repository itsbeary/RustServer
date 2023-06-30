using System;
using ProtoBuf;

// Token: 0x02000376 RID: 886
public class HungerAboveAIEvent : BaseAIEvent
{
	// Token: 0x170002A0 RID: 672
	// (get) Token: 0x0600201B RID: 8219 RVA: 0x000D72EE File Offset: 0x000D54EE
	// (set) Token: 0x0600201C RID: 8220 RVA: 0x000D72F6 File Offset: 0x000D54F6
	public float Value { get; private set; }

	// Token: 0x0600201D RID: 8221 RVA: 0x000D72FF File Offset: 0x000D54FF
	public HungerAboveAIEvent()
		: base(AIEventType.HungerAbove)
	{
		base.Rate = BaseAIEvent.ExecuteRate.Slow;
	}

	// Token: 0x0600201E RID: 8222 RVA: 0x000D7310 File Offset: 0x000D5510
	public override void Init(AIEventData data, global::BaseEntity owner)
	{
		base.Init(data, owner);
		HungerAboveAIEventData hungerAboveData = data.hungerAboveData;
		this.Value = hungerAboveData.value;
	}

	// Token: 0x0600201F RID: 8223 RVA: 0x000D7338 File Offset: 0x000D5538
	public override AIEventData ToProto()
	{
		AIEventData aieventData = base.ToProto();
		aieventData.hungerAboveData = new HungerAboveAIEventData();
		aieventData.hungerAboveData.value = this.Value;
		return aieventData;
	}

	// Token: 0x06002020 RID: 8224 RVA: 0x000D735C File Offset: 0x000D555C
	public override void Execute(AIMemory memory, AIBrainSenses senses, StateStatus stateStatus)
	{
		IAIHungerAbove iaihungerAbove = base.Owner as IAIHungerAbove;
		if (iaihungerAbove == null)
		{
			base.Result = false;
			return;
		}
		bool flag = iaihungerAbove.IsHungerAbove(this.Value);
		if (base.Inverted)
		{
			base.Result = !flag;
			return;
		}
		base.Result = flag;
	}
}
