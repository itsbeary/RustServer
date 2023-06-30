using System;
using ProtoBuf;
using UnityEngine;

// Token: 0x02000373 RID: 883
public class ChanceAIEvent : BaseAIEvent
{
	// Token: 0x1700029E RID: 670
	// (get) Token: 0x0600200E RID: 8206 RVA: 0x000D715C File Offset: 0x000D535C
	// (set) Token: 0x0600200F RID: 8207 RVA: 0x000D7164 File Offset: 0x000D5364
	public float Chance { get; set; }

	// Token: 0x06002010 RID: 8208 RVA: 0x000D716D File Offset: 0x000D536D
	public ChanceAIEvent()
		: base(AIEventType.Chance)
	{
		base.Rate = BaseAIEvent.ExecuteRate.Fast;
	}

	// Token: 0x06002011 RID: 8209 RVA: 0x000D717E File Offset: 0x000D537E
	public override void Init(AIEventData data, global::BaseEntity owner)
	{
		base.Init(data, owner);
		this.Chance = data.chanceData.value;
	}

	// Token: 0x06002012 RID: 8210 RVA: 0x000D7199 File Offset: 0x000D5399
	public override AIEventData ToProto()
	{
		AIEventData aieventData = base.ToProto();
		aieventData.chanceData = new ChanceAIEventData();
		aieventData.chanceData.value = this.Chance;
		return aieventData;
	}

	// Token: 0x06002013 RID: 8211 RVA: 0x000D71C0 File Offset: 0x000D53C0
	public override void Execute(AIMemory memory, AIBrainSenses senses, StateStatus stateStatus)
	{
		base.Result = base.Inverted;
		bool flag = UnityEngine.Random.Range(0f, 1f) <= this.Chance;
		if (base.Inverted)
		{
			base.Result = !flag;
			return;
		}
		base.Result = flag;
	}
}
