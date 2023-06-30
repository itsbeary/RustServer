using System;
using ProtoBuf;

// Token: 0x02000374 RID: 884
public class HealthBelowAIEvent : BaseAIEvent
{
	// Token: 0x1700029F RID: 671
	// (get) Token: 0x06002014 RID: 8212 RVA: 0x000D720E File Offset: 0x000D540E
	// (set) Token: 0x06002015 RID: 8213 RVA: 0x000D7216 File Offset: 0x000D5416
	public float HealthFraction { get; set; }

	// Token: 0x06002016 RID: 8214 RVA: 0x000D721F File Offset: 0x000D541F
	public HealthBelowAIEvent()
		: base(AIEventType.HealthBelow)
	{
		base.Rate = BaseAIEvent.ExecuteRate.Fast;
	}

	// Token: 0x06002017 RID: 8215 RVA: 0x000D7230 File Offset: 0x000D5430
	public override void Init(AIEventData data, global::BaseEntity owner)
	{
		base.Init(data, owner);
		HealthBelowAIEventData healthBelowData = data.healthBelowData;
		this.HealthFraction = healthBelowData.healthFraction;
	}

	// Token: 0x06002018 RID: 8216 RVA: 0x000D7258 File Offset: 0x000D5458
	public override AIEventData ToProto()
	{
		AIEventData aieventData = base.ToProto();
		aieventData.healthBelowData = new HealthBelowAIEventData();
		aieventData.healthBelowData.healthFraction = this.HealthFraction;
		return aieventData;
	}

	// Token: 0x06002019 RID: 8217 RVA: 0x000D727C File Offset: 0x000D547C
	public override void Execute(AIMemory memory, AIBrainSenses senses, StateStatus stateStatus)
	{
		base.Result = base.Inverted;
		this.combatEntity = memory.Entity.Get(base.InputEntityMemorySlot) as BaseCombatEntity;
		if (this.combatEntity == null)
		{
			return;
		}
		bool flag = this.combatEntity.healthFraction < this.HealthFraction;
		if (base.Inverted)
		{
			base.Result = !flag;
			return;
		}
		base.Result = flag;
	}

	// Token: 0x04001959 RID: 6489
	private BaseCombatEntity combatEntity;
}
