using System;
using ProtoBuf;
using UnityEngine;

// Token: 0x0200037A RID: 890
public class InRangeOfHomeAIEvent : BaseAIEvent
{
	// Token: 0x170002A2 RID: 674
	// (get) Token: 0x0600202A RID: 8234 RVA: 0x000D74F5 File Offset: 0x000D56F5
	// (set) Token: 0x0600202B RID: 8235 RVA: 0x000D74FD File Offset: 0x000D56FD
	public float Range { get; set; }

	// Token: 0x0600202C RID: 8236 RVA: 0x000D7506 File Offset: 0x000D5706
	public InRangeOfHomeAIEvent()
		: base(AIEventType.InRangeOfHome)
	{
		base.Rate = BaseAIEvent.ExecuteRate.Fast;
	}

	// Token: 0x0600202D RID: 8237 RVA: 0x000D7518 File Offset: 0x000D5718
	public override void Init(AIEventData data, global::BaseEntity owner)
	{
		base.Init(data, owner);
		InRangeOfHomeAIEventData inRangeOfHomeData = data.inRangeOfHomeData;
		this.Range = inRangeOfHomeData.range;
	}

	// Token: 0x0600202E RID: 8238 RVA: 0x000D7540 File Offset: 0x000D5740
	public override AIEventData ToProto()
	{
		AIEventData aieventData = base.ToProto();
		aieventData.inRangeOfHomeData = new InRangeOfHomeAIEventData();
		aieventData.inRangeOfHomeData.range = this.Range;
		return aieventData;
	}

	// Token: 0x0600202F RID: 8239 RVA: 0x000D7564 File Offset: 0x000D5764
	public override void Execute(AIMemory memory, AIBrainSenses senses, StateStatus stateStatus)
	{
		Vector3 vector = memory.Position.Get(4);
		base.Result = false;
		bool flag = Vector3Ex.Distance2D(base.Owner.transform.position, vector) <= this.Range;
		base.Result = (base.Inverted ? (!flag) : flag);
	}
}
