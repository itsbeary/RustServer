using System;
using ProtoBuf;
using UnityEngine;

// Token: 0x02000379 RID: 889
public class InRangeAIEvent : BaseAIEvent
{
	// Token: 0x170002A1 RID: 673
	// (get) Token: 0x06002024 RID: 8228 RVA: 0x000D7417 File Offset: 0x000D5617
	// (set) Token: 0x06002025 RID: 8229 RVA: 0x000D741F File Offset: 0x000D561F
	public float Range { get; set; }

	// Token: 0x06002026 RID: 8230 RVA: 0x000D7428 File Offset: 0x000D5628
	public InRangeAIEvent()
		: base(AIEventType.InRange)
	{
		base.Rate = BaseAIEvent.ExecuteRate.Fast;
	}

	// Token: 0x06002027 RID: 8231 RVA: 0x000D7438 File Offset: 0x000D5638
	public override void Init(AIEventData data, global::BaseEntity owner)
	{
		base.Init(data, owner);
		InRangeAIEventData inRangeData = data.inRangeData;
		this.Range = inRangeData.range;
	}

	// Token: 0x06002028 RID: 8232 RVA: 0x000D7460 File Offset: 0x000D5660
	public override AIEventData ToProto()
	{
		AIEventData aieventData = base.ToProto();
		aieventData.inRangeData = new InRangeAIEventData();
		aieventData.inRangeData.range = this.Range;
		return aieventData;
	}

	// Token: 0x06002029 RID: 8233 RVA: 0x000D7484 File Offset: 0x000D5684
	public override void Execute(AIMemory memory, AIBrainSenses senses, StateStatus stateStatus)
	{
		global::BaseEntity baseEntity = memory.Entity.Get(base.InputEntityMemorySlot);
		base.Result = false;
		if (baseEntity == null)
		{
			return;
		}
		bool flag = Vector3Ex.Distance2D(base.Owner.transform.position, baseEntity.transform.position) <= this.Range;
		base.Result = (base.Inverted ? (!flag) : flag);
	}
}
