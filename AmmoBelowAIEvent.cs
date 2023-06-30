using System;
using ProtoBuf;

// Token: 0x0200036D RID: 877
public class AmmoBelowAIEvent : BaseAIEvent
{
	// Token: 0x17000290 RID: 656
	// (get) Token: 0x06001FDD RID: 8157 RVA: 0x000D6A60 File Offset: 0x000D4C60
	// (set) Token: 0x06001FDE RID: 8158 RVA: 0x000D6A68 File Offset: 0x000D4C68
	public float Value { get; private set; }

	// Token: 0x06001FDF RID: 8159 RVA: 0x000D6A71 File Offset: 0x000D4C71
	public AmmoBelowAIEvent()
		: base(AIEventType.AmmoBelow)
	{
		base.Rate = BaseAIEvent.ExecuteRate.Normal;
	}

	// Token: 0x06001FE0 RID: 8160 RVA: 0x000D6A84 File Offset: 0x000D4C84
	public override void Init(AIEventData data, global::BaseEntity owner)
	{
		base.Init(data, owner);
		AmmoBelowAIEventData ammoBelowData = data.ammoBelowData;
		this.Value = ammoBelowData.value;
	}

	// Token: 0x06001FE1 RID: 8161 RVA: 0x000D6AAC File Offset: 0x000D4CAC
	public override AIEventData ToProto()
	{
		AIEventData aieventData = base.ToProto();
		aieventData.ammoBelowData = new AmmoBelowAIEventData();
		aieventData.ammoBelowData.value = this.Value;
		return aieventData;
	}

	// Token: 0x06001FE2 RID: 8162 RVA: 0x000D6AD0 File Offset: 0x000D4CD0
	public override void Execute(AIMemory memory, AIBrainSenses senses, StateStatus stateStatus)
	{
		base.Result = base.Inverted;
		IAIAttack iaiattack = base.Owner as IAIAttack;
		if (iaiattack == null)
		{
			return;
		}
		bool flag = iaiattack.GetAmmoFraction() < this.Value;
		if (base.Inverted)
		{
			base.Result = !flag;
			return;
		}
		base.Result = flag;
	}
}
