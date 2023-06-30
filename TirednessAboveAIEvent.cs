using System;
using ProtoBuf;

// Token: 0x0200038D RID: 909
public class TirednessAboveAIEvent : BaseAIEvent
{
	// Token: 0x170002AA RID: 682
	// (get) Token: 0x0600206D RID: 8301 RVA: 0x000D7F5D File Offset: 0x000D615D
	// (set) Token: 0x0600206E RID: 8302 RVA: 0x000D7F65 File Offset: 0x000D6165
	public float Value { get; private set; }

	// Token: 0x0600206F RID: 8303 RVA: 0x000D7F6E File Offset: 0x000D616E
	public TirednessAboveAIEvent()
		: base(AIEventType.TirednessAbove)
	{
		base.Rate = BaseAIEvent.ExecuteRate.Slow;
	}

	// Token: 0x06002070 RID: 8304 RVA: 0x000D7F80 File Offset: 0x000D6180
	public override void Init(AIEventData data, global::BaseEntity owner)
	{
		base.Init(data, owner);
		TirednessAboveAIEventData tirednessAboveData = data.tirednessAboveData;
		this.Value = tirednessAboveData.value;
	}

	// Token: 0x06002071 RID: 8305 RVA: 0x000D7FA8 File Offset: 0x000D61A8
	public override AIEventData ToProto()
	{
		AIEventData aieventData = base.ToProto();
		aieventData.tirednessAboveData = new TirednessAboveAIEventData();
		aieventData.tirednessAboveData.value = this.Value;
		return aieventData;
	}

	// Token: 0x06002072 RID: 8306 RVA: 0x000D7FCC File Offset: 0x000D61CC
	public override void Execute(AIMemory memory, AIBrainSenses senses, StateStatus stateStatus)
	{
		base.Result = base.Inverted;
		IAITirednessAbove iaitirednessAbove = base.Owner as IAITirednessAbove;
		if (iaitirednessAbove == null)
		{
			return;
		}
		bool flag = iaitirednessAbove.IsTirednessAbove(this.Value);
		if (base.Inverted)
		{
			base.Result = !flag;
			return;
		}
		base.Result = flag;
	}
}
